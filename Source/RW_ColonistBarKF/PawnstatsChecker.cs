namespace ColonistBarKF
{
    using System.Collections.Generic;
    using System.Linq;

    using RimWorld;

    using Verse;

    public static class PawnstatsChecker
    {
        #region Fields

        private static List<PawnStats> _pawnCache = new List<PawnStats>();

        #endregion Fields

        #region Methods

        public static PawnStats GetPawnStats(this Pawn pawn)
        {
            foreach (PawnStats c in _pawnCache)
            {
                if (c.pawn == pawn)
                {
                    return c;
                }
            }

            PawnStats n = new PawnStats { pawn = pawn };
            _pawnCache.Add(n);

            if (pawn.IsColonist)
            {
                PSI.PSI.UpdateColonistStats(n);
            }

            n.SpawnedAt = Find.TickManager.TicksGame;
            return n;

            // if (!PawnApparelStatCaches.ContainsKey(pawn))
            // {
            // PawnApparelStatCaches.Add(pawn, new StatCache(pawn));
            // }
            // return PawnApparelStatCaches[pawn];
        }

        // public override void ExposeData()
        // {
        // base.ExposeData();
        // // Scribe_Collections.Look(ref _pawnCache, "Pawns", LookMode.Deep);
        // if (_pawnCache == null)
        // {
        // _pawnCache = new List<PawnStats>();
        // }
        // }
        public static void ResetList()
        {
            _pawnCache = new List<PawnStats>();
        }

        #endregion Methods

        // public override void MapComponentTick()
        // {
        // base.MapComponentTick();
        // if (!Settings.PsiSettings.UsePsi)
        // {
        // foreach (PawnStats pawnStats in _pawnCache)
        // {
        // if (Find.TickManager.TicksGame - pawnStats.LastStatUpdate > 600)
        // {
        // PSI.PSI.CheckStats(pawnStats);
        // }
        // }
        // }
        // }
    }
}