using UnityEngine;

namespace Verse
{
    [StaticConstructorOnStartup]
    internal class ColonistBarTextures
    {
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

        public static readonly Texture2D SelectedTex = ContentFinder<Texture2D>.Get("UI/Overlays/SelectionBracketGUI", true);

        public static readonly Texture2D DeadColonistTex = ContentFinder<Texture2D>.Get("UI/Misc/DeadColonist", true);

        public static readonly Texture2D Icon_MentalStateNonAggro = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/MentalStateNonAggro", true);

        public static readonly Texture2D Icon_MentalStateAggro = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/MentalStateAggro", true);

        public static readonly Texture2D Icon_MedicalRest = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/MedicalRest", true);

        public static readonly Texture2D Icon_Sleeping = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Sleeping", true);

        public static readonly Texture2D Icon_Fleeing = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Fleeing", true);

        public static readonly Texture2D Icon_Attacking = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Attacking", true);

        public static readonly Texture2D Icon_Idle = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Idle", true);

        public static readonly Texture2D Icon_Burning = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Burning", true);

        public static readonly Texture2D BGTexGrey = ContentFinder<Texture2D>.Get("UI/Widgets/DesButBG_grey", true);

        public static readonly Texture2D BGTexVanilla = ContentFinder<Texture2D>.Get("UI/Widgets/DesButBG_vanilla", true);

        public static Texture2D BGTex;
    }
}
