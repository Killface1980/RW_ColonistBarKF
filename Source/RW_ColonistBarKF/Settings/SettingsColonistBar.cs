namespace ColonistBarKF.Settings
{
    public class SettingsColonistBar
    {
        public float BaseIconSize = 48f;

        public float BaseSpacingHorizontal = 24f;

        public float BaseSpacingVertical = 32f;

        // public Position.Alignment ColBarPos = Position.Alignment.Top;
        public Position.Alignment ColBarPsiIconPos = Position.Alignment.Left;

        public float IconOffsetX = 1f;

        public float IconOffsetY = 1f;

        public bool IconsHorizontal;

        public int IconsInColumn = 3;
        public float LimitBleedMult = 3f;

        // public float LimitMoodLess = 0.25f;
        public float MarginHorizontal = 520f;

        public float MarginVertical = 220f;

        public float MarginTop = 21f;

        public int MaxRowsCustom = 3;

        public Position.Alignment MoodBarPos = Position.Alignment.Right;

        public float moodRectAlpha = 0.66f;

        public float moodRectScale = 0.3f;

        public float PawnTextureCameraHorizontalOffset;

        public float PawnTextureCameraVerticalOffset = 0.3f;

        public float PawnTextureCameraZoom = 1.28205f;

        public bool ShowAggressive = true;

        public bool ShowApparelHealth = true;

        public bool ShowBedroom = true;

        public bool ShowBloodloss = true;

        public bool ShowCabinFever = true;

        public bool ShowDazed = true;

        public bool ShowDeadColonists = true;

        public bool ShowDraft = true;

        public bool ShowDrunk = true;

        public bool ShowEffectiveness = true;

        public bool ShowGreedy = true;

        // public bool ShowMarriage = true;
        public bool ShowHealth = true;

        public bool ShowHungry = true;

        public bool ShowIdle = true;

        public bool ShowJealous = true;

        public bool ShowLeave = true;

        public bool ShowLeftUnburied = true;

        public bool ShowLove = true;

        public bool ShowMedicalAttention = true;

        public bool ShowNaked = true;

        public bool ShowNightOwl = true;

        public bool ShowPacific = true;

        public bool ShowPain = true;

        public bool ShowPanic = true;

        public bool ShowProsthophile = true;

        public bool ShowProsthophobe = true;

        public bool ShowPyromaniac = true;

        public bool ShowSad;

        public bool ShowTired = true;

        public bool ShowTooCold = true;

        public bool ShowTooHot = true;

        public bool ShowToxicity = true;

        public bool ShowUnarmed = true;

        public SortByWhat SortBy;

        public bool UseCustomIconSize;

        // public static bool useCustomScale = false;
        public bool UseCustomMarginTop;

        public bool UseCustomPawnTextureCameraOffsets;

        public bool UseCustomRowCount;

        public bool UseExternalMoodBar = true;

        public bool UseGender = true;

        public bool UseGroupColors = true;

        public bool UseGrouping = true;

        public bool UseNewMood = true;

        public bool UsePsi = true;

        public bool UseWeaponIcons = true;

        public bool useZoomToMouse;

        public bool useFollowMessage = true;

        public float VerticalOffset;

        public enum SortByWhat
        {
            vanilla = -1,

            sexage,

            health,

            bleedRate,

            mood,

            weapons,

            medicTendQuality,

            // medic,
            medicSurgerySuccess,

            byName,

            diplomacy,
            tradePrice
        }
    }
}