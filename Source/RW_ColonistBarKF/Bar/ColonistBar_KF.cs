using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace ColonistBarKF.Bar
{
    [StaticConstructorOnStartup]
    public static class ColonistBar_KF
    {
        public const float SpacingLabel = 15f;

        public static ColBarHelper_KF BarHelperKF = new ColBarHelper_KF();

        [NotNull]
        public static ColonistBarColonistDrawer_KF Drawer = new ColonistBarColonistDrawer_KF();

        [NotNull]
        public static ColonistBarDrawLocsFinder_Kf DrawLocsFinder = new ColonistBarDrawLocsFinder_Kf();

        public static Vector2 BaseSize => new Vector2(
            Settings.BarSettings.BaseIconSize,
            Settings.BarSettings.BaseIconSize);

        public static Vector2 FullSize => new Vector2(
                                              Settings.BarSettings.BaseIconSize + WidthMoodBarHorizontal
                                              + WidthPSIHorizontal,
                                              Settings.BarSettings.BaseIconSize + HeightMoodBarVertical
                                              + HeightPSIVertical) * Scale;

        public static float HeightPSIVertical { get; private set; }

        public static float HeightSpacingVertical => Settings.BarSettings.BaseSpacingVertical + HeightMoodBarVertical
                                                     + HeightPSIVertical;

        public static Vector2 PawnSize => new Vector2(
                                              Settings.BarSettings.BaseIconSize,
                                              Settings.BarSettings.BaseIconSize) * Scale;

        public static int PSIRowsOnBar
        {
            get
            {
                return 2;

                int maxRows = 0;
                foreach (Pawn pawn in PawnsFinder.AllMaps_FreeColonistsSpawned)
                {
                    maxRows = Mathf.Max(pawn.GetComp<CompPSI>().ThisColCount, maxRows);
                }

                return maxRows;
            }
        }

        // public static readonly Vector2 PawnTextureSize = new Vector2(BaseSize.x - 2f, 75f);
        public static float Scale => BarHelperKF.CachedScale;

        public static bool Visible => UI.screenWidth >= 800 && UI.screenHeight >= 500;

        public static float WidthPSIHorizontal { get; private set; }

        public static float WidthSpacingHorizontal => Settings.BarSettings.BaseSpacingHorizontal
                                                      + WidthMoodBarHorizontal + WidthPSIHorizontal;

        private static float HeightMoodBarVertical { get; set; }



        private static float WidthMoodBarHorizontal { get; set; }

        [NotNull]
        public static List<Pawn> CaravanMembersInScreenRect(Rect rect)
        {
            BarHelperKF.TmpCaravanPawns.Clear();
            if (!Visible)
            {
                return BarHelperKF.TmpCaravanPawns;
            }

            List<Thing> list = ColonistsOrCorpsesInScreenRect(rect);
            for (int i = 0; i < list.Count; i++)
            {
                Pawn pawn = list[i] as Pawn;
                if (pawn != null && pawn.IsCaravanMember())
                {
                    BarHelperKF.TmpCaravanPawns.Add(pawn);
                }
            }

            return BarHelperKF.TmpCaravanPawns;
        }

        public static bool ColonistBarOnGUI_Prefix()
        {
            if (!Visible)
            {
                return false;
            }

            if (Event.current.type != EventType.Layout)
            {
                List<EntryKF> entries = BarHelperKF.Entries;
                int num = -1;
                bool showGroupFrames = BarHelperKF.ShowGroupFrames;
                for (int i = 0; i < BarHelperKF.DrawLocs.Count; i++)
                {
                    Rect rect = new Rect(
                        BarHelperKF.DrawLocs[i].x,
                        BarHelperKF.DrawLocs[i].y,
                        FullSize.x,
                        FullSize.y + SpacingLabel);
                    EntryKF entry = entries[i];
                    bool flag = num != entry.Group;
                    num = entry.Group;
                    if (entry.GroupCount > 0)
                    {
                        // Pawn can be null, click extends group
                        Drawer.HandleClicks(rect, entry.Pawn, entry.Group);
                    }

                    if (Event.current.type == EventType.Repaint)
                    {
                        if (flag && showGroupFrames)
                        {
                            Drawer.DrawGroupFrame(entry.Group);
                        }

                        if (entry.Pawn != null)
                        {
                            Drawer.DrawColonist(rect, entry.Pawn, entry.Map);
                        }
                        else
                        {
                            Drawer.DrawEmptyFrame(rect, entry.Map, entry.GroupCount);
                        }
                    }
                }

                num = -1;
                if (showGroupFrames)
                {
                    for (int j = 0; j < BarHelperKF.DrawLocs.Count; j++)
                    {
                        EntryKF entry2 = entries[j];
                        int entry2Group = entry2.Group;
                        bool flag2 = num != entry2Group;
                        num = entry2Group;
                        if (flag2)
                        {
                            Drawer.HandleGroupFrameClicks(entry2Group);
                        }
                    }
                }
            }

            return false;
        }

        [NotNull]
        public static List<Thing> ColonistsOrCorpsesInScreenRect(Rect rect)
        {
            List<Vector2> drawLocs = BarHelperKF.DrawLocs;
            List<EntryKF> entries = BarHelperKF.Entries;
            Vector2 size = FullSize;
            BarHelperKF.TmpColonistsWithMap.Clear();
            for (int i = 0; i < drawLocs.Count; i++)
            {
                if (rect.Overlaps(new Rect(drawLocs[i].x, drawLocs[i].y, size.x, size.y)))
                {
                    Pawn pawn = entries[i].Pawn;
                    if (pawn != null)
                    {
                        Thing first;
                        if (pawn.Dead && pawn.Corpse != null && pawn.Corpse.SpawnedOrAnyParentSpawned)
                        {
                            first = pawn.Corpse;
                        }
                        else
                        {
                            first = pawn;
                        }

                        BarHelperKF.TmpColonistsWithMap.Add(new Pair<Thing, Map>(first, entries[i].Map));
                    }
                }
            }

            bool flag = true;

            if (WorldRendererUtility.WorldRenderedNow)
            {
                if (BarHelperKF.TmpColonistsWithMap.Any(x => x.Second == null))
                {
                    BarHelperKF.TmpColonistsWithMap.RemoveAll(x => x.Second != null);
                    flag = false;
                }
            }

            if (flag)
            {
                if (BarHelperKF.TmpColonistsWithMap.Any(x => x.Second == Find.CurrentMap))
                {
                    BarHelperKF.TmpColonistsWithMap.RemoveAll(x => x.Second != Find.CurrentMap);
                }
            }

            BarHelperKF.TmpColonists.Clear();
            for (int j = 0; j < BarHelperKF.TmpColonistsWithMap.Count; j++)
            {
                BarHelperKF.TmpColonists.Add(BarHelperKF.TmpColonistsWithMap[j].First);
            }

            BarHelperKF.TmpColonistsWithMap.Clear();
            return BarHelperKF.TmpColonists;
        }

        public static float GetEntryRectAlpha(Rect rect)
        {
            if (Messages.CollidesWithAnyMessage(rect, out float t))
            {
                return Mathf.Lerp(1f, 0.2f, t);
            }

            return 1f;
        }

        public static void RecalcSizes()
        {
            WidthMoodBarHorizontal = 0f;

            HeightMoodBarVertical = 0f;

            WidthPSIHorizontal = 0f;

            HeightPSIVertical = 0f;

            if (Settings.BarSettings.UseExternalMoodBar)
            {
                switch (Settings.BarSettings.MoodBarPos)
                {
                    case Position.Alignment.Left:
                    case Position.Alignment.Right:
                        WidthMoodBarHorizontal = Settings.BarSettings.BaseIconSize / 4;
                        break;

                    case Position.Alignment.Top:
                    case Position.Alignment.Bottom:
                        HeightMoodBarVertical = Settings.BarSettings.BaseIconSize / 4;
                        break;

                    default: break;
                }
            }

            if (Settings.BarSettings.UsePSI)
            {
                switch (Settings.BarSettings.ColBarPSIIconPos)
                {
                    case Position.Alignment.Left:
                    case Position.Alignment.Right:
                        WidthPSIHorizontal = Settings.BarSettings.BaseIconSize
                                             / Settings.BarSettings.IconsInColumn * PSIRowsOnBar;
                        break;

                    case Position.Alignment.Top:
                    case Position.Alignment.Bottom:
                        HeightPSIVertical = Settings.BarSettings.BaseIconSize
                                            / Settings.BarSettings.IconsInColumn * PSIRowsOnBar;
                        break;

                    default: break;
                }
            }
        }
    }
}