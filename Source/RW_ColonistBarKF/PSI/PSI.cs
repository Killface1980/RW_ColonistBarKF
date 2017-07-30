using static ColonistBarKF.Bar.ColonistBarTextures;
using static ColonistBarKF.PSI.PSIDrawer;

namespace ColonistBarKF.PSI
{
    using FacialStuff.Detouring;
    using RimWorld;
    using RimWorld.Planet;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Harmony;

    using UnityEngine;
    using Verse;
    using Verse.AI;

    public class PSI : GameComponent
    {
        public static Vector3[] IconPosRectsBar;

        public static Vector3[] IconPosVectorsPSI;

        public static string[] IconSets = { "default" };

        public static Materials PSIMaterials = new Materials();

        private static PawnCapacityDef[] _pawnCapacities;

        private static PawnCapacityDef[] array;

        private CellRect _viewRect;

        public PSI()
        {
        }

        public PSI(Game game)
        {
        }

        public static float WorldScale { get; private set; } = 1f;

        private static float ViewOpacityCrit => Mathf.Max(
            Settings.PsiSettings.IconOpacityCritical,
            Settings.PsiSettings.IconOpacity);

        public static void DrawAnimalIcons(Pawn animal)
        {
            if (!animal.Spawned)
            {
                return;
            }

            if (!animal.InAggroMentalState)
            {
                return;
            }

            if (!Settings.PsiSettings.ShowAggressive)
            {
                return;
            }

            Vector3 drawPos = animal.DrawPos;
            Vector3 bodyPos = drawPos;
            int num = 0;
            DrawIconOnColonist(bodyPos, ref num, Icon.Aggressive, ColVermillion, ViewOpacityCrit);
        }

        public static void DrawColonistIconsBar(Pawn pawn, Rect psiRect, float rectAlpha)
        {
            PawnStats pawnStats = pawn.GetPawnStats();

            if (pawn.Dead || !pawn.Spawned || pawn.holdingOwner == null || pawn.Map == null)
            {
                pawnStats.thisColCount = 0;
                return;
            }

            CheckStats(pawnStats);
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
                for (int index = 0; index < Mathf.Min(Settings.ColBarSettings.IconsInColumn * 2, drawIconEntries.Count); index++)
                {
                    IconEntryBar iconEntryBar = drawIconEntries[index];
                    iconEntryBar.color.a *= rectAlpha;
                    DrawIconOnBar(psiRect, iconEntryBar, index + barIconNum, rowCount);
                }
                barIconNum += pawnStats.BarIcons.Count;
            }

            // Idle - bar icon already included - vanilla

            int colCount = Mathf.CeilToInt((float)barIconNum / Settings.ColBarSettings.IconsInColumn);

            pawnStats.thisColCount = colCount;

        }

        private static Color Evaluate(float moodOffset)
        {
            return gradient4Mood.Evaluate(Mathf.InverseLerp(-25f, 15f, moodOffset));
        }

        public static void CheckStats(Pawn pawn)
        {
            PawnStats pawnStats = pawn.GetPawnStats();
            if (pawnStats != null)
            {
                CheckStats(pawnStats);
            }
        }

        public static void CheckStats(PawnStats pawnStats)
        {
            if (Find.TickManager.CurTimeSpeed == TimeSpeed.Paused)
            {
                return;
            }

            float nextUpdate = pawnStats.LastStatUpdate
                               + pawnStats.NextStatUpdate * Find.TickManager.TickRateMultiplier;

            if (Find.TickManager.TicksGame > nextUpdate)
            {
                UpdateColonistStats(pawnStats);
                pawnStats.NextStatUpdate = Rand.Range(150f, 450f);
                pawnStats.LastStatUpdate = Find.TickManager.TicksGame;

                // Log.Message(
                // "CBKF updated stat " + pawnStats.pawn.Name + ", next update in " + pawnStats.NextStatUpdate*Find.TickManager.TickRateMultiplier
                // + " ticks.");
            }
        }

        public static void CheckRelationWithColonists(Pawn pawn)
        {
            var skip = false;

            if (pawn.relations.RelatedPawns.Any(x => x.IsColonist))
            {

                pawn.GetPawnStats().hasRelationWithColonist = true;
                skip = true;

            }
            if (!skip)
            {
                if (pawn.relations.DirectRelations.Any(x => x.otherPawn.IsColonist))
                {
                    pawn.GetPawnStats().hasRelationWithColonist = true;
                }
            }
            pawn.GetPawnStats().relationChecked = true;

        }

