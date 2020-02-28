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
using static ColonistBarKF.Settings;

namespace ColonistBarKF
{
    [StaticConstructorOnStartup]
    public class CompPSI : ThingComp
    {
        private static readonly List<Thought> Thoughts = new List<Thought>();

        private static PawnCapacityDef[] _array;

        public bool HasRelationWithColonist;

        public bool IsPacifist;

        public MentalBreaker Mb;

        public Need_Mood Mood;

        public int ProsthoStage;

        public Vector3 TargetPos = Vector3.zero;

        public int ThisColCount = 2;

        public float TotalEfficiency = 1f;

        private string _addictionLabel;

        // public float Drunkness = 0f;
        private int _bedStatus = -1;

        private float _bedStatusMoodOffset;

        private string _bedStatusTip;

        private Color _bgColor = Color.gray;

        private float _bleedRate = -1f;

        private int _needOutdoorsMoodLevel;

        private float _needOutdoorsMoodOffset;

        private string _needOutdoorsTip;

        private float _diseaseDisappearance = 1f;

        private int _drugDesire;

        private string _drugUserLabel;

        private bool _entriesDirty = true;

        private int _feelsNaked = -1;

        private float _greedyMoodOffset;

        private int _greedyThought = -1;

        private string _greedyTooltip;

        private bool _hasGreedyTrait;

        private bool _hasJealousTrait;

        private float _healthDisease = 1f;

        private string _healthTip;

        private float _humpMoodOffset;

        private string _humpTip;

        private float _immunity;

        private bool _isAddict;

        private bool _isMasochist;

        private bool _isNightOwl;

        private bool _isPyromaniac;

        private float _jealousMoodOffset;

        private int _jealousThought = -1;

        private string _jealousTooltip;

        private int _lastStatUpdate;

        private MentalStateDef _mentalSanity;

        private float _nakedMoodOffset;

        private string _nakedTip;

        private int _nextStatUpdate = 1;

        private float _nightOwlMoodOffset;

        private string _nightOwlTip;

        private int _nightOwlUnhappy = -1;

        private int _painMoodLevel;

        private float _painMoodOffset;

        private ThoughtDef _painThought;

        private string _painTip;

        private int _prostho;

        private float _prosthoMoodOffset;

        private string _prosthoTooltip;

        private int _prosthoUnhappy;

        private float _severity;

        private float _sickMoodOffset;

        private string _sickTip;

        private float _tooCold = -1f;

        private float _tooHot = -1f;

        private float _toxicBuildUpVisible;

        private string _toxicTip;

        private bool _traitsCheck;

        private int _unburied = -1;

        private float _unburiedMoodOffset;

        private string _unburiedTip;

        private int _wantsToHump = -1;

        private bool _withDrawal;

        private float _withDrawalPercent;

        public Pawn Pawn => this.parent as Pawn;

        [NotNull]
        public List<IconEntryBar> BarIconList { get; private set; } = new List<IconEntryBar>();

        public Color BgColor
        {
            get => this._bgColor;

            set => this._bgColor = value;
        }

        [NotNull]
        public List<IconEntryPSI> PSIIconList { get; private set; } = new List<IconEntryPSI>();

        public override void CompTick()
        {
            base.CompTick();

            if (this.Pawn.IsColonist || this.Pawn.IsPrisoner)
            {
                if (this._entriesDirty)
                {
                    this.UpdateColonistStats();
                    this._entriesDirty = false;
                }

                this.CheckStats();
            }
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref this._bgColor, "bgColor");
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            Pawn p = this.parent as Pawn;
            if (p == null)
            {
                return;
            }

            if (p.Faction != Faction.OfPlayer)
            {
                this.CheckRelationWithColonists(p);
            }
        }

        public void SetEntriesDirty()
        {
            this._entriesDirty = true;
        }

