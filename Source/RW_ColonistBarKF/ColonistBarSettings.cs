using UnityEngine;
using Verse;

namespace RW_ColonistBarKF
{
    public class ColonistBarSettings : IExposable
    {
        public float BaseSizeFloat = 48f;

        public void ExposeData()
        {
            Scribe_Values.LookValue(ref BaseSizeFloat, "BaseSizeFloat");
        }
    }
}
