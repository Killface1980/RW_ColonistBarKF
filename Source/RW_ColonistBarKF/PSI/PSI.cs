using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using static ColonistBarKF.CBKF;
using static ColonistBarKF.ColorsPSI;
using static ColonistBarKF.Position;

namespace ColonistBarKF.PSI
{

    internal class PSI : MonoBehaviour
    {
        private static double _fDelta;

        public static Dictionary<Pawn, PawnStats> _statsDict = new Dictionary<Pawn, PawnStats>();

        private static float _worldScale = 1f;

        public static string[] IconSets = { "default" };

        public static Materials PSIMaterials = new Materials();

        private static PawnCapacityDef[] _pawnCapacities;

        public static Vector3[] _iconPosVectorsPSI;
        private static Vector3[] _iconPosRectsBar;

        public PSI()
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
        private static CellRect viewRect;
        private static bool initialized;

        // ReSharper disable once UnusedMember.Global
        public virtual void OnGUI()
        {

            if (Current.ProgramState != ProgramState.Playing)
                return;

            if (Find.VisibleMap == null)
            {
                return;
            }



            if (!PsiSettings.UsePsi && !PsiSettings.UsePsiOnPrisoner)
                return;

            if (!WorldRendererUtility.WorldRenderedNow)
            {
                viewRect = Find.CameraDriver.CurrentViewRect;
                viewRect = viewRect.ExpandedBy(5);
                foreach (Pawn pawn in Find.VisibleMap.mapPawns.AllPawns)
                {
                    if (!viewRect.Contains(pawn.Position))
                        continue;
                    if (useGUILayout)
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
        public virtual void Update()
        {

            if (Input.GetKeyUp(KeyCode.F11))
            {
                PsiSettings.UsePsi = !PsiSettings.UsePsi;
                ColBarSettings.UsePsi = !ColBarSettings.UsePsi;
                Messages.Message(PsiSettings.UsePsi ? "PSI.Enabled".Translate() : "PSI.Disabled".Translate(), MessageSound.Standard);
            }
            _worldScale = Screen.height / (2f * Camera.current.orthographicSize);
        }

        #region Icon Drawing 

        private static void DrawIcon_posOffset(Vector3 bodyPos, Vector3 posOffset, Material material, Color color, float opacity)
        {
            color.a = opacity;
            material.color = color;
            Color guiColor = GUI.color;
            GUI.color = color;
            Vector2 vectorAtBody;

            float wordscale = _worldScale;
            if (PsiSettings.IconsScreenScale)
            {
                wordscale = 45f;
                vectorAtBody = bodyPos.MapToUIPosition();
                vectorAtBody.x += posOffset.x * 45f;
                vectorAtBody.y -= posOffset.z * 45f;
            }
            else
                vectorAtBody = (bodyPos + posOffset).MapToUIPosition();


            float num2 = wordscale * (PsiSettings.IconSizeMult * 0.5f);
            //On Colonist
            Rect position = new Rect(vectorAtBody.x, vectorAtBody.y, num2 * PsiSettings.IconSize, num2 * PsiSettings.IconSize);
            position.x -= position.width * 0.5f;
            position.y -= position.height * 0.5f;

            GUI.DrawTexture(position, material.mainTexture, ScaleMode.ScaleToFit, true);
            GUI.color = guiColor;
        }

        private static void DrawIcon_onBar(Rect rect, Vector3 posOffset, Material material, Color color, float rectAlpha)
        {
            color.a *= rectAlpha;
            Color GuiColor = GUI.color;
            GuiColor.a = rectAlpha;
            GUI.color = GuiColor;

            material.color = color;

            Rect iconRect = new Rect(rect);

            iconRect.width /= ColBarSettings.IconsInColumn;
            iconRect.height = iconRect.width;
            iconRect.x = rect.xMin;
            iconRect.y = rect.yMax;



            switch (ColBarSettings.ColBarPsiIconPos)
            {
                case Alignment.Left:
                    iconRect.x = rect.xMin - iconRect.width;
                    iconRect.y = rect.yMax - iconRect.width;
                    if (ColBarSettings.UseExternalMoodBar && ColBarSettings.MoodBarPos == Alignment.Left)
                        iconRect.x -= rect.width / 4;
                    break;
                case Alignment.Right:
                    iconRect.x = rect.xMax;
                    iconRect.y = rect.yMax - iconRect.width;
                    if (ColBarSettings.UseExternalMoodBar && ColBarSettings.MoodBarPos == Alignment.Right)
                        iconRect.x += rect.width / 4;
                    break;
                case Alignment.Top:
                    iconRect.y = rect.yMin - iconRect.width;
                    if (ColBarSettings.UseExternalMoodBar && ColBarSettings.MoodBarPos == Alignment.Top)
                        iconRect.y -= rect.height / 4;
                    break;
                case Alignment.Bottom:
                    iconRect.y = rect.yMax + ColonistBarTextures.SpacingLabel;
                    if (ColBarSettings.UseExternalMoodBar && ColBarSettings.MoodBarPos == Alignment.Bottom)
                        iconRect.y += rect.height / 4;
                    break;

            }


            //    iconRect.x += (-0.5f * CBKF.ColBarSettings.IconDistanceX - 0.5f  * CBKF.ColBarSettings.IconOffsetX) * iconRect.width;
            //    iconRect.y -= (-0.5f * CBKF.ColBarSettings.IconDistanceY + 0.5f  * CBKF.ColBarSettings.IconOffsetY) * iconRect.height;

            iconRect.x += ColBarSettings.IconOffsetX * posOffset.x * iconRect.width;
            iconRect.y -= ColBarSettings.IconOffsetY * posOffset.z * iconRect.height;
            //On Colonist
            //iconRect.x -= iconRect.width * 0.5f;
            //iconRect.y -= iconRect.height * 0.5f;


            GUI.DrawTexture(iconRect, ColonistBarTextures.BGTexIconPSI);
            GUI.color = color;

            iconRect.x += iconRect.width * 0.1f;
            iconRect.y += iconRect.height * 0.1f;
            iconRect.width *= 0.8f;
            iconRect.height *= 0.8f;

            GUI.DrawTexture(iconRect, material.mainTexture, ScaleMode.ScaleToFit, true);
            GUI.color = GuiColor;

        }


        private static void DrawIconOnColonist(Vector3 bodyPos, ref int num, Icons icon, Color color, float opacity)
        {
            if (WorldRendererUtility.WorldRenderedNow)
                return;

            Material material = PSIMaterials[icon];
            if (material == null)
            {
                Debug.LogError("Material = null.");
                return;
            }

            DrawIcon_posOffset(bodyPos, _iconPosVectorsPSI[num], material, color, opacity);
            num++;
        }

        private static void DrawIconOnBar(Rect rect, ref int num, Icons icon, Color color, float rectAlpha)
        {
            Material material = PSIMaterials[icon];

            if (material == null)
                return;

            DrawIcon_onBar(rect, _iconPosRectsBar[num], material, color, rectAlpha);
            num++;
        }

        private static void DrawIcon_FadeRedAlertToNeutral(Vector3 bodyPos, ref int num, Icons icon, float v, float opacity)
        {
            v = v * 0.9f; // max settings according to neutral icon
            DrawIconOnColonist(bodyPos, ref num, icon, new Color(0.9f, v, v), opacity);
        }

        private static void DrawIcon_FadeRedAlertToNeutral(Rect rect, ref int num, Icons icon, float v, float rectAlpha)
        {
            v = v * 0.9f; // max settings according to neutral icon
            DrawIconOnBar(rect, ref num, icon, new Color(0.9f, v, v, 1f), rectAlpha);
        }

        private static void DrawIcon_FadeFloatWithTwoColors(Vector3 bodyPos, ref int num, Icons icon, float v, Color c1, Color c2, float opacity)
        {
            DrawIconOnColonist(bodyPos, ref num, icon, Color.Lerp(c1, c2, v), opacity);
        }

        private static void DrawIcon_FadeFloatWithTwoColors(Rect rect, ref int num, Icons icon, float v, Color c1, Color c2, float rectAlpha)
        {
            DrawIconOnBar(rect, ref num, icon, Color.Lerp(c1, c2, v), rectAlpha);
        }


        private static void DrawIcon_FadeFloatWithThreeColors(Vector3 bodyPos, ref int num, Icons icon, float v, Color c1, Color c2, Color c3, float opacity)
        {
            DrawIconOnColonist(bodyPos, ref num, icon, v < 0.5 ? Color.Lerp(c1, c2, v * 2f) : Color.Lerp(c2, c3, (float)((v - 0.5) * 2.0)), opacity);
        }

        private static void DrawIcon_FadeFloatWithThreeColors(Rect rect, ref int num, Icons icon, float v, Color c1, Color c2, Color c3, float rectAlpha)
        {
            DrawIconOnBar(rect, ref num, icon, v < 0.5 ? Color.Lerp(c1, c2, v * 2f) : Color.Lerp(c2, c3, (float)((v - 0.5) * 2.0)), rectAlpha);
        }

        private static void DrawIcon_FadeFloatWithFourColorsHB(Vector3 bodyPos, ref int num, Icons icon, float v, Color c1, Color c2, Color c3, Color c4, float opacity)
        {
            if (v > 0.8f)
            {
                DrawIconOnColonist(bodyPos, ref num, icon, Color.Lerp(c2, c1, (v - 0.8f) * 5), opacity);
            }
            else if (v > 0.6f)
            {
                DrawIconOnColonist(bodyPos, ref num, icon, Color.Lerp(c3, c2, (v - 0.6f) * 5), opacity);
            }
            else if (v > 0.4f)
            {
                DrawIconOnColonist(bodyPos, ref num, icon, Color.Lerp(c4, c3, (v - 0.4f) * 5), opacity);
            }
            else
            {
                DrawIconOnColonist(bodyPos, ref num, icon, c4, opacity);
            }
        }

        private static void DrawIcon_FadeFloatWithFourColorsHB(Rect rect, ref int num, Icons icon, float v, Color c1, Color c2, Color c3, Color c4, float rectAlpha)
        {
            if (v > 0.8f)
            {
                DrawIconOnBar(rect, ref num, icon, Color.Lerp(c2, c1, (v - 0.8f) * 5), rectAlpha);
            }
            else if (v > 0.6f)
            {
                DrawIconOnBar(rect, ref num, icon, Color.Lerp(c3, c2, (v - 0.6f) * 5), rectAlpha);
            }
            else if (v > 0.4f)
            {
                DrawIconOnBar(rect, ref num, icon, Color.Lerp(c4, c3, (v - 0.4f) * 5), rectAlpha);
            }
            else
            {
                DrawIconOnBar(rect, ref num, icon, c4, rectAlpha);
            }
        }


        private static void DrawIcon_FadeFloatFiveColors(Vector3 bodyPos, ref int num, Icons icon, float v, Color c1, Color c2, Color c3, Color c4, Color c5, float opacity)
        {
            if (v < 0.2f)
            {
                DrawIconOnColonist(bodyPos, ref num, icon, Color.Lerp(c1, c2, v * 5), opacity);
            }
            else if (v < 0.4f)
            {
                DrawIconOnColonist(bodyPos, ref num, icon, Color.Lerp(c2, c3, (v - 0.2f) * 5), opacity);
            }
            else if (v < 0.6f)
            {
                DrawIconOnColonist(bodyPos, ref num, icon, Color.Lerp(c3, c4, (v - 0.4f) * 5), opacity);
            }
            else if (v < 0.8f)
            {
                DrawIconOnColonist(bodyPos, ref num, icon, Color.Lerp(c4, c5, (v - 0.6f) * 5), opacity);
            }
            else
            {
                DrawIconOnColonist(bodyPos, ref num, icon, c5, opacity);
            }
        }

        private static void DrawIcon_FadeFloatFiveColors(Rect rect, ref int num, Icons icon, float v, Color c1, Color c2, Color c3, Color c4, Color c5, float rectAlpha)
        {
            if (v < 0.2f)
            {
                DrawIconOnBar(rect, ref num, icon, Color.Lerp(c1, c2, v * 5), rectAlpha);
            }
            else if (v < 0.4f)
            {
                DrawIconOnBar(rect, ref num, icon, Color.Lerp(c2, c3, (v - 0.2f) * 5), rectAlpha);
            }
            else if (v < 0.6f)
            {
                DrawIconOnBar(rect, ref num, icon, Color.Lerp(c3, c4, (v - 0.4f) * 5), rectAlpha);
            }
            else if (v < 0.8f)
            {
                DrawIconOnBar(rect, ref num, icon, Color.Lerp(c4, c5, (v - 0.6f) * 5), rectAlpha);
            }
            else
            {
                DrawIconOnBar(rect, ref num, icon, c5, rectAlpha);
            }
        }

        private static void RecalcIconPositionsPSI()
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
                            (-0.600000023841858 * PsiSettings.IconDistanceX -
                             0.550000011920929 * PsiSettings.IconSize * PsiSettings.IconOffsetX * num1), 3f,
                        (float)
                            (-0.600000023841858 * PsiSettings.IconDistanceY +
                             0.550000011920929 * PsiSettings.IconSize * PsiSettings.IconOffsetY * num2));

            }
        }

