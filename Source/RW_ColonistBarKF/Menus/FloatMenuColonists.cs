namespace ColonistBarKF.Menus
{
    using Harmony;
    using JetBrains.Annotations;
    using System.Collections.Generic;
    using UnityEngine;
    using Verse;

    public class FloatMenuColonists : FloatMenu
    {
        public FloatMenuColonists([NotNull] List<FloatMenuOption> options, [CanBeNull] string label)
            : base(options, label)
        {
            this.givesColonistOrders = true;
            this.vanishIfMouseDistant = true;
            this.closeOnClickedOutside = true;
        }

        public override void DoWindowContents(Rect rect)
        {
            this.options.Do(
                o =>
                    {
                        // FloatMenuOptionSorting option = o as FloatMenuOptionSorting;
                        // option.Label = PathInfo.GetJobReport(option.sortBy);
                        o.SetSizeMode(FloatMenuSizeMode.Normal);
                    });
            this.windowRect = new Rect(this.windowRect.x, this.windowRect.y, this.InitialSize.x, this.InitialSize.y);
            base.DoWindowContents(this.windowRect);
        }

        public override void PostClose()
        {
            base.PostClose();

            Tools.CloseLabelMenu(false);
        }
    }
}