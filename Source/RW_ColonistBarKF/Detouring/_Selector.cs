using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using static ColonistBarKF.CBKF;

namespace ColonistBarKF.Detouring
{
    public class _Selector
    {
      
        // RimWorld.Selector
        [Detour(typeof(Selector), bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic)]
        private void SelectUnderMouse()
        {
            Caravan caravan = ColonistBar_KF.CaravanMemberCaravanAt(UI.MousePositionOnUIInverted);
            if (caravan != null)
            {
                CameraJumper.TryJumpAndSelect(caravan);
                return;
            }
            Thing thing = ColonistBar_KF.ColonistOrCorpseAt(UI.MousePositionOnUIInverted);
            if (thing != null && !thing.Spawned)
            {
                CameraJumper.TryJump(thing);
                return;
            }
            List<object> list = SelectableObjectsUnderMouse().ToList();
            if (list.Count == 0)
            {
                if (!ShiftIsHeld)
                {
                    Find.Selector.ClearSelection();
                }
            }
            else if (list.Count == 1)
            {
                object obj3 = list[0];
                if (!ShiftIsHeld)
                {
                    Find.Selector.ClearSelection();
                    Find.Selector.Select(obj3, true, true);
                }
                else if (!Find.Selector.SelectedObjects.Contains(obj3))
                {
                    Find.Selector.Select(obj3, true, true);
                }
                else
                {
                    Find.Selector.Deselect(obj3);
                }
            }
            else if (list.Count > 1)
            {
                object obj2 = (from obj in list
                               where Find.Selector.SelectedObjects.Contains(obj)
                               select obj).FirstOrDefault();
                if (obj2 != null)
                {
                    if (!ShiftIsHeld)
                    {
                        int num = list.IndexOf(obj2) + 1;
                        if (num >= list.Count)
                        {
                            num -= list.Count;
                        }
                        Find.Selector.ClearSelection();
                        Find.Selector.Select(list[num], true, true);
                    }
                    else
                    {
                        foreach (object current in list)
                        {
                            if (Find.Selector.SelectedObjects.Contains(current))
                            {
                                Find.Selector.Deselect(current);
                            }
                        }
                    }
                }
                else
                {
                    if (!ShiftIsHeld)
                    {
                        Find.Selector.ClearSelection();
                    }
                    Find.Selector.Select(list[0], true, true);
                }
            }
        }

        private bool ShiftIsHeld
        {
            get
            {
                return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            }
        }
   
        // RimWorld.Selector
        [Detour(typeof(Selector), bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic)]
        private void SelectAllMatchingObjectUnderMouseOnScreen()
        {
            List<object> list = SelectableObjectsUnderMouse().ToList();
            if (list.Count == 0)
            {
                return;
            }
            Thing clickedThing = list.FirstOrDefault(o => o is Pawn && ((Pawn)o).Faction == Faction.OfPlayer && !((Pawn)o).IsPrisoner) as Thing;
            clickedThing = (list.FirstOrDefault(o => o is Pawn) as Thing);
            if (clickedThing == null)
            {
                clickedThing = ((from o in list
                                 where o is Thing && !((Thing)o).def.neverMultiSelect
                                 select o).FirstOrDefault() as Thing);
            }
            Rect rect = new Rect(0f, 0f, UI.screenWidth, UI.screenHeight);
            if (clickedThing != null)
            {
                IEnumerable enumerable = ThingSelectionUtility.MultiSelectableThingsInScreenRectDistinct(rect);
                Predicate<Thing> predicate = delegate (Thing t)
                {
                    if (t.def != clickedThing.def || t.Faction != clickedThing.Faction || Find.Selector.IsSelected(t))
                    {
                        return false;
                    }
                    Pawn pawn = clickedThing as Pawn;
                    if (pawn != null)
                    {
                        Pawn pawn2 = t as Pawn;
                        if (pawn2.RaceProps != pawn.RaceProps)
                        {
                            return false;
                        }
                        if (pawn2.HostFaction != pawn.HostFaction)
                        {
                            return false;
                        }
                    }
                    return true;
                };
                foreach (Thing obj in enumerable)
                {
                    if (predicate(obj))
                    {
                        Find.Selector.Select(obj, true, true);
                    }
                }
                return;
            }
            if (list.FirstOrDefault(o => o is Zone && ((Zone)o).IsMultiselectable) == null)
            {
                return;
            }
            IEnumerable<Zone> enumerable2 = ThingSelectionUtility.MultiSelectableZonesInScreenRectDistinct(rect);
            foreach (Zone current in enumerable2)
            {
                if (!Find.Selector.IsSelected(current))
                {
                    Find.Selector.Select(current, true, true);
                }
            }
        }

