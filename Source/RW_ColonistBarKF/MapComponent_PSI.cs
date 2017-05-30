using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ColonistBarKF
{
    class MapComponent_PSI : MapComponent
    {
       
        private static List<PawnStats> _pawnCache = new List<PawnStats>();

        public static List<PawnStats> PawnCache => _pawnCache;

        public PawnStats GetCache(Pawn pawn)
        {
            foreach (PawnStats c in _pawnCache)
            {
                if (c.Pawn == pawn)
                {
                    return c;
                }
            }
            PawnStats n = new PawnStats { Pawn = pawn };
            _pawnCache.Add(n);
            PSI.PSI.UpdateColonistStats(n);
            return n;

            // if (!PawnApparelStatCaches.ContainsKey(pawn))
            // {
            //     PawnApparelStatCaches.Add(pawn, new StatCache(pawn));
            // }
            // return PawnApparelStatCaches[pawn];
        }

        public static MapComponent_PSI Get
        {
            get
            {
                MapComponent_PSI getComponent = Find.VisibleMap.components.OfType<MapComponent_PSI>().FirstOrDefault();
                if (getComponent != null)
                {
                    return getComponent;
                }
                getComponent = new MapComponent_PSI(Find.VisibleMap);
                Find.VisibleMap.components.Add(getComponent);

                return getComponent;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

     //       Scribe_Collections.Look(ref _pawnCache, "Pawns", LookMode.Deep);

            if (_pawnCache == null)
                _pawnCache = new List<PawnStats>();
        }

        public MapComponent_PSI(Map map)
            : base(map)
        {
            this.map = map;
        }

        public static void ResetList()
        {
            _pawnCache = new List<PawnStats>();
        }
    }
}
