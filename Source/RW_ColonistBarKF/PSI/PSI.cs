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
    using UnityEngine;
    using Verse;
    using Verse.AI;

    public class PSI : GameComponent
    {
        public static Vector3[] IconPosRectsBar;

        public static Vector3[] IconPosVectorsPsi;

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
            DrawIconOnColonist(bodyPos, ref num, Icons.Aggressive, ColVermillion, ViewOpacityCrit);
        }

        public static void DrawColonistIconsBar(Pawn pawn, Rect psiRect, float rectAlpha)
        {
            PawnStats pawnStats = pawn.GetCache();

            if (pawnStats == null)
            {
                return;
            }

            if (pawn.Dead || !pawn.Spawned || pawn.holdingOwner == null || pawn.Map == null)
            {
                pawnStats.thisColCount = 0;
                return;
            }

            CheckStats(pawnStats);

            SettingsColonistBar colBarSettings = Settings.ColBarSettings;

            int barIconNum = 0;

            // Drafted
            int rowCount = pawnStats.thisColCount;

            if (colBarSettings.ShowDraft && pawn.Drafted)
            {
                if (pawnStats.isPacifist)
                {
                    string tooltip = "IsIncapableOfViolence".Translate(pawn.NameStringShort);
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.Pacific, ColYellow, rectAlpha, rowCount, tooltip);
                }
                else
                {
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.Draft, ColVermillion, rectAlpha, rowCount);
                }
            }

            if (pawnStats.MentalSanity != null)
            {
                // Berserk
                if (colBarSettings.ShowAggressive && pawn.InAggroMentalState)
                {
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.Aggressive, ColVermillion, rectAlpha, rowCount);
                }

                // Give Up Exit
                if (colBarSettings.ShowLeave)
                {
                    if (pawnStats.MentalSanity == MentalStateDefOf.PanicFlee)
                    {
                        // was GiveUpExit
                        DrawIconOnBar(psiRect, ref barIconNum, Icons.Leave, ColVermillion, rectAlpha, rowCount);
                    }
                }

                // Daze Wander
                if (colBarSettings.ShowDazed)
                {
                    if (pawnStats.MentalSanity == MentalStateDefOf.WanderSad)
                    {
                        // + MentalStateDefOf.WanderPsychotic
                        DrawIconOnBar(psiRect, ref barIconNum, Icons.Dazed, ColYellow, rectAlpha, rowCount);
                    }
                }

                // PanicFlee
                if (colBarSettings.ShowPanic)
                {
                    if (pawnStats.MentalSanity == MentalStateDefOf.PanicFlee)
                    {
                        DrawIconOnBar(psiRect, ref barIconNum, Icons.Panic, ColYellow, rectAlpha, rowCount);
                    }
                }
            }

            // Bloodloss
            if (colBarSettings.ShowBloodloss)
            {
                if (pawnStats.BleedRate > 0.0f)
                {
                    string tooltip = "BleedingRate".Translate() + ": "
                                     + pawn.health.hediffSet.BleedRateTotal.ToStringPercent() + "/d";
                    DrawIconOnBar(
                        psiRect,
                        ref barIconNum,
                        Icons.Bloodloss,
                        gradientRedAlertToNeutral.Evaluate(1.0f - pawnStats.BleedRate),
                        rectAlpha,
                        rowCount,
                        tooltip);
                }
            }

            // Health
            // Infection
            if (colBarSettings.ShowHealth)
            {
                if (pawnStats.IsSick)
                {
                    string tooltip = pawnStats.sickTip + "\n" + pawnStats.healthTip + "\n" + "Immunity".Translate()
                                     + " / " + "PSI.DiseaseProgress".Translate() + ": \n"
                                     + pawnStats.immunity.ToStringPercent() + " / "
                                     + pawnStats.severity.ToStringPercent() + ": \n" + pawnStats.sickMoodOffset;

                    if (pawnStats.DiseaseDisappearance < Settings.PsiSettings.LimitDiseaseLess)
                    {
                        // Regular Sickness
                        DrawIconOnBar(
                            psiRect,
                            ref barIconNum,
                            Icons.Health,
                            gradient4.Evaluate(pawnStats.DiseaseDisappearance / colBarSettings.LimitDiseaseLess),
                            rectAlpha,
                            rowCount,
                            tooltip);
                    }
                    else
                    {
                        {
                            // if (pawnStats.HasLifeThreateningDisease)
                            // if (pawnStats.pawnHealth < pawnStats.HealthDisease)
                            DrawIconOnBar(
                                psiRect,
                                ref barIconNum,
                                Icons.Health,
                                gradient4.Evaluate(pawnStats.pawnHealth),
                                rectAlpha,
                                rowCount,
                                tooltip);
                        }
                    }
                }
                else if (pawn.health.summaryHealth.SummaryHealthPercent < 1f)
                {
                    string tooltip = "Health".Translate() + ": "
                                     + pawn.health.summaryHealth.SummaryHealthPercent.ToStringPercent();
                    DrawIconOnBar(
                        psiRect,
                        ref barIconNum,
                        Icons.Health,
                        gradient4.Evaluate(pawnStats.pawnHealth),
                        rectAlpha,
                        rowCount,
                        tooltip);
                }

                // else
                // {
                // string tooltip = pawnStats.sickTip;
                // DrawIcon_FadeFloatWithFourColorsHB(psiRect, ref barIconNum, Icons.Health, pawnStats.HealthDisease,
                // ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, rectAlpha, tooltip);
                // }
            }

            // Toxicity buildup
            if (colBarSettings.ShowToxicity && pawnStats.ToxicBuildUpVisible > 0f)
            {
                string tooltip = pawnStats.toxicTip;

                DrawIconOnBar(
                    psiRect,
                    ref barIconNum,
                    Icons.Toxicity,
                    gradient4.Evaluate(pawnStats.ToxicBuildUpVisible),
                    rectAlpha,
                    rowCount,
                    tooltip);
            }

            if (colBarSettings.ShowMedicalAttention)
            {
                // Check for map, else error in HealthUtility
                if (pawn.Map != null)
                {
                    string tooltip = "NeedsTendingNow".Translate();
                    if (pawnStats.ShouldBeTendedNowUrgent)
                    {
                        DrawIconOnBar(
                            psiRect,
                            ref barIconNum,
                            Icons.MedicalAttention,
                            ColVermillion,
                            rectAlpha,
                            rowCount,
                            tooltip);
                    }
                    else if (pawnStats.ShouldBeTendedNow)
                    {
                        DrawIconOnBar(psiRect, ref barIconNum, Icons.MedicalAttention, ColYellow, rectAlpha, rowCount, tooltip);
                    }
                    else if (pawnStats.ShouldHaveSurgeryDoneNow)
                    {
                        DrawIconOnBar(
                            psiRect,
                            ref barIconNum,
                            Icons.MedicalAttention,
                            ColBlueishGreen,
                            rectAlpha,
                            rowCount,
                            tooltip);
                    }
                }
            }

            // Hungry
            if (colBarSettings.ShowHungry && pawn.needs.food.CurLevel < (double)Settings.PsiSettings.LimitFoodLess)
            {
                string tooltip = pawn.needs.food.GetTipString();
                DrawIconOnBar(
                    psiRect,
                    ref barIconNum,
                    Icons.Hungry,
                    gradientRedAlertToNeutral.Evaluate(pawn.needs.food.CurLevel / Settings.PsiSettings.LimitFoodLess),
                    rectAlpha,
                    rowCount,
                    tooltip);
            }

            // Tired
            if (colBarSettings.ShowTired && pawn.needs.rest.CurLevel < (double)Settings.PsiSettings.LimitRestLess)
            {
                string tooltip = pawn.needs.rest.CurCategory.GetLabel();
                DrawIconOnBar(
                    psiRect,
                    ref barIconNum,
                    Icons.Tired,
                    gradientRedAlertToNeutral.Evaluate(pawn.needs.rest.CurLevel / Settings.PsiSettings.LimitRestLess),
                    rectAlpha,
                    rowCount,
                    tooltip);
            }

            // Idle - bar icon already included - vanilla
            bool pacifist = pawnStats.isPacifist;

            // Pacifc
            if (colBarSettings.ShowUnarmed)
            {
                if (pawn.equipment.Primary == null && !pawn.IsPrisoner && !pacifist)
                {
                    string tooltip = "PSI.Settings.Visibility.Unarmed".Translate();
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.Unarmed, ColorNeutralStatusOpaque, rectAlpha, rowCount, tooltip);
                }
            }

            if (colBarSettings.ShowPacific && pacifist)
            {
                if (pawn.drafter != null && !pawn.Drafted)
                {
                    string tooltip = "IsIncapableOfViolence".Translate(pawn.NameStringShort);
                    DrawIconOnBar(psiRect, ref barIconNum, Icons.Pacific, ColBlueishGreen, rectAlpha, rowCount, tooltip);
                }
            }

            // Trait Pyromaniac
            if (colBarSettings.ShowPyromaniac && pawnStats.isPyromaniac)
            {
                string tooltip = "PSI.Settings.Visibility.Pyromaniac".Translate();
                DrawIconOnBar(psiRect, ref barIconNum, Icons.Pyromaniac, ColYellow, rectAlpha, rowCount, tooltip);
            }

            // Addictions
            if (colBarSettings.ShowDrunk && (pawnStats.isAddict || pawnStats.drugDesire != 0))
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

                DrawIconOnBar(psiRect, ref barIconNum, Icons.Drunk, color, rectAlpha, rowCount, tooltip);
            }

            // Bad Mood
            // No need for an icon if I have a mood bar.

            // if (psi && PsiSettings.ShowSad && pawn.needs.mood.CurLevel < (double)PsiSettings.LimitMoodLess)
            // DrawIcon_FadeRedAlertToNeutral(bodyLoc, iconNum, Icons.Sad, pawn.needs.mood.CurLevel / PsiSettings.LimitMoodLess);

            // Too Cold & too hot
            if (colBarSettings.ShowTooCold && pawnStats.TooCold > 0f)
            {
                DrawIconOnBar(
                    psiRect,
                    ref barIconNum,
                    Icons.TooCold,
                    gradient4.Evaluate(pawnStats.TooCold / 2),
                    rectAlpha,
                    rowCount);
            }

            if (colBarSettings.ShowTooHot && pawnStats.TooHot > 0f)
            {
                DrawIconOnBar(
                    psiRect,
                    ref barIconNum,
                    Icons.TooHot,
                    gradient4.Evaluate(pawnStats.TooHot / 2),
                    rectAlpha,
                    rowCount);
            }

            // Pain
            if (colBarSettings.ShowPain && pawnStats.PainMoodLevel > -1)
            {
                string tooltip = pawnStats.painTip;

                DrawIconOnBar(
                    psiRect,
                    ref barIconNum,
                    Icons.Pain,
                    Evaluate(pawnStats.PainMoodOffset),
                    rectAlpha,
                    rowCount,
                    tooltip);
            }

            // Bed status
            if (colBarSettings.ShowBedroom)
            {
                if (pawnStats.wantsToHump > -1)
                {
                    DrawIconOnBar(
                        psiRect,
                        ref barIconNum,
                        Icons.Bedroom,
                        Evaluate(pawnStats.humpMoodOffset),
                        rectAlpha,
                        rowCount,
                        pawnStats.humpTip);
                }
                else if (pawnStats.BedStatus > -1)
                {
                    DrawIconOnBar(
                        psiRect,
                        ref barIconNum,
                        Icons.Bedroom,
                        Evaluate(pawnStats.BedStatusMoodOffset),
                        rectAlpha,
                        rowCount,
                        pawnStats.BedStatusTip);
                }
            }

            // Naked
            if (colBarSettings.ShowNaked)
            {
                if (pawnStats.feelsNaked > -1)
                {
                    string tooltip = pawnStats.nakedTip;
                    DrawIconOnBar(
                        psiRect,
                        ref barIconNum,
                        Icons.Naked,
                        Evaluate(pawnStats.nakedMoodOffset),
                        rectAlpha,
                        rowCount,
                        tooltip);
                }
            }

            // Apparel
            if (colBarSettings.ShowApparelHealth)
            {
                if (pawnStats.ApparelHealth < (double)Settings.PsiSettings.LimitApparelHealthLess)
                {
                    double pawnApparelHealth = pawnStats.ApparelHealth / (double)colBarSettings.LimitApparelHealthLess;
                    DrawIconOnBar(
                        psiRect,
                        ref barIconNum,
                        Icons.ApparelHealth,
                        gradientRedAlertToNeutral.Evaluate((float)pawnApparelHealth),
                        rectAlpha,
                        rowCount);
                }
            }

            // Moods caused by traits
            if (pawnStats.prosthoUnhappy > -1)
            {
                if (pawnStats.prostho == 1)
                {
                    if (colBarSettings.ShowProsthophile)
                    {
                        DrawIconOnBar(
                            psiRect,
                            ref barIconNum,
                            Icons.Prosthophile,
                            Evaluate(pawnStats.prosthoMoodOffset),
                            rectAlpha,
                            rowCount,
                            pawnStats.prosthoTooltip);
                    }
                }

                if (pawnStats.prostho == -1)
                {
                    if (colBarSettings.ShowProsthophobe)
                    {
                        DrawIconOnBar(
                            psiRect,
                            ref barIconNum,
                            Icons.Prosthophobe,
                            Evaluate(pawnStats.prosthoMoodOffset),
                            rectAlpha,
                            rowCount,
                            pawnStats.prosthoTooltip);
                    }
                }
            }

            if (colBarSettings.ShowNightOwl)
            {
                if (pawnStats.nightOwlUnhappy > -1)
                {
                    DrawIconOnBar(
                        psiRect,
                        ref barIconNum,
                        Icons.NightOwl,
                        Evaluate(pawnStats.nightOwlMoodOffset),
                        rectAlpha,
                        rowCount,
                        pawnStats.nightOwlTip);
                }
            }

            if (colBarSettings.ShowGreedy && pawnStats.greedyThought > -1)
            {
                DrawIconOnBar(
                    psiRect,
                    ref barIconNum,
                    Icons.Greedy,
                    Evaluate(pawnStats.greedyMoodOffset),
                    rectAlpha,
                    rowCount,
                    pawnStats.greedyTooltip);
            }

            if (colBarSettings.ShowJealous && pawnStats.jealousThought > -1)
            {
                DrawIconOnBar(
                    psiRect,
                    ref barIconNum,
                    Icons.Jealous,
                    Evaluate(pawnStats.jealousMoodOffset),
                    rectAlpha,
                    rowCount,
                    pawnStats.jealousTooltip);
            }

            // Effectiveness
            if (colBarSettings.ShowEffectiveness)
            {
                if (pawnStats.TotalEfficiency < (double)Settings.PsiSettings.LimitEfficiencyLess)
                {
                    string tooltip = "PSI.Efficiency".Translate() + ": " + pawnStats.efficiencyTip + " "
                                     + pawnStats.TotalEfficiency.ToStringPercent();
                    DrawIconOnBar(
                        psiRect,
                        ref barIconNum,
                        Icons.Effectiveness,
                        gradientRedAlertToNeutral.Evaluate(
                            pawnStats.TotalEfficiency / Settings.PsiSettings.LimitEfficiencyLess),
                        rectAlpha,
                        rowCount,
                        tooltip);
                }
            }

            // Bad thoughts
            if (colBarSettings.ShowLeftUnburied)
            {
                if (pawnStats.unburied > -1)
                {
                    string tooltip = pawnStats.unburiedTip;
                    DrawIconOnBar(
                        psiRect,
                        ref barIconNum,
                        Icons.LeftUnburied,
                        Evaluate(pawnStats.unburiedMoodOffset),
                        rectAlpha,
                        rowCount,
                        tooltip);
                }
            }

            // CabinFever missing since A14?
            if (colBarSettings.ShowCabinFever && pawnStats.CabinFeverMoodLevel > -1)
            {
                string tooltip = pawnStats.cabinFeverTip;
                DrawIconOnBar(
                    psiRect,
                    ref barIconNum,
                    Icons.CabinFever,
                    Evaluate(pawnStats.CabinFeverMoodOffset),
                    rectAlpha,
                    rowCount,
                    tooltip);
            }

            // if (barIconNum >= colBarSettings.IconsInColumn * 2)
            // {
            // pawnStats.IconCount = colBarSettings.IconsInColumn * 2;
            // }
            // else
            int colCount = Mathf.CeilToInt((float)barIconNum / Settings.ColBarSettings.IconsInColumn);

            pawnStats.thisColCount = colCount;

            // int newCount = PawnstatsChecker.PawnCache.Aggregate(
            // 0,
            // (current, colonist) => Mathf.Max(current, colonist.thisColCount));
            // if (newCount != ColonistBar_KF.PsiRowsOnBar)
            // {
            // ColonistBar_KF.PsiRowsOnBar = newCount;
            // ColonistBar_KF.MarkColonistsDirty();
            // }
        }

        private static Color Evaluate(float moodOffset)
        {
            return gradient4Mood.Evaluate(Mathf.InverseLerp(-25f, 15f, moodOffset));
        }

        public static void CheckStats(Pawn pawn)
        {
            PawnStats pawnStats = pawn.GetCache();
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

                pawn.GetCache().hasRelationWithColonist = true;
                skip = true;

            }
            if (!skip)
            {
                if (pawn.relations.DirectRelations.Any(x => x.otherPawn.IsColonist))
                {
                    pawn.GetCache().hasRelationWithColonist = true;
                }
            }
            pawn.GetCache().relationChecked = true;

        }

        public static void DrawColonistIconsPSI(Pawn pawn)
        {
            if (pawn.Dead || !pawn.Spawned || pawn.holdingOwner == null || pawn.Map == null)
            {
                return;
            }

            PawnStats pawnStats = pawn.GetCache();

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
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Pacific, ColYellow, ViewOpacityCrit);
                }
                else
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Draft, ColVermillion, ViewOpacityCrit);
                }
            }

            if (pawnStats.MentalSanity != null)
            {
                // Berserk
                if (psiSettings.ShowAggressive && pawn.InAggroMentalState)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Aggressive, ColVermillion, ViewOpacityCrit);
                }

                // Give Up Exit
                if (psiSettings.ShowLeave && pawnStats.MentalSanity == MentalStateDefOf.PanicFlee)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Leave, ColVermillion, ViewOpacityCrit);
                }

                // Daze Wander
                if (psiSettings.ShowDazed && pawnStats.MentalSanity == MentalStateDefOf.WanderSad)
                {
                    // + MentalStateDefOf.WanderPsychotic
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Dazed, ColYellow, ViewOpacityCrit);
                }

                // PanicFlee
                if (psiSettings.ShowPanic && pawnStats.MentalSanity == MentalStateDefOf.PanicFlee)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Panic, ColYellow, ViewOpacityCrit);
                }
            }

            // Bloodloss
            if (psiSettings.ShowBloodloss && pawnStats.BleedRate > 0.0f)
            {
                DrawIconOnColonist(
                    bodyLoc,
                    ref iconNum,
                    Icons.Bloodloss,
                    gradientRedAlertToNeutral.Evaluate(1.0f - pawnStats.BleedRate),
                    viewOpacity);
            }

            // Health

            // Infection
            if (psiSettings.ShowHealth)
            {
                if (pawnStats.IsSick)
                {
                    // Regular Sickness
                    if (pawnStats.DiseaseDisappearance < psiSettings.LimitDiseaseLess)
                    {
                        DrawIconOnColonist(
                            bodyLoc,
                            ref iconNum,
                            Icons.Health,
                            gradient4.Evaluate(pawnStats.DiseaseDisappearance / psiSettings.LimitDiseaseLess),
                            ViewOpacityCrit);
                    }
                    else
                    {
                        {
                            // if (pawnStats.HasLifeThreateningDisease)
                            DrawIconOnColonist(
                                bodyLoc,
                                ref iconNum,
                                Icons.Health,
                                gradient4.Evaluate(pawnStats.pawnHealth),
                                ViewOpacityCrit);
                        }
                    }
                }
                else if (pawn.health.summaryHealth.SummaryHealthPercent < 1f)
                {
                    DrawIconOnColonist(
                        bodyLoc,
                        ref iconNum,
                        Icons.Health,
                        gradient4.Evaluate(pawnStats.pawnHealth),
                        ViewOpacityCrit);
                }
            }

            // Toxicity buildup
            if (psiSettings.ShowToxicity && pawnStats.ToxicBuildUpVisible > 0)
            {
                DrawIconOnColonist(
                    bodyLoc,
                    ref iconNum,
                    Icons.Toxicity,
                    gradient4.Evaluate(pawnStats.ToxicBuildUpVisible),
                    ViewOpacityCrit);
            }

            if (psiSettings.ShowMedicalAttention)
            {
                // Check for map, else error in HealthUtility
                if (pawn.Map != null)
                {
                    if (pawnStats.ShouldBeTendedNowUrgent)
                    {
                        DrawIconOnColonist(
                            bodyLoc,
                            ref iconNum,
                            Icons.MedicalAttention,
                            ColVermillion,
                            ViewOpacityCrit);
                    }
                    else if (pawnStats.ShouldBeTendedNow)
                    {
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.MedicalAttention, ColYellow, ViewOpacityCrit);
                    }
                    else if (pawnStats.ShouldHaveSurgeryDoneNow)
                    {
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.MedicalAttention, ColSkyBlue, ViewOpacityCrit);
                    }
                }
            }

            // Hungry
            if (psiSettings.ShowHungry && pawn.needs.food.CurLevel < (double)psiSettings.LimitFoodLess)
            {
                DrawIconOnColonist(
                    bodyLoc,
                    ref iconNum,
                    Icons.Hungry,
                    gradientRedAlertToNeutral.Evaluate(pawn.needs.food.CurLevel / Settings.PsiSettings.LimitFoodLess),
                    ViewOpacityCrit);
            }

            // Tired
            if (psiSettings.ShowTired && pawn.needs.rest.CurLevel < (double)psiSettings.LimitRestLess)
            {
                DrawIconOnColonist(
                    bodyLoc,
                    ref iconNum,
                    Icons.Tired,
                    gradientRedAlertToNeutral.Evaluate(pawn.needs.rest.CurLevel / Settings.PsiSettings.LimitRestLess),
                    ViewOpacityCrit);
            }

            // Idle - icon only
            if (psiSettings.ShowIdle && pawn.mindState.IsIdle && GenDate.DaysPassed >= 0.1)
            {
                DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Idle, ColorNeutralStatus, viewOpacity);
            }

            // Pacifc
            bool pacifist = pawnStats.isPacifist;

            if (psiSettings.ShowUnarmed)
            {
                if (pawn.equipment.Primary == null && !pawn.IsPrisoner && !pacifist)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Unarmed, ColorNeutralStatusOpaque, viewOpacity);
                }
            }

            if (psiSettings.ShowPacific && pacifist)
            {
                if (pawn.drafter != null && !pawn.Drafted)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Pacific, ColSkyBlue, viewOpacity);
                }
            }

            // Trait Pyromaniac
            if (psiSettings.ShowPyromaniac)
            {
                if (pawnStats.isPyromaniac)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Pyromaniac, ColYellow, ViewOpacityCrit);
                }
            }

            // Addictions
            if (psiSettings.ShowDrunk && (pawnStats.isAddict || pawnStats.drugDesire != 0))
            {
                Color color = new Color();

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
                }

                DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Drunk, color, ViewOpacityCrit);
            }

            MentalBreaker mb = !pawn.Dead ? pawn.mindState.mentalBreaker : null;

            // Bad Mood
            if (psiSettings.ShowSad && pawn.needs.mood.CurLevelPercentage <= mb?.BreakThresholdMinor)
            {
                DrawIconOnColonist(
                    bodyLoc,
                    ref iconNum,
                    Icons.Sad,
                    gradientRedAlertToNeutral.Evaluate(pawn.needs.mood.CurLevelPercentage / mb.BreakThresholdMinor),
                    ViewOpacityCrit);
            }

            // if (psi && PsiSettings.ShowSad && pawn.needs.mood.CurLevel < (double)PsiSettings.LimitMoodLess)
            // DrawIcon_FadeRedAlertToNeutral(bodyLoc, iconNum, Icons.Sad, pawn.needs.mood.CurLevel / PsiSettings.LimitMoodLess);

            // Too Cold & too hot
            if (psiSettings.ShowTooCold && pawnStats.TooCold > 0f)
            {
                DrawIconOnColonist(
                    bodyLoc,
                    ref iconNum,
                    Icons.TooCold,
                    gradient4.Evaluate(pawnStats.TooCold / 2),
                    ViewOpacityCrit);
            }

            if (psiSettings.ShowTooHot && pawnStats.TooHot > 0f)
            {
                DrawIconOnColonist(
                    bodyLoc,
                    ref iconNum,
                    Icons.TooHot,
                    gradient4.Evaluate(pawnStats.TooHot / 2),
                    ViewOpacityCrit);
            }

            // Pain
            if (psiSettings.ShowPain && pawnStats.PainMoodLevel > -1)
            {
                if (psiSettings.ShowPain)
                {
                    DrawIconOnColonist(
                        bodyLoc,
                        ref iconNum,
                        Icons.Pain,
                        gradient4Mood.Evaluate(Mathf.InverseLerp(-25, 15, pawnStats.PainMoodOffset)),
                        viewOpacity);
                }
            }

            // Bed status
            if (psiSettings.ShowBedroom)
            {
                if (pawnStats.wantsToHump > -1)
                {
                    DrawIconOnColonist(
                        bodyLoc,
                        ref iconNum,
                        Icons.Bedroom,
                        Evaluate(pawnStats.humpMoodOffset),
                        ViewOpacityCrit);
                }
                else if (pawnStats.BedStatus > -1)
                {
                    DrawIconOnColonist(
                        bodyLoc,
                        ref iconNum,
                        Icons.Bedroom,
                        Evaluate(pawnStats.BedStatusMoodOffset),
                        ViewOpacityCrit);
                }
            }

            // Naked
            if (psiSettings.ShowNaked && pawnStats.feelsNaked > -1)
            {
                DrawIconOnColonist(
                    bodyLoc,
                    ref iconNum,
                    Icons.Naked,
                    Evaluate(pawnStats.nakedMoodOffset),
                    ViewOpacityCrit);
            }

            // Apparel
            if (psiSettings.ShowApparelHealth && pawnStats.ApparelHealth < (double)psiSettings.LimitApparelHealthLess)
            {
                double pawnApparelHealth = pawnStats.ApparelHealth / (double)psiSettings.LimitApparelHealthLess;
                DrawIconOnColonist(
                    bodyLoc,
                    ref iconNum,
                    Icons.ApparelHealth,
                    gradientRedAlertToNeutral.Evaluate((float)pawnApparelHealth),
                    ViewOpacityCrit);
            }

            // Moods caused by traits
            if (pawnStats.prosthoUnhappy > -1)
            {
                if (psiSettings.ShowProsthophile && pawnStats.prosthoUnhappy == 1)
                {
                    DrawIconOnColonist(
                        bodyLoc,
                        ref iconNum,
                        Icons.Prosthophile,
                        Evaluate(pawnStats.prosthoMoodOffset),
                        viewOpacity);
                }

                if (psiSettings.ShowProsthophobe && pawnStats.prosthoUnhappy == -1)
                {
                    DrawIconOnColonist(
                        bodyLoc,
                        ref iconNum,
                        Icons.Prosthophobe,
                        Evaluate(pawnStats.prosthoMoodOffset),
                        viewOpacity);
                }
            }

            if (psiSettings.ShowNightOwl)
            {
                if (pawnStats.nightOwlUnhappy > -1)
                {
                    DrawIconOnColonist(
                        bodyLoc,
                        ref iconNum,
                        Icons.NightOwl,
                        Evaluate(pawnStats.nightOwlMoodOffset),
                        ViewOpacityCrit);
                }
            }

            if (psiSettings.ShowGreedy)
            {
                if (pawnStats.greedyThought > -1)
                {
                    DrawIconOnColonist(
                        bodyLoc,
                        ref iconNum,
                        Icons.Greedy,
                        Evaluate(pawnStats.greedyMoodOffset),
                        viewOpacity);
                }
            }

            if (psiSettings.ShowJealous)
            {
                if (pawnStats.jealousThought > -1)
                {
                    DrawIconOnColonist(
                        bodyLoc,
                        ref iconNum,
                        Icons.Jealous,
                        Evaluate(pawnStats.jealousMoodOffset),
                        viewOpacity);
                }
            }

            // Effectiveness
            if (psiSettings.ShowEffectiveness && pawnStats.TotalEfficiency < (double)psiSettings.LimitEfficiencyLess)
            {
                DrawIconOnColonist(
                    bodyLoc,
                    ref iconNum,
                    Icons.Effectiveness,
                    gradientRedAlertToNeutral.Evaluate(
                        pawnStats.TotalEfficiency / Settings.PsiSettings.LimitEfficiencyLess),
                    ViewOpacityCrit);
            }

            // Bad thoughts
            if (psiSettings.ShowLeftUnburied)
            {
                if (pawnStats.unburied > -1)
                {
                    DrawIconOnColonist(
                        bodyLoc,
                        ref iconNum,
                        Icons.LeftUnburied,
                        Evaluate(pawnStats.unburiedMoodOffset),
                        ViewOpacityCrit);
                }
            }

            // CabinFever missing since A14?
            if (psiSettings.ShowCabinFever)
            {
                if (pawnStats.CabinFeverMoodLevel > -1)
                {
                    DrawIconOnColonist(
                        bodyLoc,
                        ref iconNum,
                        Icons.CabinFever,
                        Evaluate(pawnStats.CabinFeverMoodOffset),
                        ViewOpacityCrit);
                }
            }

            // if (psiSettings.ShowDeadColonists)
            // {
            // // Close Family & friends / 25
            // List<Thought> thoughts = pawnStats.Thoughts;
            // // not family, more whiter icon
            // if (HasThought(thoughts, ThoughtDef.Named("KilledColonist")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("KilledColonyAnimal")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
            // }
            // #region DeathMemory
            // //Deathmemory
            // // some of those need staging - to do
            // // edit: SCRAPPED!
            // if (HasThought(thoughts, ThoughtDef.Named("KnowGuestExecuted")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("KnowColonistExecuted")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("KnowPrisonerDiedInnocent")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("KnowColonistDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
            // }
            // //Bonded animal died
            // if (HasThought(thoughts, ThoughtDef.Named("BondedAnimalDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
            // }
            // // Friend / rival died
            // if (HasThought(thoughts, ThoughtDef.Named("thoughtWithGoodOpinionDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("thoughtWithBadOpinionDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, ColorMoodBoost, viewOpacity);
            // }
            // #endregion
            // #region DeathMemoryFamily
            // if (HasThought(thoughts, ThoughtDef.Named("MySonDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color25To21, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("MyDaughterDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color25To21, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("MyHusbandDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color25To21, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("MyWifeDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color25To21, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("MyFianceDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color20To16, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("MyFianceeDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color20To16, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("MyLoverDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color20To16, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("MyBrotherDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color15To11, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("MySisterDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color15To11, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("MyGrandchildDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color15To11, viewOpacity);
            // }
            // // 10
            // if (HasThought(thoughts, ThoughtDef.Named("MyFatherDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("MyMotherDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("MyNieceDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("MyNephewDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("MyHalfSiblingDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("MyAuntDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("MyUncleDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("MyGrandparentDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("MyCousinDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("MyKinDied")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
            // }
            // #endregion
            // //Memory misc
            // if (HasThought(thoughts, ThoughtDef.Named("WitnessedDeathAlly")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("WitnessedDeathNonAlly")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("WitnessedDeathFamily")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("WitnessedDeathBloodlust")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, ColorMoodBoost, viewOpacity);
            // }
            // if (HasThought(thoughts, ThoughtDef.Named("KilledHumanlikeBloodlust")))
            // {
            // DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, ColorMoodBoost, viewOpacity);
            // }
            // }
        }

        public static void DrawColonistRelationIconsPSI(Pawn pawn)
        {
            // Log.Message("Begin Drawing");
            if (pawn.Dead || !pawn.Spawned || pawn.holdingOwner == null || pawn.Map == null)
            {
                return;
            }

            PawnStats pawnStats = pawn.GetCache();

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
                    Icons.Health,
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

        public override void FinalizeInit()
        {
            Reinit();
        }

        public override void StartedNewGame()
        {
            PawnstatsChecker.ResetList();
            base.StartedNewGame();
        }

        public override void LoadedGame()
        {
            PawnstatsChecker.ResetList();
            base.LoadedGame();
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

        private static void GetThought(
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
                return;
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
                break;
            }
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

                IconPosVectorsPsi[index] = new Vector3(
                    (float)(-0.600000023841858 * psiSettings.IconMarginX - 0.550000011920929 * psiSettings.IconSize
                            * psiSettings.IconOffsetX * num1),
                    3f,
                    (float)(-0.600000023841858 * psiSettings.IconMarginY + 0.550000011920929 * psiSettings.IconSize
                            * psiSettings.IconOffsetY * num2));
            }
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
            pawnStats.pawnHealth = 1f - pawn.health.summaryHealth.SummaryHealthPercent;

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
                pawnStats.BleedRate = Mathf.Clamp01(
                    pawn.health.hediffSet.BleedRateTotal * Settings.PsiSettings.LimitBleedMult);

                if (pawn.Map != null)
                {
                    pawnStats.ShouldBeTendedNowUrgent = HealthAIUtility.ShouldBeTendedNowUrgent(pawn);
                    pawnStats.ShouldBeTendedNow = HealthAIUtility.ShouldBeTendedNow(pawn);
                    pawnStats.ShouldHaveSurgeryDoneNow = HealthAIUtility.ShouldHaveSurgeryDoneNow(pawn);
                }
            }

            if (pawnStats.IsSick && !pawn.Destroyed && pawn.playerSettings.medCare >= 0)
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
                GetThought(
                    thoughts,
                    ThoughtDef.Named("CabinFever"),
                    out pawnStats.CabinFeverMoodLevel,
                    out pawnStats.cabinFeverTip,
                    out pawnStats.CabinFeverMoodOffset);

                // Pain
                GetThought(
                    thoughts,
                    pawnStats.painThought,
                    out pawnStats.PainMoodLevel,
                    out pawnStats.painTip,
                    out pawnStats.PainMoodOffset);

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
                    GetThought(
                        thoughts,
                        ThoughtDef.Named("Greedy"),
                        out pawnStats.greedyThought,
                        out pawnStats.greedyTooltip,
                        out pawnStats.greedyMoodOffset);
                }

                // Jealous
                if (pawnStats.isJealous)
                {
                    GetThought(
                        thoughts,
                        ThoughtDef.Named("Jealous"),
                        out pawnStats.jealousThought,
                        out pawnStats.jealousTooltip,
                        out pawnStats.jealousMoodOffset);
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
    }
}