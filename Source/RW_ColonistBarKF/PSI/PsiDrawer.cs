using static ColonistBarKF.PSI.PSI;

namespace ColonistBarKF.PSI
{
    using ColonistBarKF.Bar;

    using RimWorld.Planet;

    using UnityEngine;

    using Verse;

    public static class PSIDrawer
    {
        public static Color IconColor(
            float v,
            Color c1,
            Color c2,
            Color c3,
            Color c4,
            Color c5)
        {
            if (v < 0.2f)
            {
                return Color.Lerp(c1, c2, v * 5);
            }

            if (v < 0.4f)
            {
                return Color.Lerp(c2, c3, (v - 0.2f) * 5);
            }

            if (v < 0.6f)
            {
                return Color.Lerp(c3, c4, (v - 0.4f) * 5);
            }

            if (v < 0.8f)
            {
                return Color.Lerp(c4, c5, (v - 0.6f) * 5);
            }

            return c5;

        }


        public static void DrawIcon_FadeFloatWithFourColorsHB(
            Vector3 bodyPos,
            ref int num,
            Icons icon,
            float v,
            Color c1,
            Color c2,
            Color c3,
            Color c4,
            float opacity)
        {
            if (v > 0.8f)
            {
                DrawIconOnColonist(bodyPos, ref num, icon, Color.Lerp(c2, c1, (v - 0.8f) * 5), opacity);
            }
            else if (v > 0.6f)
            {
                DrawIconOnColonist(bodyPos, ref num, icon, Color.Lerp(c3, c2, (v - 0.6f) * 5), opacity);
            }
            else if (v > 0.4f)
            {
                DrawIconOnColonist(bodyPos, ref num, icon, Color.Lerp(c4, c3, (v - 0.4f) * 5), opacity);
            }
            else
            {
                DrawIconOnColonist(bodyPos, ref num, icon, c4, opacity);
            }
        }

        public static void DrawIcon_FadeFloatWithFourColorsHB(
            Rect rect,
            ref int num,
            Icons icon,
            float v,
            Color c1,
            Color c2,
            Color c3,
            Color c4,
            float rectAlpha,
            string tooltip = null)
        {
            if (v > 0.8f)
            {
                DrawIconOnBar(rect, ref num, icon, Color.Lerp(c2, c1, (v - 0.8f) * 5), rectAlpha, tooltip);
            }
            else if (v > 0.6f)
            {
                DrawIconOnBar(rect, ref num, icon, Color.Lerp(c3, c2, (v - 0.6f) * 5), rectAlpha, tooltip);
            }
            else if (v > 0.4f)
            {
                DrawIconOnBar(rect, ref num, icon, Color.Lerp(c4, c3, (v - 0.4f) * 5), rectAlpha, tooltip);
            }
            else
            {
                DrawIconOnBar(rect, ref num, icon, c4, rectAlpha, tooltip);
            }
        }

        public static void DrawIcon_FadeFloatWithThreeColors(
            Vector3 bodyPos,
            ref int num,
            Icons icon,
            float v,
            Color c1,
            Color c2,
            Color c3,
            float opacity)
        {
            DrawIconOnColonist(
                bodyPos,
                ref num,
                icon,
                v < 0.5 ? Color.Lerp(c1, c2, v * 2f) : Color.Lerp(c2, c3, (float)((v - 0.5) * 2.0)),
                opacity);
        }

        public static void DrawIcon_FadeFloatWithThreeColors(
            Rect rect,
            ref int num,
            Icons icon,
            float v,
            Color c1,
            Color c2,
            Color c3,
            float rectAlpha)
        {
            DrawIconOnBar(
                rect,
                ref num,
                icon,
                v < 0.5 ? Color.Lerp(c1, c2, v * 2f) : Color.Lerp(c2, c3, (float)((v - 0.5) * 2.0)),
                rectAlpha);
        }

        public static void DrawIcon_FadeFloatWithTwoColors(
            Vector3 bodyPos,
            ref int num,
            Icons icon,
            float v,
            Color c1,
            Color c2,
            float opacity)
        {
            DrawIconOnColonist(bodyPos, ref num, icon, Color.Lerp(c1, c2, v), opacity);
        }

        public static void DrawIcon_FadeFloatWithTwoColors(
            Rect rect,
            ref int num,
            Icons icon,
            float v,
            Color c1,
            Color c2,
            float rectAlpha,
            string tooltip = null)
        {
            DrawIconOnBar(rect, ref num, icon, Color.Lerp(c1, c2, v), rectAlpha, tooltip);
        }