        public static void DrawColonistIconsPSI(Pawn pawn)
        {
            // TODO: Make list item like ...OnBar

            if (pawn.Dead || !pawn.Spawned || pawn.holdingOwner == null || pawn.Map == null)
            {
                return;
            }

            PawnStats pawnStats = pawn.GetPawnStats();

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

            PawnStats pawnStats = pawn.GetPawnStats();

            if (pawnStats == null)
            {
                return;
            }

            if (!pawnStats.relationChecked)
            {
                // wait till the pawn is properly spawned. Else FS won't find a relation.
                if (pawnStats.SpawnedAt + 120 > Find.TickManager.TicksGame)
                {
                    CheckRelationWithColonists(pawn);
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

        public override void FinalizeInit()
        {
            Reinit();
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();
            PawnstatsChecker.ResetList();
        }

        public override void LoadedGame()
        {
            base.LoadedGame();
            PawnstatsChecker.ResetList();
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
                            CheckStats(pawn);
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

        private static bool GetThought(
            List<Thought> thoughts,
            ThoughtDef tdef,
            out int stage,
            out string tooltip,
            out float moodOffset)
        {
            tooltip = null;
            stage = -1;
            moodOffset = 0;

            if (thoughts == null || !thoughts.Any())
            {
                return false;
            }

            foreach (Thought thought in thoughts)
            {
                if (thought.def != tdef)
                {
                    continue;
                }

                if (!thought.VisibleInNeedsTab)
                {
                    continue;
                }

                stage = thought.CurStageIndex;
                moodOffset = thought.MoodOffset();
                tooltip = thought.CurStage.description + "\n" + thought.LabelCap + "\n" + moodOffset;
                return true;
            }
            return false;
        }

        private static void GetWithdrawalColor(PawnStats pawnStats, out Color color)
        {
            color = Color.Lerp(ColBlueishGreen, ColVermillion, pawnStats.withDrawalPercent);
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

        public static void UpdateColonistStats(PawnStats pawnStats)
        {
            Pawn pawn = pawnStats.pawn;
            var psiIconList = new List<IconEntryPSI>();
            var barIconList = new List<IconEntryBar>();
            var barSettings = Settings.ColBarSettings;
            var psiSettings = Settings.PsiSettings;


            if (!pawn.Spawned || pawn?.mindState == null || pawn.Map == null || !pawn.RaceProps.Humanlike)
            {
                return;
            }

            if (Current.ProgramState != ProgramState.Playing)
            {
                return;
            }
            float viewOpacity = psiSettings.IconOpacity;

            List<Thought> thoughts = new List<Thought>();

            pawn.needs?.mood?.thoughts?.GetDistinctMoodThoughtGroups(thoughts);
            pawnStats.pawnHealth = 1f - pawn.health.summaryHealth.SummaryHealthPercent;

            // Idle - Colonist icon only
            if (pawn.mindState.IsIdle)
            {
                if (psiSettings.ShowIdle && GenDate.DaysPassed >= 0.1)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Idle, ColorNeutralStatus, viewOpacity));
                }
            }
            // One time traits check
            if (!pawnStats.traitsCheck)
            {
                if (pawn.story?.traits != null)
                {
                    if (pawn.RaceProps.hasGenders)
                    {
                        switch (pawn.gender)
                        {
                            case Gender.Male:
                                pawnStats.BGColor = MaleColor;
                                break;

                            case Gender.Female:
                                pawnStats.BGColor = FemaleColor;
                                break;

                            default: break;
                        }
                    }

                    // Masochist
                    pawnStats.isMasochist = pawn.story.traits.HasTrait(TraitDef.Named("Masochist"));

                    // Masochist trait check
                    pawnStats.painThought =
                        ThoughtDef.Named(
                            pawn.story.traits.HasTrait(TraitDef.Named("Masochist")) ? "MasochistPain" : "Pain");

                    // Pacifist
                    pawnStats.isPacifist = pawn.story.WorkTagIsDisabled(WorkTags.Violent);

                    // Pyromaniac
                    pawnStats.isPyromaniac = pawn.story.traits.HasTrait(TraitDefOf.Pyromaniac);

                    // Prostho
                    if (pawn.story.traits.HasTrait(TraitDefOf.Prosthophobe))
                    {
                        pawnStats.prostho = -1;
                    }

                    if (pawn.story.traits.HasTrait(TraitDef.Named("Prosthophile")))
                    {
                        pawnStats.prostho = 1;
                    }

                    // Night Owl
                    if (pawn.story.traits.HasTrait(TraitDef.Named("NightOwl")))
                    {
                        pawnStats.isNightOwl = true;
                    }

                    // Jealous
                    if (pawn.story.traits.HasTrait(TraitDef.Named("Jealous")))
                    {
                        pawnStats.hasJealousTrait = true;
                    }

                    // Drug desire
                    if (pawn.story.traits.HasTrait(TraitDefOf.DrugDesire))
                    {
                        pawnStats.drugDesire = pawn.story.traits.DegreeOfTrait(TraitDefOf.DrugDesire);
                        pawnStats.drugUserLabel = pawn.story.traits.GetTrait(TraitDefOf.DrugDesire).LabelCap;
                    }

                    // Greedy
                    if (pawn.story.traits.HasTrait(TraitDefOf.Greedy))
                    {
                        pawnStats.hasGreedyTrait = true;
                    }

                    pawnStats.traitsCheck = true;
                }
            }

            if (pawn.Dead)
            {
                pawnStats.BGColor = Color.gray;
            }

            if (pawnStats.isPacifist)
            {
                if (barSettings.ShowPacific)
                {
                    barIconList.Add(
                        new IconEntryBar(
                            Icon.Pacific,
                            ColBlueishGreen,
                            "IsIncapableOfViolence".Translate(pawn.NameStringShort)));
                }
                if (psiSettings.ShowPacific)
                {
                    psiIconList.Add(
                        new IconEntryPSI(
                            Icon.Pacific,
                            ColBlueishGreen,
                            viewOpacity));
                }
            }

            if (pawn.equipment.Primary == null && !pawn.IsPrisoner && !pawnStats.isPacifist)
            {
                if (barSettings.ShowUnarmed)
                {
                    barIconList.Add(
                        new IconEntryBar(
                            Icon.Unarmed,
                            ColorNeutralStatusOpaque,
                            "PSI.Settings.Visibility.Unarmed".Translate()));
                }
                if (psiSettings.ShowUnarmed)
                {
                    psiIconList.Add(
                        new IconEntryPSI(
                            Icon.Unarmed,
                            ColorNeutralStatusOpaque,
                            viewOpacity));
                }
            }

            // Trait Pyromaniac
            if (pawnStats.isPyromaniac)
            {
                if (barSettings.ShowPyromaniac)
                {
                    barIconList.Add(
                        new IconEntryBar(
                            Icon.Pyromaniac,
                            ColYellow,
                            "PSI.Settings.Visibility.Pyromaniac".Translate()));
                }
                if (psiSettings.ShowPyromaniac)
                {
                    psiIconList.Add(
                        new IconEntryPSI(
                            Icon.Pyromaniac,
                            ColYellow,
                           ViewOpacityCrit));
                }
            }

            // efficiency
            float efficiency = psiSettings.LimitEfficiencyLess;
            float efficiencyTotal = 1f;
            string efficiencyTip = null;
            bool flag = false;
            array = _pawnCapacities;
            foreach (PawnCapacityDef pawnCapacityDef in array)
            {
                if (pawnCapacityDef != PawnCapacityDefOf.Consciousness)
                {
                    float level = pawn.health.capacities.GetLevel(pawnCapacityDef);
                    if (level < efficiency)
                    {
                        if (efficiencyTip.NullOrEmpty())
                        {
                            efficiencyTip = "PSI.Efficiency".Translate() + ": " + pawnCapacityDef.LabelCap + " "
                                                      + level.ToStringPercent();
                        }
                        else
                        {
                            efficiencyTip += "\n" + "PSI.Efficiency".Translate() + ": " + pawnCapacityDef.LabelCap + " "
                                             + level.ToStringPercent();
                        }
                        efficiencyTotal = Mathf.Min(level, efficiencyTotal);
                        flag = true;
                    }
                }
            }
            if (flag)
            {
                if (barSettings.ShowEffectiveness)
                {
                    barIconList.Add(
                        new IconEntryBar(
                            Icon.Effectiveness,
                            gradientRedAlertToNeutral.Evaluate(efficiency / Settings.PsiSettings.LimitEfficiencyLess),
                            efficiencyTip));
                }
                if (psiSettings.ShowEffectiveness)
                {
                    psiIconList.Add(
                        new IconEntryPSI(
                            Icon.Effectiveness,
                            gradientRedAlertToNeutral.Evaluate(efficiencyTotal / Settings.PsiSettings.LimitEfficiencyLess),
                            ViewOpacityCrit));
                }
            }

            // target
            pawnStats.TargetPos = Vector3.zero;

            if (pawn.jobs.curJob != null)
            {
                JobDriver curDriver = pawn.jobs.curDriver;
                Job curJob = pawn.jobs.curJob;
                LocalTargetInfo targetInfo = curJob.targetA;
                if (curDriver is JobDriver_HaulToContainer || curDriver is JobDriver_HaulToCell
                    || curDriver is JobDriver_FoodDeliver || curDriver is JobDriver_FoodFeedPatient
                    || curDriver is JobDriver_TakeToBed || curDriver is JobDriver_TakeBeerOutOfFermentingBarrel)
                {
                    targetInfo = curJob.targetB;
                }

                if (curDriver is JobDriver_DoBill bill)
                {
                    JobDriver_DoBill jobDriverDoBill = bill;
                    if (jobDriverDoBill.workLeft >= 0.0)
                    {
                        targetInfo = curJob.targetA;
                    }
                    else if (jobDriverDoBill.workLeft <= 0.01f)
                    {
                        targetInfo = curJob.targetB;
                    }
                }

                if (curDriver is JobDriver_Hunt && pawn.carryTracker?.CarriedThing != null)
                {
                    targetInfo = curJob.targetB;
                }

                if (curJob.def == JobDefOf.Wait)
                {
                    targetInfo = null;
                }

                if (curDriver is JobDriver_Ingest)
                {
                    targetInfo = null;
                }

                if (curJob.def == JobDefOf.LayDown && pawn.InBed())
                {
                    targetInfo = null;
                }

                if (!curJob.playerForced && curJob.def == JobDefOf.Goto)
                {
                    targetInfo = null;
                }

                bool flag;
                if (targetInfo != null)
                {
                    IntVec3 arg2420 = targetInfo.Cell;
                    flag = false;
                }
                else
                {
                    flag = true;
                }

                if (!flag)
                {
                    Vector3 a = targetInfo.Cell.ToVector3Shifted();
                    pawnStats.TargetPos = a + new Vector3(0f, 3f, 0f);
                }
            }

            // temperature
            float temperatureForCell = GenTemperature.GetTemperatureForCell(pawn.Position, pawn.Map);

            pawnStats.TooCold = (float)((pawn.ComfortableTemperatureRange().min
                                         - (double)Settings.PsiSettings.LimitTempComfortOffset - temperatureForCell)
                                        / 10f);

            pawnStats.TooHot = (float)((temperatureForCell - (double)pawn.ComfortableTemperatureRange().max
                                        - Settings.PsiSettings.LimitTempComfortOffset) / 10f);

            pawnStats.TooCold = Mathf.Clamp(pawnStats.TooCold, 0f, 2f);

            pawnStats.TooHot = Mathf.Clamp(pawnStats.TooHot, 0f, 2f);

            // Too Cold & too hot
            if (pawnStats.TooCold > 0f)
            {
                if (barSettings.ShowTooCold)
                {
                    barIconList.Add(new IconEntryBar(Icon.TooCold, gradient4.Evaluate(pawnStats.TooCold / 2), "TooCold".Translate()));
                }
                if (psiSettings.ShowTooCold)
                {
                    psiIconList.Add(
                        new IconEntryPSI(
                            Icon.TooCold,
                            gradient4.Evaluate(pawnStats.TooCold / 2),
                            ViewOpacityCrit));
                }
            }

            if (pawnStats.TooHot > 0f)
            {
                if (barSettings.ShowTooHot)
                {
                    barIconList.Add(new IconEntryBar(Icon.TooCold, gradient4.Evaluate(pawnStats.TooHot / 2), "TooHot".Translate()));
                }
                if (psiSettings.ShowTooHot)
                {
                    psiIconList.Add(
                        new IconEntryPSI(Icon.TooCold, gradient4.Evaluate(pawnStats.TooHot / 2), ViewOpacityCrit));
                }
            }

            // Mental Sanity
            pawnStats.MentalSanity = null;
            if (pawn.mindState != null && pawn.InMentalState)
            {
                pawnStats.MentalSanity = pawn.MentalStateDef;
            }

            if (pawnStats.MentalSanity != null)
            {
                if (pawn.InAggroMentalState)
                {
                    if (barSettings.ShowAggressive)
                    {
                        barIconList.Add(new IconEntryBar(Icon.Aggressive, ColVermillion, null));
                    }

                    if (psiSettings.ShowAggressive)
                    {
                        psiIconList.Add(new IconEntryPSI(Icon.Aggressive, ColVermillion, ViewOpacityCrit));
                    }

                    // Give Up Exit
                    if (barSettings.ShowLeave)
                    {
                        if (pawnStats.MentalSanity == MentalStateDefOf.PanicFlee)
                        {
                            barIconList.Add(new IconEntryBar(Icon.Leave, ColVermillion, null));
                        }
                    }
                    if (psiSettings.ShowLeave)
                    {
                        if (pawnStats.MentalSanity == MentalStateDefOf.PanicFlee)
                        {
                            psiIconList.Add(new IconEntryPSI(Icon.Leave, ColVermillion, ViewOpacityCrit));
                        }
                    }

                    // Daze Wander
                    if (pawnStats.MentalSanity == MentalStateDefOf.WanderSad)
                    {
                        if (barSettings.ShowDazed)
                        {
                            // + MentalStateDefOf.WanderPsychotic
                            barIconList.Add(new IconEntryBar(Icon.Dazed, ColYellow, null));
                        }
                        if (psiSettings.ShowDazed)
                        {
                            // + MentalStateDefOf.WanderPsychotic
                            psiIconList.Add(new IconEntryPSI(Icon.Dazed, ColYellow, ViewOpacityCrit));
                        }
                    }

                    // PanicFlee
                    if (pawnStats.MentalSanity == MentalStateDefOf.PanicFlee)
                    {
                        if (barSettings.ShowPanic)
                        {
                            barIconList.Add(new IconEntryBar(Icon.Panic, ColYellow, null));
                        }
                        if (psiSettings.ShowPanic)
                        {
                            psiIconList.Add(new IconEntryPSI(Icon.Panic, ColYellow, ViewOpacityCrit));
                        }
                    }
                }
            }
            // Hungry
            if (pawn.needs.food.CurLevel < (double)Settings.PsiSettings.LimitFoodLess)
            {
                if (barSettings.ShowHungry)
                {
                    string tooltip = pawn.needs.food.GetTipString();

                    barIconList.Add(
                        new IconEntryBar(
                            Icon.Hungry,
                            gradientRedAlertToNeutral.Evaluate(
                                pawn.needs.food.CurLevel / Settings.PsiSettings.LimitFoodLess),
                            tooltip));
                }
                if (psiSettings.ShowHungry)
                {
                    string tooltip = pawn.needs.food.GetTipString();

                    psiIconList.Add(
                        new IconEntryPSI(
                            Icon.Hungry,
                            gradientRedAlertToNeutral.Evaluate(
                                pawn.needs.food.CurLevel / Settings.PsiSettings.LimitFoodLess),
                            ViewOpacityCrit));
                }
            }

            // Tired
            if (pawn.needs.rest.CurLevel < (double)Settings.PsiSettings.LimitRestLess)
            {
                if (barSettings.ShowTired)
                {
                    string tooltip = pawn.needs.rest.CurCategory.GetLabel();
                    barIconList.Add(
                        new IconEntryBar(
                            Icon.Tired,
                            gradientRedAlertToNeutral.Evaluate(
                                pawn.needs.rest.CurLevel / Settings.PsiSettings.LimitRestLess),
                            tooltip));
                }
                if (psiSettings.ShowTired)
                {
                    string tooltip = pawn.needs.rest.CurCategory.GetLabel();
                    psiIconList.Add(
                        new IconEntryPSI(
                            Icon.Tired,
                            gradientRedAlertToNeutral.Evaluate(
                                pawn.needs.rest.CurLevel / Settings.PsiSettings.LimitRestLess),
                            ViewOpacityCrit));
                }
            }


            // Mental Breaker for MoodBars
            if (pawn.needs?.mood != null)
            {
                pawnStats.Mb = pawn.mindState.mentalBreaker;
                pawnStats.Mood = pawn.needs.mood;
            }
            else
            {
                pawnStats.Mood = null;
                pawnStats.Mb = null;
            }

            // Health Calc
            pawnStats.DiseaseDisappearance = 1f;
            pawnStats.HasLifeThreateningDisease = false;
            pawnStats.HealthDisease = 1f;

            // Drug addiction
            List<Hediff> hediffs = null;

            // Sick thoughts
            if (pawn.health?.hediffSet != null)
            {
                hediffs = pawn.health.hediffSet.hediffs;

                // Health
                // Infection

                // Bleed rate
                pawnStats.BleedRate = Mathf.Clamp01(
                    pawn.health.hediffSet.BleedRateTotal * Settings.PsiSettings.LimitBleedMult);

                if (pawnStats.BleedRate > 0.0f)
                {
                    if (barSettings.ShowBloodloss)
                    {
                        string tooltip = "BleedingRate".Translate() + ": "
                                     + pawn.health.hediffSet.BleedRateTotal.ToStringPercent() + "/d";

                        barIconList.Add(
                            new IconEntryBar(
                                Icon.Bloodloss,
                                gradientRedAlertToNeutral.Evaluate(1.0f - pawnStats.BleedRate),
                                tooltip));
                    }
                }

                if (pawn.Map != null)
                {
                    if (HealthAIUtility.ShouldBeTendedNowUrgent(pawn))
                    {
                        barIconList.Add(
                            new IconEntryBar(
                                Icon.MedicalAttention,
                                ColVermillion,
                                "NeedsTendingNowUrgent".Translate()));
                    }
                    if (HealthAIUtility.ShouldBeTendedNow(pawn))
                    {
                        barIconList.Add(
                            new IconEntryBar(Icon.MedicalAttention, ColYellow, "NeedsTendingNow".Translate()));
                    }
                    if (HealthAIUtility.ShouldHaveSurgeryDoneNow(pawn))
                    {
                        barIconList.Add(
                            new IconEntryBar(
                                Icon.MedicalAttention,
                                ColBlueishGreen,
                                "ShouldHaveSurgeryDoneNow".Translate()));
                    }
                }
            }

            if (pawn.health.hediffSet.AnyHediffMakesSickThought && !pawn.Destroyed && pawn.playerSettings.medCare >= 0)
            {
                if (hediffs != null)
                {
                    pawnStats.severity = 0f;
                    pawnStats.immunity = 0f;
                    foreach (Hediff hediff in hediffs)
                    {
                        if (!hediff.Visible || hediff.IsOld() || !hediff.def.makesSickThought
                            || hediff.LabelCap.NullOrEmpty() || hediff.SeverityLabel.NullOrEmpty())
                        {
                            continue;
                        }

                        pawnStats.ToxicBuildUpVisible = 0;
                        pawnStats.healthTip = hediff.LabelCap;
                        if (!thoughts.NullOrEmpty())
                        {
                            GetThought(
                                thoughts,
                                ThoughtDef.Named("Sick"),
                                out int dummy,
                                out pawnStats.sickTip,
                                out pawnStats.sickMoodOffset);
                        }

                        // pawnStats.ToxicBuildUpVisible
                        if (hediff.def == HediffDefOf.ToxicBuildup)
                        {
                            pawnStats.toxicTip = hediff.LabelCap + "\n" + hediff.SeverityLabel;
                            pawnStats.ToxicBuildUpVisible = Mathf.InverseLerp(0.049f, 1f, hediff.Severity);
                        }

                        HediffComp_Immunizable compImmunizable = hediff.TryGetComp<HediffComp_Immunizable>();
                        if (compImmunizable != null)
                        {
                            pawnStats.severity = Mathf.Max(pawnStats.severity, hediff.Severity);
                            pawnStats.immunity = compImmunizable.Immunity;
                            float basehealth = pawnStats.HealthDisease - (pawnStats.severity - pawnStats.immunity / 4)
                                               - 0.25f;
                            pawnStats.HasLifeThreateningDisease = true;
                            pawnStats.HealthDisease = basehealth;
                        }
                        else
                        {
                            continue;
                        }

                        if (!hediff.def.PossibleToDevelopImmunityNaturally())
                        {
                            continue;
                        }

                        if (hediff.CurStage?.capMods == null)
                        {
                            continue;
                        }

                        if (!hediff.CurStage.everVisible)
                        {
                            continue;
                        }

                        if (hediff.FullyImmune())
                        {
                            continue;
                        }

                        if (!hediff.def.tendable)
                        {
                            continue;
                        }

                        if (Math.Abs(pawn.health.immunity.GetImmunity(hediff.def) - 1.0) < 0.05)
                        {
                            continue;
                        }

                        if (pawnStats.DiseaseDisappearance > compImmunizable.Immunity)
                        {
                            pawnStats.DiseaseDisappearance = compImmunizable.Immunity;
                        }

                        // break;
                    }
                }
                if (pawnStats.DiseaseDisappearance < Settings.PsiSettings.LimitDiseaseLess)
                {
                    string tooltip = pawnStats.sickTip + "\n" + pawnStats.healthTip + "\n" + "Immunity".Translate()
                                     + " / " + "PSI.DiseaseProgress".Translate() + ": \n"
                                     + pawnStats.immunity.ToStringPercent() + " / "
                                     + pawnStats.severity.ToStringPercent() + ": \n" + pawnStats.sickMoodOffset;

                    if (barSettings.ShowHealth)
                    {
                        // Regular Sickness
                        barIconList.Add(
                            new IconEntryBar(
                                Icon.Health,
                                gradient4.Evaluate(pawnStats.DiseaseDisappearance / psiSettings.LimitDiseaseLess),
                                tooltip));
                    }
                    if (psiSettings.ShowHealth)
                    {
                        // Regular Sickness
                        psiIconList.Add(
                            new IconEntryPSI(
                                Icon.Health,
                                gradient4.Evaluate(pawnStats.DiseaseDisappearance / psiSettings.LimitDiseaseLess),
                                ViewOpacityCrit));
                    }
                }
                else if (pawn.health.summaryHealth.SummaryHealthPercent < 1f)
                {
                    string tooltip = "Health".Translate() + ": "
                                     + pawn.health.summaryHealth.SummaryHealthPercent.ToStringPercent();
                    if (barSettings.ShowHealth)
                    {
                        barIconList.Add(
                        new IconEntryBar(
                            Icon.Health,
                            gradient4.Evaluate(pawnStats.pawnHealth),
                            tooltip));
                    }
                    if (psiSettings.ShowHealth)
                    {
                        psiIconList.Add(
                        new IconEntryPSI(
                            Icon.Health,
                            gradient4.Evaluate(pawnStats.pawnHealth),
                            ViewOpacityCrit));
                    }
                }
            }
            // Toxicity buildup
            if (pawnStats.ToxicBuildUpVisible > 0f)
            {
                if (barSettings.ShowToxicity)
                {
                    string tooltip = pawnStats.toxicTip;

                    barIconList.Add(new IconEntryBar(Icon.Toxicity, gradient4.Evaluate(pawnStats.ToxicBuildUpVisible), tooltip));
                }
                if (psiSettings.ShowToxicity)
                {
                    string tooltip = pawnStats.toxicTip;

                    psiIconList.Add(new IconEntryPSI(Icon.Toxicity, gradient4.Evaluate(pawnStats.ToxicBuildUpVisible), ViewOpacityCrit));
                }
            }


            // Apparel Calc
            float worstApparel = 999f;
            List<Apparel> apparelListForReading = pawn.apparel.WornApparel;
            foreach (Apparel t in apparelListForReading)
            {
                float curApparel = t.HitPoints / (float)t.MaxHitPoints;
                if (curApparel >= 0f && curApparel < worstApparel)
                {
                    worstApparel = curApparel;
                }
            }

            pawnStats.ApparelHealth = worstApparel;

            // Apparel
            if (pawnStats.ApparelHealth < (double)Settings.PsiSettings.LimitApparelHealthLess)
            {
                if (barSettings.ShowApparelHealth)
                {
                    double pawnApparelHealth = pawnStats.ApparelHealth / (double)psiSettings.LimitApparelHealthLess;
                    barIconList.Add(
                        new IconEntryBar(
                            Icon.ApparelHealth,
                            gradientRedAlertToNeutral.Evaluate((float)pawnApparelHealth),
                            null));

                }
                if (psiSettings.ShowApparelHealth)
                {
                    double pawnApparelHealth = pawnStats.ApparelHealth / (double)psiSettings.LimitApparelHealthLess;
                    psiIconList.Add(
                        new IconEntryPSI(
                            Icon.ApparelHealth,
                            gradientRedAlertToNeutral.Evaluate((float)pawnApparelHealth),
                            viewOpacity));

                }
            }

            if (!thoughts.NullOrEmpty())
            {
                if (pawnStats.prostho != 0)
                {
                    switch (pawnStats.prostho)
                    {
                        case -1:
                            GetThought(
                                thoughts,
                                ThoughtDef.Named("ProsthophobeUnhappy"),
                                out pawnStats.prosthoUnhappy,
                                out pawnStats.prosthoTooltip,
                                out pawnStats.prosthoMoodOffset);
                            break;

                        case 1:
                            GetThought(
                                thoughts,
                                ThoughtDef.Named("ProsthophileNoProsthetic"),
                                out pawnStats.prosthoUnhappy,
                                out pawnStats.prosthoTooltip,
                                out pawnStats.prosthoMoodOffset);
                            break;

                        default: break;
                    }
                }

                // Bed status
                if (pawn.ownership.OwnedBed != null)
                {
                    GetThought(
                        thoughts,
                        ThoughtDef.Named("SharedBed"),
                        out pawnStats.BedStatus,
                        out pawnStats.BedStatusTip,
                        out pawnStats.BedStatusMoodOffset);
                }
                else
                {
                    pawnStats.BedStatus = 1;
                    pawnStats.BedStatusTip = "NeedColonistBeds".Translate();
                }

                // Humping
                GetThought(
                    thoughts,
                    ThoughtDef.Named("WantToSleepWithSpouseOrLover"),
                    out pawnStats.wantsToHump,
                    out pawnStats.humpTip,
                    out pawnStats.humpMoodOffset);

                // Cabin Fever
                if (GetThought(
                    thoughts,
                    ThoughtDef.Named("CabinFever"),
                    out pawnStats.CabinFeverMoodLevel,
                    out pawnStats.cabinFeverTip,
                    out pawnStats.CabinFeverMoodOffset))
                {
                    if (barSettings.ShowCabinFever)
                    {
                        string tooltip = pawnStats.cabinFeverTip;
                        barIconList.Add(new IconEntryBar(Icon.CabinFever, Evaluate(pawnStats.CabinFeverMoodOffset), tooltip));
                    }
                    if (psiSettings.ShowCabinFever)
                    {
                        psiIconList.Add(new IconEntryPSI(Icon.CabinFever, Evaluate(pawnStats.CabinFeverMoodOffset), ViewOpacityCrit));
                    }
                }

                // Pain
                if (GetThought(
                    thoughts,
                    pawnStats.painThought,
                    out pawnStats.PainMoodLevel,
                    out pawnStats.painTip,
                    out pawnStats.PainMoodOffset))
                {
                    if (barSettings.ShowPain)
                    {
                        barIconList.Add(new IconEntryBar(Icon.Pain, Evaluate(pawnStats.PainMoodOffset), pawnStats.painTip));
                    }
                    if (psiSettings.ShowPain)
                    {
                        psiIconList.Add(new IconEntryPSI(Icon.Pain, Evaluate(pawnStats.PainMoodOffset), ViewOpacityCrit));
                    }
                }






                // Naked
                if (GetThought(
                    thoughts,
                    ThoughtDefOf.Naked,
                    out pawnStats.feelsNaked,
                    out pawnStats.nakedTip,
                    out pawnStats.nakedMoodOffset))
                {
                    if (barSettings.ShowNaked)
                    {
                        barIconList.Add(
                            new IconEntryBar(Icon.Naked, Evaluate(pawnStats.nakedMoodOffset), pawnStats.nakedTip));
                    }
                    if (psiSettings.ShowNaked)
                    {
                        psiIconList.Add(
                            new IconEntryPSI(Icon.Naked, Evaluate(pawnStats.nakedMoodOffset), viewOpacity));
                    }
                }

                // Night Owl
                if (pawnStats.isNightOwl)
                {
                    if (GetThought(
                        thoughts,
                        ThoughtDef.Named("NightOwlDuringTheDay"),
                        out pawnStats.nightOwlUnhappy,
                        out pawnStats.nightOwlTip,
                        out pawnStats.nightOwlMoodOffset))
                    {
                        if (barSettings.ShowNightOwl)
                        {
                            barIconList.Add(
                                new IconEntryBar(
                                    Icon.NightOwl,
                                    Evaluate(pawnStats.nightOwlMoodOffset),
                                    pawnStats.nightOwlTip));
                        }
                        if (psiSettings.ShowNightOwl)
                        {
                            psiIconList.Add(
                                new IconEntryPSI(
                                    Icon.NightOwl,
                                    Evaluate(pawnStats.nightOwlMoodOffset),
                                    ViewOpacityCrit));
                        }
                    }
                }

                // Greedy
                if (pawnStats.hasGreedyTrait)
                {
                    if (GetThought(
                        thoughts,
                        ThoughtDef.Named("Greedy"),
                        out pawnStats.greedyThought,
                        out pawnStats.greedyTooltip,
                        out pawnStats.greedyMoodOffset))
                    {
                        if (barSettings.ShowGreedy)
                        {
                            barIconList.Add(
                                new IconEntryBar(
                                    Icon.Greedy,
                                    Evaluate(pawnStats.greedyMoodOffset),
                                    pawnStats.greedyTooltip));
                        }
                        if (psiSettings.ShowGreedy)
                        {
                            psiIconList.Add(
                                new IconEntryPSI(
                                    Icon.Greedy,
                                    Evaluate(pawnStats.greedyMoodOffset),
                                    viewOpacity));
                        }
                    }
                }

                // Jealous
                if (pawnStats.hasJealousTrait)
                {
                    if (GetThought(
                        thoughts,
                        ThoughtDef.Named("Jealous"),
                        out pawnStats.jealousThought,
                        out pawnStats.jealousTooltip,
                        out pawnStats.jealousMoodOffset))
                    {
                        if (barSettings.ShowJealous)
                        {
                            barIconList.Add(
                                new IconEntryBar(
                                    Icon.Jealous,
                                    Evaluate(pawnStats.jealousMoodOffset),
                                    pawnStats.jealousTooltip));

                        }
                        if (psiSettings.ShowJealous)
                        {
                            psiIconList.Add(
                                new IconEntryPSI(
                                    Icon.Jealous,
                                    Evaluate(pawnStats.jealousMoodOffset),
                                    viewOpacity));

                        }

                    }
                }





                // Unburied
                if (GetThought(
                    thoughts,
                    ThoughtDef.Named("ColonistLeftUnburied"),
                    out pawnStats.unburied,
                    out pawnStats.unburiedTip,
                    out pawnStats.unburiedMoodOffset))
                {
                    if (barSettings.ShowLeftUnburied)
                    {
                        string tooltip = pawnStats.unburiedTip;
                        barIconList.Add(new IconEntryBar(Icon.LeftUnburied, Evaluate(pawnStats.unburiedMoodOffset), tooltip));
                    }
                    if (psiSettings.ShowLeftUnburied)
                    {
                        string tooltip = pawnStats.unburiedTip;
                        psiIconList.Add(new IconEntryPSI(Icon.LeftUnburied, Evaluate(pawnStats.unburiedMoodOffset), viewOpacity));
                    }
                }
            }

            pawnStats.isAddict = false;
            pawnStats.withDrawal = false;
            pawnStats.withDrawalPercent = 0f;
            pawnStats.addictionLabel = null;
            if (hediffs != null)
            {
                foreach (Hediff hediff in hediffs)
                {
                    if (hediff is Hediff_Addiction)
                    {
                        pawnStats.isAddict = true;
                        pawnStats.withDrawalPercent = hediff.Severity;
                        pawnStats.withDrawal = hediff.CurStageIndex > 0;
                        if (pawnStats.addictionLabel.NullOrEmpty())
                        {
                            pawnStats.addictionLabel = hediff.LabelCap;
                        }
                        else
                        {
                            pawnStats.addictionLabel += "\n" + hediff.LabelCap;
                        }
                    }
                }
            }

            if ((pawnStats.isAddict || pawnStats.drugDesire != 0))
            {
                Color color = new Color();
                string tooltip = null;
                if (pawnStats.isAddict)
                {
                    if (pawnStats.withDrawal)
                    {
                        GetWithdrawalColor(pawnStats, out color);
                    }
                    else
                    {
                        color = ColVermillion;
                    }

                    if (!pawnStats.drugUserLabel.NullOrEmpty())
                    {
                        tooltip = pawnStats.drugUserLabel + "\n" + pawnStats.addictionLabel;
                    }
                    else
                    {
                        tooltip = pawnStats.addictionLabel;
                    }
                }
                else
                {
                    switch (pawnStats.drugDesire)
                    {
                        case -1:
                            color = ColSkyBlue;
                            break;

                        case 1:
                            color = ColYellow;
                            break;

                        case 2:
                            color = ColOrange;
                            break;

                        default: break;
                    }

                    tooltip = pawnStats.drugUserLabel;
                }
                if (barSettings.ShowDrunk)
                {
                    barIconList.Add(new IconEntryBar(Icon.Drunk, color, tooltip));
                }
                if (psiSettings.ShowDrunk)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Drunk, color, ViewOpacityCrit));
                }

            }

