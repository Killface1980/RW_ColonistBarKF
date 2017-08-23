namespace ColonistBarKF.Menus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Harmony;

    using JetBrains.Annotations;

    using Verse;

    public static class Tools
    {
        #region Public Fields

        public static FloatMenuColonists actionMenu;
        public static FloatMenuLabels labelMenu;

        #endregion Public Fields

        #region Public Methods

        public static void CloseLabelMenu(bool sound)
        {
            if (labelMenu != null)
            {
                Find.WindowStack.TryRemove(labelMenu, sound);
                labelMenu = null;
            }
        }

        public static FloatMenuOption MakeMenuItemForLabel([NotNull] string label, [NotNull] List<FloatMenuOption> fmo)
        {
            // List<SortByWhat> sortByWhats = fmo.Keys.ToList();
            List<FloatMenuOption> options = fmo.ToList();
            string labelFixed = label;
            FloatMenuOptionNoClose option = new FloatMenuOptionNoClose(labelFixed, () =>
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
                                     fmo.Do(menuOption =>
                                         {
                                             actions.Add(
                                                 new FloatMenuOption(
                                                     menuOption.Label,
                                                     () =>
                                                         {
                                                             FloatMenuOption pawnOption = menuOption;
                                                             actionMenu.Close(true);
                                                             CloseLabelMenu(true);
                                                             pawnOption.action();
                                                         },
                                                     (MenuOptionPriority)i++,
                                                     () =>
                                                         {
                                                             // PathInfo.current = pawn;
                                                         }));
                                         });
                                     actionMenu = new FloatMenuColonists(actions, null);
                                     Find.WindowStack.Add(actionMenu);
                                 }
                             })
            {
                Disabled = options.All(o => o.Disabled)
            };
            return option;
        }

        #endregion Public Methods
    }
}