        private static bool GetThought(ThoughtDef tdef, out int stage, out string tooltip, out float moodOffset)
        {
            tooltip = null;
            stage = -1;
            moodOffset = 0;

            if (Thoughts.NullOrEmpty())
            {
                return false;
            }

            foreach (Thought thought in Thoughts)
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
            Job curJob = this.Pawn.CurJob;
            if (curJob != null)
            {
                JobDriver curDriver = this.Pawn.jobs.curDriver;
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

                if (curDriver is JobDriver_Hunt && this.Pawn.carryTracker?.CarriedThing != null)
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

                if (curJob.def == JobDefOf.LayDown && this.Pawn.InBed())
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

        private void CheckRelationWithColonists(Pawn p)
        {
            bool skip = false;

            if (p.relations.FamilyByBlood.Any(x => x.Faction == Faction.OfPlayer))
            {
                this.HasRelationWithColonist = true;
                skip = true;
            }

            if (!skip)
            {
                if (p.relations.DirectRelations.Any(x => x.otherPawn.Faction == Faction.OfPlayer))
                {
                    this.HasRelationWithColonist = true;
                }
            }
        }

        private void CheckStats()
        {
            // if (Find.TickManager.CurTimeSpeed == TimeSpeed.Paused)
            // {
            // return;
            // }
            int nextUpdate = (int)(this._lastStatUpdate + this._nextStatUpdate * Find.TickManager.TickRateMultiplier);

            if (Find.TickManager.TicksGame <= nextUpdate)
            {
                return;
            }

            this.UpdateColonistStats();

            this._lastStatUpdate = Find.TickManager.TicksGame;
            this._nextStatUpdate = (int)Rand.Range(120f, 300f);

            // Log.Message(
            // "CBKF updated stat " + (this.parent as Pawn).Name + ", next update in " + NextStatUpdate * Find.TickManager.TickRateMultiplier
            // + " ticks.");
        }

        public void CheckTraits()
        {
            {
                if (this.Pawn.RaceProps.hasGenders)
                {
                    if (this.Pawn.Dead)
                    {
                        this.BgColor = Color.gray;
                    }
                    else
                    {
                        switch (this.Pawn.gender)
                        {
                            case Gender.Male:
                                this.BgColor = Textures.MaleColor;
                                break;

                            case Gender.Female:
                                this.BgColor = Textures.FemaleColor;
                                break;
                        }
                    }
                }

                // One time traits check
                if (this.Pawn.story?.traits != null)
                {
                    // Masochist
                    this._isMasochist = this.Pawn.story.traits.HasTrait(TraitDef.Named("Masochist"));

                    // Masochist trait check
                    this._painThought = ThoughtDef.Named(this.Pawn.story.traits.HasTrait(TraitDef.Named("Masochist")) ? "MasochistPain" : "Pain");

                    // Pacifist
                    this.IsPacifist = Pawn.WorkTagIsDisabled(WorkTags.Violent);

                    // Pyromaniac
                    this._isPyromaniac = this.Pawn.story.traits.HasTrait(TraitDefOf.Pyromaniac);

                    // Prostho
                    if (this.Pawn.story.traits.HasTrait(TraitDefOf.BodyPurist))
                    {
                        this._prostho = -1;
                    }

                    if (this.Pawn.story.traits.HasTrait(TraitDefOf.Transhumanist))
                    {
                        this._prostho = 1;
                    }

                    // Night Owl
                    if (this.Pawn.story.traits.HasTrait(TraitDef.Named("NightOwl")))
                    {
                        this._isNightOwl = true;
                    }

                    // Jealous
                    if (this.Pawn.story.traits.HasTrait(TraitDef.Named("Jealous")))
                    {
                        this._hasJealousTrait = true;
                    }

                    // Drug desire
                    if (this.Pawn.story.traits.HasTrait(TraitDefOf.DrugDesire))
                    {
                        this._drugDesire = this.Pawn.story.traits.DegreeOfTrait(TraitDefOf.DrugDesire);
                        this._drugUserLabel = this.Pawn.story.traits.GetTrait(TraitDefOf.DrugDesire).LabelCap;
                    }

                    // Greedy
                    if (this.Pawn.story.traits.HasTrait(TraitDefOf.Greedy))
                    {
                        this._hasGreedyTrait = true;
                    }

                    this._traitsCheck = true;
                }
            }
        }

        private void GetWithdrawalColor(out Color color)
        {
            color = Color.Lerp(Textures.ColBlueishGreen, Textures.ColVermillion, this._withDrawalPercent);
        }

        private void UpdateColonistStats()
        {
            List<IconEntryPSI> psiIconList = new List<IconEntryPSI>();
            List<IconEntryBar> barIconList = new List<IconEntryBar>();
            SettingsColonistBar barSettings = BarSettings;
            SettingsPSI psiSettings = PSISettings;

            if (barSettings == null || psiSettings == null)
            {
                GameComponentPSI.Reinit(true, false, false);
                return;
            }

            if (!this._traitsCheck)
            {
                this.CheckTraits();
            }

            if (!this.Pawn.Spawned || this.Pawn.Map == null)
            {
                return;
            }

            if (Current.ProgramState != ProgramState.Playing)
            {
                return;
            }

            Pawn_NeedsTracker needs = this.Pawn.needs;
            if (needs?.mood != null)
            {
                this.Mb = this.Pawn.mindState?.mentalBreaker;
                this.Mood = needs.mood;
            }
            else
            {
                this.Mood = null;
                this.Mb = null;
            }

            if (!barSettings.UsePSI && !psiSettings.UsePSI)
            {
                return;
            }

            // target
            this.CheckJobs();
            float viewOpacity = psiSettings.IconOpacity;
            float viewOpacityCrit = ViewOpacityCrit;

            // Mental Breaker for MoodBars
            if (needs != null)
            {
                needs.mood?.thoughts?.GetDistinctMoodThoughtGroups(Thoughts);

                // Hungry
                if (needs.food != null)
                {
                    if ((double)needs.food?.CurLevel < PSISettings.LimitFoodLess)
                    {
                        Color color =
                            Statics.GradientRedAlertToNeutral.Evaluate(
                                needs.food.CurLevel / PSISettings.LimitFoodLess);
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
                    if (needs.rest.CurLevel < (double)PSISettings.LimitRestLess)
                    {
                        Color color =
                            Statics.GradientRedAlertToNeutral.Evaluate(
                                needs.rest.CurLevel / PSISettings.LimitRestLess);

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
            if ((this.Pawn.mindState != null) && this.Pawn.mindState.IsIdle)
            {
                if (psiSettings.ShowIdle && GenDate.DaysPassed >= 0.1)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Idle, Textures.ColorNeutralStatus, viewOpacity));
                }
            }

            if (this.IsPacifist)
            {
                if (barSettings.ShowPacific)
                {
                    barIconList.Add(
                        new IconEntryBar(
                            Icon.Pacific,
                            Textures.ColBlueishGreen,
                            "IsIncapableOfViolence".Translate(this.Pawn.LabelShort)));
                }

                if (psiSettings.ShowPacific)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Pacific, Textures.ColBlueishGreen, viewOpacity));
                }
            }

            if (this.Pawn.equipment?.Primary == null && !this.Pawn.IsPrisoner && !this.IsPacifist)
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
            if (this._isPyromaniac)
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
            float temperatureForCell = GenTemperature.GetTemperatureForCell(this.Pawn.Position, this.Pawn.Map);

            this._tooCold = (float)((this.Pawn.ComfortableTemperatureRange().min
                                    - (double)PSISettings.LimitTempComfortOffset - temperatureForCell) / 10f);

            this._tooHot = (float)((temperatureForCell - (double) this.Pawn.ComfortableTemperatureRange().max
                                   - PSISettings.LimitTempComfortOffset) / 10f);

            this._tooCold = Mathf.Clamp(this._tooCold, 0f, 2f);

            this._tooHot = Mathf.Clamp(this._tooHot, 0f, 2f);

            // Too Cold & too hot
            if (this._tooCold > 0f)
            {
                Color color = Statics.Gradient4.Evaluate(this._tooCold / 2);
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

            if (this._tooHot > 0f)
            {
                Color color = Statics.Gradient4.Evaluate(this._tooHot / 2);
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
            this._mentalSanity = null;
            if (this.Pawn.mindState != null && this.Pawn.InMentalState)
            {
                this._mentalSanity = this.Pawn.MentalStateDef;
            }

            if (this._mentalSanity != null)
            {
                if (this.Pawn.InAggroMentalState)
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
                    if (this._mentalSanity == MentalStateDefOf.PanicFlee)
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
                    if (this._mentalSanity == MentalStateDefOf.Wander_Sad)
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
                    if (this._mentalSanity == MentalStateDefOf.PanicFlee)
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
            this._diseaseDisappearance = 1f;
            this._healthDisease = 1f;

            // Drug addiction
            List<Hediff> hediffs = null;

            // Sick thoughts
            Pawn_HealthTracker health = this.Pawn.health;
            if (health != null)
            {
                _array = GameComponentPSI.PawnCapacities;
                for (int i = 0; i < _array.Length; i++)
                {
                    PawnCapacityDef pawnCapacityDef = _array[i];
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
                        Statics.GradientRedAlertToNeutral.Evaluate(
                            efficiencyTotal / PSISettings.LimitEfficiencyLess);

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
                    this._bleedRate = Mathf.Clamp01(
                        health.hediffSet.BleedRateTotal * PSISettings.LimitBleedMult);

                    if (this._bleedRate > 0.0f)
                    {
                        Color color = Statics.GradientRedAlertToNeutral.Evaluate(1.0f - this._bleedRate);
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

                    if (HealthAIUtility.ShouldBeTendedNowByPlayerUrgent(this.Pawn))
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
                    else if (HealthAIUtility.ShouldBeTendedNowByPlayer(this.Pawn))
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

                    if (HealthAIUtility.ShouldHaveSurgeryDoneNow(this.Pawn))
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

                    if ((health?.hediffSet?.AnyHediffMakesSickThought ?? false) && !this.Pawn.Destroyed
                        && this.Pawn.playerSettings.medCare >= 0)
                    {
                        if (hediffs != null)
                        {
                            this._severity = 0f;
                            this._immunity = 0f;
                            foreach (Hediff hediff in hediffs)
                            {
                                if (!hediff.Visible /*|| hediff.IsOld()*/ || !hediff.def.makesSickThought
                                    || hediff.LabelCap.NullOrEmpty() || hediff.SeverityLabel.NullOrEmpty())
                                {
                                    continue;
                                }

                                this._toxicBuildUpVisible = 0;
                                this._healthTip = hediff.LabelCap;
                                if (!Thoughts.NullOrEmpty())
                                {
                                    GetThought(
                                        ThoughtDefOf.Sick,
                                        out int dummy,
                                        out this._sickTip,
                                        out this._sickMoodOffset);
                                }

                                // this.ToxicBuildUpVisible
                                if (hediff.def == HediffDefOf.ToxicBuildup)
                                {
                                    this._toxicTip = hediff.LabelCap + "\n" + hediff.SeverityLabel;
                                    this._toxicBuildUpVisible = Mathf.InverseLerp(0.049f, 1f, hediff.Severity);
                                    continue;
                                }

                                HediffComp_Immunizable compImmunizable = hediff.TryGetComp<HediffComp_Immunizable>();
                                if (compImmunizable != null)
                                {
                                    this._severity = Mathf.Max(this._severity, hediff.Severity);
                                    this._immunity = compImmunizable.Immunity;
                                    float basehealth = this._healthDisease - (this._severity - this._immunity / 4) - 0.25f;
                                    this._healthDisease = basehealth;
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

                                if (this._diseaseDisappearance > compImmunizable.Immunity)
                                {
                                    this._diseaseDisappearance = compImmunizable.Immunity;
                                }

                                // break;
                            }
                        }

                        if (this._diseaseDisappearance < PSISettings.LimitDiseaseLess)
                        {
                            string tooltip = this._sickTip + "\n" + this._healthTip + "\n" + "Immunity".Translate()
                                             + " / " + "PSI.DiseaseProgress".Translate() + ": \n"
                                             + this._immunity.ToStringPercent() + " / " + this._severity.ToStringPercent()
                                             + ": \n" + this._sickMoodOffset;

                            Color color =
                                Statics.Gradient4.Evaluate(this._diseaseDisappearance / psiSettings.LimitDiseaseLess);
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
                    float pawnHealth = 1f - this.Pawn.health?.summaryHealth?.SummaryHealthPercent ?? 1f;

                    Color color = Statics.Gradient4.Evaluate(pawnHealth);
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
            if (this._toxicBuildUpVisible > 0f)
            {
                Color color = Statics.Gradient4.Evaluate(this._toxicBuildUpVisible);
                if (barSettings.ShowToxicity)
                {
                    string tooltip = this._toxicTip;

                    barIconList.Add(
                        new IconEntryBar(Icon.Toxicity, color, tooltip));
                }

                if (psiSettings.ShowToxicity)
                {
                    string tooltip = this._toxicTip;

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
            if (GetThought(ThoughtDefOf.Naked, out this._feelsNaked, out this._nakedTip, out this._nakedMoodOffset))
            {
                Color moodOffsetColor = this._nakedMoodOffset.MoodOffsetColor();
                if (barSettings.ShowNaked)
                {
                    barIconList.Add(new IconEntryBar(Icon.Naked, moodOffsetColor, this._nakedTip));
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
            if (this._prostho != 0)
            {
                switch (this._prostho)
                {
                    case -1:
                        GetThought(
                            ThoughtDefOf.ProsthophobeUnhappy,
                            out this._prosthoUnhappy,
                            out this._prosthoTooltip,
                            out this._prosthoMoodOffset);
                        break;

                    case 1:
                        GetThought(
                            ThoughtDefOf.ProsthophileNoProsthetic,
                            out this._prosthoUnhappy,
                            out this._prosthoTooltip,
                            out this._prosthoMoodOffset);
                        break;

                    default: break;
                }
            }

            // Bed status
            if (this.Pawn.ownership.OwnedBed != null)
            {
                GetThought(
                    ThoughtDefOf.SharedBed,
                    out this._bedStatus,
                    out this._bedStatusTip,
                    out this._bedStatusMoodOffset);
            }
            else
            {
                this._bedStatus = 1;
                this._bedStatusTip = "NeedColonistBeds".Translate();
            }

            // Cabin Fever
            if (GetThought(
                ThoughtDefOf.NeedOutdoors,
                out this._needOutdoorsMoodLevel,
                out this._needOutdoorsTip,
                out this._needOutdoorsMoodOffset))
            {
                Color moodOffset = this._needOutdoorsMoodOffset.MoodOffsetColor();
                if (barSettings.ShowCabinFever)
                {
                    string tooltip = this._needOutdoorsTip;
                    barIconList.Add(new IconEntryBar(Icon.CabinFever, moodOffset, tooltip));
                }

                if (psiSettings.ShowCabinFever)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.CabinFever, moodOffset, viewOpacityCrit));
                }
            }

            // Pain
            if (GetThought(this._painThought, out this._painMoodLevel, out this._painTip, out this._painMoodOffset))
            {
                Color moodOffset = this._painMoodOffset.MoodOffsetColor();
                if (barSettings.ShowPain)
                {
                    barIconList.Add(new IconEntryBar(Icon.Pain, moodOffset, this._painTip));
                }

                if (psiSettings.ShowPain)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Pain, moodOffset, viewOpacityCrit));
                }
            }

            // Night Owl
            if (this._isNightOwl)
            {
                if (GetThought(
                    ThoughtDefOf.NightOwlDuringTheDay,
                    out this._nightOwlUnhappy,
                    out this._nightOwlTip,
                    out this._nightOwlMoodOffset))
                {
                    Color moodOffset = this._nightOwlMoodOffset.MoodOffsetColor();
                    if (barSettings.ShowNightOwl)
                    {
                        barIconList.Add(new IconEntryBar(Icon.NightOwl, moodOffset, this._nightOwlTip));
                    }

                    if (psiSettings.ShowNightOwl)
                    {
                        psiIconList.Add(new IconEntryPSI(Icon.NightOwl, moodOffset, viewOpacityCrit));
                    }
                }
            }

            // Greedy
            if (this._hasGreedyTrait)
            {
                if (GetThought(
                    ThoughtDefOf.Greedy,
                    out this._greedyThought,
                    out this._greedyTooltip,
                    out this._greedyMoodOffset))
                {
                    Color moodOffsetColor = this._greedyMoodOffset.MoodOffsetColor();
                    if (barSettings.ShowGreedy)
                    {
                        barIconList.Add(new IconEntryBar(Icon.Greedy, moodOffsetColor, this._greedyTooltip));
                    }

                    if (psiSettings.ShowGreedy)
                    {
                        psiIconList.Add(new IconEntryPSI(Icon.Greedy, moodOffsetColor, viewOpacity));
                    }
                }
            }

            // Jealous
            if (this._hasJealousTrait)
            {
                if (GetThought(
                    ThoughtDefOf.Jealous,
                    out this._jealousThought,
                    out this._jealousTooltip,
                    out this._jealousMoodOffset))
                {
                    Color moodOffset = this._jealousMoodOffset.MoodOffsetColor();
                    if (barSettings.ShowJealous)
                    {
                        barIconList.Add(new IconEntryBar(Icon.Jealous, moodOffset, this._jealousTooltip));
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
                out this._unburied,
                out this._unburiedTip,
                out this._unburiedMoodOffset))
            {
                Color moodOffset = this._unburiedMoodOffset.MoodOffsetColor();
                if (barSettings.ShowLeftUnburied)
                {
                    string tooltip = this._unburiedTip;
                    barIconList.Add(new IconEntryBar(Icon.LeftUnburied, moodOffset, tooltip));
                }

                if (psiSettings.ShowLeftUnburied)
                {
                    string tooltip = this._unburiedTip;
                    psiIconList.Add(new IconEntryPSI(Icon.LeftUnburied, moodOffset, viewOpacity));
                }
            }

            this._isAddict = false;
            this._withDrawal = false;
            this._withDrawalPercent = 0f;
            this._addictionLabel = null;
            if (hediffs != null)
            {
                for (int i = 0; i < hediffs.Count; i++)
                {
                    Hediff hediff = hediffs[i];
                    if (hediff is Hediff_Addiction)
                    {
                        this._isAddict = true;
                        this._withDrawalPercent = hediff.Severity;
                        this._withDrawal = hediff.CurStageIndex > 0;
                        if (this._addictionLabel.NullOrEmpty())
                        {
                            this._addictionLabel = hediff.LabelCap;
                        }
                        else
                        {
                            this._addictionLabel += "\n" + hediff.LabelCap;
                        }
                    }
                }
            }

            if (this._isAddict || this._drugDesire != 0)
            {
                Color color = new Color();
                string tooltip = null;
                if (this._isAddict)
                {
                    if (this._withDrawal)
                    {
                        this.GetWithdrawalColor(out color);
                    }
                    else
                    {
                        color = Textures.ColVermillion;
                    }

                    if (!this._drugUserLabel.NullOrEmpty())
                    {
                        tooltip = this._drugUserLabel + "\n" + this._addictionLabel;
                    }
                    else
                    {
                        tooltip = this._addictionLabel;
                    }
                }
                else
                {
                    switch (this._drugDesire)
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

                    tooltip = this._drugUserLabel;
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
                out this._wantsToHump,
                out this._humpTip,
                out this._humpMoodOffset);

            // Bed status
            if (this._wantsToHump > -1)
            {
                Color moodOffset = this._humpMoodOffset.MoodOffsetColor();
                if (barSettings.ShowBedroom)
                {
                    barIconList.Add(new IconEntryBar(Icon.Bedroom, moodOffset, this._humpTip));
                }

                if (psiSettings.ShowBedroom)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Bedroom, moodOffset, viewOpacityCrit));
                }
            }
            else if (this._bedStatus > -1)
            {
                Color moodOffset = this._bedStatusMoodOffset.MoodOffsetColor();
                if (barSettings.ShowBedroom)
                {
                    barIconList.Add(new IconEntryBar(Icon.Bedroom, moodOffset, this._bedStatusTip));
                }

                if (psiSettings.ShowBedroom)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Bedroom, moodOffset, viewOpacity));
                }

                // Moods caused by traits
                if (this._prosthoUnhappy > -1)
                {
                    Color offset = this._prosthoMoodOffset.MoodOffsetColor();

                    if (this._prostho == 1)
                    {
                        if (barSettings.ShowProsthophile)
                        {
                            barIconList.Add(new IconEntryBar(Icon.Prosthophile, offset, this._prosthoTooltip));
                        }

                        if (psiSettings.ShowProsthophile)
                        {
                            psiIconList.Add(new IconEntryPSI(Icon.Prosthophile, offset, viewOpacity));
                        }
                    }

                    if (this._prostho == -1)
                    {
                        if (barSettings.ShowProsthophobe)
                        {
                            barIconList.Add(new IconEntryBar(Icon.Prosthophobe, offset, this._prosthoTooltip));
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
            this.BarIconList = barIconList.OrderBy(x => x.Icon).ToList();
            this.PSIIconList = psiIconList.OrderBy(x => x.Icon).ToList();
        }

        /*
                public override void PostDraw()
                {
                    base.PostDraw();

                    SettingsPSI psiSettings = PSISettings;
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
                        if (Settings.PSISettings.IconsScreenScale)
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

                        float num2 = worldScale * (Settings.PSISettings.IconSizeMult * 0.5f);

                        // On Colonist
                        Rect iconRect = new Rect(
                            vectorAtBody.x,
                            vectorAtBody.y,
                            num2 * Settings.PSISettings.IconSize,
                            num2 * Settings.PSISettings.IconSize);
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