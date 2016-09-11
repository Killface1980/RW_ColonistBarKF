
using System;
using UnityEngine;
using Verse;
using Object = UnityEngine.Object;

namespace ColonistBarKF       // Replace with yours.
{       // This code is mostly borrowed from Pawn State Icons mod by Dan Sadler, which has open source and no license I could find, so...
    [StaticConstructorOnStartup]
    public class MapComponentInjector : MonoBehaviour
    {
        private static Type followMe = typeof(MapComponent_FollowMe);
        private static Type zoomToMouse = typeof(MapComponent_ZoomToMouse);


        #region No editing required


        public void FixedUpdate()
        {
            if (Current.ProgramState != ProgramState.MapPlaying)
            {
                return;
            }

            if (Find.Map.components.FindAll(c => c.GetType() == followMe).Count == 0)
            {
                Find.Map.components.Add((MapComponent)Activator.CreateInstance(followMe));

                Log.Message("ColonistBarKF :: Added FollowMe to the map.");
            }
            if (Find.Map.components.FindAll(c => c.GetType() == zoomToMouse).Count == 0)
            {
                Find.Map.components.Add((MapComponent)Activator.CreateInstance(zoomToMouse));

                Log.Message("ColonistBarKF :: Added ZoomToMouse to the map.");
            }
            Destroy(this);
        }

    }
}
#endregion