        private static void RecalcBarPositionAndSize()
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

                ColonistBar_KF.BaseSize.x = ColBarSettings.BaseSizeFloat;
                ColonistBar_KF.BaseSize.y = ColBarSettings.BaseSizeFloat;
                ColonistBar_KF.PawnTextureSize.x = ColBarSettings.BaseSizeFloat - 2f;
                ColonistBar_KF.PawnTextureSize.y = ColBarSettings.BaseSizeFloat * 1.5f;
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

                PawnStats pawnStats = _statsDict[colonist];

            pawnStats.thoughts = colonist.needs.mood.thoughts.DistinctThoughtGroups();


            // efficiency
            float efficiency = 10f;

            PawnCapacityDef[] array = _pawnCapacities;
            foreach (PawnCapacityDef pawnCapacityDef in array)
            {
                if (pawnCapacityDef != PawnCapacityDefOf.Consciousness)
                {
                    efficiency = Math.Min(efficiency, colonist.health.capacities.GetEfficiency(pawnCapacityDef));
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
                pawnStats.mb = colonist.mindState.mentalBreaker;
                pawnStats.mood = colonist.needs.mood;
            }
            else
            {
                pawnStats.mood = null;
                pawnStats.mb = null;
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
                        else
                        {
                            //if (!hediff.FullyHealableOnlyByTend())
                            //{
                            //    continue;
                            //}
                        }
                        var compImmunizable = hediff.TryGetComp<HediffComp_Immunizable>();
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



                        if (!hediff.def.PossibleToDevelopImmunity()) continue;

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


            pawnStats.CabinFeverMoodLevel = -1;

            pawnStats.PainMoodLevel = -1;


            _statsDict[colonist] = pawnStats;
        }

        private static bool HasThought(Pawn pawn, ThoughtDef tdef)
        {
            if (pawn.mindState == null)
                return false;
            PawnStats pawnStats = _statsDict[pawn];
            return pawnStats.thoughts.Any((thought) => thought.def == tdef);

        }

        //   public void UpdateOptionsDialog()
        //   {
        //       Dialog_Options dialogOptions = Find.WindowStack.WindowOfType<Dialog_Options>();
        //       bool optionsOpened = dialogOptions != null;
        //       bool PsiSettingsShowed = Find.WindowStack.IsOpen(typeof(Dialog_Settings));
        //       if (optionsOpened && PsiSettingsShowed)
        //       {
        //           _settingsDialog.OptionsDialog = dialogOptions;
        //           if (_inGame)
        //           {                    
        //           RecalcIconPositions();
        //           }
        //           return;
        //       }
        //       if (optionsOpened && !PsiSettingsShowed)
        //       {
        //           if (!_settingsDialog.CloseButtonClicked)
        //           {
        //               Find.UIRoot.windows.Add(_settingsDialog);
        //               _settingsDialog.Page = "main";
        //               return;
        //           }
        //           dialogOptions.Close(true);
        //       }
        //       else
        //       {
        //           if (!optionsOpened && PsiSettingsShowed)
        //           {
        //               _settingsDialog.Close(false);
        //               return;
        //           }
        //           if (!optionsOpened && !PsiSettingsShowed)
        //           {
        //               _settingsDialog.CloseButtonClicked = false;
        //           }
        //       }
        //   }

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

        public virtual void FixedUpdate()
        {
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
                if (pawn.Dead || pawn.DestroyedOrNull() || !pawn.Name.IsValid || pawn.Name == null) continue;
                if (_statsDict.ContainsKey(pawn)) continue;
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

        public static void DrawColonistIcons(Pawn colonist, bool psi, float rectAlpha = 1f, Rect rect = new Rect())
        {

            PawnStats pawnStats;
            if (colonist.Dead || colonist.holdingContainer != null || !_statsDict.TryGetValue(colonist, out pawnStats))
                return;

            if (Find.TickManager.TicksGame  > pawnStats.lastStatUpdate + Rand.Range(30, 90))
            {
                UpdateColonistStats(colonist);
                pawnStats.lastStatUpdate = Find.TickManager.TicksGame;
            }

            float viewOpacity = PsiSettings.IconOpacity;

            int barIconNum = 0;
            int iconNum = 0;

            Vector3 bodyLoc = colonist.DrawPos;

            // Target Point 
            if (psi)
                if (psi && PsiSettings.ShowTargetPoint && (pawnStats.TargetPos != Vector3.zero))
                {
                    if (psi && PsiSettings.UseColoredTarget)
                    {
                        Color skinColor = colonist.story.SkinColor;
                        Color hairColor = colonist.story.hairColor;

                        Material skinMat = PSIMaterials[Icons.TargetSkin];
                        Material hairMat = PSIMaterials[Icons.TargetHair];

                        if (skinMat == null)
                            return;

                        if (hairMat == null)
                            return;

                        DrawIcon_posOffset(pawnStats.TargetPos, Vector3.zero, skinMat, skinColor, 1f);
                        DrawIcon_posOffset(pawnStats.TargetPos, Vector3.zero, hairMat, hairColor, 1f);
                        ;
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
            if (colonist.Drafted)
            {
                if (colonist.story != null && colonist.story.WorkTagIsDisabled(WorkTags.Violent))
                {
                    if (!psi && ColBarSettings.ShowDraft)
                    {
                        DrawIconOnBar(rect, ref barIconNum, Icons.Pacific, ColorYellowAlert, rectAlpha);
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
                        DrawIconOnBar(rect, ref barIconNum, Icons.Draft, ColorRedAlert, rectAlpha);
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
                if (colonist.InAggroMentalState)
                {
                    if (!psi && ColBarSettings.ShowAggressive && colonist.InAggroMentalState)
                        DrawIconOnBar(rect, ref barIconNum, Icons.Aggressive, ColorRedAlert, rectAlpha);
                    if (psi && PsiSettings.ShowAggressive)
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Aggressive, ColorRedAlert, ViewOpacityCrit);
                }

                // Binging on alcohol - needs refinement
                {
                    if (pawnStats.MentalSanity == MentalStateDefOf.BingingDrugMajor)
                    {
                        if (!psi && ColBarSettings.ShowDrunk)
                            DrawIconOnBar(rect, ref barIconNum, Icons.Drunk, ColorOrangeAlert, rectAlpha);
                        if (psi && PsiSettings.ShowDrunk)
                            DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Drunk, ColorOrangeAlert, ViewOpacityCrit);
                    }

                    if (pawnStats.MentalSanity == MentalStateDefOf.BingingDrugExtreme)
                    {
                        if (!psi && ColBarSettings.ShowDrunk)
                            DrawIconOnBar(rect, ref barIconNum, Icons.Drunk, ColorRedAlert, rectAlpha);
                        if (psi && PsiSettings.ShowDrunk)
                            DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Drunk, ColorRedAlert, ViewOpacityCrit);
                    }
                }

                // Give Up Exit
                if (pawnStats.MentalSanity == MentalStateDefOf.PanicFlee) // was GiveUpExit
                {
                    if (!psi && ColBarSettings.ShowLeave)
                        DrawIconOnBar(rect, ref barIconNum, Icons.Leave, ColorRedAlert, rectAlpha);

                    if (psi && PsiSettings.ShowLeave)
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Leave, ColorRedAlert, ViewOpacityCrit);
                }

                //Daze Wander
                if (pawnStats.MentalSanity == MentalStateDefOf.WanderSad) // + MentalStateDefOf.WanderPsychotic
                {
                    if (!psi && ColBarSettings.ShowDazed)
                        DrawIconOnBar(rect, ref barIconNum, Icons.Dazed, ColorYellowAlert, rectAlpha);

                    if (psi && PsiSettings.ShowDazed)
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Dazed, ColorYellowAlert, ViewOpacityCrit);
                }

                //PanicFlee
                if (pawnStats.MentalSanity == MentalStateDefOf.PanicFlee)
                {
                    if (!psi && ColBarSettings.ShowPanic)
                        DrawIconOnBar(rect, ref barIconNum, Icons.Panic, ColorYellowAlert, rectAlpha);

                    if (psi && PsiSettings.ShowPanic)
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Panic, ColorYellowAlert, ViewOpacityCrit);
                }
            }

            // Drunkness percent
            if (pawnStats.Drunkness > 0.05)
            {
                if (!psi && ColBarSettings.ShowDrunk)
                    DrawIcon_FadeFloatWithThreeColors(rect, ref barIconNum, Icons.Drunk, pawnStats.Drunkness,
                        ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, rectAlpha);
                if (psi && PsiSettings.ShowDrunk)
                    DrawIcon_FadeFloatWithThreeColors(bodyLoc, ref iconNum, Icons.Drunk, pawnStats.Drunkness,
                        ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, ViewOpacityCrit);
            }


            // Pacifc + Unarmed

            if (colonist.story != null && colonist.story.WorkTagIsDisabled(WorkTags.Violent))
            {
                if (colonist.drafter != null && !colonist.Drafted)
                {
                    if (!psi && ColBarSettings.ShowPacific)
                        DrawIconOnBar(rect, ref barIconNum, Icons.Pacific, ColorNeutralStatus, rectAlpha);
                    if (psi && PsiSettings.ShowPacific)
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Pacific, ColorNeutralStatus, viewOpacity);
                }
            }
            else if (colonist.equipment.Primary == null && !colonist.IsPrisoner)
            {
                if (!psi && ColBarSettings.ShowUnarmed)
                    DrawIconOnBar(rect, ref barIconNum, Icons.Unarmed, ColorNeutralStatus, rectAlpha);
                if (psi && PsiSettings.ShowUnarmed)
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Unarmed, ColorNeutralStatus, viewOpacity);
            }


