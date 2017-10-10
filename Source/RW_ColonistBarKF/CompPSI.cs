namespace ColonistBarKF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using static ColonistBarKF.Bar.ColonistBarTextures;
    using ColonistBarKF.PSI;

    using JetBrains.Annotations;

    using RimWorld;

    using static Settings;

    using static Statics;

    using UnityEngine;

    using Verse;
    using Verse.AI;

    [StaticConstructorOnStartup]
    public class CompPSI : ThingComp
    {
        #region Public Fields

        private Color bgColor = Color.gray;

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

        #endregion Public Fields

        #region Private Fields

        private static PawnCapacityDef[] array;

        private static readonly List<Thought> thoughts = new List<Thought>();

        [CanBeNull]
        private string addictionLabel;

        // public float Drunkness = 0f;
        private int BedStatus = -1;

        private float BedStatusMoodOffset;

        [CanBeNull]
        private string BedStatusTip;

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

        private float pawnHealth = 1f;

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

        #endregion Private Fields

        #region Public Properties

        [NotNull]
        public List<IconEntryBar> BarIconList { get; private set; } = new List<IconEntryBar>();



        [NotNull]
        public List<IconEntryPSI> PSIIconList { get; private set; } = new List<IconEntryPSI>();



        public Color BGColor
        {
            get => this.bgColor;

            set => this.bgColor = value;
        }

        #endregion Public Properties

        #region Public Methods

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

        public override void CompTick()
        {
            base.CompTick();
            Pawn pawn = this.parent as Pawn;
            if (pawn == null)
            {
                return;
            }

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

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref this.bgColor, "bgColor");
        }

        public void SetEntriesDirty()
        {
            this.entriesDirty = true;
        }

        #endregion Public Methods

        #region Private Methods


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

        private void CheckTraits(Pawn pawn)
        {
            {
                if (pawn.RaceProps.hasGenders)
                {
                    if (pawn.Dead)
                    {
                        this.bgColor = Color.gray;
                    }
                    else
                    {
                        switch (pawn.gender)
                        {
                            case Gender.Male:
                                this.bgColor = MaleColor;
                                break;

                            case Gender.Female:
                                this.bgColor = FemaleColor;
                                break;
                        }
                    }
                }

                // One time traits check
                if (pawn.story?.traits != null)
                {

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

        private void GetWithdrawalColor(out Color color)
        {
            color = Color.Lerp(ColBlueishGreen, ColVermillion, this.withDrawalPercent);
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

            Pawn pawn = this.parent as Pawn;
            if (pawn == null)
            {
                return;
            }

            if (!this.traitsCheck)
            {
                this.CheckTraits(pawn);
            }

            if (!pawn.Spawned || pawn.Map == null)
            {
                return;
            }

            if (Current.ProgramState != ProgramState.Playing)
            {
                return;
            }

            // target
            this.CheckJobs(pawn);

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

            if (!barSettings.UsePsi && !psiSettings.UsePsi)
            {
                return;
            }

            float viewOpacity = psiSettings.IconOpacity;
            float viewOpacityCrit = ViewOpacityCrit;

            pawn.needs?.mood?.thoughts?.GetDistinctMoodThoughtGroups(thoughts);
            this.pawnHealth = 1f - pawn.health.summaryHealth.SummaryHealthPercent;

            // Log.Message(pawn + " health: " + this.pawnHealth);
            // Idle - Colonist icon only
            if (pawn.mindState.IsIdle)
            {
                if (psiSettings.ShowIdle && GenDate.DaysPassed >= 0.1)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Idle, ColorNeutralStatus, viewOpacity));
                }
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
            array = GameComponentPSI.pawnCapacities;
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
                            gradientRedAlertToNeutral.Evaluate(efficiencyTotal / PsiSettings.LimitEfficiencyLess),
                            efficiencyTip));
                }

                if (psiSettings.ShowEffectiveness)
                {
                    psiIconList.Add(
                        new IconEntryPSI(
                            Icon.Effectiveness,
                            gradientRedAlertToNeutral.Evaluate(efficiencyTotal / PsiSettings.LimitEfficiencyLess),
                            viewOpacityCrit));
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
                        new IconEntryBar(
                            Icon.TooCold,
                            gradient4.Evaluate(this.TooCold / 2),
                            "PSI.Settings.Visibility.TooCold".Translate()));
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
                        new IconEntryBar(
                            Icon.TooHot,
                            gradient4.Evaluate(this.TooHot / 2),
                            "PSI.Settings.Visibility.TooHot".Translate()));
                }

                if (psiSettings.ShowTooHot)
                {
                    psiIconList.Add(
                        new IconEntryPSI(Icon.TooHot, gradient4.Evaluate(this.TooHot / 2), viewOpacityCrit));
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
                    if (this.MentalSanity == MentalStateDefOf.PanicFlee)
                    {
                        if (barSettings.ShowLeave)
                        {
                            barIconList.Add(new IconEntryBar(Icon.Leave, ColVermillion, null));
                        }

                        if (psiSettings.ShowLeave)
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
                this.BleedRate = Mathf.Clamp01(pawn.health.hediffSet.BleedRateTotal * PsiSettings.LimitBleedMult);

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

                if (HealthAIUtility.ShouldBeTendedNowUrgent(pawn))
                {
                    if (barSettings.ShowMedicalAttention)
                    {
                        barIconList.Add(
                            new IconEntryBar(Icon.MedicalAttention, ColVermillion, "NeedsTendingNow".Translate()));
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

            if (pawn.health.hediffSet.AnyHediffMakesSickThought && !pawn.Destroyed
                && pawn.playerSettings.medCare >= 0)
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
                            GetThought(ThoughtDefOf.Sick, out int dummy, out this.sickTip, out this.sickMoodOffset);
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

            string appareltip = null;
            bool barAp = false;
            bool psiAp = false;

            // Apparel Calc

            // Naked
            if (GetThought(ThoughtDefOf.Naked, out this.feelsNaked, out this.nakedTip, out this.nakedMoodOffset))
            {
                if (barSettings.ShowNaked)
                {
                    barIconList.Add(
                        new IconEntryBar(Icon.Naked, EvaluateMoodOffset(this.nakedMoodOffset), this.nakedTip));
                }

                if (psiSettings.ShowNaked)
                {
                    psiIconList.Add(
                        new IconEntryPSI(Icon.Naked, EvaluateMoodOffset(this.nakedMoodOffset), viewOpacity));
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
                        barIconList.Add(new IconEntryBar(Icon.ApparelHealth, ColVermillion, appareltip));
                    }
                }

                if (psiAp)
                {
                    if (psiSettings.ShowApparelHealth)
                    {
                        psiIconList.Add(new IconEntryPSI(Icon.ApparelHealth, ColVermillion, viewOpacity));
                    }
                }
            }

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
            if (pawn.ownership.OwnedBed != null)
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
                if (barSettings.ShowCabinFever)
                {
                    string tooltip = this.cabinFeverTip;
                    barIconList.Add(
                        new IconEntryBar(Icon.CabinFever, EvaluateMoodOffset(this.CabinFeverMoodOffset), tooltip));
                }

                if (psiSettings.ShowCabinFever)
                {
                    psiIconList.Add(
                        new IconEntryPSI(
                            Icon.CabinFever,
                            EvaluateMoodOffset(this.CabinFeverMoodOffset),
                            viewOpacityCrit));
                }
            }

            // Pain
            if (GetThought(this.painThought, out this.PainMoodLevel, out this.painTip, out this.PainMoodOffset))
            {
                if (barSettings.ShowPain)
                {
                    barIconList.Add(new IconEntryBar(Icon.Pain, EvaluateMoodOffset(this.PainMoodOffset), this.painTip));
                }

                if (psiSettings.ShowPain)
                {
                    psiIconList.Add(
                        new IconEntryPSI(Icon.Pain, EvaluateMoodOffset(this.PainMoodOffset), viewOpacityCrit));
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
                    if (barSettings.ShowNightOwl)
                    {
                        barIconList.Add(
                            new IconEntryBar(
                                Icon.NightOwl,
                                EvaluateMoodOffset(this.nightOwlMoodOffset),
                                this.nightOwlTip));
                    }

                    if (psiSettings.ShowNightOwl)
                    {
                        psiIconList.Add(
                            new IconEntryPSI(
                                Icon.NightOwl,
                                EvaluateMoodOffset(this.nightOwlMoodOffset),
                                viewOpacityCrit));
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
                    if (barSettings.ShowGreedy)
                    {
                        barIconList.Add(
                            new IconEntryBar(
                                Icon.Greedy,
                                EvaluateMoodOffset(this.greedyMoodOffset),
                                this.greedyTooltip));
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
                    ThoughtDefOf.Jealous,
                    out this.jealousThought,
                    out this.jealousTooltip,
                    out this.jealousMoodOffset))
                {
                    if (barSettings.ShowJealous)
                    {
                        barIconList.Add(
                            new IconEntryBar(
                                Icon.Jealous,
                                EvaluateMoodOffset(this.jealousMoodOffset),
                                this.jealousTooltip));
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
                ThoughtDefOf.ColonistLeftUnburied,
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

            // Humping
            GetThought(
                ThoughtDefOf.WantToSleepWithSpouseOrLover,
                out this.wantsToHump,
                out this.humpTip,
                out this.humpMoodOffset);

            // Bed status
            if (this.wantsToHump > -1)
            {
                if (barSettings.ShowBedroom)
                {
                    barIconList.Add(
                        new IconEntryBar(Icon.Bedroom, EvaluateMoodOffset(this.humpMoodOffset), this.humpTip));
                }

                if (psiSettings.ShowBedroom)
                {
                    psiIconList.Add(
                        new IconEntryPSI(Icon.Bedroom, EvaluateMoodOffset(this.humpMoodOffset), viewOpacityCrit));
                }
            }
            else if (this.BedStatus > -1)
            {
                if (barSettings.ShowBedroom)
                {
                    barIconList.Add(
                        new IconEntryBar(
                            Icon.Bedroom,
                            EvaluateMoodOffset(this.BedStatusMoodOffset),
                            this.BedStatusTip));
                }

                if (psiSettings.ShowBedroom)
                {
                    psiIconList.Add(
                        new IconEntryPSI(Icon.Bedroom, EvaluateMoodOffset(this.BedStatusMoodOffset), viewOpacity));
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
                                new IconEntryPSI(
                                    Icon.Prosthophile,
                                    EvaluateMoodOffset(this.prosthoMoodOffset),
                                    viewOpacity));
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
                                new IconEntryPSI(
                                    Icon.Prosthophobe,
                                    EvaluateMoodOffset(this.prosthoMoodOffset),
                                    viewOpacity));
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

        private void CheckJobs(Pawn pawn)
        {
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

                if (targetInfo != null)
                {
                    Vector3 a = targetInfo.Cell.ToVector3Shifted();
                    this.TargetPos = a + new Vector3(0f, 3f, 0f);
                }
            }
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

        #endregion Private Methods

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