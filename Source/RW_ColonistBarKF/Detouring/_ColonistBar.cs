using RimWorld.Planet;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ColonistBarKF
{
    using System.Linq;
    using System.Reflection;

    using global::ColonistBarKF;

    using global::ColonistBarKF.PSI;

    using RimWorld;

    using static ColonistBarKF.CBKF;
    using static ColonistBarKF.Position;
    using static ColonistBarKF.SettingsColonistBar.SortByWhat;

    [StaticConstructorOnStartup]
    public class ColonistBar_KF
    {

        public const float BaseSelectedTexJump = 20f;

        public const float BaseSelectedTexScale = 0.4f;

        public const float EntryInAnotherMapAlpha = 0.4f;

        public const float BaseSpaceBetweenGroups = 25f;

        public const float BaseSpaceBetweenColonistsHorizontal = 24f;

        public const float BaseSpaceBetweenColonistsVertical = 32f;

        public ColonistBarColonistDrawerKF drawerKF = new ColonistBarColonistDrawerKF();

        private ColonistBarDrawLocsFinderKF drawLocsFinderKf = new ColonistBarDrawLocsFinderKF();

        private List<ColonistBar.Entry> cachedEntries = new List<ColonistBar.Entry>();

        private List<Vector2> cachedDrawLocs = new List<Vector2>();

        private float cachedScale = 1f;

        private bool entriesDirty = true;

        public static readonly Texture2D BGTex = Command.BGTex;

        public static Vector2 BaseSize = new Vector2(48f, 48f);

        private static List<Pawn> tmpPawns = new List<Pawn>();

        private static List<Map> tmpMaps = new List<Map>();

        private static List<Caravan> tmpCaravans = new List<Caravan>();

        private static List<Pawn> tmpColonistsInOrder = new List<Pawn>();

        private static List<Pair<Thing, Map>> tmpColonistsWithMap = new List<Pair<Thing, Map>>();

        private static List<Thing> tmpColonists = new List<Thing>();

        private static List<Thing> tmpMapColonistsOrCorpsesInScreenRect = new List<Thing>();

        private static List<Pawn> tmpCaravanPawns = new List<Pawn>();

        public List<ColonistBar.Entry> Entries
        {
            get
            {
                CheckRecacheEntries();
                return cachedEntries;
            }
        }

        private bool ShowGroupFrames
        {
            get
            {
                List<ColonistBar.Entry> entries = Entries;
                int num = -1;
                for (int i = 0; i < entries.Count; i++)
                {
                    num = Mathf.Max(num, entries[i].group);
                }
                return num >= 1;
            }
        }
        public static float CurrentScale;

        public float Scale
        {
            get
            {
                if (ColBarSettings.UseFixedIconScale)
                {
                    CurrentScale = 1f;

                    if (ColBarSettings.UseFixedIconScale)
                    {
                        return ColBarSettings.FixedIconScaleFloat;
                    }

                    if (ColBarSettings.ColBarPos == Alignment.Left || ColBarSettings.ColBarPos == Alignment.Right)
                    {
                        while (true)
                        {
                            int allowedColumnsCountForScale = GetAllowedRowsCountForScale(CurrentScale);
                            int num2 = ColumnsCountAssumingScale(CurrentScale);
                            if (num2 <= allowedColumnsCountForScale)
                            {
                                break;
                            }
                            CurrentScale *= 0.95f;
                        }
                        return CurrentScale;
                    }

                    while (true)
                    {
                        int allowedRowsCountForScale = GetAllowedRowsCountForScale(CurrentScale);

                        int rowsCountAssumingScale = RowsCountAssumingScale(CurrentScale);
                        if (rowsCountAssumingScale <= allowedRowsCountForScale)
                        {
                            break;
                        }
                        CurrentScale *= 0.95f;
                    }
                    return CurrentScale;

                }
                return cachedScale;
            }
        }
        private int RowsCountAssumingScale(float scale)
        {
            return Mathf.CeilToInt(cachedDrawLocs.Count / (float)ColonistsPerRowAssumingScale(scale));
        }
        private int ColumnsCountAssumingScale(float scale)
        {
            return Mathf.CeilToInt(cachedDrawLocs.Count / (float)ColonistsPerColumnAssumingScale(scale));
        }
        private static int ColonistsPerRowAssumingScale(float scale)
        {
            if (ColBarSettings.ColBarPos == Alignment.Bottom)
            {
                ColBarSettings.MaxColonistBarWidth = Screen.width - ColBarSettings.MarginLeftHorBottom - ColBarSettings.MarginRightHorBottom;
                ColBarSettings.HorizontalOffset = ColBarSettings.MarginLeftHorBottom / 2 - ColBarSettings.MarginRightHorBottom / 2;

            }
            else
            {
                ColBarSettings.MaxColonistBarWidth = Screen.width - ColBarSettings.MarginLeftHorTop - ColBarSettings.MarginRightHorTop;
                ColBarSettings.HorizontalOffset = ColBarSettings.MarginLeftHorTop / 2 - ColBarSettings.MarginRightHorTop / 2;

            }
            return Mathf.FloorToInt((ColBarSettings.MaxColonistBarWidth) / (SizeAssumingScale(scale).x + SpacingHorizontalAssumingScale(scale) + SpacingHorizontalPSIAssumingScale(scale) + SpacingHorizontalMoodBarAssumingScale(scale)));
        }

        private static float SpacingHorizontalAssumingScale(float scale)
        {
            return ColBarSettings.BaseSpacingHorizontal * scale;
        }

        private static float SpacingVerticalAssumingScale(float scale)
        {
            return ColBarSettings.BaseSpacingVertical * scale;
        }

        private static float SpacingHorizontalPSIAssumingScale(float scale)
        {
            if (ColBarSettings.UsePsi)
                if (ColBarSettings.ColBarPsiIconPos == Alignment.Left || ColBarSettings.ColBarPsiIconPos == Alignment.Right)
                {

                    return ColBarSettings.BaseSizeFloat / ColBarSettings.IconsInColumn * scale * PsiRowsOnBar;
                }
            return 0f;
        }

        private static float SpacingVerticalPSIAssumingScale(float scale)
        {
            if (ColBarSettings.UsePsi)
                if (ColBarSettings.ColBarPsiIconPos == Alignment.Bottom || ColBarSettings.ColBarPsiIconPos == Alignment.Top)
                {
                    return ColBarSettings.BaseSizeFloat / ColBarSettings.IconsInColumn * scale * PsiRowsOnBar;
                }
            return 0f;
        }
        private static float SpacingHorizontalMoodBarAssumingScale(float scale)
        {
            if (ColBarSettings.UseMoodColors && (ColBarSettings.MoodBarPos == Alignment.Left || ColBarSettings.MoodBarPos == Alignment.Right))
                return ColBarSettings.BaseSizeFloat / 4 * scale;

            return 0f;
        }

        private static float SpacingVerticalgMoodBarAssumingScale(float scale)
        {
            if (ColBarSettings.UseMoodColors &&
                (ColBarSettings.MoodBarPos == Alignment.Bottom || ColBarSettings.MoodBarPos == Alignment.Top))
                return ColBarSettings.BaseSizeFloat / 4 * scale;
            return 0f;
        }
        private static Vector2 SizeAssumingScale(float scale)
        {
            BaseSize.x = ColBarSettings.BaseSizeFloat;
            BaseSize.y = ColBarSettings.BaseSizeFloat;
            return BaseSize * scale;
        }

        private static int ColonistsPerColumnAssumingScale(float scale)
        {
            if (ColBarSettings.ColBarPos == Alignment.Right)
            {
                ColBarSettings.MaxColonistBarHeight = Screen.height - ColBarSettings.MarginTopVerRight - ColBarSettings.MarginBottomVerRight;
                ColBarSettings.VerticalOffset = ColBarSettings.MarginTopVerRight / 2 - ColBarSettings.MarginBottomVerRight / 2;
            }
            else
            {
                ColBarSettings.MaxColonistBarHeight = Screen.height - ColBarSettings.MarginTopVerLeft - ColBarSettings.MarginBottomVerLeft;
                ColBarSettings.VerticalOffset = ColBarSettings.MarginTopVerLeft / 2 - ColBarSettings.MarginBottomVerLeft / 2;
            }
            return Mathf.FloorToInt((ColBarSettings.MaxColonistBarHeight) / (SizeAssumingScale(scale).y + SpacingVerticalAssumingScale(scale) + SpacingVerticalPSIAssumingScale(scale)));
        }






        private static int PsiRowsOnBar
        {
            get
            {
                int maxCount = 0;
                foreach (KeyValuePair<Pawn, PawnStats> colonist in PSI.PSI._statsDict)
                {
                    if (colonist.Value.IconCount > maxCount)
                        maxCount = colonist.Value.IconCount;
                }
                int psiRowsOnBar = Mathf.CeilToInt((float)maxCount / ColBarSettings.IconsInColumn);
                return psiRowsOnBar;
            }
        }


        private static int GetAllowedRowsCountForScale(float scale)
        {
            if (ColBarSettings.UseCustomRowCount)
            {
                switch (ColBarSettings.MaxRowsCustom)
                {
                    case 1:
                        {
                            return 1;
                        }
                    case 2:
                        {
                            if (scale > 0.54f)
                            {
                                return 1;
                            }
                            return 2;
                        }
                    case 3:
                        {
                            if (scale > 0.66f)
                            {
                                return 1;
                            }
                            if (scale > 0.54f)
                            {
                                return 2;
                            }
                            return 3;
                        }
                    case 4:
                        {
                            if (scale > 0.78f)
                            {
                                return 1;
                            }
                            if (scale > 0.66f)
                            {
                                return 2;
                            }
                            if (scale > 0.54f)
                            {
                                return 3;
                            }
                            return 4;
                        }
                    case 5:
                        {
                            if (scale > 0.9f)
                            {
                                return 1;
                            }
                            if (scale > 0.78f)
                            {
                                return 2;
                            }
                            if (scale > 0.66f)
                            {
                                return 3;
                            }
                            if (scale > 0.54f)
                            {
                                return 4;
                            }
                            return 5;
                        }

                }
            }


            if (scale > 0.58f)
            {
                return 1;
            }
            if (scale > 0.42f)
            {
                return 2;
            }
            return 3;
        }


        public List<Vector2> DrawLocs
        {
            get
            {
                return cachedDrawLocs;
            }
        }

        public Vector2 Size
        {
            get
            {
                return ColonistBar_KF.BaseSize * Scale;
            }
        }

        public float SpaceBetweenColonistsHorizontal
        {
            get
            {
                return 24f * Scale;
            }
        }

        public static Vector2 PawnTextureSize = new Vector2(ColBarSettings.BaseSizeFloat - 2f, ColBarSettings.BaseSizeFloat * 1.5f);

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public void MarkColonistsDirty()
        {
            entriesDirty = true;
        }

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public void ColonistBarOnGUI()
        {
            //KF
            if (Event.current.type == EventType.Layout)
            {
                BaseSize.x = ColBarSettings.BaseSizeFloat;
                BaseSize.y = ColBarSettings.BaseSizeFloat;
                PawnTextureSize.x = ColBarSettings.BaseSizeFloat - 2f;
                PawnTextureSize.y = ColBarSettings.BaseSizeFloat * 1.5f;
            }
            //end

            if (Event.current.type != EventType.Layout)
            {
                List<ColonistBar.Entry> entries = Entries;
                int num = -1;
                bool showGroupFrames = ShowGroupFrames;
                for (int i = 0; i < cachedDrawLocs.Count; i++)
                {
                    Rect rect = new Rect(cachedDrawLocs[i].x, cachedDrawLocs[i].y, Size.x, Size.y);
                    ColonistBar.Entry entry = entries[i];
                    bool flag = num != entry.group;
                    num = entry.group;
                    if (entry.pawn != null)
                    {
                        drawerKF.HandleClicks(rect, entry.pawn);
                    }
                    if (Event.current.type == EventType.Repaint)
                    {
                        if (flag && showGroupFrames)
                        {
                            drawerKF.DrawGroupFrame(entry.group);
                        }
                        if (entry.pawn != null)
                        {
                            drawerKF.DrawColonist(rect, entry.pawn, entry.map);
                        }
                    }
                }
                num = -1;
                if (showGroupFrames)
                {
                    for (int j = 0; j < cachedDrawLocs.Count; j++)
                    {
                        ColonistBar.Entry entry2 = entries[j];
                        bool flag2 = num != entry2.group;
                        num = entry2.group;
                        if (flag2)
                        {
                            drawerKF.HandleGroupFrameClicks(entry2.group);
                        }
                    }
                }
            }
        }

        private void CheckRecacheEntries()
        {
            if (!entriesDirty)
            {
                return;
            }
            entriesDirty = false;
            cachedEntries.Clear();
            if (Find.PlaySettings.showColonistBar)
            {
                ColonistBar_KF.tmpMaps.Clear();
                ColonistBar_KF.tmpMaps.AddRange(Find.Maps);
                ColonistBar_KF.tmpMaps.SortBy((Map x) => !x.IsPlayerHome, (Map x) => x.uniqueID);
                int num = 0;
                for (int i = 0; i < ColonistBar_KF.tmpMaps.Count; i++)
                {
                    ColonistBar_KF.tmpPawns.Clear();
                    ColonistBar_KF.tmpPawns.AddRange(ColonistBar_KF.tmpMaps[i].mapPawns.FreeColonists);
                    List<Thing> list = ColonistBar_KF.tmpMaps[i].listerThings.ThingsInGroup(ThingRequestGroup.Corpse);
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (!list[j].IsDessicated())
                        {
                            Pawn innerPawn = ((Corpse)list[j]).InnerPawn;
                            if (innerPawn != null)
                            {
                                if (innerPawn.IsColonist)
                                {
                                    ColonistBar_KF.tmpPawns.Add(innerPawn);
                                }
                            }
                        }
                    }
                    List<Pawn> allPawnsSpawned = ColonistBar_KF.tmpMaps[i].mapPawns.AllPawnsSpawned;
                    for (int k = 0; k < allPawnsSpawned.Count; k++)
                    {
                        Corpse corpse = allPawnsSpawned[k].carryTracker.CarriedThing as Corpse;
                        if (corpse != null && !corpse.IsDessicated() && corpse.InnerPawn.IsColonist)
                        {
                            ColonistBar_KF.tmpPawns.Add(corpse.InnerPawn);
                        }
                    }
                    //    ColonistBar_KF.tmpPawns.SortBy((Pawn x) => x.thingIDNumber);
                    SortCachedColonists(ref tmpPawns);
                    for (int l = 0; l < ColonistBar_KF.tmpPawns.Count; l++)
                    {
                        cachedEntries.Add(new ColonistBar.Entry(ColonistBar_KF.tmpPawns[l], ColonistBar_KF.tmpMaps[i], num));
                    }
                    if (!GenCollection.Any<Pawn>(ColonistBar_KF.tmpPawns))
                    {
                        cachedEntries.Add(new ColonistBar.Entry(null, ColonistBar_KF.tmpMaps[i], num));
                    }
                    num++;
                }
                ColonistBar_KF.tmpCaravans.Clear();
                ColonistBar_KF.tmpCaravans.AddRange(Find.WorldObjects.Caravans);
                ColonistBar_KF.tmpCaravans.SortBy((Caravan x) => x.ID);
                for (int m = 0; m < ColonistBar_KF.tmpCaravans.Count; m++)
                {
                    if (ColonistBar_KF.tmpCaravans[m].IsPlayerControlled)
                    {
                        ColonistBar_KF.tmpPawns.Clear();
                        ColonistBar_KF.tmpPawns.AddRange(ColonistBar_KF.tmpCaravans[m].PawnsListForReading);
                        ColonistBar_KF.tmpPawns.SortBy((Pawn x) => x.thingIDNumber);
                        for (int n = 0; n < ColonistBar_KF.tmpPawns.Count; n++)
                        {
                            if (ColonistBar_KF.tmpPawns[n].IsColonist)
                            {
                                cachedEntries.Add(new ColonistBar.Entry(ColonistBar_KF.tmpPawns[n], null, num));
                            }
                        }
                        num++;
                    }
                }
            }
            drawerKF.Notify_RecachedEntries();
            ColonistBar_KF.tmpPawns.Clear();
            ColonistBar_KF.tmpMaps.Clear();
            ColonistBar_KF.tmpCaravans.Clear();
            drawLocsFinderKf.CalculateDrawLocs(cachedDrawLocs, out cachedScale);
        }

        public float GetEntryRectAlpha(Rect rect)
        {
            float t;
            if (Messages.CollidesWithAnyMessage(rect, out t))
            {
                return Mathf.Lerp(1f, 0.2f, t);
            }
            return 1f;
        }

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public bool AnyColonistOrCorpseAt(Vector2 pos)
        {
            ColonistBar.Entry entry;
            return TryGetEntryAt(pos, out entry) && entry.pawn != null;
        }

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public bool TryGetEntryAt(Vector2 pos, out ColonistBar.Entry entry)
        {
            List<Vector2> drawLocs = DrawLocs;
            List<ColonistBar.Entry> entries = Entries;
            Vector2 size = Size;
            for (int i = 0; i < drawLocs.Count; i++)
            {
                Rect rect = new Rect(drawLocs[i].x, drawLocs[i].y, size.x, size.y);
                if (rect.Contains(pos))
                {
                    entry = entries[i];
                    return true;
                }
            }
            entry = default(ColonistBar.Entry);
            return false;
        }

        private void SortCachedColonists(ref List<Pawn> tmpColonists)
        {
            IOrderedEnumerable<Pawn> orderedEnumerable = null;
            switch (ColBarSettings.SortBy)
            {
                case vanilla:
                    tmpColonists.SortBy(x => x.thingIDNumber);
                    SaveBarSettings();
                    break;

                case byName:
                    orderedEnumerable = tmpColonists.OrderBy(x => x.LabelCap != null).ThenBy(x => x.LabelCap);
                    tmpColonists = orderedEnumerable.ToList();
                    SaveBarSettings();
                    break;

                case sexage:
                    orderedEnumerable = tmpColonists.OrderBy(x => x.gender.GetLabel() != null).ThenBy(x => x.gender.GetLabel()).ThenBy(x => x.ageTracker.AgeBiologicalYears);
                    tmpColonists = orderedEnumerable.ToList();
                    SaveBarSettings();
                    break;

                case health:
                    orderedEnumerable = tmpColonists.OrderBy(x => x?.health?.summaryHealth?.SummaryHealthPercent);
                    tmpColonists = orderedEnumerable.ToList();
                    SaveBarSettings();
                    break;

                case mood:
                    orderedEnumerable = tmpColonists.OrderByDescending(x => x?.needs?.mood?.CurLevelPercentage);
                    tmpColonists = orderedEnumerable.ToList();
                    SaveBarSettings();
                    break;

                case weapons:
                    orderedEnumerable = tmpColonists.OrderByDescending(a => a.equipment.Primary != null && a.equipment.Primary.def.IsMeleeWeapon)
                        .ThenByDescending(c => c.equipment.Primary != null && c.equipment.Primary.def.IsRangedWeapon).ThenByDescending(b => b.skills.AverageOfRelevantSkillsFor(WorkTypeDefOf.Hunting));
                    tmpColonists = orderedEnumerable.ToList();
                    SaveBarSettings();
                    break;

                case medic:
                    orderedEnumerable = tmpColonists.OrderByDescending(b => b.skills.AverageOfRelevantSkillsFor(WorkTypeDefOf.Doctor));
                    tmpColonists = orderedEnumerable.ToList();
                    SaveBarSettings();
                    break;

                default:
                    tmpColonists.SortBy(x => x.thingIDNumber);
                    SaveBarSettings();
                    break;
            }
        }


        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public List<Pawn> GetColonistsInOrder()
        {
            List<ColonistBar.Entry> entries = Entries;
            ColonistBar_KF.tmpColonistsInOrder.Clear();
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].pawn != null)
                {
                    ColonistBar_KF.tmpColonistsInOrder.Add(entries[i].pawn);
                }
            }
            return ColonistBar_KF.tmpColonistsInOrder;
        }

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public List<Thing> ColonistsOrCorpsesInScreenRect(Rect rect)
        {
            List<Vector2> drawLocs = DrawLocs;
            List<ColonistBar.Entry> entries = Entries;
            Vector2 size = Size;
            ColonistBar_KF.tmpColonistsWithMap.Clear();
            for (int i = 0; i < drawLocs.Count; i++)
            {
                if (rect.Overlaps(new Rect(drawLocs[i].x, drawLocs[i].y, size.x, size.y)))
                {
                    Pawn pawn = entries[i].pawn;
                    if (pawn != null)
                    {
                        Thing first;
                        if (pawn.Dead && pawn.Corpse != null && pawn.Corpse.MapHeld != null)
                        {
                            first = pawn.Corpse;
                        }
                        else
                        {
                            first = pawn;
                        }
                        ColonistBar_KF.tmpColonistsWithMap.Add(new Pair<Thing, Map>(first, entries[i].map));
                    }
                }
            }
            if (WorldRendererUtility.WorldRenderedNow)
            {
                if (GenCollection.Any(ColonistBar_KF.tmpColonistsWithMap, (Pair<Thing, Map> x) => x.Second == null))
                {
                    ColonistBar_KF.tmpColonistsWithMap.RemoveAll((Pair<Thing, Map> x) => x.Second != null);
                    goto IL_1A1;
                }
            }
            if (GenCollection.Any(ColonistBar_KF.tmpColonistsWithMap, (Pair<Thing, Map> x) => x.Second == Find.VisibleMap))
            {
                ColonistBar_KF.tmpColonistsWithMap.RemoveAll((Pair<Thing, Map> x) => x.Second != Find.VisibleMap);
            }
            IL_1A1:
            ColonistBar_KF.tmpColonists.Clear();
            for (int j = 0; j < ColonistBar_KF.tmpColonistsWithMap.Count; j++)
            {
                ColonistBar_KF.tmpColonists.Add(ColonistBar_KF.tmpColonistsWithMap[j].First);
            }
            ColonistBar_KF.tmpColonistsWithMap.Clear();
            return ColonistBar_KF.tmpColonists;
        }

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public List<Thing> MapColonistsOrCorpsesInScreenRect(Rect rect)
        {
            ColonistBar_KF.tmpMapColonistsOrCorpsesInScreenRect.Clear();
            List<Thing> list = ColonistsOrCorpsesInScreenRect(rect);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Spawned)
                {
                    ColonistBar_KF.tmpMapColonistsOrCorpsesInScreenRect.Add(list[i]);
                }
            }
            return ColonistBar_KF.tmpMapColonistsOrCorpsesInScreenRect;
        }

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public List<Pawn> CaravanMembersInScreenRect(Rect rect)
        {
            ColonistBar_KF.tmpCaravanPawns.Clear();
            List<Thing> list = ColonistsOrCorpsesInScreenRect(rect);
            for (int i = 0; i < list.Count; i++)
            {
                Pawn pawn = list[i] as Pawn;
                if (pawn != null && pawn.IsCaravanMember())
                {
                    ColonistBar_KF.tmpCaravanPawns.Add(pawn);
                }
            }
            return ColonistBar_KF.tmpCaravanPawns;
        }

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public List<Caravan> CaravanMembersCaravansInScreenRect(Rect rect)
        {
            ColonistBar_KF.tmpCaravans.Clear();
            List<Pawn> list = CaravanMembersInScreenRect(rect);
            for (int i = 0; i < list.Count; i++)
            {
                ColonistBar_KF.tmpCaravans.Add(list[i].GetCaravan());
            }
            return ColonistBar_KF.tmpCaravans;
        }

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public Caravan CaravanMemberCaravanAt(Vector2 at)
        {
            Pawn pawn = ColonistOrCorpseAt(at) as Pawn;
            if (pawn != null && pawn.IsCaravanMember())
            {
                return pawn.GetCaravan();
            }
            return null;
        }

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public Thing ColonistOrCorpseAt(Vector2 pos)
        {
            ColonistBar.Entry entry;
            if (!TryGetEntryAt(pos, out entry))
            {
                return null;
            }
            Pawn pawn = entry.pawn;
            Thing result;
            if (pawn != null && pawn.Dead && pawn.Corpse != null && pawn.Corpse.MapHeld != null)
            {
                result = pawn.Corpse;
            }
            else
            {
                result = pawn;
            }
            return result;
        }
    }
}
