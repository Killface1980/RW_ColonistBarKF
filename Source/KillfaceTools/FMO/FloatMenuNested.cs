namespace KillfaceTools.FMO
{
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using UnityEngine;

    using Verse;

    public class FloatMenuNested : FloatMenu
    {
        public FloatMenuNested([NotNull] List<FloatMenuOption> options, [CanBeNull] string label)
            : base(options, label)
        {
            this.givesColonistOrders = true;
            this.vanishIfMouseDistant = true;
            this.closeOnClickedOutside = true;
        }

        public override void DoWindowContents(Rect rect)
        {
            this.options.ForEach(
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