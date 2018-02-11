using JetBrains.Annotations;
using Verse;

namespace ColonistBarKF
{
    public struct EntryKf
    {
        [CanBeNull]
        public Pawn Pawn;

        [CanBeNull]
        public Map Map;

        public int Group;

        public int GroupCount;

        public EntryKf([CanBeNull] Pawn pawn, [CanBeNull] Map map, int group, int groupCount)
        {
            this.Pawn = pawn;
            this.Map = map;
            this.Group = group;
            this.GroupCount = groupCount;
        }
    }
}