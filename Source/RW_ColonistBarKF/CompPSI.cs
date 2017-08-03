using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColonistBarKF
{
    using System.Runtime.CompilerServices;

    using static ColonistBarKF.Bar.ColonistBarTextures;
    using ColonistBarKF.PSI;
    using static ColonistBarKF.PSI.PSIDrawer;

    using RimWorld;
    using RimWorld.Planet;

    using UnityEngine;

    using Verse;
    using Verse.AI;

    using static Settings;

    using static Statics;

    [StaticConstructorOnStartup]
    public class CompPSI : ThingComp
    {
        public float TotalEfficiency = 1f;

        private float TooCold = -1f;

        private float TooHot = -1f;

        private float BleedRate = -1f;

        public Vector3 TargetPos = Vector3.zero;

        private float DiseaseDisappearance = 1f;

        private float ApparelHealth = 1f;

        // public float Drunkness = 0f;
        private int BedStatus = -1;

        private int CabinFeverMoodLevel = 0;

        private int PainMoodLevel = 0;

        private float ToxicBuildUpVisible = 0;

        private MentalStateDef MentalSanity = null;

        private float HealthDisease = 1f;

        public MentalBreaker Mb = null;

        public Need_Mood Mood = null;
        private int drugDesire = 0;
        private bool traitsCheck = false;
        private bool isAddict = false;
        private ThoughtDef painThought;
        private bool withDrawal = false;
        private string drugUserLabel = null;
        private string addictionLabel = null;
        private int wantsToHump = -1;
        private int feelsNaked = -1;
        private int prostho = 0;
        private int prosthoUnhappy = 0;
        private int nightOwlUnhappy = -1;
        private bool hasGreedyTrait = false;
        private int greedyThought = -1;
        private bool hasJealousTrait = false;
        private int jealousThought = -1;
        private int unburied = -1;

        public bool isPacifist = false;
        private bool isPyromaniac = false;
        private bool isMasochist = false;
        private float withDrawalPercent = 0f;
        private float pawnHealth = 1f;
        private float severity;
        private float immunity;
        private string humpTip;
        private string sickTip;
        private string painTip;
        private string cabinFeverTip;
        private string nakedTip;
        private string nightOwlTip;
        private bool isNightOwl = false;
        private string unburiedTip;

        private string BedStatusTip;
        public Color BGColor = Color.gray;

        public int thisColCount = 2;

        private string toxicTip;

        private string healthTip;

        private int LastStatUpdate = 0;

        private float sickMoodOffset;

        private float unburiedMoodOffset;

        private float nightOwlMoodOffset;

        private float nakedMoodOffset;

        private float BedStatusMoodOffset;

        private float humpMoodOffset;

        private float CabinFeverMoodOffset;

        private float PainMoodOffset;

        private float prosthoMoodOffset;

        public int prosthoStage;

        private string prosthoTooltip;

        private float jealousMoodOffset;

        private float greedyMoodOffset;

        private string greedyTooltip;

        private string jealousTooltip;

        public bool hasRelationWithColonist = false;

        public bool relationChecked = false;

        public int SpawnedAt;

        private List<IconEntryBar> _barIconList = new List<IconEntryBar>();

        private List<IconEntryPSI> _psiIconList = new List<IconEntryPSI>();


        private static PawnCapacityDef[] array;

        public List<IconEntryBar> BarIconList => this._barIconList;

        public List<IconEntryPSI> PSIIconList => this._psiIconList;

        public override void CompTick()
        {
            base.CompTick();
            if (pawn.IsColonist || pawn.IsPrisoner)
            {
                if (this.entriesDirty)
                {
                    this.UpdateColonistStats();
                    this.entriesDirty = false;
                }
                this.CheckStats();
            }


        }

        public override void PostDraw()
        {
            // TODO: make this one draw the icons on colonists
            // deactivated for now
            base.PostDraw();
            return;

            if (pawn.Dead || !pawn.Spawned || pawn.holdingOwner == null || pawn.Map == null)
            {
                return;
            }

            SettingsPSI psiSettings = PsiSettings;
            float viewOpacity = psiSettings.IconOpacity;

            int iconNum = 0;

            Vector3 bodyLoc = pawn.DrawPos;

            // // Target Point
            // if (psiSettings.ShowTargetPoint && TargetPos != Vector3.zero)
            // {
            //     if (psiSettings.UseColoredTarget)
            //     {
            //         Color skinColor = pawn.story.SkinColor;
            //         Color hairColor = pawn.story.hairColor;
            //
            //         if (skinMat == null)
            //         {
            //             return;
            //         }
            //
            //         if (hairMat == null)
            //         {
            //             return;
            //         }
            //
            //         DrawIcon_posOffset(TargetPos, Vector3.zero, skinMat, skinColor, 1f);
            //         DrawIcon_posOffset(TargetPos, Vector3.zero, hairMat, hairColor, 1f);
            //     }
            //     else
            //     {
            //         if (targetMat == null)
            //         {
            //             return;
            //         }
            //
            //         DrawIcon_posOffset(TargetPos, Vector3.zero, targetMat, ColorNeutralStatus, viewOpacity);
            //     }
            // }

            // Drafted
            if (psiSettings.ShowDraft && pawn.Drafted)
            {
                if (isPacifist)
                {
                    DrawIconOnColonist(bodyLoc, new IconEntryPSI(Icon.Pacific, ColYellow, ViewOpacityCrit), iconNum);
                }
                else
                {
                    DrawIconOnColonist(bodyLoc, new IconEntryPSI(Icon.Draft, ColVermillion, ViewOpacityCrit), iconNum);
                }
                iconNum++;
            }

            List<IconEntryPSI> drawIconEntries = PSIIconList;
            if (!drawIconEntries.NullOrEmpty())
            {
                for (int index = 0; index < drawIconEntries.Count; index++)
                {
                    IconEntryPSI iconEntryBar = drawIconEntries[index];
                    DrawIconOnColonist(bodyLoc, iconEntryBar, index + iconNum);
                }
            }

        }

        private static void DrawIconOnColonist(Vector3 bodyPos, IconEntryPSI entryPSI, int entryCount)
        {
            Material material = GameComponentPSI.PSIMaterials[entryPSI.icon];
            if (material == null)
            {
                Debug.LogError("Material = null.");
                return;
            }

            Vector3 posOffset = GameComponentPSI.IconPosVectorsPSI[entryCount];

            entryPSI.color.a = entryPSI.opacity;
            material.color = entryPSI.color;
            Color guiColor = GUI.color;
            GUI.color = entryPSI.color;
            Vector2 vectorAtBody;

            float worldScale = GameComponentPSI.WorldScale;
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

        private bool entriesDirty = true;

        private int NextStatUpdate = 1;

        public void CheckStats()
        {
            // if (Find.TickManager.CurTimeSpeed == TimeSpeed.Paused)
            // {
            //     return;
            // }

            int nextUpdate = (int)(this.LastStatUpdate + (this.NextStatUpdate * Find.TickManager.TickRateMultiplier));

            if (Find.TickManager.TicksGame > nextUpdate)
            {
                this.UpdateColonistStats();

                this.LastStatUpdate = Find.TickManager.TicksGame;
                this.NextStatUpdate = (int)Rand.Range(120f, 300f);
                // Log.Message(
                // "CBKF updated stat " + (this.parent as Pawn).Name + ", next update in " + NextStatUpdate * Find.TickManager.TickRateMultiplier
                // + " ticks.");
            }
        }

        public void CheckRelationWithColonists(Pawn pawn)
        {
            bool skip = false;

            if (pawn.relations.RelatedPawns.Any(x => x.IsColonist))
            {
                this.hasRelationWithColonist = true;
                skip = true;

            }

            if (!skip)
            {
                if (pawn.relations.DirectRelations.Any(x => x.otherPawn.IsColonist))
                {
                    this.hasRelationWithColonist = true;
                }
            }

            this.relationChecked = true;

        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref this.relationChecked, "relationChecked");
        }

        private void UpdateColonistStats()
        {
            List<IconEntryPSI> psiIconList = new List<IconEntryPSI>();
            List<IconEntryBar> barIconList = new List<IconEntryBar>();
            SettingsColonistBar barSettings = ColBarSettings;
            SettingsPSI psiSettings = PsiSettings;

            if (barSettings == null || psiSettings == null)
            {
                GameComponentPSI.Reinit(true, false, false);
                return;
            }

            if (!pawn.Spawned || pawn.Map == null)
            {
                return;
            }

            if (Current.ProgramState != ProgramState.Playing)
            {
                return;
            }

            float viewOpacity = psiSettings.IconOpacity;
            float viewOpacityCrit = ViewOpacityCrit;

            List<Thought> thoughts = new List<Thought>();

            pawn.needs?.mood?.thoughts?.GetDistinctMoodThoughtGroups(thoughts);
            this.pawnHealth = 1f - pawn.health.summaryHealth.SummaryHealthPercent;
            //   Log.Message(pawn + " health: " + this.pawnHealth);
            // Idle - Colonist icon only
            if (pawn.mindState.IsIdle)
            {
                if (psiSettings.ShowIdle && GenDate.DaysPassed >= 0.1)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Idle, ColorNeutralStatus, viewOpacity));
                }
            }

            if (!this.traitsCheck)
            {
                this.CheckTraits(pawn);
            }

            if (this.isPacifist)
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
                    psiIconList.Add(new IconEntryPSI(Icon.Pacific, ColBlueishGreen, viewOpacity));
                }
            }

            if (pawn.equipment.Primary == null && !pawn.IsPrisoner && !this.isPacifist)
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
                    psiIconList.Add(new IconEntryPSI(Icon.Unarmed, ColorNeutralStatusOpaque, viewOpacity));
                }
            }

            // Trait Pyromaniac
            if (this.isPyromaniac)
            {
                if (barSettings.ShowPyromaniac)
                {
                    barIconList.Add(
                        new IconEntryBar(Icon.Pyromaniac, ColYellow, "PSI.Settings.Visibility.Pyromaniac".Translate()));
                }

                if (psiSettings.ShowPyromaniac)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Pyromaniac, ColYellow, viewOpacityCrit));
                }
            }

            // efficiency
            float efficiency = psiSettings.LimitEfficiencyLess;
            float efficiencyTotal = 1f;
            string efficiencyTip = null;
            bool flag2 = false;
            array = PSI.GameComponentPSI._pawnCapacities;
            foreach (PawnCapacityDef pawnCapacityDef in array)
            {
                {
                    // if (pawnCapacityDef != PawnCapacityDefOf.Consciousness)
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
                        flag2 = true;
                    }
                }
            }

            if (flag2)
            {
                if (barSettings.ShowEffectiveness)
                {
                    barIconList.Add(
                        new IconEntryBar(
                            Icon.Effectiveness,
                            gradientRedAlertToNeutral.Evaluate(
                                efficiencyTotal / PsiSettings.LimitEfficiencyLess),
                            efficiencyTip));
                }

                if (psiSettings.ShowEffectiveness)
                {
                    psiIconList.Add(
                        new IconEntryPSI(
                            Icon.Effectiveness,
                            gradientRedAlertToNeutral.Evaluate(
                                efficiencyTotal / PsiSettings.LimitEfficiencyLess),
                            viewOpacityCrit));
                }
            }

            // target
            this.TargetPos = Vector3.zero;

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
                    flag = false;
                }
                else
                {
                    //        this.TargetPos = pawn.Position.ToVector3Shifted();
                    flag = true;
                }

                if (!flag)
                {
                    Vector3 a = targetInfo.Cell.ToVector3Shifted();
                    this.TargetPos = a + new Vector3(0f, 3f, 0f);
                }
            }

            // temperature
            float temperatureForCell = GenTemperature.GetTemperatureForCell(pawn.Position, pawn.Map);

            this.TooCold = (float)((pawn.ComfortableTemperatureRange().min
                                    - (double)PsiSettings.LimitTempComfortOffset - temperatureForCell) / 10f);

            this.TooHot = (float)((temperatureForCell - (double)pawn.ComfortableTemperatureRange().max
                                   - PsiSettings.LimitTempComfortOffset) / 10f);

            this.TooCold = Mathf.Clamp(this.TooCold, 0f, 2f);

            this.TooHot = Mathf.Clamp(this.TooHot, 0f, 2f);

            // Too Cold & too hot
            if (this.TooCold > 0f)
            {
                if (barSettings.ShowTooCold)
                {
                    barIconList.Add(
                        new IconEntryBar(Icon.TooCold, gradient4.Evaluate(this.TooCold / 2), "TooCold".Translate()));
                }

                if (psiSettings.ShowTooCold)
                {
                    psiIconList.Add(
                        new IconEntryPSI(Icon.TooCold, gradient4.Evaluate(this.TooCold / 2), viewOpacityCrit));
                }
            }

            if (this.TooHot > 0f)
            {
                if (barSettings.ShowTooHot)
                {
                    barIconList.Add(
                        new IconEntryBar(Icon.TooCold, gradient4.Evaluate(this.TooHot / 2), "TooHot".Translate()));
                }

                if (psiSettings.ShowTooHot)
                {
                    psiIconList.Add(
                        new IconEntryPSI(Icon.TooCold, gradient4.Evaluate(this.TooHot / 2), viewOpacityCrit));
                }
            }

            // Mental Sanity
            this.MentalSanity = null;
            if (pawn.mindState != null && pawn.InMentalState)
            {
                this.MentalSanity = pawn.MentalStateDef;
            }

            if (this.MentalSanity != null)
            {
                if (pawn.InAggroMentalState)
                {
                    if (barSettings.ShowAggressive)
                    {
                        barIconList.Add(new IconEntryBar(Icon.Aggressive, ColVermillion, null));
                    }

                    if (psiSettings.ShowAggressive)
                    {
                        psiIconList.Add(new IconEntryPSI(Icon.Aggressive, ColVermillion, viewOpacityCrit));
                    }

                    // Give Up Exit
                    if (barSettings.ShowLeave)
                    {
                        if (this.MentalSanity == MentalStateDefOf.PanicFlee)
                        {
                            barIconList.Add(new IconEntryBar(Icon.Leave, ColVermillion, null));
                        }
                    }

                    if (psiSettings.ShowLeave)
                    {
                        if (this.MentalSanity == MentalStateDefOf.PanicFlee)
                        {
                            psiIconList.Add(new IconEntryPSI(Icon.Leave, ColVermillion, viewOpacityCrit));
                        }
                    }

                    // Daze Wander
                    if (this.MentalSanity == MentalStateDefOf.WanderSad)
                    {
                        if (barSettings.ShowDazed)
                        {
                            // + MentalStateDefOf.WanderPsychotic
                            barIconList.Add(new IconEntryBar(Icon.Dazed, ColYellow, null));
                        }

                        if (psiSettings.ShowDazed)
                        {
                            // + MentalStateDefOf.WanderPsychotic
                            psiIconList.Add(new IconEntryPSI(Icon.Dazed, ColYellow, viewOpacityCrit));
                        }
                    }

                    // PanicFlee
                    if (this.MentalSanity == MentalStateDefOf.PanicFlee)
                    {
                        if (barSettings.ShowPanic)
                        {
                            barIconList.Add(new IconEntryBar(Icon.Panic, ColYellow, null));
                        }

                        if (psiSettings.ShowPanic)
                        {
                            psiIconList.Add(new IconEntryPSI(Icon.Panic, ColYellow, viewOpacityCrit));
                        }
                    }
                }
            }

            // Hungry
            if (pawn.needs.food.CurLevel < (double)PsiSettings.LimitFoodLess)
            {
                if (barSettings.ShowHungry)
                {
                    string tooltip = pawn.needs.food.GetTipString();

                    barIconList.Add(
                        new IconEntryBar(
                            Icon.Hungry,
                            gradientRedAlertToNeutral.Evaluate(
                                pawn.needs.food.CurLevel / PsiSettings.LimitFoodLess),
                            tooltip));
                }

                if (psiSettings.ShowHungry)
                {
                    string tooltip = pawn.needs.food.GetTipString();

                    psiIconList.Add(
                        new IconEntryPSI(
                            Icon.Hungry,
                            gradientRedAlertToNeutral.Evaluate(
                                pawn.needs.food.CurLevel / PsiSettings.LimitFoodLess),
                            viewOpacityCrit));
                }
            }

            // Tired
            if (pawn.needs.rest.CurLevel < (double)PsiSettings.LimitRestLess)
            {
                if (barSettings.ShowTired)
                {
                    string tooltip = pawn.needs.rest.CurCategory.GetLabel();
                    barIconList.Add(
                        new IconEntryBar(
                            Icon.Tired,
                            gradientRedAlertToNeutral.Evaluate(
                                pawn.needs.rest.CurLevel / PsiSettings.LimitRestLess),
                            tooltip));
                }

                if (psiSettings.ShowTired)
                {
                    string tooltip = pawn.needs.rest.CurCategory.GetLabel();
                    psiIconList.Add(
                        new IconEntryPSI(
                            Icon.Tired,
                            gradientRedAlertToNeutral.Evaluate(
                                pawn.needs.rest.CurLevel / PsiSettings.LimitRestLess),
                            viewOpacityCrit));
                }
            }

            // Mental Breaker for MoodBars
            if (pawn.needs?.mood != null)
            {
                this.Mb = pawn.mindState.mentalBreaker;
                this.Mood = pawn.needs.mood;
            }
            else
            {
                this.Mood = null;
                this.Mb = null;
            }

            // Health Calc
            this.DiseaseDisappearance = 1f;
            this.HealthDisease = 1f;

            // Drug addiction
            List<Hediff> hediffs = null;

            // Sick thoughts
            if (pawn.health?.hediffSet != null)
            {
                hediffs = pawn.health.hediffSet.hediffs;

                // Health
                // Infection

                // Bleed rate
                this.BleedRate = Mathf.Clamp01(
                    pawn.health.hediffSet.BleedRateTotal * PsiSettings.LimitBleedMult);

                if (this.BleedRate > 0.0f)
                {
                    if (barSettings.ShowBloodloss)
                    {
                        string tooltip = "BleedingRate".Translate() + ": "
                                         + pawn.health.hediffSet.BleedRateTotal.ToStringPercent() + "/d";

                        barIconList.Add(
                            new IconEntryBar(
                                Icon.Bloodloss,
                                gradientRedAlertToNeutral.Evaluate(1.0f - this.BleedRate),
                                tooltip));
                    }
                    if (psiSettings.ShowBloodloss)
                    {

                        psiIconList.Add(
                            new IconEntryPSI(
                                Icon.Bloodloss,
                                gradientRedAlertToNeutral.Evaluate(1.0f - this.BleedRate),
                                viewOpacityCrit));
                    }
                }

                if (pawn.Map != null)
                {
                    if (HealthAIUtility.ShouldBeTendedNowUrgent(pawn))
                    {
                        if (barSettings.ShowMedicalAttention)
                        {
                            barIconList.Add(
                                new IconEntryBar(
                                    Icon.MedicalAttention,
                                    ColVermillion,
                                    "NeedsTendingNow".Translate()));
                        }

                        if (psiSettings.ShowMedicalAttention)
                        {
                            psiIconList.Add(new IconEntryPSI(Icon.MedicalAttention, ColVermillion, viewOpacityCrit));
                        }
                    }
                    else if (HealthAIUtility.ShouldBeTendedNow(pawn))
                    {
                        if (barSettings.ShowMedicalAttention)
                        {
                            barIconList.Add(
                                new IconEntryBar(Icon.MedicalAttention, ColYellow, "NeedsTendingNow".Translate()));
                        }

                        if (psiSettings.ShowMedicalAttention)
                        {
                            psiIconList.Add(new IconEntryPSI(Icon.MedicalAttention, ColYellow, viewOpacityCrit));
                        }
                    }

                    if (HealthAIUtility.ShouldHaveSurgeryDoneNow(pawn))
                    {
                        if (barSettings.ShowMedicalAttention)
                        {
                            barIconList.Add(
                                new IconEntryBar(
                                    Icon.MedicalAttention,
                                    ColBlueishGreen,
                                    "ShouldHaveSurgeryDoneNow".Translate()));
                        }

                        if (psiSettings.ShowMedicalAttention)
                        {
                            psiIconList.Add(new IconEntryPSI(Icon.MedicalAttention, ColBlueishGreen, viewOpacityCrit));
                        }
                    }
                }
            }

            if (pawn.health.hediffSet.AnyHediffMakesSickThought && !pawn.Destroyed && pawn.playerSettings.medCare >= 0)
            {
                if (hediffs != null)
                {
                    this.severity = 0f;
                    this.immunity = 0f;
                    foreach (Hediff hediff in hediffs)
                    {
                        if (!hediff.Visible || hediff.IsOld() || !hediff.def.makesSickThought
                            || hediff.LabelCap.NullOrEmpty() || hediff.SeverityLabel.NullOrEmpty())
                        {
                            continue;
                        }

                        this.ToxicBuildUpVisible = 0;
                        this.healthTip = hediff.LabelCap;
                        if (!thoughts.NullOrEmpty())
                        {
                            GetThought(
                                thoughts,
                                ThoughtDef.Named("Sick"),
                                out int dummy,
                                out this.sickTip,
                                out this.sickMoodOffset);
                        }

                        // this.ToxicBuildUpVisible
                        if (hediff.def == HediffDefOf.ToxicBuildup)
                        {
                            this.toxicTip = hediff.LabelCap + "\n" + hediff.SeverityLabel;
                            this.ToxicBuildUpVisible = Mathf.InverseLerp(0.049f, 1f, hediff.Severity);
                            continue;
                        }

                        HediffComp_Immunizable compImmunizable = hediff.TryGetComp<HediffComp_Immunizable>();
                        if (compImmunizable != null)
                        {
                            this.severity = Mathf.Max(this.severity, hediff.Severity);
                            this.immunity = compImmunizable.Immunity;
                            float basehealth = this.HealthDisease - (this.severity - this.immunity / 4) - 0.25f;
                            this.HealthDisease = basehealth;
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

                        if (this.DiseaseDisappearance > compImmunizable.Immunity)
                        {
                            this.DiseaseDisappearance = compImmunizable.Immunity;
                        }

                        // break;
                    }
                }

                if (this.DiseaseDisappearance < PsiSettings.LimitDiseaseLess)
                {
                    string tooltip = this.sickTip + "\n" + this.healthTip + "\n" + "Immunity".Translate() + " / "
                                     + "PSI.DiseaseProgress".Translate() + ": \n" + this.immunity.ToStringPercent()
                                     + " / " + this.severity.ToStringPercent() + ": \n" + this.sickMoodOffset;

                    if (barSettings.ShowHealth)
                    {
                        // Regular Sickness
                        barIconList.Add(
                            new IconEntryBar(
                                Icon.Health,
                                gradient4.Evaluate(this.DiseaseDisappearance / psiSettings.LimitDiseaseLess),
                                tooltip));
                    }

                    if (psiSettings.ShowHealth)
                    {
                        // Regular Sickness
                        psiIconList.Add(
                            new IconEntryPSI(
                                Icon.Health,
                                gradient4.Evaluate(this.DiseaseDisappearance / psiSettings.LimitDiseaseLess),
                                viewOpacityCrit));
                    }
                }
            }
            else if (pawn.health.summaryHealth.SummaryHealthPercent < 1f)
            {
                string tooltip = "Health".Translate() + ": "
                                 + pawn.health.summaryHealth.SummaryHealthPercent.ToStringPercent();
                if (barSettings.ShowHealth)
                {
                    barIconList.Add(new IconEntryBar(Icon.Health, gradient4.Evaluate(this.pawnHealth), tooltip));
                }

                if (psiSettings.ShowHealth)
                {
                    psiIconList.Add(
                        new IconEntryPSI(Icon.Health, gradient4.Evaluate(this.pawnHealth), viewOpacityCrit));
                }
            }

            // Toxicity buildup
            if (this.ToxicBuildUpVisible > 0f)
            {
                if (barSettings.ShowToxicity)
                {
                    string tooltip = this.toxicTip;

                    barIconList.Add(
                        new IconEntryBar(Icon.Toxicity, gradient4.Evaluate(this.ToxicBuildUpVisible), tooltip));
                }

                if (psiSettings.ShowToxicity)
                {
                    string tooltip = this.toxicTip;

                    psiIconList.Add(
                        new IconEntryPSI(Icon.Toxicity, gradient4.Evaluate(this.ToxicBuildUpVisible), viewOpacityCrit));
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

            this.ApparelHealth = worstApparel;

            // Apparel
            if (this.ApparelHealth < (double)PsiSettings.LimitApparelHealthLess)
            {
                if (barSettings.ShowApparelHealth)
                {
                    double pawnApparelHealth = this.ApparelHealth / (double)psiSettings.LimitApparelHealthLess;
                    barIconList.Add(
                        new IconEntryBar(
                            Icon.ApparelHealth,
                            gradientRedAlertToNeutral.Evaluate((float)pawnApparelHealth),
                            null));
                }

                if (psiSettings.ShowApparelHealth)
                {
                    double pawnApparelHealth = this.ApparelHealth / (double)psiSettings.LimitApparelHealthLess;
                    psiIconList.Add(
                        new IconEntryPSI(
                            Icon.ApparelHealth,
                            gradientRedAlertToNeutral.Evaluate((float)pawnApparelHealth),
                            viewOpacity));
                }
            }

            if (!thoughts.NullOrEmpty())
            {
                if (this.prostho != 0)
                {
                    switch (this.prostho)
                    {
                        case -1:
                            GetThought(
                                thoughts,
                                ThoughtDef.Named("ProsthophobeUnhappy"),
                                out this.prosthoUnhappy,
                                out this.prosthoTooltip,
                                out this.prosthoMoodOffset);
                            break;

                        case 1:
                            GetThought(
                                thoughts,
                                ThoughtDef.Named("ProsthophileNoProsthetic"),
                                out this.prosthoUnhappy,
                                out this.prosthoTooltip,
                                out this.prosthoMoodOffset);
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
                        out this.BedStatus,
                        out this.BedStatusTip,
                        out this.BedStatusMoodOffset);
                }
                else
                {
                    this.BedStatus = 1;
                    this.BedStatusTip = "NeedColonistBeds".Translate();
                }

                // Humping
                GetThought(
                    thoughts,
                    ThoughtDef.Named("WantToSleepWithSpouseOrLover"),
                    out this.wantsToHump,
                    out this.humpTip,
                    out this.humpMoodOffset);

                // Cabin Fever
                if (GetThought(
                    thoughts,
                    ThoughtDef.Named("CabinFever"),
                    out this.CabinFeverMoodLevel,
                    out this.cabinFeverTip,
                    out this.CabinFeverMoodOffset))
                {
                    if (barSettings.ShowCabinFever)
                    {
                        string tooltip = this.cabinFeverTip;
                        barIconList.Add(
                            new IconEntryBar(Icon.CabinFever, EvaluateMoodOffset(this.CabinFeverMoodOffset), tooltip));
                    }

                    if (psiSettings.ShowCabinFever)
                    {
                        psiIconList.Add(
                            new IconEntryPSI(Icon.CabinFever, EvaluateMoodOffset(this.CabinFeverMoodOffset), viewOpacityCrit));
                    }
                }

                // Pain
                if (GetThought(
                    thoughts,
                    this.painThought,
                    out this.PainMoodLevel,
                    out this.painTip,
                    out this.PainMoodOffset))
                {
                    if (barSettings.ShowPain)
                    {
                        barIconList.Add(new IconEntryBar(Icon.Pain, EvaluateMoodOffset(this.PainMoodOffset), this.painTip));
                    }

                    if (psiSettings.ShowPain)
                    {
                        psiIconList.Add(new IconEntryPSI(Icon.Pain, EvaluateMoodOffset(this.PainMoodOffset), viewOpacityCrit));
                    }
                }

                // Naked
                if (GetThought(
                    thoughts,
                    ThoughtDefOf.Naked,
                    out this.feelsNaked,
                    out this.nakedTip,
                    out this.nakedMoodOffset))
                {
                    if (barSettings.ShowNaked)
                    {
                        barIconList.Add(new IconEntryBar(Icon.Naked, EvaluateMoodOffset(this.nakedMoodOffset), this.nakedTip));
                    }

                    if (psiSettings.ShowNaked)
                    {
                        psiIconList.Add(new IconEntryPSI(Icon.Naked, EvaluateMoodOffset(this.nakedMoodOffset), viewOpacity));
                    }
                }

                // Night Owl
                if (this.isNightOwl)
                {
                    if (GetThought(
                        thoughts,
                        ThoughtDef.Named("NightOwlDuringTheDay"),
                        out this.nightOwlUnhappy,
                        out this.nightOwlTip,
                        out this.nightOwlMoodOffset))
                    {
                        if (barSettings.ShowNightOwl)
                        {
                            barIconList.Add(
                                new IconEntryBar(Icon.NightOwl, EvaluateMoodOffset(this.nightOwlMoodOffset), this.nightOwlTip));
                        }

                        if (psiSettings.ShowNightOwl)
                        {
                            psiIconList.Add(
                                new IconEntryPSI(Icon.NightOwl, EvaluateMoodOffset(this.nightOwlMoodOffset), viewOpacityCrit));
                        }
                    }
                }

                // Greedy
                if (this.hasGreedyTrait)
                {
                    if (GetThought(
                        thoughts,
                        ThoughtDef.Named("Greedy"),
                        out this.greedyThought,
                        out this.greedyTooltip,
                        out this.greedyMoodOffset))
                    {
                        if (barSettings.ShowGreedy)
                        {
                            barIconList.Add(
                                new IconEntryBar(Icon.Greedy, EvaluateMoodOffset(this.greedyMoodOffset), this.greedyTooltip));
                        }

                        if (psiSettings.ShowGreedy)
                        {
                            psiIconList.Add(
                                new IconEntryPSI(Icon.Greedy, EvaluateMoodOffset(this.greedyMoodOffset), viewOpacity));
                        }
                    }
                }

                // Jealous
                if (this.hasJealousTrait)
                {
                    if (GetThought(
                        thoughts,
                        ThoughtDef.Named("Jealous"),
                        out this.jealousThought,
                        out this.jealousTooltip,
                        out this.jealousMoodOffset))
                    {
                        if (barSettings.ShowJealous)
                        {
                            barIconList.Add(
                                new IconEntryBar(Icon.Jealous, EvaluateMoodOffset(this.jealousMoodOffset), this.jealousTooltip));
                        }

                        if (psiSettings.ShowJealous)
                        {
                            psiIconList.Add(
                                new IconEntryPSI(Icon.Jealous, EvaluateMoodOffset(this.jealousMoodOffset), viewOpacity));
                        }
                    }
                }

                // Unburied
                if (GetThought(
                    thoughts,
                    ThoughtDef.Named("ColonistLeftUnburied"),
                    out this.unburied,
                    out this.unburiedTip,
                    out this.unburiedMoodOffset))
                {
                    if (barSettings.ShowLeftUnburied)
                    {
                        string tooltip = this.unburiedTip;
                        barIconList.Add(
                            new IconEntryBar(Icon.LeftUnburied, EvaluateMoodOffset(this.unburiedMoodOffset), tooltip));
                    }

                    if (psiSettings.ShowLeftUnburied)
                    {
                        string tooltip = this.unburiedTip;
                        psiIconList.Add(
                            new IconEntryPSI(Icon.LeftUnburied, EvaluateMoodOffset(this.unburiedMoodOffset), viewOpacity));
                    }
                }
            }

            this.isAddict = false;
            this.withDrawal = false;
            this.withDrawalPercent = 0f;
            this.addictionLabel = null;
            if (hediffs != null)
            {
                foreach (Hediff hediff in hediffs)
                {
                    if (hediff is Hediff_Addiction)
                    {
                        this.isAddict = true;
                        this.withDrawalPercent = hediff.Severity;
                        this.withDrawal = hediff.CurStageIndex > 0;
                        if (this.addictionLabel.NullOrEmpty())
                        {
                            this.addictionLabel = hediff.LabelCap;
                        }
                        else
                        {
                            this.addictionLabel += "\n" + hediff.LabelCap;
                        }
                    }
                }
            }

            if (this.isAddict || this.drugDesire != 0)
            {
                Color color = new Color();
                string tooltip = null;
                if (this.isAddict)
                {
                    if (this.withDrawal)
                    {
                        this.GetWithdrawalColor(out color);
                    }
                    else
                    {
                        color = ColVermillion;
                    }

                    if (!this.drugUserLabel.NullOrEmpty())
                    {
                        tooltip = this.drugUserLabel + "\n" + this.addictionLabel;
                    }
                    else
                    {
                        tooltip = this.addictionLabel;
                    }
                }
                else
                {
                    switch (this.drugDesire)
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

                    tooltip = this.drugUserLabel;
                }

                if (barSettings.ShowDrunk)
                {
                    barIconList.Add(new IconEntryBar(Icon.Drunk, color, tooltip));
                }

                if (psiSettings.ShowDrunk)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Drunk, color, viewOpacityCrit));
                }
            }

            // Bed status
            if (this.wantsToHump > -1)
            {
                if (barSettings.ShowBedroom)
                {
                    barIconList.Add(new IconEntryBar(Icon.Bedroom, EvaluateMoodOffset(this.humpMoodOffset), this.humpTip));
                }

                if (psiSettings.ShowBedroom)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Bedroom, EvaluateMoodOffset(this.humpMoodOffset), viewOpacityCrit));
                }
            }
            else if (this.BedStatus > -1)
            {
                if (barSettings.ShowBedroom)
                {
                    barIconList.Add(
                        new IconEntryBar(Icon.Bedroom, EvaluateMoodOffset(this.BedStatusMoodOffset), this.BedStatusTip));
                }

                if (psiSettings.ShowBedroom)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Bedroom, EvaluateMoodOffset(this.BedStatusMoodOffset), viewOpacity));
                }

                // Moods caused by traits
                if (this.prosthoUnhappy > -1)
                {
                    if (this.prostho == 1)
                    {
                        if (barSettings.ShowProsthophile)
                        {
                            barIconList.Add(
                                new IconEntryBar(
                                    Icon.Prosthophile,
                                    EvaluateMoodOffset(this.prosthoMoodOffset),
                                    this.prosthoTooltip));
                        }

                        if (psiSettings.ShowProsthophile)
                        {
                            psiIconList.Add(
                                new IconEntryPSI(Icon.Prosthophile, EvaluateMoodOffset(this.prosthoMoodOffset), viewOpacity));
                        }
                    }

                    if (this.prostho == -1)
                    {
                        if (barSettings.ShowProsthophobe)
                        {
                            barIconList.Add(
                                new IconEntryBar(
                                    Icon.Prosthophobe,
                                    EvaluateMoodOffset(this.prosthoMoodOffset),
                                    this.prosthoTooltip));
                        }

                        if (psiSettings.ShowProsthophobe)
                        {
                            psiIconList.Add(
                                new IconEntryPSI(Icon.Prosthophobe, EvaluateMoodOffset(this.prosthoMoodOffset), viewOpacity));
                        }
                    }
                }
            }
            // Log.Message(this.PSIPawn.LabelShort + " icons to be displayed: ");
            // foreach (IconEntryBar entry in barIconList)
            // {
            //     Log.Message(entry.icon + " - " + entry.tooltip);
            // }
            this._barIconList = barIconList.OrderBy(x => x.icon).ToList();
            this._psiIconList = psiIconList.OrderBy(x => x.icon).ToList();
        }

        private void CheckTraits(Pawn pawn)
        {
            {
                // One time traits check
                if (pawn.story?.traits != null)
                {
                    if (pawn.RaceProps.hasGenders)
                    {
                        switch (pawn.gender)
                        {
                            case Gender.Male:
                                this.BGColor = MaleColor;
                                break;

                            case Gender.Female:
                                this.BGColor = FemaleColor;
                                break;

                            default: break;
                        }
                    }

                    // Masochist
                    this.isMasochist = pawn.story.traits.HasTrait(TraitDef.Named("Masochist"));

                    // Masochist trait check
                    this.painThought = ThoughtDef.Named(
                        pawn.story.traits.HasTrait(TraitDef.Named("Masochist")) ? "MasochistPain" : "Pain");

                    // Pacifist
                    this.isPacifist = pawn.story.WorkTagIsDisabled(WorkTags.Violent);

                    // Pyromaniac
                    this.isPyromaniac = pawn.story.traits.HasTrait(TraitDefOf.Pyromaniac);

                    // Prostho
                    if (pawn.story.traits.HasTrait(TraitDefOf.Prosthophobe))
                    {
                        this.prostho = -1;
                    }

                    if (pawn.story.traits.HasTrait(TraitDef.Named("Prosthophile")))
                    {
                        this.prostho = 1;
                    }

                    // Night Owl
                    if (pawn.story.traits.HasTrait(TraitDef.Named("NightOwl")))
                    {
                        this.isNightOwl = true;
                    }

                    // Jealous
                    if (pawn.story.traits.HasTrait(TraitDef.Named("Jealous")))
                    {
                        this.hasJealousTrait = true;
                    }

                    // Drug desire
                    if (pawn.story.traits.HasTrait(TraitDefOf.DrugDesire))
                    {
                        this.drugDesire = pawn.story.traits.DegreeOfTrait(TraitDefOf.DrugDesire);
                        this.drugUserLabel = pawn.story.traits.GetTrait(TraitDefOf.DrugDesire).LabelCap;
                    }

                    // Greedy
                    if (pawn.story.traits.HasTrait(TraitDefOf.Greedy))
                    {
                        this.hasGreedyTrait = true;
                    }

                    this.traitsCheck = true;
                }
            }
        }

        private void GetWithdrawalColor(out Color color)
        {
            color = Color.Lerp(ColBlueishGreen, ColVermillion, this.withDrawalPercent);
        }

        public Pawn pawn
        {
            get
            {
                if (_psiPawn == null)
                {
                    this._psiPawn = this.parent as Pawn;
                }
                return this._psiPawn;
            }
        }

        private Pawn _psiPawn;
        /*
        public override void PostDraw()
        {
            base.PostDraw();

            SettingsPSI psiSettings = PsiSettings;
            float viewOpacity = psiSettings.IconOpacity;

            int iconNum = 0;

            Vector3 bodyLoc = pawn.DrawPos;

            // Target Point
            if (psiSettings.ShowTargetPoint && TargetPos != Vector3.zero)
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

                    DrawIcon_posOffset(TargetPos, Vector3.zero, skinMat, skinColor, 1f);
                    DrawIcon_posOffset(TargetPos, Vector3.zero, hairMat, hairColor, 1f);
                }
                else
                {
                    if (targetMat == null)
                    {
                        return;
                    }

                    DrawIcon_posOffset(TargetPos, Vector3.zero, targetMat, ColorNeutralStatus, viewOpacity);
                }
            }

            // Drafted
            if (psiSettings.ShowDraft && pawn.Drafted)
            {
                if (isPacifist)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icon.Pacific, ColYellow, ViewOpacityCrit);
                }
                else
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icon.Draft, ColVermillion, ViewOpacityCrit);
                }
            }

            List<IconEntryPSI> drawIconEntries = PSIIconList;
            if (!drawIconEntries.NullOrEmpty())
            {
                for (int index = 0; index < drawIconEntries.Count; index++)
                {
                    IconEntryPSI iconEntryBar = drawIconEntries[index];
                    DrawIconOnColonist(bodyLoc, iconEntryBar, index + iconNum);
                }
            }

        }
        */
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



        /*
        public override void PostDraw()
        {
            base.PostDraw();

            PawnStats pawnstats = this.PSIPawn.GetPawnStats();
            Vector3 bodyPos = this.PSIPawn.DrawPos;
            if (this.PSIIconList.NullOrEmpty())
            {
                return;
            }

            if (PSI.PSI.PSIMaterials[0].NullOrBad())
            {
                PSI.PSI.Reinit(false, true, false);
            }

            for (int index = 0; index < this.PSIIconList.Count; index++)
            {
                var entry = this.PSIIconList[index];
                Vector3 posOffset = PSI.PSI.IconPosVectorsPSI[index];
                Material material = PSI.PSI.PSIMaterials[entry.icon];

                float opacity = entry.opacity;

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

                Log.Message("Drawing at " + this.PSIPawn + " - " + iconRect);
            }
        }
        */
        public void SetEntriesDirty()
        {
            this.entriesDirty = true;
        }
    }
}
