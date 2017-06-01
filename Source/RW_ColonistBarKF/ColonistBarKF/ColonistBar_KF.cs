using static ColonistBarKF.Position;

namespace ColonistBarKF
{
    using System.Collections.Generic;
    using System.Reflection;

    using RimWorld;
    using RimWorld.Planet;

    using UnityEngine;

    using Verse;

    [StaticConstructorOnStartup]
    public static class ColonistBar_KF
    {


        public const float SpacingLabel = 15f;

        private const float PawnTextureHorizontalPadding = 1f;

        public static ColBarHelper_KF BarHelperKf = new ColBarHelper_KF();

        public static ColonistBarColonistDrawer_KF drawer = new ColonistBarColonistDrawer_KF();

        public static ColonistBarDrawLocsFinder_KF drawLocsFinder = new ColonistBarDrawLocsFinder_KF();

        private static float heightMoodBarVertical = 0f;

        private static float hightPsiVertical = 0f;

        private static float widthMoodBarHorizontal = 0f;

        private static float widthPsiHorizontal = 0f;

        public static Vector2 BaseSize
            => new Vector2(Settings.ColBarSettings.BaseSizeFloat, Settings.ColBarSettings.BaseSizeFloat);

        public static Vector2 FullSize => new Vector2(
                                              Settings.ColBarSettings.BaseSizeFloat + WidthMoodBarHorizontal + WidthPSIHorizontal,
                                              Settings.ColBarSettings.BaseSizeFloat + HeightMoodBarVertical + HeightPSIVertical) * Scale;

        public static float HeightMoodBarVertical => heightMoodBarVertical;

        public static float HeightPSIVertical => hightPsiVertical;

        public static Vector2 PawnSize => (new Vector2(Settings.ColBarSettings.BaseSizeFloat, Settings.ColBarSettings.BaseSizeFloat))
                                          * Scale;

        public static int PsiRowsOnBar { get; set; }

        // public static readonly Vector2 PawnTextureSize = new Vector2(BaseSize.x - 2f, 75f);
        public static float Scale => BarHelperKf.cachedScale;

        public static float WidthMoodBarHorizontal => widthMoodBarHorizontal;

        public static float WidthPSIHorizontal => widthPsiHorizontal;

        public static float WidthSpacingHorizontal
        {
            get
            {
                return Settings.ColBarSettings.BaseSpacingHorizontal + WidthMoodBarHorizontal + WidthPSIHorizontal;
            }
        }

        public static float HeightSpacingVertical
        {
            get
            {
                return Settings.ColBarSettings.BaseSpacingVertical + HeightMoodBarVertical + HeightPSIVertical;
            }
        }

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

        private static bool Visible
        {
            get
            {
                return UI.screenWidth >= 800 && UI.screenHeight >= 500;
            }
        }



 //       [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
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

 //       [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public static List<Caravan> CaravanMembersCaravansInScreenRect(Rect rect)
        {
            BarHelperKf.tmpCaravans.Clear();
            if (!Visible)
            {
                return BarHelperKf.tmpCaravans;
            }

            List<Pawn> list = CaravanMembersInScreenRect(rect);
            for (int i = 0; i < list.Count; i++)
            {
                BarHelperKf.tmpCaravans.Add(list[i].GetCaravan());
            }

            return BarHelperKf.tmpCaravans;
        }

  //      [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
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

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public static void ColonistBarOnGUI()
        {
            if (!Visible)
            {
                return;
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
        }

        // Detour not working -> RW
        //       [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public static Thing ColonistOrCorpseAt(Vector2 pos)
        {
            if (!Visible)
            {
                return null;
            }
            ColonistBar.Entry entry;
            if (!BarHelperKf.TryGetEntryAt(pos, out entry))
            {
                return null;
            }
            Pawn pawn = entry.pawn;
            Thing result;
            if (pawn != null && pawn.Dead && pawn.Corpse != null && pawn.Corpse.SpawnedOrAnyParentSpawned)
            {
                result = pawn.Corpse;
            }
            else
            {
                result = pawn;
            }
            return result;
        }


        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public static List<Pawn> GetColonistsInOrder()
        {
            List<ColonistBar.Entry> entries = BarHelperKf.Entries;
            BarHelperKf.tmpColonistsInOrder.Clear();
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].pawn != null)
                {
                    BarHelperKf.tmpColonistsInOrder.Add(entries[i].pawn);
                }
            }

            return BarHelperKf.tmpColonistsInOrder;
        }

        public static float GetEntryRectAlpha(Rect rect)
        {
            float t;
            if (Messages.CollidesWithAnyMessage(rect, out t))
            {
                return Mathf.Lerp(1f, 0.2f, t);
            }

            return 1f;
        }

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public static List<Thing> MapColonistsOrCorpsesInScreenRect(Rect rect)
        {
            BarHelperKf.tmpMapColonistsOrCorpsesInScreenRect.Clear();
            if (!Visible)
            {
                return BarHelperKf.tmpMapColonistsOrCorpsesInScreenRect;
            }

            List<Thing> list = ColonistsOrCorpsesInScreenRect(rect);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Spawned)
                {
                    BarHelperKf.tmpMapColonistsOrCorpsesInScreenRect.Add(list[i]);
                }
            }

            return BarHelperKf.tmpMapColonistsOrCorpsesInScreenRect;
        }

        // detour not working - JIT?!?
        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public static void MarkColonistsDirty()
        {
            RecalcSizes();
            BarHelperKf.entriesDirty = true;

            // Log.Message("Colonists marked dirty.01");
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
                if (BarHelperKf.tmpColonistsWithMap.Any((Pair<Thing, Map> x) => x.Second == null))
                {
                    BarHelperKf.tmpColonistsWithMap.RemoveAll((Pair<Thing, Map> x) => x.Second != null);
                    goto IL_1A1;
                }
            }
            if (BarHelperKf.tmpColonistsWithMap.Any((Pair<Thing, Map> x) => x.Second == Find.VisibleMap))
            {
                BarHelperKf.tmpColonistsWithMap.RemoveAll((Pair<Thing, Map> x) => x.Second != Find.VisibleMap);
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

        public static void RecalcSizes()
        {
            widthMoodBarHorizontal = 0f;

            heightMoodBarVertical = 0f;

            widthPsiHorizontal = 0f;

            hightPsiVertical = 0f;

            if (Settings.ColBarSettings.UseExternalMoodBar)
            {
                switch (Settings.ColBarSettings.MoodBarPos)
                {
                    case Alignment.Left:
                    case Alignment.Right:
                        widthMoodBarHorizontal = Settings.ColBarSettings.BaseSizeFloat / 4;
                        break;
                    case Alignment.Top:
                    case Alignment.Bottom:
                        heightMoodBarVertical = Settings.ColBarSettings.BaseSizeFloat / 4;
                        break;
                    default:
                        break;
                }
            }

            if (Settings.ColBarSettings.UsePsi)
            {
                switch (Settings.ColBarSettings.ColBarPsiIconPos)
                {
                    case Alignment.Left:
                    case Alignment.Right:
                        widthPsiHorizontal = Settings.ColBarSettings.BaseSizeFloat
                                              / Settings.ColBarSettings.IconsInColumn * PsiRowsOnBar;
                        break;
                    case Alignment.Top:
                    case Alignment.Bottom:
                        hightPsiVertical = Settings.ColBarSettings.BaseSizeFloat
                                            / Settings.ColBarSettings.IconsInColumn * PsiRowsOnBar;
                        break;
                    default:
                        break;
                }
            }
        }


    }
}