using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColonistBarKF
{
    using ColonistBarKF.Bar;
    using ColonistBarKF.PSI;

    using UnityEngine;

    using Verse;

    public class CompPSI : ThingComp
    {
        private Pawn psiPawn;

        public Pawn PSIPawn => this.psiPawn ?? (this.psiPawn = this.parent as Pawn);

        public override void PostDraw()
        {
            base.PostDraw();

            PawnStats pawnstats = this.PSIPawn.GetPawnStats();
            Vector3 bodyPos = this.PSIPawn.DrawPos;

            for (int index = 0; index < pawnstats.BarIcons.Count; index++)
            {
                var entry = pawnstats.BarIcons[index];
                Vector3 posOffset = PSI.PSI.IconPosVectorsPSI[index];
                Material material = PSI.PSI.PSIMaterials[entry.icon];

                float opacity = 1f;

                entry.color.a = opacity;
                material.color = entry.color;
                Vector2 vectorAtBody;

                float worldScale = PSI.PSI.WorldScale;
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
                Rect iconRect = new Rect(
                    vectorAtBody.x,
                    vectorAtBody.y,
                    num2 * Settings.PsiSettings.IconSize,
                    num2 * Settings.PsiSettings.IconSize);
                iconRect.x -= iconRect.width * 0.5f;
                iconRect.y -= iconRect.height * 0.5f;

                Graphics.DrawTexture(iconRect, ColonistBarTextures.BGTexIconPSI);
                Graphics.DrawTexture(iconRect, material.mainTexture);
            }
        }

    }
}
