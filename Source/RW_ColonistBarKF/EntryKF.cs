namespace ColonistBarKF
{
    using Verse;

    public struct EntryKF
    {
        [CanBeNull]
        public Pawn pawn;

        [CanBeNull]
        public Map map;

        public int group;

        public int groupCount;

        public EntryKF([CanBeNull] Pawn pawn, [CanBeNull] Map map, int group, int groupCount)
        {
            this.pawn = pawn;
            this.map = map;
            this.group = group;
            this.groupCount = groupCount;
        }
    }
}
