namespace ColonistBarKF
{
    using ColonistBarKF.Bar;

    using UnityEngine;

    public static class Statics
    {
        [NotNull]
        public static Gradient gradient4 = new Gradient();

        [NotNull]
        public static Gradient gradient4Mood = new Gradient();

        [NotNull]
        public static Gradient gradientRedAlertToNeutral = new Gradient();

        public static Color EvaluateMoodOffset(float moodOffset)
        {
            return gradient4Mood.Evaluate(Mathf.InverseLerp(-25f, 15f, moodOffset));
        }

        static Statics()
        {
            // Build gradients
            GradientColorKey[] gck = new GradientColorKey[4];
            gck[0].color = ColonistBarTextures.ColorNeutralStatus;
            gck[0].time = 0.0f;
            gck[1].color = ColonistBarTextures.ColYellow;
            gck[1].time = 0.33f;
            gck[2].color = ColonistBarTextures.ColOrange;
            gck[2].time = 0.66f;
            gck[3].color = ColonistBarTextures.ColVermillion;
            gck[3].time = 1f;
            GradientAlphaKey[] gak = new GradientAlphaKey[3];
            gak[0].alpha = 0.8f;
            gak[0].time = 0.0f;
            gak[1].alpha = 1f;
            gak[1].time = 0.1625f;
            gak[2].alpha = 1.0f;
            gak[2].time = 1.0f;
            gradient4.SetKeys(gck, gak);

            gck = new GradientColorKey[5];
            gck[0].color = ColonistBarTextures.ColVermillion;
            gck[0].time = 0f;
            gck[1].color = ColonistBarTextures.ColOrange;
            gck[1].time = 0.375f;
            gck[2].color = ColonistBarTextures.ColYellow;
            gck[2].time = 0.5f;
            gck[3].color = ColonistBarTextures.ColorNeutralStatus;
            gck[3].time = 0.625f;
            gck[4].color = ColonistBarTextures.ColBlueishGreen;
            gck[4].time = 1f;
            gak = new GradientAlphaKey[4];
            gak[0].alpha = 1.0f;
            gak[0].time = 0.0f;
            gak[1].alpha = 1.0f;
            gak[1].time = 0.5f;
            gak[2].alpha = 0.8f;
            gak[2].time = 0.625f;
            gak[3].alpha = 1.0f;
            gak[3].time = 0.75f;
            gradient4Mood.SetKeys(gck, gak);

            gck = new GradientColorKey[2];
            gck[0].color = ColonistBarTextures.ColVermillion;
            gck[0].time = 0.0f;
            gck[1].color = ColonistBarTextures.ColorNeutralStatus;
            gck[1].time = 1f;
            gak = new GradientAlphaKey[3];
            gak[0].alpha = 1.0f;
            gak[0].time = 0.0f;
            gak[1].alpha = 1.0f;
            gak[1].time = 0.75f;
            gak[2].alpha = 0.8f;
            gak[2].time = 1.0f;
            gradientRedAlertToNeutral.SetKeys(gck, gak);
        }
    }
}