using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColonistBarKF.Detouring;
using ColonistBarKF.PSI;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;
using static ColonistBarKF.CBKF;
using static ColonistBarKF.Position;

namespace ColonistBarKF
{
    [StaticConstructorOnStartup]
    public class ColonistBar_KF
    {
        private static ColonistBarDrawLocsFinder_KF drawLocsFinder = new ColonistBarDrawLocsFinder_KF();
        private static ColonistBarColonistDrawer_KF drawer = new ColonistBarColonistDrawer_KF();

        public ColonistBar_KF()
        {
        }

        private const float PawnTextureHorizontalPadding = 1f;

        private static List<ColonistBar.Entry> cachedEntries = new List<ColonistBar.Entry>();

        private static List<Vector2> cachedDrawLocs = new List<Vector2>();

        private static bool entriesDirty = true;

        private static Pawn clickedColonist;

        private static float clickedAt;


        // custom test

        public static  Vector2 BaseSize => new Vector2(ColBarSettings.BaseSizeFloat, ColBarSettings.BaseSizeFloat);

        //      public static readonly Vector2 PawnTextureSize = new Vector2(BaseSize.x - 2f, 75f);



        public static float cachedScale;

        public float Scale
        {
            get
            {
                return cachedScale;
            }
        }
        public Vector2 Size => SizeAssumingScale(Scale);


        public float SpacingHorizontal => SpacingHorizontalAssumingScale(Scale);

        private float SpacingVertical => SpacingVerticalAssumingScale(Scale);

        public float SpacingPSIHorizontal => SpacingHorizontalPSIAssumingScale(Scale);

        private float SpacingPSIVertical => SpacingVerticalPSIAssumingScale(Scale);

        private float SpacingMoodBarVertical => SpacingVerticalgMoodBarAssumingScale(Scale);

        public float SpacingMoodBarHorizontal => SpacingHorizontalMoodBarAssumingScale(Scale);

        private float SpacingHorizontalMoodBarAssumingScale(float scale)
        {
            if (ColBarSettings.UseExternalMoodBar && (ColBarSettings.MoodBarPos == Alignment.Left || ColBarSettings.MoodBarPos == Alignment.Right))
                return ColBarSettings.BaseSizeFloat / 4 * scale;

            return 0f;
        }

        private float SpacingVerticalgMoodBarAssumingScale(float scale)
        {
            if (ColBarSettings.UseExternalMoodBar &&
                (ColBarSettings.MoodBarPos == Alignment.Bottom || ColBarSettings.MoodBarPos == Alignment.Top))
                return ColBarSettings.BaseSizeFloat / 4 * scale;
            return 0f;
        }

        private int ColonistsPerRow => ColonistsPerRowAssumingScale(Scale);

        private Vector2 SizeAssumingScale(float scale)
        {
            return BaseSize * scale;
        }

        private int RowsCountAssumingScale(float scale)
        {
            return Mathf.CeilToInt(cachedDrawLocs.Count / (float)ColonistsPerRowAssumingScale(scale));
        }

        private int ColonistsPerRowAssumingScale(float scale)
        {
            ColBarSettings.MaxColonistBarWidth = Screen.width - ColBarSettings.MarginLeft - ColBarSettings.MarginRight;
            ColBarSettings.HorizontalOffset = ColBarSettings.MarginLeft / 2 - ColBarSettings.MarginRight / 2;

            return Mathf.FloorToInt((ColBarSettings.MaxColonistBarWidth) / (SizeAssumingScale(scale).x + SpacingHorizontalAssumingScale(scale) + SpacingHorizontalPSIAssumingScale(scale) + SpacingHorizontalMoodBarAssumingScale(scale)));
        }

        private  float SpacingHorizontalAssumingScale(float scale)
        {
            return ColBarSettings.BaseSpacingHorizontal * scale;
        }

        private  float SpacingVerticalAssumingScale(float scale)
        {
            return ColBarSettings.BaseSpacingVertical * scale;
        }

        private  float SpacingHorizontalPSIAssumingScale(float scale)
        {
            if (ColBarSettings.UsePsi)
                if (ColBarSettings.ColBarPsiIconPos == Alignment.Left || ColBarSettings.ColBarPsiIconPos == Alignment.Right)
                {
                    return ColBarSettings.BaseSizeFloat / ColBarSettings.IconsInColumn * scale * PsiRowsOnBar;
                }
            return 0f;
        }

