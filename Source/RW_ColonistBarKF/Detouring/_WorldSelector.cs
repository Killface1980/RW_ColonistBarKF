namespace ColonistBarKF.Detouring
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using ColonistBarKF.Bar;

    using RimWorld;
    using RimWorld.Planet;

    using UnityEngine;

    using Verse;

    public class _WorldSelector
    {
        // RimWorld.Planet.WorldSelector
        [Detour(typeof(WorldSelector), bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic)]
        private void SelectInsideDragBox()
        {
            if (!ShiftIsHeld)
            {
                Find.WorldSelector.ClearSelection();
            }

            bool flag = false;
            if (Current.ProgramState == ProgramState.Playing)
            {
                List<Caravan> list = ColonistBar_KF.CaravanMembersCaravansInScreenRect(Find.WorldSelector.dragBox.ScreenRect);
                for (int i = 0; i < list.Count; i++)
                {
                    flag = true;
                    Find.WorldSelector.Select(list[i], true);
                }
            }

            if (!flag && Current.ProgramState == ProgramState.Playing)
            {
                List<Thing> list2 = ColonistBar_KF.MapColonistsOrCorpsesInScreenRect(Find.WorldSelector.dragBox.ScreenRect);
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
                List<WorldObject> list3 = WorldObjectSelectionUtility.MultiSelectableWorldObjectsInScreenRectDistinct(Find.WorldSelector.dragBox.ScreenRect).ToList();
                if (list3.Any(x => x is Caravan))
                {
                    list3.RemoveAll(x => !(x is Caravan));
                    if (list3.Any(x => x.Faction == Faction.OfPlayer))
                    {
                        list3.RemoveAll(x => x.Faction != Faction.OfPlayer);
                    }
                }

                for (int k = 0; k < list3.Count; k++)
                {
                    flag = true;
                    Find.WorldSelector.Select(list3[k], true);
                }
            }

            if (!flag)
            {
                bool canSelectTile = Find.WorldSelector.dragBox.Diagonal < 30f;
                SelectUnderMouse(canSelectTile);
            }
        }

        private bool ShiftIsHeld
        {
            get
            {
                return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            }
        }

        // RimWorld.Planet.WorldSelector
        [Detour(typeof(WorldSelector), bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic)]
        private void SelectUnderMouse(bool canSelectTile = true)
        {
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
            List<WorldObject> list = SelectableObjectsUnderMouse(out flag, out flag2).ToList();
            if (flag2 || (flag && list.Count >= 2))
            {
                canSelectTile = false;
            }

            if (list.Count == 0)
            {
                if (!ShiftIsHeld)
                {
                    Find.WorldSelector.ClearSelection();
                    if (canSelectTile)
                    {
                        Find.WorldSelector.selectedTile = GenWorld.MouseTile();
                    }
                }
            }
            else
            {
                WorldObject worldObject = (from obj in list
                                           where Find.WorldSelector.SelectedObjects.Contains(obj)
                                           select obj).FirstOrDefault();
                if (worldObject != null)
                {
                    if (!ShiftIsHeld)
                    {
                        int tile = (!canSelectTile) ? -1 : GenWorld.MouseTile();
                        SelectFirstOrNextFrom(list, tile);
                    }
                    else
                    {
                        foreach (WorldObject current in list)
                        {
                            if (Find.WorldSelector.SelectedObjects.Contains(current))
                            {
                                Find.WorldSelector.Deselect(current);
                            }
                        }
                    }
                }
                else
                {
                    if (!ShiftIsHeld)
                    {
                        Find.WorldSelector.ClearSelection();
                    }

                    Find.WorldSelector.Select(list[0], true);
                }
            }
        }

        // RimWorld.Planet.WorldSelector
        [Detour(typeof(WorldSelector), bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic)]
        private void SelectFirstOrNextFrom(List<WorldObject> objects, int tile)
        {
            int num = objects.FindIndex(x => Find.WorldSelector.SelectedObjects.Contains(x));
            int num2 = -1;
            int num3 = -1;
            if (num != -1)
            {
                if (num == objects.Count - 1 || Find.WorldSelector.SelectedObjects.Count >= 2)
                {
                    if (Find.WorldSelector.SelectedObjects.Count >= 2)
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

            Find.WorldSelector.ClearSelection();
            if (num3 >= 0)
            {
                Find.WorldSelector.Select(objects[num3], true);
            }

            Find.WorldSelector.selectedTile = num2;
        }

        // RimWorld.Planet.WorldSelector
        private IEnumerable<WorldObject> SelectableObjectsUnderMouse(out bool clickedDirectlyOnCaravan, out bool usedColonistBar)
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
