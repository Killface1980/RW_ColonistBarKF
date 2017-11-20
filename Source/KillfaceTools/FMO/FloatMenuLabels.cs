namespace KillfaceTools.FMO
{
    using System;
    using System.Collections.Generic;

    using UnityEngine;

    using Verse;

    public class FloatMenuLabels : FloatMenu
    {
        public FloatMenuLabels(List<FloatMenuOption> options)
            : base(options, null)
        {
            this.givesColonistOrders = false;
            this.vanishIfMouseDistant = true;
            this.closeOnClickedOutside = false;
        }
    }

    public class FloatMenuOptionNoClose : FloatMenuOption
    {
        public FloatMenuOptionNoClose(string label, Action action)
            : base(label, action)
        {
        }

        public override bool DoGUI(Rect rect, bool colonistOrdering)
        {
            base.DoGUI(rect, colonistOrdering);
            return false; // don't close after an item is selected
        }
    }
}