namespace ColonistBarKF.PSI
{
    using JetBrains.Annotations;

    using UnityEngine;

    public struct IconEntryBar
    {
        public Icon icon;

        public Color color;

        public int priority;

        [CanBeNull]
        public string tooltip;

        public IconEntryBar(Icon icon, Color color, [CanBeNull] string tooltip, int priority = 10)
        {
            this.icon = icon;
            this.color = color;
            this.tooltip = tooltip;
            this.priority = priority;
        }
    }
}