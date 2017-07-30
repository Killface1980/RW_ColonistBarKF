namespace ColonistBarKF.Bar
{
    using RimWorld;
    using RimWorld.Planet;
    using System.Collections.Generic;
    using UnityEngine;
    using Verse;

    [StaticConstructorOnStartup]
    public static class ColonistBar_KF
    {
        #region Fields

        public const float SpacingLabel = 15f;

        public static ColBarHelper_KF BarHelperKf = new ColBarHelper_KF();
        public static ColonistBarColonistDrawer_KF drawer = new ColonistBarColonistDrawer_KF();
        public static ColonistBarDrawLocsFinder_KF drawLocsFinder = new ColonistBarDrawLocsFinder_KF();
        private const float PawnTextureHorizontalPadding = 1f;

        #endregion Fields

        #region Properties

        public static Vector2 FullSize => new Vector2(
                                              Settings.ColBarSettings.BaseSizeFloat + WidthMoodBarHorizontal
                                              + WidthPSIHorizontal,
                                              Settings.ColBarSettings.BaseSizeFloat + HeightMoodBarVertical
                                              + HeightPSIVertical) * Scale;

        public static float HeightMoodBarVertical { get; private set; }

        public static float HeightPSIVertical { get; private set; }

        public static float HeightSpacingVertical => Settings.ColBarSettings.BaseSpacingVertical + HeightMoodBarVertical
                                                     + HeightPSIVertical;

        public static Vector2 PawnSize => new Vector2(
                                              Settings.ColBarSettings.BaseSizeFloat,
                                              Settings.ColBarSettings.BaseSizeFloat) * Scale;

        public static int PsiRowsOnBar
        {
            get
            {
                return 2;

                int maxRows = 0;
                foreach (Pawn pawn in PawnsFinder.AllMaps_FreeColonistsSpawned)
                {
                    maxRows = Mathf.Max(pawn.TryGetComp<CompPSI>().thisColCount, maxRows);
                }
                return maxRows;
            }
        }

        // public static readonly Vector2 PawnTextureSize = new Vector2(BaseSize.x - 2f, 75f);
        public static float Scale => BarHelperKf.cachedScale;

        public static bool Visible => UI.screenWidth >= 800 && UI.screenHeight >= 500;

        public static float WidthMoodBarHorizontal { get; private set; }

        public static float WidthPSIHorizontal { get; private set; }

        public static float WidthSpacingHorizontal => Settings.ColBarSettings.BaseSpacingHorizontal
                                                      + WidthMoodBarHorizontal + WidthPSIHorizontal;

        public static Vector2 BaseSize => new Vector2(
                                                                                                    Settings.ColBarSettings.BaseSizeFloat,
            Settings.ColBarSettings.BaseSizeFloat);

        private static bool ShowGroupFrames
        {
            get
            {
                List<ColonistBar.Entry> entries = BarHelperKf.Entries;
                int num = -1;
                for (int i = 0; i < entries.Count; i++)
                {
                    num = Mathf.Max(num, entries[i].group);
                }

                return num >= 1;
            }
        }

        #endregion Properties


        #region Methods

        public static List<Pawn> CaravanMembersInScreenRect(Rect rect)
        {
            BarHelperKf.tmpCaravanPawns.Clear();
            if (!Visible)
            {
                return BarHelperKf.tmpCaravanPawns;
            }

            List<Thing> list = ColonistsOrCorpsesInScreenRect(rect);
            for (int i = 0; i < list.Count; i++)
            {
                Pawn pawn = list[i] as Pawn;
                if (pawn != null && pawn.IsCaravanMember())
                {
                    BarHelperKf.tmpCaravanPawns.Add(pawn);
                }
            }

            return BarHelperKf.tmpCaravanPawns;
        }

        public static bool ColonistBarOnGUI_Prefix()
        {
            if (!Visible)
            {
                return false;
            }

            if (Event.current.type != EventType.Layout)
            {
                List<ColonistBar.Entry> entries = BarHelperKf.Entries;
                int num = -1;
                bool showGroupFrames = ShowGroupFrames;
                for (int i = 0; i < BarHelperKf.cachedDrawLocs.Count; i++)
                {
                    Rect rect = new Rect(
                        BarHelperKf.cachedDrawLocs[i].x,
                        BarHelperKf.cachedDrawLocs[i].y,
                        FullSize.x,
                        FullSize.y + SpacingLabel);
                    ColonistBar.Entry entry = entries[i];
                    bool flag = num != entry.group;
                    num = entry.group;
                    if (entry.pawn != null)
                    {
                        drawer.HandleClicks(rect, entry.pawn);
                    }

                    if (Event.current.type == EventType.Repaint)
                    {
                        if (flag && showGroupFrames)
                        {
                            drawer.DrawGroupFrame(entry.group);
                        }

                        if (entry.pawn != null)
                        {
                            drawer.DrawColonist(rect, entry.pawn, entry.map);
                        }
                    }
                }

                num = -1;
                if (showGroupFrames)
                {
                    for (int j = 0; j < BarHelperKf.cachedDrawLocs.Count; j++)
                    {
                        ColonistBar.Entry entry2 = entries[j];
                        bool flag2 = num != entry2.group;
                        num = entry2.group;
                        if (flag2)
                        {
                            drawer.HandleGroupFrameClicks(entry2.group);
                        }
                    }
                }
            }

            return false;
        }

        public static List<Thing> ColonistsOrCorpsesInScreenRect(Rect rect)
        {
            List<Vector2> drawLocs = BarHelperKf.DrawLocs;
            List<ColonistBar.Entry> entries = BarHelperKf.Entries;
            Vector2 size = FullSize;
            BarHelperKf.tmpColonistsWithMap.Clear();
            for (int i = 0; i < drawLocs.Count; i++)
            {
                if (rect.Overlaps(new Rect(drawLocs[i].x, drawLocs[i].y, size.x, size.y)))
                {
                    Pawn pawn = entries[i].pawn;
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

                        BarHelperKf.tmpColonistsWithMap.Add(new Pair<Thing, Map>(first, entries[i].map));
                    }
                }
            }

            if (WorldRendererUtility.WorldRenderedNow)
            {
                if (BarHelperKf.tmpColonistsWithMap.Any(x => x.Second == null))
                {
                    BarHelperKf.tmpColonistsWithMap.RemoveAll(x => x.Second != null);
                    goto IL_1A1;
                }
            }

            if (BarHelperKf.tmpColonistsWithMap.Any(x => x.Second == Find.VisibleMap))
            {
                BarHelperKf.tmpColonistsWithMap.RemoveAll(x => x.Second != Find.VisibleMap);
            }

            IL_1A1:
            BarHelperKf.tmpColonists.Clear();
            for (int j = 0; j < BarHelperKf.tmpColonistsWithMap.Count; j++)
            {
                BarHelperKf.tmpColonists.Add(BarHelperKf.tmpColonistsWithMap[j].First);
            }

            BarHelperKf.tmpColonistsWithMap.Clear();
            return BarHelperKf.tmpColonists;
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

            if (Settings.ColBarSettings.UseExternalMoodBar)
            {
                switch (Settings.ColBarSettings.MoodBarPos)
                {
                    case Position.Alignment.Left:
                    case Position.Alignment.Right:
                        WidthMoodBarHorizontal = Settings.ColBarSettings.BaseSizeFloat / 4;
                        break;

                    case Position.Alignment.Top:
                    case Position.Alignment.Bottom:
                        HeightMoodBarVertical = Settings.ColBarSettings.BaseSizeFloat / 4;
                        break;

                    default: break;
                }
            }

            if (Settings.ColBarSettings.UsePsi)
            {
                switch (Settings.ColBarSettings.ColBarPsiIconPos)
                {
                    case Position.Alignment.Left:
                    case Position.Alignment.Right:
                        WidthPSIHorizontal = Settings.ColBarSettings.BaseSizeFloat
                                             / Settings.ColBarSettings.IconsInColumn * PsiRowsOnBar;
                        break;

                    case Position.Alignment.Top:
                    case Position.Alignment.Bottom:
                        HeightPSIVertical = Settings.ColBarSettings.BaseSizeFloat
                                            / Settings.ColBarSettings.IconsInColumn * PsiRowsOnBar;
                        break;

                    default: break;
                }
            }
        }

        #endregion Methods
    }
}