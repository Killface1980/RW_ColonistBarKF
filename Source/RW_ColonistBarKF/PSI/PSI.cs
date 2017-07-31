using static ColonistBarKF.Bar.ColonistBarTextures;
using static ColonistBarKF.PSI.PSIDrawer;

namespace ColonistBarKF.PSI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FacialStuff.Detouring;

    using Harmony;

    using RimWorld;
    using RimWorld.Planet;

    using UnityEngine;

    using Verse;
    using Verse.AI;

    public class PSI : GameComponent
    {
        public static Vector3[] IconPosRectsBar;

        public static Vector3[] IconPosVectorsPSI;

        public static string[] IconSets = { "default" };

        public static Materials PSIMaterials = new Materials();



        private CellRect _viewRect;

        public static PawnCapacityDef[] _pawnCapacities;

        public static float ViewOpacityCrit => Mathf.Max(
            Settings.PsiSettings.IconOpacityCritical,
            Settings.PsiSettings.IconOpacity);

        public PSI()
        {
        }

        public PSI(Game game)
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
            if (!animal.InAggroMentalState)
            {
                return;
            }

            if (!Settings.PsiSettings.ShowAggressive)
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

            SettingsColonistBar colBarSettings = Settings.ColBarSettings;
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


            List<IconEntryBar> drawIconEntries = pawnStats.BarIcons;
            if (!pawnStats.BarIcons.NullOrEmpty())
            {
                int maxRowCount = Mathf.Min(Settings.ColBarSettings.IconsInColumn * 2, drawIconEntries.Count);
                for (int index = 0; index < maxRowCount; index++)
                {
                    IconEntryBar iconEntryBar = drawIconEntries[index];
                    iconEntryBar.color.a *= rectAlpha;
                    DrawIconOnBar(psiRect, iconEntryBar, index + barIconNum, rowCount);
                }

                barIconNum += maxRowCount;
            }

            // Idle - bar icon already included - vanilla
            int colCount = Mathf.CeilToInt((float)barIconNum / Settings.ColBarSettings.IconsInColumn);

            pawnStats.thisColCount = colCount;

        }




        public static void DrawColonistIconsPSI(Pawn pawn)
        {
            // TODO: Make list item like ...OnBar
            if (pawn.Dead || !pawn.Spawned || pawn.holdingOwner == null || pawn.Map == null)
            {
                return;
            }

            CompPSI pawnStats = pawn.GetComp<CompPSI>();

            if (pawnStats == null)
            {
                return;
            }

            SettingsPSI psiSettings = Settings.PsiSettings;
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

            List<IconEntryPSI> drawIconEntries = pawnStats.PSIIcons;
            if (!drawIconEntries.NullOrEmpty())
            {
                for (int index = 0; index < drawIconEntries.Count; index++)
                {
                    IconEntryPSI iconEntryBar = drawIconEntries[index];
                    DrawIconOnColonist(bodyLoc, iconEntryBar, index + iconNum, ViewOpacityCrit);
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

            if (!pawnStats.relationChecked)
            {
                // wait till the pawn is properly spawned. Else FS won't find a relation.
                if (pawnStats.SpawnedAt + 120 > Find.TickManager.TicksGame)
                {
                    pawnStats.CheckRelationWithColonists(pawn);
                }

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
                Settings.ColBarSettings = Settings.LoadBarSettings();
                Settings.PsiSettings = Settings.LoadPsiSettings();
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
                            PSIMaterials = new Materials(Settings.PsiSettings.IconSet);

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

        private static void BuildGradients()
        {
            // Build gradients
            GradientColorKey[] gck = new GradientColorKey[4];
            gck[0].color = ColorNeutralStatus;
            gck[0].time = 0.0f;
            gck[1].color = ColYellow;
            gck[1].time = 0.33f;
            gck[2].color = ColOrange;
            gck[2].time = 0.66f;
            gck[3].color = ColVermillion;
            gck[3].time = 1f;
            GradientAlphaKey[] gak = new GradientAlphaKey[3];
            gak[0].alpha = 0.8f;
            gak[0].time = 0.0f;
            gak[1].alpha = 1f;
            gak[1].time = 0.1625f;
            gak[2].alpha = 1.0f;
            gak[2].time = 1.0f;
            gradient4.SetKeys(gck, gak);

            gck = new GradientColorKey[5];
            gck[0].color = ColVermillion;
            gck[0].time = 0f;
            gck[1].color = ColOrange;
            gck[1].time = 0.375f;
            gck[2].color = ColYellow;
            gck[2].time = 0.5f;
            gck[3].color = ColorNeutralStatus;
            gck[3].time = 0.625f;
            gck[4].color = ColBlueishGreen;
            gck[4].time = 1f;
            gak = new GradientAlphaKey[4];
            gak[0].alpha = 1.0f;
            gak[0].time = 0.0f;
            gak[1].alpha = 1.0f;
            gak[1].time = 0.5f;
            gak[2].alpha = 0.8f;
            gak[2].time = 0.625f;
            gak[3].alpha = 1.0f;
            gak[3].time = 0.75f;
            gradient4Mood.SetKeys(gck, gak);

            gck = new GradientColorKey[2];
            gck[0].color = ColVermillion;
            gck[0].time = 0.0f;
            gck[1].color = ColorNeutralStatus;
            gck[1].time = 1f;
            gak = new GradientAlphaKey[3];
            gak[0].alpha = 1.0f;
            gak[0].time = 0.0f;
            gak[1].alpha = 1.0f;
            gak[1].time = 0.75f;
            gak[2].alpha = 0.8f;
            gak[2].time = 1.0f;
            gradientRedAlertToNeutral.SetKeys(gck, gak);
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

            if (!Settings.PsiSettings.UsePsi && !Settings.PsiSettings.UsePsiOnPrisoner
                && !Settings.PsiSettings.ShowRelationsOnStrangers)
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

                if (pawn.Map != map)
                {
                    continue;
                }


                // if (useGUILayout)
                if (pawn.RaceProps.Animal)
                {
                    if (Settings.PsiSettings.UsePsiOnAnimals)
                    {
                        DrawAnimalIcons(pawn);
                    }
                }
                else if (pawn.RaceProps.Humanlike)
                {
                    if (pawn.IsColonist)
                    {
                        if (Settings.PsiSettings.UsePsi)
                        {
                            DrawColonistIconsPSI(pawn);
                        }
                    }
                    else
                    {
                        if (pawn.IsPrisoner && Settings.PsiSettings.UsePsiOnPrisoner)
                        {
                            DrawColonistIconsPSI(pawn);
                        }
                        else if (Settings.PsiSettings.ShowRelationsOnStrangers)
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
                Settings.PsiSettings.UsePsi = !Settings.PsiSettings.UsePsi;
                Settings.ColBarSettings.UsePsi = !Settings.ColBarSettings.UsePsi;
                Messages.Message(
                    Settings.PsiSettings.UsePsi ? "PSI.Enabled".Translate() : "PSI.Disabled".Translate(),
                    MessageSound.Standard);
            }

            WorldScale = UI.screenHeight / (2f * Camera.current.orthographicSize);
        }

        public override void LoadedGame()
        {
            base.LoadedGame();
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();
        }

        public override void FinalizeInit()
        {
            InjectCompPSI();
            Reinit();
        }

        private static void InjectCompPSI()
        {
            Log.Message("Start injecting PSI to pawns ...");
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs.Where(x => x.race != null && x.race.Humanlike && x.race.IsFlesh))
            {
                Log.Message("PSI check: " + def);
                if (def?.comps != null)
                {
                    def.comps.Add(new CompProperties(typeof(CompPSI)));
                    Log.Message("PSI injected " + def);
                }
            }

            // foreach (Pawn pawn in PawnsFinder.AllMaps)
            // {
            // if (pawn.RaceProps.Animal)
            // {
            // continue;
            // }
            // bool flag = false;
            // foreach (ThingComp comp in pawn.AllComps)
            // {
            // CompPSI psi = comp as CompPSI;
            // if (psi != null)
            // {
            // flag = true;
            // }
            // }
            // if (flag)
            // {
            // pawn.InitializeComps();
            // }
            // }
        }

        // private static bool HasThought(List<Thought> thoughts, ThoughtDef tdef)
        // {
        // return thoughts.Any(thought => thought.def == tdef);
        // }
        private static void RecalcBarPositionAndSize()
        {
            SettingsColonistBar settings = Settings.ColBarSettings;
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
            SettingsPSI psiSettings = Settings.PsiSettings;

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