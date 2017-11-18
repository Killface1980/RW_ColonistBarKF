namespace ColonistBarKF.PSI
{
    using System.Collections.Generic;

    using ColonistBarKF.Bar;

    using JetBrains.Annotations;

    using UnityEngine;

    using Verse;

    public static class BarIconDrawer
    {
        #region Private Fields

        [NotNull]
        private static Vector3[] iconPosRectsBar;

        #endregion Private Fields

        #region Public Methods

        public static void DrawColonistIconsBar([NotNull]this Pawn pawn, Rect psiRect, float rectAlpha)
        {
            CompPSI pawnStats = pawn.GetComp<CompPSI>();

            if (pawn.Dead || !pawn.Spawned || pawn.holdingOwner == null || pawn.Map == null)
            {
                pawnStats.thisColCount = 0;
                return;
            }

            SettingsColonistBar colBarSettings = Settings.barSettings;
            int barIconNum = 0;
            int rowCount = pawnStats.thisColCount;

            // Drafted
            if (colBarSettings.ShowDraft && pawn.Drafted)
            {
                if (pawnStats.isPacifist)
                {
                    DrawIconOnBar(psiRect, ref barIconNum, Icon.Draft, Textures.ColYellow, rectAlpha, rowCount);
                }
                else
                {
                    DrawIconOnBar(psiRect, ref barIconNum, Icon.Draft, Textures.ColVermillion, rectAlpha, rowCount);
                }
            }

            List<IconEntryBar> drawIconEntries = pawnStats.BarIconList;
            if (!pawnStats.BarIconList.NullOrEmpty())
            {
                int maxIconCount = Mathf.Min(
                    Settings.barSettings.IconsInColumn * 2,
                    drawIconEntries.Count + barIconNum);
                for (int index = 0; index < maxIconCount - barIconNum; index++)
                {
                    IconEntryBar iconEntryBar = drawIconEntries[index];
                    iconEntryBar.color.a *= rectAlpha;
                    DrawIconOnBar(psiRect, iconEntryBar, index + barIconNum, rowCount);
                }

                barIconNum += maxIconCount;
            }

            // Idle - bar icon already included - vanilla
            int colCount = Mathf.CeilToInt((float)barIconNum / Settings.barSettings.IconsInColumn);

            pawnStats.thisColCount = colCount;
        }

        public static void RecalcBarPositionAndSize()
        {
            SettingsColonistBar settings = Settings.barSettings;
            iconPosRectsBar = new Vector3[40];
            for (int index = 0; index < iconPosRectsBar.Length; ++index)
            {
                int num1;
                int num2;
                num1 = index / settings.IconsInColumn;
                num2 = index % settings.IconsInColumn;
                if (settings.IconsHorizontal)
                {
                    int num3 = num1;
                    num1 = num2;
                    num2 = num3;

                    // num2 = index / ColBarSettings.IconsInColumn;
                    // num1 = index % ColBarSettings.IconsInColumn;
                }

                iconPosRectsBar[index] = new Vector3(-num1, 3f, num2);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static void DrawIcon_onBar(
            Rect rect,
            Vector3 posOffset,
            [NotNull] Material material,
            Color color,
            float rectAlpha,
            int rowCount,
            [CanBeNull] string tooltip = null)
        {
            // Widgets.DrawBoxSolid(rect, Color.cyan);
            color.a *= rectAlpha;
            Color GuiColor = GUI.color;
            GuiColor.a = rectAlpha;
            GUI.color = GuiColor;

            material.color = color;

            Rect iconRect = new Rect(rect);

            float size = Mathf.Min(iconRect.width, iconRect.height) / rowCount;

            iconRect.height = iconRect.width = size;

            switch (Settings.barSettings.ColBarPsiIconPos)
            {
                case Position.Alignment.Left:
                    iconRect.x = rect.xMax - size;
                    iconRect.y = rect.yMax - size;
                    break;

                case Position.Alignment.Right:
                    iconRect.x = rect.xMin;
                    iconRect.y = rect.yMax - size;
                    break;

                case Position.Alignment.Top:
                    iconRect.y = rect.yMax - size;
                    break;

                case Position.Alignment.Bottom:
                    iconRect.y = rect.yMin;
                    break;
            }

            // iconRect.x += (-0.5f * CBKF.ColBarSettings.IconMarginX - 0.5f  * CBKF.ColBarSettings.IconOffsetX) * iconRect.width;
            // iconRect.y -= (-0.5f * CBKF.ColBarSettings.IconDistanceY + 0.5f  * CBKF.ColBarSettings.IconOffsetY) * iconRect.height;
            iconRect.x += Settings.barSettings.IconOffsetX * posOffset.x * size;
            iconRect.y -= Settings.barSettings.IconOffsetY * posOffset.z * iconRect.height;

            // On Colonist
            // iconRect.x -= iconRect.width * 0.5f;
            // iconRect.y -= iconRect.height * 0.5f;
            GUI.DrawTexture(iconRect, Textures.BgTexIconPSI);
            GUI.color = color;

            iconRect.x += size * 0.1f;
            iconRect.y += iconRect.height * 0.1f;
            iconRect.width *= 0.8f;
            iconRect.height *= 0.8f;

            GUI.DrawTexture(iconRect, material.mainTexture, ScaleMode.ScaleToFit, true);
            GUI.color = GuiColor;

            if (tooltip != null)
            {
                TooltipHandler.TipRegion(iconRect, tooltip);
            }
        }

        private static void DrawIconOnBar(
                            Rect psiRect,
            ref int num,
            Icon icon,
            Color color,
            float rectAlpha,
            int rowCount,
            [CanBeNull] string tooltip = null)
        {
            // only two columns visible
            if (num == Settings.barSettings.IconsInColumn * 2)
            {
                return;
            }

            Material material = GameComponentPSI.PSIMaterials[icon];

            if (material == null)
            {
                return;
            }

            DrawIcon_onBar(psiRect, iconPosRectsBar[num], material, color, rectAlpha, rowCount, tooltip);

            num++;
        }

        private static void DrawIconOnBar(Rect psiRect, IconEntryBar iconEntryBar, int entry, int rowCount)
        {
            Material material = GameComponentPSI.PSIMaterials[iconEntryBar.icon];

            if (material == null)
            {
                return;
            }

            Vector3 posOffset = iconPosRectsBar[entry];

            Color GuiColor = GUI.color;
            GuiColor.a = iconEntryBar.color.a;
            GUI.color = GuiColor;

            material.color = iconEntryBar.color;

            Rect iconRect = new Rect(psiRect);

            float size = Mathf.Min(iconRect.width, iconRect.height) / rowCount;

            iconRect.height = iconRect.width = size;

            switch (Settings.barSettings.ColBarPsiIconPos)
            {
                case Position.Alignment.Left:
                    iconRect.x = psiRect.xMax - size;
                    iconRect.y = psiRect.yMax - size;
                    break;

                case Position.Alignment.Right:
                    iconRect.x = psiRect.xMin;
                    iconRect.y = psiRect.yMax - size;
                    break;

                case Position.Alignment.Top:
                    iconRect.y = psiRect.yMax - size;
                    break;

                case Position.Alignment.Bottom:
                    iconRect.y = psiRect.yMin;
                    break;
            }

            // iconRect.x += (-0.5f * CBKF.ColBarSettings.IconMarginX - 0.5f  * CBKF.ColBarSettings.IconOffsetX) * iconRect.width;
            // iconRect.y -= (-0.5f * CBKF.ColBarSettings.IconDistanceY + 0.5f  * CBKF.ColBarSettings.IconOffsetY) * iconRect.height;
            iconRect.x += Settings.barSettings.IconOffsetX * posOffset.x * size;
            iconRect.y -= Settings.barSettings.IconOffsetY * posOffset.z * iconRect.height;

            // On Colonist
            // iconRect.x -= iconRect.width * 0.5f;
            // iconRect.y -= iconRect.height * 0.5f;
            GUI.DrawTexture(iconRect, Textures.BgTexIconPSI);
            GUI.color = iconEntryBar.color;

            iconRect.x += size * 0.1f;
            iconRect.y += iconRect.height * 0.1f;
            iconRect.width *= 0.8f;
            iconRect.height *= 0.8f;

            GUI.DrawTexture(iconRect, material.mainTexture, ScaleMode.ScaleToFit, true);
            GUI.color = GuiColor;

            if (iconEntryBar.tooltip != null)
            {
                TooltipHandler.TipRegion(iconRect, iconEntryBar.tooltip);
            }
        }

        #endregion Private Methods
    }
}