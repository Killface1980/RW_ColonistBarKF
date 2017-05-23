using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using static ColonistBarKF.PSI.PSIDrawer;
using static ColonistBarKF.Settings;
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

        public static Vector3[] _iconPosVectorsPSI;
        public static Vector3[] _iconPosRectsBar;

        public override void FinalizeInit()
        {
            Reinit();
        }

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
                ColBarSettings = LoadBarSettings();
                PsiSettings = LoadPsiSettings();
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
                    PSIMaterials = new Materials(PsiSettings.IconSet);
                    //PSISettings SettingsPSI =
                    //    XmlLoader.ItemFromXmlFile<PSISettings>(GenFilePaths.CoreModsFolderPath + "/RW_PawnStateIcons/Textures/UI/Overlays/PawnStateIcons/" + PSI.SettingsPSI.IconSet + "/iconset.cfg");
                    //PSI.PsiSettings.IconSizeMult = SettingsPSI.IconSizeMult;
                    PSIMaterials.ReloadTextures(true);
                    //   Log.Message(GenFilePaths.CoreModsFolderPath + "/RW_PawnStateIcons/Textures/UI/Overlays/PawnStateIcons/" + ColBarSettings.IconSet + "/iconset.cfg");
                });
            }

        }
        private CellRect _viewRect;
        private static bool initialized;

        public override void GameComponentOnGUI()
        {

            if (Current.ProgramState != ProgramState.Playing)
                return;

            if (WorldRendererUtility.WorldRenderedNow)
            {
                return;
            }

            if (!PsiSettings.UsePsi && !PsiSettings.UsePsiOnPrisoner)
                return;

            {
                _viewRect = Find.CameraDriver.CurrentViewRect;
                _viewRect = _viewRect.ExpandedBy(5);
                foreach (Pawn pawn in Find.VisibleMap.mapPawns.AllPawns)
                {
                    if (!_viewRect.Contains(pawn.Position))
                        continue;
                    //     if (useGUILayout)
                    {
                        if (pawn != null && pawn.RaceProps.Animal)
                            DrawAnimalIcons(pawn);
                        else if (pawn != null && ((PsiSettings.UsePsi && pawn.IsColonist) || (PsiSettings.UsePsiOnPrisoner && pawn.IsPrisoner)))
                        {
                            DrawColonistIcons(pawn, true);
                        }
                    }
                }
            }
        }

        // ReSharper disable once UnusedMember.Global
        public override void GameComponentUpdate()
        {
            if (Input.GetKeyUp(KeyCode.F11))
            {
                PsiSettings.UsePsi = !PsiSettings.UsePsi;
                ColBarSettings.UsePsi = !ColBarSettings.UsePsi;
                Messages.Message(PsiSettings.UsePsi ? "PSI.Enabled".Translate() : "PSI.Disabled".Translate(), MessageSound.Standard);
            }
            _worldScale = Screen.height / (2f * Camera.current.orthographicSize);
        }

        public override void GameComponentTick()
        {
            // Scans the map for new pawns

            if (Current.ProgramState != ProgramState.Playing)
                return;

            if (!ColBarSettings.UsePsi && !PsiSettings.UsePsi)
                return;

            _fDelta += Time.fixedDeltaTime;
            if (_fDelta < 5)
                return;
            _fDelta = 0.0;

            foreach (Pawn pawn in PawnsFinder.AllMaps_FreeColonistsAndPrisonersSpawned) //.FreeColonistsAndPrisoners)                                                                                        //               foreach (var colonist in Find.Map.mapPawns.FreeColonistsAndPrisonersSpawned) //.FreeColonistsAndPrisoners)
            {
                if (pawn.Dead || pawn.DestroyedOrNull() || !pawn.Name.IsValid || pawn.Name == null)
                    continue;

                if (_statsDict.ContainsKey(pawn))
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


        #region Icon Drawing 



        public static void RecalcIconPositionsPSI()
        {
            //            _iconPosVectors = new Vector3[18];
            _iconPosVectorsPSI = new Vector3[40];
            for (int index = 0; index < _iconPosVectorsPSI.Length; ++index)
            {
                int num1 = index / PsiSettings.IconsInColumn;
                int num2 = index % PsiSettings.IconsInColumn;
                if (PsiSettings.IconsHorizontal)
                {
                    int num3 = num1;
                    num1 = num2;
                    num2 = num3;
                }


                _iconPosVectorsPSI[index] =
                    new Vector3(
                        (float)
                            (-0.600000023841858 * PsiSettings.IconMarginX -
                             0.550000011920929 * PsiSettings.IconSize * PsiSettings.IconOffsetX * num1), 3f,
                        (float)
                            (-0.600000023841858 * PsiSettings.IconMarginY +
                             0.550000011920929 * PsiSettings.IconSize * PsiSettings.IconOffsetY * num2));

            }
        }

        public static void RecalcBarPositionAndSize()
        {

            _iconPosRectsBar = new Vector3[40];
            for (int index = 0; index < _iconPosRectsBar.Length; ++index)
            {
                int num1;
                int num2;
                num1 = index / (ColBarSettings.IconsInColumn);
                num2 = index % (ColBarSettings.IconsInColumn);
                if (ColBarSettings.IconsHorizontal)
                {
                    int num3 = num1;
                    num1 = num2;
                    num2 = num3;

                    //   num2 = index / ColBarSettings.IconsInColumn;
                    //   num1 = index % ColBarSettings.IconsInColumn;

                }

                _iconPosRectsBar[index] =
                    new Vector3(
                        -num1,
                        3f,
                        num2);
            }
        }

        #endregion

        #region Status + Updates

        private static void UpdateColonistStats(Pawn colonist)
        {
            if (colonist != null && !_statsDict.ContainsKey(colonist))
            {
                _statsDict.Add(colonist, new PawnStats());
            }

            if (colonist == null)
            {
                return;
            }
            List<Thought> thoughts = new List<Thought>();
            PawnStats pawnStats = _statsDict[colonist];
            colonist.needs.mood.thoughts.GetDistinctMoodThoughtGroups(thoughts);
            pawnStats.Thoughts = thoughts;


            // efficiency
            float efficiency = 10f;

            PawnCapacityDef[] array = _pawnCapacities;
            foreach (PawnCapacityDef pawnCapacityDef in array)
            {
                if (pawnCapacityDef != PawnCapacityDefOf.Consciousness)
                {
                    efficiency = Math.Min(efficiency, colonist.health.capacities.GetLevel(pawnCapacityDef));
                }
                if (efficiency < 0f)
                    efficiency = 0f;
            }

            pawnStats.TotalEfficiency = efficiency;



            //target
            pawnStats.TargetPos = Vector3.zero;

            if (colonist.jobs.curJob != null)
            {
                JobDriver curDriver = colonist.jobs.curDriver;
                Job curJob = colonist.jobs.curJob;
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
                if (curDriver is JobDriver_Hunt && colonist.carryTracker?.CarriedThing != null)
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
                if (curJob.def == JobDefOf.LayDown && colonist.InBed())
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
            float temperatureForCell = GenTemperature.GetTemperatureForCell(colonist.Position, colonist.Map);

            pawnStats.TooCold =
                (float)
                    ((colonist.ComfortableTemperatureRange().min - (double)PsiSettings.LimitTempComfortOffset -
                      temperatureForCell) / 10f);

            pawnStats.TooHot =
                (float)
                    ((temperatureForCell - (double)colonist.ComfortableTemperatureRange().max -
                      PsiSettings.LimitTempComfortOffset) / 10f);

            pawnStats.TooCold = Mathf.Clamp(pawnStats.TooCold, 0f, 2f);

            pawnStats.TooHot = Mathf.Clamp(pawnStats.TooHot, 0f, 2f);
            /*
            // Drunkness - DEACTIVATED FOR NOW
            pawnStats.Drunkness =  DrugUtility.DrunknessPercent(colonist);
        */
            // Mental Sanity
            pawnStats.MentalSanity = null;
            if (colonist.mindState != null && colonist.InMentalState)
            {
                pawnStats.MentalSanity = colonist.MentalStateDef;
            }

            // Mental Breaker for MoodBars
            if (colonist.needs != null && colonist.needs.mood != null)
            {
                pawnStats.Mb = colonist.mindState.mentalBreaker;
                pawnStats.Mood = colonist.needs.mood;
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

            // Sick thoughts
            if (colonist.health?.hediffSet != null)
                pawnStats.IsSick = colonist.health.hediffSet.AnyHediffMakesSickThought;


            if (pawnStats.IsSick && !colonist.Destroyed && colonist.playerSettings.medCare >= 0)
            {
                if (colonist.health?.hediffSet?.hediffs != null)
                {
                    int i;
                    for (i = 0; i < colonist.health.hediffSet.hediffs.Count; i++)
                    {
                        Hediff hediff = colonist.health.hediffSet.hediffs[i];
                        if (!hediff.Visible) continue;
                        //         HediffWithComps hediffWithComps;

                        //      if ((HediffWithComps)hediff != null)
                        //          hediffWithComps = (HediffWithComps)hediff;
                        //      else continue;
                        //
                        //      if (hediffWithComps.IsOld()) continue;

                        pawnStats.ToxicBuildUp = 0;

                        //pawnStats.ToxicBuildUp
                        if (hediff.def == HediffDefOf.ToxicBuildup)
                        {
                            pawnStats.ToxicBuildUp = hediff.Severity;
                        }
                        HediffComp_Immunizable compImmunizable = hediff.TryGetComp<HediffComp_Immunizable>();
                        if (compImmunizable != null)
                        {
                            float severity = hediff.Severity;
                            float immunity = compImmunizable.Immunity;
                            float basehealth = pawnStats.HealthDisease - (severity - immunity / 4) - 0.25f;
                            pawnStats.HasLifeThreateningDisease = true;
                            pawnStats.HealthDisease = basehealth;
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

                        if (Math.Abs(colonist.health.immunity.GetImmunity(hediff.def) - 1.0) < 0.05) continue;

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
            List<Apparel> apparelListForReading = colonist.apparel.WornApparel;
            foreach (Apparel t in apparelListForReading)
            {
                float curApparel = t.HitPoints / (float)t.MaxHitPoints;
                if (curApparel >= 0f && curApparel < worstApparel)
                {
                    worstApparel = curApparel;
                }
            }
            pawnStats.ApparelHealth = worstApparel;

            // Bleed rate
            if (colonist.health?.hediffSet != null)
                pawnStats.BleedRate = Mathf.Clamp01(colonist.health.hediffSet.BleedRateTotal * PsiSettings.LimitBleedMult);


            // Bed status
            pawnStats.HasBed = colonist.ownership.OwnedBed != null;

            // Cabin Fever
            ThoughtDef thought = ThoughtDef.Named("CabinFever");
            if (!HasThought(colonist, thought))
                pawnStats.CabinFeverMoodLevel = -1;
            else
                pawnStats.CabinFeverMoodLevel = thought.Worker.CurrentState(colonist).StageIndex;

            if (colonist.story.traits.HasTrait(TraitDef.Named("Masochist")))
                thought = ThoughtDef.Named("MasochistPain");
            else
                thought = ThoughtDef.Named("Pain");

            if (!HasThought(colonist, thought))
                pawnStats.PainMoodLevel = -1;
            else
                pawnStats.PainMoodLevel = thought.Worker.CurrentState(colonist).StageIndex;




            _statsDict[colonist] = pawnStats;
        }

        private static bool HasThought(Pawn pawn, ThoughtDef tdef)
        {
            if (pawn.mindState == null)
                return false;
            PawnStats pawnStats = _statsDict[pawn];
            return pawnStats.Thoughts.Any(thought => thought.def == tdef);

        }

        #endregion

        #region Draw Icons

        private static float ViewOpacityCrit
        {
            get
            {
                return Mathf.Max(PsiSettings.IconOpacityCritical,
                       PsiSettings.IconOpacity);
            }
        }

        public static float WorldScale => _worldScale;

        private static void DrawAnimalIcons(Pawn animal)
        {
            if (!PsiSettings.ShowAggressive)
                return;
            if (!animal.InAggroMentalState)
                return;
            if (!animal.Spawned)
                return;

            Vector3 drawPos = animal.DrawPos;
            Vector3 bodyPos = drawPos;
            int num = 0;
            DrawIconOnColonist(bodyPos, ref num, Icons.Aggressive, ColorRedAlert, ViewOpacityCrit);
        }


        public static void DrawColonistIcons(Pawn pawn, bool psi, float rectAlpha = 1f, Rect psiRect = new Rect())
        {

            PawnStats pawnStats;
            if (pawn.Dead || !pawn.Spawned || pawn.holdingOwner == null || !_statsDict.TryGetValue(pawn, out pawnStats))
                return;

            if (Find.TickManager.TicksGame > pawnStats.LastStatUpdate + Rand.Range(60, 180))
            {
                UpdateColonistStats(pawn);
                pawnStats.LastStatUpdate = Find.TickManager.TicksGame;
            }


            float viewOpacity = PsiSettings.IconOpacity;

            int barIconNum = 0;
            int iconNum = 0;

            Vector3 bodyLoc = pawn.DrawPos;
            string tooltip = null;
            // Target Point 
            if (psi)
                if (PsiSettings.ShowTargetPoint && (pawnStats.TargetPos != Vector3.zero))
                {
                    if (PsiSettings.UseColoredTarget)
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
            if (pawn.Drafted)
            {
                if (pawn.story != null && pawn.story.WorkTagIsDisabled(WorkTags.Violent))
                {
                    if (!psi && ColBarSettings.ShowDraft)
                    {
                        tooltip = "IsIncapableOfViolence".Translate(pawn.NameStringShort);
                        DrawIconOnBar(psiRect, ref barIconNum, Icons.Pacific, ColorYellowAlert, rectAlpha, tooltip);
                    }
                    if (psi && PsiSettings.ShowDraft)
                    {
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Pacific, ColorYellowAlert, ViewOpacityCrit);
                    }
                }
                else
                {
                    if (!psi && ColBarSettings.ShowDraft)
                    {
                        DrawIconOnBar(psiRect, ref barIconNum, Icons.Draft, ColorRedAlert, rectAlpha);
                    }
                    if (psi && PsiSettings.ShowDraft)
                    {
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Draft, ColorRedAlert, ViewOpacityCrit);
                    }
                }
            }

            if (pawnStats.MentalSanity != null)
            {
                // Berserk
                if (pawn.InAggroMentalState)
                {
                    if (!psi && ColBarSettings.ShowAggressive && pawn.InAggroMentalState)
                        DrawIconOnBar(psiRect, ref barIconNum, Icons.Aggressive, ColorRedAlert, rectAlpha);
                    if (psi && PsiSettings.ShowAggressive)
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Aggressive, ColorRedAlert, ViewOpacityCrit);
                }

                // Binging on alcohol - needs refinement
                {
                    if (pawnStats.MentalSanity == MentalStateDefOf.BingingDrugMajor)
                    {
                        if (!psi && ColBarSettings.ShowDrunk)
                            DrawIconOnBar(psiRect, ref barIconNum, Icons.Drunk, ColorOrangeAlert, rectAlpha);
                        if (psi && PsiSettings.ShowDrunk)
                            DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Drunk, ColorOrangeAlert, ViewOpacityCrit);
                    }

                    if (pawnStats.MentalSanity == MentalStateDefOf.BingingDrugExtreme)
                    {
                        if (!psi && ColBarSettings.ShowDrunk)
                            DrawIconOnBar(psiRect, ref barIconNum, Icons.Drunk, ColorRedAlert, rectAlpha);
                        if (psi && PsiSettings.ShowDrunk)
                            DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Drunk, ColorRedAlert, ViewOpacityCrit);
                    }
                }

                // Give Up Exit
                if (pawnStats.MentalSanity == MentalStateDefOf.PanicFlee) // was GiveUpExit
                {
                    if (!psi && ColBarSettings.ShowLeave)
                        DrawIconOnBar(psiRect, ref barIconNum, Icons.Leave, ColorRedAlert, rectAlpha);

                    if (psi && PsiSettings.ShowLeave)
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Leave, ColorRedAlert, ViewOpacityCrit);
                }

                //Daze Wander
                if (pawnStats.MentalSanity == MentalStateDefOf.WanderSad) // + MentalStateDefOf.WanderPsychotic
                {
                    if (!psi && ColBarSettings.ShowDazed)
                        DrawIconOnBar(psiRect, ref barIconNum, Icons.Dazed, ColorYellowAlert, rectAlpha);

                    if (psi && PsiSettings.ShowDazed)
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Dazed, ColorYellowAlert, ViewOpacityCrit);
                }

                //PanicFlee
                if (pawnStats.MentalSanity == MentalStateDefOf.PanicFlee)
                {
                    if (!psi && ColBarSettings.ShowPanic)
                        DrawIconOnBar(psiRect, ref barIconNum, Icons.Panic, ColorYellowAlert, rectAlpha);

                    if (psi && PsiSettings.ShowPanic)
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Panic, ColorYellowAlert, ViewOpacityCrit);
                }
            }

            // Drunkness percent
            if (pawnStats.Drunkness > 0.05)
            {
                if (!psi && ColBarSettings.ShowDrunk)
                    DrawIcon_FadeFloatWithThreeColors(psiRect, ref barIconNum, Icons.Drunk, pawnStats.Drunkness,
                        ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, rectAlpha);
                if (psi && PsiSettings.ShowDrunk)
                    DrawIcon_FadeFloatWithThreeColors(bodyLoc, ref iconNum, Icons.Drunk, pawnStats.Drunkness,
                        ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, ViewOpacityCrit);
            }


            // Pacifc + Unarmed

            if (pawn.story != null && pawn.story.WorkTagIsDisabled(WorkTags.Violent))
            {
                if (pawn.drafter != null && !pawn.Drafted)
                {
                    tooltip = "IsIncapableOfViolence".Translate(pawn.NameStringShort);
                    if (!psi && ColBarSettings.ShowPacific)
                        DrawIconOnBar(psiRect, ref barIconNum, Icons.Pacific, ColorNeutralStatus, rectAlpha, tooltip);
                    if (psi && PsiSettings.ShowPacific)
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Pacific, ColorNeutralStatus, viewOpacity);
                }
            }
            else if (pawn.equipment.Primary == null && !pawn.IsPrisoner)
            {
                if (!psi && ColBarSettings.ShowUnarmed)
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.Unarmed, ColorNeutralStatus, rectAlpha);
                if (psi && PsiSettings.ShowUnarmed)
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Unarmed, ColorNeutralStatus, viewOpacity);
            }


            // Trait Pyromaniac
            if (pawn.story.traits.HasTrait(TraitDefOf.Pyromaniac))
            {
                if (!psi && ColBarSettings.ShowPyromaniac)
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.Pyromaniac, ColorYellowAlert, rectAlpha);
                if (psi && PsiSettings.ShowPyromaniac)
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Pyromaniac, ColorYellowAlert, ViewOpacityCrit);
            }

            // Idle - icon only
            if (psi && PsiSettings.ShowIdle && pawn.mindState.IsIdle)
                DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Idle, ColorNeutralStatus, viewOpacity);

            MentalBreaker mb = !pawn.Dead ? pawn.mindState.mentalBreaker : null;

            // Bad Mood
            if (pawn.needs.mood.CurLevelPercentage <= mb?.BreakThresholdMinor)
            {
                if (!psi && ColBarSettings.ShowSad)
                    DrawIcon_FadeRedAlertToNeutral(psiRect, ref barIconNum, Icons.Sad,
                        pawn.needs.mood.CurLevelPercentage / mb.BreakThresholdMinor, rectAlpha);
                if (psi && PsiSettings.ShowSad)
                    DrawIcon_FadeRedAlertToNeutral(bodyLoc, ref iconNum, Icons.Sad,
                        pawn.needs.mood.CurLevelPercentage / mb.BreakThresholdMinor, ViewOpacityCrit);
            }
            //   if (psi && PsiSettings.ShowSad && colonist.needs.mood.CurLevel < (double)PsiSettings.LimitMoodLess)
            //DrawIcon_FadeRedAlertToNeutral(bodyLoc, iconNum, Icons.Sad, colonist.needs.mood.CurLevel / PsiSettings.LimitMoodLess);

            // Bloodloss
            if (pawnStats.BleedRate > 0.0f)
            {
                if (!psi && ColBarSettings.ShowBloodloss)
                {
                    DrawIcon_FadeFloatWithTwoColors(psiRect, ref barIconNum, Icons.Bloodloss, pawnStats.BleedRate,
                        ColorRedAlert, ColorNeutralStatus, rectAlpha);
                }
                if (psi && PsiSettings.ShowBloodloss)
                {
                    DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.Bloodloss, pawnStats.BleedRate,
                        ColorRedAlert, ColorNeutralStatus, viewOpacity);
                }
            }

            //Health

            float pawnHealth = pawn.health.summaryHealth.SummaryHealthPercent;
            //Infection
            if (pawnStats.HasLifeThreateningDisease)
            {
                if (pawnHealth < pawnStats.HealthDisease)
                {
                    if (!psi && ColBarSettings.ShowHealth)
                        DrawIcon_FadeFloatWithFourColorsHB(psiRect, ref barIconNum, Icons.Health, pawnHealth,
                            ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, rectAlpha);

                    if (psi && PsiSettings.ShowHealth)
                        DrawIcon_FadeFloatWithFourColorsHB(bodyLoc, ref iconNum, Icons.Health, pawnHealth,
                            ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert,
                            ViewOpacityCrit);
                }

                else
                {
                    if (!psi && ColBarSettings.ShowHealth)
                        DrawIcon_FadeFloatWithFourColorsHB(psiRect, ref barIconNum, Icons.Health, pawnStats.HealthDisease,
                            ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, rectAlpha);
                    if (psi && PsiSettings.ShowHealth)
                        DrawIcon_FadeFloatWithFourColorsHB(bodyLoc, ref iconNum, Icons.Health, pawnStats.HealthDisease,
                            ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, ViewOpacityCrit);
                }
            }
            else if (pawn.health.summaryHealth.SummaryHealthPercent < 1f)
            {
                if (!psi && ColBarSettings.ShowHealth)
                    DrawIcon_FadeFloatWithFourColorsHB(psiRect, ref barIconNum, Icons.Health, pawnHealth,
                        ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, rectAlpha);

                if (psi && PsiSettings.ShowHealth)
                    DrawIcon_FadeFloatWithFourColorsHB(bodyLoc, ref iconNum, Icons.Health, pawnHealth,
                        ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, ViewOpacityCrit);
            }

            //Toxicity buildup
            if (pawnStats.ToxicBuildUp > 0.04f)
            {
                if (!psi && ColBarSettings.ShowToxicity)
                    DrawIcon_FadeFloatFiveColors(psiRect, ref barIconNum, Icons.Toxicity, pawnStats.ToxicBuildUp,
                        ColorNeutralStatusFade, ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert,
                        rectAlpha);

                if (psi && PsiSettings.ShowToxicity)
                    DrawIcon_FadeFloatFiveColors(bodyLoc, ref iconNum, Icons.Toxicity, pawnStats.ToxicBuildUp,
                        ColorNeutralStatusFade, ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert,
                        ViewOpacityCrit);
            }


            // Sickness
            if (pawnStats.IsSick)
            {
                if (pawnStats.DiseaseDisappearance < PsiSettings.LimitDiseaseLess)
                {
                    if (!psi && ColBarSettings.ShowMedicalAttention)
                        DrawIcon_FadeFloatWithFourColorsHB(psiRect, ref barIconNum, Icons.Sickness,
                            pawnStats.DiseaseDisappearance / ColBarSettings.LimitDiseaseLess, ColorNeutralStatus,
                            ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, rectAlpha);
                    if (psi && PsiSettings.ShowMedicalAttention)
                        DrawIcon_FadeFloatWithFourColorsHB(bodyLoc, ref iconNum, Icons.Sickness,
                            pawnStats.DiseaseDisappearance / PsiSettings.LimitDiseaseLess, ColorNeutralStatus,
                            ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, ViewOpacityCrit);
                }
                else
                {
                    if (!psi && ColBarSettings.ShowMedicalAttention)
                        DrawIconOnBar(psiRect, ref barIconNum, Icons.Sickness, ColorNeutralStatus, rectAlpha);
                    if (psi && PsiSettings.ShowMedicalAttention)
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Sickness, ColorNeutralStatus, viewOpacity);
                }
            }

            // Pain
            if (ColBarSettings.ShowPain || PsiSettings.ShowPain)
            {
                if (pawnStats.PainMoodLevel > -1)
                {
                    Color color = new Color();
                    bool isMasochist = pawn.story.traits.HasTrait(TraitDef.Named("Masochist"));
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
                    tooltip =  HealthCardUtility.GetPainTip(pawn);
                    if (!psi && ColBarSettings.ShowPain)
                        DrawIconOnBar(psiRect, ref barIconNum, Icons.Pain, color, rectAlpha, tooltip);
                    if (psi && PsiSettings.ShowPain)
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Pain, color, viewOpacity);
                }
            }
            if (HealthAIUtility.ShouldBeTendedNowUrgent(pawn))
            {
                if (!psi && ColBarSettings.ShowMedicalAttention)
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.MedicalAttention, ColorRedAlert, rectAlpha);
                if (psi && PsiSettings.ShowMedicalAttention)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.MedicalAttention, ColorRedAlert, ViewOpacityCrit);
                }
            }
            else if (HealthAIUtility.ShouldBeTendedNow(pawn))
            {
                if (!psi && ColBarSettings.ShowMedicalAttention)
                {
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.MedicalAttention, ColorYellowAlert, rectAlpha);
                }
                if (psi && PsiSettings.ShowMedicalAttention)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.MedicalAttention, ColorYellowAlert, ViewOpacityCrit);
                }
            }
            else if (HealthAIUtility.ShouldHaveSurgeryDoneNow(pawn))
            {
                if (!psi && ColBarSettings.ShowMedicalAttention)
                {
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.MedicalAttention, ColorYellowAlert, rectAlpha);
                }
                if (psi && PsiSettings.ShowMedicalAttention)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.MedicalAttention, ColorYellowAlert, ViewOpacityCrit);
                }
            }

            // Hungry
            if (pawn.needs.food.CurLevel < (double)PsiSettings.LimitFoodLess)
            {
                if (!psi && ColBarSettings.ShowHungry)
                {
                    tooltip = pawn.needs.food.GetTipString();
                    DrawIcon_FadeRedAlertToNeutral(psiRect, ref barIconNum, Icons.Hungry,
                        pawn.needs.food.CurLevel / PsiSettings.LimitFoodLess, rectAlpha, tooltip);}
                if (psi && PsiSettings.ShowHungry)
                    DrawIcon_FadeRedAlertToNeutral(bodyLoc, ref iconNum, Icons.Hungry,
                        pawn.needs.food.CurLevel / PsiSettings.LimitFoodLess, ViewOpacityCrit);
            }

            //Tired
            if (pawn.needs.rest.CurLevel < (double)PsiSettings.LimitRestLess)
            {
                if (!psi && ColBarSettings.ShowTired && pawn.needs.rest.CurLevel < (double)PsiSettings.LimitRestLess)
                    DrawIcon_FadeRedAlertToNeutral(psiRect, ref barIconNum, Icons.Tired,
                        pawn.needs.rest.CurLevel / PsiSettings.LimitRestLess, rectAlpha);

                if (psi && PsiSettings.ShowTired)
                    DrawIcon_FadeRedAlertToNeutral(bodyLoc, ref iconNum, Icons.Tired,
                        pawn.needs.rest.CurLevel / PsiSettings.LimitRestLess, ViewOpacityCrit);
            }

            // Too Cold & too hot
            if (pawnStats.TooCold > 0f)
            {
                if (pawnStats.TooCold <= 1f)
                {
                    if (!psi && ColBarSettings.ShowTooCold)
                        DrawIcon_FadeFloatWithTwoColors(psiRect, ref barIconNum, Icons.TooCold, pawnStats.TooCold,
                            ColorNeutralStatusFade, ColorYellowAlert, rectAlpha);
                    if (psi && PsiSettings.ShowTooCold)
                        DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.TooCold, pawnStats.TooCold,
                            ColorNeutralStatusFade, ColorYellowAlert, ViewOpacityCrit);
                }
                else if (pawnStats.TooCold <= 1.5f)
                {
                    if (!psi && ColBarSettings.ShowTooCold)
                        DrawIcon_FadeFloatWithTwoColors(psiRect, ref barIconNum, Icons.TooCold, (pawnStats.TooCold - 1f) * 2f,
                            ColorYellowAlert, ColorOrangeAlert, rectAlpha);
                    if (psi && PsiSettings.ShowTooCold)
                    {
                        DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.TooCold, (pawnStats.TooCold - 1f) * 2f,
                            ColorYellowAlert, ColorOrangeAlert, ViewOpacityCrit);
                    }
                }
                else
                {
                    if (!psi && ColBarSettings.ShowTooCold)
                        DrawIcon_FadeFloatWithTwoColors(psiRect, ref barIconNum, Icons.TooCold,
                            (pawnStats.TooCold - 1.5f) * 2f, ColorOrangeAlert, ColorRedAlert, rectAlpha);
                    if (psi && PsiSettings.ShowTooCold)
                    {
                        DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.TooCold,
                            (pawnStats.TooCold - 1.5f) * 2f, ColorOrangeAlert, ColorRedAlert, ViewOpacityCrit);
                    }
                }
            }

            else if (pawnStats.TooHot > 0f)
            {
                if (pawnStats.TooHot <= 1f)
                {
                    if (!psi && ColBarSettings.ShowTooHot)
                        DrawIcon_FadeFloatWithTwoColors(psiRect, ref barIconNum, Icons.TooHot, pawnStats.TooHot,
                            ColorNeutralStatusFade, ColorYellowAlert, rectAlpha);
                    if (psi && PsiSettings.ShowTooHot)
                        DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.TooHot, pawnStats.TooHot,
                            ColorNeutralStatusFade, ColorYellowAlert, ViewOpacityCrit);
                }
                else if (pawnStats.TooHot <= 1.5f)
                {
                    if (!psi && ColBarSettings.ShowTooHot)
                        DrawIcon_FadeFloatWithTwoColors(psiRect, ref barIconNum, Icons.TooHot, pawnStats.TooHot,
                            ColorYellowAlert, ColorOrangeAlert, rectAlpha);
                    if (psi && PsiSettings.ShowTooHot)
                        DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.TooHot, pawnStats.TooHot,
                            ColorYellowAlert, ColorOrangeAlert, ViewOpacityCrit);
                }
                else
                {
                    if (!psi && ColBarSettings.ShowTooHot)
                        DrawIcon_FadeFloatWithTwoColors(psiRect, ref barIconNum, Icons.TooHot, pawnStats.TooHot - 1f,
                            ColorOrangeAlert, ColorRedAlert, rectAlpha);
                    if (psi && PsiSettings.ShowTooHot)
                        DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.TooHot, pawnStats.TooHot - 1f,
                            ColorOrangeAlert, ColorRedAlert, ViewOpacityCrit);
                }
            }

            // Bed status
            if (!pawnStats.HasBed)
            {
                if (!psi && ColBarSettings.ShowBedroom)
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.Bedroom, Color10To06, rectAlpha);
                if (psi && PsiSettings.ShowBedroom)
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Bedroom, Color10To06, ViewOpacityCrit);
            }


            // Usage of bed ...
            if (HasThought(pawn, ThoughtDef.Named("WantToSleepWithSpouseOrLover")))
            {
                if (!psi && ColBarSettings.ShowLove)
                {
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.Love, ColorYellowAlert, rectAlpha);
                }
                if (psi && PsiSettings.ShowLove)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Love, ColorYellowAlert, ViewOpacityCrit);
                }
            }


            //  if (psi && PsiSettings.ShowMarriage && HasThought(colonist, ThoughtDef.Named("GotMarried")))
            //  {
            //      DrawIcon(bodyLoc, ref iconNum, Icons.Marriage, colorMoodBoost);
            //  }
            //
            //  if (psi && PsiSettings.ShowMarriage && HasThought(colonist, ThoughtDef.Named("HoneymoonPhase")))
            //  {
            //      DrawIcon(bodyLoc, ref iconNum, Icons.Marriage, colorMoodBoost / 2);
            //  }
            //
            //  if (psi && PsiSettings.ShowMarriage && HasThought(colonist, ThoughtDef.Named("AttendedWedding")))
            //  {
            //      DrawIcon(bodyLoc, ref iconNum, Icons.Marriage, colorMoodBoost / 4);
            //  }

            // Naked
            if (HasThought(pawn, ThoughtDefOf.Naked))
            {
                // Naked
                if (!psi && ColBarSettings.ShowNaked)
                {
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.Naked, Color10To06, rectAlpha);
                }
                if (psi && PsiSettings.ShowNaked)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Naked, Color10To06, ViewOpacityCrit);
                }
            }

            // Apparel
            if (pawnStats.ApparelHealth < (double)PsiSettings.LimitApparelHealthLess)
            {
                if (!psi && ColBarSettings.ShowApparelHealth)
                {
                    double pawnApparelHealth = pawnStats.ApparelHealth / (double)ColBarSettings.LimitApparelHealthLess;
                    DrawIcon_FadeRedAlertToNeutral(psiRect, ref barIconNum, Icons.ApparelHealth, (float)pawnApparelHealth,
                        rectAlpha);
                }
                if (psi && PsiSettings.ShowApparelHealth)
                {
                    double pawnApparelHealth = pawnStats.ApparelHealth / (double)PsiSettings.LimitApparelHealthLess;
                    DrawIcon_FadeRedAlertToNeutral(bodyLoc, ref iconNum, Icons.ApparelHealth, (float)pawnApparelHealth,
                        ViewOpacityCrit);
                }
            }

            // Moods caused by traits

            if (HasThought(pawn, ThoughtDef.Named("ProsthophileNoProsthetic")))
            {
                if (!psi && ColBarSettings.ShowProsthophile)
                {
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.Prosthophile, Color05AndLess, rectAlpha);
                }
                if (psi && PsiSettings.ShowProsthophile)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Prosthophile, Color05AndLess, viewOpacity);
                }
            }

            if (HasThought(pawn, ThoughtDef.Named("ProsthophobeUnhappy")))
            {
                if (!psi && ColBarSettings.ShowProsthophobe)
                {
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.Prosthophobe, Color10To06, rectAlpha);
                }
                if (psi && PsiSettings.ShowProsthophobe)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Prosthophobe, Color10To06, viewOpacity);
                }
            }

            if (HasThought(pawn, ThoughtDef.Named("NightOwlDuringTheDay")))
            {
                if (!psi && ColBarSettings.ShowNightOwl)
                {
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.NightOwl, Color10To06, rectAlpha);
                }
                if (psi && PsiSettings.ShowNightOwl)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.NightOwl, Color10To06, ViewOpacityCrit);
                }
            }

            if (HasThought(pawn, ThoughtDef.Named("Greedy")))
            {
                if (!psi && ColBarSettings.ShowGreedy)
                {
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.Greedy, Color10To06, rectAlpha);
                }
                if (psi && PsiSettings.ShowGreedy)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Greedy, Color10To06, viewOpacity);
                }
            }

            if (HasThought(pawn, ThoughtDef.Named("Jealous")))
            {
                if (!psi && ColBarSettings.ShowJealous)
                {
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.Jealous, Color10To06, rectAlpha);
                }
                if (psi && PsiSettings.ShowJealous)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Jealous, Color10To06, viewOpacity);
                }
            }


            // Effectiveness
            if (pawnStats.TotalEfficiency < (double)PsiSettings.LimitEfficiencyLess)
            {
                if (!psi && ColBarSettings.ShowEffectiveness)
                    DrawIcon_FadeRedAlertToNeutral(psiRect, ref barIconNum, Icons.Effectiveness,
                        pawnStats.TotalEfficiency / PsiSettings.LimitEfficiencyLess, rectAlpha);

                if (psi && PsiSettings.ShowEffectiveness)
                {
                    DrawIcon_FadeRedAlertToNeutral(bodyLoc, ref iconNum, Icons.Effectiveness,
                        pawnStats.TotalEfficiency / PsiSettings.LimitEfficiencyLess, ViewOpacityCrit);
                }
            }

            // Bad thoughts

            if (HasThought(pawn, ThoughtDef.Named("ColonistLeftUnburied")))
            {
                if (!psi && ColBarSettings.ShowLeftUnburied)
                {
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.LeftUnburied, Color10To06, rectAlpha);
                }
                if (psi && PsiSettings.ShowLeftUnburied)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.LeftUnburied, Color10To06, ViewOpacityCrit);
                }
            }

            if (psi && PsiSettings.ShowDeadColonists)
            {
                // Close Family & friends / 25


                // not family, more whiter icon
                if (HasThought(pawn, ThoughtDef.Named("KilledColonist")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }

                if (HasThought(pawn, ThoughtDef.Named("KilledColonyAnimal")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }

                #region DeathMemory

                //Deathmemory
                // some of those need staging - to do
                if (HasThought(pawn, ThoughtDef.Named("KnowGuestExecuted")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }
                if (HasThought(pawn, ThoughtDef.Named("KnowColonistExecuted")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }
                if (HasThought(pawn, ThoughtDef.Named("KnowPrisonerDiedInnocent")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
                }
                if (HasThought(pawn, ThoughtDef.Named("KnowColonistDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }
                //Bonded animal died
                if (HasThought(pawn, ThoughtDef.Named("BondedAnimalDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
                }
                // Friend / rival died
                if (HasThought(pawn, ThoughtDef.Named("PawnWithGoodOpinionDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
                }

                if (HasThought(pawn, ThoughtDef.Named("PawnWithBadOpinionDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, ColorMoodBoost, viewOpacity);
                }

                #endregion

                #region DeathMemoryFamily

                if (HasThought(pawn, ThoughtDef.Named("MySonDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color25To21, viewOpacity);
                }

                if (HasThought(pawn, ThoughtDef.Named("MyDaughterDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color25To21, viewOpacity);
                }

                if (HasThought(pawn, ThoughtDef.Named("MyHusbandDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color25To21, viewOpacity);
                }

                if (HasThought(pawn, ThoughtDef.Named("MyWifeDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color25To21, viewOpacity);
                }

                if (HasThought(pawn, ThoughtDef.Named("MyFianceDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color20To16, viewOpacity);
                }

                if (HasThought(pawn, ThoughtDef.Named("MyFianceeDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color20To16, viewOpacity);
                }

                if (HasThought(pawn, ThoughtDef.Named("MyLoverDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color20To16, viewOpacity);
                }

                if (HasThought(pawn, ThoughtDef.Named("MyBrotherDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color15To11, viewOpacity);
                }

                if (HasThought(pawn, ThoughtDef.Named("MySisterDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color15To11, viewOpacity);
                }

                if (HasThought(pawn, ThoughtDef.Named("MyGrandchildDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color15To11, viewOpacity);
                }

                // 10

                if (HasThought(pawn, ThoughtDef.Named("MyFatherDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
                }

                if (HasThought(pawn, ThoughtDef.Named("MyMotherDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
                }

                if (HasThought(pawn, ThoughtDef.Named("MyNieceDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }

                if (HasThought(pawn, ThoughtDef.Named("MyNephewDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }

                if (HasThought(pawn, ThoughtDef.Named("MyHalfSiblingDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }

                if (HasThought(pawn, ThoughtDef.Named("MyAuntDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }

                if (HasThought(pawn, ThoughtDef.Named("MyUncleDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }

                if (HasThought(pawn, ThoughtDef.Named("MyGrandparentDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }

                if (HasThought(pawn, ThoughtDef.Named("MyCousinDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }
                if (HasThought(pawn, ThoughtDef.Named("MyKinDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }

                #endregion

                //Memory misc
                if (HasThought(pawn, ThoughtDef.Named("WitnessedDeathAlly")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
                }
                if (HasThought(pawn, ThoughtDef.Named("WitnessedDeathNonAlly")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }
                if (HasThought(pawn, ThoughtDef.Named("WitnessedDeathFamily")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
                }
                if (HasThought(pawn, ThoughtDef.Named("WitnessedDeathBloodlust")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, ColorMoodBoost, viewOpacity);
                }
                if (HasThought(pawn, ThoughtDef.Named("KilledHumanlikeBloodlust")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, ColorMoodBoost, viewOpacity);
                }

                // CabinFever missing since A14?

                if (pawnStats.CabinFeverMoodLevel >= 0)
                {
                    if (pawnStats.CabinFeverMoodLevel == 0)
                    {
                        if (!psi && ColBarSettings.ShowCabinFever)
                            DrawIconOnBar(psiRect, ref barIconNum, Icons.CabinFever, ColorNeutralStatusFade, rectAlpha);
                        if (psi && PsiSettings.ShowCabinFever)
                            DrawIconOnColonist(bodyLoc, ref iconNum, Icons.CabinFever, ColorNeutralStatusFade,
                                ViewOpacityCrit);
                    }
                    if (pawnStats.CabinFeverMoodLevel == 1)
                    {
                        if (!psi && ColBarSettings.ShowCabinFever)
                            DrawIconOnBar(psiRect, ref barIconNum, Icons.CabinFever, ColorYellowAlert, rectAlpha);
                        if (psi && PsiSettings.ShowCabinFever)
                            DrawIconOnColonist(bodyLoc, ref iconNum, Icons.CabinFever, ColorYellowAlert, ViewOpacityCrit);
                    }
                    if (pawnStats.CabinFeverMoodLevel == 2)
                    {
                        if (!psi && ColBarSettings.ShowCabinFever)
                            DrawIconOnBar(psiRect, ref barIconNum, Icons.CabinFever, ColorOrangeAlert, rectAlpha);
                        if (psi && PsiSettings.ShowCabinFever)
                            DrawIconOnColonist(bodyLoc, ref iconNum, Icons.CabinFever, ColorOrangeAlert, ViewOpacityCrit);
                    }
                }
            }
        }

        #endregion
    }
}