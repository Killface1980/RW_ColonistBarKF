using System.Collections.Generic;
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
            if (ColonistBar_KF.BarHelperKf.Entries.Count == 0)
            {
                outDrawLocs.Clear();
                scale = 1f;
                return;
            }

            CalculateColonistsInGroup();

            scale = FindBestScale(out bool onlyOneRow, out int maxPerGlobalRow);

            CalculateDrawLocs(outDrawLocs, scale, onlyOneRow, maxPerGlobalRow);
        }

        // modded
        private static int GetAllowedRowsCountForScale(float scale)
        {
            if (Settings.BarSettings.UseCustomRowCount)
            {
                switch (Settings.BarSettings.MaxRowsCustom)
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
            _entriesInGroup.Clear();
            List<EntryKF> entries = ColonistBar_KF.BarHelperKf.Entries;
            int num = CalculateGroupsCount();
            for (int i = 0; i < num; i++)
            {
                _entriesInGroup.Add(0);
            }

            for (int j = 0; j < entries.Count; j++)
            {
                List<int> list;
                List<int> entryList = list = _entriesInGroup;
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
                for (int i = 0; i < _horizontalSlotsPerGroup.Count; i++)
                {
                    _horizontalSlotsPerGroup[i] =
                        Mathf.Min(_horizontalSlotsPerGroup[i], _entriesInGroup[i]);
                }

                entriesCount = ColonistBar_KF.BarHelperKf.Entries.Count;
            }

            int groupsCount = CalculateGroupsCount();
            List<EntryKF> entries = ColonistBar_KF.BarHelperKf.Entries;
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
                        groupStartX += _horizontalSlotsPerGroup[index] * scale
                                       * (ColonistBar_KF.BaseSize.x + ColonistBar_KF.WidthSpacingHorizontal);
                    }

                    numInGroup = 0;
                    index = entries[j].Group;
                }
                else
                {
                    numInGroup++;
                }

                Vector2 drawLoc = GetDrawLoc(
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
            List<EntryKF> entries = ColonistBar_KF.BarHelperKf.Entries;
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
            List<EntryKF> entries = ColonistBar_KF.BarHelperKf.Entries;
            int groupsCount = CalculateGroupsCount();
            while (true)
            {
                // float num3 = (ColonistBar.BaseSize.x + 24f) * num;
                float neededPerEntry = (ColonistBar_KF.BaseSize.x + ColonistBar_KF.WidthSpacingHorizontal) * bestScale;
                float availableScreen = MaxColonistBarWidth - ((groupsCount - 1) * 25f * bestScale);

                maxPerGlobalRow = Mathf.FloorToInt(availableScreen / neededPerEntry);
                onlyOneRow = true;
                if (TryDistributeHorizontalSlotsBetweenGroups(maxPerGlobalRow))
                {
                    int allowedRowsCountForScale = GetAllowedRowsCountForScale(bestScale);
                    bool flag = true;
                    int mapNum = -1;
                    for (int i = 0; i < entries.Count; i++)
                    {
                        if (mapNum != entries[i].Group)
                        {
                            mapNum = entries[i].Group;
                            int rows = Mathf.CeilToInt(
                                _entriesInGroup[entries[i].Group]
                                / (float)_horizontalSlotsPerGroup[entries[i].Group]);
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
            float x = groupStartX + numInGroup % _horizontalSlotsPerGroup[group] * scale
                      * (ColonistBar_KF.BaseSize.x + ColonistBar_KF.WidthSpacingHorizontal);
            float y = groupStartY + numInGroup / _horizontalSlotsPerGroup[group] * scale
                      * (ColonistBar_KF.BaseSize.y + ColonistBar_KF.HeightSpacingVertical);
            y += numInGroup / _horizontalSlotsPerGroup[group] * ColonistBar_KF.SpacingLabel;

            bool flag = numInGroup >= _entriesInGroup[group]
                        - _entriesInGroup[group] % _horizontalSlotsPerGroup[group];
            if (flag)
            {
                int num2 = _horizontalSlotsPerGroup[group]
                           - _entriesInGroup[group] % _horizontalSlotsPerGroup[group];
                x += num2 * scale * (ColonistBar_KF.BaseSize.x + ColonistBar_KF.WidthSpacingHorizontal) * 0.5f;
            }

            return new Vector2(x, y);
        }

        private bool TryDistributeHorizontalSlotsBetweenGroups(int maxPerGlobalRow)
        {
            int groupsCount = CalculateGroupsCount();
            _horizontalSlotsPerGroup.Clear();
            for (int k = 0; k < groupsCount; k++)
            {
                _horizontalSlotsPerGroup.Add(0);
            }

            GenMath.DHondtDistribution(
                _horizontalSlotsPerGroup,
                i => (float)_entriesInGroup[i],
                maxPerGlobalRow);
            for (int j = 0; j < _horizontalSlotsPerGroup.Count; j++)
            {
                if (_horizontalSlotsPerGroup[j] == 0)
                {
                    int maxSlots = _horizontalSlotsPerGroup.Max();
                    if (maxSlots <= 1)
                    {
                        return false;
                    }

                    int num3 = _horizontalSlotsPerGroup.IndexOf(maxSlots);
                    List<int> list;
                    List<int> listInt = list = _horizontalSlotsPerGroup;
                    int num4;
                    int index = num4 = num3;
                    num4 = list[num4];
                    listInt[index] = num4 - 1;
                    List<int> list2;
                    List<int> slots = list2 = _horizontalSlotsPerGroup;
                    int integerK = num4 = j;
                    num4 = list2[num4];
                    slots[integerK] = num4 + 1;
                }
            }

            return true;
        }
    }
}