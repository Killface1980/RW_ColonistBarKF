﻿using UnityEngine;

namespace RW_ColonistBarKF
{
    public class Settings 
    {
        public static bool firstload = false;
        public static bool reloadsettings = true;

  //      public static bool useCustomScale = false;
        public static bool useCustomMarginTop = false;
        public static bool useCustomBaseSpacingHorizontal = false;
        public static bool useCustomBaseSpacingVertical = false;
        public static bool useCustomIconSize = false;
        public static bool useCustomPawnTextureCameraVerticalOffset = false;
        public static bool useCustomPawnTextureCameraZoom = false;
        public static bool useCustomMaxColonistBarWidth = false;
        public static bool useCustomDoubleClickTime = false;
        public static bool useGender = true;


        public static float MarginTop = 21f;
        public static float BaseSpacingHorizontal = 24f;
        public static float BaseSpacingVertical = 32f;
        public static float BaseSizeFloat = 48f;
        public static float BaseIconSize = 20f;
        public static float PawnTextureCameraVerticalOffset = 0.3f;
        public static float PawnTextureCameraZoom = 1.28205f;
        public static float MaxColonistBarWidth = Screen.width - 320f;
        public static float DoubleClickTime = 0.5f;




    }
}