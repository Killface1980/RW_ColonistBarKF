using Verse;

namespace ColonistBarKF
{
    using JetBrains.Annotations;

    public struct EntryKF
    {
        [CanBeNull]
        public Pawn pawn;

        public Map map;

        public int group;

        public int groupCount;

        public EntryKF(Pawn pawn, Map map, int group, int groupCount)
        {
            this.pawn = pawn;
            this.map = map;
            this.group = group;
            this.groupCount = groupCount;
        }
    }
}
