namespace ColonistBarKF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ColonistBarKF.Bar;
    using ColonistBarKF.PSI;

    using JetBrains.Annotations;

    using RimWorld;

    using UnityEngine;

    using Verse;
    using Verse.AI;

    [StaticConstructorOnStartup]
    public class CompPSI : ThingComp
    {
        private static readonly List<Thought> thoughts = new List<Thought>();

        private static PawnCapacityDef[] array;

        public bool hasRelationWithColonist;

        public bool isPacifist;

        [CanBeNull]
        public MentalBreaker Mb;

        [CanBeNull]
        public Need_Mood Mood;

        public int prosthoStage;

        public Vector3 TargetPos = Vector3.zero;

        public int thisColCount = 2;

        public float TotalEfficiency = 1f;

        [CanBeNull]
        private string addictionLabel;

        // public float Drunkness = 0f;
        private int BedStatus = -1;

        private float BedStatusMoodOffset;

        [CanBeNull]
        private string BedStatusTip;

        private Color bgColor = Color.gray;

        private float BleedRate = -1f;

        private int CabinFeverMoodLevel;

        private float CabinFeverMoodOffset;

        [CanBeNull]
        private string cabinFeverTip;

        private float DiseaseDisappearance = 1f;

        private int drugDesire;

        [CanBeNull]
        private string drugUserLabel;

        private bool entriesDirty = true;

        private int feelsNaked = -1;

        private float greedyMoodOffset;

        private int greedyThought = -1;

        [CanBeNull]
        private string greedyTooltip;

        private bool hasGreedyTrait;

        private bool hasJealousTrait;

        private float HealthDisease = 1f;

        [CanBeNull]
        private string healthTip;

        private float humpMoodOffset;

        [CanBeNull]
        private string humpTip;

        private float immunity;

        private bool isAddict;

        private bool isMasochist;

        private bool isNightOwl;

        private bool isPyromaniac;

        private float jealousMoodOffset;

        private int jealousThought = -1;

        [CanBeNull]
        private string jealousTooltip;

        private int LastStatUpdate;

        [CanBeNull]
        private MentalStateDef MentalSanity;

        private float nakedMoodOffset;

        [CanBeNull]
        private string nakedTip;

        private int NextStatUpdate = 1;

        private float nightOwlMoodOffset;

        [CanBeNull]
        private string nightOwlTip;

        private int nightOwlUnhappy = -1;

        private int PainMoodLevel;

        private float PainMoodOffset;

        [CanBeNull]
        private ThoughtDef painThought;

        [CanBeNull]
        private string painTip;

        private int prostho;

        private float prosthoMoodOffset;

        [CanBeNull]
        private string prosthoTooltip;

        private int prosthoUnhappy;

        private float severity;

        private float sickMoodOffset;

        [CanBeNull]
        private string sickTip;

        private float TooCold = -1f;

        private float TooHot = -1f;

        private float ToxicBuildUpVisible;

        [CanBeNull]
        private string toxicTip;

        private bool traitsCheck;

        private int unburied = -1;

        private float unburiedMoodOffset;

        [CanBeNull]
        private string unburiedTip;

        private int wantsToHump = -1;

        private bool withDrawal;

        private float withDrawalPercent;

        public Pawn pawn => this.parent as Pawn;

        [NotNull]
        public List<IconEntryBar> BarIconList { get; private set; } = new List<IconEntryBar>();

        public Color BGColor
        {
            get => this.bgColor;

            set => this.bgColor = value;
        }

        [NotNull]
        public List<IconEntryPSI> PSIIconList { get; private set; } = new List<IconEntryPSI>();

        public override void CompTick()
        {
            base.CompTick();

            if (this.pawn.IsColonist || this.pawn.IsPrisoner)
            {
                if (this.entriesDirty)
                {
                    this.UpdateColonistStats();
                    this.entriesDirty = false;
                }

                this.CheckStats();
            }
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref this.bgColor, "bgColor");
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            Pawn pawn = this.parent as Pawn;
            if (pawn == null)
            {
                return;
            }

            if (pawn.Faction != Faction.OfPlayer)
            {
                this.CheckRelationWithColonists(pawn);
            }
        }

        public void SetEntriesDirty()
        {
            this.entriesDirty = true;
        }

        private static void AddIconsToList(
            bool addBarEntry,
            ref List<IconEntryBar> barList,
            bool addPSIEntry,
            ref List<IconEntryPSI> psiList,
            Icon icon,
            Color color,
            string tooltip)
        {
        }

        private static bool GetThought(ThoughtDef tdef, out int stage, out string tooltip, out float moodOffset)
        {
            tooltip = null;
            stage = -1;
            moodOffset = 0;

            if (thoughts.NullOrEmpty())
            {
                return false;
            }

            foreach (Thought thought in thoughts)
            {
                if (thought == null)
                {
                    continue;
                }

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
                if (!thought.LabelCap.NullOrEmpty())
                {
                    tooltip = thought.CurStage.description + "\n" + thought.LabelCap + "\n" + moodOffset;
                }

                return true;
            }

            return false;
        }

        private void CheckJobs()
        {
            this.TargetPos = Vector3.zero;
            Job curJob = this.pawn.CurJob;
            if (curJob != null)
            {
                JobDriver curDriver = this.pawn.jobs.curDriver;
                LocalTargetInfo targetInfo = curJob.targetA;
                if (curDriver is JobDriver_HaulToContainer || curDriver is JobDriver_HaulToCell
                    || curDriver is JobDriver_FoodDeliver || curDriver is JobDriver_FoodFeedPatient
                    || curDriver is JobDriver_TakeToBed || curDriver is JobDriver_TakeBeerOutOfFermentingBarrel)
                {
                    targetInfo = curJob.targetB;
                }

                if (curDriver != null && curDriver is JobDriver_DoBill)
                {
                    JobDriver_DoBill jobDriverDoBill = curDriver as JobDriver_DoBill;
                    if (jobDriverDoBill.workLeft >= 0.0)
                    {
                        targetInfo = curJob.targetA;
                    }
                    else if (jobDriverDoBill.workLeft <= 0.01f)
                    {
                        targetInfo = curJob.targetB;
                    }
                }

                if (curDriver is JobDriver_Hunt && this.pawn.carryTracker?.CarriedThing != null)
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

                if (curJob.def == JobDefOf.LayDown && this.pawn.InBed())
                {
                    targetInfo = null;
                }

                if (!curJob.playerForced && curJob.def == JobDefOf.Goto)
                {
                    targetInfo = null;
                }

                if (targetInfo != null)
                {
                    Vector3 a = targetInfo.Cell.ToVector3Shifted();
                    this.TargetPos = a + new Vector3(0f, 3f, 0f);
                }
            }
        }

        private void CheckRelationWithColonists(Pawn pawn)
        {
            bool skip = false;

            if (pawn.relations.FamilyByBlood.Any(x => x.Faction == Faction.OfPlayer))
            {
                this.hasRelationWithColonist = true;
                skip = true;
            }

            if (!skip)
            {
                if (pawn.relations.DirectRelations.Any(x => x.otherPawn.Faction == Faction.OfPlayer))
                {
                    this.hasRelationWithColonist = true;
                }
            }
        }

        private void CheckStats()
        {
            // if (Find.TickManager.CurTimeSpeed == TimeSpeed.Paused)
            // {
            // return;
            // }
            int nextUpdate = (int)(this.LastStatUpdate + this.NextStatUpdate * Find.TickManager.TickRateMultiplier);

            if (Find.TickManager.TicksGame <= nextUpdate)
            {
                return;
            }

            this.UpdateColonistStats();

            this.LastStatUpdate = Find.TickManager.TicksGame;
            this.NextStatUpdate = (int)Rand.Range(120f, 300f);

            // Log.Message(
            // "CBKF updated stat " + (this.parent as Pawn).Name + ", next update in " + NextStatUpdate * Find.TickManager.TickRateMultiplier
            // + " ticks.");
        }

        public void CheckTraits()
        {
            {
                if (this.pawn.RaceProps.hasGenders)
                {
                    if (this.pawn.Dead)
                    {
                        this.BGColor = Color.gray;
                    }
                    else
                    {
                        switch (this.pawn.gender)
                        {
                            case Gender.Male:
                                this.BGColor = Textures.MaleColor;
                                break;

                            case Gender.Female:
                                this.BGColor = Textures.FemaleColor;
                                break;
                        }
                    }
                }

                // One time traits check
                if (this.pawn.story?.traits != null)
                {
                    // Masochist
                    this.isMasochist = this.pawn.story.traits.HasTrait(TraitDef.Named("Masochist"));

                    // Masochist trait check
                    this.painThought = ThoughtDef.Named(this.pawn.story.traits.HasTrait(TraitDef.Named("Masochist")) ? "MasochistPain" : "Pain");

                    // Pacifist
                    this.isPacifist = this.pawn.story.WorkTagIsDisabled(WorkTags.Violent);

                    // Pyromaniac
                    this.isPyromaniac = this.pawn.story.traits.HasTrait(TraitDefOf.Pyromaniac);

                    // Prostho
                    if (this.pawn.story.traits.HasTrait(TraitDefOf.Prosthophobe))
                    {
                        this.prostho = -1;
                    }

                    if (this.pawn.story.traits.HasTrait(TraitDef.Named("Prosthophile")))
                    {
                        this.prostho = 1;
                    }

                    // Night Owl
                    if (this.pawn.story.traits.HasTrait(TraitDef.Named("NightOwl")))
                    {
                        this.isNightOwl = true;
                    }

                    // Jealous
                    if (this.pawn.story.traits.HasTrait(TraitDef.Named("Jealous")))
                    {
                        this.hasJealousTrait = true;
                    }

                    // Drug desire
                    if (this.pawn.story.traits.HasTrait(TraitDefOf.DrugDesire))
                    {
                        this.drugDesire = this.pawn.story.traits.DegreeOfTrait(TraitDefOf.DrugDesire);
                        this.drugUserLabel = this.pawn.story.traits.GetTrait(TraitDefOf.DrugDesire).LabelCap;
                    }

                    // Greedy
                    if (this.pawn.story.traits.HasTrait(TraitDefOf.Greedy))
                    {
                        this.hasGreedyTrait = true;
                    }

                    this.traitsCheck = true;
                }
            }
        }

        private void GetWithdrawalColor(out Color color)
        {
            color = Color.Lerp(Textures.ColBlueishGreen, Textures.ColVermillion, this.withDrawalPercent);
        }

        private void UpdateColonistStats()
        {
            List<IconEntryPSI> psiIconList = new List<IconEntryPSI>();
            List<IconEntryBar> barIconList = new List<IconEntryBar>();
            SettingsColonistBar barSettings = Settings.barSettings;
            SettingsPSI psiSettings = Settings.psiSettings;

            if (barSettings == null || psiSettings == null)
            {
                GameComponentPSI.Reinit(true, false, false);
                return;
            }

            if (!this.traitsCheck)
            {
                this.CheckTraits();
            }

            if (!this.pawn.Spawned || this.pawn.Map == null)
            {
                return;
            }

            if (Current.ProgramState != ProgramState.Playing)
            {
                return;
            }

            Pawn_NeedsTracker needs = this.pawn.needs;
            if (needs?.mood != null)
            {
                this.Mb = this.pawn.mindState?.mentalBreaker;
                this.Mood = needs.mood;
            }
            else
            {
                this.Mood = null;
                this.Mb = null;
            }

            if (!barSettings.UsePsi && !psiSettings.UsePsi)
            {
                return;
            }

            // target
            this.CheckJobs();
            float viewOpacity = psiSettings.IconOpacity;
            float viewOpacityCrit = Settings.ViewOpacityCrit;

            // Mental Breaker for MoodBars
            if (needs != null)
            {
                needs.mood?.thoughts?.GetDistinctMoodThoughtGroups(thoughts);

                // Hungry
                if (needs.food != null)
                {
                    if ((double)needs.food?.CurLevel < Settings.psiSettings.LimitFoodLess)
                    {
                        Color color =
                            Statics.gradientRedAlertToNeutral.Evaluate(
                                needs.food.CurLevel / Settings.psiSettings.LimitFoodLess);
                        if (barSettings.ShowHungry)
                        {
                            string tooltip = needs.food.GetTipString();

                            barIconList.Add(new IconEntryBar(Icon.Hungry, color, tooltip));
                        }

                        if (psiSettings.ShowHungry)
                        {
                            psiIconList.Add(new IconEntryPSI(Icon.Hungry, color, viewOpacityCrit));
                        }
                    }
                }

                // Tired
                if (needs.rest != null)
                {
                    if (needs.rest.CurLevel < (double)Settings.psiSettings.LimitRestLess)
                    {
                        Color color =
                            Statics.gradientRedAlertToNeutral.Evaluate(
                                needs.rest.CurLevel / Settings.psiSettings.LimitRestLess);

                        if (barSettings.ShowTired)
                        {
                            string tooltip = needs.rest.CurCategory.GetLabel();
                            barIconList.Add(new IconEntryBar(Icon.Tired, color, tooltip));
                        }

                        if (psiSettings.ShowTired)
                        {
                            psiIconList.Add(new IconEntryPSI(Icon.Tired, color, viewOpacityCrit));
                        }
                    }
                }
            }

            // Log.Message(pawn + " health: " + this.pawnHealth);
            // Idle - Colonist icon only
            if ((this.pawn.mindState != null) && this.pawn.mindState.IsIdle)
            {
                if (psiSettings.ShowIdle && GenDate.DaysPassed >= 0.1)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Idle, Textures.ColorNeutralStatus, viewOpacity));
                }
            }

            if (this.isPacifist)
            {
                if (barSettings.ShowPacific)
                {
                    barIconList.Add(
                        new IconEntryBar(
                            Icon.Pacific,
                            Textures.ColBlueishGreen,
                            "IsIncapableOfViolence".Translate(this.pawn.NameStringShort)));
                }

                if (psiSettings.ShowPacific)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Pacific, Textures.ColBlueishGreen, viewOpacity));
                }
            }

            if (this.pawn.equipment?.Primary == null && !this.pawn.IsPrisoner && !this.isPacifist)
            {
                // AddIconsToList(
                // barSettings.ShowUnarmed,
                // ref barIconList,
                // psiSettings.ShowUnarmed,
                // ref psiIconList,
                // Icon.Unarmed,
                // ColorNeutralStatusOpaque,
                // "PSI.Settings.Visibility.Unarmed".Translate());
                if (barSettings.ShowUnarmed)
                {
                    barIconList.Add(
                        new IconEntryBar(
                            Icon.Unarmed,
                            Textures.ColorNeutralStatusOpaque,
                            "PSI.Settings.Visibility.Unarmed".Translate()));
                }

                if (psiSettings.ShowUnarmed)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Unarmed, Textures.ColorNeutralStatusOpaque, viewOpacity));
                }
            }

            // Trait Pyromaniac
            if (this.isPyromaniac)
            {
                if (barSettings.ShowPyromaniac)
                {
                    barIconList.Add(
                        new IconEntryBar(
                            Icon.Pyromaniac,
                            Textures.ColYellow,
                            "PSI.Settings.Visibility.Pyromaniac".Translate()));
                }

                if (psiSettings.ShowPyromaniac)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Pyromaniac, Textures.ColYellow, viewOpacityCrit));
                }
            }

            // efficiency
            float efficiency = psiSettings.LimitEfficiencyLess;
            float efficiencyTotal = 1f;
            string efficiencyTip = null;
            bool flag2 = false;

            // temperature
            float temperatureForCell = GenTemperature.GetTemperatureForCell(this.pawn.Position, this.pawn.Map);

            this.TooCold = (float)((this.pawn.ComfortableTemperatureRange().min
                                    - (double)Settings.psiSettings.LimitTempComfortOffset - temperatureForCell) / 10f);

            this.TooHot = (float)((temperatureForCell - (double)this.pawn.ComfortableTemperatureRange().max
                                   - Settings.psiSettings.LimitTempComfortOffset) / 10f);

            this.TooCold = Mathf.Clamp(this.TooCold, 0f, 2f);

            this.TooHot = Mathf.Clamp(this.TooHot, 0f, 2f);

            // Too Cold & too hot
            if (this.TooCold > 0f)
            {
                Color color = Statics.gradient4.Evaluate(this.TooCold / 2);
                if (barSettings.ShowTooCold)
                {
                    barIconList.Add(
                        new IconEntryBar(
                            Icon.TooCold,
                            color,
                            "PSI.Settings.Visibility.TooCold".Translate()));
                }

                if (psiSettings.ShowTooCold)
                {
                    psiIconList.Add(
                        new IconEntryPSI(Icon.TooCold, color, viewOpacityCrit));
                }
            }

            if (this.TooHot > 0f)
            {
                Color color = Statics.gradient4.Evaluate(this.TooHot / 2);
                if (barSettings.ShowTooHot)
                {
                    barIconList.Add(
                        new IconEntryBar(
                            Icon.TooHot,
                            color,
                            "PSI.Settings.Visibility.TooHot".Translate()));
                }

                if (psiSettings.ShowTooHot)
                {
                    psiIconList.Add(
                        new IconEntryPSI(Icon.TooHot, color, viewOpacityCrit));
                }
            }

            // Mental Sanity
            this.MentalSanity = null;
            if (this.pawn.mindState != null && this.pawn.InMentalState)
            {
                this.MentalSanity = this.pawn.MentalStateDef;
            }

            if (this.MentalSanity != null)
            {
                if (this.pawn.InAggroMentalState)
                {
                    if (barSettings.ShowAggressive)
                    {
                        barIconList.Add(new IconEntryBar(Icon.Aggressive, Textures.ColVermillion, null));
                    }

                    if (psiSettings.ShowAggressive)
                    {
                        psiIconList.Add(new IconEntryPSI(Icon.Aggressive, Textures.ColVermillion, viewOpacityCrit));
                    }

                    // Give Up Exit
                    if (this.MentalSanity == MentalStateDefOf.PanicFlee)
                    {
                        if (barSettings.ShowLeave)
                        {
                            barIconList.Add(new IconEntryBar(Icon.Leave, Textures.ColVermillion, null));
                        }

                        if (psiSettings.ShowLeave)
                        {
                            psiIconList.Add(new IconEntryPSI(Icon.Leave, Textures.ColVermillion, viewOpacityCrit));
                        }
                    }

                    // Daze Wander
                    if (this.MentalSanity == MentalStateDefOf.WanderSad)
                    {
                        if (barSettings.ShowDazed)
                        {
                            // + MentalStateDefOf.WanderPsychotic
                            barIconList.Add(new IconEntryBar(Icon.Dazed, Textures.ColYellow, null));
                        }

                        if (psiSettings.ShowDazed)
                        {
                            // + MentalStateDefOf.WanderPsychotic
                            psiIconList.Add(new IconEntryPSI(Icon.Dazed, Textures.ColYellow, viewOpacityCrit));
                        }
                    }

                    // PanicFlee
                    if (this.MentalSanity == MentalStateDefOf.PanicFlee)
                    {
                        if (barSettings.ShowPanic)
                        {
                            barIconList.Add(new IconEntryBar(Icon.Panic, Textures.ColYellow, null));
                        }

                        if (psiSettings.ShowPanic)
                        {
                            psiIconList.Add(new IconEntryPSI(Icon.Panic, Textures.ColYellow, viewOpacityCrit));
                        }
                    }
                }
            }

            // Health Calc
            this.DiseaseDisappearance = 1f;
            this.HealthDisease = 1f;

            // Drug addiction
            List<Hediff> hediffs = null;

            // Sick thoughts
            Pawn_HealthTracker health = this.pawn.health;
            if (health != null)
            {
                array = GameComponentPSI.pawnCapacities;
                for (int i = 0; i < array.Length; i++)
                {
                    PawnCapacityDef pawnCapacityDef = array[i];
                    {
                        // if (pawnCapacityDef != PawnCapacityDefOf.Consciousness)
                        float level = health?.capacities?.GetLevel(pawnCapacityDef) ?? 1f;
                        if (level < efficiency)
                        {
                            if (efficiencyTip.NullOrEmpty())
                            {
                                efficiencyTip = "PSI.Efficiency".Translate() + ": " + pawnCapacityDef.LabelCap + " "
                                                + level.ToStringPercent();
                            }
                            else
                            {
                                efficiencyTip += "\n" + "PSI.Efficiency".Translate() + ": " + pawnCapacityDef.LabelCap
                                                 + " " + level.ToStringPercent();
                            }

                            efficiencyTotal = Mathf.Min(level, efficiencyTotal);
                            flag2 = true;
                        }
                    }
                }

                if (flag2)
                {
                    Color color =
                        Statics.gradientRedAlertToNeutral.Evaluate(
                            efficiencyTotal / Settings.psiSettings.LimitEfficiencyLess);

                    if (barSettings.ShowEffectiveness)
                    {
                        barIconList.Add(new IconEntryBar(Icon.Effectiveness, color, efficiencyTip));
                    }

                    if (psiSettings.ShowEffectiveness)
                    {
                        psiIconList.Add(new IconEntryPSI(Icon.Effectiveness, color, viewOpacityCrit));
                    }
                }

                if (health.hediffSet != null)
                {
                    hediffs = health.hediffSet.hediffs;

                    // Health
                    // Infection

                    // Bleed rate
                    this.BleedRate = Mathf.Clamp01(
                        health.hediffSet.BleedRateTotal * Settings.psiSettings.LimitBleedMult);

                    if (this.BleedRate > 0.0f)
                    {
                        Color color = Statics.gradientRedAlertToNeutral.Evaluate(1.0f - this.BleedRate);
                        if (barSettings.ShowBloodloss)
                        {
                            string tooltip = "BleedingRate".Translate() + ": "
                                             + health.hediffSet.BleedRateTotal.ToStringPercent() + "/d";

                            barIconList.Add(new IconEntryBar(Icon.Bloodloss, color, tooltip));
                        }

                        if (psiSettings.ShowBloodloss)
                        {
                            psiIconList.Add(new IconEntryPSI(Icon.Bloodloss, color, viewOpacityCrit));
                        }
                    }

                    if (HealthAIUtility.ShouldBeTendedNowUrgent(this.pawn))
                    {
                        if (barSettings.ShowMedicalAttention)
                        {
                            barIconList.Add(
                                new IconEntryBar(
                                    Icon.MedicalAttention,
                                    Textures.ColVermillion,
                                    "NeedsTendingNow".Translate()));
                        }

                        if (psiSettings.ShowMedicalAttention)
                        {
                            psiIconList.Add(
                                new IconEntryPSI(Icon.MedicalAttention, Textures.ColVermillion, viewOpacityCrit));
                        }
                    }
                    else if (HealthAIUtility.ShouldBeTendedNow(this.pawn))
                    {
                        if (barSettings.ShowMedicalAttention)
                        {
                            barIconList.Add(
                                new IconEntryBar(
                                    Icon.MedicalAttention,
                                    Textures.ColYellow,
                                    "NeedsTendingNow".Translate()));
                        }

                        if (psiSettings.ShowMedicalAttention)
                        {
                            psiIconList.Add(
                                new IconEntryPSI(Icon.MedicalAttention, Textures.ColYellow, viewOpacityCrit));
                        }
                    }

                    if (HealthAIUtility.ShouldHaveSurgeryDoneNow(this.pawn))
                    {
                        if (barSettings.ShowMedicalAttention)
                        {
                            barIconList.Add(
                                new IconEntryBar(
                                    Icon.MedicalAttention,
                                    Textures.ColBlueishGreen,
                                    "ShouldHaveSurgeryDoneNow".Translate()));
                        }

                        if (psiSettings.ShowMedicalAttention)
                        {
                            psiIconList.Add(
                                new IconEntryPSI(Icon.MedicalAttention, Textures.ColBlueishGreen, viewOpacityCrit));
                        }
                    }

                    if ((health?.hediffSet?.AnyHediffMakesSickThought ?? false) && !this.pawn.Destroyed
                        && this.pawn.playerSettings.medCare >= 0)
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
                                        ThoughtDefOf.Sick,
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

                                if (!hediff.CurStage.becomeVisible)
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

                                if (Math.Abs(health.immunity.GetImmunity(hediff.def) - 1.0) < 0.05)
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

                        if (this.DiseaseDisappearance < Settings.psiSettings.LimitDiseaseLess)
                        {
                            string tooltip = this.sickTip + "\n" + this.healthTip + "\n" + "Immunity".Translate()
                                             + " / " + "PSI.DiseaseProgress".Translate() + ": \n"
                                             + this.immunity.ToStringPercent() + " / " + this.severity.ToStringPercent()
                                             + ": \n" + this.sickMoodOffset;

                            Color color =
                                Statics.gradient4.Evaluate(this.DiseaseDisappearance / psiSettings.LimitDiseaseLess);
                            if (barSettings.ShowHealth)
                            {
                                // Regular Sickness
                                barIconList.Add(
                                    new IconEntryBar(
                                        Icon.Health,
                                        color,
                                        tooltip));
                            }

                            if (psiSettings.ShowHealth)
                            {
                                // Regular Sickness
                                psiIconList.Add(
                                    new IconEntryPSI(
                                        Icon.Health,
                                        color,
                                        viewOpacityCrit));
                            }
                        }
                    }
                }
                else if (health.summaryHealth?.SummaryHealthPercent < 1f)
                {
                    string tooltip = "Health".Translate() + ": "
                                     + health.summaryHealth.SummaryHealthPercent.ToStringPercent();
                    var pawnHealth = 1f - this.pawn.health?.summaryHealth?.SummaryHealthPercent ?? 1f;

                    Color color = Statics.gradient4.Evaluate(pawnHealth);
                    if (barSettings.ShowHealth)
                    {
                        barIconList.Add(
                            new IconEntryBar(Icon.Health, color, tooltip));
                    }

                    if (psiSettings.ShowHealth)
                    {
                        psiIconList.Add(
                            new IconEntryPSI(
                                Icon.Health,
                                color,
                                viewOpacityCrit));
                    }
                }
            }

            // Toxicity buildup
            if (this.ToxicBuildUpVisible > 0f)
            {
                Color color = Statics.gradient4.Evaluate(this.ToxicBuildUpVisible);
                if (barSettings.ShowToxicity)
                {
                    string tooltip = this.toxicTip;

                    barIconList.Add(
                        new IconEntryBar(Icon.Toxicity, color, tooltip));
                }

                if (psiSettings.ShowToxicity)
                {
                    string tooltip = this.toxicTip;

                    psiIconList.Add(
                        new IconEntryPSI(
                            Icon.Toxicity,
                            color,
                            viewOpacityCrit));
                }
            }

            string appareltip = null;
            bool barAp = false;
            bool psiAp = false;

            // Apparel Calc

            // Naked
            if (GetThought(ThoughtDefOf.Naked, out this.feelsNaked, out this.nakedTip, out this.nakedMoodOffset))
            {
                Color moodOffsetColor = this.nakedMoodOffset.MoodOffsetColor();
                if (barSettings.ShowNaked)
                {
                    barIconList.Add(new IconEntryBar(Icon.Naked, moodOffsetColor, this.nakedTip));
                }

                if (psiSettings.ShowNaked)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Naked, moodOffsetColor, viewOpacity));
                }
            }
            else
            {
                if (GetThought(
                    ThoughtDefOf.ApparelDamaged,
                    out int apparelStage,
                    out string apparelTooltip,
                    out float moodOffset))
                {
                    barAp = true;
                    psiAp = true;
                    appareltip = appareltip == null ? apparelTooltip : appareltip + "\n" + apparelTooltip;
                }

                if (GetThought(
                    ThoughtDefOf.HumanLeatherApparelSad,
                    out apparelStage,
                    out apparelTooltip,
                    out moodOffset))
                {
                    barAp = true;
                    psiAp = true;
                    appareltip = appareltip == null ? apparelTooltip : appareltip + "\n" + apparelTooltip;
                }

                if (GetThought(ThoughtDefOf.DeadMansApparel, out apparelStage, out apparelTooltip, out moodOffset))
                {
                    barAp = true;
                    psiAp = true;
                    appareltip = appareltip == null ? apparelTooltip : appareltip + "\n" + apparelTooltip;
                }

                if (barAp)
                {
                    if (barSettings.ShowApparelHealth)
                    {
                        barIconList.Add(new IconEntryBar(Icon.ApparelHealth, Textures.ColVermillion, appareltip));
                    }
                }

                if (psiAp)
                {
                    if (psiSettings.ShowApparelHealth)
                    {
                        psiIconList.Add(new IconEntryPSI(Icon.ApparelHealth, Textures.ColVermillion, viewOpacity));
                    }
                }
            }

            // Prostho check only if pawn has any traits
            if (this.prostho != 0)
            {
                switch (this.prostho)
                {
                    case -1:
                        GetThought(
                            ThoughtDefOf.ProsthophobeUnhappy,
                            out this.prosthoUnhappy,
                            out this.prosthoTooltip,
                            out this.prosthoMoodOffset);
                        break;

                    case 1:
                        GetThought(
                            ThoughtDefOf.ProsthophileNoProsthetic,
                            out this.prosthoUnhappy,
                            out this.prosthoTooltip,
                            out this.prosthoMoodOffset);
                        break;

                    default: break;
                }
            }

            // Bed status
            if (this.pawn.ownership.OwnedBed != null)
            {
                GetThought(
                    ThoughtDefOf.SharedBed,
                    out this.BedStatus,
                    out this.BedStatusTip,
                    out this.BedStatusMoodOffset);
            }
            else
            {
                this.BedStatus = 1;
                this.BedStatusTip = "NeedColonistBeds".Translate();
            }

            // Cabin Fever
            if (GetThought(
                ThoughtDefOf.CabinFever,
                out this.CabinFeverMoodLevel,
                out this.cabinFeverTip,
                out this.CabinFeverMoodOffset))
            {
                Color moodOffset = this.CabinFeverMoodOffset.MoodOffsetColor();
                if (barSettings.ShowCabinFever)
                {
                    string tooltip = this.cabinFeverTip;
                    barIconList.Add(new IconEntryBar(Icon.CabinFever, moodOffset, tooltip));
                }

                if (psiSettings.ShowCabinFever)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.CabinFever, moodOffset, viewOpacityCrit));
                }
            }

            // Pain
            if (GetThought(this.painThought, out this.PainMoodLevel, out this.painTip, out this.PainMoodOffset))
            {
                Color moodOffset = this.PainMoodOffset.MoodOffsetColor();
                if (barSettings.ShowPain)
                {
                    barIconList.Add(new IconEntryBar(Icon.Pain, moodOffset, this.painTip));
                }

                if (psiSettings.ShowPain)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Pain, moodOffset, viewOpacityCrit));
                }
            }

            // Night Owl
            if (this.isNightOwl)
            {
                if (GetThought(
                    ThoughtDefOf.NightOwlDuringTheDay,
                    out this.nightOwlUnhappy,
                    out this.nightOwlTip,
                    out this.nightOwlMoodOffset))
                {
                    Color moodOffset = this.nightOwlMoodOffset.MoodOffsetColor();
                    if (barSettings.ShowNightOwl)
                    {
                        barIconList.Add(new IconEntryBar(Icon.NightOwl, moodOffset, this.nightOwlTip));
                    }

                    if (psiSettings.ShowNightOwl)
                    {
                        psiIconList.Add(new IconEntryPSI(Icon.NightOwl, moodOffset, viewOpacityCrit));
                    }
                }
            }

            // Greedy
            if (this.hasGreedyTrait)
            {
                if (GetThought(
                    ThoughtDefOf.Greedy,
                    out this.greedyThought,
                    out this.greedyTooltip,
                    out this.greedyMoodOffset))
                {
                    Color moodOffsetColor = this.greedyMoodOffset.MoodOffsetColor();
                    if (barSettings.ShowGreedy)
                    {
                        barIconList.Add(new IconEntryBar(Icon.Greedy, moodOffsetColor, this.greedyTooltip));
                    }

                    if (psiSettings.ShowGreedy)
                    {
                        psiIconList.Add(new IconEntryPSI(Icon.Greedy, moodOffsetColor, viewOpacity));
                    }
                }
            }

            // Jealous
            if (this.hasJealousTrait)
            {
                if (GetThought(
                    ThoughtDefOf.Jealous,
                    out this.jealousThought,
                    out this.jealousTooltip,
                    out this.jealousMoodOffset))
                {
                    Color moodOffset = this.jealousMoodOffset.MoodOffsetColor();
                    if (barSettings.ShowJealous)
                    {
                        barIconList.Add(new IconEntryBar(Icon.Jealous, moodOffset, this.jealousTooltip));
                    }

                    if (psiSettings.ShowJealous)
                    {
                        psiIconList.Add(new IconEntryPSI(Icon.Jealous, moodOffset, viewOpacity));
                    }
                }
            }

            // Unburied
            if (GetThought(
                ThoughtDefOf.ColonistLeftUnburied,
                out this.unburied,
                out this.unburiedTip,
                out this.unburiedMoodOffset))
            {
                Color moodOffset = this.unburiedMoodOffset.MoodOffsetColor();
                if (barSettings.ShowLeftUnburied)
                {
                    string tooltip = this.unburiedTip;
                    barIconList.Add(new IconEntryBar(Icon.LeftUnburied, moodOffset, tooltip));
                }

                if (psiSettings.ShowLeftUnburied)
                {
                    string tooltip = this.unburiedTip;
                    psiIconList.Add(new IconEntryPSI(Icon.LeftUnburied, moodOffset, viewOpacity));
                }
            }

            this.isAddict = false;
            this.withDrawal = false;
            this.withDrawalPercent = 0f;
            this.addictionLabel = null;
            if (hediffs != null)
            {
                for (int i = 0; i < hediffs.Count; i++)
                {
                    Hediff hediff = hediffs[i];
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
                        color = Textures.ColVermillion;
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
                            color = Textures.ColSkyBlue;
                            break;

                        case 1:
                            color = Textures.ColYellow;
                            break;

                        case 2:
                            color = Textures.ColOrange;
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

            // Humping
            GetThought(
                ThoughtDefOf.WantToSleepWithSpouseOrLover,
                out this.wantsToHump,
                out this.humpTip,
                out this.humpMoodOffset);

            // Bed status
            if (this.wantsToHump > -1)
            {
                Color moodOffset = this.humpMoodOffset.MoodOffsetColor();
                if (barSettings.ShowBedroom)
                {
                    barIconList.Add(new IconEntryBar(Icon.Bedroom, moodOffset, this.humpTip));
                }

                if (psiSettings.ShowBedroom)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Bedroom, moodOffset, viewOpacityCrit));
                }
            }
            else if (this.BedStatus > -1)
            {
                Color moodOffset = this.BedStatusMoodOffset.MoodOffsetColor();
                if (barSettings.ShowBedroom)
                {
                    barIconList.Add(new IconEntryBar(Icon.Bedroom, moodOffset, this.BedStatusTip));
                }

                if (psiSettings.ShowBedroom)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Bedroom, moodOffset, viewOpacity));
                }

                // Moods caused by traits
                if (this.prosthoUnhappy > -1)
                {
                    Color offset = this.prosthoMoodOffset.MoodOffsetColor();

                    if (this.prostho == 1)
                    {
                        if (barSettings.ShowProsthophile)
                        {
                            barIconList.Add(new IconEntryBar(Icon.Prosthophile, offset, this.prosthoTooltip));
                        }

                        if (psiSettings.ShowProsthophile)
                        {
                            psiIconList.Add(new IconEntryPSI(Icon.Prosthophile, offset, viewOpacity));
                        }
                    }

                    if (this.prostho == -1)
                    {
                        if (barSettings.ShowProsthophobe)
                        {
                            barIconList.Add(new IconEntryBar(Icon.Prosthophobe, offset, this.prosthoTooltip));
                        }

                        if (psiSettings.ShowProsthophobe)
                        {
                            psiIconList.Add(new IconEntryPSI(Icon.Prosthophobe, offset, viewOpacity));
                        }
                    }
                }
            }

            // Log.Message(this.PSIPawn.LabelShort + " icons to be displayed: ");
            // foreach (IconEntryBar entry in barIconList)
            // {
            // Log.Message(entry.icon + " - " + entry.tooltip);
            // }
            this.BarIconList = barIconList.OrderBy(x => x.icon).ToList();
            this.PSIIconList = psiIconList.OrderBy(x => x.icon).ToList();
        }

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
    }
}