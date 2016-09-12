using UnityEngine;

namespace ColonistBarKF
{
    public class SettingsColonistBar
    {
        //      public static bool useCustomScale = false;
        public bool UseCustomMarginTopHor = false;
        public bool UseCustomMarginBottomHor = false;
        public bool UseCustomMarginLeftHorTop = false;
        public bool UseCustomMarginLeftHorBottom = false;
        public bool UseCustomMarginRightHorTop = false;
        public bool UseCustomMarginRightHorBottom = false;

        public bool UseCustomMarginTopVerLeft = false;
        public bool UseCustomMarginTopVerRight = false;
        public bool UseCustomMarginLeftVer = false;
        public bool UseCustomMarginRightVer = false;
        public bool UseCustomMarginBottomVerLeft = false;
        public bool UseCustomMarginBottomVerRight = false;

        public bool UseCustomBaseSpacingHorizontal = false;
        public bool UseCustomBaseSpacingVertical = false;
        public bool UseCustomIconSize = false;
        public bool UseFixedIconScale = false;
        public bool UseCustomPawnTextureCameraHorizontalOffset = false;
        public bool UseCustomPawnTextureCameraVerticalOffset = false;
        public bool UseCustomPawnTextureCameraZoom = false;
        public bool UseCustomDoubleClickTime = false;
        public bool UseGender = true;
        public bool UseVerticalAlignment = false;
        public bool UseRightAlignment = true;
        public bool UseBottomAlignment;

        public bool UseMoodColors = true;
        public bool UseWeaponIcons = true;

        public float MarginTopHor = 21f;
        public float MarginBottomHor = 21f;
        public float MarginLeftHorTop = 160f;
        public float MarginRightHorTop = 160f;
        public float MarginLeftHorBottom = 160f;
        public float MarginRightHorBottom = 160f;

        public float MarginTopVerLeft = 120f;
        public float MarginBottomVerLeft = 120f;
        public float MarginTopVerRight = 120f;
        public float MarginBottomVerRight = 120f;
        public float MarginLeftVer = 21f;
        public float MarginRightVer = 21f;

        public float BaseSpacingHorizontal = 24f;
        public float BaseSpacingVertical = 32f;
        public float BaseSizeFloat = 48f;
        public float BaseIconSize = 20f;
        public float PawnTextureCameraHorizontalOffset = 0f;
        public float PawnTextureCameraVerticalOffset = 0.3f;
        public float PawnTextureCameraZoom = 1.28205f;
        public float MaxColonistBarWidth = Screen.width - 320f;
        public float MaxColonistBarHeight = Screen.height - 240f;
        public float DoubleClickTime = 0.5f;
        public float VerticalOffset = 0f;
        public float HorizontalOffset = 0f;


        public Color FemaleColor = new Color(1f, 0.64f, 0.8f, 1f);
        public Color MaleColor = new Color(0.52f, 0.75f, 0.92f, 1f);

        public bool useZoomToMouse = false;
        public float moodRectScale = 0.3f;

        public int MaxRows;

        public SortByWhat SortBy;

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

        public int IconAlignment = 0;
        public bool UsePsi = true;

        public float IconSize = 1f;
        public float IconSizeMult = 1f;
        public float IconDistanceX = 1f;
        public float IconDistanceY = 1f;
        public float IconOffsetX = 1f;
        public float IconOffsetY = 1f;

        public int IconsInColumn = 4;
        public bool IconsHorizontal;
        public bool IconsScreenScale = true;
        public string IconSet = "default";

        public bool ShowTargetPoint = true;
        public bool ShowAggressive = true;
        public bool ShowDazed = true;
        public bool ShowLeave = true;
        public bool ShowDraft = true;
        public bool ShowIdle = true;
        public bool ShowUnarmed = true;
        public bool ShowHungry = true;
        public bool ShowSad = true;
        public bool ShowTired = true;
        public bool ShowDisease = true;
        public bool ShowEffectiveness = true;
        public bool ShowBloodloss = true;
        public bool ShowHot = true;
        public bool ShowCold = true;
        public bool ShowNaked = true;
        public bool ShowDrunk = true;
        public bool ShowApparelHealth = true;
        public bool ShowPacific = true;
        public bool ShowProsthophile = true;
        public bool ShowProsthophobe = true;
        public bool ShowNightOwl = true;
        public bool ShowGreedy = true;
        public bool ShowJealous = true;
        public bool ShowLovers = true;
        public bool ShowDeadColonists = true;
        public bool ShowLeftUnburied = true;
        public bool ShowRoomStatus = true;
        public bool ShowPain = true;
        public bool ShowBedroom = true;
        public bool ShowHealth = true;
        public bool ShowPyromaniac = true;

   //     public float LimitMoodLess = 0.25f;
        public float LimitFoodLess = 0.25f;
        public float LimitRestLess = 0.25f;
        public float LimitEfficiencyLess = 0.33f;
        public float LimitDiseaseLess = 1f;
        public float LimitBleedMult = 3f;
        public float LimitApparelHealthLess = 0.5f;
        public float LimitTempComfortOffset;
        public float IconOpacity = 0.7f;
        public float IconOpacityCritical = 0.6f;
        public bool UseColoredTarget = true;

    }

}
