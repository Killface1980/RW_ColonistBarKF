using UnityEngine;

namespace RW_ColonistBarKF
{
    internal class Settings
    {
        public static bool Firstload = false;
        public static bool Reloadsettings = true;

        //      public static bool useCustomScale = false;
        public static bool UseCustomMarginTopHor = false;
        public static bool UseCustomMarginBottomHor = false;
        public static bool UseCustomMarginLeftHorTop = false;
        public static bool UseCustomMarginLeftHorBottom = false;
        public static bool UseCustomMarginRightHorTop = false;
        public static bool UseCustomMarginRightHorBottom = false;

        public static bool UseCustomMarginTopVerLeft = false;
        public static bool UseCustomMarginTopVerRight = false;
        public static bool UseCustomMarginLeftVer = false;
        public static bool UseCustomMarginRightVer = false;
        public static bool UseCustomMarginBottomVerLeft = false;
        public static bool UseCustomMarginBottomVerRight = false;

        public static bool UseCustomBaseSpacingHorizontal = false;
        public static bool UseCustomBaseSpacingVertical = false;
        public static bool UseCustomIconSize = false;
        public static bool UseFixedIconScale = false;
        public static bool UseCustomPawnTextureCameraVerticalOffset = false;
        public static bool UseCustomPawnTextureCameraZoom = false;
        public static bool UseCustomDoubleClickTime = false;
        public static bool UseGender = true;
        public static bool UseVerticalAlignment = false;
        public static bool UseRightAlignment = true;
        public static bool UseBottomAlignment;

        public static bool UseMoodColors;


        public static float MarginTopHor = 21f;
        public static float MarginBottomHor = 21f;
        public static float MarginLeftHorTop = 160f;
        public static float MarginRightHorTop = 160f;
        public static float MarginLeftHorBottom = 160f;
        public static float MarginRightHorBottom = 160f;

        public static float MarginTopVerLeft = 120f;
        public static float MarginBottomVerLeft = 120f;
        public static float MarginTopVerRight = 120f;
        public static float MarginBottomVerRight = 120f;
        public static float MarginLeftVer = 21f;
        public static float MarginRightVer = 21f;



        public static float BaseSpacingHorizontal = 24f;
        public static float BaseSpacingVertical = 32f;
        public static float BaseSizeFloat = 48f;
        public static float BaseIconSize = 20f;
        public static float PawnTextureCameraVerticalOffset = 0.3f;
        public static float PawnTextureCameraZoom = 1.28205f;
        public static float MaxColonistBarWidth = Screen.width - 320f;
        public static float MaxColonistBarHeight = Screen.height - 240f;
        public static float DoubleClickTime = 0.5f;
        public static float VerticalOffset = 0f;
        public static float HorizontalOffset = 0f;

        public static Color FemaleColor = new Color(1f, 0.64f, 0.8f, 1f);
        public static Color MaleColor = new Color(0.52f, 0.75f, 0.92f, 1f);

        public static int MaxRows;
    }
}
