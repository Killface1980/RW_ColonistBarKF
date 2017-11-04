namespace ColonistBarKF.Fluffy
{
    using System.Reflection;

    using UnityEngine;

    using Verse;

    public class ZoomToMouse : GameComponent
    {
        // backing private fields / properties
        private const BindingFlags AllFlags = (BindingFlags)60;

        // ReSharper disable once InconsistentNaming
        private readonly MethodInfo _curPos = typeof(CameraDriver).GetProperty("CurrentRealPosition", AllFlags)
            .GetGetMethod(true);

        // ReSharper disable once InconsistentNaming
        private readonly FieldInfo _curSizeFi = typeof(CameraDriver).GetField("rootSize", AllFlags);

        // ReSharper disable once InconsistentNaming
        private readonly FieldInfo _desSizeFi = typeof(CameraDriver).GetField("desiredSize", AllFlags);

        // tolerance for zoom
        private readonly float tolerance = 0.1f;

        // helpers
        // ReSharper disable once InconsistentNaming
        private Vector3 _lastMouseMapPosition = Vector3.zero;

        public ZoomToMouse(Game game)
        {
            // nothing
        }

        public ZoomToMouse()
        {
        }

        private Vector3 CurrentMouseMapPosition => UI.MouseMapPosition();

        private Vector3 CurrentRealPosition => (Vector3)this._curPos.Invoke(Current.CameraDriver, null);

        // reflection helpers
        private float CurrentSize => (float)this._curSizeFi.GetValue(Current.CameraDriver);

        private float DesiredSize => (float)this._desSizeFi.GetValue(Current.CameraDriver);

        private Vector3 MouseMapOffset => this._lastMouseMapPosition - this.CurrentMouseMapPosition;

        public override void GameComponentOnGUI()
        {
            if (Current.ProgramState != ProgramState.Playing)
            {
                return;
            }

            if (!Settings.ColBarSettings.useZoomToMouse)
            {
                return;
            }

            if (FollowMe.CurrentlyFollowing)
            {
                return;
            }

            // determine zoom action
            float action = this.CurrentSize - this.DesiredSize;

            // zoom action has taken place
            if (action > this.tolerance)
            {
                Current.CameraDriver.JumpToVisibleMapLoc(this.CurrentRealPosition + this.MouseMapOffset);
            }
            else
            {
                // update last known location.
                this._lastMouseMapPosition = this.CurrentMouseMapPosition;
            }

            // NOTE: Ideally, we'ld like to do this within the zooming code. I've been unable to get access without causing errors (detours + loads of reflection).
            // the net result is the current simple but slightly wonky behavious. Movement of the map is a bit jittery, and moving the mouse during scroll moves the map directly.
        }
    }
}