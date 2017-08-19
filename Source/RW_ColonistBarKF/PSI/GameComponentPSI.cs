using static ColonistBarKF.Bar.ColonistBarTextures;
using static ColonistBarKF.PSI.PSIDrawer;

namespace ColonistBarKF.PSI
{
    using System.Collections.Generic;

    using FacialStuff.Detouring;

    using RimWorld;
    using RimWorld.Planet;

    using static Settings;

    using static Statics;

    using UnityEngine;

    using Verse;

    public class GameComponentPSI : GameComponent
    {
        public static Vector3[] IconPosRectsBar;

        public static Vector3[] IconPosVectorsPSI;

        public static string[] IconSets = { "default" };

        public static Materials PSIMaterials = new Materials();

        public static PawnCapacityDef[] _pawnCapacities;

        private CellRect _viewRect;

        public GameComponentPSI()
        {
        }

        public GameComponentPSI(Game game)
        {
        }

        public static float WorldScale { get; private set; } = 1f;

        private static void DrawAnimalIcons(Pawn animal)
        {
            if (!animal.Spawned || animal.Dead)
            {
                return;
            }

            int iconNum = 0;
            Vector3 bodyLoc = animal.DrawPos;

            if (animal.Faction != null && animal.Faction.IsPlayer)
            {
                if (animal.health.HasHediffsNeedingTend())
                {
                    if (animal.health?.hediffSet != null)
                    {
                        float hediffSetBleedRateTotal = animal.health.hediffSet.BleedRateTotal;

                        if (hediffSetBleedRateTotal > 0.01f)
                        {
                            DrawIconOnColonist(
                                bodyLoc,
                                ref iconNum,
                                Icon.Bloodloss,
                                gradientRedAlertToNeutral.Evaluate(1.0f - hediffSetBleedRateTotal),
                                ViewOpacityCrit);
                        }
                    }

                    if (animal.health?.summaryHealth != null)
                    {
                        float summaryHealthSummaryHealthPercent = 1f - animal.health.summaryHealth.SummaryHealthPercent;
                        if (summaryHealthSummaryHealthPercent > 0.01f)
                        {
                            DrawIconOnColonist(
                                bodyLoc,
                                ref iconNum,
                                Icon.Health,
                                gradient4.Evaluate(summaryHealthSummaryHealthPercent),
                                ViewOpacityCrit);
                        }
                    }
                }
            }

            if (!animal.InAggroMentalState)
            {
                return;
            }

            if (!PsiSettings.ShowAggressive)
            {
                return;
            }

            DrawIconOnColonist(bodyLoc, ref iconNum, Icon.Aggressive, ColVermillion, ViewOpacityCrit);
        }

        public static void DrawColonistIconsBar(Pawn pawn, Rect psiRect, float rectAlpha)
        {
            CompPSI pawnStats = pawn.GetComp<CompPSI>();

            if (pawn.Dead || !pawn.Spawned || pawn.holdingOwner == null || pawn.Map == null)
            {
                pawnStats.thisColCount = 0;
                return;
            }

            SettingsColonistBar colBarSettings = ColBarSettings;
            int barIconNum = 0;
            int rowCount = pawnStats.thisColCount;

            // Drafted
            if (colBarSettings.ShowDraft && pawn.Drafted)
            {
                if (pawnStats.isPacifist)
                {
                    DrawIconOnBar(psiRect, ref barIconNum, Icon.Draft, ColYellow, rectAlpha, rowCount);
                }
                else
                {
                    DrawIconOnBar(psiRect, ref barIconNum, Icon.Draft, ColVermillion, rectAlpha, rowCount);
                }
            }

            List<IconEntryBar> drawIconEntries = pawnStats.BarIconList;
            if (!pawnStats.BarIconList.NullOrEmpty())
            {
                int maxIconCount = Mathf.Min(ColBarSettings.IconsInColumn * 2, drawIconEntries.Count) - barIconNum;
                for (int index = 0; index < maxIconCount; index++)
                {
                    IconEntryBar iconEntryBar = drawIconEntries[index];
                    iconEntryBar.color.a *= rectAlpha;
                    DrawIconOnBar(psiRect, iconEntryBar, index + barIconNum, rowCount);
                }

                barIconNum += maxIconCount;
            }

            // Idle - bar icon already included - vanilla
            int colCount = Mathf.CeilToInt((float)barIconNum / ColBarSettings.IconsInColumn);

            pawnStats.thisColCount = colCount;
        }