            // Trait Pyromaniac
            if (colonist.story.traits.HasTrait(TraitDefOf.Pyromaniac))
            {
                if (!psi && ColBarSettings.ShowPyromaniac)
                    DrawIconOnBar(rect, ref barIconNum, Icons.Pyromaniac, ColorYellowAlert, rectAlpha);
                if (psi && PsiSettings.ShowPyromaniac)
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Pyromaniac, ColorYellowAlert, ViewOpacityCrit);
            }

            // Idle - icon only
            if (psi && PsiSettings.ShowIdle && colonist.mindState.IsIdle)
                DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Idle, ColorNeutralStatus, viewOpacity);


            //Cabin Fever
            if (DefDatabase<ThoughtDef>.GetNamed("CabinFever").Worker.CurrentState(colonist).StageIndex > 0)
            {
                if (!psi && ColBarSettings.ShowCabinFever)
                    DrawIconOnBar(rect, ref barIconNum, Icons.CabinFever, ColorYellowAlert, rectAlpha);

                if (psi && PsiSettings.ShowCabinFever)
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.CabinFever, ColorYellowAlert, ViewOpacityCrit);
            }

            MentalBreaker mb = !colonist.Dead ? colonist.mindState.mentalBreaker : null;

            // Bad Mood
            if (colonist.needs.mood.CurLevelPercentage <= mb?.BreakThresholdMinor)
            {
                if (!psi && ColBarSettings.ShowSad)
                    DrawIcon_FadeRedAlertToNeutral(rect, ref barIconNum, Icons.Sad,
                        colonist.needs.mood.CurLevelPercentage / mb.BreakThresholdMinor, rectAlpha);
                if (psi && PsiSettings.ShowSad)
                    DrawIcon_FadeRedAlertToNeutral(bodyLoc, ref iconNum, Icons.Sad,
                        colonist.needs.mood.CurLevelPercentage / mb.BreakThresholdMinor, ViewOpacityCrit);
            }
            //   if (psi && PsiSettings.ShowSad && colonist.needs.mood.CurLevel < (double)PsiSettings.LimitMoodLess)
            //DrawIcon_FadeRedAlertToNeutral(bodyLoc, iconNum, Icons.Sad, colonist.needs.mood.CurLevel / PsiSettings.LimitMoodLess);

            // Bloodloss
            if (pawnStats.BleedRate > 0.0f)
            {
                if (!psi && ColBarSettings.ShowBloodloss)
                {
                    DrawIcon_FadeFloatWithTwoColors(rect, ref barIconNum, Icons.Bloodloss, pawnStats.BleedRate,
                        ColorRedAlert, ColorNeutralStatus, rectAlpha);
                }
                if (psi && PsiSettings.ShowBloodloss)
                {
                    DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.Bloodloss, pawnStats.BleedRate,
                        ColorRedAlert, ColorNeutralStatus, viewOpacity);
                }
            }

            //Health

            float pawnHealth = colonist.health.summaryHealth.SummaryHealthPercent;
            //Infection
            if (pawnStats.HasLifeThreateningDisease)
            {
                if (pawnHealth < pawnStats.HealthDisease)
                {
                    if (!psi && ColBarSettings.ShowHealth)
                        DrawIcon_FadeFloatWithFourColorsHB(rect, ref barIconNum, Icons.Health, pawnHealth,
                            ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, rectAlpha);

                    if (psi && PsiSettings.ShowHealth)
                        DrawIcon_FadeFloatWithFourColorsHB(bodyLoc, ref iconNum, Icons.Health, pawnHealth,
                            ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert,
                            ViewOpacityCrit);
                }

                else
                {
                    if (!psi && ColBarSettings.ShowHealth)
                        DrawIcon_FadeFloatWithFourColorsHB(rect, ref barIconNum, Icons.Health, pawnStats.HealthDisease,
                            ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, rectAlpha);
                    if (psi && PsiSettings.ShowHealth)
                        DrawIcon_FadeFloatWithFourColorsHB(bodyLoc, ref iconNum, Icons.Health, pawnStats.HealthDisease,
                            ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, ViewOpacityCrit);
                }
            }
            else if (colonist.health.summaryHealth.SummaryHealthPercent < 1f)
            {
                if (!psi && ColBarSettings.ShowHealth)
                    DrawIcon_FadeFloatWithFourColorsHB(rect, ref barIconNum, Icons.Health, pawnHealth,
                        ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, rectAlpha);

                if (psi && PsiSettings.ShowHealth)
                    DrawIcon_FadeFloatWithFourColorsHB(bodyLoc, ref iconNum, Icons.Health, pawnHealth,
                        ColorHealthBarGreen, ColorYellowAlert, ColorOrangeAlert, ColorRedAlert, ViewOpacityCrit);
            }

            //Toxicity buildup
            if (pawnStats.ToxicBuildUp > 0.04f)
            {
                if (!psi && ColBarSettings.ShowToxicity)
                    DrawIcon_FadeFloatFiveColors(rect, ref barIconNum, Icons.Toxicity, pawnStats.ToxicBuildUp,
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
                        DrawIcon_FadeFloatWithFourColorsHB(rect, ref barIconNum, Icons.Sickness,
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
                        DrawIconOnBar(rect, ref barIconNum, Icons.Sickness, ColorNeutralStatus, rectAlpha);
                    if (psi && PsiSettings.ShowMedicalAttention)
                        DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Sickness, ColorNeutralStatus, viewOpacity);
                }
            }

            // Pain

            {
                if (colonist.story.traits.HasTrait(TraitDef.Named("Masochist")))
                {
                    if (pawnStats.PainMoodLevel == 0)
                    {
                        if (!psi && ColBarSettings.ShowPain)
                            DrawIconOnBar(rect, ref barIconNum, Icons.Pain, ColorMoodBoost * 0.4f, rectAlpha);
                        if (psi && PsiSettings.ShowPain)
                            DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Pain, ColorMoodBoost * 0.4f, viewOpacity);
                    }
                    if (pawnStats.PainMoodLevel == 1)
                    {
                        if (!psi && ColBarSettings.ShowPain)
                            DrawIconOnBar(rect, ref barIconNum, Icons.Pain, ColorMoodBoost * 0.6f, rectAlpha);
                        if (psi && PsiSettings.ShowPain)
                            DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Pain, ColorMoodBoost * 0.6f, viewOpacity);
                    }
                    if (pawnStats.PainMoodLevel == 2)
                    {
                        if (!psi && ColBarSettings.ShowPain)
                            DrawIconOnBar(rect, ref barIconNum, Icons.Pain, ColorMoodBoost * 0.8f, rectAlpha);
                        if (psi && PsiSettings.ShowPain)
                            DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Pain, ColorMoodBoost * 0.8f, viewOpacity);
                    }
                    if (pawnStats.PainMoodLevel == 3)
                    {
                        if (!psi && ColBarSettings.ShowPain)
                            DrawIconOnBar(rect, ref barIconNum, Icons.Pain, ColorMoodBoost, rectAlpha);
                        if (psi && PsiSettings.ShowPain)
                            DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Pain, ColorMoodBoost, viewOpacity);
                    }
                }
                else
                {
                    // pain is always worse, +5 to the icon color
                    if (pawnStats.PainMoodLevel == 0)
                    {
                        if (!psi && ColBarSettings.ShowPain)
                            DrawIconOnBar(rect, ref barIconNum, Icons.Pain, Color10To06, rectAlpha);
                        if (psi && PsiSettings.ShowPain)
                            DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Pain, Color10To06, ViewOpacityCrit);
                    }
                    if (pawnStats.PainMoodLevel == 1)
                    {
                        if (!psi && ColBarSettings.ShowPain)
                            DrawIconOnBar(rect, ref barIconNum, Icons.Pain, Color15To11, rectAlpha);
                        if (psi && PsiSettings.ShowPain)
                            DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Pain, Color15To11, ViewOpacityCrit);
                    }
                    if (pawnStats.PainMoodLevel == 2)
                    {
                        if (!psi && ColBarSettings.ShowPain)
                            DrawIconOnBar(rect, ref barIconNum, Icons.Pain, Color20To16, rectAlpha);
                        if (psi && PsiSettings.ShowPain)
                            DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Pain, Color20To16, ViewOpacityCrit);
                    }
                    if (pawnStats.PainMoodLevel == 3)
                    {
                        if (!psi && ColBarSettings.ShowPain)
                            DrawIconOnBar(rect, ref barIconNum, Icons.Pain, Color25To21, rectAlpha);
                        if (psi && PsiSettings.ShowPain)
                            DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Pain, Color25To21, ViewOpacityCrit);
                    }
                }
            }
            if (HealthAIUtility.ShouldBeTendedNowUrgent(colonist))
            {
                if (!psi && ColBarSettings.ShowMedicalAttention)
                    DrawIconOnBar(rect, ref barIconNum, Icons.MedicalAttention, ColorRedAlert, rectAlpha);
                if (psi && PsiSettings.ShowMedicalAttention)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.MedicalAttention, ColorRedAlert, ViewOpacityCrit);
                }
            }
            else if (HealthAIUtility.ShouldBeTendedNow(colonist))
            {
                if (!psi && ColBarSettings.ShowMedicalAttention)
                {
                    DrawIconOnBar(rect, ref barIconNum, Icons.MedicalAttention, ColorYellowAlert, rectAlpha);
                }
                if (psi && PsiSettings.ShowMedicalAttention)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.MedicalAttention, ColorYellowAlert, ViewOpacityCrit);
                }
            }
            else if (HealthAIUtility.ShouldHaveSurgeryDoneNow(colonist))
            {
                if (!psi && ColBarSettings.ShowMedicalAttention)
                {
                    DrawIconOnBar(rect, ref barIconNum, Icons.MedicalAttention, ColorYellowAlert, rectAlpha);
                }
                if (psi && PsiSettings.ShowMedicalAttention)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.MedicalAttention, ColorYellowAlert, ViewOpacityCrit);
                }
            }

            // Hungry
            if (colonist.needs.food.CurLevel < (double)PsiSettings.LimitFoodLess)
            {
                if (!psi && ColBarSettings.ShowHungry)
                    DrawIcon_FadeRedAlertToNeutral(rect, ref barIconNum, Icons.Hungry,
                        colonist.needs.food.CurLevel / PsiSettings.LimitFoodLess, rectAlpha);
                if (psi && PsiSettings.ShowHungry)
                    DrawIcon_FadeRedAlertToNeutral(bodyLoc, ref iconNum, Icons.Hungry,
                        colonist.needs.food.CurLevel / PsiSettings.LimitFoodLess, ViewOpacityCrit);
            }

            //Tired
            if (colonist.needs.rest.CurLevel < (double)PsiSettings.LimitRestLess)
            {
                if (!psi && ColBarSettings.ShowTired && colonist.needs.rest.CurLevel < (double)PsiSettings.LimitRestLess)
                    DrawIcon_FadeRedAlertToNeutral(rect, ref barIconNum, Icons.Tired,
                        colonist.needs.rest.CurLevel / PsiSettings.LimitRestLess, rectAlpha);

                if (psi && PsiSettings.ShowTired)
                    DrawIcon_FadeRedAlertToNeutral(bodyLoc, ref iconNum, Icons.Tired,
                        colonist.needs.rest.CurLevel / PsiSettings.LimitRestLess, ViewOpacityCrit);
            }

            // Too Cold & too hot
            if (pawnStats.TooCold > 0f)
            {
                if (pawnStats.TooCold <= 1f)
                {
                    if (!psi && ColBarSettings.ShowTooCold)
                        DrawIcon_FadeFloatWithTwoColors(rect, ref barIconNum, Icons.TooCold, pawnStats.TooCold,
                            ColorNeutralStatusFade, ColorYellowAlert, rectAlpha);
                    if (psi && PsiSettings.ShowTooCold)
                        DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.TooCold, pawnStats.TooCold,
                            ColorNeutralStatusFade, ColorYellowAlert, ViewOpacityCrit);
                }
                else if (pawnStats.TooCold <= 1.5f)
                {
                    if (!psi && ColBarSettings.ShowTooCold)
                        DrawIcon_FadeFloatWithTwoColors(rect, ref barIconNum, Icons.TooCold, (pawnStats.TooCold - 1f) * 2f,
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
                        DrawIcon_FadeFloatWithTwoColors(rect, ref barIconNum, Icons.TooCold,
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
                        DrawIcon_FadeFloatWithTwoColors(rect, ref barIconNum, Icons.TooHot, pawnStats.TooHot,
                            ColorNeutralStatusFade, ColorYellowAlert, rectAlpha);
                    if (psi && PsiSettings.ShowTooHot)
                        DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.TooHot, pawnStats.TooHot,
                            ColorNeutralStatusFade, ColorYellowAlert, ViewOpacityCrit);
                }
                else if (pawnStats.TooHot <= 1.5f)
                {
                    if (!psi && ColBarSettings.ShowTooHot)
                        DrawIcon_FadeFloatWithTwoColors(rect, ref barIconNum, Icons.TooHot, pawnStats.TooHot,
                            ColorYellowAlert, ColorOrangeAlert, rectAlpha);
                    if (psi && PsiSettings.ShowTooHot)
                        DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.TooHot, pawnStats.TooHot,
                            ColorYellowAlert, ColorOrangeAlert, ViewOpacityCrit);
                }
                else
                {
                    if (!psi && ColBarSettings.ShowTooHot)
                        DrawIcon_FadeFloatWithTwoColors(rect, ref barIconNum, Icons.TooHot, pawnStats.TooHot - 1f,
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
                    DrawIconOnBar(rect, ref barIconNum, Icons.Bedroom, Color10To06, rectAlpha);
                if (psi && PsiSettings.ShowBedroom)
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Bedroom, Color10To06, ViewOpacityCrit);
            }


            // Usage of bed ...
            if (HasThought(colonist, ThoughtDef.Named("WantToSleepWithSpouseOrLover")))
            {
                if (!psi && ColBarSettings.ShowLove)
                {
                    DrawIconOnBar(rect, ref barIconNum, Icons.Love, ColorYellowAlert, rectAlpha);
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
            if (HasThought(colonist, ThoughtDefOf.Naked))
            {
                // Naked
                if (!psi && ColBarSettings.ShowNaked)
                {
                    DrawIconOnBar(rect, ref barIconNum, Icons.Naked, Color10To06, rectAlpha);
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
                    DrawIcon_FadeRedAlertToNeutral(rect, ref barIconNum, Icons.ApparelHealth, (float)pawnApparelHealth,
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

            if (HasThought(colonist, ThoughtDef.Named("ProsthophileNoProsthetic")))
            {
                if (!psi && ColBarSettings.ShowProsthophile)
                {
                    DrawIconOnBar(rect, ref barIconNum, Icons.Prosthophile, Color05AndLess, rectAlpha);
                }
                if (psi && PsiSettings.ShowProsthophile)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Prosthophile, Color05AndLess, viewOpacity);
                }
            }

            if (HasThought(colonist, ThoughtDef.Named("ProsthophobeUnhappy")))
            {
                if (!psi && ColBarSettings.ShowProsthophobe)
                {
                    DrawIconOnBar(rect, ref barIconNum, Icons.Prosthophobe, Color10To06, rectAlpha);
                }
                if (psi && PsiSettings.ShowProsthophobe)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Prosthophobe, Color10To06, viewOpacity);
                }
            }

            if (HasThought(colonist, ThoughtDef.Named("NightOwlDuringTheDay")))
            {
                if (!psi && ColBarSettings.ShowNightOwl)
                {
                    DrawIconOnBar(rect, ref barIconNum, Icons.NightOwl, Color10To06, rectAlpha);
                }
                if (psi && PsiSettings.ShowNightOwl)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.NightOwl, Color10To06, ViewOpacityCrit);
                }
            }

            if (HasThought(colonist, ThoughtDef.Named("Greedy")))
            {
                if (!psi && ColBarSettings.ShowGreedy)
                {
                    DrawIconOnBar(rect, ref barIconNum, Icons.Greedy, Color10To06, rectAlpha);
                }
                if (psi && PsiSettings.ShowGreedy)
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.Greedy, Color10To06, viewOpacity);
                }
            }

            if (HasThought(colonist, ThoughtDef.Named("Jealous")))
            {
                if (!psi && ColBarSettings.ShowJealous)
                {
                    DrawIconOnBar(rect, ref barIconNum, Icons.Jealous, Color10To06, rectAlpha);
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
                    DrawIcon_FadeRedAlertToNeutral(rect, ref barIconNum, Icons.Effectiveness,
                        pawnStats.TotalEfficiency / PsiSettings.LimitEfficiencyLess, rectAlpha);

                if (psi && PsiSettings.ShowEffectiveness)
                {
                    DrawIcon_FadeRedAlertToNeutral(bodyLoc, ref iconNum, Icons.Effectiveness,
                        pawnStats.TotalEfficiency / PsiSettings.LimitEfficiencyLess, ViewOpacityCrit);
                }
            }

            // Bad thoughts

            if (HasThought(colonist, ThoughtDef.Named("ColonistLeftUnburied")))
            {
                if (!psi && ColBarSettings.ShowLeftUnburied)
                {
                    DrawIconOnBar(rect, ref barIconNum, Icons.LeftUnburied, Color10To06, rectAlpha);
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
                if (HasThought(colonist, ThoughtDef.Named("KilledColonist")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }

                if (HasThought(colonist, ThoughtDef.Named("KilledColonyAnimal")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }

                #region DeathMemory

                //Deathmemory
                // some of those need staging - to do
                if (HasThought(colonist, ThoughtDef.Named("KnowGuestExecuted")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }
                if (HasThought(colonist, ThoughtDef.Named("KnowColonistExecuted")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }
                if (HasThought(colonist, ThoughtDef.Named("KnowPrisonerDiedInnocent")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
                }
                if (HasThought(colonist, ThoughtDef.Named("KnowColonistDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }
                //Bonded animal died
                if (HasThought(colonist, ThoughtDef.Named("BondedAnimalDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
                }
                // Friend / rival died
                if (HasThought(colonist, ThoughtDef.Named("PawnWithGoodOpinionDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
                }

                if (HasThought(colonist, ThoughtDef.Named("PawnWithBadOpinionDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, ColorMoodBoost, viewOpacity);
                }

                #endregion

                #region DeathMemoryFamily

                if (HasThought(colonist, ThoughtDef.Named("MySonDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color25To21, viewOpacity);
                }

                if (HasThought(colonist, ThoughtDef.Named("MyDaughterDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color25To21, viewOpacity);
                }

                if (HasThought(colonist, ThoughtDef.Named("MyHusbandDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color25To21, viewOpacity);
                }

                if (HasThought(colonist, ThoughtDef.Named("MyWifeDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color25To21, viewOpacity);
                }

                if (HasThought(colonist, ThoughtDef.Named("MyFianceDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color20To16, viewOpacity);
                }

                if (HasThought(colonist, ThoughtDef.Named("MyFianceeDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color20To16, viewOpacity);
                }

                if (HasThought(colonist, ThoughtDef.Named("MyLoverDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color20To16, viewOpacity);
                }

                if (HasThought(colonist, ThoughtDef.Named("MyBrotherDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color15To11, viewOpacity);
                }

                if (HasThought(colonist, ThoughtDef.Named("MySisterDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color15To11, viewOpacity);
                }

                if (HasThought(colonist, ThoughtDef.Named("MyGrandchildDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color15To11, viewOpacity);
                }

                // 10

                if (HasThought(colonist, ThoughtDef.Named("MyFatherDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
                }

                if (HasThought(colonist, ThoughtDef.Named("MyMotherDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
                }

                if (HasThought(colonist, ThoughtDef.Named("MyNieceDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }

                if (HasThought(colonist, ThoughtDef.Named("MyNephewDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }

                if (HasThought(colonist, ThoughtDef.Named("MyHalfSiblingDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }

                if (HasThought(colonist, ThoughtDef.Named("MyAuntDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }

                if (HasThought(colonist, ThoughtDef.Named("MyUncleDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }

                if (HasThought(colonist, ThoughtDef.Named("MyGrandparentDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }

                if (HasThought(colonist, ThoughtDef.Named("MyCousinDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }
                if (HasThought(colonist, ThoughtDef.Named("MyKinDied")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }

                #endregion

                //Memory misc
                if (HasThought(colonist, ThoughtDef.Named("WitnessedDeathAlly")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
                }
                if (HasThought(colonist, ThoughtDef.Named("WitnessedDeathNonAlly")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color05AndLess, viewOpacity);
                }
                if (HasThought(colonist, ThoughtDef.Named("WitnessedDeathFamily")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, Color10To06, viewOpacity);
                }
                if (HasThought(colonist, ThoughtDef.Named("WitnessedDeathBloodlust")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, ColorMoodBoost, viewOpacity);
                }
                if (HasThought(colonist, ThoughtDef.Named("KilledHumanlikeBloodlust")))
                {
                    DrawIconOnColonist(bodyLoc, ref iconNum, Icons.DeadColonist, ColorMoodBoost, viewOpacity);
                }

                // CabinFever missing since A14?

                if (pawnStats.CabinFeverMoodLevel != 0)
                {
                    if (pawnStats.CabinFeverMoodLevel == 0)
                    {
                        if (!psi && ColBarSettings.ShowCabinFever)
                            DrawIconOnBar(rect, ref barIconNum, Icons.CabinFever, ColorNeutralStatusFade, rectAlpha);
                        if (psi && PsiSettings.ShowCabinFever)
                            DrawIconOnColonist(bodyLoc, ref iconNum, Icons.CabinFever, ColorNeutralStatusFade,
                                ViewOpacityCrit);
                    }
                    if (pawnStats.CabinFeverMoodLevel == 1)
                    {
                        if (!psi && ColBarSettings.ShowCabinFever)
                            DrawIconOnBar(rect, ref barIconNum, Icons.CabinFever, ColorYellowAlert, rectAlpha);
                        if (psi && PsiSettings.ShowCabinFever)
                            DrawIconOnColonist(bodyLoc, ref iconNum, Icons.CabinFever, ColorYellowAlert, ViewOpacityCrit);
                    }
                    if (pawnStats.CabinFeverMoodLevel == 2)
                    {
                        if (!psi && ColBarSettings.ShowCabinFever)
                            DrawIconOnBar(rect, ref barIconNum, Icons.CabinFever, ColorOrangeAlert, rectAlpha);
                        if (psi && PsiSettings.ShowCabinFever)
                            DrawIconOnColonist(bodyLoc, ref iconNum, Icons.CabinFever, ColorOrangeAlert, ViewOpacityCrit);
                    }
                }
            }
        }

        #endregion
    }
}