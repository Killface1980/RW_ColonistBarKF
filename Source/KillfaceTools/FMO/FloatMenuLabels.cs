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
        public FloatMenuOptionNoClose(string label, Action action, float extraPartWidth, Func<Rect, bool> extraPartOnGUI =null)
            : base(label, action, extraPartWidth: extraPartWidth, extraPartOnGUI: extraPartOnGUI)
        {
        }

        public override bool DoGUI(Rect rect, bool colonistOrdering, FloatMenu floatMenu)
        {
            base.DoGUI(rect, colonistOrdering, floatMenu);
            return false; // don't close after an item is selected
        }
    }
}