        public static void DrawColonistIconsPSI(Pawn pawn)
        {
            if (pawn.Dead || !pawn.Spawned || pawn.holdingOwner == null || pawn.Map == null)
            {
                return;
            }

            CompPSI pawnStats = pawn.GetComp<CompPSI>();

            if (pawnStats == null)
            {
                return;
            }

            SettingsPSI psiSettings = PsiSettings;
            float viewOpacity = psiSettings.IconOpacity;

            int iconNum = 0;

            Vector3 bodyLoc = pawn.DrawPos;

            // Target Point
            if (psiSettings.ShowTargetPoint && pawnStats.TargetPos != Vector3.zero)
            {
                if (psiSettings.UseColoredTarget)
                {
                    Color skinColor = pawn.story.SkinColor;
                    Color hairColor = pawn.story.hairColor;

                    if (skinMat == null)
                    {
                        return;
                    }

                    if (hairMat == null)
                    {
                        return;
                    }

                    DrawIcon_posOffset(pawnStats.TargetPos, Vector3.zero, skinMat, skinColor, 1f);
                    DrawIcon_posOffset(pawnStats.TargetPos, Vector3.zero, hairMat, hairColor, 1f);
                }
                else
                {
                    if (targetMat == null)
                    {
                        return;
                    }

                    DrawIcon_posOffset(pawnStats.TargetPos, Vector3.zero, targetMat, ColorNeutralStatus, viewOpacity);
                }
            }

            // Drafted
            if (psiSettings.ShowDraft && pawn.Drafted)
            {
                if (pawnStats.isPacifist)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icon.Pacific, ColYellow, ViewOpacityCrit);
                }
                else
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icon.Draft, ColVermillion, ViewOpacityCrit);
                }
            }

            List<IconEntryPSI> drawIconEntries = pawnStats.PSIIconList;
            if (!drawIconEntries.NullOrEmpty())
            {
                for (int index = 0; index < drawIconEntries.Count; index++)
                {
                    IconEntryPSI iconEntryBar = drawIconEntries[index];
                    DrawIconOnColonist(bodyLoc, iconEntryBar, index + iconNum);
                }
            }
        }

        public static void DrawColonistRelationIconsPSI(Pawn pawn)
        {
            // Log.Message("Begin Drawing");
            if (pawn.Dead || !pawn.Spawned || pawn.holdingOwner == null || pawn.Map == null)
            {
                return;
            }

            CompPSI pawnStats = pawn.GetComp<CompPSI>();

            if (pawnStats == null)
            {
                return;
            }

            // Log.Message("Relations checked");
            if (!pawnStats.hasRelationWithColonist)
            {
                return;
            }

            // Log.Message("Has relation");
            int iconNum = 0;

            // Pawn is no colonist, thus no further stat checks
            Vector3 bodyLoc = pawn.DrawPos;
            {
                float hediffSetBleedRateTotal = pawn.health.hediffSet.BleedRateTotal;

                if (hediffSetBleedRateTotal > 0.01f)
                {
                    DrawIconOnColonist(
                        bodyLoc,
                        ref iconNum,
                        Icon.Bloodloss,
                        gradientRedAlertToNeutral.Evaluate(1.0f - hediffSetBleedRateTotal),
                        ViewOpacityCrit);
                }

                DrawIconOnColonist(
                    bodyLoc,
                    ref iconNum,
                    Icon.Health,
                    gradient4.Evaluate(1f - pawn.health.summaryHealth.SummaryHealthPercent),
                    ViewOpacityCrit);
            }
        }

        public static void Reinit(bool reloadSettings = true, bool reloadIconSet = true, bool recalcIconPos = true)
        {
            _pawnCapacities = new[]
                                  {
                                      PawnCapacityDefOf.BloodFiltration, PawnCapacityDefOf.BloodPumping,
                                      PawnCapacityDefOf.Breathing, PawnCapacityDefOf.Consciousness,
                                      PawnCapacityDefOf.Eating, PawnCapacityDefOf.Hearing,
                                      PawnCapacityDefOf.Manipulation, PawnCapacityDefOf.Metabolism,
                                      PawnCapacityDefOf.Moving, PawnCapacityDefOf.Sight, PawnCapacityDefOf.Talking
                                  };

            if (reloadSettings)
            {
                ColBarSettings = LoadBarSettings();
                PsiSettings = LoadPsiSettings();
                HarmonyPatches.MarkColonistsDirty_Postfix();
            }

            if (recalcIconPos)
            {
                RecalcBarPositionAndSize();
                RecalcIconPositionsPSI();
            }

            if (reloadIconSet)
            {
                LongEventHandler.ExecuteWhenFinished(
                    () =>
                        {
                            PSIMaterials = new Materials(PsiSettings.IconSet);

                            // PSISettings SettingsPSI =
                            // XmlLoader.ItemFromXmlFile<PSISettings>(GenFilePaths.CoreModsFolderPath + "/RW_PawnStateIcons/Textures/UI/Overlays/PawnStateIcons/" + PSI.SettingsPSI.IconSet + "/iconset.cfg");
                            // PSI.PsiSettings.IconSizeMult = SettingsPSI.IconSizeMult;
                            PSIMaterials.ReloadTextures(true);

                            skinMat = PSIMaterials[Icon.TargetSkin];
                            hairMat = PSIMaterials[Icon.TargetHair];
                            targetMat = PSIMaterials[Icon.Target];

                            // Log.Message(GenFilePaths.CoreModsFolderPath + "/RW_PawnStateIcons/Textures/UI/Overlays/PawnStateIcons/" + ColBarSettings.IconSet + "/iconset.cfg");
                        });

                BuildGradients();
            }
        }

        public override void GameComponentOnGUI()
        {
            if (Current.ProgramState != ProgramState.Playing)
            {
                return;
            }

            if (WorldRendererUtility.WorldRenderedNow)
            {
                return;
            }

            if (!PsiSettings.UsePsi && !PsiSettings.UsePsiOnPrisoner && !PsiSettings.ShowRelationsOnStrangers)
            {
                return;
            }

            this._viewRect = Find.CameraDriver.CurrentViewRect;
            this._viewRect = this._viewRect.ExpandedBy(5);
            Map map = Find.VisibleMap;

            foreach (Pawn pawn in map.mapPawns.AllPawnsSpawned)
            {
                if (!this._viewRect.Contains(pawn.Position))
                {
                    continue;
                }

                if (pawn.Map == null)
                {
                    continue;
                }

                // if (useGUILayout)
                if (pawn.RaceProps.Animal)
                {
                    if (PsiSettings.UsePsiOnAnimals)
                    {
                        DrawAnimalIcons(pawn);
                    }
                }
                else if (pawn.RaceProps.Humanlike)
                {
                    if (pawn.IsColonist)
                    {
                        if (PsiSettings.UsePsi)
                        {
                            DrawColonistIconsPSI(pawn);
                        }
                    }
                    else
                    {
                        if (pawn.IsPrisoner && PsiSettings.UsePsiOnPrisoner)
                        {
                            DrawColonistIconsPSI(pawn);
                        }
                        else if (PsiSettings.ShowRelationsOnStrangers)
                        {
                            DrawColonistRelationIconsPSI(pawn);
                        }
                    }
                }
            }
        }

        // public override void GameComponentTick()
        // {
        // // Scans the map for new pawns
        // if (Current.ProgramState != ProgramState.Playing)
        // {
        // return;
        // }
        // if (!Settings.ColBarSettings.UsePsi && !Settings.PsiSettings.UsePsi)
        // {
        // return;
        // }
        // this._fDelta += Time.fixedDeltaTime;
        // if (this._fDelta < 5)
        // {
        // return;
        // }
        // this._fDelta = 0.0;
        // }
        public override void GameComponentUpdate()
        {
            if (Input.GetKeyUp(KeyCode.F11))
            {
                PsiSettings.UsePsi = !PsiSettings.UsePsi;
                ColBarSettings.UsePsi = !ColBarSettings.UsePsi;
                Messages.Message(
                    PsiSettings.UsePsi ? "PSI.Enabled".Translate() : "PSI.Disabled".Translate(),
                    MessageSound.Standard);
            }

            WorldScale = UI.screenHeight / (2f * Camera.current.orthographicSize);
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            Reinit();
        }

        // private static bool HasThought(List<Thought> thoughts, ThoughtDef tdef)
        // {
        // return thoughts.Any(thought => thought.def == tdef);
        // }
        private static void RecalcBarPositionAndSize()
        {
            SettingsColonistBar settings = ColBarSettings;
            IconPosRectsBar = new Vector3[40];
            for (int index = 0; index < IconPosRectsBar.Length; ++index)
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

                IconPosRectsBar[index] = new Vector3(-num1, 3f, num2);
            }
        }

        private static void RecalcIconPositionsPSI()
        {
            SettingsPSI psiSettings = PsiSettings;

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
    }
}