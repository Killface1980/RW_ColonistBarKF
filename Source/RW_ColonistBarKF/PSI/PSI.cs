using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using static ColonistBarKF.PSI.PSIDrawer;
using static Verse.ColonistBarTextures;


namespace ColonistBarKF.PSI
{

    public class PSI : GameComponent
    {
        public PSI()
        {
            //
        }

        public PSI(Game game)
        {
            //
        }

        private double _fDelta;


        private static float _worldScale = 1f;

        public static string[] IconSets = { "default" };

        public static Materials PSIMaterials = new Materials();

        private static PawnCapacityDef[] _pawnCapacities;

        private static PawnCapacityDef[] array;

        public static Vector3[] IconPosVectorsPsi;
        public static Vector3[] IconPosRectsBar;
        private CellRect _viewRect;

        public override void FinalizeInit()
        {
            Reinit();
            UpdateDictionary();
            _fDelta = 0;
        }

        public override void GameComponentOnGUI()
        {
            if (Current.ProgramState != ProgramState.Playing)
                return;

            if (WorldRendererUtility.WorldRenderedNow)
            {
                return;
            }

            if (!Settings.PsiSettings.UsePsi && !Settings.PsiSettings.UsePsiOnPrisoner)
                return;

            _viewRect = Find.CameraDriver.CurrentViewRect;
            _viewRect = _viewRect.ExpandedBy(5);

            foreach (Pawn pawn in Find.VisibleMap.mapPawns.AllPawnsSpawned)
            {
                if (!_viewRect.Contains(pawn.Position))
                    continue;
                //     if (useGUILayout)
                {
                    if (pawn.RaceProps.Animal)
                        DrawAnimalIcons(pawn);
                    else if (((Settings.PsiSettings.UsePsi && pawn.IsColonist) || (Settings.PsiSettings.UsePsiOnPrisoner && pawn.IsPrisoner)))
                    {
                        DrawColonistIcons(pawn, true);
                    }
                }
            }
        }

        public override void GameComponentUpdate()
        {
            if (Input.GetKeyUp(KeyCode.F11))
            {
                Settings.PsiSettings.UsePsi = !Settings.PsiSettings.UsePsi;
                Settings.ColBarSettings.UsePsi = !Settings.ColBarSettings.UsePsi;
                Messages.Message(Settings.PsiSettings.UsePsi ? "PSI.Enabled".Translate() : "PSI.Disabled".Translate(), MessageSound.Standard);
            }
            _worldScale = Screen.height / (2f * Camera.current.orthographicSize);
        }

        public override void GameComponentTick()
        {
            // Scans the map for new pawns

            if (Current.ProgramState != ProgramState.Playing)
                return;

            if (!Settings.ColBarSettings.UsePsi && !Settings.PsiSettings.UsePsi)
                return;

            _fDelta += Time.fixedDeltaTime;
            if (_fDelta < 5)
                return;
            _fDelta = 0.0;

            UpdateDictionary();
        }

        #region Icon Drawing 

        public static void Reinit(bool reloadSettings = true, bool reloadIconSet = true, bool recalcIconPos = true)
        {
            _pawnCapacities = new[]
            {
                PawnCapacityDefOf.BloodFiltration,
                PawnCapacityDefOf.BloodPumping,
                PawnCapacityDefOf.Breathing,
                PawnCapacityDefOf.Consciousness,
                PawnCapacityDefOf.Eating,
                PawnCapacityDefOf.Hearing,
                PawnCapacityDefOf.Manipulation,
                PawnCapacityDefOf.Metabolism,
                PawnCapacityDefOf.Moving,
                PawnCapacityDefOf.Sight,
                PawnCapacityDefOf.Talking
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
                LongEventHandler.ExecuteWhenFinished(() =>
                {
                    PSIMaterials = new Materials(Settings.PsiSettings.IconSet);
                    //PSISettings SettingsPSI =
                    //    XmlLoader.ItemFromXmlFile<PSISettings>(GenFilePaths.CoreModsFolderPath + "/RW_PawnStateIcons/Textures/UI/Overlays/PawnStateIcons/" + PSI.SettingsPSI.IconSet + "/iconset.cfg");
                    //PSI.PsiSettings.IconSizeMult = SettingsPSI.IconSizeMult;
                    PSIMaterials.ReloadTextures(true);
                    //   Log.Message(GenFilePaths.CoreModsFolderPath + "/RW_PawnStateIcons/Textures/UI/Overlays/PawnStateIcons/" + ColBarSettings.IconSet + "/iconset.cfg");
                });
            }

        }

