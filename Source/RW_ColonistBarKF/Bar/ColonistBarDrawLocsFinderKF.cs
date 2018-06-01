using System;
using System.Collections.Generic;
using System.Linq;
using ColonistBarKF.PSI;
using RimWorld;
using UnityEngine;
using Verse;
using static ColonistBarKF.CBKF;
using static ColonistBarKF.ColonistBar_KF;

namespace ColonistBarKF
{
    public static class ColonistBarDrawLocsFinderKF
    {
        private static List<int> entriesInGroup = new List<int>();
        private static List<int> horizontalSlotsPerGroup = new List<int>();
        // RimWorld.ColonistBarColonistDrawer

        public static int PsiRowsOnBar
        {
            get
            {
                int psiRowsOnBar = Mathf.CeilToInt((float)IconCount / ColBarSettings.IconsInColumn);
                return psiRowsOnBar;
            }
        }

        public static float SpacingHorizontal => SpacingHorizontalAssumingScale(Scale);

        public static float SpacingVertical => SpacingVerticalAssumingScale(Scale);

        public static float SpacingPSIHorizontal => SpacingHorizontalPSI();

        public static float SpacingPSIVertical => SpacingVerticalPSIAssumingScale(Scale);

        public static float SpacingMoodBarVertical => SpacingVerticalgMoodBarAssumingScale(Scale);

        public static float SpacingMoodBarHorizontal => SpacingHorizontalMoodBar();

        private static float SpacingHorizontalMoodBar()
        {
            if (ColBarSettings.UseExternalMoodBar && (ColBarSettings.MoodBarPos == Position.Alignment.Left || ColBarSettings.MoodBarPos == Position.Alignment.Right))
                return ColBarSettings.BaseSizeFloat / 4;

            return 0f;
        }

        private static float SpacingVerticalgMoodBarAssumingScale(float scale)
        {
            if (ColBarSettings.UseExternalMoodBar &&
                (ColBarSettings.MoodBarPos == Position.Alignment.Bottom || ColBarSettings.MoodBarPos == Position.Alignment.Top))
                return ColBarSettings.BaseSizeFloat / 4 * scale;
            return 0f;
        }

        public static int ColonistsPerRow => ColonistsPerRowAssumingScale(Scale);

        internal static Vector2 SizeAssumingScale(float scale)
        {
            BaseSize.x = ColBarSettings.BaseSizeFloat;
            BaseSize.y = ColBarSettings.BaseSizeFloat;
            return BaseSize * scale;
        }

        public static int RowsCountAssumingScale(float scale)
        {
            return Mathf.CeilToInt(cachedDrawLocs.Count / (float)ColonistsPerRowAssumingScale(scale));
        }

        private static int ColonistsPerRowAssumingScale(float scale)
        {
            {
                ColBarSettings.MaxColonistBarWidth = Screen.width - ColBarSettings.MarginLeftHorTop - ColBarSettings.MarginRightHorTop;
                ColBarSettings.HorizontalOffset = ColBarSettings.MarginLeftHorTop / 2 - ColBarSettings.MarginRightHorTop / 2;

            }
            return Mathf.FloorToInt((ColBarSettings.MaxColonistBarWidth) / (SizeAssumingScale(scale).x + SpacingHorizontalAssumingScale(scale) + SpacingHorizontalPSI() + SpacingHorizontalMoodBar()));
        }


        private static float SpacingHorizontalAssumingScale(float scale)
        {
            return ColBarSettings.BaseSpacingHorizontal * scale;
        }

        private static float SpacingVerticalAssumingScale(float scale)
        {
            return ColBarSettings.BaseSpacingVertical * scale;
        }

        private static float SpacingHorizontalPSI()
        {
            if (ColBarSettings.UsePSI)
                if (ColBarSettings.ColBarPsiIconPos == Position.Alignment.Left || ColBarSettings.ColBarPsiIconPos == Position.Alignment.Right)
                {

                    return ColBarSettings.BaseSizeFloat / ColBarSettings.IconsInColumn * PsiRowsOnBar;
                }
            return 0f;
        }

