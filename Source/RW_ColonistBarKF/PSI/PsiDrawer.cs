using static ColonistBarKF.PSI.PSI;

namespace ColonistBarKF.PSI
{
    using ColonistBarKF.Bar;

    using RimWorld.Planet;

    using UnityEngine;

    using Verse;

    public static class PSIDrawer
    {
        public static Gradient gradient4 = new Gradient();

        public static Gradient gradient4Mood = new Gradient();

        public static Gradient gradientRedAlertToNeutral = new Gradient();

        public static void DrawIcon_posOffset(
            Vector3 bodyPos,
            Vector3 posOffset,
            Material material,
            Color color,
            float opacity)
        {
            color.a = opacity;
            material.color = color;
            Color guiColor = GUI.color;
            GUI.color = color;
            Vector2 vectorAtBody;

            float worldScale = WorldScale;
            if (Settings.PsiSettings.IconsScreenScale)
            {
                worldScale = 45f;
                vectorAtBody = bodyPos.MapToUIPosition();
                vectorAtBody.x += posOffset.x * 45f;
                vectorAtBody.y -= posOffset.z * 45f;
            }
            else
            {
                vectorAtBody = (bodyPos + posOffset).MapToUIPosition();
            }

            float num2 = worldScale * (Settings.PsiSettings.IconSizeMult * 0.5f);

            // On Colonist
            Rect position = new Rect(
                vectorAtBody.x,
                vectorAtBody.y,
                num2 * Settings.PsiSettings.IconSize,
                num2 * Settings.PsiSettings.IconSize);
            position.x -= position.width * 0.5f;
            position.y -= position.height * 0.5f;

            GUI.DrawTexture(position, material.mainTexture, ScaleMode.ScaleToFit, true);
            GUI.color = guiColor;
        }

        public static void DrawIconOnBar(Rect psiRect, IconEntryBar iconEntryBar, int entry, int rowCount)
        {

            Material material = PSIMaterials[iconEntryBar.icon];

            if (material == null)
            {
                return;
            }

            var posOffset = IconPosRectsBar[entry];

            Color GuiColor = GUI.color;
            GuiColor.a = iconEntryBar.color.a;
            GUI.color = GuiColor;

            material.color = iconEntryBar.color;

            Rect iconRect = new Rect(psiRect);

            float size = Mathf.Min(iconRect.width, iconRect.height) / rowCount;

            iconRect.height = iconRect.width = size;


            switch (Settings.ColBarSettings.ColBarPsiIconPos)
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
            iconRect.x += Settings.ColBarSettings.IconOffsetX * posOffset.x * size;
            iconRect.y -= Settings.ColBarSettings.IconOffsetY * posOffset.z * iconRect.height;

            // On Colonist
            // iconRect.x -= iconRect.width * 0.5f;
            // iconRect.y -= iconRect.height * 0.5f;
            GUI.DrawTexture(iconRect, ColonistBarTextures.BGTexIconPSI);
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


        public static void DrawIconOnBar(Rect psiRect, ref int num, Icon icon, Color color, float rectAlpha, int rowCount, string tooltip = null)
        {
            // only two columns visible
            if (num == Settings.ColBarSettings.IconsInColumn * 2)
            {
                return;
            }

            Material material = PSIMaterials[icon];

            if (material == null)
            {
                return;
            }

            DrawIcon_onBar(psiRect, IconPosRectsBar[num], material, color, rectAlpha, rowCount, tooltip);

            num++;
        }

        public static void DrawIconOnColonist(Vector3 bodyPos, IconEntryPSI entryBar, int entryCount, float opacity)
        {
            if (WorldRendererUtility.WorldRenderedNow)
            {
                return;
            }

            Material material = PSIMaterials[entryBar.icon];
            if (material == null)
            {
                Debug.LogError("Material = null.");
                return;
            }

            Vector3 posOffset = IconPosVectorsPSI[entryCount];

            entryBar.color.a = opacity;
            material.color = entryBar.color;
            Color guiColor = GUI.color;
            GUI.color = entryBar.color;
            Vector2 vectorAtBody;

            float worldScale = WorldScale;
            if (Settings.PsiSettings.IconsScreenScale)
            {
                worldScale = 45f;
                vectorAtBody = bodyPos.MapToUIPosition();
                vectorAtBody.x += posOffset.x * 45f;
                vectorAtBody.y -= posOffset.z * 45f;
            }
            else
            {
                vectorAtBody = (bodyPos + posOffset).MapToUIPosition();
            }

            float num2 = worldScale * (Settings.PsiSettings.IconSizeMult * 0.5f);

            // On Colonist
            Rect position = new Rect(
                vectorAtBody.x,
                vectorAtBody.y,
                num2 * Settings.PsiSettings.IconSize,
                num2 * Settings.PsiSettings.IconSize);
            position.x -= position.width * 0.5f;
            position.y -= position.height * 0.5f;

            GUI.DrawTexture(position, material.mainTexture, ScaleMode.ScaleToFit, true);
            GUI.color = guiColor;

        }

        public static void DrawIconOnColonist(Vector3 bodyPos, ref int num, Icon icon, Color color, float opacity)
        {
            if (WorldRendererUtility.WorldRenderedNow)
            {
                return;
            }

            Material material = PSIMaterials[icon];
            if (material == null)
            {
                Debug.LogError("Material = null.");
                return;
            }

            DrawIcon_posOffset(bodyPos, IconPosVectorsPSI[num], material, color, opacity);
            num++;
        }

        private static void DrawIcon_onBar(Rect rect, Vector3 posOffset, Material material, Color color, float rectAlpha, int rowCount, string tooltip = null)
        {
            //    Widgets.DrawBoxSolid(rect, Color.cyan);

            color.a *= rectAlpha;
            Color GuiColor = GUI.color;
            GuiColor.a = rectAlpha;
            GUI.color = GuiColor;

            material.color = color;

            Rect iconRect = new Rect(rect);

            float size = Mathf.Min(iconRect.width, iconRect.height) / rowCount;

            iconRect.height = iconRect.width = size;


            switch (Settings.ColBarSettings.ColBarPsiIconPos)
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
            iconRect.x += Settings.ColBarSettings.IconOffsetX * posOffset.x * size;
            iconRect.y -= Settings.ColBarSettings.IconOffsetY * posOffset.z * iconRect.height;

            // On Colonist
            // iconRect.x -= iconRect.width * 0.5f;
            // iconRect.y -= iconRect.height * 0.5f;
            GUI.DrawTexture(iconRect, ColonistBarTextures.BGTexIconPSI);
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
    }
}