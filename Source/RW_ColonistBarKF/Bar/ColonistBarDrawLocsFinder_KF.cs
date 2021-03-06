﻿using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace ColonistBarKF.Bar
{
    public class ColonistBarDrawLocsFinder_Kf
    {
        private readonly List<int> _entriesInGroup = new List<int>();

        private readonly List<int> _horizontalSlotsPerGroup = new List<int>();

        private static float MaxColonistBarWidth => UI.screenWidth - Settings.BarSettings.MarginHorizontal;

        private static float MaxColonistBarHeight => UI.screenHeight - Settings.BarSettings.MarginVertical;

        public void CalculateDrawLocs([NotNull] List<Vector2> outDrawLocs, out float scale)
        {
            if (ColonistBar_KF.BarHelperKF.Entries.Count == 0)
            {
                outDrawLocs.Clear();
                scale = 1f;
                return;
            }

            this.CalculateColonistsInGroup();

            scale = this.FindBestScale(out bool onlyOneRow, out int maxPerGlobalRow);

            this.CalculateDrawLocs(outDrawLocs, scale, onlyOneRow, maxPerGlobalRow);
        }

        // modded
        private static int GetAllowedRowsCountForScale(float scale)
        {
            if (Settings.BarSettings.UseCustomRowCount)
            {
                int maxRowsCustom = Settings.BarSettings.MaxRowsCustom;

                return Mathf.RoundToInt(Mathf.Lerp(maxRowsCustom, 1f, scale));
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

        private void CalculateColonistsInGroup()
        {
            this._entriesInGroup.Clear();
            List<EntryKF> entries = ColonistBar_KF.BarHelperKF.Entries;
            int num = this.CalculateGroupsCount();
            for (int i = 0; i < num; i++)
            {
                this._entriesInGroup.Add(0);
            }

            for (int j = 0; j < entries.Count; j++)
            {
                List<int> list;
                List<int> entryList = list = this._entriesInGroup;
                int num2;
                int entryGroup = num2 = entries[j].Group;
                num2 = list[num2];
                entryList[entryGroup] = num2 + 1;
            }
        }

        private void CalculateDrawLocs([NotNull] List<Vector2> outDrawLocs, float scale, bool onlyOneRow, int maxPerGlobalRow)
        {
            outDrawLocs.Clear();
            int entriesCount = maxPerGlobalRow;
            if (onlyOneRow)
            {
                for (int i = 0; i < this._horizontalSlotsPerGroup.Count; i++)
                {
                    this._horizontalSlotsPerGroup[i] =
                        Mathf.Min(this._horizontalSlotsPerGroup[i], this._entriesInGroup[i]);
                }

                entriesCount = ColonistBar_KF.BarHelperKF.Entries.Count;
            }

            int groupsCount = this.CalculateGroupsCount();
            List<EntryKF> entries = ColonistBar_KF.BarHelperKF.Entries;
            int index = -1;
            int numInGroup = -1;

            float scaledEntryWidthFloat = (ColonistBar_KF.BaseSize.x + ColonistBar_KF.WidthSpacingHorizontal) * scale;
            float groupWidth = entriesCount * scaledEntryWidthFloat + (groupsCount - 1) * 25f * scale;
            float groupStartX = (UI.screenWidth - groupWidth) / 2f;

            for (int j = 0; j < entries.Count; j++)
            {
                if (index != entries[j].Group)
                {
                    if (index >= 0)
                    {
                        groupStartX += 25f * scale;
                        groupStartX += this._horizontalSlotsPerGroup[index] * scale
                                       * (ColonistBar_KF.BaseSize.x + ColonistBar_KF.WidthSpacingHorizontal);
                    }

                    numInGroup = 0;
                    index = entries[j].Group;
                }
                else
                {
                    numInGroup++;
                }

                Vector2 drawLoc = this.GetDrawLoc(
                    groupStartX,
                    Settings.BarSettings.MarginTop,
                    entries[j].Group,
                    numInGroup,
                    scale);
                outDrawLocs.Add(drawLoc);
            }
        }

        private int CalculateGroupsCount()
        {
            List<EntryKF> entries = ColonistBar_KF.BarHelperKF.Entries;
            int num = -1;
            int num2 = 0;
            for (int i = 0; i < entries.Count; i++)
            {
                if (num != entries[i].Group)
                {
                    num2++;
                    num = entries[i].Group;
                }
            }

            return num2;
        }

        //   private bool horizontal = true;

        private float FindBestScale(out bool onlyOneRow, out int maxPerGlobalRow)
        {
            float bestScale = 1f;
            List<EntryKF> entries = ColonistBar_KF.BarHelperKF.Entries;
            int groupsCount = this.CalculateGroupsCount();
            while (true)
            {
                // float num3 = (ColonistBar.BaseSize.x + 24f) * num;
                float neededPerEntry = (ColonistBar_KF.BaseSize.x + ColonistBar_KF.WidthSpacingHorizontal) * bestScale;
                float availableScreen = MaxColonistBarWidth - ((groupsCount - 1) * 25f * bestScale);

                maxPerGlobalRow = Mathf.FloorToInt(availableScreen / neededPerEntry);
                onlyOneRow = true;
                if (this.TryDistributeHorizontalSlotsBetweenGroups(maxPerGlobalRow))
                {
                    int allowedRowsCountForScale = GetAllowedRowsCountForScale(bestScale);
                    bool flag = true;
                    int mapNum = -1;
                    for (int i = 0; i < entries.Count; i++)
                    {
                        if (mapNum != entries[i].Group)
                        {
                            mapNum = entries[i].Group;
                            int rows = Mathf.CeilToInt(this._entriesInGroup[entries[i].Group]
                                / (float)this._horizontalSlotsPerGroup[entries[i].Group]);
                            if (rows > 1)
                            {
                                onlyOneRow = false;
                            }

                            if (rows > allowedRowsCountForScale)
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

                bestScale -= 0.03f;
            }

            return bestScale;
        }

        private Vector2 GetDrawLoc(float groupStartX, float groupStartY, int group, int numInGroup, float scale)
        {
            float x = groupStartX + numInGroup % this._horizontalSlotsPerGroup[group] * scale
                      * (ColonistBar_KF.BaseSize.x + ColonistBar_KF.WidthSpacingHorizontal);
            float y = groupStartY + numInGroup / this._horizontalSlotsPerGroup[group] * scale
                      * (ColonistBar_KF.BaseSize.y + ColonistBar_KF.HeightSpacingVertical);
            y += numInGroup / this._horizontalSlotsPerGroup[group] * ColonistBar_KF.SpacingLabel;

            bool flag = numInGroup >= this._entriesInGroup[group]
                        - this._entriesInGroup[group] % this._horizontalSlotsPerGroup[group];
            if (flag)
            {
                int num2 = this._horizontalSlotsPerGroup[group]
                           - this._entriesInGroup[group] % this._horizontalSlotsPerGroup[group];
                x += num2 * scale * (ColonistBar_KF.BaseSize.x + ColonistBar_KF.WidthSpacingHorizontal) * 0.5f;
            }

            return new Vector2(x, y);
        }

        private bool TryDistributeHorizontalSlotsBetweenGroups(int maxPerGlobalRow)
        {
            int groupsCount = this.CalculateGroupsCount();
            this._horizontalSlotsPerGroup.Clear();
            for (int k = 0; k < groupsCount; k++)
            {
                this._horizontalSlotsPerGroup.Add(0);
            }

            GenMath.DHondtDistribution(this._horizontalSlotsPerGroup,
                i => (float)this._entriesInGroup[i],
                maxPerGlobalRow);
            for (int j = 0; j < this._horizontalSlotsPerGroup.Count; j++)
            {
                if (this._horizontalSlotsPerGroup[j] == 0)
                {
                    int maxSlots = this._horizontalSlotsPerGroup.Max();
                    if (maxSlots <= 1)
                    {
                        return false;
                    }

                    int num3 = this._horizontalSlotsPerGroup.IndexOf(maxSlots);
                    List<int> list;
                    List<int> listInt = list = this._horizontalSlotsPerGroup;
                    int num4;
                    int index = num4 = num3;
                    num4 = list[num4];
                    listInt[index] = num4 - 1;
                    List<int> list2;
                    List<int> slots = list2 = this._horizontalSlotsPerGroup;
                    int integerK = num4 = j;
                    num4 = list2[num4];
                    slots[integerK] = num4 + 1;
                }
            }

            return true;
        }
    }
}