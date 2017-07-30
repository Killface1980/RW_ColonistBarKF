namespace ColonistBarKF.PSI
{
    using UnityEngine;

    public struct DrawIconEntry
    {
        public Icon icon;

        public Color color;

        public string tooltip;

        public DrawIconEntry(Icon icon, Color color, string tooltip)
        {
            this.icon = icon;
            this.color = color;
            this.tooltip = tooltip;
        }
    }

}