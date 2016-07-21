using System.Linq;
using UnityEngine;
using Verse;

namespace RW_ColonistBarKF
{
    public class MapComponent_ColonistBarKF : MapComponent
    {
        public static float BaseSizeFloat = 48f;

        public override void ExposeData()
        {
            Scribe_Values.LookValue(ref BaseSizeFloat, "BaseSizeFloat", 48f, false);
        }
    }
}
