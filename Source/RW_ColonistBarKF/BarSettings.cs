using UnityEngine;

namespace ColonistBarKF
{
    public class BarSettings
    {
        public bool Firstload = false;
        public bool Reloadsettings = true;

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
    }
 
}
