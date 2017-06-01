using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace ColonistBarKF.Detouring
{
    using Harmony;

    using Verse.Sound;

    class _WorldSelectorHarmony
    {

        //    [Detour(typeof(WorldSelector), bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic)]
        [HarmonyPatch(typeof(WorldSelector), "SelectInsideDragBox")]
        static class SelectInsideDragBox_Pretfix
        {
            [HarmonyPrefix]
            private static void SelectInsideDragBox(WorldSelector __instance)
            {
                if (!ShiftIsHeld)
                {
                    __instance.ClearSelection();
                }
                bool flag = false;
                if (Current.ProgramState == ProgramState.Playing)
                {
                    List<Caravan> list = ColonistBar_KF.CaravanMembersCaravansInScreenRect(__instance.dragBox.ScreenRect);
                    for (int i = 0; i < list.Count; i++)
                    {
                        flag = true;
                        __instance.Select(list[i], true);
                    }
                }
                if (!flag && Current.ProgramState == ProgramState.Playing)
                {
                    List<Thing> list2 = ColonistBar_KF.MapColonistsOrCorpsesInScreenRect(__instance.dragBox.ScreenRect);
                    for (int j = 0; j < list2.Count; j++)
                    {
                        if (!flag)
                        {
                            CameraJumper.TryJumpAndSelect(list2[j]);
                            flag = true;
                        }
                        else
                        {
                            Find.Selector.Select(list2[j], true, true);
                        }
                    }
                }
                if (!flag)
                {
                    List<WorldObject> list3 = WorldObjectSelectionUtility.MultiSelectableWorldObjectsInScreenRectDistinct(__instance.dragBox.ScreenRect).ToList<WorldObject>();
                    if (list3.Any((WorldObject x) => x is Caravan))
                    {
                        list3.RemoveAll((WorldObject x) => !(x is Caravan));
                        if (list3.Any((WorldObject x) => x.Faction == Faction.OfPlayer))
                        {
                            list3.RemoveAll((WorldObject x) => x.Faction != Faction.OfPlayer);
                        }
                    }
                    for (int k = 0; k < list3.Count; k++)
                    {
                        flag = true;
                        __instance.Select(list3[k], true);
                    }
                }
                if (!flag)
                {
                    bool canSelectTile = __instance.dragBox.Diagonal < 30f;
                    SelectUnderMouse_Pretfix.SelectUnderMouse(__instance, canSelectTile);
                }
            }
        }

        [HarmonyPatch(typeof(WorldSelector), "SelectUnderMouse")]
        static class SelectUnderMouse_Pretfix
        {
            private static readonly FieldInfo selectedField = AccessTools.Field(typeof(WorldSelector), "selected");

            [HarmonyPrefix]
            //    [Detour(typeof(WorldSelector), bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic)]
            public static void SelectUnderMouse(WorldSelector __instance, bool canSelectTile = true)
            {
                List<object> __result = (List<object>)selectedField.GetValue(__instance);

                if (Current.ProgramState == ProgramState.Playing)
                {
                    Thing thing = ColonistBar_KF.ColonistOrCorpseAt(UI.MousePositionOnUIInverted);
                    Pawn pawn = thing as Pawn;
                    if (thing != null && (pawn == null || !pawn.IsCaravanMember()))
                    {
                        if (thing.Spawned)
                        {
                            CameraJumper.TryJumpAndSelect(thing);
                        }
                        else
                        {
                            CameraJumper.TryJump(thing);
                        }
                        return;
                    }
                }
                bool flag;
                bool flag2;
                List<WorldObject> list = SelectableObjectsUnderMouse(out flag, out flag2).ToList<WorldObject>();
                if (flag2 || (flag && list.Count >= 2))
                {
                    canSelectTile = false;
                }
                if (list.Count == 0)
                {
                    if (!ShiftIsHeld)
                    {
                        __instance.ClearSelection();
                        if (canSelectTile)
                        {
                            __instance.selectedTile = GenWorld.MouseTile(false);
                        }
                    }
                }
                else
                {
                    WorldObject worldObject = (from obj in list
                                               where __result.Contains(obj)
                                               select obj).FirstOrDefault<WorldObject>();
                    if (worldObject != null)
                    {
                        if (!ShiftIsHeld)
                        {
                            int tile = (!canSelectTile) ? -1 : GenWorld.MouseTile(false);
                            SelectFirstOrNextFrom_Pretfix.SelectFirstOrNextFrom(__instance, list, tile);
                        }
                        else
                        {
                            foreach (WorldObject current in list)
                            {
                                if (__result.Contains(current))
                                {
                                    __instance.Deselect(current);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!ShiftIsHeld)
                        {
                            __instance.ClearSelection();
                        }
                        __instance.Select(list[0], true);
                    }
                }
            }


        }


        [HarmonyPatch(typeof(WorldSelector), "SelectFirstOrNextFrom")]
        static class SelectFirstOrNextFrom_Pretfix
        {
            private static readonly FieldInfo selectedField = AccessTools.Field(typeof(WorldSelector), "selected");

            [HarmonyPrefix]
            //    [Detour(typeof(WorldSelector), bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic)]
            public static void SelectFirstOrNextFrom(WorldSelector __instance, List<WorldObject> objects, int tile)
            {
                List<object> __result = (List<object>)selectedField.GetValue(__instance);

                int num = objects.FindIndex((WorldObject x) => __result.Contains(x));
                int num2 = -1;
                int num3 = -1;
                if (num != -1)
                {
                    if (num == objects.Count - 1 || __result.Count >= 2)
                    {
                        if (__result.Count >= 2)
                        {
                            num3 = 0;
                        }
                        else if (tile >= 0)
                        {
                            num2 = tile;
                        }
                        else
                        {
                            num3 = 0;
                        }
                    }
                    else
                    {
                        num3 = num + 1;
                    }
                }
                else if (objects.Count == 0)
                {
                    num2 = tile;
                }
                else
                {
                    num3 = 0;
                }
                __instance.ClearSelection();
                if (num3 >= 0)
                {
                    __instance.Select(objects[num3], true);
                }
                __instance.selectedTile = num2;
            }
        }


        private static bool ShiftIsHeld
        {
            get
            {
                return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            }
        }

        private static IEnumerable<WorldObject> SelectableObjectsUnderMouse(out bool clickedDirectlyOnCaravan, out bool usedColonistBar)
        {
            Vector2 mousePositionOnUIInverted = UI.MousePositionOnUIInverted;
            if (Current.ProgramState == ProgramState.Playing)
            {
                Caravan caravan = ColonistBar_KF.CaravanMemberCaravanAt(mousePositionOnUIInverted);
                if (caravan != null)
                {
                    clickedDirectlyOnCaravan = true;
                    usedColonistBar = true;
                    return Gen.YieldSingle<WorldObject>(caravan);
                }
            }

            List<WorldObject> list = GenWorldUI.WorldObjectsUnderMouse(UI.MousePositionOnUI);
            clickedDirectlyOnCaravan = false;
            if (list.Count > 0 && list[0] is Caravan && list[0].DistanceToMouse(UI.MousePositionOnUI) < GenWorldUI.CaravanDirectClickRadius)
            {
                clickedDirectlyOnCaravan = true;
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    WorldObject worldObject = list[i];
                    if (worldObject is Caravan && worldObject.DistanceToMouse(UI.MousePositionOnUI) > GenWorldUI.CaravanDirectClickRadius)
                    {
                        list.Remove(worldObject);
                    }
                }
            }

            usedColonistBar = false;
            return list;
        }


    }
}