        public static void DrawIcon_FadeRedAlertToNeutral(
            Vector3 bodyPos,
            ref int num,
            Icons icon,
            float v,
            float opacity)
        {
            v = v * 0.9f; // max settings according to neutral icon
            DrawIconOnColonist(bodyPos, ref num, icon, new Color(0.9f, v, v), opacity);
        }

        public static void DrawIcon_FadeRedAlertToNeutral(
            Rect rect,
            ref int num,
            Icons icon,
            float v,
            float rectAlpha,
            string tooltip = null)
        {
            v = v * 0.9f; // max settings according to neutral icon
            DrawIconOnBar(rect, ref num, icon, new Color(0.9f, v, v, 1f), rectAlpha, tooltip);
        }

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

            float wordscale = WorldScale;
            if (Settings.PsiSettings.IconsScreenScale)
            {
                wordscale = 45f;
                vectorAtBody = bodyPos.MapToUIPosition();
                vectorAtBody.x += posOffset.x * 45f;
                vectorAtBody.y -= posOffset.z * 45f;
            }
            else
            {
                vectorAtBody = (bodyPos + posOffset).MapToUIPosition();
            }

            float num2 = wordscale * (Settings.PsiSettings.IconSizeMult * 0.5f);

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

        public static void DrawIconOnBar(
            Rect psiRect,
            ref int num,
            Icons icon,
            Color color,
            float rectAlpha,
            string tooltip = null)
        {
            // only two columns visible
            if (num  == Settings.ColBarSettings.IconsInColumn * 2)
            {
                return;
            }

            Material material = PSIMaterials[icon];

            if (material == null)
            {
                return;
            }

            DrawIcon_onBar(psiRect, IconPosRectsBar[num], material, color, rectAlpha, tooltip);

            num++;
        }

        public static void DrawIconOnColonist(Vector3 bodyPos, ref int num, Icons icon, Color color, float opacity)
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

            DrawIcon_posOffset(bodyPos, IconPosVectorsPsi[num], material, color, opacity);
            num++;
        }

        private static void DrawIcon_onBar(
            Rect rect,
            Vector3 posOffset,
            Material material,
            Color color,
            float rectAlpha,
            string tooltip = null)
        {
            color.a *= rectAlpha;
            Color GuiColor = GUI.color;
            GuiColor.a = rectAlpha;
            GUI.color = GuiColor;

            material.color = color;

            Rect iconRect = new Rect(rect);

            iconRect.width /= Settings.ColBarSettings.IconsInColumn;
            iconRect.height = iconRect.width;
            iconRect.x = rect.xMin;
            iconRect.y = rect.yMax;

            switch (Settings.ColBarSettings.ColBarPsiIconPos)
            {
                case Position.Alignment.Left:
                    iconRect.x = rect.xMax - iconRect.width;
                    iconRect.y = rect.yMax - iconRect.width;

                    // if (ColBarSettings.UseExternalMoodBar && ColBarSettings.MoodBarPos == Alignment.Left)
                    // iconRect.x -= rect.width / 4;
                    break;
                case Position.Alignment.Right:
                    iconRect.x = rect.xMin;
                    iconRect.y = rect.yMax - iconRect.width;

                    // if (ColBarSettings.UseExternalMoodBar && ColBarSettings.MoodBarPos == Alignment.Right)
                    // iconRect.x += rect.width / 4;
                    break;
                case Position.Alignment.Top:
                    iconRect.y = rect.yMax - iconRect.height;

                    // if (ColBarSettings.UseExternalMoodBar && ColBarSettings.MoodBarPos == Alignment.Top)
                    // iconRect.y -= rect.height / 4;
                    break;
                case Position.Alignment.Bottom:
                    iconRect.y = rect.yMin;

                    // if (ColBarSettings.UseExternalMoodBar && ColBarSettings.MoodBarPos == Alignment.Bottom)
                    // iconRect.y += rect.height / 4;
                    break;
            }

            // iconRect.x += (-0.5f * CBKF.ColBarSettings.IconMarginX - 0.5f  * CBKF.ColBarSettings.IconOffsetX) * iconRect.width;
            // iconRect.y -= (-0.5f * CBKF.ColBarSettings.IconDistanceY + 0.5f  * CBKF.ColBarSettings.IconOffsetY) * iconRect.height;
            iconRect.x += Settings.ColBarSettings.IconOffsetX * posOffset.x * iconRect.width;
            iconRect.y -= Settings.ColBarSettings.IconOffsetY * posOffset.z * iconRect.height;

            // On Colonist
            // iconRect.x -= iconRect.width * 0.5f;
            // iconRect.y -= iconRect.height * 0.5f;
            GUI.DrawTexture(iconRect, ColonistBarTextures.BGTexIconPSI);
            GUI.color = color;

            iconRect.x += iconRect.width * 0.1f;
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