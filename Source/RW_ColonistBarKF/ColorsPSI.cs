using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using static ColonistBarKF.CBKF;

namespace ColonistBarKF
{
    [StaticConstructorOnStartup]
    public static class ColorsPSI
    {
        public static Color Color25To21 = new Color(0.8f, 0f, 0f);

        public static Color Color20To16 = new Color(0.9f, 0.45f, 0f);

        public static Color Color15To11 = new Color(0.95f, 0.95f, 0f);

        public static Color Color10To06 = new Color(0.95f, 0.95f, 0.66f);

        public static Color Color05AndLess = new Color(0.9f, 0.9f, 0.9f);

        public static Color ColorMoodBoost = new Color(0f, 0.8f, 0f);

        public static Color ColorNeutralStatus = Color05AndLess; 

        public static Color ColorNeutralStatusSolid = new Color(ColorNeutralStatus.r, ColorNeutralStatus.g, ColorNeutralStatus.b);

        public static Color ColorNeutralStatusFade = new Color(ColorNeutralStatus.r, ColorNeutralStatus.g, ColorNeutralStatus.b / 4);

        public static Color ColorHealthBarGreen = new Color(0f, 0.8f, 0f * 0.5f);


        public static Color ColorRedAlert = new Color(0.8f, 0, 0);

        public static Color ColorOrangeAlert = Color20To16;

        public static Color ColorYellowAlert = Color15To11;

    }
}
