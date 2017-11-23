namespace ColonistBarKF.PSI
{
    using UnityEngine;

    public struct IconEntryPSI
    {
        public Icon icon;

        public Color color;

        public float opacity;

        public int priority;

        public IconEntryPSI(Icon icon, Color color, float opacity, int priority = 10)
        {
            this.icon = icon;
            this.color = color;
            this.opacity = opacity;
            this.priority = priority;
        }
    }
}