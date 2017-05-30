using static ColonistBarKF.Position;

namespace ColonistBarKF
{
    public class SettingsColonistBar
    {
        public float BaseSizeFloat = 48f;

        public float BaseSpacingHorizontal = 24f;

        public float BaseSpacingVertical = 32f;

      //  public Position.Alignment ColBarPos = Position.Alignment.Top;

        public Alignment ColBarPsiIconPos = Alignment.Left;



        public float IconOffsetX = 1f;

        public float IconOffsetY = 1f;

        public string IconSet = "default";

        public bool IconsHorizontal;

        public int IconsInColumn = 3;

        public bool IconsScreenScale = true;

        public float LimitApparelHealthLess = 0.5f;

        public float LimitBleedMult = 3f;

        public float LimitDiseaseLess = 1f;

        // public float LimitMoodLess = 0.25f;



        public float MarginHorizontal = 520f;

        public float MarginTop = 21f;

        public int MaxRowsCustom = 3;

        public Alignment MoodBarPos = Alignment.Right;

        public float moodRectAlpha = 0.66f;

        public float moodRectScale = 0.3f;

        public float PawnTextureCameraHorizontalOffset = 0f;

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

        public bool ShowSad = false;

        public bool ShowTired = true;

        public bool ShowTooCold = true;

        public bool ShowTooHot = true;

        public bool ShowToxicity = true;

        public bool ShowUnarmed = true;

        public SortByWhat SortBy;

        public bool UseCustomIconSize = false;

        // public static bool useCustomScale = false;
        public bool UseCustomMarginTop = false;

        public bool UseCustomPawnTextureCameraOffsets = false;

        public bool UseCustomRowCount = false;

        public bool UseGender = true;

        public bool UseExternalMoodBar = true;

        public bool UsePsi = true;

        public bool UseWeaponIcons = true;

        public bool useZoomToMouse = false;

        public float VerticalOffset = 0f;

        public enum SortByWhat
        {
            vanilla = -1,

            sexage,

            health,

            mood,

            weapons,

            medic,

            byName
        }

        public bool UseNewMood = true;
    
    }
}