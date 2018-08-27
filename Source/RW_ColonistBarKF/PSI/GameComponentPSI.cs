using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using static ColonistBarKF.Bar.Textures;

namespace ColonistBarKF.PSI
{
    public class GameComponentPSI : GameComponent
    {


        [NotNull]
        public static PawnCapacityDef[] PawnCapacities;

        [NotNull]
        public static Materials PSIMaterials = new Materials();

        private CellRect _viewRect;
        private bool hidePSIOnGUI;

        public GameComponentPSI()
        {

        }

        public GameComponentPSI(Game game)
        {

        }

        public static float WorldScale { get; private set; } = 1f;

        public static void Reinit(bool reloadSettings = true, bool reloadIconSet = true, bool recalcIconPos = true)
        {
            PawnCapacities = new[]
                                 {
                                     PawnCapacityDefOf.BloodFiltration, PawnCapacityDefOf.BloodPumping,
                                     PawnCapacityDefOf.Breathing, PawnCapacityDefOf.Consciousness,
                                     PawnCapacityDefOf.Eating, PawnCapacityDefOf.Hearing,
                                     PawnCapacityDefOf.Manipulation, PawnCapacityDefOf.Metabolism,
                                     PawnCapacityDefOf.Moving, PawnCapacityDefOf.Sight, PawnCapacityDefOf.Talking
                                 };

            if (reloadSettings)
            {
                Settings.BarSettings = Settings.LoadBarSettings();
                Settings.PSISettings = Settings.LoadPSISettings();
                HarmonyPatches.MarkColonistsDirty_Postfix();
            }

            if (recalcIconPos)
            {
                BarIconDrawer.RecalcBarPositionAndSize();
                PSIDrawer.RecalcIconPositionsPSI();
            }

            if (reloadIconSet)
            {
                LongEventHandler.ExecuteWhenFinished(
                    () =>
                        {
                            PSIMaterials = new Materials(Settings.PSISettings.IconSet);

                            // PSISettings SettingsPSI =
                            // XmlLoader.ItemFromXmlFile<PSISettings>(GenFilePaths.CoreModsFolderPath + "/RW_PawnStateIcons/Textures/UI/Overlays/PawnStateIcons/" + PSI.SettingsPSI.IconSet + "/iconset.cfg");
                            // PSI.PSISettings.IconSizeMult = SettingsPSI.IconSizeMult;
                            PSIMaterials.ReloadTextures(true);

                            SkinMat = PSIMaterials[Icon.TargetSkin];
                            HairMat = PSIMaterials[Icon.TargetHair];
                            TargetMat = PSIMaterials[Icon.Target];

                            // Log.Message(GenFilePaths.CoreModsFolderPath + "/RW_PawnStateIcons/Textures/UI/Overlays/PawnStateIcons/" + ColBarSettings.IconSet + "/iconset.cfg");
                        });
            }
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            Reinit();
        }