        private static float SpacingVerticalPSIAssumingScale(float scale)
        {
            if (ColBarSettings.UsePSI)
                if (ColBarSettings.ColBarPsiIconPos == Position.Alignment.Bottom || ColBarSettings.ColBarPsiIconPos == Position.Alignment.Top)
                {
                    return ColBarSettings.BaseSizeFloat / ColBarSettings.IconsInColumn * scale * PsiRowsOnBar;
                }
            return 0f;
        }


        // RimWorld.ColonistBarDrawLocsFinder
        public static void CalculateDrawLocs(List<Vector2> outDrawLocs, out float scale)
        {
            if (Entries.Count == 0)
            {
                outDrawLocs.Clear();
                scale = 1f;
                return;
            }
            CalculateColonistsInGroup();
            bool onlyOneRow;
            int maxPerGlobalRow;
            scale = FindBestScale(out onlyOneRow, out maxPerGlobalRow);
            CalculateDrawLocs(outDrawLocs, scale, onlyOneRow, maxPerGlobalRow);
        }

        // RimWorld.ColonistBarDrawLocsFinder
        private static float FindBestScale(out bool onlyOneRow, out int maxPerGlobalRow)
        {
            float findBestScale = 1f;


            List<EntryKF> entries = Entries;
            int calculateGroupsCount = CalculateGroupsCount();
            while (true)
            {
                float suggestedSize = (BaseSize.x + 24f) * findBestScale;
                float num4 = MaxColonistBarWidth - (float)(calculateGroupsCount - 1) * 25f * findBestScale;
                maxPerGlobalRow = Mathf.FloorToInt(num4 / suggestedSize);
                if (!ColBarSettings.UseCustomRowCount)
                    onlyOneRow = false;
                else
                    onlyOneRow = true;

                if (ColBarSettings.UseFixedIconScale)
                {
                    if (ColBarSettings.UseCustomRowCount && ColBarSettings.MaxRowsCustom != 1)
                        onlyOneRow = false;

                    return ColBarSettings.FixedIconScaleFloat;
                }


                if (TryDistributeHorizontalSlotsBetweenGroups(maxPerGlobalRow))
                {
                    int allowedRowsCountForScale = GetAllowedRowsCountForScale(findBestScale);
                    bool flag = true;
                    int @group = -1;
                    for (int i = 0; i < entries.Count; i++)
                    {
                        if (@group != entries[i].group)
                        {
                            @group = entries[i].group;
                            int num6 = Mathf.CeilToInt((float)entriesInGroup[entries[i].group] / (float)horizontalSlotsPerGroup[entries[i].group]);
                            if (num6 > 1)
                            {
                                onlyOneRow = false;
                            }
                            if (num6 > allowedRowsCountForScale)
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
                findBestScale *= 0.95f;
            }
            return findBestScale;
        }

        public static int GetAllowedRowsCountForScale(float scale)
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


        // RimWorld.ColonistBarDrawLocsFinder
        private static bool TryDistributeHorizontalSlotsBetweenGroups(int maxPerGlobalRow)
        {
            int num = CalculateGroupsCount();
            horizontalSlotsPerGroup.Clear();
            for (int k = 0; k < num; k++)
            {
                horizontalSlotsPerGroup.Add(0);
            }
            GenMath.DHondtDistribution(horizontalSlotsPerGroup, (int i) => (float)entriesInGroup[i], maxPerGlobalRow);
            for (int j = 0; j < horizontalSlotsPerGroup.Count; j++)
            {
                if (horizontalSlotsPerGroup[j] == 0)
                {
                    int num2 = horizontalSlotsPerGroup.Max();
                    if (num2 <= 1)
                    {
                        return false;
                    }
                    int num3 = horizontalSlotsPerGroup.IndexOf(num2);
                    List<int> list;
                    List<int> expr_89 = list = horizontalSlotsPerGroup;
                    int num4;
                    int expr_8E = num4 = num3;
                    num4 = list[num4];
                    expr_89[expr_8E] = num4 - 1;
                    List<int> list2;
                    List<int> expr_AB = list2 = horizontalSlotsPerGroup;
                    int expr_AF = num4 = j;
                    num4 = list2[num4];
                    expr_AB[expr_AF] = num4 + 1;
                }
            }
            return true;
        }


        // RimWorld.ColonistBarDrawLocsFinder
        private static float MaxColonistBarWidth
        {
            get
            {
                return (float)UI.screenWidth - 520f;
            }
        }


        // RimWorld.ColonistBarDrawLocsFinder
        private static void CalculateDrawLocs(List<Vector2> outDrawLocs, float scale, bool onlyOneRow, int maxPerGlobalRow)
        {
            outDrawLocs.Clear();
            int perGlobalRow = maxPerGlobalRow;
            if (onlyOneRow)
            {
                for (int i = 0; i < horizontalSlotsPerGroup.Count; i++)
                {
                    horizontalSlotsPerGroup[i] = Mathf.Min(horizontalSlotsPerGroup[i], entriesInGroup[i]);
                }
                perGlobalRow = Entries.Count;
            }
            int groupsCount = CalculateGroupsCount();
            float scaledSize = (ColBarSettings.BaseSizeFloat + ColBarSettings.BaseSpacingHorizontal + SpacingMoodBarHorizontal + SpacingPSIHorizontal) * scale;
            float barWidth = (float)perGlobalRow * scaledSize + (float)(groupsCount - 1) * 25f * scale;
            List<EntryKF> entries = Entries;
            int @group = -1;
            int numInGroup = -1;
            float groupStartX = ((float)UI.screenWidth - barWidth) / 2f;
            for (int j = 0; j < entries.Count; j++)
            {
                if (@group != entries[j].group)
                {
                    if (@group >= 0)
                    {
                        groupStartX += 25f * scale;
                        groupStartX += (float)horizontalSlotsPerGroup[@group] * scale * (BaseSize.x + 24f);
                    }
                    numInGroup = 0;
                    @group = entries[j].group;
                }
                else
                {
                    numInGroup++;
                }
                Vector2 drawLoc = GetDrawLoc(groupStartX, ColBarSettings.MarginTopHor, entries[j].group, numInGroup, scale);
                outDrawLocs.Add(drawLoc);
            }
        }

        // RimWorld.ColonistBarDrawLocsFinder
        private static Vector2 GetDrawLoc(float groupStartX, float groupStartY, int group, int numInGroup, float scale)
        {
            float num = groupStartX + (float)(numInGroup % horizontalSlotsPerGroup[group]) * scale * (BaseSize.x + 24f);
            float y = groupStartY + (float)(numInGroup / horizontalSlotsPerGroup[group]) * scale * (BaseSize.y + 32f);
            bool flag = numInGroup >= entriesInGroup[group] - entriesInGroup[group] % horizontalSlotsPerGroup[group];
            if (flag)
            {
                int num2 = horizontalSlotsPerGroup[group] - entriesInGroup[group] % horizontalSlotsPerGroup[group];
                num += (float)num2 * scale * (BaseSize.x + 24f) * 0.5f;
            }
            return new Vector2(num, y);
        }



        private static void CalculateColonistsInGroup()
        {
            entriesInGroup.Clear();
            List<EntryKF> entries = Entries;
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

        private static int CalculateGroupsCount()
        {
            List<EntryKF> entries = Entries;
            int num = -1;
            int num2 = 0;
            for (int i = 0; i < entries.Count; i++)
            {
                if (num != entries[i].@group)
                {
                    num2++;
                    num = entries[i].@group;
                }
            }
            return num2;
        }
    }
}
