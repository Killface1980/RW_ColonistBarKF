namespace ColonistBarKF.Bar
{
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using UnityEngine;

    using Verse;

    public class ColonistBarDrawLocsFinder_KF
    {
        private readonly List<int> entriesInGroup = new List<int>();

        private readonly List<int> horizontalSlotsPerGroup = new List<int>();

        private static float MaxColonistBarWidth => UI.screenWidth - Settings.barSettings.MarginHorizontal;

        private static float MaxColonistBarHeight => UI.screenHeight - Settings.barSettings.MarginVertical;

        public void CalculateDrawLocs([NotNull] List<Vector2> outDrawLocs, out float scale)
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
            if (Settings.barSettings.UseCustomRowCount)
            {
                switch (Settings.barSettings.MaxRowsCustom)
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
            List<EntryKF> entries = ColonistBar_KF.BarHelperKf.Entries;
            int num = this.CalculateGroupsCount();
            for (int i = 0; i < num; i++)
            {
                this.entriesInGroup.Add(0);
            }

            for (int j = 0; j < entries.Count; j++)
            {
                List<int> list;
                List<int> entryList = list = this.entriesInGroup;
                int num2;
                int entryGroup = num2 = entries[j].group;
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
                for (int i = 0; i < this.horizontalSlotsPerGroup.Count; i++)
                {
                    this.horizontalSlotsPerGroup[i] =
                        Mathf.Min(this.horizontalSlotsPerGroup[i], this.entriesInGroup[i]);
                }

                entriesCount = ColonistBar_KF.BarHelperKf.Entries.Count;
            }

            int groupsCount = this.CalculateGroupsCount();
            List<EntryKF> entries = ColonistBar_KF.BarHelperKf.Entries;
            int index = -1;
            int numInGroup = -1;

            float scaledEntryWidthFloat = (ColonistBar_KF.BaseSize.x + ColonistBar_KF.WidthSpacingHorizontal) * scale;
            float groupWidth = entriesCount * scaledEntryWidthFloat + (groupsCount - 1) * 25f * scale;
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
                    Settings.barSettings.MarginTop,
                    entries[j].group,
                    numInGroup,
                    scale);
                outDrawLocs.Add(drawLoc);
            }
        }

        private int CalculateGroupsCount()
        {
            List<EntryKF> entries = ColonistBar_KF.BarHelperKf.Entries;
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

        //   private bool horizontal = true;

        private float FindBestScale(out bool onlyOneRow, out int maxPerGlobalRow)
        {
            float bestScale = 1f;
            List<EntryKF> entries = ColonistBar_KF.BarHelperKf.Entries;
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
                        if (mapNum != entries[i].group)
                        {
                            mapNum = entries[i].group;
                            int rows = Mathf.CeilToInt(
                                this.entriesInGroup[entries[i].group]
                                / (float)this.horizontalSlotsPerGroup[entries[i].group]);
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
                    int maxSlots = this.horizontalSlotsPerGroup.Max();
                    if (maxSlots <= 1)
                    {
                        return false;
                    }

                    int num3 = this.horizontalSlotsPerGroup.IndexOf(maxSlots);
                    List<int> list;
                    List<int> listInt = list = this.horizontalSlotsPerGroup;
                    int num4;
                    int index = num4 = num3;
                    num4 = list[num4];
                    listInt[index] = num4 - 1;
                    List<int> list2;
                    List<int> slots = list2 = this.horizontalSlotsPerGroup;
                    int integerK = num4 = j;
                    num4 = list2[num4];
                    slots[integerK] = num4 + 1;
                }
            }

            return true;
        }
    }
}