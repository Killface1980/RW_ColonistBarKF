using UnityEngine;

namespace Verse
{
    [StaticConstructorOnStartup]
    internal class ColonistBarTextures
    {
        public static float SpacingLabel = 20f;
        public static readonly Color HighlightColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        public static readonly Texture2D BGTex = Command.BGTex;
        public static readonly Texture2D BGTexIconPSI = SolidColorMaterials.NewSolidColorTexture(new Color(0, 0, 0, 0.5f));
        public static readonly Texture2D GrayFond = SolidColorMaterials.NewSolidColorTexture(new Color(1, 1, 1, 0.07f));
        public static readonly Texture2D DarkGrayFond = SolidColorMaterials.NewSolidColorTexture(new Color(1, 1, 1, 0.05f));

        public static readonly Texture2D RedHover =
     SolidColorMaterials.NewSolidColorTexture(new Color(0.7f, 0, 0, 0.12f));

        public static readonly Texture2D GrayLines =
     SolidColorMaterials.NewSolidColorTexture(new Color(1, 1, 1, 0.25f));

        public static readonly Texture2D MoodTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.8f, 0.85f, 0.5f));
        public static readonly Texture2D MoodNeutral = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.2f, 0.8f));
        public static readonly Texture2D MoodMinorCrossedTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.85f, 0.85f, 0.2f, 0.4f));
        public static readonly Texture2D MoodMajorCrossedTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.95f, 0.55f, 0.05f, 0.75f));
        public static readonly Texture2D MoodExtremeCrossedTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.95f, 0.15f, 0.00f, 0.8f));
        public static readonly Texture2D MoodExtremeCrossedBGTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.9f, 0.1f, 0.00f, 0.45f));

        public static readonly Texture2D MoodTargetTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.7f, 0.9f, 0.95f, 0.7f));
        public static readonly Texture2D MoodBreakTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.1f, 0.2f, 0.22f, 0.8f));




        public static readonly Texture2D SelectedTex = ContentFinder<Texture2D>.Get("UI/Overlays/SelectionBracketGUI");

        public static readonly Texture2D DeadColonistTex = ContentFinder<Texture2D>.Get("UI/Misc/DeadColonist");

        public static readonly Texture2D Icon_MentalStateNonAggro = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/MentalStateNonAggro");

        public static readonly Texture2D Icon_MentalStateAggro = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/MentalStateAggro");

        public static readonly Texture2D Icon_MedicalRest = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/MedicalRest");

        public static readonly Texture2D Icon_Sleeping = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Sleeping");

        public static readonly Texture2D Icon_Fleeing = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Fleeing");

        public static readonly Texture2D Icon_Attacking = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Attacking");

        public static readonly Texture2D Icon_Idle = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Idle");

        public static readonly Texture2D Icon_Burning = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Burning");

        public static readonly Texture2D BGTexGrey = ContentFinder<Texture2D>.Get("UI/Widgets/CBKF/DesButBG_grey");

        public static readonly Texture2D BGTexVanilla = ContentFinder<Texture2D>.Get("UI/Widgets/CBKF/DesButBG_vanilla");
    }
}
