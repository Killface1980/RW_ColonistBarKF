using UnityEngine;

namespace RW_ColonistBarKF
{
    internal class Settings
    {
        public static bool firstload = false;
        public static bool reloadsettings = true;

        //      public static bool useCustomScale = false;
        public static bool useCustomMarginTopHor = false;
        public static bool useCustomMarginLeftHor = false;
        public static bool useCustomMarginRightHor = false;

        public static bool useCustomMarginTopVer = false;
        public static bool useCustomMarginLeftRightVer = false;
        public static bool useCustomMarginBottomVer = false;


        public static bool useCustomBaseSpacingHorizontal = false;
        public static bool useCustomBaseSpacingVertical = false;
        public static bool useCustomIconSize = false;
        public static bool useFixedIconScale = false;
        public static bool useCustomPawnTextureCameraVerticalOffset = false;
        public static bool useCustomPawnTextureCameraZoom = false;
        public static bool useCustomDoubleClickTime = false;
        public static bool useGender = true;
        public static bool useVerticalAlignment = false;
        public static bool useRightAlignment = true;
        public static bool useBottomAlignment;


        public static float MarginTopBottomHor = 21f;
        public static float MarginLeftHor = 160f;
        public static float MarginRightHor = 160f;

        public static float MarginTopVer = 120f;
        public static float MarginLeftRightVer = 21f;
        public static float MarginBottomVer = 120f;



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
    }
}
