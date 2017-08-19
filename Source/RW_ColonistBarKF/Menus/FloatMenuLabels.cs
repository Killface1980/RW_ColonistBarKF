namespace ColonistBarKF.Menus
{
    using System;
    using System.Collections.Generic;

    using UnityEngine;

    using Verse;

    public class FloatMenuLabels : FloatMenu
    {
        #region Public Constructors

        public FloatMenuLabels(List<FloatMenuOption> options)
            : base(options, null, false)
        {
            this.givesColonistOrders = false;
            this.vanishIfMouseDistant = true;
            this.closeOnClickedOutside = false;
        }

        #endregion Public Constructors
    }

    public class FloatMenuOptionNoClose : FloatMenuOption
    {
        #region Public Constructors

        public FloatMenuOptionNoClose(string label, Action action)
            : base(label, action, MenuOptionPriority.Default, null, null, 0, null, null)
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public override bool DoGUI(Rect rect, bool colonistOrdering)
        {
            base.DoGUI(rect, colonistOrdering);
            return false; // don't close after an item is selected
        }

        #endregion Public Methods
    }
}