using UnityEngine;

namespace RW_ColonistBarKF
{
    public class Settings 
    {
        public static bool firstload = false;
        public static bool reloadsettings = true;

  //      public static bool useCustomScale = false;
        public static bool useCustomMarginTop = false;
        public static bool useCustomMarginHorizontal = false;
        public static bool useCustomBaseSpacingHorizontal = false;
        public static bool useCustomBaseSpacingVertical = false;
        public static bool useCustomIconSize = false;
        public static bool useCustomPawnTextureCameraVerticalOffset = false;
        public static bool useCustomPawnTextureCameraZoom = false;
        public static bool useCustomMaxColonistBarWidth = false;
        public static bool useCustomMaxColonistBarHeight = false;
        public static bool useCustomDoubleClickTime = false;
        public static bool useGender = true;
        public static bool useVerticalAlignment = false;
        public static bool useRightAlignment = true;
        public static bool useVerticalOffset;
        public static bool useHorizontalOffset;


        public static float MarginTop = 21f;
        public static float MarginHorizontal = 21f;
        public static float BaseSpacingHorizontal = 24f;
        public static float BaseSpacingVertical = 32f;
        public static float BaseSizeFloat = 48f;
        public static float BaseIconSize = 20f;
        public static float PawnTextureCameraVerticalOffset = 0.3f;
        public static float PawnTextureCameraZoom = 1.28205f;
        public static float MaxColonistBarWidth = Screen.width - 320f;
        public static float MaxColonistBarHeight = Screen.height - 200f;
        public static float DoubleClickTime = 0.5f;
        public static float VerticalOffset = 0f;
        public static float HorizontalOffset = 0f;

        public static Color FemaleColor = new Color(1f, 0.64f, 0.8f, 1f);
        public static Color MaleColor = new Color(0.52f, 0.75f, 0.92f, 1f);
    }
}