        // RimWorld.Selector
        [Detour(typeof(Selector), bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic)]
        private void SelectInsideDragBox()
        {
            if (!ShiftIsHeld)
            {
                Find.Selector.ClearSelection();
            }
            bool selectedSomething = false;
            List<Thing> list = ColonistBar_KF.MapColonistsOrCorpsesInScreenRect(Find.Selector.dragBox.ScreenRect);
            for (int i = 0; i < list.Count; i++)
            {
                selectedSomething = true;
                Find.Selector.Select(list[i], true, true);
            }
            if (selectedSomething)
            {
                return;
            }
            List<Caravan> list2 = ColonistBar_KF.CaravanMembersCaravansInScreenRect(Find.Selector.dragBox.ScreenRect);
            for (int j = 0; j < list2.Count; j++)
            {
                if (!selectedSomething)
                {
                    CameraJumper.TryJumpAndSelect(list2[j]);
                    selectedSomething = true;
                }
                else
                {
                    Find.WorldSelector.Select(list2[j], true);
                }
            }
            if (selectedSomething)
            {
                return;
            }
            List<Thing> boxThings = ThingSelectionUtility.MultiSelectableThingsInScreenRectDistinct(Find.Selector.dragBox.ScreenRect).ToList();
            Func<Predicate<Thing>, bool> func = delegate (Predicate<Thing> predicate)
            {
                foreach (Thing current2 in from t in boxThings
                                           where predicate(t)
                                           select t)
                {
                    Find.Selector.Select(current2, true, true);
                    selectedSomething = true;
                }
                return selectedSomething;
            };
            Predicate<Thing> arg = t => t.def.category == ThingCategory.Pawn && ((Pawn)t).RaceProps.Humanlike && t.Faction == Faction.OfPlayer;
            if (func(arg))
            {
                return;
            }
            Predicate<Thing> arg2 = t => t.def.category == ThingCategory.Pawn && ((Pawn)t).RaceProps.Humanlike;
            if (func(arg2))
            {
                return;
            }
            Predicate<Thing> arg3 = t => t.def.CountAsResource;
            if (func(arg3))
            {
                return;
            }
            Predicate<Thing> arg4 = t => t.def.category == ThingCategory.Pawn;
            if (func(arg4))
            {
                return;
            }
            if (func(t => t.def.selectable))
            {
                return;
            }
            List<Zone> list3 = ThingSelectionUtility.MultiSelectableZonesInScreenRectDistinct(Find.Selector.dragBox.ScreenRect).ToList();
            foreach (Zone current in list3)
            {
                selectedSomething = true;
                Find.Selector.Select(current, true, true);
            }
            if (selectedSomething)
            {
                return;
            }
            SelectUnderMouse();
        }


        // RimWorld.Selector
        [DebuggerHidden]
        private IEnumerable<object> SelectableObjectsUnderMouse()
        {
            Vector2 mousePos = UI.MousePositionOnUIInverted;
            Thing colonistOrCorpse = ColonistBar_KF.ColonistOrCorpseAt(mousePos);
            if (colonistOrCorpse != null && colonistOrCorpse.Spawned)
            {
                yield return colonistOrCorpse;
            }
            else if (UI.MouseCell().InBounds(Find.VisibleMap))
            {
                TargetingParameters selectParams = new TargetingParameters();
                selectParams.mustBeSelectable = true;
                selectParams.canTargetPawns = true;
                selectParams.canTargetBuildings = true;
                selectParams.canTargetItems = true;
                selectParams.mapObjectTargetsMustBeAutoAttackable = false;
                List<Thing> selectableList = GenUI.ThingsUnderMouse(UI.MouseMapPosition(), 1f, selectParams);
                if (selectableList.Count > 0 && selectableList[0] is Pawn && (selectableList[0].DrawPos - UI.MouseMapPosition()).MagnitudeHorizontal() < 0.4f)
                {
                    for (int i = selectableList.Count - 1; i >= 0; i--)
                    {
                        Thing t = selectableList[i];
                        if (t.def.category == ThingCategory.Pawn && (t.DrawPos - UI.MouseMapPosition()).MagnitudeHorizontal() > 0.4f)
                        {
                            selectableList.Remove(t);
                        }
                    }
                }
                for (int j = 0; j < selectableList.Count; j++)
                {
                    yield return selectableList[j];
                }
                Zone z = Find.VisibleMap.zoneManager.ZoneAt(UI.MouseCell());
                if (z != null)
                {
                    yield return z;
                }
            }
        }

    }
}
