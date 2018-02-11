using System.Reflection;
using UnityEngine;
using Verse;

namespace ColonistBarKF.Fluffy
{
    public class ZoomToMouse : GameComponent
    {
        // backing private fields / properties
        private const BindingFlags AllFlags = (BindingFlags)60;

        // ReSharper disable once InconsistentNaming
        private readonly MethodInfo _curPos = typeof(CameraDriver).GetProperty("CurrentRealPosition", AllFlags)
                                                                 ?.GetGetMethod(true);

        // ReSharper disable once InconsistentNaming
        private readonly FieldInfo _curSizeFi = typeof(CameraDriver).GetField("rootSize", AllFlags);

        // ReSharper disable once InconsistentNaming
        private readonly FieldInfo _desSizeFi = typeof(CameraDriver).GetField("desiredSize", AllFlags);

        // tolerance for zoom
        private readonly float _tolerance = 0.1f;

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

        private Vector3 CurrentRealPosition => (Vector3)_curPos.Invoke(Current.CameraDriver, null);

        // reflection helpers
        private float CurrentSize => (float)_curSizeFi.GetValue(Current.CameraDriver);

        private float DesiredSize => (float)_desSizeFi.GetValue(Current.CameraDriver);

        private Vector3 MouseMapOffset => _lastMouseMapPosition - CurrentMouseMapPosition;

        public override void GameComponentOnGUI()
        {
            if (Current.ProgramState != ProgramState.Playing)
            {
                return;
            }

            if (!Settings.Settings.BarSettings.useZoomToMouse)
            {
                return;
            }

            if (FollowMe.CurrentlyFollowing)
            {
                return;
            }

            // determine zoom action
            float action = CurrentSize - DesiredSize;

            // zoom action has taken place
            if (action > _tolerance)
            {
                Current.CameraDriver.JumpToVisibleMapLoc(CurrentRealPosition + MouseMapOffset);
            }
            else
            {
                // update last known location.
                _lastMouseMapPosition = CurrentMouseMapPosition;
            }

            // NOTE: Ideally, we'ld like to do this within the zooming code. I've been unable to get access without causing errors (detours + loads of reflection).
            // the net __result is the current simple but slightly wonky behavious. Movement of the map is a bit jittery, and moving the mouse during scroll moves the map directly.
        }
    }
}