using System;
using System.Collections.Generic;
using System.Linq;
using ColonistBarKF.Bar;
using ColonistBarKF.PSI;
using ColonistBarKF.Settings;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

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

        private int _cabinFeverMoodLevel;

        private float _cabinFeverMoodOffset;

        private string _cabinFeverTip;

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

        public Pawn Pawn => parent as Pawn;

        [NotNull]
        public List<IconEntryBar> BarIconList { get; private set; } = new List<IconEntryBar>();

        public Color BgColor
        {
            get => _bgColor;

            set => _bgColor = value;
        }

        [NotNull]
        public List<IconEntryPSI> PSIIconList { get; private set; } = new List<IconEntryPSI>();

        public override void CompTick()
        {
            base.CompTick();

            if (Pawn.IsColonist || Pawn.IsPrisoner)
            {
                if (_entriesDirty)
                {
                    UpdateColonistStats();
                    _entriesDirty = false;
                }

                CheckStats();
            }
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref _bgColor, "bgColor");
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            Pawn p = parent as Pawn;
            if (p == null)
            {
                return;
            }

            if (p.Faction != Faction.OfPlayer)
            {
                CheckRelationWithColonists(p);
            }
        }

        public void SetEntriesDirty()
        {
            _entriesDirty = true;
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
            TargetPos = Vector3.zero;
            Job curJob = Pawn.CurJob;
            if (curJob != null)
            {
                JobDriver curDriver = Pawn.jobs.curDriver;
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

                if (curDriver is JobDriver_Hunt && Pawn.carryTracker?.CarriedThing != null)
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

                if (curJob.def == JobDefOf.LayDown && Pawn.InBed())
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
                    TargetPos = a + new Vector3(0f, 3f, 0f);
                }
            }
        }

        private void CheckRelationWithColonists(Pawn p)
        {
            bool skip = false;

            if (p.relations.FamilyByBlood.Any(x => x.Faction == Faction.OfPlayer))
            {
                HasRelationWithColonist = true;
                skip = true;
            }

            if (!skip)
            {
                if (p.relations.DirectRelations.Any(x => x.otherPawn.Faction == Faction.OfPlayer))
                {
                    HasRelationWithColonist = true;
                }
            }
        }

        private void CheckStats()
        {
            // if (Find.TickManager.CurTimeSpeed == TimeSpeed.Paused)
            // {
            // return;
            // }
            int nextUpdate = (int)(_lastStatUpdate + _nextStatUpdate * Find.TickManager.TickRateMultiplier);

            if (Find.TickManager.TicksGame <= nextUpdate)
            {
                return;
            }

            UpdateColonistStats();

            _lastStatUpdate = Find.TickManager.TicksGame;
            _nextStatUpdate = (int)Rand.Range(120f, 300f);

            // Log.Message(
            // "CBKF updated stat " + (this.parent as Pawn).Name + ", next update in " + NextStatUpdate * Find.TickManager.TickRateMultiplier
            // + " ticks.");
        }

        public void CheckTraits()
        {
            {
                if (Pawn.RaceProps.hasGenders)
                {
                    if (Pawn.Dead)
                    {
                        BgColor = Color.gray;
                    }
                    else
                    {
                        switch (Pawn.gender)
                        {
                            case Gender.Male:
                                BgColor = Textures.MaleColor;
                                break;

                            case Gender.Female:
                                BgColor = Textures.FemaleColor;
                                break;
                        }
                    }
                }

                // One time traits check
                if (Pawn.story?.traits != null)
                {
                    // Masochist
                    _isMasochist = Pawn.story.traits.HasTrait(TraitDef.Named("Masochist"));

                    // Masochist trait check
                    _painThought = ThoughtDef.Named(Pawn.story.traits.HasTrait(TraitDef.Named("Masochist")) ? "MasochistPain" : "Pain");

                    // Pacifist
                    IsPacifist = Pawn.story.WorkTagIsDisabled(WorkTags.Violent);

                    // Pyromaniac
                    _isPyromaniac = Pawn.story.traits.HasTrait(TraitDefOf.Pyromaniac);

                    // Prostho
                    if (Pawn.story.traits.HasTrait(TraitDefOf.Prosthophobe))
                    {
                        _prostho = -1;
                    }

                    if (Pawn.story.traits.HasTrait(TraitDef.Named("Prosthophile")))
                    {
                        _prostho = 1;
                    }

                    // Night Owl
                    if (Pawn.story.traits.HasTrait(TraitDef.Named("NightOwl")))
                    {
                        _isNightOwl = true;
                    }

                    // Jealous
                    if (Pawn.story.traits.HasTrait(TraitDef.Named("Jealous")))
                    {
                        _hasJealousTrait = true;
                    }

                    // Drug desire
                    if (Pawn.story.traits.HasTrait(TraitDefOf.DrugDesire))
                    {
                        _drugDesire = Pawn.story.traits.DegreeOfTrait(TraitDefOf.DrugDesire);
                        _drugUserLabel = Pawn.story.traits.GetTrait(TraitDefOf.DrugDesire).LabelCap;
                    }

                    // Greedy
                    if (Pawn.story.traits.HasTrait(TraitDefOf.Greedy))
                    {
                        _hasGreedyTrait = true;
                    }

                    _traitsCheck = true;
                }
            }
        }

        private void GetWithdrawalColor(out Color color)
        {
            color = Color.Lerp(Textures.ColBlueishGreen, Textures.ColVermillion, _withDrawalPercent);
        }

        private void UpdateColonistStats()
        {
            List<IconEntryPSI> psiIconList = new List<IconEntryPSI>();
            List<IconEntryBar> barIconList = new List<IconEntryBar>();
            SettingsColonistBar barSettings = Settings.Settings.BarSettings;
            SettingsPSI psiSettings = Settings.Settings.PSISettings;

            if (barSettings == null || psiSettings == null)
            {
                GameComponentPSI.Reinit(true, false, false);
                return;
            }

            if (!_traitsCheck)
            {
                CheckTraits();
            }

            if (!Pawn.Spawned || Pawn.Map == null)
            {
                return;
            }

            if (Current.ProgramState != ProgramState.Playing)
            {
                return;
            }

            Pawn_NeedsTracker needs = Pawn.needs;
            if (needs?.mood != null)
            {
                Mb = Pawn.mindState?.mentalBreaker;
                Mood = needs.mood;
            }
            else
            {
                Mood = null;
                Mb = null;
            }

            if (!barSettings.UsePsi && !psiSettings.UsePsi)
            {
                return;
            }

            // target
            CheckJobs();
            float viewOpacity = psiSettings.IconOpacity;
            float viewOpacityCrit = Settings.Settings.ViewOpacityCrit;

            // Mental Breaker for MoodBars
            if (needs != null)
            {
                needs.mood?.thoughts?.GetDistinctMoodThoughtGroups(Thoughts);

                // Hungry
                if (needs.food != null)
                {
                    if ((double)needs.food?.CurLevel < Settings.Settings.PSISettings.LimitFoodLess)
                    {
                        Color color =
                            Statics.GradientRedAlertToNeutral.Evaluate(
                                needs.food.CurLevel / Settings.Settings.PSISettings.LimitFoodLess);
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
                    if (needs.rest.CurLevel < (double)Settings.Settings.PSISettings.LimitRestLess)
                    {
                        Color color =
                            Statics.GradientRedAlertToNeutral.Evaluate(
                                needs.rest.CurLevel / Settings.Settings.PSISettings.LimitRestLess);

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
            if ((Pawn.mindState != null) && Pawn.mindState.IsIdle)
            {
                if (psiSettings.ShowIdle && GenDate.DaysPassed >= 0.1)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Idle, Textures.ColorNeutralStatus, viewOpacity));
                }
            }

            if (IsPacifist)
            {
                if (barSettings.ShowPacific)
                {
                    barIconList.Add(
                        new IconEntryBar(
                            Icon.Pacific,
                            Textures.ColBlueishGreen,
                            "IsIncapableOfViolence".Translate(Pawn.NameStringShort)));
                }

                if (psiSettings.ShowPacific)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Pacific, Textures.ColBlueishGreen, viewOpacity));
                }
            }

            if (Pawn.equipment?.Primary == null && !Pawn.IsPrisoner && !IsPacifist)
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
            if (_isPyromaniac)
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
            float temperatureForCell = GenTemperature.GetTemperatureForCell(Pawn.Position, Pawn.Map);

            _tooCold = (float)((Pawn.ComfortableTemperatureRange().min
                                    - (double)Settings.Settings.PSISettings.LimitTempComfortOffset - temperatureForCell) / 10f);

            _tooHot = (float)((temperatureForCell - (double)Pawn.ComfortableTemperatureRange().max
                                   - Settings.Settings.PSISettings.LimitTempComfortOffset) / 10f);

            _tooCold = Mathf.Clamp(_tooCold, 0f, 2f);

            _tooHot = Mathf.Clamp(_tooHot, 0f, 2f);

            // Too Cold & too hot
            if (_tooCold > 0f)
            {
                Color color = Statics.Gradient4.Evaluate(_tooCold / 2);
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

            if (_tooHot > 0f)
            {
                Color color = Statics.Gradient4.Evaluate(_tooHot / 2);
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
            _mentalSanity = null;
            if (Pawn.mindState != null && Pawn.InMentalState)
            {
                _mentalSanity = Pawn.MentalStateDef;
            }

            if (_mentalSanity != null)
            {
                if (Pawn.InAggroMentalState)
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
                    if (_mentalSanity == MentalStateDefOf.PanicFlee)
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
                    if (_mentalSanity == MentalStateDefOf.WanderSad)
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
                    if (_mentalSanity == MentalStateDefOf.PanicFlee)
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
            _diseaseDisappearance = 1f;
            _healthDisease = 1f;

            // Drug addiction
            List<Hediff> hediffs = null;

            // Sick thoughts
            Pawn_HealthTracker health = Pawn.health;
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
                            efficiencyTotal / Settings.Settings.PSISettings.LimitEfficiencyLess);

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
                    _bleedRate = Mathf.Clamp01(
                        health.hediffSet.BleedRateTotal * Settings.Settings.PSISettings.LimitBleedMult);

                    if (_bleedRate > 0.0f)
                    {
                        Color color = Statics.GradientRedAlertToNeutral.Evaluate(1.0f - _bleedRate);
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

                    if (HealthAIUtility.ShouldBeTendedNowUrgent(Pawn))
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
                    else if (HealthAIUtility.ShouldBeTendedNow(Pawn))
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

                    if (HealthAIUtility.ShouldHaveSurgeryDoneNow(Pawn))
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

                    if ((health?.hediffSet?.AnyHediffMakesSickThought ?? false) && !Pawn.Destroyed
                        && Pawn.playerSettings.medCare >= 0)
                    {
                        if (hediffs != null)
                        {
                            _severity = 0f;
                            _immunity = 0f;
                            foreach (Hediff hediff in hediffs)
                            {
                                if (!hediff.Visible || hediff.IsOld() || !hediff.def.makesSickThought
                                    || hediff.LabelCap.NullOrEmpty() || hediff.SeverityLabel.NullOrEmpty())
                                {
                                    continue;
                                }

                                _toxicBuildUpVisible = 0;
                                _healthTip = hediff.LabelCap;
                                if (!Thoughts.NullOrEmpty())
                                {
                                    GetThought(
                                        ThoughtDefOf.Sick,
                                        out int dummy,
                                        out _sickTip,
                                        out _sickMoodOffset);
                                }

                                // this.ToxicBuildUpVisible
                                if (hediff.def == HediffDefOf.ToxicBuildup)
                                {
                                    _toxicTip = hediff.LabelCap + "\n" + hediff.SeverityLabel;
                                    _toxicBuildUpVisible = Mathf.InverseLerp(0.049f, 1f, hediff.Severity);
                                    continue;
                                }

                                HediffComp_Immunizable compImmunizable = hediff.TryGetComp<HediffComp_Immunizable>();
                                if (compImmunizable != null)
                                {
                                    _severity = Mathf.Max(_severity, hediff.Severity);
                                    _immunity = compImmunizable.Immunity;
                                    float basehealth = _healthDisease - (_severity - _immunity / 4) - 0.25f;
                                    _healthDisease = basehealth;
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

                                if (_diseaseDisappearance > compImmunizable.Immunity)
                                {
                                    _diseaseDisappearance = compImmunizable.Immunity;
                                }

                                // break;
                            }
                        }

                        if (_diseaseDisappearance < Settings.Settings.PSISettings.LimitDiseaseLess)
                        {
                            string tooltip = _sickTip + "\n" + _healthTip + "\n" + "Immunity".Translate()
                                             + " / " + "PSI.DiseaseProgress".Translate() + ": \n"
                                             + _immunity.ToStringPercent() + " / " + _severity.ToStringPercent()
                                             + ": \n" + _sickMoodOffset;

                            Color color =
                                Statics.Gradient4.Evaluate(_diseaseDisappearance / psiSettings.LimitDiseaseLess);
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
                    float pawnHealth = 1f - Pawn.health?.summaryHealth?.SummaryHealthPercent ?? 1f;

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
            if (_toxicBuildUpVisible > 0f)
            {
                Color color = Statics.Gradient4.Evaluate(_toxicBuildUpVisible);
                if (barSettings.ShowToxicity)
                {
                    string tooltip = _toxicTip;

                    barIconList.Add(
                        new IconEntryBar(Icon.Toxicity, color, tooltip));
                }

                if (psiSettings.ShowToxicity)
                {
                    string tooltip = _toxicTip;

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
            if (GetThought(ThoughtDefOf.Naked, out _feelsNaked, out _nakedTip, out _nakedMoodOffset))
            {
                Color moodOffsetColor = _nakedMoodOffset.MoodOffsetColor();
                if (barSettings.ShowNaked)
                {
                    barIconList.Add(new IconEntryBar(Icon.Naked, moodOffsetColor, _nakedTip));
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
            if (_prostho != 0)
            {
                switch (_prostho)
                {
                    case -1:
                        GetThought(
                            ThoughtDefOf.ProsthophobeUnhappy,
                            out _prosthoUnhappy,
                            out _prosthoTooltip,
                            out _prosthoMoodOffset);
                        break;

                    case 1:
                        GetThought(
                            ThoughtDefOf.ProsthophileNoProsthetic,
                            out _prosthoUnhappy,
                            out _prosthoTooltip,
                            out _prosthoMoodOffset);
                        break;

                    default: break;
                }
            }

            // Bed status
            if (Pawn.ownership.OwnedBed != null)
            {
                GetThought(
                    ThoughtDefOf.SharedBed,
                    out _bedStatus,
                    out _bedStatusTip,
                    out _bedStatusMoodOffset);
            }
            else
            {
                _bedStatus = 1;
                _bedStatusTip = "NeedColonistBeds".Translate();
            }

            // Cabin Fever
            if (GetThought(
                ThoughtDefOf.CabinFever,
                out _cabinFeverMoodLevel,
                out _cabinFeverTip,
                out _cabinFeverMoodOffset))
            {
                Color moodOffset = _cabinFeverMoodOffset.MoodOffsetColor();
                if (barSettings.ShowCabinFever)
                {
                    string tooltip = _cabinFeverTip;
                    barIconList.Add(new IconEntryBar(Icon.CabinFever, moodOffset, tooltip));
                }

                if (psiSettings.ShowCabinFever)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.CabinFever, moodOffset, viewOpacityCrit));
                }
            }

            // Pain
            if (GetThought(_painThought, out _painMoodLevel, out _painTip, out _painMoodOffset))
            {
                Color moodOffset = _painMoodOffset.MoodOffsetColor();
                if (barSettings.ShowPain)
                {
                    barIconList.Add(new IconEntryBar(Icon.Pain, moodOffset, _painTip));
                }

                if (psiSettings.ShowPain)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Pain, moodOffset, viewOpacityCrit));
                }
            }

            // Night Owl
            if (_isNightOwl)
            {
                if (GetThought(
                    ThoughtDefOf.NightOwlDuringTheDay,
                    out _nightOwlUnhappy,
                    out _nightOwlTip,
                    out _nightOwlMoodOffset))
                {
                    Color moodOffset = _nightOwlMoodOffset.MoodOffsetColor();
                    if (barSettings.ShowNightOwl)
                    {
                        barIconList.Add(new IconEntryBar(Icon.NightOwl, moodOffset, _nightOwlTip));
                    }

                    if (psiSettings.ShowNightOwl)
                    {
                        psiIconList.Add(new IconEntryPSI(Icon.NightOwl, moodOffset, viewOpacityCrit));
                    }
                }
            }

            // Greedy
            if (_hasGreedyTrait)
            {
                if (GetThought(
                    ThoughtDefOf.Greedy,
                    out _greedyThought,
                    out _greedyTooltip,
                    out _greedyMoodOffset))
                {
                    Color moodOffsetColor = _greedyMoodOffset.MoodOffsetColor();
                    if (barSettings.ShowGreedy)
                    {
                        barIconList.Add(new IconEntryBar(Icon.Greedy, moodOffsetColor, _greedyTooltip));
                    }

                    if (psiSettings.ShowGreedy)
                    {
                        psiIconList.Add(new IconEntryPSI(Icon.Greedy, moodOffsetColor, viewOpacity));
                    }
                }
            }

            // Jealous
            if (_hasJealousTrait)
            {
                if (GetThought(
                    ThoughtDefOf.Jealous,
                    out _jealousThought,
                    out _jealousTooltip,
                    out _jealousMoodOffset))
                {
                    Color moodOffset = _jealousMoodOffset.MoodOffsetColor();
                    if (barSettings.ShowJealous)
                    {
                        barIconList.Add(new IconEntryBar(Icon.Jealous, moodOffset, _jealousTooltip));
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
                out _unburied,
                out _unburiedTip,
                out _unburiedMoodOffset))
            {
                Color moodOffset = _unburiedMoodOffset.MoodOffsetColor();
                if (barSettings.ShowLeftUnburied)
                {
                    string tooltip = _unburiedTip;
                    barIconList.Add(new IconEntryBar(Icon.LeftUnburied, moodOffset, tooltip));
                }

                if (psiSettings.ShowLeftUnburied)
                {
                    string tooltip = _unburiedTip;
                    psiIconList.Add(new IconEntryPSI(Icon.LeftUnburied, moodOffset, viewOpacity));
                }
            }

            _isAddict = false;
            _withDrawal = false;
            _withDrawalPercent = 0f;
            _addictionLabel = null;
            if (hediffs != null)
            {
                for (int i = 0; i < hediffs.Count; i++)
                {
                    Hediff hediff = hediffs[i];
                    if (hediff is Hediff_Addiction)
                    {
                        _isAddict = true;
                        _withDrawalPercent = hediff.Severity;
                        _withDrawal = hediff.CurStageIndex > 0;
                        if (_addictionLabel.NullOrEmpty())
                        {
                            _addictionLabel = hediff.LabelCap;
                        }
                        else
                        {
                            _addictionLabel += "\n" + hediff.LabelCap;
                        }
                    }
                }
            }

            if (_isAddict || _drugDesire != 0)
            {
                Color color = new Color();
                string tooltip = null;
                if (_isAddict)
                {
                    if (_withDrawal)
                    {
                        GetWithdrawalColor(out color);
                    }
                    else
                    {
                        color = Textures.ColVermillion;
                    }

                    if (!_drugUserLabel.NullOrEmpty())
                    {
                        tooltip = _drugUserLabel + "\n" + _addictionLabel;
                    }
                    else
                    {
                        tooltip = _addictionLabel;
                    }
                }
                else
                {
                    switch (_drugDesire)
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

                    tooltip = _drugUserLabel;
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
                out _wantsToHump,
                out _humpTip,
                out _humpMoodOffset);

            // Bed status
            if (_wantsToHump > -1)
            {
                Color moodOffset = _humpMoodOffset.MoodOffsetColor();
                if (barSettings.ShowBedroom)
                {
                    barIconList.Add(new IconEntryBar(Icon.Bedroom, moodOffset, _humpTip));
                }

                if (psiSettings.ShowBedroom)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Bedroom, moodOffset, viewOpacityCrit));
                }
            }
            else if (_bedStatus > -1)
            {
                Color moodOffset = _bedStatusMoodOffset.MoodOffsetColor();
                if (barSettings.ShowBedroom)
                {
                    barIconList.Add(new IconEntryBar(Icon.Bedroom, moodOffset, _bedStatusTip));
                }

                if (psiSettings.ShowBedroom)
                {
                    psiIconList.Add(new IconEntryPSI(Icon.Bedroom, moodOffset, viewOpacity));
                }

                // Moods caused by traits
                if (_prosthoUnhappy > -1)
                {
                    Color offset = _prosthoMoodOffset.MoodOffsetColor();

                    if (_prostho == 1)
                    {
                        if (barSettings.ShowProsthophile)
                        {
                            barIconList.Add(new IconEntryBar(Icon.Prosthophile, offset, _prosthoTooltip));
                        }

                        if (psiSettings.ShowProsthophile)
                        {
                            psiIconList.Add(new IconEntryPSI(Icon.Prosthophile, offset, viewOpacity));
                        }
                    }

                    if (_prostho == -1)
                    {
                        if (barSettings.ShowProsthophobe)
                        {
                            barIconList.Add(new IconEntryBar(Icon.Prosthophobe, offset, _prosthoTooltip));
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
            BarIconList = barIconList.OrderBy(x => x.Icon).ToList();
            PSIIconList = psiIconList.OrderBy(x => x.Icon).ToList();
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