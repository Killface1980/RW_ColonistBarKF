namespace Verse
{
    using UnityEngine;

    [StaticConstructorOnStartup]
    internal class ColonistBarTextures
    {
        public static readonly Texture2D BGTex = Command.BGTex;

        public static readonly Texture2D BGTexGrey = ContentFinder<Texture2D>.Get("UI/Widgets/CBKF/DesButBG_grey");

        public static readonly Texture2D BGTexIconPSI =
            SolidColorMaterials.NewSolidColorTexture(new Color(0f, 0f, 0f, 0.8f));

        public static readonly Texture2D BGTexVanilla = ContentFinder<Texture2D>.Get("UI/Widgets/CBKF/DesButBG_vanilla");

        public static readonly Color ColBlue = new Color32(0, 114, 178, 255);

        public static readonly Color ColBlueBG = new Color32(0, 114, 178, 80);

        public static readonly Color ColBlueishGreen = new Color32(0, 158, 115, 255);

        public static readonly Color ColBlueishGreenBg = new Color32(0, 158, 115, 60);

        // public static readonly Color32 ColVermillion = new Color32(179, 55, 0, 255);
        public static readonly Color ColOrange = new Color32(177, 123, 0, 255);

        public static readonly Color ColOrangeBg = new Color32(177, 123, 0, 60);

        public static readonly Color ColReddishPurple = new Color32(204, 121, 167, 255);

        public static readonly Color ColSkyBlue = new Color32(92, 180, 230, 255);

        public static readonly Color ColSkyBlueBg = new Color32(92, 180, 230, 80);

        // Color blind palette
        public static readonly Color ColVermillion = new Color32(204, 63, 0, 255);

        public static readonly Color ColYellow = new Color32(176, 179, 0, 255);

        public static readonly Color ColYellowBg = new Color32(176, 179, 0, 60);

        public static readonly Texture2D DarkGrayFond =
            SolidColorMaterials.NewSolidColorTexture(new Color(1, 1, 1, 0.05f));

        public static readonly Texture2D DeadColonistTex = ContentFinder<Texture2D>.Get("UI/Misc/DeadColonist");

        public static readonly Color FemaleColor = new Color(1f, 0.64f, 0.8f, 1f);

        public static readonly Texture2D GrayFond = SolidColorMaterials.NewSolidColorTexture(new Color(1, 1, 1, 0.07f));

        public static readonly Texture2D GrayLines = SolidColorMaterials.NewSolidColorTexture(new Color(1, 1, 1, 0.25f));

        public static readonly Color HighlightColor = new Color(0.5f, 0.5f, 0.5f, 1f);

        public static readonly Texture2D Icon_Attacking = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Attacking");

        public static readonly Texture2D Icon_Burning = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Burning");

        public static readonly Texture2D Icon_Fleeing = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Fleeing");

        public static readonly Texture2D Icon_Idle = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Idle");

        public static readonly Texture2D Icon_MedicalRest =
            ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/MedicalRest");

        public static readonly Texture2D Icon_MentalStateAggro =
            ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/MentalStateAggro");

        public static readonly Texture2D Icon_MentalStateNonAggro =
            ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/MentalStateNonAggro");

        public static readonly Texture2D Icon_Sleeping = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Sleeping");

        public static readonly Color MaleColor = new Color(0.52f, 0.75f, 0.92f, 1f);

        public static readonly Texture2D MoodBGTex =
            SolidColorMaterials.NewSolidColorTexture(new Color(0f, 0f, 0f, 0.4f));

        public static readonly Texture2D MoodBreakTex =
            SolidColorMaterials.NewSolidColorTexture(new Color(0.1f, 0.2f, 0.22f, 0.8f));

        public static readonly Texture2D MoodNeutralTex = SolidColorMaterials.NewSolidColorTexture(Color.white);

        public static readonly Texture2D MoodNeutralBGTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.6f, 0.6f, 0.6f, 0.3f));


        public static readonly Texture2D MoodTargetTex =
            SolidColorMaterials.NewSolidColorTexture(new Color(0.7f, 0.9f, 0.95f, 0.7f));

        public static readonly Texture2D RedHover =
            SolidColorMaterials.NewSolidColorTexture(new Color(0.7f, 0, 0, 0.12f));

        // public static readonly Texture2D MoodNeutralTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.8f, 0.85f, 0.5f));
        // public static readonly Texture2D VanillaMoodBGTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.4f, 0.47f, 0.53f, 0.44f));
        // public static readonly Texture2D MoodNeutralBGTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.2f, 0.8f));
        // public static readonly Texture2D MoodMinorCrossedTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.65f, 0.65f, 0.2f, 0.5f));
        // public static readonly Texture2D MoodMinorCrossedBGTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.35f, 0.35f, 0.1f, 0.44f));
        // public static readonly Texture2D MoodMajorCrossedTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.95f, 0.65f, 0.05f,  0.5f));
        // public static readonly Texture2D MoodMajorCrossedBGTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.45f, 0.35f, 0.05f, 0.44f));
        // public static readonly Texture2D MoodExtremeCrossedTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.9f, 0.1f, 0.00f, 0.5f));
        // public static readonly Texture2D MoodExtremeCrossedBGTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.6f, 0.15f, 0.00f, 0.44f));
        // public static readonly Texture2D MoodTargetTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.7f, 0.9f, 0.95f, 0.7f));
        // public static readonly Texture2D MoodBreakTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.1f, 0.2f, 0.22f, 0.8f));
        public static readonly Texture2D SelectedTex = ContentFinder<Texture2D>.Get("UI/Overlays/SelectionBracketGUI");

        public static readonly Texture2D VanillaMoodBGTex =
            SolidColorMaterials.NewSolidColorTexture(new Color(0.4f, 0.47f, 0.53f, 0.44f));

        public static Color Color05AndLess = new Color(0.8f, 0.75f, 0.59f);

        public static Color Color10To06 = ColReddishPurple;

        public static Color Color15To11 = ColYellow;

        // public static Color Color25To21 = new Color(0.95f, 0f, 0f);
        public static Color Color20To16 = ColOrange;

        public static Color Color25To21 = ColVermillion;

        public static Color ColorHealthBarGreen = ColBlueishGreen;

        public static Color ColorMoodBoost = ColBlueishGreen;

        // public static Color ColorMoodBoost = new Color(0f, 0.8f, 0f);
        public static Color ColorNeutralStatus = new Color(0.8f, 0.8f, 0.8f);

        public static Color ColorNeutralStatusFade = new Color(
                                                         ColorNeutralStatus.r,
                                                         ColorNeutralStatus.g,
                                                         ColorNeutralStatus.b / 4);

        public static Color ColorNeutralStatusSolid = new Color(
                                                          ColorNeutralStatus.r,
                                                          ColorNeutralStatus.g,
                                                          ColorNeutralStatus.b);

        public static Color ColorOrangeAlert = ColOrange;

        // public static Color ColorHealthBarGreen = new Color(0f, 0.8f, 0f);

        // public static Color ColorRedAlert = new Color(0.95f, 0, 0);
        // public static Color ColorOrangeAlert = Color20To16;
        // public static Color ColorYellowAlert = Color15To11;
        public static Color ColorRedAlert = ColVermillion;

        public static Color ColorYellowAlert = ColYellow;

        public static Texture2D resolvedIcon;
    }
}