        private  float SpacingVerticalPSIAssumingScale(float scale)
        {
            if (ColBarSettings.UsePsi)
                if (ColBarSettings.ColBarPsiIconPos == Alignment.Bottom || ColBarSettings.ColBarPsiIconPos == Alignment.Top)
                {
                    return ColBarSettings.BaseSizeFloat / ColBarSettings.IconsInColumn * scale * PsiRowsOnBar;
                }
            return 0f;
        }

        private static int PsiRowsOnBar
        {
            get
            {
                return 2;

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

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public List<Pawn> GetColonistsInOrder()
        {
            List<ColonistBar.Entry> entries = Entries;
            tmpColonistsInOrder.Clear();
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].pawn != null)
                {
                    tmpColonistsInOrder.Add(entries[i].pawn);
                }
            }
            return tmpColonistsInOrder;
        }

        static Rect ColbarRect = new Rect();

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public void ColonistBarOnGUI()
        {
            if (!Visible)
            {
                return;
            }


            //if (Event.current.type == EventType.Layout)
            //{
            //    BaseSize.x = ColBarSettings.BaseSizeFloat;
            //    BaseSize.y = ColBarSettings.BaseSizeFloat;
            //    PawnTextureSize.x = ColBarSettings.BaseSizeFloat - 2f;
            //    PawnTextureSize.y = ColBarSettings.BaseSizeFloat * 1.5f;
            //
            //    MarkColonistsDirty();
            //    RecacheDrawLocs();
            //}


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
                        HandleClicks(rect, entry.pawn);
                    }
                    if (Event.current.type == EventType.Repaint)
                    {
                        if (flag && showGroupFrames)
                        {
                            DrawGroupFrame(entry.group);
                        }
                        if (entry.pawn != null)
                        {
                            drawer.DrawColonist(rect, entry.pawn, entry.map);
                            if (ColBarSettings.UsePsi)
                            {
                                float entryRectAlpha = GetEntryRectAlpha(rect);
                                drawer.ApplyEntryInAnotherMapAlphaFactor(entry.map, ref entryRectAlpha);
                                PSI.PSI.DrawColonistIcons(entry.pawn, false, entryRectAlpha, rect);
                            }
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
                            HandleGroupFrameClicks(entry2.group);
                        }
                    }
                }

            }
        }

        // RimWorld.ColonistBarColonistDrawer
        public void HandleGroupFrameClicks(int group)
        {
            Rect rect = GroupFrameRect(group);
            if (Mouse.IsOver(rect))
            {
                bool worldRenderedNow = WorldRendererUtility.WorldRenderedNow;
                if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                {
                    Event.current.Use();
                }
                else if (Event.current.type == EventType.MouseUp)
                {
                    if (Event.current.button == 0)
                    {
                        if (!AnyColonistOrCorpseAt(UI.MousePositionOnUIInverted))
                        {
                            if ((!worldRenderedNow && !Find.Selector.dragBox.IsValidAndActive) || (worldRenderedNow && !Find.WorldSelector.dragBox.IsValidAndActive))
                            {
                                Find.Selector.dragBox.active = false;
                                Find.WorldSelector.dragBox.active = false;
                                ColonistBar.Entry entry = Entries.Find(x => x.group == group);
                                Map map = entry.map;
                                if (map == null)
                                {
                                    if (Find.MainTabsRoot.OpenTab == MainButtonDefOf.World)
                                    {
                                        CameraJumper.TrySelect(entry.pawn);
                                    }
                                    else
                                    {
                                        CameraJumper.TryJumpAndSelect(entry.pawn);
                                    }
                                }
                                else
                                {
                                    if (!CameraJumper.TryHideWorld() && Current.Game.VisibleMap != map)
                                    {
                                        SoundDefOf.MapSelected.PlayOneShotOnCamera();
                                    }
                                    Current.Game.VisibleMap = map;
                                }
                            }
                        }
                    }
                    //else if (Event.current.button == 1)
                    //{
                    //    ColonistBar.Entry entry2 = Entries.Find(x => x.group == group);
                    //    if (entry2.map != null)
                    //    {
                    //        CameraJumper.TryJumpAndSelect(CameraJumper.GetGlobalTargetInfoForMap(entry2.map));
                    //    }
                    //    else if (entry2.pawn != null)
                    //    {
                    //        CameraJumper.TryJumpAndSelect(entry2.pawn);
                    //    }
                    //}
                }
            }
        }

        public bool AnyColonistOrCorpseAt(Vector2 pos)
        {
            ColonistBar.Entry entry;
            return TryGetEntryAt(pos, out entry) && entry.pawn != null;
        }

        // RimWorld.ColonistBar
        public bool TryGetEntryAt(Vector2 pos, out ColonistBar.Entry entry)
        {
            List<Vector2> drawLocs = cachedDrawLocs;
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


        // RimWorld.ColonistBarColonistDrawer
        private Rect GroupFrameRect(int group)
        {
            float pos_x = 99999f;
            float pos_y = 99999f;
            float width = 0f;
            float height = 0f;
            List<ColonistBar.Entry> entries = Entries;
            List<Vector2> drawLocs = cachedDrawLocs;
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].group == group)
                {
                    pos_x = Mathf.Min(pos_x, drawLocs[i].x);
                    pos_y = Mathf.Min(pos_y, drawLocs[i].y);
                    width = Mathf.Max(width, drawLocs[i].x + Size.x);
                    height = Mathf.Max(height, drawLocs[i].y + Size.y);
                }
            }
            return new Rect(pos_x - ColBarSettings.BaseSizeFloat / 4, pos_y - ColBarSettings.BaseSizeFloat / 8, width - pos_x + ColBarSettings.BaseSizeFloat / 2, height).ContractedBy(-12f * Scale);
        }



        // RimWorld.ColonistBarColonistDrawer
        public void DrawGroupFrame(int group)
        {
            Rect position = GroupFrameRect(group);
            List<ColonistBar.Entry> entries = Entries;
            Map map = entries.Find(x => x.group == group).map;
            float alpha;
            if (map == null)
            {
                if (WorldRendererUtility.WorldRenderedNow)
                {
                    alpha = 1f;
                }
                else
                {
                    alpha = 0.75f;
                }
            }
            else if (map != Find.VisibleMap || WorldRendererUtility.WorldRenderedNow)
            {
                alpha = 0.75f;
            }
            else
            {
                alpha = 1f;
            }
            Widgets.DrawRectFast(position, new Color(0.5f, 0.5f, 0.5f, 0.4f * alpha), null);
        }


        // RimWorld.ColonistBar
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


        // RimWorld.ColonistBar
        // ReSharper disable once UnusedMember.Global
        // RimWorld.ColonistBar
        public List<Thing> ColonistsOrCorpsesInScreenRect(Rect rect)
        {
            List<Vector2> drawLocs = cachedDrawLocs;
            List<ColonistBar.Entry> entries = Entries;
            Vector2 size = Size;
            tmpColonistsWithMap.Clear();
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
                        tmpColonistsWithMap.Add(new Pair<Thing, Map>(first, entries[i].map));
                    }
                }
            }
            if (WorldRendererUtility.WorldRenderedNow)
            {
                if (tmpColonistsWithMap.Any(x => x.Second == null))
                {
                    tmpColonistsWithMap.RemoveAll(x => x.Second != null);
                    goto IL_1A1;
                }
            }
            if (tmpColonistsWithMap.Any(x => x.Second == Find.VisibleMap))
            {
                tmpColonistsWithMap.RemoveAll(x => x.Second != Find.VisibleMap);
            }
            IL_1A1:
            tmpColonists.Clear();
            for (int j = 0; j < tmpColonistsWithMap.Count; j++)
            {
                tmpColonists.Add(tmpColonistsWithMap[j].First);
            }
            tmpColonistsWithMap.Clear();
            return tmpColonists;
        }

        public static List<ColonistBar.Entry> Entries
        {
            get
            {
                CheckRecacheEntries();
                return cachedEntries;
            }
        }
        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public Thing ColonistOrCorpseAt(Vector2 pos)
        {
            if (!Visible)
            {
                return null;
            }
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
/*
        public void RecacheDrawLocs()
        {
            CheckRecacheEntries();
            Vector2 size = Size;
            int colonistsPerRow = ColonistsPerRow;
            float spacingHorizontal = SpacingHorizontal + SpacingPSIHorizontal + SpacingMoodBarHorizontal;
            float spacingVertical = SpacingVertical + SpacingPSIVertical + SpacingMoodBarVertical;


            float cachedDrawLocs_x = 0f + ColBarSettings.MarginLeft * Scale;
            float cachedDrawLocs_y = ColBarSettings.MarginTop * Scale;


            cachedDrawLocs.Clear();

            #region Vertical

            for (int i = 0; i < cachedEntries.Count; i++)
            {
                if (i % colonistsPerRow == 0)
                {
                    int maxColInRow = Mathf.Min(colonistsPerRow, cachedEntries.Count - i);
                    float num4 = maxColInRow * size.x + (maxColInRow - 1) * spacingHorizontal;
                    cachedDrawLocs_x = (Screen.width - num4) / 2f + ColBarSettings.HorizontalOffset;

                    if (ColBarSettings.UsePsi)
                        ModifyBasicDrawLocsForPsi(size, ref cachedDrawLocs_x, ref cachedDrawLocs_y);

                    if (ColBarSettings.UseExternalMoodBar)
                        ModifyBasicDrawLocsForMoodBar(size, ref cachedDrawLocs_x, ref cachedDrawLocs_y);


                    if (i != 0)
                        cachedDrawLocs_y += size.y + spacingVertical + SpacingLabel;


                }
                else
                {
                    cachedDrawLocs_x += size.x + spacingHorizontal;
                }
                cachedDrawLocs.Add(new Vector2(cachedDrawLocs_x, cachedDrawLocs_y));
            }


            #endregion


        }
*/
        private static void ModifyBasicDrawLocsForPsi(Vector2 size, ref float cachedDrawLocs_x, ref float cachedDrawLocs_y)
        {
            switch (ColBarSettings.ColBarPsiIconPos)
            {
                case Alignment.Left:
                    cachedDrawLocs_x += size.x / ColBarSettings.IconsInColumn * PsiRowsOnBar / 2;
                    break;

                case Alignment.Right:
                    cachedDrawLocs_x -= size.x / ColBarSettings.IconsInColumn * PsiRowsOnBar / 2;
                    break;

                case Alignment.Bottom:
                    //      cachedDrawLocs_y -= size.y/ColBarSettings.IconsInColumn*PsiRowsOnBar;
                    break;
                case Alignment.Top:
                    cachedDrawLocs_y += size.y / ColBarSettings.IconsInColumn * PsiRowsOnBar;
                    break;
            }
        }

        private static void ModifyBasicDrawLocsForMoodBar(Vector2 size, ref float cachedDrawLocs_x, ref float cachedDrawLocs_y)
        {
            switch (ColBarSettings.MoodBarPos)
            {
                case Alignment.Left:
                    cachedDrawLocs_x += size.x / 8;
                    break;

                case Alignment.Right:
                    cachedDrawLocs_x -= size.x / 8;
                    break;

                case Alignment.Top:
                    cachedDrawLocs_y += size.y / 8;
                    break;

                case Alignment.Bottom:
                    //      cachedDrawLocs_y -= size.y/ColBarSettings.IconsInColumn*PsiRowsOnBar;
                    break;
            }
        }


        private static void CheckRecacheEntries()
        {
            if (!entriesDirty)
            {
                return;
            }
            entriesDirty = false;
            cachedEntries.Clear();
            if (Find.PlaySettings.showColonistBar)
            {
                tmpMaps.Clear();
                tmpMaps.AddRange(Find.Maps);
                tmpMaps.SortBy(x => !x.IsPlayerHome, x => x.uniqueID);
                int num = 0;
                for (int i = 0; i < tmpMaps.Count; i++)
                {
                    tmpPawns.Clear();
                    tmpPawns.AddRange(tmpMaps[i].mapPawns.FreeColonists);
                    List<Thing> list = tmpMaps[i].listerThings.ThingsInGroup(ThingRequestGroup.Corpse);
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (!list[j].IsDessicated())
                        {
                            Pawn innerPawn = ((Corpse)list[j]).InnerPawn;
                            if (innerPawn != null)
                            {
                                if (innerPawn.IsColonist)
                                {
                                    tmpPawns.Add(innerPawn);
                                }
                            }
                        }
                    }
                    List<Pawn> allPawnsSpawned = tmpMaps[i].mapPawns.AllPawnsSpawned;
                    for (int k = 0; k < allPawnsSpawned.Count; k++)
                    {
                        Corpse corpse = allPawnsSpawned[k].carryTracker.CarriedThing as Corpse;
                        if (corpse != null && !corpse.IsDessicated() && corpse.InnerPawn.IsColonist)
                        {
                            tmpPawns.Add(corpse.InnerPawn);
                        }
                    }
                    //         tmpPawns.SortBy((Pawn x) => x.thingIDNumber);
                    SortCachedColonists(ref tmpPawns);
                    for (int l = 0; l < tmpPawns.Count; l++)
                    {
                        cachedEntries.Add(new ColonistBar.Entry(tmpPawns[l], tmpMaps[i], num));
                    }
                    if (!tmpPawns.Any())
                    {
                        cachedEntries.Add(new ColonistBar.Entry(null, tmpMaps[i], num));
                    }
                    num++;
                }
                tmpCaravans.Clear();
                tmpCaravans.AddRange(Find.WorldObjects.Caravans);
                tmpCaravans.SortBy(x => x.ID);
                for (int m = 0; m < tmpCaravans.Count; m++)
                {
                    if (tmpCaravans[m].IsPlayerControlled)
                    {
                        tmpPawns.Clear();
                        tmpPawns.AddRange(tmpCaravans[m].PawnsListForReading);
                        //  tmpPawns.SortBy((Pawn x) => x.thingIDNumber);
                        SortCachedColonists(ref tmpPawns);
                        for (int n = 0; n < tmpPawns.Count; n++)
                        {
                            if (tmpPawns[n].IsColonist)
                            {
                                cachedEntries.Add(new ColonistBar.Entry(tmpPawns[n], null, num));
                            }
                        }
                        num++;
                    }
                }
            }
            //        RecacheDrawLocs();
            drawer.Notify_RecachedEntries();
            tmpPawns.Clear();
            tmpMaps.Clear();
            tmpCaravans.Clear();
            drawLocsFinder.CalculateDrawLocs(cachedDrawLocs, out cachedScale);
        }

        private static List<int> entriesInGroup = new List<int>();
        private static List<int> horizontalSlotsPerGroup = new List<int>();



        private static void SortCachedColonists(ref List<Pawn> tmpColonists)
        {
            IOrderedEnumerable<Pawn> orderedEnumerable = null;
            switch (ColBarSettings.SortBy)
            {
                case SettingsColonistBar.SortByWhat.vanilla:
                    tmpColonists.SortBy(x => x.thingIDNumber);
                    SaveBarSettings();
                    break;

                case SettingsColonistBar.SortByWhat.byName:
                    orderedEnumerable = tmpColonists.OrderBy(x => x.LabelCap != null).ThenBy(x => x.LabelCap);
                    tmpColonists = orderedEnumerable.ToList();
                    SaveBarSettings();
                    break;

                case SettingsColonistBar.SortByWhat.sexage:
                    orderedEnumerable = tmpColonists.OrderBy(x => x.gender.GetLabel() != null).ThenBy(x => x.gender.GetLabel()).ThenBy(x => x.ageTracker.AgeBiologicalYears);
                    tmpColonists = orderedEnumerable.ToList();
                    SaveBarSettings();
                    break;

                case SettingsColonistBar.SortByWhat.health:
                    orderedEnumerable = tmpColonists.OrderBy(x => x?.health?.summaryHealth?.SummaryHealthPercent);
                    tmpColonists = orderedEnumerable.ToList();
                    SaveBarSettings();
                    break;

                case SettingsColonistBar.SortByWhat.mood:
                    orderedEnumerable = tmpColonists.OrderByDescending(x => x?.needs?.mood?.CurLevelPercentage);
                    tmpColonists = orderedEnumerable.ToList();
                    SaveBarSettings();
                    break;

                case SettingsColonistBar.SortByWhat.weapons:
                    orderedEnumerable = tmpColonists.OrderByDescending(a => a.equipment.Primary != null && a.equipment.Primary.def.IsMeleeWeapon)
                        .ThenByDescending(c => c.equipment.Primary != null && c.equipment.Primary.def.IsRangedWeapon).ThenByDescending(b => b.skills.AverageOfRelevantSkillsFor(WorkTypeDefOf.Hunting));
                    tmpColonists = orderedEnumerable.ToList();
                    SaveBarSettings();
                    break;

                case SettingsColonistBar.SortByWhat.medic:
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



        // RimWorld.ColonistBarColonistDrawer



        public float GetEntryRectAlpha(Rect rect)
        {
            float t;
            if (Messages.CollidesWithAnyMessage(rect, out t))
            {
                return Mathf.Lerp(1f, 0.2f, t);
            }
            return 1f;
        }




        private static Pawn SelPawn => Find.Selector.SingleSelectedThing as Pawn;

        private static int clickCount;

        private void HandleClicks(Rect rect, Pawn colonist)
        {
            if (Mouse.IsOver(rect))
            {
                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        {
                            switch (Event.current.button)
                            {
                                case 0:
                                    {
                                        if (clickedColonist == colonist && Time.time - clickedAt < ColBarSettings.DoubleClickTime)
                                        {
                                            // use event so it doesn't bubble through
                                            Event.current.Use();

                                            if (FollowMe.CurrentlyFollowing)
                                            {
                                                FollowMe.StopFollow("Selected another colonist on bar");
                                                FollowMe.TryStartFollow(colonist);
                                            }
                                            else
                                            {
                                                CameraJumper.TryJump(colonist);
                                            }
                                            clickedColonist = null;
                                        }
                                        else
                                        {
                                            clickedColonist = colonist;
                                            clickedAt = Time.time;
                                            clickCount++;
                                        }
                                        break;
                                    }

                                case 1:
                                    {
                                        List<FloatMenuOption> floatOptionList = new List<FloatMenuOption>();

                                        if (clickedColonist != null && SelPawn != null && SelPawn != clickedColonist)
                                        {
                                            foreach (FloatMenuOption choice in FloatMenuMakerMap.ChoicesAtFor(clickedColonist.TrueCenter(), SelPawn))
                                            {
                                                floatOptionList.Add(choice);
                                            }
                                            if (floatOptionList.Any())
                                                floatOptionList.Add(new FloatMenuOption("--------------------", delegate
                                                {
                                                }));

                                        }
                                        if (!FollowMe.CurrentlyFollowing)
                                        {
                                            floatOptionList.Add(new FloatMenuOption("FollowMe.StartFollow".Translate(),
                                                delegate { FollowMe.TryStartFollow(colonist); }));
                                        }
                                        else
                                        {
                                            floatOptionList.Add(new FloatMenuOption("FollowMe.StopFollow".Translate(),
                                                delegate { FollowMe.StopFollow("Canceled in dropdown"); }));
                                        }
                                        floatOptionList.Add(new FloatMenuOption("CBKF.Settings.Vanilla".Translate(), delegate
                                        {
                                            ColBarSettings.SortBy = SettingsColonistBar.SortByWhat.vanilla;
                                            MarkColonistsDirty();
                                            //           CheckRecacheEntries();
                                        }));
                                        floatOptionList.Add(new FloatMenuOption("CBKF.Settings.ByName".Translate(), delegate
                                        {
                                            ColBarSettings.SortBy = SettingsColonistBar.SortByWhat.byName;
                                            MarkColonistsDirty();
                                            //            CheckRecacheEntries();
                                        }));

                                        floatOptionList.Add(new FloatMenuOption("CBKF.Settings.SexAge".Translate(), delegate
                                        {
                                            ColBarSettings.SortBy = SettingsColonistBar.SortByWhat.sexage;
                                            MarkColonistsDirty();
                                            //          CheckRecacheEntries();
                                        }));

                                        floatOptionList.Add(new FloatMenuOption("CBKF.Settings.Mood".Translate(), delegate
                                        {
                                            ColBarSettings.SortBy = SettingsColonistBar.SortByWhat.mood;
                                            MarkColonistsDirty();
                                            //        CheckRecacheEntries();
                                        }));
                                        floatOptionList.Add(new FloatMenuOption("CBKF.Settings.Health".Translate(), delegate
                                        {
                                            ColBarSettings.SortBy = SettingsColonistBar.SortByWhat.health;
                                            MarkColonistsDirty();
                                            //    CheckRecacheEntries();
                                        }));
                                        floatOptionList.Add(new FloatMenuOption("CBKF.Settings.Medic".Translate(), delegate
                                        {
                                            ColBarSettings.SortBy = SettingsColonistBar.SortByWhat.medic;
                                            MarkColonistsDirty();
                                            //  CheckRecacheEntries();
                                        }));
                                        floatOptionList.Add(new FloatMenuOption("CBKF.Settings.Weapons".Translate(), delegate
                                        {
                                            ColBarSettings.SortBy = SettingsColonistBar.SortByWhat.weapons;
                                            MarkColonistsDirty();
                                            //      CheckRecacheEntries();
                                        }));

                                        floatOptionList.Add(new FloatMenuOption("CBKF.Settings.SettingsColonistBar".Translate(), delegate { Find.WindowStack.Add(new ColonistBarKF_Settings()); }));
                                        FloatMenu window = new FloatMenu(floatOptionList, "CBKF.Settings.SortingOptions".Translate());
                                        Find.WindowStack.Add(window);

                                        // use event so it doesn't bubble through
                                        Event.current.Use();
                                        break;
                                    }
                            }
                            break;
                        }
                }

                if (Event.current.type == EventType.mouseUp && Event.current.button == 2)
                {

                    // start following
                    if (FollowMe.CurrentlyFollowing)
                    {
                        FollowMe.StopFollow("Canceled by user");
                    }
                    else
                    {
                        FollowMe.TryStartFollow(colonist);
                    }

                    // use event so it doesn't bubble through
                    Event.current.Use();

                }
            }


        }

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public  void MarkColonistsDirty()
        {
            entriesDirty = true;
            // added
            CheckRecacheEntries();
        }




        private static List<Pawn> tmpPawns = new List<Pawn>();

        private static List<Map> tmpMaps = new List<Map>();

        private static List<Caravan> tmpCaravans = new List<Caravan>();

        private static List<Pawn> tmpColonistsInOrder = new List<Pawn>();

        private static List<Pair<Thing, Map>> tmpColonistsWithMap = new List<Pair<Thing, Map>>();

        private static List<Thing> tmpColonists = new List<Thing>();

        private static List<Thing> tmpMapColonistsOrCorpsesInScreenRect = new List<Thing>();

        private static List<Pawn> tmpCaravanPawns = new List<Pawn>();
        // RimWorld.ColonistBar
        private static bool Visible
        {
            get
            {
                return UI.screenWidth >= 800 && UI.screenHeight >= 500;
            }
        }



        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public Caravan CaravanMemberCaravanAt(Vector2 at)
        {
            if (!Visible)
            {
                return null;
            }
            Pawn pawn = ColonistOrCorpseAt(at) as Pawn;
            if (pawn != null && pawn.IsCaravanMember())
            {
                return pawn.GetCaravan();
            }
            return null;
        }

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public List<Caravan> CaravanMembersCaravansInScreenRect(Rect rect)
        {
            tmpCaravans.Clear();
            if (!Visible)
            {
                return tmpCaravans;
            }
            List<Pawn> list = CaravanMembersInScreenRect(rect);
            for (int i = 0; i < list.Count; i++)
            {
                tmpCaravans.Add(list[i].GetCaravan());
            }
            return tmpCaravans;
        }

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public List<Pawn> CaravanMembersInScreenRect(Rect rect)
        {
            tmpCaravanPawns.Clear();
            if (!Visible)
            {
                return tmpCaravanPawns;
            }
            List<Thing> list = ColonistsOrCorpsesInScreenRect(rect);
            for (int i = 0; i < list.Count; i++)
            {
                Pawn pawn = list[i] as Pawn;
                if (pawn != null && pawn.IsCaravanMember())
                {
                    tmpCaravanPawns.Add(pawn);
                }
            }
            return tmpCaravanPawns;
        }

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public List<Thing> MapColonistsOrCorpsesInScreenRect(Rect rect)
        {
            tmpMapColonistsOrCorpsesInScreenRect.Clear();
            if (!Visible)
            {
                return tmpMapColonistsOrCorpsesInScreenRect;
            }
            List<Thing> list = ColonistsOrCorpsesInScreenRect(rect);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Spawned)
                {
                    tmpMapColonistsOrCorpsesInScreenRect.Add(list[i]);
                }
            }
            return tmpMapColonistsOrCorpsesInScreenRect;
        }

    }
}
