namespace ColonistBarKF.PSI
{
    using UnityEngine;

    public struct IconEntryBar
    {
        public Icon icon;

        public Color color;

        public string tooltip;

        public IconEntryBar(Icon icon, Color color, string tooltip)
        {
            this.icon = icon;
            this.color = color;
            this.tooltip = tooltip;
        }
    }

}