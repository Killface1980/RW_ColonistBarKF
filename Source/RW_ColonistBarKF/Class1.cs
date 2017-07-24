using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColonistBarKF
{
    using CBKF_Menu;

    using ColonistBarKF.Bar;

    using PawnStats;

    using RimWorld;

    using UnityEngine;

    using Verse;
    using Verse.AI;

    using static CBKF_Menu.ColonistBarTextures;

    public static class Class1
    {

            public static PawnCapacityDef[] _pawnCapacities;
            private static PawnCapacityDef[] array;

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
                ColonistBar_KF.MarkColonistsDirty();
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

                            skinMat = PSIMaterials[Icons.TargetSkin];
                            hairMat = PSIMaterials[Icons.TargetHair];
                            targetMat = PSIMaterials[Icons.Target];

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

        public static void UpdateColonistStats(PawnStats pawnStats)
        {
            Pawn pawn = pawnStats.pawn;

            if (!pawn.Spawned || pawn?.mindState == null || pawn.Map == null || !pawn.RaceProps.Humanlike)
            {
                return;
            }

            if (Current.ProgramState != ProgramState.Playing)
            {
                return;
            }

            List<Thought> thoughts = new List<Thought>();

            pawn.needs?.mood?.thoughts?.GetDistinctMoodThoughtGroups(thoughts);
            pawnStats.pawnHealth = pawn.health.summaryHealth.SummaryHealthPercent;

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
                                pawnStats.BGColor = ColonistBarTextures.MaleColor;
                                break;
                            case Gender.Female:
                                pawnStats.BGColor = ColonistBarTextures.FemaleColor;
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
                        pawnStats.isJealous = true;
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
                        pawnStats.isGreedy = true;
                    }

                    pawnStats.traitsCheck = true;


                }
            }

            if (pawn.Dead)
            {
                pawnStats.BGColor = Color.gray;
            }

            // efficiency
            float efficiency = 10f;

            array = _pawnCapacities;
            foreach (PawnCapacityDef pawnCapacityDef in array)
            {
                if (pawnCapacityDef != PawnCapacityDefOf.Consciousness)
                {
                    float level = pawn.health.capacities.GetLevel(pawnCapacityDef);

                    if (efficiency > level)
                    {
                        efficiency = level;
                        pawnStats.efficiencyTip = pawnCapacityDef.LabelCap;
                    }
                }

                if (efficiency < 0f)
                {
                    efficiency = 0f;
                }
            }

            pawnStats.TotalEfficiency = efficiency;

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

                if (curDriver is JobDriver_Hunt && (pawn.carryTracker?.CarriedThing != null))
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

                if ((curJob.def == JobDefOf.LayDown) && pawn.InBed())
                {
                    targetInfo = null;
                }

                if (!curJob.playerForced && (curJob.def == JobDefOf.Goto))
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

            pawnStats.TooCold =
                (float)
                ((pawn.ComfortableTemperatureRange().min - (double)Settings.PsiSettings.LimitTempComfortOffset
                  - temperatureForCell) / 10f);

            pawnStats.TooHot =
                (float)
                ((temperatureForCell - (double)pawn.ComfortableTemperatureRange().max
                  - Settings.PsiSettings.LimitTempComfortOffset) / 10f);

            pawnStats.TooCold = Mathf.Clamp(pawnStats.TooCold, 0f, 2f);

            pawnStats.TooHot = Mathf.Clamp(pawnStats.TooHot, 0f, 2f);

            /*
                        // Drunkness - DEACTIVATED FOR NOW
                        pawnStats.Drunkness =  DrugUtility.DrunknessPercent(pawn);
                    */
            // Mental Sanity
            pawnStats.MentalSanity = null;
            if ((pawn.mindState != null) && pawn.InMentalState)
            {
                pawnStats.MentalSanity = pawn.MentalStateDef;
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

                pawnStats.IsSick = pawn.health.hediffSet.AnyHediffMakesSickThought;

                // Bleed rate
                pawnStats.BleedRate =
                    Mathf.Clamp01(pawn.health.hediffSet.BleedRateTotal * Settings.PsiSettings.LimitBleedMult);

                if (pawn.Map != null)
                {
                    pawnStats.ShouldBeTendedNowUrgent = HealthAIUtility.ShouldBeTendedNowUrgent(pawn);
                    pawnStats.ShouldBeTendedNow = HealthAIUtility.ShouldBeTendedNow(pawn);
                    pawnStats.ShouldHaveSurgeryDoneNow = HealthAIUtility.ShouldHaveSurgeryDoneNow(pawn);
                }
            }

            if (pawnStats.IsSick && !pawn.Destroyed && (pawn.playerSettings.medCare >= 0))
            {
                if (hediffs != null)
                {
                    pawnStats.severity = 0f;
                    pawnStats.immunity = 0f;
                    foreach (Hediff hediff in hediffs)
                    {
                        if (!hediff.Visible || hediff.IsOld() || !hediff.def.makesSickThought || hediff.LabelCap.NullOrEmpty() || hediff.SeverityLabel.NullOrEmpty())
                        {
                            continue;
                        }

                        pawnStats.ToxicBuildUpVisible = 0;
                        pawnStats.healthTip = hediff.LabelCap;
                        if (!thoughts.NullOrEmpty())
                            GetThought(thoughts, ThoughtDef.Named("Sick"), out int dummy, out pawnStats.sickTip, out pawnStats.sickMoodOffset);

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
                        //     break;
                    }
                }
            }

            // Apparel Calc
            float worstApparel = 999f;
            List<Apparel> apparelListForReading = pawn.apparel.WornApparel;
            foreach (Apparel t in apparelListForReading)
            {
                float curApparel = t.HitPoints / (float)t.MaxHitPoints;
                if ((curApparel >= 0f) && (curApparel < worstApparel))
                {
                    worstApparel = curApparel;
                }
            }

            pawnStats.ApparelHealth = worstApparel;

            if (!thoughts.NullOrEmpty())
            {
                if (pawnStats.prostho != 0)
                {
                    switch (pawnStats.prostho)
                    {
                        case -1:
                            GetThought(thoughts, ThoughtDef.Named("ProsthophobeUnhappy"), out pawnStats.prosthoUnhappy, out pawnStats.prosthoTooltip, out pawnStats.prosthoMoodOffset);
                            break;
                        case 1:
                            GetThought(thoughts, ThoughtDef.Named("ProsthophileNoProsthetic"), out pawnStats.prosthoUnhappy, out pawnStats.prosthoTooltip, out pawnStats.prosthoMoodOffset);
                            break;
                        default:
                            break;
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
                GetThought(
                    thoughts,
                    ThoughtDef.Named("CabinFever"),
                    out pawnStats.CabinFeverMoodLevel,
                    out pawnStats.cabinFeverTip,
                    out pawnStats.CabinFeverMoodOffset);

                // Pain
                GetThought(thoughts, pawnStats.painThought, out pawnStats.PainMoodLevel, out pawnStats.painTip, out pawnStats.PainMoodOffset);


                // Naked
                GetThought(
                    thoughts,
                    ThoughtDefOf.Naked,
                    out pawnStats.feelsNaked,
                    out pawnStats.nakedTip,
                    out pawnStats.nakedMoodOffset);

                // Night Owl
                if (pawnStats.isNightOwl)
                {
                    GetThought(
                        thoughts,
                        ThoughtDef.Named("NightOwlDuringTheDay"),
                        out pawnStats.nightOwlUnhappy,
                        out pawnStats.nightOwlTip,
                        out pawnStats.nightOwlMoodOffset);
                }

                // Greedy
                if (pawnStats.isGreedy)
                {
                    GetThought(thoughts, ThoughtDef.Named("Greedy"), out pawnStats.greedyThought, out pawnStats.greedyTooltip, out pawnStats.greedyMoodOffset);
                }

                // Jealous
                if (pawnStats.isJealous)
                {
                    GetThought(thoughts, ThoughtDef.Named("Jealous"), out pawnStats.jealousThought, out pawnStats.jealousTooltip, out pawnStats.jealousMoodOffset);
                }

                // Unburied
                GetThought(
                    thoughts,
                    ThoughtDef.Named("ColonistLeftUnburied"),
                    out pawnStats.unburied,
                    out pawnStats.unburiedTip,
                    out pawnStats.unburiedMoodOffset);
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
        }

        private static void GetThought(List<Thought> thoughts, ThoughtDef tdef, out int stage, out string tooltip, out float moodOffset)
        {
            tooltip = null;
            stage = -1;
            moodOffset = 0;

            if (thoughts == null || !thoughts.Any())
            {
                return;
            }

            foreach (Thought thought in thoughts)
            {
                if (thought.def != tdef)
                {
                    continue;
                }

                if (!thought.VisibleInNeedsTab)
                    continue;

                stage = thought.CurStageIndex;
                moodOffset = thought.MoodOffset();
                tooltip = thought.CurStage.description + "\n" + thought.LabelCap + "\n" + moodOffset;
                break;
            }
        }

        public static void CheckStats(this Pawn pawn)
        {
            PawnStats pawnStats = GetCache(pawn);
            if (pawnStats != null)
            {
                CheckStats(pawnStats);
            }
        }

        public static void CheckStats(this PawnStats pawnStats)
        {
            if (Find.TickManager.CurTimeSpeed == TimeSpeed.Paused)
            {
                return;
            }

            float nextUpdate = pawnStats.LastStatUpdate
                               + pawnStats.NextStatUpdate * Find.TickManager.TickRateMultiplier;

            if (Find.TickManager.TicksGame > nextUpdate)
            {
                Class1.UpdateColonistStats(pawnStats);
                pawnStats.NextStatUpdate = Rand.Range(150f, 450f);
                pawnStats.LastStatUpdate = Find.TickManager.TicksGame;



                //   Log.Message(
                //       "CBKF updated stat " + pawnStats.pawn.Name + ", next update in " + pawnStats.NextStatUpdate*Find.TickManager.TickRateMultiplier
                //       + " ticks.");
            }

        }

        public static void CheckRelationWithColonists(PawnStats pawnStats)
        {
            if (pawnStats.pawn.relations.RelatedToAnyoneOrAnyoneRelatedToMe)
            {
                foreach (Pawn related in pawnStats.pawn.relations.RelatedPawns)
                {
                    if (related.Faction == Faction.OfPlayer)
                    {
                        pawnStats.hasRelationWithColonist = true;
                    }
                    break;
                }

            }
            pawnStats.relationChecked = true;

        }
        public static List<PawnStats> _pawnCache = new List<PawnStats>();

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
            IconPosVectorsPsi = new Vector3[40];
            for (int index = 0; index < IconPosVectorsPsi.Length; ++index)
            {
                int num1 = index / psiSettings.IconsInColumn;
                int num2 = index % psiSettings.IconsInColumn;
                if (psiSettings.IconsHorizontal)
                {
                    int num3 = num1;
                    num1 = num2;
                    num2 = num3;
                }

                IconPosVectorsPsi[index] =
                    new Vector3(
                        (float)
                        (-0.600000023841858 * psiSettings.IconMarginX
                         - 0.550000011920929 * psiSettings.IconSize * psiSettings.IconOffsetX * num1),
                        3f,
                        (float)
                        (-0.600000023841858 * psiSettings.IconMarginY
                         + 0.550000011920929 * psiSettings.IconSize * psiSettings.IconOffsetY * num2));
            }
        }

        public static void GetWithdrawalColor(PawnStats pawnStats, out Color color)
        {
            color = Color.Lerp(ColBlueishGreen, ColVermillion, pawnStats.withDrawalPercent);
        }

        public static Color Evaluate(float moodOffset)
        {
            return gradient4Mood.Evaluate(Mathf.InverseLerp(-25f, 15f, moodOffset));
        }

        public static PawnStats GetCache(this Pawn pawn)
        {
            foreach (PawnStats c in _pawnCache)
            {
                if (c.pawn == pawn)
                {
                    return c;
                }
            }
            PawnStats n = new PawnStats { pawn = pawn };
            _pawnCache.Add(n);

            if (pawn.Faction == Faction.OfPlayer)
            {
                Class1.UpdateColonistStats(n);
            }
            else
            {
                Class1.CheckRelationWithColonists(n);
            }
            return n;

            // if (!PawnApparelStatCaches.ContainsKey(pawn))
            // {
            // PawnApparelStatCaches.Add(pawn, new StatCache(pawn));
            // }
            // return PawnApparelStatCaches[pawn];
        }

    }
}