            // Bed status
            if (pawnStats.wantsToHump > -1)
            {
                if (barSettings.ShowBedroom)
                {
                    barIconList.Add(new IconEntryBar(Icon.Bedroom, Evaluate(pawnStats.humpMoodOffset), pawnStats.humpTip));
                }
                if (psiSettings.ShowBedroom)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Bedroom, Evaluate(pawnStats.humpMoodOffset), ViewOpacityCrit));
                }
            }
            else if (pawnStats.BedStatus > -1)
            {
                if (barSettings.ShowBedroom)
                {
                    barIconList.Add(new IconEntryBar(Icon.Bedroom, Evaluate(pawnStats.BedStatusMoodOffset), pawnStats.BedStatusTip));
                }
                if (psiSettings.ShowBedroom)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Bedroom, Evaluate(pawnStats.BedStatusMoodOffset), viewOpacity));
                }

                // Moods caused by traits
                if (pawnStats.prosthoUnhappy > -1)
                {
                    if (pawnStats.prostho == 1)
                    {
                        if (barSettings.ShowProsthophile)
                        {
                            barIconList.Add(
                                new IconEntryBar(
                                    Icon.Prosthophile,
                                    Evaluate(pawnStats.prosthoMoodOffset),
                                    pawnStats.prosthoTooltip));
                        }
                        if (psiSettings.ShowProsthophile)
                        {
                            psiIconList.Add(
                                new IconEntryPSI(
                                    Icon.Prosthophile,
                                    Evaluate(pawnStats.prosthoMoodOffset),
                                    viewOpacity));
                        }
                    }

                    if (pawnStats.prostho == -1)
                    {
                        if (barSettings.ShowProsthophobe)
                        {
                            barIconList.Add(
                                new IconEntryBar(
                                    Icon.Prosthophobe,
                                    Evaluate(pawnStats.prosthoMoodOffset),
                                    pawnStats.prosthoTooltip));

                        }
                        if (psiSettings.ShowProsthophobe)
                        {
                            psiIconList.Add(
                                new IconEntryPSI(
                                    Icon.Prosthophobe,
                                    Evaluate(pawnStats.prosthoMoodOffset),
                                    viewOpacity));

                        }
                    }
                }

                pawnStats.BarIcons = barIconList;
                pawnStats.PSIIcons = psiIconList;
            }
        }
    }
}