        public override void GameComponentOnGUI()
        {
            if (Current.ProgramState != ProgramState.Playing)
            {
                return;
            }

            if (this.hidePSIOnGUI)
            {
                return;

            }

            if (WorldRendererUtility.WorldRenderedNow)
            {
                return;
            }

            if (!Settings.PSISettings.UsePSI && !Settings.PSISettings.UsePSIOnPrisoner
                && !Settings.PSISettings.ShowRelationsOnStrangers)
            {
                return;
            }

            this._viewRect = Find.CameraDriver.CurrentViewRect;
            this._viewRect = this._viewRect.ExpandedBy(5);
            Map map = Find.CurrentMap;

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
                    if (Settings.PSISettings.UsePSIOnAnimals)
                    {
                        DrawAnimalIcons(pawn);
                    }
                }
                else if (pawn.RaceProps.Humanlike)
                {
                    if (pawn.IsColonist)
                    {
                        if (Settings.PSISettings.UsePSI)
                        {
                            DrawColonistIconsPSI(pawn);
                        }
                    }
                    else
                    {
                        if (pawn.IsPrisoner && Settings.PSISettings.UsePSIOnPrisoner)
                        {
                            DrawColonistIconsPSI(pawn);
                        }
                        else if (Settings.PSISettings.ShowRelationsOnStrangers)
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
        // if (!Settings.ColBarSettings.UsePSI && !Settings.PSISettings.UsePSI)
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
                if (Input.GetKeyUp(KeyBindingDefOf.ToggleScreenshotMode.MainKey))
                {
                    this.hidePSIOnGUI  =!this.hidePSIOnGUI;
            }

            WorldScale = UI.screenHeight / (2f * Camera.current.orthographicSize);
        }

        private static void DrawAnimalIcons([NotNull] Pawn animal)
        {
            if (!animal.Spawned || animal.Dead)
            {
                return;
            }

            int iconNum = 0;
            Vector3 bodyLoc = animal.DrawPos;

            if (animal.Faction?.IsPlayer == true)
            {
                if (animal.health.HasHediffsNeedingTend())
                {
                    if (animal.health?.hediffSet != null)
                    {
                        float hediffSetBleedRateTotal = animal.health.hediffSet.BleedRateTotal;

                        if (hediffSetBleedRateTotal > 0.01f)
                        {
                            PSIDrawer.DrawIconOnColonist(
                                bodyLoc,
                                ref iconNum,
                                Icon.Bloodloss,
                                Statics.GradientRedAlertToNeutral.Evaluate(1.0f - hediffSetBleedRateTotal),
                                Settings.ViewOpacityCrit);
                        }
                    }

                    if (animal.health?.summaryHealth != null)
                    {
                        float summaryHealthSummaryHealthPercent = 1f - animal.health.summaryHealth.SummaryHealthPercent;
                        if (summaryHealthSummaryHealthPercent > 0.01f)
                        {
                            PSIDrawer.DrawIconOnColonist(
                                bodyLoc,
                                ref iconNum,
                                Icon.Health,
                                Statics.Gradient4.Evaluate(summaryHealthSummaryHealthPercent),
                                Settings.ViewOpacityCrit);
                        }
                    }
                }
            }

            if (!animal.InAggroMentalState)
            {
                return;
            }

            if (!Settings.PSISettings.ShowAggressive)
            {
                return;
            }

            PSIDrawer.DrawIconOnColonist(bodyLoc, ref iconNum, Icon.Aggressive, ColVermillion, Settings.ViewOpacityCrit);
        }

        private static void DrawColonistIconsPSI([NotNull] Pawn pawn)
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

            SettingsPSI psiSettings = Settings.PSISettings;
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

                    if (SkinMat == null)
                    {
                        return;
                    }

                    if (HairMat == null)
                    {
                        return;
                    }

                    PSIDrawer.DrawIcon_posOffset(pawnStats.TargetPos, Vector3.zero, SkinMat, skinColor, 1f);
                    PSIDrawer.DrawIcon_posOffset(pawnStats.TargetPos, Vector3.zero, HairMat, hairColor, 1f);
                }
                else
                {
                    if (TargetMat == null)
                    {
                        return;
                    }

                    PSIDrawer.DrawIcon_posOffset(pawnStats.TargetPos, Vector3.zero, TargetMat, ColorNeutralStatus, viewOpacity);
                }
            }

            // Drafted
            if (psiSettings.ShowDraft && pawn.Drafted)
            {
                if (pawnStats.IsPacifist)
                {
                    PSIDrawer.DrawIconOnColonist(bodyLoc, ref iconNum, Icon.Pacific, ColYellow, Settings.ViewOpacityCrit);
                }
                else
                {
                    PSIDrawer.DrawIconOnColonist(bodyLoc, ref iconNum, Icon.Draft, ColVermillion, Settings.ViewOpacityCrit);
                }
            }

            List<IconEntryPSI> drawIconEntries = pawnStats.PSIIconList;
            if (!drawIconEntries.NullOrEmpty())
            {
                for (int index = 0; index < drawIconEntries.Count; index++)
                {
                    IconEntryPSI iconEntryBar = drawIconEntries[index];
                    PSIDrawer.DrawIconOnColonist(bodyLoc, iconEntryBar, index + iconNum);
                }
            }
        }

        private static void DrawColonistRelationIconsPSI([NotNull] Pawn pawn)
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
            if (!pawnStats.HasRelationWithColonist)
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
                    PSIDrawer.DrawIconOnColonist(
                        bodyLoc,
                        ref iconNum,
                        Icon.Bloodloss,
                        Statics.GradientRedAlertToNeutral.Evaluate(1.0f - hediffSetBleedRateTotal),
                        Settings.ViewOpacityCrit);
                }

                Color color = Statics.Gradient4.Evaluate(1f - pawn.health.summaryHealth.SummaryHealthPercent);
                PSIDrawer.DrawIconOnColonist(
                    bodyLoc,
                    ref iconNum,
                    Icon.Health,
                    color,
                    Settings.ViewOpacityCrit);
            }
        }

        // private static bool HasThought(List<Thought> thoughts, ThoughtDef tdef)
        // {
        // return thoughts.Any(thought => thought.def == tdef);
        // }
    }
}