using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public static class ColonistBar_KF
    {

        private const float PawnTextureHorizontalPadding = 1f;

        private static List<ColonistBar.Entry> cachedEntries = new List<ColonistBar.Entry>();

        private static List<Vector2> cachedDrawLocs = new List<Vector2>();

        private static bool entriesDirty = true;

        private static Dictionary<string, string> pawnLabelsCache = new Dictionary<string, string>();

        private static Pawn clickedColonist;

        private static float clickedAt;

        public static float SpacingLabel = 20f;

        // custom test

        public static Vector2 BaseSize = new Vector2(ColBarSettings.BaseSizeFloat, ColBarSettings.BaseSizeFloat);

        //      public static readonly Vector2 PawnTextureSize = new Vector2(BaseSize.x - 2f, 75f);
        public static Vector2 PawnTextureSize = new Vector2(ColBarSettings.BaseSizeFloat - 2f, ColBarSettings.BaseSizeFloat * 1.5f);

        private static Vector3 _pawnTextureCameraOffset;

        public static Vector3 PawnTextureCameraOffset
        {
            get
            {
                float pawnTextureCameraOffsetNew = ColBarSettings.PawnTextureCameraZoom / 1.28205f;
                _pawnTextureCameraOffset = new Vector3(ColBarSettings.PawnTextureCameraHorizontalOffset / pawnTextureCameraOffsetNew, 0f, ColBarSettings.PawnTextureCameraVerticalOffset / pawnTextureCameraOffsetNew);
                return _pawnTextureCameraOffset;
            }

            set { _pawnTextureCameraOffset = value; }
        }

        public static float CurrentScale;

        private static float Scale
        {
            get
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
        }

        private static Vector2 Size => SizeAssumingScale(Scale);

        private static float SpacingHorizontal => SpacingHorizontalAssumingScale(Scale);

        private static float SpacingVertical => SpacingVerticalAssumingScale(Scale);

        private static float SpacingPSIHorizontal => SpacingHorizontalPSIAssumingScale(Scale);

        private static float SpacingPSIVertical => SpacingVerticalPSIAssumingScale(Scale);

        private static float SpacingMoodBarVertical => SpacingVerticalgMoodBarAssumingScale(Scale);

        private static float SpacingMoodBarHorizontal => SpacingHorizontalMoodBarAssumingScale(Scale);

        private static float SpacingHorizontalMoodBarAssumingScale(float scale)
        {
            if (ColBarSettings.UseExternalMoodBar && (ColBarSettings.MoodBarPos == Alignment.Left || ColBarSettings.MoodBarPos == Alignment.Right))
                return ColBarSettings.BaseSizeFloat / 4 * scale;

            return 0f;
        }

        private static float SpacingVerticalgMoodBarAssumingScale(float scale)
        {
            if (ColBarSettings.UseExternalMoodBar &&
                (ColBarSettings.MoodBarPos == Alignment.Bottom || ColBarSettings.MoodBarPos == Alignment.Top))
                return ColBarSettings.BaseSizeFloat / 4 * scale;
            return 0f;
        }

        private static int ColonistsPerRow => ColonistsPerRowAssumingScale(Scale);

        private static int ColonistsPerColumn => ColonistsPerColumnAssumingScale(Scale);

        private static Vector2 SizeAssumingScale(float scale)
        {
            BaseSize.x = ColBarSettings.BaseSizeFloat;
            BaseSize.y = ColBarSettings.BaseSizeFloat;
            return BaseSize * scale;
        }

        private static int RowsCountAssumingScale(float scale)
        {
            return Mathf.CeilToInt(cachedDrawLocs.Count / (float)ColonistsPerRowAssumingScale(scale));
        }

        private static int ColumnsCountAssumingScale(float scale)
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

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public static List<Pawn> GetColonistsInOrder()
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

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once InconsistentNaming
        public static void ColonistBarOnGUI()
        {
            if (!Visible)
            {
                return;
            }


            if (Event.current.type == EventType.Layout)
            {
                BaseSize.x = ColBarSettings.BaseSizeFloat;
                BaseSize.y = ColBarSettings.BaseSizeFloat;
                PawnTextureSize.x = ColBarSettings.BaseSizeFloat - 2f;
                PawnTextureSize.y = ColBarSettings.BaseSizeFloat * 1.5f;

                RecacheDrawLocs();
            }


            if (Event.current.type != EventType.Layout)
            {
                List<Vector2> drawLocs = cachedDrawLocs;
                List<ColonistBar.Entry> entries = Entries;
                int num = -1;
                bool showGroupFrames = ShowGroupFrames;
                for (int i = 0; i < drawLocs.Count; i++)
                {
                    Rect rect = new Rect(drawLocs[i].x, drawLocs[i].y, Size.x, Size.y);
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
                            DrawColonist(rect, entry.pawn, entry.map);
                            if (ColBarSettings.UsePsi)
                            {
                                float entryRectAlpha = GetEntryRectAlpha(rect);
                                ApplyEntryInAnotherMapAlphaFactor(entry.map, ref entryRectAlpha);
                                PSI.PSI.DrawColonistIconsOnBar(rect, entry.pawn, entryRectAlpha);
                            }
                        }
                    }
                }
                num = -1;
                if (showGroupFrames)
                {
                    for (int j = 0; j < drawLocs.Count; j++)
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
        public static void HandleGroupFrameClicks(int group)
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
                                ColonistBar.Entry entry = Entries.Find(x => x.@group == @group);
                                Map map = entry.map;
                                if (map == null)
                                {
                                    if (Find.MainTabsRoot.OpenTab == MainTabDefOf.World)
                                    {
                                        JumpToTargetUtility.TrySelect(entry.pawn);
                                    }
                                    else
                                    {
                                        JumpToTargetUtility.TryJumpAndSelect(entry.pawn);
                                    }
                                }
                                else
                                {
                                    if (!JumpToTargetUtility.CloseWorldTab() && Current.Game.VisibleMap != map)
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
                    //        JumpToTargetUtility.TryJumpAndSelect(JumpToTargetUtility.GetGlobalTargetInfoForMap(entry2.map));
                    //    }
                    //    else if (entry2.pawn != null)
                    //    {
                    //        JumpToTargetUtility.TryJumpAndSelect(entry2.pawn);
                    //    }
                    //}
                }
            }
        }

        public static bool AnyColonistOrCorpseAt(Vector2 pos)
        {
            ColonistBar.Entry entry;
            return TryGetEntryAt(pos, out entry) && entry.pawn != null;
        }

        // RimWorld.ColonistBar
        public static bool TryGetEntryAt(Vector2 pos, out ColonistBar.Entry entry)
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
        private static Rect GroupFrameRect(int group)
        {
            float pos_x = 99999f;
            float width = 0f;
            float height = 0f;
            List<ColonistBar.Entry> entries = Entries;
            List<Vector2> drawLocs = cachedDrawLocs;
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].group == group)
                {
                    pos_x = Mathf.Min(pos_x, drawLocs[i].x);
                    width = Mathf.Max(width, drawLocs[i].x + Size.x);
                    height = Mathf.Max(height, drawLocs[i].y + Size.y);
                }
            }
            return new Rect(pos_x, 0f, width - pos_x, height).ContractedBy(-12f * Scale);
        }



        // RimWorld.ColonistBarColonistDrawer
        public static void DrawGroupFrame(int group)
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
        private static bool ShowGroupFrames
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
        public static List<Thing> ColonistsOrCorpsesInScreenRect(Rect rect)
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
        public static Thing ColonistOrCorpseAt(Vector2 pos)
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

        public static void RecacheDrawLocs()
        {
            CheckRecacheEntries();
            Vector2 size = Size;
            int colonistsPerRow = ColonistsPerRow;
            int colonistsPerColumn = ColonistsPerColumn;
            float spacingHorizontal = SpacingHorizontal + SpacingPSIHorizontal + SpacingMoodBarHorizontal;
            float spacingVertical = SpacingVertical + SpacingPSIVertical + SpacingMoodBarVertical;

            float cachedDrawLocs_x = 0f + ColBarSettings.MarginLeftHorTop * Scale;
            float cachedDrawLocs_y = ColBarSettings.MarginTopHor * Scale;

            cachedDrawLocs.Clear();

            #region Horizontal Alignment

            if (ColBarSettings.ColBarPos == Alignment.Left || ColBarSettings.ColBarPos == Alignment.Right)
                for (int i = 0; i < cachedEntries.Count; i++)
                {
                    //         Debug.Log("Colonists count: " + i);
                    if (i % colonistsPerColumn == 0)
                    {
                        int maxColInColumn = Mathf.Min(colonistsPerColumn, cachedEntries.Count - i);
                        float num4 = maxColInColumn * size.y + (maxColInColumn - 1) * (spacingVertical + SpacingLabel);
                        cachedDrawLocs_y = (Screen.height - num4) / 2f + ColBarSettings.VerticalOffset;

                        if (ColBarSettings.UsePsi)
                            ModifyBasicDrawLocsForPsi(size, ref cachedDrawLocs_x, ref cachedDrawLocs_y);
                        if (ColBarSettings.UseExternalMoodBar)
                            ModifyBasicDrawLocsForMoodBar(size, ref cachedDrawLocs_x, ref cachedDrawLocs_y);

                        if (i != 0)
                        {
                            if (ColBarSettings.ColBarPos == Alignment.Right)
                            {
                                cachedDrawLocs_x -= size.x + spacingHorizontal;
                            }
                            else
                            {
                                cachedDrawLocs_x += size.x + spacingHorizontal;
                            }
                        }
                        //         Debug.Log("maxColInColumn " + maxColInColumn);
                    }
                    else
                    {
                        cachedDrawLocs_y += size.y + spacingVertical + SpacingLabel;
                    }
                    cachedDrawLocs.Add(new Vector2(cachedDrawLocs_x, cachedDrawLocs_y));

                    //      Debug.Log("MaxColonistBarHeight:" + SettingsMaxColonistBarHeight+ " + SpacingVerticalAssumingScale(1f): "+ SpacingVerticalAssumingScale(1f) + " / (SizeAssumingScale(1f).y: "+ SizeAssumingScale(1f).y + " + SpacingVerticalAssumingScale(1f): "+ SpacingVerticalAssumingScale(1f));
                    //
                    //      Debug.Log("colonistsPerRow " + colonistsPerRow);
                    //      Debug.Log("colonistsPerColumn " + colonistsPerColumn);
                    //      Debug.Log("cachedDrawLocs_x: " + cachedDrawLocs_x);
                    //      Debug.Log("cachedDrawLocs_y: " + cachedDrawLocs_y);
                    //      Debug.Log("cachedEntries: " + i);
                }
            #endregion
            #region Vertical
            else
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
                        {
                            if (ColBarSettings.ColBarPos == Alignment.Bottom)
                            {
                                cachedDrawLocs_y -= size.y + spacingVertical + SpacingLabel;
                            }
                            else
                            {
                                cachedDrawLocs_y += size.y + spacingVertical + SpacingLabel;
                            }
                        }
                    }
                    else
                    {
                        cachedDrawLocs_x += size.x + spacingHorizontal;
                    }
                    cachedDrawLocs.Add(new Vector2(cachedDrawLocs_x, cachedDrawLocs_y));
                }


            #endregion


        }

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
            RecacheDrawLocs();
            Notify_RecachedEntries();
            tmpPawns.Clear();
            tmpMaps.Clear();
            tmpCaravans.Clear();
            CalculateColonistsInGroup();
        }

        private static List<int> entriesInGroup = new List<int>();

        private static void CalculateColonistsInGroup()
        {
            entriesInGroup.Clear();
            List<ColonistBar.Entry> entries = Entries;
            int num = CalculateGroupsCount();
            for (int i = 0; i < num; i++)
            {
                entriesInGroup.Add(0);
            }
            for (int j = 0; j < entries.Count; j++)
            {
                List<int> list;
                List<int> expr_49 = list = entriesInGroup;
                int num2;
                int expr_5C = num2 = entries[j].group;
                num2 = list[num2];
                expr_49[expr_5C] = num2 + 1;
            }
        }

        // RimWorld.ColonistBarDrawLocsFinder
        private static int CalculateGroupsCount()
        {
            List<ColonistBar.Entry> entries = Entries;
            int num = -1;
            int num2 = 0;
            for (int i = 0; i < entries.Count; i++)
            {
                if (num != entries[i].group)
                {
                    num2++;
                    num = entries[i].group;
                }
            }
            return num2;
        }


        public static void Notify_RecachedEntries()
        {
            pawnLabelsCache.Clear();
        }
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

        public static void DrawColonist(Rect rect, Pawn colonist, Map pawnMap)
        {
            float entryRectAlpha = GetEntryRectAlpha(rect);
            ApplyEntryInAnotherMapAlphaFactor(pawnMap, ref entryRectAlpha);

            bool colonistAlive = (!colonist.Dead) ? Find.Selector.SelectedObjects.Contains(colonist) : Find.Selector.SelectedObjects.Contains(colonist.Corpse);

            Color color = new Color(1f, 1f, 1f, entryRectAlpha);
            GUI.color = color;

            Color BGColor = new Color();

            Need_Mood mood;
            MentalBreaker mb;

            if (colonist.needs != null && colonist.needs.mood != null)
            {
                mb = colonist.mindState.mentalBreaker;
                mood = colonist.needs.mood;
            }
            else
            {
                mood = null;
                mb = null;
            }

            if (ColBarSettings.UseExternalMoodBar)
            {
                // draw mood border
                Rect moodBorderRect = new Rect(rect);


                switch (ColBarSettings.MoodBarPos)
                {
                    case Alignment.Right:
                        moodBorderRect.x = rect.xMax;
                        moodBorderRect.width /= 4;
                        break;
                    case Alignment.Left:
                        moodBorderRect.x = rect.xMin - rect.width / 4;
                        moodBorderRect.width /= 4;
                        break;
                    case Alignment.Top:
                        moodBorderRect.x = rect.xMin;
                        moodBorderRect.y = rect.yMin - rect.height / 4;
                        moodBorderRect.height /= 4;
                        break;
                    case Alignment.Bottom:
                        moodBorderRect.x = rect.xMin;
                        moodBorderRect.y = moodBorderRect.yMax + SpacingLabel;
                        moodBorderRect.height /= 4;
                        break;
                }

                if (mood != null && mb != null)
                {
                    if (mood.CurLevelPercentage <= mb.BreakThresholdExtreme)
                    {
                        GUI.DrawTexture(moodBorderRect, ColonistBarTextures.MoodNeutral);
                        GUI.DrawTexture(moodBorderRect, ColonistBarTextures.MoodExtremeCrossedTex);
                    }
                    else if (mood.CurLevelPercentage <= mb.BreakThresholdMajor)
                    {
                        GUI.DrawTexture(moodBorderRect, ColonistBarTextures.MoodNeutral);
                        GUI.DrawTexture(moodBorderRect, ColonistBarTextures.MoodMajorCrossedTex);
                    }
                    else if (mood.CurLevelPercentage <= mb.BreakThresholdMinor)
                    {
                        GUI.DrawTexture(moodBorderRect, ColonistBarTextures.MoodNeutral);
                        GUI.DrawTexture(moodBorderRect, ColonistBarTextures.MoodMinorCrossedTex);
                    }
                    else
                    {
                        GUI.DrawTexture(moodBorderRect, ColonistBarTextures.MoodNeutral);
                    }
                }
            }



            if (ColBarSettings.UseGender)
            {
                if (colonist.gender == Gender.Male)
                {
                    BGColor = ColBarSettings.MaleColor;
                }
                if (colonist.gender == Gender.Female)
                {
                    BGColor = ColBarSettings.FemaleColor;
                }

                if (colonist.Dead)
                    BGColor = BGColor * Color.gray;

                BGColor.a = entryRectAlpha;

                GUI.color = BGColor;
            }


            GUI.DrawTexture(rect, ColBarSettings.UseGender ? ColonistBarTextures.BGTexGrey : ColonistBarTextures.BGTexVanilla);

            GUI.color = color;

            if (ColBarSettings.UseExternalMoodBar)
            {
                //         Rect moodRect = new Rect(rect.xMax, rect.y, rect.width/4, rect.height);
                DrawExternalMoodRect(rect, mood, mb);
            }
            else
            {
                if (colonist.needs != null && colonist.needs.mood != null)
                {
                    Rect position = rect.ContractedBy(2f);
                    float num = position.height * colonist.needs.mood.CurLevelPercentage;
                    position.yMin = position.yMax - num;
                    position.height = num;
                    GUI.DrawTexture(position, ColonistBarTextures.MoodBGTex);
                }
            }



            Rect rect2 = rect.ContractedBy(-2f * Scale);

            if (colonistAlive && !WorldRendererUtility.WorldRenderedNow)
            {
                DrawSelectionOverlayOnGUI(colonist, rect2);
            }
            else if (WorldRendererUtility.WorldRenderedNow && colonist.IsCaravanMember() && Find.WorldSelector.IsSelected(colonist.GetCaravan()))
            {
                DrawCaravanSelectionOverlayOnGUI(colonist.GetCaravan(), rect2);
            }

            GUI.DrawTexture(GetPawnTextureRect(rect.x, rect.y), PortraitsCache.Get(colonist, PawnTextureSize, PawnTextureCameraOffset, ColBarSettings.PawnTextureCameraZoom));

            if (ColBarSettings.UseWeaponIcons)
            {
                DrawWeaponIcon(rect, colonist);
            }

            GUI.color = new Color(1f, 1f, 1f, entryRectAlpha * 0.8f);
            DrawIcons(rect, colonist);
            GUI.color = color;
            if (colonist.Dead)
            {
                GUI.DrawTexture(rect, ColonistBarTextures.DeadColonistTex);
            }
            //       float num = 4f * Scale;
            Vector2 pos = new Vector2(rect.center.x, rect.yMax + 1f * Scale);
            GenMapUI.DrawPawnLabel(colonist, pos, entryRectAlpha, rect.width + SpacingHorizontal - 2f, pawnLabelsCache);
            GUI.color = Color.white;
        }

        private static void DrawExternalMoodRect(Rect rect, Need_Mood mood, MentalBreaker mb)
        {
            Rect moodRect = rect.ContractedBy(2.0f);
            switch (ColBarSettings.MoodBarPos)
            {
                case Alignment.Right:
                    moodRect.x = rect.xMax;
                    moodRect.width /= 4;
                    break;
                case Alignment.Left:
                    moodRect.x = rect.xMin - rect.width / 4;
                    moodRect.width /= 4;
                    break;
                case Alignment.Top:
                    moodRect.x = rect.xMin;
                    moodRect.y = rect.yMin - rect.height / 4;
                    moodRect.height /= 4;
                    break;
                case Alignment.Bottom:
                    moodRect.x = rect.xMin;
                    moodRect.y = moodRect.yMax + SpacingLabel;
                    moodRect.height /= 4;
                    break;
            }

            if (mood != null && mb != null)
            {
                if (mood.CurLevelPercentage > mb.BreakThresholdMinor)
                {
                    if (ColBarSettings.MoodBarPos == Alignment.Left || ColBarSettings.MoodBarPos == Alignment.Right)
                        GUI.DrawTexture(moodRect.BottomPart(mood.CurLevelPercentage), ColonistBarTextures.MoodTex);
                    else
                        GUI.DrawTexture(moodRect.LeftPart(mood.CurLevelPercentage), ColonistBarTextures.MoodTex);
                }
                else if (mood.CurLevelPercentage > mb.BreakThresholdMajor)
                {
                    if (ColBarSettings.MoodBarPos == Alignment.Left || ColBarSettings.MoodBarPos == Alignment.Right)
                        GUI.DrawTexture(moodRect.BottomPart(mood.CurLevelPercentage), ColonistBarTextures.MoodMinorCrossedTex);
                    else
                        GUI.DrawTexture(moodRect.LeftPart(mood.CurLevelPercentage), ColonistBarTextures.MoodMinorCrossedTex);
                }
                else if (mood.CurLevelPercentage > mb.BreakThresholdExtreme)
                {
                    if (ColBarSettings.MoodBarPos == Alignment.Left || ColBarSettings.MoodBarPos == Alignment.Right)
                        GUI.DrawTexture(moodRect.BottomPart(mood.CurLevelPercentage), ColonistBarTextures.MoodMajorCrossedTex);
                    else
                        GUI.DrawTexture(moodRect.LeftPart(mood.CurLevelPercentage), ColonistBarTextures.MoodMajorCrossedTex);
                }
                else
                {
                    GUI.DrawTexture(moodRect, ColonistBarTextures.MoodExtremeCrossedBGTex);
                    if (ColBarSettings.MoodBarPos == Alignment.Left || ColBarSettings.MoodBarPos == Alignment.Right)
                        GUI.DrawTexture(moodRect.BottomPart(mood.CurLevelPercentage), ColonistBarTextures.MoodExtremeCrossedTex);
                    else
                        GUI.DrawTexture(moodRect.LeftPart(mood.CurLevelPercentage), ColonistBarTextures.MoodExtremeCrossedTex);
                }

                DrawMentalThreshold(moodRect, mb.BreakThresholdExtreme, mood.CurLevelPercentage);
                DrawMentalThreshold(moodRect, mb.BreakThresholdMajor, mood.CurLevelPercentage);
                DrawMentalThreshold(moodRect, mb.BreakThresholdMinor, mood.CurLevelPercentage);

                switch (ColBarSettings.MoodBarPos)
                {
                    case Alignment.Left:
                    case Alignment.Right:
                        GUI.DrawTexture(
                            new Rect(moodRect.x, moodRect.yMax - moodRect.height * mood.CurInstantLevelPercentage, moodRect.width,
                                1), ColonistBarTextures.MoodTargetTex);
                        GUI.DrawTexture(
                            new Rect(moodRect.xMax + 1, moodRect.yMax - moodRect.height * mood.CurInstantLevelPercentage - 1, 2, 3),
                            ColonistBarTextures.MoodTargetTex);
                        break;
                    case Alignment.Top:
                        GUI.DrawTexture(
                            new Rect(moodRect.x + moodRect.width * mood.CurInstantLevelPercentage, moodRect.y, 1, moodRect.height),
                            ColonistBarTextures.MoodTargetTex);
                        GUI.DrawTexture(
                            new Rect(moodRect.x + moodRect.width * mood.CurInstantLevelPercentage - 1, moodRect.yMin - 1, 3, 2),
                            ColonistBarTextures.MoodTargetTex);
                        break;
                    case Alignment.Bottom:
                        GUI.DrawTexture(
                            new Rect(moodRect.x + moodRect.width * mood.CurInstantLevelPercentage, moodRect.y, 1, moodRect.height),
                            ColonistBarTextures.MoodTargetTex);
                        GUI.DrawTexture(
                            new Rect(moodRect.x + moodRect.width * mood.CurInstantLevelPercentage - 1, moodRect.yMax + 1, 3, 2),
                            ColonistBarTextures.MoodTargetTex);
                        break;
                }
            }
        }

        // RimWorld.ColonistBarColonistDrawer
        private static void DrawCaravanSelectionOverlayOnGUI(Caravan caravan, Rect rect)
        {
            float num = 0.4f * Scale;
            Vector2 textureSize = new Vector2(SelectionDrawerUtility.SelectedTexGUI.width * num, SelectionDrawerUtility.SelectedTexGUI.height * num);
            SelectionDrawerUtility.CalculateSelectionBracketPositionsUI(bracketLocs, caravan, rect, WorldSelectionDrawer.SelectTimes, textureSize, 20f * Scale);
            DrawSelectionOverlayOnGUI(bracketLocs, num);
        }


        // RimWorld.ColonistBarColonistDrawer
        private static void ApplyEntryInAnotherMapAlphaFactor(Map map, ref float alpha)
        {
            if (map == null)
            {
                if (!WorldRendererUtility.WorldRenderedNow)
                {
                    alpha = Mathf.Min(alpha, 0.4f);
                }
            }
            else if (map != Find.VisibleMap || WorldRendererUtility.WorldRenderedNow)
            {
                alpha = Mathf.Min(alpha, 0.4f);
            }
        }



        private static readonly Color HighlightColor = new Color(0.5f, 0.5f, 0.5f, 1f);


        internal static void DrawMentalThreshold(Rect moodRect, float threshold, float currentMood)
        {
            if (ColBarSettings.MoodBarPos == Alignment.Left || ColBarSettings.MoodBarPos == Alignment.Right)
                GUI.DrawTexture(new Rect(moodRect.x, moodRect.yMax - moodRect.height * threshold, moodRect.width, 1), ColonistBarTextures.MoodBreakTex);
            else
                GUI.DrawTexture(new Rect(moodRect.x + moodRect.width * threshold, moodRect.y, 1, moodRect.height), ColonistBarTextures.MoodBreakTex);

            /*if (currentMood <= threshold)
			{
				GUI.DrawTexture(new Rect(moodRect.xMax-4, moodRect.yMax - moodRect.height * threshold, 8, 2), MoodBreakCrossedTex);
			}*/
        }

        private static float GetEntryRectAlpha(Rect rect)
        {
            float t;
            if (Messages.CollidesWithAnyMessage(rect, out t))
            {
                return Mathf.Lerp(1f, 0.2f, t);
            }
            return 1f;
        }

        private static Rect GetPawnTextureRect(float x, float y)
        {
            Vector2 vector = PawnTextureSize * Scale;
            return new Rect(x + 1f, y - (vector.y - Size.y) - 1f, vector.x, vector.y);
        }

        private static void DrawIcons(Rect rect, Pawn colonist)
        {
            if (colonist.Dead)
            {
                return;
            }
            Vector2 vector = new Vector2(rect.x + 1f, rect.yMax - rect.width / 5 * 2 - 1f);
            bool flag = false;
            if (colonist.CurJob != null)
            {
                JobDef def = colonist.CurJob.def;
                if (def == JobDefOf.AttackMelee || def == JobDefOf.AttackStatic)
                {
                    flag = true;
                }
                else if (def == JobDefOf.WaitCombat)
                {
                    Stance_Busy stance_Busy = colonist.stances.curStance as Stance_Busy;
                    if (stance_Busy != null && stance_Busy.focusTarg.IsValid)
                    {
                        flag = true;
                    }
                }
            }
            if (colonist.InAggroMentalState)
            {
                //        DrawIcon(PSI.PSI.PSIMaterials[Icons.Aggressive].mainTexture as Texture2D, ref vector, colonist.MentalStateDef.LabelCap);
                DrawIcon(ColonistBarTextures.Icon_MentalStateAggro, ref vector, colonist.MentalStateDef.LabelCap);
            }
            else if (colonist.InMentalState)
            {
                //        DrawIcon(PSI.PSI.PSIMaterials[Icons.Dazed].mainTexture as Texture2D, ref vector, colonist.MentalStateDef.LabelCap);
                DrawIcon(ColonistBarTextures.Icon_MentalStateNonAggro, ref vector, colonist.MentalStateDef.LabelCap);
            }
            else if (colonist.InBed() && colonist.CurrentBed().Medical)
            {
                //        DrawIcon(PSI.PSI.PSIMaterials[Icons.Health].mainTexture as Texture2D, ref vector, "ActivityIconMedicalRest".Translate());
                DrawIcon(ColonistBarTextures.Icon_MedicalRest, ref vector, "ActivityIconMedicalRest".Translate());
            }
            else if (colonist.CurJob != null && colonist.jobs.curDriver.asleep)
            {
                //      DrawIcon(PSI.PSI.PSIMaterials[Icons.Tired].mainTexture as Texture2D, ref vector, "ActivityIconSleeping".Translate());
                DrawIcon(ColonistBarTextures.Icon_Sleeping, ref vector, "ActivityIconSleeping".Translate());
            }
            else if (colonist.CurJob != null && colonist.CurJob.def == JobDefOf.FleeAndCower)
            {
                //      DrawIcon(PSI.PSI.PSIMaterials[Icons.Leave].mainTexture as Texture2D, ref vector, "ActivityIconFleeing".Translate());
                DrawIcon(ColonistBarTextures.Icon_Fleeing, ref vector, "ActivityIconFleeing".Translate());
            }
            else if (flag)
            {
                DrawIcon(ColonistBarTextures.Icon_Attacking, ref vector, "ActivityIconAttacking".Translate());
            }
            else if (colonist.mindState.IsIdle && GenDate.DaysPassed >= 1)
            {
                //  DrawIcon(PSI.PSI.PSIMaterials[Icons.Idle].mainTexture as Texture2D, ref vector, "ActivityIconIdle".Translate());
                DrawIcon(ColonistBarTextures.Icon_Idle, ref vector, "ActivityIconIdle".Translate());
            }
            if (false)
            {
                PawnStats pawnStats;
                if (colonist.Dead || colonist.holdingContainer != null || !PSI.PSI._statsDict.TryGetValue(colonist, out pawnStats) ||
                    colonist.drafter == null || colonist.skills == null)
                    return;

                if (pawnStats.MentalSanity == MentalStateDefOf.BingingDrugMajor || pawnStats.MentalSanity == MentalStateDefOf.BingingDrugExtreme)
                {
                    Material material = PSI.PSI.PSIMaterials[Icons.Drunk];
                    DrawIcon(material.mainTexture as Texture2D, ref vector, colonist.MentalStateDef.LabelCap);
                }
            }

            if (colonist.IsBurning())
            {
                DrawIcon(ColonistBarTextures.Icon_Burning, ref vector, "ActivityIconBurning".Translate());
            }
        }

        private static void DrawIcon(Texture2D icon, ref Vector2 pos, string tooltip)
        {
            float num = ColBarSettings.BaseSizeFloat * 0.4f * Scale;
            Rect rect = new Rect(pos.x, pos.y, num, num);
            GUI.DrawTexture(rect, icon);
            TooltipHandler.TipRegion(rect, tooltip);
            pos.x += num;
        }

        private static void DrawWeaponIcon(Rect rect, Pawn colonist)
        {
            float entryRectAlpha = GetEntryRectAlpha(rect);
            ApplyEntryInAnotherMapAlphaFactor(colonist.Map, ref entryRectAlpha);
            Color color = new Color(1f, 1f, 1f, entryRectAlpha);
            GUI.color = color;
            if (colonist?.equipment.Primary != null)
            {
                var thing = colonist.equipment.Primary;
                Rect rect2 = rect.ContractedBy(rect.width / 3);

                rect2.x = rect.xMax - rect2.width - rect.width / 12;
                rect2.y = rect.yMax - rect2.height - rect.height / 12;

                GUI.color = color;
                Texture2D resolvedIcon;
                if (!thing.def.uiIconPath.NullOrEmpty())
                {
                    resolvedIcon = thing.def.uiIcon;
                }
                else
                {
                    resolvedIcon = thing.Graphic.ExtractInnerGraphicFor(thing).MatSingle.mainTexture as Texture2D;
                }
                // color labe by thing
                Color iconcolor = new Color();

                if (thing.def.IsMeleeWeapon)
                {
                    GUI.color = new Color(0.85f, 0.2f, 0.2f, entryRectAlpha);
                    iconcolor = new Color(0.2f, 0.05f, 0.05f, 0.95f * entryRectAlpha);
                }
                if (thing.def.IsRangedWeapon)
                {
                    GUI.color = new Color(0.15f, 0.3f, 0.85f, entryRectAlpha);
                    iconcolor = new Color(0.03f, 0.075f, 0.2f, 0.95f * entryRectAlpha);
                }
                Widgets.DrawBoxSolid(rect2, iconcolor);
                Widgets.DrawBox(rect2);
                GUI.color = color;
                Rect rect3 = rect2.ContractedBy(rect2.width / 8);

                Widgets.DrawTextureRotated(rect3, resolvedIcon, 0);
                if (Mouse.IsOver(rect2))
                {
                    GUI.color = HighlightColor;
                    GUI.DrawTexture(rect2, TexUI.HighlightTex);
                }
                TooltipHandler.TipRegion(rect2, thing.def.LabelCap);

            }
        }

        private static Pawn SelPawn => Find.Selector.SingleSelectedThing as Pawn;

        private static int clickCount = 0;

        private static void HandleClicks(Rect rect, Pawn colonist)
        {
            if (Mouse.IsOver(rect))
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    if (Event.current.button == 0)
                    {
                        if (clickedColonist == colonist && Time.time - clickedAt < ColBarSettings.DoubleClickTime)
                        {
                            // use event so it doesn't bubble through
                            Event.current.Use();
                            JumpToTargetUtility.TryJump(colonist);
                            clickedColonist = null;
                        }
                        else
                        {
                            clickedColonist = colonist;
                            clickedAt = Time.time;
                            clickCount++;
                        }
                    }
                    else if (Event.current.button == 1)
                    {
                        if (Event.current.type == EventType.MouseDown)
                        {
                            List<FloatMenuOption> floatOptionList = new List<FloatMenuOption>();

                            if (clickedColonist != null && SelPawn != null && SelPawn != clickedColonist)
                            {
                                foreach (FloatMenuOption choice in FloatMenuMakerMap.ChoicesAtFor(clickedColonist.TrueCenter(), SelPawn))
                                {
                                    floatOptionList.Add(choice);
                                }
                                floatOptionList.Add(new FloatMenuOption("--------------------", delegate
                                {
                                }));

                            }
                            floatOptionList.Add(new FloatMenuOption("FollowMe™", delegate
                            {
                                MapComponent_FollowMe.TryStartFollow(colonist);

                            }));
                            floatOptionList.Add(new FloatMenuOption("CBKF.Settings.Vanilla".Translate(), delegate
                            {
                                ColBarSettings.SortBy = SettingsColonistBar.SortByWhat.vanilla;
                                MarkColonistsDirty();
                                CheckRecacheEntries();
                            }));
                            floatOptionList.Add(new FloatMenuOption("CBKF.Settings.ByName".Translate(), delegate
                            {
                                ColBarSettings.SortBy = SettingsColonistBar.SortByWhat.byName;
                                MarkColonistsDirty();
                                CheckRecacheEntries();
                            }));

                            floatOptionList.Add(new FloatMenuOption("CBKF.Settings.SexAge".Translate(), delegate
                            {
                                ColBarSettings.SortBy = SettingsColonistBar.SortByWhat.sexage;
                                MarkColonistsDirty();
                                CheckRecacheEntries();
                            }));

                            floatOptionList.Add(new FloatMenuOption("CBKF.Settings.Mood".Translate(), delegate
                            {
                                ColBarSettings.SortBy = SettingsColonistBar.SortByWhat.mood;
                                MarkColonistsDirty();
                                CheckRecacheEntries();
                            }));
                            floatOptionList.Add(new FloatMenuOption("CBKF.Settings.Health".Translate(), delegate
                            {
                                ColBarSettings.SortBy = SettingsColonistBar.SortByWhat.health;
                                MarkColonistsDirty();
                                CheckRecacheEntries();
                            }));
                            floatOptionList.Add(new FloatMenuOption("CBKF.Settings.Medic".Translate(), delegate
                            {
                                ColBarSettings.SortBy = SettingsColonistBar.SortByWhat.medic;
                                MarkColonistsDirty();
                                CheckRecacheEntries();
                            }));
                            floatOptionList.Add(new FloatMenuOption("CBKF.Settings.Weapons".Translate(), delegate
                            {
                                ColBarSettings.SortBy = SettingsColonistBar.SortByWhat.weapons;
                                MarkColonistsDirty();
                                CheckRecacheEntries();
                            }));

                            floatOptionList.Add(new FloatMenuOption("CBKF.Settings.SettingsColonistBar".Translate(), delegate { Find.WindowStack.Add(new ColonistBarKF_Settings()); }));
                            FloatMenu window = new FloatMenu(floatOptionList, "CBKF.Settings.SortingOptions".Translate());
                            Find.WindowStack.Add(window);

                            // use event so it doesn't bubble through
                            Event.current.Use();
                        }

                    }
                }

            }


        }

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public static void MarkColonistsDirty()
        {
            entriesDirty = true;
        }



        private static Vector2[] bracketLocs = new Vector2[4];

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
                return UI.screenWidth >= 1000 && UI.screenHeight >= 760;
            }
        }

        private static void DrawSelectionOverlayOnGUI(Pawn colonist, Rect rect)
        {
            Thing obj = colonist;
            if (colonist.Dead)
            {
                obj = colonist.Corpse;
            }
            float num = 0.4f * Scale;
            Vector2 textureSize = new Vector2(SelectionDrawerUtility.SelectedTexGUI.width * num, SelectionDrawerUtility.SelectedTexGUI.height * num);
            SelectionDrawerUtility.CalculateSelectionBracketPositionsUI(bracketLocs, obj, rect, SelectionDrawer.SelectTimes, textureSize, ColBarSettings.BaseSizeFloat * 0.4f * Scale);
            DrawSelectionOverlayOnGUI(bracketLocs, num);
        }

        private static void DrawSelectionOverlayOnGUI(Vector2[] bracketLocs, float selectedTexScale)
        {
            int num = 90;
            for (int i = 0; i < 4; i++)
            {
                Widgets.DrawTextureRotated(bracketLocs[i], SelectionDrawerUtility.SelectedTexGUI, num, selectedTexScale);
                num += 90;
            }
        }

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public static Caravan CaravanMemberCaravanAt(Vector2 at)
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
        public static List<Caravan> CaravanMembersCaravansInScreenRect(Rect rect)
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
        public static List<Pawn> CaravanMembersInScreenRect(Rect rect)
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
        public static List<Thing> MapColonistsOrCorpsesInScreenRect(Rect rect)
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
