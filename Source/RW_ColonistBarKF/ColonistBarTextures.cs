using UnityEngine;

namespace Verse
{
    [StaticConstructorOnStartup]
    internal class ColonistBarTextures
    {
        public static readonly Texture2D BGTexIconPSI = SolidColorMaterials.NewSolidColorTexture(new Color(0, 0, 0, 0.5f));
        public static readonly Texture2D PureDarkGray = SolidColorMaterials.NewSolidColorTexture(new Color(0.15f, 0.15f, 0.15f, 1));

        public static readonly Texture2D HoverBG =
            SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.2f, 1));

        public static readonly Texture2D MoodBGTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.7f, 0.7f, 0.7f, 0.7f));
        public static readonly Texture2D MoodGoodTex = SolidColorMaterials.NewSolidColorTexture(new Color(0f, 0.8f, 0f, 0.7f));
        public static readonly Texture2D MoodNeutral = SolidColorMaterials.NewSolidColorTexture(new Color(0.7f, 0.7f, 0.7f, 0.85f));
        public static readonly Texture2D MoodMinorCrossedTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.8f, 0.6f, 0.1f, 0.85f));
        public static readonly Texture2D MoodMajorCrossedTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.8f, 0.3f, 0f, 0.85f));
        public static readonly Texture2D MoodExtremeCrossedTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.8f, 0f, 0f, 0.85f));

        public static readonly Texture2D MoodTargetTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.8f, 0.8f, 0.8f, 0.9f));
        public static readonly Texture2D MoodBreakTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.1f, 0.1f, 0.1f, 0.9f));


        //   private static Texture2D BGTex = ContentFinder<Texture2D>.Get("UI/Widgets/DesButBG", true);
        //   private static readonly Texture2D BGTex = Command.BGTex;
    //    public static Texture2D BGTex = Command.BGTex;

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

        public static readonly Texture2D BGTexGrey = ContentFinder<Texture2D>.Get("UI/Widgets/DesButBG_grey");

        public static readonly Texture2D BGTexVanilla = ContentFinder<Texture2D>.Get("UI/Widgets/DesButBG_vanilla");

        public static Texture2D BGTex;
    }
}
