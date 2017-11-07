using static ColonistBarKF.PSI.GameComponentPSI;

namespace ColonistBarKF.PSI
{
    using ColonistBarKF.Bar;

    using JetBrains.Annotations;

    using RimWorld.Planet;

    using UnityEngine;

    using Verse;

    public class PSIDrawer
    {
        [NotNull]
        public static Vector3[] IconPosVectorsPSI;

        public static void RecalcIconPositionsPSI()
        {
            SettingsPSI psiSettings = Settings.psiSettings;

            // _iconPosVectors = new Vector3[18];
            IconPosVectorsPSI = new Vector3[40];
            for (int index = 0; index < IconPosVectorsPSI.Length; ++index)
            {
                int num1 = index / psiSettings.IconsInColumn;
                int num2 = index % psiSettings.IconsInColumn;
                if (psiSettings.IconsHorizontal)
                {
                    int num3 = num1;
                    num1 = num2;
                    num2 = num3;
                }

                float y = Altitudes.AltitudeFor(AltitudeLayer.MetaOverlays);

                IconPosVectorsPSI[index] = new Vector3(
                    (float)(-0.600000023841858 * psiSettings.IconMarginX - 0.550000011920929 * psiSettings.IconSize
                            * psiSettings.IconOffsetX * num1),
                    y,
                    (float)(-0.600000023841858 * psiSettings.IconMarginY + 0.550000011920929 * psiSettings.IconSize
                            * psiSettings.IconOffsetY * num2));
            }
        }

        public static void DrawIcon_posOffset(
            Vector3 bodyPos,
            Vector3 posOffset,
            [NotNull] Material material,
            Color color,
            float opacity)
        {
            color.a = opacity;
            material.color = color;
            Color guiColor = GUI.color;
            GUI.color = color;
            Vector2 vectorAtBody;

            float worldScale = WorldScale;
            if (Settings.psiSettings.IconsScreenScale)
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

            float num2 = worldScale * (Settings.psiSettings.IconSizeMult * 0.5f);

            // On Colonist
            Rect position = new Rect(
                vectorAtBody.x,
                vectorAtBody.y,
                num2 * Settings.psiSettings.IconSize,
                num2 * Settings.psiSettings.IconSize);
            position.x -= position.width * 0.5f;
            position.y -= position.height * 0.5f;

            GUI.DrawTexture(position, material.mainTexture, ScaleMode.ScaleToFit, true);
            GUI.color = guiColor;
        }




        public static void DrawIconOnColonist(Vector3 bodyPos, IconEntryPSI entryPSI, int entryCount)
        {
            if (WorldRendererUtility.WorldRenderedNow)
            {
                return;
            }

            Material material = GameComponentPSI.PSIMaterials[entryPSI.icon];
            if (material == null)
            {
                Debug.LogError("Material = null.");
                return;
            }

            Vector3 posOffset = IconPosVectorsPSI[entryCount];

            entryPSI.color.a = entryPSI.opacity;
            material.color = entryPSI.color;
            Color guiColor = GUI.color;
            GUI.color = entryPSI.color;
            Vector2 vectorAtBody;

            float worldScale = WorldScale;
            if (Settings.psiSettings.IconsScreenScale)
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

            float num2 = worldScale * (Settings.psiSettings.IconSizeMult * 0.5f);

            // On Colonist
            Rect position = new Rect(
                vectorAtBody.x,
                vectorAtBody.y,
                num2 * Settings.psiSettings.IconSize,
                num2 * Settings.psiSettings.IconSize);
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

            Material material = GameComponentPSI.PSIMaterials[icon];
            if (material == null)
            {
                // Debug.LogError("Material = null.");
                return;
            }

            DrawIcon_posOffset(bodyPos, IconPosVectorsPSI[num], material, color, opacity);
            num++;
        }
    }
}