using JetBrains.Annotations;
using RimWorld;

namespace ColonistBarKF
{
    [DefOf]
    public static class ThoughtDefOf
    {
        // ThoughtDefs which are not included in RW ThoughtDefOf
        [NotNull]
        public static ThoughtDef ApparelDamaged;

        [NotNull]
        public static ThoughtDef NeedOutdoors;

        [NotNull]
        public static ThoughtDef ColonistLeftUnburied;

        [NotNull]
        public static ThoughtDef DeadMansApparel;

        [NotNull]
        public static ThoughtDef Greedy;

        [NotNull]
        public static ThoughtDef HumanLeatherApparelSad;

        [NotNull]
        public static ThoughtDef Jealous;

        [NotNull]
        public static ThoughtDef Naked;

        [NotNull]
        public static ThoughtDef NightOwlDuringTheDay;

        [NotNull]
        public static ThoughtDef ProsthophileNoProsthetic;

        [NotNull]
        public static ThoughtDef ProsthophobeUnhappy;

        [NotNull]
        public static ThoughtDef SharedBed;

        [NotNull]
        public static ThoughtDef Sick;

        [NotNull]
        public static ThoughtDef WantToSleepWithSpouseOrLover;
    }
}