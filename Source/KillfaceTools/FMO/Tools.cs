namespace KillfaceTools.FMO
{
    using Harmony;
    using JetBrains.Annotations;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;

    using Verse;

    public static class Tools
    {
        public const string NestedString = " ►";

        public static FloatMenuLabels LabelMenu;

        private static FloatMenuNested actionMenu;

        public static void CloseLabelMenu(bool sound)
        {
            if (LabelMenu == null)
            {
                return;
            }

            Find.WindowStack.TryRemove(LabelMenu, sound);
            LabelMenu = null;
        }

        public static FloatMenuOption MakeMenuItemForLabel([NotNull] string label, [NotNull] List<FloatMenuOption> fmo)
        {
            // List<SortByWhat> sortByWhats = fmo.Keys.ToList();
            List<FloatMenuOption> options = fmo.ToList();
            string labelFixed = label;
            bool isSingle = options.Count == 1 && !labelFixed.Contains(NestedString);



            FloatMenuOptionNoClose option = new FloatMenuOptionNoClose(
                                                                       labelFixed,
                                                                       () =>
                                                                       {
                                                                           if (isSingle && options[0].Disabled == false)
                                                                           {
                                                                               Action action = options[0].action;
                                                                               if (action == null)
                                                                               {
                                                                                   return;
                                                                               }

                                                                               CloseLabelMenu(true);
                                                                               action();
                                                                           }
                                                                           else
                                                                           {
                                                                               int i = 0;
                                                                               List<FloatMenuOption> actions =
                                                                               new List<FloatMenuOption>();
                                                                               fmo.Do(
                                                                                      menuOption =>
                                                                                      {
                                                                                          FloatMenuOption
                                                                                          floatMenuOption =
                                                                                          new FloatMenuOption(
                                                                                                              menuOption
                                                                                                             .Label,
                                                                                                              () =>
                                                                                                              {
                                                                                                                  FloatMenuOption
                                                                                                                  pawnOption
                                                                                                                  =
                                                                                                                  menuOption;
                                                                                                                  actionMenu
                                                                                                                 .Close();
                                                                                                                  CloseLabelMenu(true);
                                                                                                                  pawnOption
                                                                                                                 .action();
                                                                                                              },
                                                                                                              (MenuOptionPriority
                                                                                                              ) i++,
                                                                                                              menuOption
                                                                                                             .mouseoverGuiAction,
                                                                                                              menuOption
                                                                                                             .revalidateClickTarget,
                                                                                                              menuOption
                                                                                                             .extraPartWidth,
                                                                                                              menuOption
                                                                                                             .extraPartOnGUI);
                                                                                          actions.Add(floatMenuOption);
                                                                                      });
                                                                               actionMenu =
                                                                               new FloatMenuNested(actions, null);
                                                                               Find.WindowStack.Add(actionMenu);
                                                                           }
                                                                       },
                                                                       isSingle ? options[0].extraPartWidth : 0f,
                                                                       isSingle ? options[0].extraPartOnGUI : null)
                                            {
                                            Disabled = options.All(o => o.Disabled),
                                            };
            return option;
        }
    }
}