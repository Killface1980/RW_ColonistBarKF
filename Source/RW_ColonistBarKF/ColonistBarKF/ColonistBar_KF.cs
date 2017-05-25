using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColonistBarKF.ColorPicker;
using ColonistBarKF.Detouring;
using ColonistBarKF.PSI;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;
using static ColonistBarKF.Position;

namespace ColonistBarKF
{
    [StaticConstructorOnStartup]
    public static class ColonistBar_KF
    {
        public static ColonistBarDrawLocsFinder_KF drawLocsFinder = new ColonistBarDrawLocsFinder_KF();
        public static ColonistBarColonistDrawer_KF drawer = new ColonistBarColonistDrawer_KF();

        public static ColBarHelper_KF BarHelperKf = new ColBarHelper_KF();

        private const float PawnTextureHorizontalPadding = 1f;




        // custom test

        public static Vector2 BaseSize => new Vector2(Settings.ColBarSettings.BaseSizeFloat, Settings.ColBarSettings.BaseSizeFloat);

        //      public static readonly Vector2 PawnTextureSize = new Vector2(BaseSize.x - 2f, 75f);




        public static float Scale
        {
            get
            {
                return BarHelperKf.cachedScale;
            }
        }

        public static Vector2 FullSize
        {
            get
            {
                return new Vector2(Settings.ColBarSettings.BaseSizeFloat + WidthMoodBarHorizontal() + WidthPSIHorizontal(), Settings.ColBarSettings.BaseSizeFloat + HeightMoodBarVertical() + HeightPSIVertical()) * Scale;
            }
        }
        public static Vector2 PawnSize
        {
            get
            {
                return (new Vector2(Settings.ColBarSettings.BaseSizeFloat, Settings.ColBarSettings.BaseSizeFloat)) * Scale;
            }
        }
        public static float SpaceBetweenColonistsHorizontal
        {
            get
            {
                return Settings.ColBarSettings.BaseSpacingHorizontal * Scale;
            }
        }

        public static float WidthMoodBarHorizontal()
        {
            if (Settings.ColBarSettings.UseExternalMoodBar &&
                (Settings.ColBarSettings.MoodBarPos == Alignment.Left || Settings.ColBarSettings.MoodBarPos == Alignment.Right))
                return Settings.ColBarSettings.BaseSizeFloat / 4;

            return 0f;
        }

        public static float HeightMoodBarVertical()
        {
            if (Settings.ColBarSettings.UseExternalMoodBar &&
                (Settings.ColBarSettings.MoodBarPos == Alignment.Bottom || Settings.ColBarSettings.MoodBarPos == Alignment.Top))
                return Settings.ColBarSettings.BaseSizeFloat / 4;
            return 0f;
        }

        public static float WidthPSIHorizontal()
        {
            if (Settings.ColBarSettings.UsePsi)
                if (Settings.ColBarSettings.ColBarPsiIconPos == Alignment.Left || Settings.ColBarSettings.ColBarPsiIconPos == Alignment.Right)
                {
                    return Settings.ColBarSettings.BaseSizeFloat /Settings.ColBarSettings.IconsInColumn * PsiRowsOnBar;
                }
            return 0f;
        }

        public static float HeightPSIVertical()
        {
            if (Settings.ColBarSettings.UsePsi)
                if (Settings.ColBarSettings.ColBarPsiIconPos == Alignment.Bottom || Settings.ColBarSettings.ColBarPsiIconPos == Alignment.Top)
                {
                    return Settings.ColBarSettings.BaseSizeFloat /Settings.ColBarSettings.IconsInColumn * PsiRowsOnBar;
                }
            return 0f;
        }

        public static int PsiRowsOnBar
        {
            get
            {
                return 2;

                int maxCount = 0;
                foreach (KeyValuePair<Pawn, PawnStats> colonist in Settings._statsDict)
                {
                    maxCount = Mathf.Max(maxCount, colonist.Value.IconCount);
                }
                int psiRowsOnBar = Mathf.CeilToInt((float)maxCount /Settings.ColBarSettings.IconsInColumn);
                return psiRowsOnBar;
            }
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

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public static void ColonistBarOnGUI()
        {
            if (!Visible)
            {
                return;
            }


            // if (Event.current.type == EventType.Layout)
            // {
            //    if (Rand.Value>0.99f)
            //         MarkColonistsDirty();
            // }


            if (Event.current.type != EventType.Layout)
            {
                List<ColonistBar.Entry> entries = BarHelperKf.Entries;
                int num = -1;
                bool showGroupFrames = ShowGroupFrames;
                for (int i = 0; i < BarHelperKf.cachedDrawLocs.Count; i++)
                {
                    Rect rect = new Rect(BarHelperKf.cachedDrawLocs[i].x, BarHelperKf.cachedDrawLocs[i].y, FullSize.x, FullSize.y + 12f);
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


        public static bool AnyColonistOrCorpseAt(Vector2 pos)
        {
            ColonistBar.Entry entry;
            return TryGetEntryAt(pos, out entry) && entry.pawn != null;
        }

        public static bool TryGetEntryAt(Vector2 pos, out ColonistBar.Entry entry)
        {
            List<Vector2> drawLocs = BarHelperKf.cachedDrawLocs;
            List<ColonistBar.Entry> entries = BarHelperKf.Entries;
            Vector2 size = FullSize;
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

        public static List<Thing> ColonistsOrCorpsesInScreenRect(Rect rect)
        {
            List<Vector2> drawLocs = BarHelperKf.cachedDrawLocs;
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
                        if (pawn.Dead && pawn.Corpse != null && pawn.Corpse.MapHeld != null)
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
        /*
                public void RecacheDrawLocs()
                {
                    CheckRecacheEntries();
                    Vector2 size = Size;
                    int colonistsPerRow = ColonistsPerRow;
                    float spacingHorizontal = SpacingHorizontal + WidthPSIHorizontal + WidthMoodBarHorizontal;
                    float spacingVertical = SpacingVertical + HeightPSIVertical + HeightMoodBarVertical;


                    float cachedDrawLocs_x = 0f + ColBarSettings.MarginHorizontal * Scale;
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

        public static float GetEntryRectAlpha(Rect rect)
        {
            float t;
            if (Messages.CollidesWithAnyMessage(rect, out t))
            {
                return Mathf.Lerp(1f, 0.2f, t);
            }
            return 1f;
        }





        // detour not working - JIT?!?
        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        public static void MarkColonistsDirty()
        {
            BarHelperKf.entriesDirty = true;
            //    Log.Message("Colonists marked dirty.01");
        }

        private static bool Visible
        {
            get
            {
                return UI.screenWidth >= 800 && UI.screenHeight >= 500;
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

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
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
    }
}
