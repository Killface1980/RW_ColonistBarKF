namespace KillfaceTools.FMO
{
    using Harmony;
    using JetBrains.Annotations;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Verse;

    public static class Tools
    {
        public static FloatMenuLabels LabelMenu;

        private static FloatMenuColonists actionMenu;

        public static void CloseLabelMenu(bool sound)
        {
            if (LabelMenu != null)
            {
                Find.WindowStack.TryRemove(LabelMenu, sound);
                LabelMenu = null;
            }
        }

        public static FloatMenuOption MakeMenuItemForLabel([NotNull] string label, [NotNull] List<FloatMenuOption> fmo)
        {
            // List<SortByWhat> sortByWhats = fmo.Keys.ToList();
            List<FloatMenuOption> options = fmo.ToList();
            string labelFixed = label;
            FloatMenuOptionNoClose option = new FloatMenuOptionNoClose(
                                                labelFixed,
                                                () =>
                                                    {
                                                        if (options.Count() == 1 && options[0].Disabled == false)
                                                        {
                                                            Action action = options[0].action;
                                                            if (action != null)
                                                            {
                                                                CloseLabelMenu(true);
                                                                action();
                                                            }
                                                        }
                                                        else
                                                        {
                                                            int i = 0;
                                                            List<FloatMenuOption> actions = new List<FloatMenuOption>();
                                                            fmo.Do(
                                                                menuOption =>
                                                                    {
                                                                        FloatMenuOption floatMenuOption =
                                                                            new FloatMenuOption(
                                                                                menuOption.Label,
                                                                                () =>
                                                                                    {
                                                                                        FloatMenuOption pawnOption =
                                                                                            menuOption;
                                                                                        actionMenu.Close();
                                                                                        CloseLabelMenu(true);
                                                                                        pawnOption.action();
                                                                                    },
                                                                                (MenuOptionPriority)i++,
                                                                                () =>
                                                                                    {
                                                                                        // PathInfo.current = pawn;
                                                                                    });
                                                                        actions.Add(floatMenuOption);
                                                                    });
                                                            actionMenu = new FloatMenuColonists(actions, null);
                                                            Find.WindowStack.Add(actionMenu);
                                                        }
                                                    }) {
                                                          Disabled = options.All(o => o.Disabled)
                                                       };
            return option;
        }
    }
}