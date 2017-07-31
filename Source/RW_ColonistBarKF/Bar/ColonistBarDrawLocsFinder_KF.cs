namespace ColonistBarKF.Bar
{
    using System.Collections.Generic;
    using System.Linq;

    using RimWorld;

    using static Settings;

    using UnityEngine;

    using Verse;

    public class ColonistBarDrawLocsFinder_KF
    {
        #region Fields

        private readonly List<int> entriesInGroup = new List<int>();

        private readonly List<int> horizontalSlotsPerGroup = new List<int>();

        #endregion Fields

        #region Properties

        private static float MaxColonistBarWidth => UI.screenWidth - ColBarSettings.MarginHorizontal;

        #endregion Properties

        #region Methods

        public void CalculateDrawLocs(List<Vector2> outDrawLocs, out float scale)
        {
            if (ColonistBar_KF.BarHelperKf.Entries.Count == 0)
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

        private void CalculateColonistsInGroup()
        {
            this.entriesInGroup.Clear();
            List<ColonistBar.Entry> entries = ColonistBar_KF.BarHelperKf.Entries;
            int num = this.CalculateGroupsCount();
            for (int i = 0; i < num; i++)
            {
                this.entriesInGroup.Add(0);
            }

            for (int j = 0; j < entries.Count; j++)
            {
                List<int> list;
                List<int> expr_49 = list = this.entriesInGroup;
                int num2;
                int expr_5C = num2 = entries[j].group;
                num2 = list[num2];
                expr_49[expr_5C] = num2 + 1;
            }
        }

        private void CalculateDrawLocs(List<Vector2> outDrawLocs, float scale, bool onlyOneRow, int maxPerGlobalRow)
        {
            outDrawLocs.Clear();
            int entriesCount = maxPerGlobalRow;
            if (onlyOneRow)
            {
                for (int i = 0; i < this.horizontalSlotsPerGroup.Count; i++)
                {
                    this.horizontalSlotsPerGroup[i] =
                        Mathf.Min(this.horizontalSlotsPerGroup[i], this.entriesInGroup[i]);
                }

                entriesCount = ColonistBar_KF.BarHelperKf.Entries.Count;
            }

            int groupsCount = this.CalculateGroupsCount();
            float scaledEntryWidthFloat = (ColonistBar_KF.BaseSize.x + ColonistBar_KF.WidthSpacingHorizontal) * scale;
            float groupWidth = entriesCount * scaledEntryWidthFloat + (groupsCount - 1) * 25f * scale;
            List<ColonistBar.Entry> entries = ColonistBar_KF.BarHelperKf.Entries;
            int index = -1;
            int numInGroup = -1;
            float groupStartX = (UI.screenWidth - groupWidth) / 2f;
            for (int j = 0; j < entries.Count; j++)
            {
                if (index != entries[j].group)
                {
                    if (index >= 0)
                    {
                        groupStartX += 25f * scale;
                        groupStartX += this.horizontalSlotsPerGroup[index] * scale
                                       * (ColonistBar_KF.BaseSize.x + ColonistBar_KF.WidthSpacingHorizontal);
                    }

                    numInGroup = 0;
                    index = entries[j].group;
                }
                else
                {
                    numInGroup++;
                }

                Vector2 drawLoc = this.GetDrawLoc(
                    groupStartX,
                    ColBarSettings.MarginTop,
                    entries[j].group,
                    numInGroup,
                    scale);
                outDrawLocs.Add(drawLoc);
            }
        }

        private int CalculateGroupsCount()
        {
            List<ColonistBar.Entry> entries = ColonistBar_KF.BarHelperKf.Entries;
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
            List<ColonistBar.Entry> entries = ColonistBar_KF.BarHelperKf.Entries;
            int groupsCount = this.CalculateGroupsCount();
            while (true)
            {
                // float num3 = (ColonistBar.BaseSize.x + 24f) * num;
                float neededPerEntry = (ColonistBar_KF.BaseSize.x + ColonistBar_KF.WidthSpacingHorizontal) * bestScale;
                float availableScreen = MaxColonistBarWidth - (groupsCount - 1) * 25f * bestScale;
                maxPerGlobalRow = Mathf.FloorToInt(availableScreen / neededPerEntry);
                onlyOneRow = true;
                if (this.TryDistributeHorizontalSlotsBetweenGroups(maxPerGlobalRow))
                {
                    int allowedRowsCountForScale = GetAllowedRowsCountForScale(bestScale);
                    bool flag = true;
                    int num5 = -1;
                    for (int i = 0; i < entries.Count; i++)
                    {
                        if (num5 != entries[i].group)
                        {
                            num5 = entries[i].group;
                            int num6 = Mathf.CeilToInt(
                                this.entriesInGroup[entries[i].group]
                                / (float)this.horizontalSlotsPerGroup[entries[i].group]);
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

                bestScale -= 0.01f;
            }

            return bestScale;
        }

        private Vector2 GetDrawLoc(float groupStartX, float groupStartY, int group, int numInGroup, float scale)
        {
            float x = groupStartX + numInGroup % this.horizontalSlotsPerGroup[group] * scale
                      * (ColonistBar_KF.BaseSize.x + ColonistBar_KF.WidthSpacingHorizontal);
            float y = groupStartY + numInGroup / this.horizontalSlotsPerGroup[group] * scale
                      * (ColonistBar_KF.BaseSize.y + ColonistBar_KF.HeightSpacingVertical);
            y += numInGroup / this.horizontalSlotsPerGroup[group] * ColonistBar_KF.SpacingLabel;

            bool flag = numInGroup >= this.entriesInGroup[group]
                        - this.entriesInGroup[group] % this.horizontalSlotsPerGroup[group];
            if (flag)
            {
                int num2 = this.horizontalSlotsPerGroup[group]
                           - this.entriesInGroup[group] % this.horizontalSlotsPerGroup[group];
                x += num2 * scale * (ColonistBar_KF.BaseSize.x + ColonistBar_KF.WidthSpacingHorizontal) * 0.5f;
            }

            return new Vector2(x, y);
        }

        private bool TryDistributeHorizontalSlotsBetweenGroups(int maxPerGlobalRow)
        {
            int groupsCount = this.CalculateGroupsCount();
            this.horizontalSlotsPerGroup.Clear();
            for (int k = 0; k < groupsCount; k++)
            {
                this.horizontalSlotsPerGroup.Add(0);
            }

            GenMath.DHondtDistribution(
                this.horizontalSlotsPerGroup,
                i => (float)this.entriesInGroup[i],
                maxPerGlobalRow);
            for (int j = 0; j < this.horizontalSlotsPerGroup.Count; j++)
            {
                if (this.horizontalSlotsPerGroup[j] == 0)
                {
                    int num2 = this.horizontalSlotsPerGroup.Max();
                    if (num2 <= 1)
                    {
                        return false;
                    }

                    int num3 = this.horizontalSlotsPerGroup.IndexOf(num2);
                    List<int> list;
                    List<int> listInt = list = this.horizontalSlotsPerGroup;
                    int num4;
                    int index = num4 = num3;
                    num4 = list[num4];
                    listInt[index] = num4 - 1;
                    List<int> list2;
                    List<int> expr_AB = list2 = this.horizontalSlotsPerGroup;
                    int integerK = num4 = j;
                    num4 = list2[num4];
                    expr_AB[integerK] = num4 + 1;
                }
            }

            return true;
        }

        #endregion Methods
    }
}