        private static void RecalcIconPositionsPSI()
        {
            SettingsPSI psiSettings = Settings.PsiSettings;
            //            _iconPosVectors = new Vector3[18];
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
                            (-0.600000023841858 * psiSettings.IconMarginX -
                             0.550000011920929 * psiSettings.IconSize * psiSettings.IconOffsetX * num1), 3f,
                        (float)
                            (-0.600000023841858 * psiSettings.IconMarginY +
                             0.550000011920929 * psiSettings.IconSize * psiSettings.IconOffsetY * num2));

            }
        }

        private static void RecalcBarPositionAndSize()
        {
            SettingsColonistBar settings = Settings.ColBarSettings;
            IconPosRectsBar = new Vector3[40];
            for (int index = 0; index < IconPosRectsBar.Length; ++index)
            {
                int num1;
                int num2;
                num1 = index / (settings.IconsInColumn);
                num2 = index % (settings.IconsInColumn);
                if (settings.IconsHorizontal)
                {
                    int num3 = num1;
                    num1 = num2;
                    num2 = num3;

                    //   num2 = index / ColBarSettings.IconsInColumn;
                    //   num1 = index % ColBarSettings.IconsInColumn;

                }

                IconPosRectsBar[index] =
                    new Vector3(
                        -num1,
                        3f,
                        num2);
            }
        }

        #endregion

        #region Status + Updates

        private static void UpdateDictionary()
        {
            foreach (Pawn pawn in PawnsFinder.AllMaps_FreeColonistsAndPrisonersSpawned)
            {
                if (pawn.Dead || pawn.DestroyedOrNull() || !pawn.Name.IsValid || pawn.Name == null)
                    continue;

                if (Settings.StatsDict.ContainsKey(pawn))
                    continue;

                try
                {
                    UpdateColonistStats(pawn);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                    //Log.Notify_Exception(ex);
                }
            }
        }

        private static void UpdateColonistStats(Pawn pawn)
        {
            if (pawn == null)
            {
                return;
            }

            if (!Settings.StatsDict.ContainsKey(pawn))
            {
                Settings.StatsDict.Add(pawn, new PawnStats());
            }

            List<Thought> thoughts = new List<Thought>();

            PawnStats pawnStats = Settings.StatsDict[pawn];
            pawn.needs?.mood?.thoughts?.GetDistinctMoodThoughtGroups(thoughts);
            pawnStats.Thoughts = thoughts;
            pawnStats.pawnHealth = pawn.health.summaryHealth.SummaryHealthPercent;


            // One time traits check
            if (!pawnStats.traitsCheck)
            {
                if (pawn.story?.traits != null)
                {
                    if (pawn.RaceProps.hasGenders)
                        switch (pawn.gender)
                        {
                            case Gender.Male:
                                pawnStats.BGColor = MaleColor;
                                break;
                            case Gender.Female:
                                pawnStats.BGColor = FemaleColor;
                                break;
                            default:
                                break;
                        }
                    // Masochist 
                    pawnStats.isMasochist = pawn.story.traits.HasTrait(TraitDef.Named("Masochist"));

                    // Masochist trait check
                    pawnStats.painThought = ThoughtDef.Named(pawn.story.traits.HasTrait(TraitDef.Named("Masochist"))
                        ? "MasochistPain" : "Pain");

                    // Pacifist 
                    pawnStats.isPacifist = pawn.story.WorkTagIsDisabled(WorkTags.Violent);

                    //Pyromaniac
                    pawnStats.isPyromaniac = pawn.story.traits.HasTrait(TraitDefOf.Pyromaniac);

                    // Prostho
                    if (pawn.story.traits.HasTrait(TraitDefOf.Prosthophobe))
                        pawnStats.prostho = -1;
                    if (pawn.story.traits.HasTrait(TraitDef.Named("Prosthophile")))
                        pawnStats.prostho = 1;

                    // Night Owl
                    if (pawn.story.traits.HasTrait(TraitDef.Named("NightOwl")))
                        pawnStats.isNightOwl = true;

                    // Jealous
                    if (pawn.story.traits.HasTrait(TraitDef.Named("Jealous")))
                        pawnStats.jealous = true;

                    // Drug desire
                    if (pawn.story.traits.HasTrait(TraitDefOf.DrugDesire))
                    {
                        pawnStats.drugDesire = pawn.story.traits.DegreeOfTrait(TraitDefOf.DrugDesire);
                        pawnStats.drugUserLabel = pawn.story.traits.GetTrait(TraitDefOf.DrugDesire).LabelCap;
                    }

                    //Greedy
                    if (pawn.story.traits.HasTrait(TraitDefOf.Greedy))
                        pawnStats.greedy = true;

                    pawnStats.traitsCheck = true;
                }

            }

            if (pawn.Dead) pawnStats.BGColor = Color.gray;



            // efficiency
            float efficiency = 10f;

            array = _pawnCapacities;
            foreach (PawnCapacityDef pawnCapacityDef in array)
            {
                if (pawnCapacityDef != PawnCapacityDefOf.Consciousness)
                {
                    efficiency = Math.Min(efficiency, pawn.health.capacities.GetLevel(pawnCapacityDef));
                }
                if (efficiency < 0f)
                    efficiency = 0f;
            }

            pawnStats.TotalEfficiency = efficiency;



            //target
            pawnStats.TargetPos = Vector3.zero;

            if (pawn.jobs.curJob != null)
            {
                JobDriver curDriver = pawn.jobs.curDriver;
                Job curJob = pawn.jobs.curJob;
                LocalTargetInfo targetInfo = curJob.targetA;
                if (curDriver is JobDriver_HaulToContainer || curDriver is JobDriver_HaulToCell ||
                    curDriver is JobDriver_FoodDeliver || curDriver is JobDriver_FoodFeedPatient ||
                    curDriver is JobDriver_TakeToBed || curDriver is JobDriver_TakeBeerOutOfFermentingBarrel)
                {
                    targetInfo = curJob.targetB;
                }
                JobDriver_DoBill bill = curDriver as JobDriver_DoBill;
                if (bill != null)
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

            pawnStats.TooCold =
                (float)
                    ((pawn.ComfortableTemperatureRange().min - (double)Settings.PsiSettings.LimitTempComfortOffset -
                      temperatureForCell) / 10f);

            pawnStats.TooHot =
                (float)
                    ((temperatureForCell - (double)pawn.ComfortableTemperatureRange().max -
                      Settings.PsiSettings.LimitTempComfortOffset) / 10f);

            pawnStats.TooCold = Mathf.Clamp(pawnStats.TooCold, 0f, 2f);

            pawnStats.TooHot = Mathf.Clamp(pawnStats.TooHot, 0f, 2f);
            /*
            // Drunkness - DEACTIVATED FOR NOW
            pawnStats.Drunkness =  DrugUtility.DrunknessPercent(pawn);
        */
            // Mental Sanity
            pawnStats.MentalSanity = null;
            if (pawn.mindState != null && pawn.InMentalState)
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
                pawnStats.BleedRate = Mathf.Clamp01(pawn.health.hediffSet.BleedRateTotal * Settings.PsiSettings.LimitBleedMult);
            }


            if (pawnStats.IsSick && !pawn.Destroyed && pawn.playerSettings.medCare >= 0)
            {
                if (hediffs != null)
                {
                    foreach (Hediff hediff in hediffs)
                    {
                        if (!hediff.Visible) continue;

                        pawnStats.ToxicBuildUp = 0;

                        //pawnStats.ToxicBuildUp
                        if (hediff.def == HediffDefOf.ToxicBuildup)
                        {
                            pawnStats.ToxicBuildUp = hediff.Severity;
                        }
                        HediffComp_Immunizable compImmunizable = hediff.TryGetComp<HediffComp_Immunizable>();
                        if (compImmunizable != null)
                        {
                            pawnStats.severity = hediff.Severity;
                            pawnStats.immunity = compImmunizable.Immunity;
                            float basehealth = pawnStats.HealthDisease - (pawnStats.severity - pawnStats.immunity / 4) - 0.25f;
                            pawnStats.HasLifeThreateningDisease = true;
                            pawnStats.HealthDisease = basehealth;
                            int dummy;
                            GetThought(thoughts, ThoughtDef.Named("Sick"), out dummy, out pawnStats.sickTip);
                        }
                        else
                        {
                            continue;
                        }



                        if (!hediff.def.PossibleToDevelopImmunityNaturally()) continue;

                        if (hediff.CurStage?.capMods == null) continue;

                        if (!hediff.CurStage.everVisible) continue;

                        if (hediff.FullyImmune()) continue;

                        // if (hediff.def.naturallyHealed) continue;

                        if (!hediff.def.makesSickThought) continue;

                        if (!hediff.def.tendable) continue;

                        if (Math.Abs(pawn.health.immunity.GetImmunity(hediff.def) - 1.0) < 0.05) continue;

                        //


                        if (pawnStats.DiseaseDisappearance > compImmunizable.Immunity)
                        {
                            pawnStats.DiseaseDisappearance = compImmunizable.Immunity;
                        }
                    }
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


            if (thoughts != null)
            {
                if (pawnStats.prostho != 0)
                    switch (pawnStats.prostho)
                    {
                        case -1:
                            pawnStats.prosthoWant = HasThought(thoughts, ThoughtDef.Named("ProsthophobeUnhappy")) ? -1 : 0;
                            break;
                        case 1:
                            pawnStats.prosthoWant = HasThought(thoughts, ThoughtDef.Named("ProsthophileNoProsthetic")) ? 1 : 0;
                            break;
                        default:
                            break;
                    }
                // Bed status
                if (pawn.ownership.OwnedBed != null)
                {
                    GetThought(thoughts, ThoughtDef.Named("SharedBed"), out pawnStats.BedStatus, out pawnStats.BedStatusTip);
                }
                else
                {
                    pawnStats.BedStatus = 1;
                    pawnStats.BedStatusTip = "NeedColonistBeds".Translate();
                }

                // Humping
                GetThought(thoughts, ThoughtDef.Named("WantToSleepWithSpouseOrLover"), out pawnStats.wantsToHump, out pawnStats.humpTip);

                // Cabin Fever
                GetThought(thoughts, ThoughtDef.Named("CabinFever"), out pawnStats.CabinFeverMoodLevel, out pawnStats.cabinFeverTip);

                //Pain
                GetThought(thoughts, pawnStats.painThought, out pawnStats.PainMoodLevel, out pawnStats.painTip);
                pawnStats.PainMoodLevel = HasThought(thoughts, pawnStats.painThought)
                    ? pawnStats.painThought.Worker.CurrentState(pawn).StageIndex
                    : -1;


                //Naked
                GetThought(thoughts, ThoughtDefOf.Naked, out pawnStats.feelsNaked, out pawnStats.nakedTip);

                // Night Owl
                if (pawnStats.isNightOwl)
                    GetThought(thoughts, ThoughtDef.Named("NightOwlDuringTheDay"), out pawnStats.nightOwlUnhappy, out pawnStats.nightOwlTip);

                // Greedy
                if (pawnStats.greedy)
                    pawnStats.greedyThought = HasThought(thoughts, ThoughtDef.Named("Greedy"));

                // Jealous
                if (pawnStats.jealous)
                    pawnStats.jealousThought = HasThought(thoughts, ThoughtDef.Named("Jealous"));

                // Unburied
                GetThought(thoughts, ThoughtDef.Named("ColonistLeftUnburied"), out pawnStats.unburied, out pawnStats.unburiedTip);

            }


            pawnStats.isAddict = false;
            pawnStats.withDrawal = false;
            pawnStats.withDrawalPercent = 0f;
            pawnStats.addictionLabel = null;

            if (hediffs != null)
                foreach (Hediff hediff in hediffs)
                {
                    if (hediff is Hediff_Addiction)
                    {
                        pawnStats.isAddict = true;
                        pawnStats.withDrawalPercent = hediff.Severity;
                        pawnStats.withDrawal = hediff.CurStageIndex > 0;
                        if (pawnStats.addictionLabel.NullOrEmpty())
                            pawnStats.addictionLabel = hediff.LabelCap;
                        else
                            pawnStats.addictionLabel += "\n" + hediff.LabelCap;
                    }
                }

            Settings.StatsDict[pawn] = pawnStats;
        }

        private static bool HasThought(List<Thought> thoughts, ThoughtDef tdef)
        {
            return thoughts.Any(thought => thought.def == tdef);
        }
        private static void GetThought(List<Thought> thoughts, ThoughtDef tdef, out int stage, out string tooltip)
        {
            tooltip = null;
            stage = -1;
            foreach (Thought thought in thoughts)
            {
                if (thought.def != tdef)
                    continue;

                stage = thought.CurStageIndex;
                tooltip = thought.CurStage.description + "\n" + thought.LabelCap;
                break;

            }
        }
        #endregion

        #region Draw Icons

        private static float ViewOpacityCrit
        {
            get
            {
                return Mathf.Max(Settings.PsiSettings.IconOpacityCritical,
                       Settings.PsiSettings.IconOpacity);
            }
        }

        public static float WorldScale => _worldScale;

        public static void DrawAnimalIcons(Pawn animal)
        {
            if (!animal.InAggroMentalState)
                return;
            if (!Settings.PsiSettings.ShowAggressive)
                return;
            if (!animal.Spawned)
                return;

            Vector3 drawPos = animal.DrawPos;
            Vector3 bodyPos = drawPos;
            int num = 0;
            DrawIconOnColonist(bodyPos, ref num, Icons.Aggressive, ColorRedAlert, ViewOpacityCrit);
        }


        public static void DrawColonistIcons(Pawn pawn, bool psi, float rectalpha = 1f, Rect psiRect = new Rect())
        {

            PawnStats pawnStats;
            if (pawn.Dead || !pawn.Spawned || pawn.holdingOwner == null || !Settings.StatsDict.TryGetValue(pawn, out pawnStats))
                return;

            pawnStats.LastStatUpdate += Time.fixedDeltaTime;
            if (pawnStats.LastStatUpdate > Rand.Range(1, 3))
            {
                UpdateColonistStats(pawn);
                pawnStats.LastStatUpdate = 0.0;
            }
            if (psi)
            {
                DrawColonistIconsPsi(pawn, pawnStats);
            }
            else
            {
                DrawColonistIconsBar(pawn, pawnStats, psiRect, rectalpha);
            }
        }

        public static void DrawColonistIconsPsi(Pawn pawn, PawnStats pawnStats)
        {
            SettingsPSI psiSettings = Settings.PsiSettings;
            float viewOpacity = psiSettings.IconOpacity;

            int iconNum = 0;

            Vector3 bodyLoc = pawn.DrawPos;

            // Target Point 
            if (psiSettings.ShowTargetPoint && (pawnStats.TargetPos != Vector3.zero))
            {
                if (psiSettings.UseColoredTarget)
                {
                    Color skinColor = pawn.story.SkinColor;
                    Color hairColor = pawn.story.hairColor;

                    Material skinMat = PSIMaterials[Icons.TargetSkin];
                    Material hairMat = PSIMaterials[Icons.TargetHair];

                    if (skinMat == null)
                        return;

                    if (hairMat == null)
                        return;

                    DrawIcon_posOffset(pawnStats.TargetPos, Vector3.zero, skinMat, skinColor, 1f);
                    DrawIcon_posOffset(pawnStats.TargetPos, Vector3.zero, hairMat, hairColor, 1f);

                }
                else
                {
                    Material targetMat = PSIMaterials[Icons.Target];
                    if (targetMat == null)
                        return;
                    DrawIcon_posOffset(pawnStats.TargetPos, Vector3.zero, targetMat, ColorNeutralStatusSolid,
                        viewOpacity);
                }
            }

            //Drafted
            if (psiSettings.ShowDraft)
            {
                if (pawn.Drafted)
                {
                    if (pawnStats.isPacifist)
                    {
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Pacific, ColorYellowAlert, ViewOpacityCrit);
                    }
                    else
                    {
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Draft, ColorRedAlert, ViewOpacityCrit);
                    }
                }
            }

            if (pawnStats.MentalSanity != null)
            {
                // Berserk
                if (psiSettings.ShowAggressive)
                    if (pawn.InAggroMentalState)
                    {
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Aggressive, ColorRedAlert, ViewOpacityCrit);
                    }

                // Give Up Exit
                if (psiSettings.ShowLeave)
                    if (pawnStats.MentalSanity == MentalStateDefOf.PanicFlee) // was GiveUpExit
                    {

                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Leave, ColorRedAlert, ViewOpacityCrit);
                    }

                //Daze Wander
                if (psiSettings.ShowDazed)
                    if (pawnStats.MentalSanity == MentalStateDefOf.WanderSad) // + MentalStateDefOf.WanderPsychotic
                    {

                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Dazed, ColorYellowAlert, ViewOpacityCrit);
                    }

                //PanicFlee
                if (psiSettings.ShowPanic)
                    if (pawnStats.MentalSanity == MentalStateDefOf.PanicFlee)
                    {

                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Panic, ColorYellowAlert, ViewOpacityCrit);
                    }
            }


            // Bloodloss
            if (psiSettings.ShowBloodloss)
                if (pawnStats.BleedRate > 0.0f)
                {
                    DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.Bloodloss, pawnStats.BleedRate,
                         ColorNeutralStatus, ColorRedAlert, viewOpacity);
                }

            //Health

            //Infection
            if (psiSettings.ShowHealth)
                if (pawnStats.IsSick)
                    if (pawnStats.HasLifeThreateningDisease)
                    {
               //         if (pawnStats.pawnHealth < pawnStats.HealthDisease)
                        {

                            DrawIcon_FadeFloatWithFourColorsHB(bodyLoc, ref iconNum, Icons.Health, pawnStats.pawnHealth,
                                ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert,
                                ViewOpacityCrit);
                        }

                   //   else
                   //   {
                   //       DrawIcon_FadeFloatWithFourColorsHB(bodyLoc, ref iconNum, Icons.Health, pawnStats.HealthDisease,
                   //           ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, ViewOpacityCrit);
                   //   }
                    }
                    // Regular Sickness
                    else if (pawnStats.DiseaseDisappearance < psiSettings.LimitDiseaseLess)
                    {
                        DrawIcon_FadeFloatWithFourColorsHB(bodyLoc, ref iconNum, Icons.Health,
                            pawnStats.DiseaseDisappearance / psiSettings.LimitDiseaseLess, ColorNeutralStatus,
                            ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, ViewOpacityCrit);
                    }
                    else if (pawn.health.summaryHealth.SummaryHealthPercent < 1f)
                    {

                        DrawIcon_FadeFloatWithFourColorsHB(bodyLoc, ref iconNum, Icons.Health, pawnStats.pawnHealth,
                            ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, ViewOpacityCrit);
                    }
            if (psiSettings.ShowMedicalAttention)
                if (HealthAIUtility.ShouldBeTendedNowUrgent(pawn))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.MedicalAttention, ColorRedAlert, ViewOpacityCrit);
                }
                else if (HealthAIUtility.ShouldBeTendedNow(pawn))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.MedicalAttention, ColorYellowAlert, ViewOpacityCrit);
                }
                else if (HealthAIUtility.ShouldHaveSurgeryDoneNow(pawn))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.MedicalAttention, ColorYellowAlert, ViewOpacityCrit);
                }

            // Hungry
            if (psiSettings.ShowHungry)
                if (pawn.needs.food.CurLevel < (double)psiSettings.LimitFoodLess)
                {
                    DrawIcon_FadeRedAlertToNeutral(bodyLoc, ref iconNum, Icons.Hungry,
                        pawn.needs.food.CurLevel / psiSettings.LimitFoodLess, ViewOpacityCrit);
                }


            // Addictions
            if (psiSettings.ShowDrunk)
            {
                if (pawnStats.isAddict || pawnStats.drugDesire != 0)
                {
                    Color color = new Color();

                    if (pawnStats.isAddict)
                    {
                        if (pawnStats.withDrawal)
                            GetWithdrawalColor(pawnStats, out color);
                        else
                            color = ColVermillion;
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
                            default:
                                break;
                        }
                    }

                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Drunk, color, ViewOpacityCrit);

                }
            }

            // Idle - icon only
            if (psiSettings.ShowIdle && pawn.mindState.IsIdle)
                DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Idle, ColorNeutralStatus, viewOpacity);

            // Pacifc

            bool pacifist = pawnStats.isPacifist;

            if (psiSettings.ShowUnarmed)
                if (pawn.equipment.Primary == null && !pawn.IsPrisoner && !pacifist)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Unarmed, ColReddishPurple, viewOpacity);
                }

            if (psiSettings.ShowPacific)
                if (pacifist)
                {
                    if (pawn.drafter != null && !pawn.Drafted)
                    {
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Pacific, ColBlueishGreen, viewOpacity);
                    }
                }

            // Trait Pyromaniac
            if (psiSettings.ShowPyromaniac)
                if (pawnStats.isPyromaniac)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Pyromaniac, ColorYellowAlert, ViewOpacityCrit);
                }



            MentalBreaker mb = !pawn.Dead ? pawn.mindState.mentalBreaker : null;

            // Bad Mood
            if (psiSettings.ShowSad)
                if (pawn.needs.mood.CurLevelPercentage <= mb?.BreakThresholdMinor)
                {

                    DrawIcon_FadeRedAlertToNeutral(bodyLoc, ref iconNum, Icons.Sad,
                        pawn.needs.mood.CurLevelPercentage / mb.BreakThresholdMinor, ViewOpacityCrit);
                }
            //   if (psi && PsiSettings.ShowSad && pawn.needs.mood.CurLevel < (double)PsiSettings.LimitMoodLess)
            //DrawIcon_FadeRedAlertToNeutral(bodyLoc, iconNum, Icons.Sad, pawn.needs.mood.CurLevel / PsiSettings.LimitMoodLess);


            //Toxicity buildup
            if (psiSettings.ShowToxicity)
                if (pawnStats.ToxicBuildUp > 0.04f)
                {

                    DrawIcon_FadeFloatFiveColors(bodyLoc, ref iconNum, Icons.Toxicity, pawnStats.ToxicBuildUp,
                        ColorNeutralStatusFade, ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert,
                        ViewOpacityCrit);
                }




            // Pain
            if (psiSettings.ShowPain)
            {
                if (pawnStats.PainMoodLevel > -1)
                {
                    Color color = new Color();
                    bool isMasochist = pawnStats.isMasochist;
                    switch (pawnStats.PainMoodLevel)
                    {
                        case 0:
                            {
                                color = isMasochist ? ColorMoodBoost * 0.4f : Color10To06;
                                break;
                            }
                        case 1:
                            {
                                color = isMasochist ? ColorMoodBoost * 0.6f : Color15To11;
                                break;
                            }
                        case 2:
                            {
                                color = isMasochist ? ColorMoodBoost * 0.8f : Color20To16;
                                break;
                            }
                        case 3:
                            {
                                color = isMasochist ? ColorMoodBoost * 1f : Color25To21;
                                break;
                            }

                    }
                    string tooltip;
                    tooltip = HealthCardUtility.GetPainTip(pawn);
                    if (psiSettings.ShowPain)
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Pain, color, viewOpacity);
                }
            }



            //Tired
            if (psiSettings.ShowTired)
                if (pawn.needs.rest.CurLevel < (double)psiSettings.LimitRestLess)
                {

                    DrawIcon_FadeRedAlertToNeutral(bodyLoc, ref iconNum, Icons.Tired,
                        pawn.needs.rest.CurLevel / psiSettings.LimitRestLess, ViewOpacityCrit);
                }

            // Too Cold & too hot
            if (psiSettings.ShowTooCold)
                if (pawnStats.TooCold > 0f)
                {
                    if (pawnStats.TooCold <= 1f)
                    {
                        DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.TooCold, pawnStats.TooCold,
                            ColorNeutralStatusFade, ColorYellowAlert, ViewOpacityCrit);
                    }
                    else if (pawnStats.TooCold <= 1.5f)
                    {
                        {
                            DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.TooCold, (pawnStats.TooCold - 1f) * 2f,
                                ColorYellowAlert, ColorOrangeAlert, ViewOpacityCrit);
                        }
                    }
                    else
                    {
                        {
                            DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.TooCold,
                                (pawnStats.TooCold - 1.5f) * 2f, ColorOrangeAlert, ColorRedAlert, ViewOpacityCrit);
                        }
                    }
                }


            if (psiSettings.ShowTooHot)
                if (pawnStats.TooHot > 0f)
                {
                    if (pawnStats.TooHot <= 1f)
                    {

                        DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.TooHot, pawnStats.TooHot,
                            ColorNeutralStatusFade, ColorYellowAlert, ViewOpacityCrit);
                    }
                    else if (pawnStats.TooHot <= 1.5f)
                    {
                        DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.TooHot, pawnStats.TooHot,
                            ColorYellowAlert, ColorOrangeAlert, ViewOpacityCrit);
                    }
                    else
                    {
                        DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.TooHot, pawnStats.TooHot - 1f,
                            ColorOrangeAlert, ColorRedAlert, ViewOpacityCrit);
                    }
                }

            // Bed status
            if (psiSettings.ShowBedroom)
                if (pawnStats.wantsToHump > -1)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Bedroom, ColorYellowAlert, ViewOpacityCrit);
                }
                else if (pawnStats.BedStatus > -1)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Bedroom, Color10To06, ViewOpacityCrit);
                }



            // Naked
            if (psiSettings.ShowNaked)
                if (pawnStats.feelsNaked > -1)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Naked, Color10To06, ViewOpacityCrit);
                }

            // Apparel
            if (psiSettings.ShowApparelHealth)
                if (pawnStats.ApparelHealth < (double)psiSettings.LimitApparelHealthLess)
                {
                    double pawnApparelHealth = pawnStats.ApparelHealth / (double)psiSettings.LimitApparelHealthLess;
                    DrawIcon_FadeRedAlertToNeutral(bodyLoc, ref iconNum, Icons.ApparelHealth, (float)pawnApparelHealth,
                        ViewOpacityCrit);
                }

            // Moods caused by traits

            if (pawnStats.prosthoWant != 0)
            {
                if (pawnStats.prosthoWant == -1)
                {

                    if (psiSettings.ShowProsthophile)
                    {
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Prosthophile, Color05AndLess, viewOpacity);
                    }
                }

                if (pawnStats.prosthoWant == 1)
                {

                    if (psiSettings.ShowProsthophobe)
                    {
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Prosthophobe, Color10To06, viewOpacity);
                    }
                }
            }

            if (psiSettings.ShowNightOwl)
                if (pawnStats.nightOwlUnhappy > -1)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.NightOwl, Color10To06, ViewOpacityCrit);
                }

            if (psiSettings.ShowGreedy)
                if (pawnStats.greedyThought)
                {

                    {
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Greedy, Color10To06, viewOpacity);
                    }
                }

            if (psiSettings.ShowJealous)
                if (pawnStats.jealousThought)
                {

                    {
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Jealous, Color10To06, viewOpacity);
                    }
                }


            // Effectiveness
            if (psiSettings.ShowEffectiveness)
                if (pawnStats.TotalEfficiency < (double)psiSettings.LimitEfficiencyLess)
                {

                    {
                        DrawIcon_FadeRedAlertToNeutral(bodyLoc, ref iconNum, Icons.Effectiveness,
                            pawnStats.TotalEfficiency / psiSettings.LimitEfficiencyLess, ViewOpacityCrit);
                    }
                }

            // Bad thoughts

            if (psiSettings.ShowLeftUnburied)
                if (pawnStats.unburied > -1)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.LeftUnburied, Color10To06, ViewOpacityCrit);
                }


            // CabinFever missing since A14?

            if (psiSettings.ShowCabinFever)
                if (pawnStats.CabinFeverMoodLevel > -1)
                {
                    if (pawnStats.CabinFeverMoodLevel == 0)
                    {
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.CabinFever, ColorNeutralStatusFade,
                            ViewOpacityCrit);
                    }
                    if (pawnStats.CabinFeverMoodLevel == 1)
                    {
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.CabinFever, ColorYellowAlert, ViewOpacityCrit);
                    }
                    if (pawnStats.CabinFeverMoodLevel == 2)
                    {
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.CabinFever, ColorOrangeAlert, ViewOpacityCrit);
                    }
                }

          //if (psiSettings.ShowDeadColonists)
          //{
          //    // Close Family & friends / 25
          //
          //    List<Thought> thoughts = pawnStats.Thoughts;
          //    // not family, more whiter icon
          //    if (HasThought(thoughts, ThoughtDef.Named("KilledColonist")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
          //    }
          //
          //    if (HasThought(thoughts, ThoughtDef.Named("KilledColonyAnimal")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
          //    }
          //
          //    #region DeathMemory
          //
          //    //Deathmemory
          //    // some of those need staging - to do 
          //    // edit: SCRAPPED!
          //    if (HasThought(thoughts, ThoughtDef.Named("KnowGuestExecuted")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
          //    }
          //    if (HasThought(thoughts, ThoughtDef.Named("KnowColonistExecuted")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
          //    }
          //    if (HasThought(thoughts, ThoughtDef.Named("KnowPrisonerDiedInnocent")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
          //    }
          //    if (HasThought(thoughts, ThoughtDef.Named("KnowColonistDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
          //    }
          //    //Bonded animal died
          //    if (HasThought(thoughts, ThoughtDef.Named("BondedAnimalDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
          //    }
          //    // Friend / rival died
          //    if (HasThought(thoughts, ThoughtDef.Named("thoughtWithGoodOpinionDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
          //    }
          //
          //    if (HasThought(thoughts, ThoughtDef.Named("thoughtWithBadOpinionDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, ColorMoodBoost, viewOpacity);
          //    }
          //
          //    #endregion
          //
          //    #region DeathMemoryFamily
          //
          //    if (HasThought(thoughts, ThoughtDef.Named("MySonDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color25To21, viewOpacity);
          //    }
          //
          //    if (HasThought(thoughts, ThoughtDef.Named("MyDaughterDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color25To21, viewOpacity);
          //    }
          //
          //    if (HasThought(thoughts, ThoughtDef.Named("MyHusbandDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color25To21, viewOpacity);
          //    }
          //
          //    if (HasThought(thoughts, ThoughtDef.Named("MyWifeDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color25To21, viewOpacity);
          //    }
          //
          //    if (HasThought(thoughts, ThoughtDef.Named("MyFianceDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color20To16, viewOpacity);
          //    }
          //
          //    if (HasThought(thoughts, ThoughtDef.Named("MyFianceeDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color20To16, viewOpacity);
          //    }
          //
          //    if (HasThought(thoughts, ThoughtDef.Named("MyLoverDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color20To16, viewOpacity);
          //    }
          //
          //    if (HasThought(thoughts, ThoughtDef.Named("MyBrotherDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color15To11, viewOpacity);
          //    }
          //
          //    if (HasThought(thoughts, ThoughtDef.Named("MySisterDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color15To11, viewOpacity);
          //    }
          //
          //    if (HasThought(thoughts, ThoughtDef.Named("MyGrandchildDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color15To11, viewOpacity);
          //    }
          //
          //    // 10
          //
          //    if (HasThought(thoughts, ThoughtDef.Named("MyFatherDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
          //    }
          //
          //    if (HasThought(thoughts, ThoughtDef.Named("MyMotherDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
          //    }
          //
          //    if (HasThought(thoughts, ThoughtDef.Named("MyNieceDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
          //    }
          //
          //    if (HasThought(thoughts, ThoughtDef.Named("MyNephewDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
          //    }
          //
          //    if (HasThought(thoughts, ThoughtDef.Named("MyHalfSiblingDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
          //    }
          //
          //    if (HasThought(thoughts, ThoughtDef.Named("MyAuntDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
          //    }
          //
          //    if (HasThought(thoughts, ThoughtDef.Named("MyUncleDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
          //    }
          //
          //    if (HasThought(thoughts, ThoughtDef.Named("MyGrandparentDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
          //    }
          //
          //    if (HasThought(thoughts, ThoughtDef.Named("MyCousinDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
          //    }
          //    if (HasThought(thoughts, ThoughtDef.Named("MyKinDied")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
          //    }
          //
          //    #endregion
          //
          //    //Memory misc
          //    if (HasThought(thoughts, ThoughtDef.Named("WitnessedDeathAlly")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
          //    }
          //    if (HasThought(thoughts, ThoughtDef.Named("WitnessedDeathNonAlly")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
          //    }
          //    if (HasThought(thoughts, ThoughtDef.Named("WitnessedDeathFamily")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
          //    }
          //    if (HasThought(thoughts, ThoughtDef.Named("WitnessedDeathBloodlust")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, ColorMoodBoost, viewOpacity);
          //    }
          //    if (HasThought(thoughts, ThoughtDef.Named("KilledHumanlikeBloodlust")))
          //    {
          //        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, ColorMoodBoost, viewOpacity);
          //    }
          //
          //}

        }

        public static void DrawColonistIconsBar(Pawn pawn, PawnStats pawnStats, Rect psiRect, float rectAlpha)
        {
            SettingsColonistBar colBarSettings = Settings.ColBarSettings;

            int barIconNum = 0;

            //Drafted
            if (colBarSettings.ShowDraft)
                if (pawn.Drafted)
                {
                    if (pawnStats.isPacifist)
                    {
                        string tooltip = "IsIncapableOfViolence".Translate(pawn.NameStringShort);
                        DrawIconOnBar(psiRect, ref barIconNum, Icons.Pacific, ColorYellowAlert, rectAlpha, tooltip);
                    }
                    else
                    {
                        DrawIconOnBar(psiRect, ref barIconNum, Icons.Draft, ColorRedAlert, rectAlpha);
                    }
                }

            if (pawnStats.MentalSanity != null)
            {
                // Berserk
                if (colBarSettings.ShowAggressive && pawn.InAggroMentalState)
                {
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.Aggressive, ColorRedAlert, rectAlpha);
                }

                // Give Up Exit
                if (colBarSettings.ShowLeave)
                    if (pawnStats.MentalSanity == MentalStateDefOf.PanicFlee) // was GiveUpExit
                    {
                        DrawIconOnBar(psiRect, ref barIconNum, Icons.Leave, ColorRedAlert, rectAlpha);

                    }

                //Daze Wander
                if (colBarSettings.ShowDazed)
                    if (pawnStats.MentalSanity == MentalStateDefOf.WanderSad) // + MentalStateDefOf.WanderPsychotic
                    {
                        DrawIconOnBar(psiRect, ref barIconNum, Icons.Dazed, ColorYellowAlert, rectAlpha);

                    }

                //PanicFlee
                if (colBarSettings.ShowPanic)
                    if (pawnStats.MentalSanity == MentalStateDefOf.PanicFlee)
                    {
                        DrawIconOnBar(psiRect, ref barIconNum, Icons.Panic, ColorYellowAlert, rectAlpha);

                    }
            }


            // Bloodloss
            if (colBarSettings.ShowBloodloss)
                if (pawnStats.BleedRate > 0.0f)
                {
                    string tooltip = "BleedingRate".Translate() + ": " + pawn.health.hediffSet.BleedRateTotal.ToStringPercent() + "/d";
                    DrawIcon_FadeFloatWithTwoColors(psiRect, ref barIconNum, Icons.Bloodloss, pawnStats.BleedRate,
                         ColorNeutralStatus, ColorRedAlert, rectAlpha, tooltip);
                }




            //Health
            //Infection
            if (colBarSettings.ShowHealth)
                if (pawnStats.IsSick)
                    if (pawnStats.HasLifeThreateningDisease)
                    {
                       // if (pawnStats.pawnHealth < pawnStats.HealthDisease)
                        {
                            string tooltip = "Immunity".Translate() + " / " + "PSI.WoundInfection".Translate() + ": \n" +
                                             pawnStats.immunity.ToStringPercent() + "/" + pawnStats.severity.ToStringPercent()
                                             +"\n" + pawnStats.painTip;

                            
                            DrawIcon_FadeFloatWithFourColorsHB(psiRect, ref barIconNum, Icons.Health, pawnStats.pawnHealth,
                                ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, rectAlpha, tooltip);
                        }

                      //else
                      //{
                      //    string tooltip = pawnStats.sickTip;
                      //    DrawIcon_FadeFloatWithFourColorsHB(psiRect, ref barIconNum, Icons.Health, pawnStats.HealthDisease,
                      //        ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, rectAlpha, tooltip);
                      //}
                    }
                    else if (pawnStats.DiseaseDisappearance < Settings.PsiSettings.LimitDiseaseLess)
                    {
                        // Regular Sickness
                        string tooltip = "Immunity".Translate() + " / " + "PSI.WoundInfection".Translate() + ": \n" +
                                         pawnStats.immunity.ToStringPercent() + " / " + pawnStats.severity.ToStringPercent();

                        DrawIcon_FadeFloatWithFourColorsHB(psiRect, ref barIconNum, Icons.Health,
                            pawnStats.DiseaseDisappearance / colBarSettings.LimitDiseaseLess, ColorNeutralStatus,
                            ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, rectAlpha, tooltip);
                    }
                    else if (pawn.health.summaryHealth.SummaryHealthPercent < 1f)
                    {
                        string tooltip = "Health".Translate() + ": " +
                                         pawn.health.summaryHealth.SummaryHealthPercent.ToStringPercent();
                        DrawIcon_FadeFloatWithFourColorsHB(psiRect, ref barIconNum, Icons.Health, pawnStats.pawnHealth,
                            ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, rectAlpha, tooltip);
                    }
            // Idle - bar icon already included - vanilla
            bool pacifist = pawnStats.isPacifist;
    
            // Pacifc
            if (colBarSettings.ShowUnarmed)
                if (pawn.equipment.Primary == null && !pawn.IsPrisoner && !pacifist)
                {
                    string tooltip = "PSI.Settings.Visibility.Unarmed".Translate();
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.Unarmed, ColReddishPurple, rectAlpha, tooltip);
                }


            if (colBarSettings.ShowPacific)
                if (pacifist)
                {
                    if (pawn.drafter != null && !pawn.Drafted)
                    {
                        string tooltip;
                        tooltip = "IsIncapableOfViolence".Translate(pawn.NameStringShort);
                        DrawIconOnBar(psiRect, ref barIconNum, Icons.Pacific, ColBlueishGreen, rectAlpha, tooltip);
                    }
                }


            // Trait Pyromaniac
            if (colBarSettings.ShowPyromaniac)
                if (pawnStats.isPyromaniac)
                {
                    string tooltip = "PSI.Settings.Visibility.Pyromaniac".Translate();
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.Pyromaniac, ColorYellowAlert, rectAlpha, tooltip);
                }



            if (colBarSettings.ShowMedicalAttention)
                if (HealthAIUtility.ShouldBeTendedNowUrgent(pawn))
                {
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.MedicalAttention, ColorRedAlert, rectAlpha);
                }
                else if (HealthAIUtility.ShouldBeTendedNow(pawn))
                {
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.MedicalAttention, ColorYellowAlert, rectAlpha);
                }
                else if (HealthAIUtility.ShouldHaveSurgeryDoneNow(pawn))
                {
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.MedicalAttention, ColorYellowAlert, rectAlpha);
                }

            // Hungry
            if (colBarSettings.ShowHungry)
                if (pawn.needs.food.CurLevel < (double)Settings.PsiSettings.LimitFoodLess)
                {
                    string tooltip = pawn.needs.food.GetTipString();
                    DrawIcon_FadeRedAlertToNeutral(psiRect, ref barIconNum, Icons.Hungry,
                        pawn.needs.food.CurLevel / Settings.PsiSettings.LimitFoodLess, rectAlpha, tooltip);
                }


            // Addictions
            if (colBarSettings.ShowDrunk)
            {
                if (pawnStats.isAddict || pawnStats.drugDesire != 0)
                {
                    Color color = new Color();
                    string tooltip = null;

                    if (pawnStats.isAddict)
                    {
                        if (pawnStats.withDrawal)
                            GetWithdrawalColor(pawnStats, out color);
                        else
                            color = ColVermillion;

                        if (!pawnStats.drugUserLabel.NullOrEmpty())
                            tooltip = pawnStats.drugUserLabel + "\n" + pawnStats.addictionLabel;
                        else
                            tooltip = pawnStats.addictionLabel;
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
                            default:
                                break;
                        }
                        tooltip = pawnStats.drugUserLabel;
                    }

                    DrawIconOnBar(psiRect, ref barIconNum, Icons.Drunk, color, rectAlpha, tooltip);

                }




                // Bad Mood
                // No need for an icon if I have a mood bar.

                //   if (psi && PsiSettings.ShowSad && pawn.needs.mood.CurLevel < (double)PsiSettings.LimitMoodLess)
                //DrawIcon_FadeRedAlertToNeutral(bodyLoc, iconNum, Icons.Sad, pawn.needs.mood.CurLevel / PsiSettings.LimitMoodLess);


                //Toxicity buildup
                if (colBarSettings.ShowToxicity)
                    if (pawnStats.ToxicBuildUp > 0.04f)
                    {
                        DrawIcon_FadeFloatFiveColors(psiRect, ref barIconNum, Icons.Toxicity, pawnStats.ToxicBuildUp,
                            ColorNeutralStatusFade, ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert,
                            rectAlpha);
                    }




                // Pain
                if (colBarSettings.ShowPain)
                {
                    if (pawnStats.PainMoodLevel > -1)
                    {
                        Color color = new Color();
                        bool isMasochist = pawnStats.isMasochist;
                        switch (pawnStats.PainMoodLevel)
                        {
                            case 0:
                                {
                                    color = isMasochist ? ColorMoodBoost * 0.4f : Color10To06;
                                    break;
                                }
                            case 1:
                                {
                                    color = isMasochist ? ColorMoodBoost * 0.6f : Color15To11;
                                    break;
                                }
                            case 2:
                                {
                                    color = isMasochist ? ColorMoodBoost * 0.8f : Color20To16;
                                    break;
                                }
                            case 3:
                                {
                                    color = isMasochist ? ColorMoodBoost * 1f : Color25To21;
                                    break;
                                }

                        }
                        string tooltip = pawnStats.painTip;

                        DrawIconOnBar(psiRect, ref barIconNum, Icons.Pain, color, rectAlpha, tooltip);
                    }
                }



                //Tired
                if (colBarSettings.ShowTired)
                    if (pawn.needs.rest.CurLevel < (double)Settings.PsiSettings.LimitRestLess)
                    {
                        if (pawn.needs.rest.CurLevel < (double)Settings.PsiSettings.LimitRestLess)
                            DrawIcon_FadeRedAlertToNeutral(psiRect, ref barIconNum, Icons.Tired,
                                pawn.needs.rest.CurLevel / Settings.PsiSettings.LimitRestLess, rectAlpha);
                    }

                // Too Cold & too hot
                if (colBarSettings.ShowTooCold)
                    if (pawnStats.TooCold > 0f)
                    {
                        if (pawnStats.TooCold <= 1f)
                        {
                            DrawIcon_FadeFloatWithTwoColors(psiRect, ref barIconNum, Icons.TooCold, pawnStats.TooCold,
                                ColorNeutralStatusFade, ColorYellowAlert, rectAlpha);
                        }
                        else if (pawnStats.TooCold <= 1.5f)
                        {
                            DrawIcon_FadeFloatWithTwoColors(psiRect, ref barIconNum, Icons.TooCold, (pawnStats.TooCold - 1f) * 2f,
                                ColorYellowAlert, ColorOrangeAlert, rectAlpha);

                        }
                        else
                        {
                            DrawIcon_FadeFloatWithTwoColors(psiRect, ref barIconNum, Icons.TooCold,
                                (pawnStats.TooCold - 1.5f) * 2f, ColorOrangeAlert, ColorRedAlert, rectAlpha);

                        }
                    }

                if (colBarSettings.ShowTooHot)
                    if (pawnStats.TooHot > 0f)
                    {
                        if (pawnStats.TooHot <= 1f)
                        {
                            DrawIcon_FadeFloatWithTwoColors(psiRect, ref barIconNum, Icons.TooHot, pawnStats.TooHot,
                                ColorNeutralStatusFade, ColorYellowAlert, rectAlpha);
                        }
                        else if (pawnStats.TooHot <= 1.5f)
                        {
                            DrawIcon_FadeFloatWithTwoColors(psiRect, ref barIconNum, Icons.TooHot, pawnStats.TooHot,
                                ColorYellowAlert, ColorOrangeAlert, rectAlpha);
                        }
                        else
                        {
                            DrawIcon_FadeFloatWithTwoColors(psiRect, ref barIconNum, Icons.TooHot, pawnStats.TooHot - 1f,
                                ColorOrangeAlert, ColorRedAlert, rectAlpha);
                        }
                    }

                // Bed status
                if (colBarSettings.ShowBedroom)
                {
                    if (pawnStats.wantsToHump > -1)
                    {
                        string tooltip = pawnStats.humpTip;
                        DrawIconOnBar(psiRect, ref barIconNum, Icons.Bedroom, ColorYellowAlert, rectAlpha, tooltip);
                    }
                    else if (pawnStats.BedStatus > -1)
                    {
                        string tooltip = pawnStats.BedStatusTip;

                        DrawIconOnBar(psiRect, ref barIconNum, Icons.Bedroom, Color10To06, rectAlpha, tooltip);
                    }
                }


                // Naked
                if (colBarSettings.ShowNaked)
                    if (pawnStats.feelsNaked > 1)
                    {
                        // Naked
                        {
                            string tooltip = pawnStats.nakedTip;
                            DrawIconOnBar(psiRect, ref barIconNum, Icons.Naked, Color10To06, rectAlpha, tooltip);
                        }

                    }

                // Apparel
                if (colBarSettings.ShowApparelHealth)
                    if (pawnStats.ApparelHealth < (double)Settings.PsiSettings.LimitApparelHealthLess)
                    {
                        double pawnApparelHealth = pawnStats.ApparelHealth /
                                                   (double)colBarSettings.LimitApparelHealthLess;
                        DrawIcon_FadeRedAlertToNeutral(psiRect, ref barIconNum, Icons.ApparelHealth,
                            (float)pawnApparelHealth,
                            rectAlpha);
                    }

                // Moods caused by traits

                if (pawnStats.prosthoWant != 0)
                {
                    if (pawnStats.prosthoWant == -1)
                    {
                        if (colBarSettings.ShowProsthophile)
                        {
                            DrawIconOnBar(psiRect, ref barIconNum, Icons.Prosthophile, Color05AndLess, rectAlpha);
                        }

                    }

                    if (pawnStats.prosthoWant == 1)
                    {
                        if (colBarSettings.ShowProsthophobe)
                        {
                            DrawIconOnBar(psiRect, ref barIconNum, Icons.Prosthophobe, Color10To06, rectAlpha);
                        }

                    }
                }

                if (colBarSettings.ShowNightOwl)
                    if (pawnStats.nightOwlUnhappy > -1)
                    {
                        string tooltip = pawnStats.nightOwlTip;
                        DrawIconOnBar(psiRect, ref barIconNum, Icons.NightOwl, Color10To06, rectAlpha, tooltip);
                    }

                if (colBarSettings.ShowGreedy && pawnStats.greedyThought)
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.Greedy, Color10To06, rectAlpha);

                if (colBarSettings.ShowJealous && pawnStats.jealousThought)
                {
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.Jealous, Color10To06, rectAlpha);
                }


                // Effectiveness
                if (colBarSettings.ShowEffectiveness)
                    if (pawnStats.TotalEfficiency < (double)Settings.PsiSettings.LimitEfficiencyLess)
                    {
                        string tooltip = "PSI.Efficiency".Translate() + ": " + pawnStats.TotalEfficiency.ToStringPercent();
                        DrawIcon_FadeRedAlertToNeutral(psiRect, ref barIconNum, Icons.Effectiveness,
                            pawnStats.TotalEfficiency / Settings.PsiSettings.LimitEfficiencyLess, rectAlpha, tooltip);

                    }

                // Bad thoughts

                if (colBarSettings.ShowLeftUnburied)
                    if (pawnStats.unburied > -1)
                    {
                        string tooltip = pawnStats.unburiedTip;
                        DrawIconOnBar(psiRect, ref barIconNum, Icons.LeftUnburied, Color10To06, rectAlpha, tooltip);
                    }

                // CabinFever missing since A14?

                if (colBarSettings.ShowCabinFever)
                    if (pawnStats.CabinFeverMoodLevel > -1)
                    {
                        string tooltip = pawnStats.cabinFeverTip;
                        if (pawnStats.CabinFeverMoodLevel == 0)
                        {
                            DrawIconOnBar(psiRect, ref barIconNum, Icons.CabinFever, ColorNeutralStatusFade, rectAlpha, tooltip);
                        }
                        if (pawnStats.CabinFeverMoodLevel == 1)
                        {
                            DrawIconOnBar(psiRect, ref barIconNum, Icons.CabinFever, ColorYellowAlert, rectAlpha, tooltip);
                        }
                        if (pawnStats.CabinFeverMoodLevel == 2)
                        {
                            if (colBarSettings.ShowCabinFever)
                                DrawIconOnBar(psiRect, ref barIconNum, Icons.CabinFever, ColorOrangeAlert, rectAlpha, tooltip);
                        }
                    }
            }
            pawnStats.IconCount = barIconNum > colBarSettings.MaxRowsCustom ? colBarSettings.MaxRowsCustom : barIconNum;

        }

        private static void GetWithdrawalColor(PawnStats pawnStats, out Color color)
        {
            color = Color.Lerp(ColBlueishGreen, ColVermillion, pawnStats.withDrawalPercent);
        }

        #endregion
    }
}