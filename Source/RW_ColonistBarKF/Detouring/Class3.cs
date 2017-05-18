using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using static ColonistBarKF.CBKF;

namespace ColonistBarKF
{
    public class ColonistBarDrawLocsFinder_KF
    {
        public static float AllSpacingHor = ColonistBar_KF.SpacingHorizontal + ColonistBar_KF.SpacingMoodBarHorizontal + ColonistBar_KF.SpacingPSIHorizontal;

        private List<int> entriesInGroup = new List<int>();

        private List<int> horizontalSlotsPerGroup = new List<int>();


        private static float MaxColonistBarWidth
        {
            get
            {
                return Screen.width - ColBarSettings.MarginLeft - ColBarSettings.MarginRight;

            }
        }

        public void CalculateDrawLocs(List<Vector2> outDrawLocs, out float scale)
        {
            if (ColonistBar_KF.Entries.Count == 0)
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

        private void CalculateColonistsInGroup()
        {
            entriesInGroup.Clear();
            List<ColonistBar.Entry> entries = ColonistBar_KF.Entries;
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

        private int CalculateGroupsCount()
        {
            List<ColonistBar.Entry> entries = ColonistBar_KF.Entries;
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

        private float FindBestScale(out bool onlyOneRow, out int maxPerGlobalRow)
        {
            float bestScale = 1f;
            List<ColonistBar.Entry> entries = ColonistBar_KF.Entries;
            int groupsCount = CalculateGroupsCount();
            while (true)
            {
                float num3 = (ColonistBar_KF.BaseSize.x + 24f + AllSpacingHor) * bestScale;
                float num4 = MaxColonistBarWidth - (float)(groupsCount - 1) * 25f * bestScale;
                maxPerGlobalRow = Mathf.FloorToInt(num4 / num3);
                onlyOneRow = true;
                if (TryDistributeHorizontalSlotsBetweenGroups(maxPerGlobalRow))
                {
                    int allowedRowsCountForScale = GetAllowedRowsCountForScale(bestScale);
                    bool flag = true;
                    int num5 = -1;
                    for (int i = 0; i < entries.Count; i++)
                    {
                        if (num5 != entries[i].group)
                        {
                            num5 = entries[i].group;
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
                bestScale *= 0.95f;
            }
            return bestScale;
        }

        private bool TryDistributeHorizontalSlotsBetweenGroups(int maxPerGlobalRow)
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

        //modded
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

        private void CalculateDrawLocs(List<Vector2> outDrawLocs, float scale, bool onlyOneRow, int maxPerGlobalRow)
        {
            outDrawLocs.Clear();
            int entriesCount = maxPerGlobalRow;
            if (onlyOneRow)
            {
                for (int i = 0; i < horizontalSlotsPerGroup.Count; i++)
                {
                    horizontalSlotsPerGroup[i] = Mathf.Min(horizontalSlotsPerGroup[i], entriesInGroup[i]);
                }
                entriesCount = ColonistBar_KF.Entries.Count;
            }
            int groupsCount = CalculateGroupsCount();
            float scaledEntryWidthFloat = (ColonistBar_KF.BaseSize.x + 24f + AllSpacingHor) * scale;
            float colBarWidth = (float)entriesCount * scaledEntryWidthFloat + (float)(groupsCount - 1) * 25f * scale;
            List<ColonistBar.Entry> entries = ColonistBar_KF.Entries;
            int index = -1;
            int numInGroup = -1;
            float groupStartX = ((float)UI.screenWidth - colBarWidth) / 2f;
            for (int j = 0; j < entries.Count; j++)
            {
                if (index != entries[j].group)
                {
                    if (index >= 0)
                    {
                        groupStartX += 25f * scale;
                        groupStartX += (float)horizontalSlotsPerGroup[index] * scale * (ColonistBar_KF.BaseSize.x + 24f + AllSpacingHor);
                    }
                    numInGroup = 0;
                    index = entries[j].group;
                }
                else
                {
                    numInGroup++;
                }
                Vector2 drawLoc = GetDrawLoc(groupStartX, ColBarSettings.MarginTopHor, entries[j].group, numInGroup, scale);
                outDrawLocs.Add(drawLoc);
            }
        }

        private Vector2 GetDrawLoc(float groupStartX, float groupStartY, int group, int numInGroup, float scale)
        {
            float x = groupStartX + (float)(numInGroup % horizontalSlotsPerGroup[group]) * scale * (ColonistBar_KF.BaseSize.x + 24f + AllSpacingHor);
            float y = groupStartY + (float)(numInGroup / horizontalSlotsPerGroup[group]) * scale * (ColonistBar_KF.BaseSize.y + 32f);
            bool flag = numInGroup >= entriesInGroup[group] - entriesInGroup[group] % horizontalSlotsPerGroup[group];
            if (flag)
            {
                int num2 = horizontalSlotsPerGroup[group] - entriesInGroup[group] % horizontalSlotsPerGroup[group];
                x += (float)num2 * scale * (ColonistBar_KF.BaseSize.x + 24f + AllSpacingHor) * 0.5f;
            }
            return new Vector2(x, y);
        }
    }
}
