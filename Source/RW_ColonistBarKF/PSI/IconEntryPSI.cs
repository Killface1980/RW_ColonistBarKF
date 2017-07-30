namespace ColonistBarKF.PSI
{
    using UnityEngine;

    public struct IconEntryPSI
    {
        public Icon icon;

        public Color color;

        public float opacity;

        public IconEntryPSI(Icon icon, Color color, float opacity)
        {
            this.icon = icon;
            this.color = color;
            this.opacity = opacity;
        }
    }

}