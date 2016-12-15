using System.Reflection;
using UnityEngine;
using Verse;
using static ColonistBarKF.CBKF;

namespace ColonistBarKF
{
    public class MapComponent_ZoomToMouse : MapComponent
    {
        // backing private fields / properties
        private const BindingFlags all = (BindingFlags)60;
        private readonly FieldInfo curSize_FI = typeof(CameraDriver).GetField("rootSize", all);
        private readonly FieldInfo desSize_FI = typeof(CameraDriver).GetField("desiredSize", all);
        private readonly MethodInfo curPos = typeof(CameraDriver).GetProperty("CurrentRealPosition", all).GetGetMethod(true);

        // tolerance for zoom
        private float tolerance = .1f;

        // helpers
        private Vector3 LastMouseMapPosition = Vector3.zero;
        private Vector3 CurrentMouseMapPosition => UI.MouseMapPosition();
        private Vector3 MouseMapOffset => LastMouseMapPosition - CurrentMouseMapPosition;

        // reflection helpers
        private float CurrentSize => (float)curSize_FI.GetValue(Current.CameraDriver);
        private float DesiredSize => (float)desSize_FI.GetValue(Current.CameraDriver);
        private Vector3 CurrentRealPosition => (Vector3)curPos.Invoke(Current.CameraDriver, null);

        public override void MapComponentUpdate()
        {
            if (CBKF.ColBarSettings.useZoomToMouse)
            {
                // determine zoom action
                float action = CurrentSize - DesiredSize;

                // zoom action has taken place
                if (action > tolerance)
                    Current.CameraDriver.JumpTo(CurrentRealPosition + MouseMapOffset);
                else
                    // update last known location.
                    LastMouseMapPosition = CurrentMouseMapPosition;

                // NOTE: Ideally, we'ld like to do this within the zooming code. I've been unable to get access without causing errors (detours + loads of reflection).
                // the net result is the current simple but slightly wonky behavious. Movement of the map is a bit jittery, and moving the mouse during scroll moves the map directly.
            }
        }

        public MapComponent_ZoomToMouse(Map map)
            : base(map)
        {
        }
    }
}