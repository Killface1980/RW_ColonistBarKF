using static ColonistBarKF.Position;

namespace ColonistBarKF
{
    using UnityEngine;

    public class SettingsColonistBar
    {
        public float BaseSizeFloat = 48f;

        public float BaseSpacingHorizontal = 24f;

        public float BaseSpacingVertical = 32f;

        public Position.Alignment ColBarPos = Position.Alignment.Top;

        public Position.Alignment ColBarPsiIconPos = Position.Alignment.Left;

        public float DoubleClickTime = 0.5f;

        public Color FemaleColor = new Color(1f, 0.64f, 0.8f, 1f);

        public float FixedIconScaleFloat = 1f;

        public float HorizontalOffset = 0f;

        public float IconOffsetX = 1f;

        public float IconOffsetY = 1f;

        public string IconSet = "default";

        public bool IconsHorizontal;

        public int IconsInColumn = 3;

        public bool IconsScreenScale = true;

        public float LimitApparelHealthLess = 0.5f;

        public float LimitBleedMult = 3f;

        public float LimitDiseaseLess = 1f;

        public float LimitEfficiencyLess = 0.33f;

        // public float LimitMoodLess = 0.25f;
        public float LimitFoodLess = 0.25f;

        public float LimitRestLess = 0.25f;

        public Color MaleColor = new Color(0.52f, 0.75f, 0.92f, 1f);

        public float MarginBottomHor = 21f;

        public float MarginBottomVerLeft = 120f;

        public float MarginBottomVerRight = 120f;

        public float MarginLeftHorBottom = 160f;

        public float MarginLeftHorTop = 160f;

        public float MarginLeftVer = 21f;

        public float MarginRightHorBottom = 160f;

        public float MarginRightHorTop = 160f;

        public float MarginRightVer = 21f;

        public float MarginTopHor = 21f;

        public float MarginTopVerLeft = 120f;

        public float MarginTopVerRight = 120f;

        public float MaxColonistBarHeight = Screen.height - 240f;

        public float MaxColonistBarWidth = Screen.width - 320f;

        public int MaxRowsCustom = 3;

        public Position.Alignment MoodBarPos = Position.Alignment.Right;

        public float moodRectAlpha = 0.66f;

        public float moodRectScale = 0.3f;

        public float PawnTextureCameraHorizontalOffset = 0f;

        public float PawnTextureCameraVerticalOffset = 0.3f;

        public float PawnTextureCameraZoom = 1.28205f;

        public bool ShowAggressive = true;

        public bool ShowApparelHealth = true;

        public bool ShowBedroom = true;

        public bool ShowBloodloss = true;

        public bool ShowCrowded = true;

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

        public bool ShowSad = true;

        public bool ShowTired = true;

        public bool ShowTooCold = true;

        public bool ShowTooHot = true;

        public bool ShowToxicity = true;

        public bool ShowUnarmed = true;

        public SortByWhat SortBy;

        public bool UseCustomDoubleClickTime = false;

        public bool UseCustomIconSize = false;

        public bool UseCustomMarginBottom = false;

        public bool UseCustomMarginLeft = false;

        public bool UseCustomMarginRight = false;

        // public static bool useCustomScale = false;
        public bool UseCustomMarginTopHor = false;

        public bool UseCustomPawnTextureCameraOffsets = false;

        public bool UseCustomRowCount = false;

        public bool UseFixedIconScale = false;

        public bool UseGender = true;

        public bool UseMoodBarBG;

        public bool UseMoodColors = true;

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
    }
}