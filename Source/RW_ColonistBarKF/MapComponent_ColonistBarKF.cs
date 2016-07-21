using System.Linq;
using UnityEngine;
using Verse;

namespace RW_ColonistBarKF
{
    public class MapComponent_ColonistBarKF : MapComponent
    {
        public float BaseSizeFloat = 48f;

        public static MapComponent_ColonistBarKF Settings = new MapComponent_ColonistBarKF();

        public static MapComponent_ColonistBarKF Get
        {
            get
            {
                MapComponent_ColonistBarKF getComponent = Find.Map.components.OfType<MapComponent_ColonistBarKF>().FirstOrDefault();
                if (getComponent == null)
                {
                    getComponent = new MapComponent_ColonistBarKF();
                    Find.Map.components.Add(getComponent);
                }

                return getComponent;
            }
        }

        public override void ExposeData()
        {
            Scribe_Values.LookValue(ref BaseSizeFloat, "BaseSizeFloat", 48f, false);
        }
    }
}
