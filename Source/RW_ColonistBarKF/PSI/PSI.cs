using System;
using System.Collections.Generic;
using RimWorld;
using RW_ColonistBarKF;
using UnityEngine;
using Verse;
using Verse.AI;
using static ColonistBarKF.CBKF;

namespace ColonistBarKF.PSI
{

    internal class PSI : MonoBehaviour
    {
        private static double _fDelta;

        private static Dictionary<Pawn, PawnStats> _statsDict = new Dictionary<Pawn, PawnStats>();

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

        // ReSharper disable once UnusedMember.Global
        public virtual void OnGUI()
        {

            if (Current.ProgramState != ProgramState.MapPlaying)
                return;

            if (!PsiSettings.UsePsi)
                return;

            foreach (Pawn pawn in Find.Map.mapPawns.AllPawns)
            {
                if (pawn != null && pawn.RaceProps.Animal)
                    DrawAnimalIcons(pawn);
                else if (pawn != null && (pawn.IsColonist || pawn.IsPrisonerOfColony))
                {
                    DrawColonistIcons(pawn);
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

        private static void DrawIcon_posOffset(Vector3 bodyPos, Vector3 posOffset, Material material, Color color)
        {
            material.color = color;
            Color guiColor = GUI.color;
            GUI.color = color;
            Vector2 vectorAtBody;

            float wordscale = _worldScale;
            if (PsiSettings.IconsScreenScale)
            {
                wordscale = 45f;
                vectorAtBody = bodyPos.ToScreenPosition();
                vectorAtBody.x += posOffset.x * 45f;
                vectorAtBody.y -= posOffset.z * 45f;
            }
            else
                vectorAtBody = (bodyPos + posOffset).ToScreenPosition();


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

            switch (ColBarSettings.IconAlignment)
            {
                case 0:
                    iconRect.x = rect.xMin - iconRect.width;
                    iconRect.y = rect.yMax - iconRect.width;
                    break;
                case 1:
                    iconRect.x = rect.xMax;
                    iconRect.y = rect.yMax - iconRect.width;
                    break;
                case 2:
                    iconRect.x = rect.xMin;
                    iconRect.y = rect.yMin - iconRect.width * 1.5f;
                    break;
                case 3:
                    iconRect.x = rect.xMin;
                    iconRect.y = rect.yMax + 0.5f * iconRect.width;
                    break;

                default:
                    iconRect.x = rect.xMin;
                    iconRect.y = rect.yMax;
                    break;
            }

            //    iconRect.x += (-0.5f * CBKF.ColBarSettings.IconDistanceX - 0.5f  * CBKF.ColBarSettings.IconOffsetX) * iconRect.width;
            //    iconRect.y -= (-0.5f * CBKF.ColBarSettings.IconDistanceY + 0.5f  * CBKF.ColBarSettings.IconOffsetY) * iconRect.height;

            iconRect.x += ColBarSettings.IconDistanceX * ColBarSettings.IconOffsetX * posOffset.x * iconRect.width;
            iconRect.y -= ColBarSettings.IconDistanceY * ColBarSettings.IconOffsetY * posOffset.z * iconRect.height;
            //On Colonist
            //iconRect.x -= iconRect.width * 0.5f;
            //iconRect.y -= iconRect.height * 0.5f;


            GUI.DrawTexture(iconRect, ColonistBarTextures.BGTexIconPSI);
            GUI.color = color;


            //GUILayout.BeginArea(position);
            //GUILayout.Box(material.mainTexture);
            //GUILayout.EndArea();

            //    Rect realIcon = iconRect.ContractedBy(iconRect.width);

            iconRect.x += iconRect.width * 0.1f;
            iconRect.y += iconRect.height * 0.1f;
            iconRect.width *= 0.8f;
            iconRect.height *= 0.8f;

            GUI.DrawTexture(iconRect, material.mainTexture, ScaleMode.ScaleToFit, true);
            GUI.color = GuiColor;
        }


        private static void DrawIcon(Vector3 bodyPos, ref int num, Icons icon, Color color)
        {
            Material material = PSIMaterials[icon];
            if (material == null)
            {
                Debug.LogError("Material = null.");
                return;
            }

            DrawIcon_posOffset(bodyPos, _iconPosVectorsPSI[num], material, color);
            num++;
        }

        private static void DrawIcon(Rect rect, ref int num, Icons icon, Color color, float rectAlpha)
        {
            Material material = PSIMaterials[icon];

            if (material == null)
                return;

            DrawIcon_onBar(rect, _iconPosRectsBar[num], material, color, rectAlpha);
            num++;
        }

        private static void DrawIcon_FadeRedAlertToNeutral(Vector3 bodyPos, ref int num, Icons icon, float v)
        {
            v = v * 0.9f; // max settings according to neutral icon
            DrawIcon(bodyPos, ref num, icon, new Color(0.9f, v, v, PsiSettings.IconOpacity));
        }

        private static void DrawIcon_FadeRedAlertToNeutral(Rect rect, ref int num, Icons icon, float v, float rectAlpha)
        {
            v = v * 0.9f; // max settings according to neutral icon
            DrawIcon(rect, ref num, icon, new Color(0.9f, v, v, 1f), rectAlpha);
        }

        private static void DrawIcon_FadeFloatWithTwoColors(Vector3 bodyPos, ref int num, Icons icon, float v, Color c1, Color c2)
        {
            DrawIcon(bodyPos, ref num, icon, Color.Lerp(c1, c2, v));
        }

        private static void DrawIcon_FadeFloatWithTwoColors(Rect rect, ref int num, Icons icon, float v, Color c1, Color c2, float rectAlpha)
        {
            DrawIcon(rect, ref num, icon, Color.Lerp(c1, c2, v), rectAlpha);
        }


        private static void DrawIcon_FadeFloatWithThreeColors(Vector3 bodyPos, ref int num, Icons icon, float v, Color c1, Color c2, Color c3)
        {
            DrawIcon(bodyPos, ref num, icon, v < 0.5 ? Color.Lerp(c1, c2, v * 2f) : Color.Lerp(c2, c3, (float)((v - 0.5) * 2.0)));
        }

        private static void DrawIcon_FadeFloatWithThreeColors(Rect rect, ref int num, Icons icon, float v, Color c1, Color c2, Color c3, float rectAlpha)
        {
            DrawIcon(rect, ref num, icon, v < 0.5 ? Color.Lerp(c1, c2, v * 2f) : Color.Lerp(c2, c3, (float)((v - 0.5) * 2.0)), rectAlpha);
        }

        private static void DrawIcon_FadeFloatWithFourColorsHB(Vector3 bodyPos, ref int num, Icons icon, float v, Color c1, Color c2, Color c3, Color c4)
        {
            if (v > 0.8f)
            {
                DrawIcon(bodyPos, ref num, icon, Color.Lerp(c2, c1, (v - 0.8f) * 5));
            }
            else if (v > 0.6f)
            {
                DrawIcon(bodyPos, ref num, icon, Color.Lerp(c3, c2, (v - 0.6f) * 5));
            }
            else if (v > 0.4f)
            {
                DrawIcon(bodyPos, ref num, icon, Color.Lerp(c4, c3, (v - 0.4f) * 5));
            }
            else
            {
                DrawIcon(bodyPos, ref num, icon, c4);
            }
        }

        private static void DrawIcon_FadeFloatWithFourColorsHB(Rect rect, ref int num, Icons icon, float v, Color c1, Color c2, Color c3, Color c4, float rectAlpha)
        {
            if (v > 0.8f)
            {
                DrawIcon(rect, ref num, icon, Color.Lerp(c2, c1, (v - 0.8f) * 5), rectAlpha);
            }
            else if (v > 0.6f)
            {
                DrawIcon(rect, ref num, icon, Color.Lerp(c3, c2, (v - 0.6f) * 5), rectAlpha);
            }
            else if (v > 0.4f)
            {
                DrawIcon(rect, ref num, icon, Color.Lerp(c4, c3, (v - 0.4f) * 5), rectAlpha);
            }
            else
            {
                DrawIcon(rect, ref num, icon, c4, rectAlpha);
            }
        }


        private static void DrawIcon_FadeFloatFiveColors(Vector3 bodyPos, ref int num, Icons icon, float v, Color c1, Color c2, Color c3, Color c4, Color c5)
        {
            if (v < 0.2f)
            {
                DrawIcon(bodyPos, ref num, icon, Color.Lerp(c1, c2, v * 5));
            }
            else if (v < 0.4f)
            {
                DrawIcon(bodyPos, ref num, icon, Color.Lerp(c2, c3, (v - 0.2f) * 5));
            }
            else if (v < 0.6f)
            {
                DrawIcon(bodyPos, ref num, icon, Color.Lerp(c3, c4, (v - 0.4f) * 5));
            }
            else if (v < 0.8f)
            {
                DrawIcon(bodyPos, ref num, icon, Color.Lerp(c4, c5, (v - 0.6f) * 5));
            }
            else
            {
                DrawIcon(bodyPos, ref num, icon, c5);
            }
        }

        private static void DrawIcon_FadeFloatFiveColors(Rect rect, ref int num, Icons icon, float v, Color c1, Color c2, Color c3, Color c4, Color c5, float rectAlpha)
        {
            if (v < 0.2f)
            {
                DrawIcon(rect, ref num, icon, Color.Lerp(c1, c2, v * 5), rectAlpha);
            }
            else if (v < 0.4f)
            {
                DrawIcon(rect, ref num, icon, Color.Lerp(c2, c3, (v - 0.2f) * 5), rectAlpha);
            }
            else if (v < 0.6f)
            {
                DrawIcon(rect, ref num, icon, Color.Lerp(c3, c4, (v - 0.4f) * 5), rectAlpha);
            }
            else if (v < 0.8f)
            {
                DrawIcon(rect, ref num, icon, Color.Lerp(c4, c5, (v - 0.6f) * 5), rectAlpha);
            }
            else
            {
                DrawIcon(rect, ref num, icon, c5, rectAlpha);
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

            if (colonist == null) return;

            PawnStats pawnStats = _statsDict[colonist];


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
                TargetInfo targetInfo = curJob.targetA;
                if (curDriver is JobDriver_HaulToContainer || curDriver is JobDriver_HaulToCell ||
                    curDriver is JobDriver_FoodDeliver || curDriver is JobDriver_FoodFeedPatient ||
                    curDriver is JobDriver_TakeToBed)
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
                if (curDriver is JobDriver_Hunt && colonist.carrier?.CarriedThing != null)
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
            float temperatureForCell = GenTemperature.GetTemperatureForCell(colonist.Position);

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

            // Health Calc
            pawnStats.DiseaseDisappearance = 1f;
            pawnStats.HasLifeThreateningDisease = false;
            pawnStats.HealthDisease = 1f;

            // Sick thoughts
            if (colonist.health?.hediffSet != null)
                pawnStats.IsSick = colonist.health.hediffSet.AnyHediffMakesSickThought;


            if (pawnStats.IsSick && colonist.SelectableNow() && !colonist.Destroyed && colonist.playerSettings.medCare >= 0)
            {
                if (colonist.health?.hediffSet?.hediffs != null)
                {
                    int i;
                    for (i = 0; i < colonist.health.hediffSet.hediffs.Count; i++)
                    {
                        Hediff hediff = colonist.health.hediffSet.hediffs[i];
                        //         HediffWithComps hediffWithComps;

                        //      if ((HediffWithComps)hediff != null)
                        //          hediffWithComps = (HediffWithComps)hediff;
                        //      else continue;
                        //
                        //      if (hediffWithComps.IsOld()) continue;

                        pawnStats.ToxicBuildUp = 0;

                        //pawnStats.ToxicBuildUp
                        if (hediff.def.defName.Equals("ToxicBuildup"))
                        {
                            pawnStats.ToxicBuildUp = hediff.Severity;
                        }

                        if (hediff.def.defName.Equals("WoundInfection") || hediff.def.defName.Equals("Flu") || hediff.def.defName.Equals("Plague") || hediff.def.defName.Equals("Malaria") || hediff.def.defName.Equals("SleepingSickness"))
                        {
                            float severity = hediff.Severity;
                            float immunity = colonist.health.immunity.GetImmunity(hediff.def);
                            float basehealth = pawnStats.HealthDisease - (severity - immunity / 4) - 0.25f;
                            pawnStats.HasLifeThreateningDisease = true;
                            pawnStats.HealthDisease = basehealth;
                        }


                        if (!hediff.Visible) continue;

                        if (!hediff.def.PossibleToDevelopImmunity()) continue;

                        if (hediff.CurStage == null) continue;

                        if (!hediff.CurStage.everVisible) continue;

                        if (hediff.FullyImmune()) continue;

                        if (hediff.def.naturallyHealed) continue;

                        if (!hediff.def.makesSickThought) continue;

                        if (!hediff.def.tendable) continue;

                        if (Math.Abs(colonist.health.immunity.GetImmunity(hediff.def) - 1.0) < 0.05) continue;

                        //


                        if (pawnStats.DiseaseDisappearance > colonist.health.immunity.GetImmunity(hediff.def))
                        {
                            pawnStats.DiseaseDisappearance = colonist.health.immunity.GetImmunity(hediff.def);
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
                pawnStats.BleedRate = Mathf.Clamp01(colonist.health.hediffSet.BleedingRate * PsiSettings.LimitBleedMult);


            // Bed status
            if (colonist.ownership.OwnedBed != null)
                pawnStats.HasBed = true;
            if (colonist.ownership.OwnedBed == null)
            {
                pawnStats.HasBed = false;
            }


            pawnStats.CrowdedMoodLevel = -1;

            pawnStats.PainMoodLevel = -1;

            // Moods

            if (colonist.needs.mood.thoughts.Thoughts != null)
            {
                int i;
                for (i = 0; i < colonist.needs.mood.thoughts.Thoughts.Count; i++)
                {
                    Thought thoughtDef = colonist.needs.mood.thoughts.Thoughts[i];
                    if (thoughtDef.CurStage != null)
                    {
                        if (thoughtDef.def.defName.Equals("Crowded"))
                        {
                            pawnStats.CrowdedMoodLevel = thoughtDef.CurStageIndex;
                        }
                        else
                        {
                            pawnStats.CrowdedMoodLevel = -1;
                        }
                        if (thoughtDef.def.defName.Equals("Pain") || thoughtDef.def.defName.Equals("MasochistPain"))
                        {
                            pawnStats.PainMoodLevel = thoughtDef.CurStageIndex;

                        }
                        else pawnStats.PainMoodLevel = -1;
                    }
                }
            }

            _statsDict[colonist] = pawnStats;
        }

        public static bool HasMood(Pawn pawn, ThoughtDef tdef)
        {
            return pawn.needs.mood.thoughts.Thoughts.Any(thought => thought.def == tdef);
        }

        // ReSharper disable once UnusedMember.Global
        public virtual void FixedUpdate()
        {
            if (Current.ProgramState != ProgramState.MapPlaying)
                return;

            _fDelta += Time.fixedDeltaTime;

            if (_fDelta < 0.1)
                return;
            _fDelta = 0.0;

            foreach (Pawn pawn in Find.Map.mapPawns.FreeColonistsAndPrisoners) //.FreeColonistsAndPrisoners)
                                                                               //               foreach (var colonist in Find.Map.mapPawns.FreeColonistsAndPrisonersSpawned) //.FreeColonistsAndPrisoners)
            {
                if (!pawn.SelectableNow() || pawn.Dead || pawn.DestroyedOrNull() || !pawn.Name.IsValid || pawn.Name == null) continue;
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

        private static void DrawAnimalIcons(Pawn animal)
        {
            float transparancy = PsiSettings.IconOpacity;
            Color colorRedAlert = new Color(1f, 0f, 0f, transparancy);

            if (animal.Dead || animal.holder != null)
                return;
            Vector3 drawPos = animal.DrawPos;

            if (!PsiSettings.ShowAggressive || animal.MentalStateDef != MentalStateDefOf.Berserk && animal.MentalStateDef != MentalStateDefOf.Manhunter)
                return;
            Vector3 bodyPos = drawPos;
            int num = 0;
            DrawIcon(bodyPos, ref num, Icons.Aggressive, colorRedAlert);
        }

        private static void DrawColonistIcons(Pawn colonist)
        {
            var opacity = PsiSettings.IconOpacity;

            float opacityCritical = PsiSettings.IconOpacityCritical;

            Color color25To21 = new Color(0.8f, 0f, 0f, opacity);

            Color color20To16 = new Color(0.9f, 0.45f, 0f, opacity);

            Color color15To11 = new Color(0.95f, 0.95f, 0f, opacity);

            Color color10To06 = new Color(0.95f, 0.95f, 0.66f, opacity);

            Color color05AndLess = new Color(0.9f, 0.9f, 0.9f, opacity);

            Color colorMoodBoost = new Color(0f, 0.8f, 0f, opacity);

            Color colorNeutralStatus = color05AndLess; // new Color(1f, 1f, 1f, transparancy);

            Color colorNeutralStatusSolid = new Color(colorNeutralStatus.r, colorNeutralStatus.g, colorNeutralStatus.b, 0.5f + opacity * 0.2f);

            Color colorNeutralStatusFade = new Color(colorNeutralStatus.r, colorNeutralStatus.g, colorNeutralStatus.b, opacity / 4);

            Color colorHealthBarGreen = new Color(0f, 0.8f, 0f, opacity * 0.5f);

            Color colorRedAlert = new Color(0.8f, 0, 0, opacityCritical + (1 - opacityCritical) * opacity);

            //            Color colorRedAlert = new Color(color25To21.r, color25To21.g, color25To21.b, opacityCritical + (1 - opacityCritical) * opacity);

            Color colorOrangeAlert = new Color(color20To16.r, color20To16.g, color20To16.b, opacityCritical + (1 - opacityCritical) * opacity);

            Color colorYellowAlert = new Color(color15To11.r, color15To11.g, color15To11.b, opacityCritical + (1 - opacityCritical) * opacity);

            int iconNum = 0;

            PawnStats pawnStats;
            if (colonist.Dead || colonist.holder != null || !_statsDict.TryGetValue(colonist, out pawnStats) ||
                colonist.drafter == null || colonist.skills == null)
                return;

            Vector3 bodyLoc = colonist.DrawPos;

            // Target Point 
            if (PsiSettings.ShowTargetPoint && (pawnStats.TargetPos != Vector3.zero))
            {
                if (PsiSettings.UseColoredTarget)
                {
                    Color skinColor = colonist.story.SkinColor;
                    Color hairColor = colonist.story.hairColor;

                    Material skinMat = PSIMaterials[Icons.TargetSkin];
                    Material hairMat = PSIMaterials[Icons.TargetHair];

                    if (skinMat == null)
                        return;

                    if (hairMat == null)
                        return;

                    DrawIcon_posOffset(pawnStats.TargetPos, Vector3.zero, skinMat, new Color(skinColor.r, skinColor.g, skinColor.b, 0.5f + opacity * 0.2f));
                    DrawIcon_posOffset(pawnStats.TargetPos, Vector3.zero, hairMat, new Color(hairColor.r, hairColor.g, hairColor.b, 0.5f + opacity * 0.2f)); ;
                }
                else
                {
                    Material targetMat = PSIMaterials[Icons.Target];
                    if (targetMat == null)
                        return;
                    DrawIcon_posOffset(pawnStats.TargetPos, Vector3.zero, targetMat, colorNeutralStatusSolid);
                }


            }

            //Drafted
            if (PsiSettings.ShowDraft && colonist.drafter.Drafted)
                DrawIcon(bodyLoc, ref iconNum, Icons.Draft, colorNeutralStatusSolid);

            if (pawnStats.MentalSanity != null)
            {
                // Berserk
                if (PsiSettings.ShowAggressive && pawnStats.MentalSanity == MentalStateDefOf.Berserk)
                    DrawIcon(bodyLoc, ref iconNum, Icons.Aggressive, colorRedAlert);

                // Binging on alcohol - needs refinement
                if (PsiSettings.ShowDrunk)
                {
                    if (pawnStats.MentalSanity == MentalStateDefOf.BingingDrugMajor)
                        DrawIcon(bodyLoc, ref iconNum, Icons.Drunk, colorOrangeAlert);
                    if (pawnStats.MentalSanity == MentalStateDefOf.BingingDrugExtreme)
                        DrawIcon(bodyLoc, ref iconNum, Icons.Drunk, colorRedAlert);
                }

                // Give Up Exit
                if (PsiSettings.ShowLeave && pawnStats.MentalSanity == MentalStateDefOf.PanicFlee) // was GiveUpExit
                    DrawIcon(bodyLoc, ref iconNum, Icons.Leave, colorRedAlert);

                //Daze Wander
                if (PsiSettings.ShowDazed && pawnStats.MentalSanity == MentalStateDefOf.WanderSad) // + MentalStateDefOf.WanderPsychotic
                    DrawIcon(bodyLoc, ref iconNum, Icons.Dazed, colorYellowAlert);

                //PanicFlee
                if (pawnStats.MentalSanity == MentalStateDefOf.PanicFlee)
                    DrawIcon(bodyLoc, ref iconNum, Icons.Panic, colorYellowAlert);
            }

            // Drunknes percent
            if (PsiSettings.ShowDrunk)
                if (pawnStats.Drunkness > 0.05)
                    DrawIcon_FadeFloatWithThreeColors(bodyLoc, ref iconNum, Icons.Drunk, pawnStats.Drunkness, colorYellowAlert, colorOrangeAlert, colorRedAlert);


            // Pacifc + Unarmed
            if (PsiSettings.ShowPacific || PsiSettings.ShowUnarmed)
            {
                if (colonist.skills.GetSkill(SkillDefOf.Melee).TotallyDisabled && colonist.skills.GetSkill(SkillDefOf.Shooting).TotallyDisabled)
                {
                    if (PsiSettings.ShowPacific)
                        DrawIcon(bodyLoc, ref iconNum, Icons.Pacific, colorNeutralStatus);
                }
                else if (PsiSettings.ShowUnarmed && colonist.equipment.Primary == null && !colonist.IsPrisonerOfColony)
                    DrawIcon(bodyLoc, ref iconNum, Icons.Unarmed, colorNeutralStatus);
            }
            // Trait Pyromaniac
            if (PsiSettings.ShowPyromaniac && colonist.story.traits.HasTrait(TraitDef.Named("Pyromaniac")))
                DrawIcon(bodyLoc, ref iconNum, Icons.Pyromaniac, colorYellowAlert);

            // Idle
            if (PsiSettings.ShowIdle && colonist.mindState.IsIdle)
                DrawIcon(bodyLoc, ref iconNum, Icons.Idle, colorNeutralStatus);

            MentalBreaker mb = !colonist.Dead ? colonist.mindState.mentalBreaker : null;

            // Bad Mood
            if (PsiSettings.ShowSad && colonist.needs.mood.CurLevelPercentage <= mb?.BreakThresholdMinor)
                DrawIcon_FadeRedAlertToNeutral(bodyLoc, ref iconNum, Icons.Sad, colonist.needs.mood.CurLevelPercentage / mb.BreakThresholdMinor);
            //   if (PsiSettings.ShowSad && colonist.needs.mood.CurLevel < (double)PsiSettings.LimitMoodLess)
            //DrawIcon_FadeRedAlertToNeutral(bodyLoc, iconNum, Icons.Sad, colonist.needs.mood.CurLevel / PsiSettings.LimitMoodLess);

            // Bloodloss
            if (PsiSettings.ShowBloodloss && pawnStats.BleedRate > 0.0f)
                DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.Bloodloss, pawnStats.BleedRate, colorRedAlert, colorNeutralStatus);

            //Health
            if (PsiSettings.ShowHealth)
            {
                float pawnHealth = colonist.health.summaryHealth.SummaryHealthPercent;
                //Infection
                if (pawnStats.HasLifeThreateningDisease)
                {
                    if (pawnHealth < pawnStats.HealthDisease)
                        DrawIcon_FadeFloatWithFourColorsHB(bodyLoc, ref iconNum, Icons.Health, pawnHealth, colorHealthBarGreen, colorYellowAlert, colorOrangeAlert, colorRedAlert);

                    else DrawIcon_FadeFloatWithFourColorsHB(bodyLoc, ref iconNum, Icons.Health, pawnStats.HealthDisease, colorHealthBarGreen, colorYellowAlert, colorOrangeAlert, colorRedAlert);

                }
                else if (colonist.health.summaryHealth.SummaryHealthPercent < 1f)
                {
                    DrawIcon_FadeFloatWithFourColorsHB(bodyLoc, ref iconNum, Icons.Health, pawnHealth, colorHealthBarGreen, colorYellowAlert, colorOrangeAlert, colorRedAlert);
                }

                //Toxic buildup
                if (pawnStats.ToxicBuildUp > 0.04f)
                    DrawIcon_FadeFloatFiveColors(bodyLoc, ref iconNum, Icons.Toxic, pawnStats.ToxicBuildUp, colorNeutralStatusFade, colorHealthBarGreen, colorYellowAlert, colorOrangeAlert, colorRedAlert);
            }



            // Sickness
            if (PsiSettings.ShowDisease && pawnStats.IsSick)
            {
                if (pawnStats.DiseaseDisappearance < PsiSettings.LimitDiseaseLess)
                {
                    DrawIcon_FadeFloatWithFourColorsHB(bodyLoc, ref iconNum, Icons.Sickness, pawnStats.DiseaseDisappearance / PsiSettings.LimitDiseaseLess, colorNeutralStatus, colorYellowAlert, colorOrangeAlert, colorRedAlert);
                }
                else
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.Sickness, colorNeutralStatus);
                }

            }

            // Pain

            if (PsiSettings.ShowPain)
            {
                if (colonist.story.traits.HasTrait(TraitDef.Named("Masochist")))
                {
                    if (pawnStats.PainMoodLevel == 0)
                        DrawIcon(bodyLoc, ref iconNum, Icons.Pain, colorMoodBoost * 0.4f);
                    if (pawnStats.PainMoodLevel == 1)
                        DrawIcon(bodyLoc, ref iconNum, Icons.Pain, colorMoodBoost * 0.6f);
                    if (pawnStats.PainMoodLevel == 2)
                        DrawIcon(bodyLoc, ref iconNum, Icons.Pain, colorMoodBoost * 0.8f);
                    if (pawnStats.PainMoodLevel == 3)
                        DrawIcon(bodyLoc, ref iconNum, Icons.Pain, colorMoodBoost);
                }
                else
                {
                    // pain is always worse, +5 to the icon color
                    if (pawnStats.PainMoodLevel == 0)
                        DrawIcon(bodyLoc, ref iconNum, Icons.Pain, color10To06);
                    if (pawnStats.PainMoodLevel == 1)
                        DrawIcon(bodyLoc, ref iconNum, Icons.Pain, color15To11);
                    if (pawnStats.PainMoodLevel == 2)
                        DrawIcon(bodyLoc, ref iconNum, Icons.Pain, color20To16);
                    if (pawnStats.PainMoodLevel == 3)
                        DrawIcon(bodyLoc, ref iconNum, Icons.Pain, color25To21);
                }

            }

            if (PsiSettings.ShowDisease)
            {
                if (colonist.health.ShouldBeTendedNow && !colonist.health.ShouldDoSurgeryNow)
                    DrawIcon(bodyLoc, ref iconNum, Icons.MedicalAttention, colorOrangeAlert);
                else if (colonist.health.ShouldBeTendedNow && colonist.health.ShouldDoSurgeryNow)
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.MedicalAttention, colorYellowAlert);
                    DrawIcon(bodyLoc, ref iconNum, Icons.MedicalAttention, colorOrangeAlert);
                }
                else if (colonist.health.ShouldDoSurgeryNow)
                    DrawIcon(bodyLoc, ref iconNum, Icons.MedicalAttention, colorYellowAlert);
            }

            // Hungry
            if (PsiSettings.ShowHungry && colonist.needs.food.CurLevel < (double)PsiSettings.LimitFoodLess)
                DrawIcon_FadeRedAlertToNeutral(bodyLoc, ref iconNum, Icons.Hungry,
                    colonist.needs.food.CurLevel / PsiSettings.LimitFoodLess);

            //Tired
            if (PsiSettings.ShowTired && colonist.needs.rest.CurLevel < (double)PsiSettings.LimitRestLess)
                DrawIcon_FadeRedAlertToNeutral(bodyLoc, ref iconNum, Icons.Tired,
                    colonist.needs.rest.CurLevel / PsiSettings.LimitRestLess);

            // Too Cold & too hot
            if (PsiSettings.ShowCold && pawnStats.TooCold > 0f)
            {
                if (pawnStats.TooCold >= 0f)
                {
                    if (pawnStats.TooCold <= 1f)
                        DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.Freezing, pawnStats.TooCold, colorNeutralStatusFade, colorYellowAlert);
                    else if (pawnStats.TooCold <= 1.5f)
                        DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.Freezing, (pawnStats.TooCold - 1f) * 2f, colorYellowAlert, colorOrangeAlert);
                    else
                        DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.Freezing, (pawnStats.TooCold - 1.5f) * 2f, colorOrangeAlert, colorRedAlert);
                }
            }
            else if (PsiSettings.ShowHot && pawnStats.TooHot > 0f && pawnStats.TooCold >= 0f)
            {
                if (pawnStats.TooHot <= 1f)
                    DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.Hot, pawnStats.TooHot, colorNeutralStatusFade, colorYellowAlert);
                else if (pawnStats.TooHot <= 1.5f)
                    DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.Hot, pawnStats.TooHot, colorYellowAlert, colorOrangeAlert);
                else
                    DrawIcon_FadeFloatWithTwoColors(bodyLoc, ref iconNum, Icons.Hot, pawnStats.TooHot - 1f, colorOrangeAlert, colorRedAlert);
            }


            // Bed status
            if (PsiSettings.ShowBedroom && !pawnStats.HasBed)
                DrawIcon(bodyLoc, ref iconNum, Icons.Bedroom, color10To06);


            // Usage of bed ...
            if (PsiSettings.ShowLovers && HasMood(colonist, ThoughtDef.Named("WantToSleepWithSpouseOrLover")))
            {
                DrawIcon(bodyLoc, ref iconNum, Icons.Love, colorYellowAlert);
            }

            //    if (ColBarSettings.ShowLovers && HasMood(colonist, ThoughtDef.Named("GotSomeLovin")))
            //    {
            //        DrawIcon(bodyLoc, iconNum, Icons.Love, colorMoodBoost);
            //    }



            if (PsiSettings.ShowLovers && HasMood(colonist, ThoughtDef.Named("GotMarried")))
            {
                DrawIcon(bodyLoc, ref iconNum, Icons.Marriage, colorMoodBoost);
            }

            if (PsiSettings.ShowLovers && HasMood(colonist, ThoughtDef.Named("HoneymoonPhase")))
            {
                DrawIcon(bodyLoc, ref iconNum, Icons.Marriage, colorMoodBoost / 2);
            }

            if (PsiSettings.ShowLovers && HasMood(colonist, ThoughtDef.Named("AttendedWedding")))
            {
                DrawIcon(bodyLoc, ref iconNum, Icons.Marriage, colorMoodBoost / 4);
            }

            // Naked
            if (PsiSettings.ShowNaked && HasMood(colonist, ThoughtDef.Named("Naked")))
            {
                DrawIcon(bodyLoc, ref iconNum, Icons.Naked, color10To06);
            }

            // Apparel
            if (PsiSettings.ShowApparelHealth && pawnStats.ApparelHealth < (double)PsiSettings.LimitApparelHealthLess)
            {
                double pawnApparelHealth = pawnStats.ApparelHealth / (double)PsiSettings.LimitApparelHealthLess;
                DrawIcon_FadeRedAlertToNeutral(bodyLoc, ref iconNum, Icons.ApparelHealth, (float)pawnApparelHealth);
            }

            // Moods caused by traits

            if (PsiSettings.ShowProsthophile && HasMood(colonist, ThoughtDef.Named("ProsthophileNoProsthetic")))
            {
                DrawIcon(bodyLoc, ref iconNum, Icons.Prosthophile, color05AndLess);
            }

            if (PsiSettings.ShowProsthophobe && HasMood(colonist, ThoughtDef.Named("ProsthophobeUnhappy")))
            {
                DrawIcon(bodyLoc, ref iconNum, Icons.Prosthophobe, color10To06);
            }

            if (PsiSettings.ShowNightOwl && HasMood(colonist, ThoughtDef.Named("NightOwlDuringTheDay")))
            {
                DrawIcon(bodyLoc, ref iconNum, Icons.NightOwl, color10To06);
            }

            if (PsiSettings.ShowGreedy && HasMood(colonist, ThoughtDef.Named("Greedy")))
            {
                DrawIcon(bodyLoc, ref iconNum, Icons.Greedy, color10To06);
            }

            if (PsiSettings.ShowJealous && HasMood(colonist, ThoughtDef.Named("Jealous")))
            {
                DrawIcon(bodyLoc, ref iconNum, Icons.Jealous, color10To06);
            }


            // Effectiveness
            if (PsiSettings.ShowEffectiveness && pawnStats.TotalEfficiency < (double)PsiSettings.LimitEfficiencyLess)
                DrawIcon_FadeRedAlertToNeutral(bodyLoc, ref iconNum, Icons.Effectiveness,
                    pawnStats.TotalEfficiency / PsiSettings.LimitEfficiencyLess);






            // Bad thoughts

            if (PsiSettings.ShowLeftUnburied && HasMood(colonist, ThoughtDef.Named("ColonistLeftUnburied")))
            {
                DrawIcon(bodyLoc, ref iconNum, Icons.LeftUnburied, color10To06);
            }

            if (PsiSettings.ShowDeadColonists)
            {
                // Close Family & friends / 25


                // not family, more whiter icon
                if (HasMood(colonist, ThoughtDef.Named("KilledColonist")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color05AndLess);
                }

                if (HasMood(colonist, ThoughtDef.Named("KilledColonyAnimal")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color05AndLess);
                }

                #region DeathMemory
                //Deathmemory
                // some of those need staging - to do
                if (HasMood(colonist, ThoughtDef.Named("KnowGuestExecuted")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color05AndLess);
                }
                if (HasMood(colonist, ThoughtDef.Named("KnowColonistExecuted")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color05AndLess);
                }
                if (HasMood(colonist, ThoughtDef.Named("KnowPrisonerDiedInnocent")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color10To06);
                }
                if (HasMood(colonist, ThoughtDef.Named("KnowColonistDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color05AndLess);
                }
                //Bonded animal died
                if (HasMood(colonist, ThoughtDef.Named("BondedAnimalDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color10To06);
                }
                // Friend / rival died
                if (HasMood(colonist, ThoughtDef.Named("PawnWithGoodOpinionDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color10To06);
                }

                if (HasMood(colonist, ThoughtDef.Named("PawnWithBadOpinionDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, colorMoodBoost);
                }

                #endregion

                #region DeathMemoryFamily

                if (HasMood(colonist, ThoughtDef.Named("MySonDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color25To21);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyDaughterDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color25To21);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyHusbandDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color25To21);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyWifeDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color25To21);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyFianceDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color20To16);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyFianceeDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color20To16);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyLoverDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color20To16);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyBrotherDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color15To11);
                }

                if (HasMood(colonist, ThoughtDef.Named("MySisterDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color15To11);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyGrandchildDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color15To11);
                }

                // 10

                if (HasMood(colonist, ThoughtDef.Named("MyFatherDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color10To06);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyMotherDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color10To06);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyNieceDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color05AndLess);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyNephewDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color05AndLess);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyHalfSiblingDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color05AndLess);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyAuntDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color05AndLess);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyUncleDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color05AndLess);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyGrandparentDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color05AndLess);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyCousinDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color05AndLess);
                }
                if (HasMood(colonist, ThoughtDef.Named("MyKinDied")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color05AndLess);
                }

                #endregion

                //Memory misc
                if (HasMood(colonist, ThoughtDef.Named("WitnessedDeathAlly")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color10To06);
                }
                if (HasMood(colonist, ThoughtDef.Named("WitnessedDeathNonAlly")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color05AndLess);
                }
                if (HasMood(colonist, ThoughtDef.Named("WitnessedDeathFamily")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, color10To06);
                }
                if (HasMood(colonist, ThoughtDef.Named("WitnessedDeathBloodlust")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, colorMoodBoost);
                }
                if (HasMood(colonist, ThoughtDef.Named("KilledHumanlikeBloodlust")))
                {
                    DrawIcon(bodyLoc, ref iconNum, Icons.DeadColonist, colorMoodBoost);
                }

                // Crowded missing since A14?

                if (PsiSettings.ShowRoomStatus && pawnStats.CrowdedMoodLevel != 0)
                {
                    if (pawnStats.CrowdedMoodLevel == 0)
                        DrawIcon(bodyLoc, ref iconNum, Icons.Crowded, colorNeutralStatusFade);
                    if (pawnStats.CrowdedMoodLevel == 1)
                        DrawIcon(bodyLoc, ref iconNum, Icons.Crowded, colorYellowAlert);
                    if (pawnStats.CrowdedMoodLevel == 2)
                        DrawIcon(bodyLoc, ref iconNum, Icons.Crowded, colorOrangeAlert);
                }
            }

        }

        public static void DrawColonistIconsOnBar(Rect rect, Pawn colonist, float rectAlpha)
        {
            float opacityCritical = 1f;
            var opacity = 1f;


            Color color25To21 = new Color(0.8f, 0f, 0f, opacity);

            Color color20To16 = new Color(0.9f, 0.45f, 0f, opacity);

            Color color15To11 = new Color(0.95f, 0.95f, 0f, opacity);

            Color color10To06 = new Color(0.95f, 0.95f, 0.66f, opacity);

            Color color05AndLess = new Color(0.9f, 0.9f, 0.9f, opacity);

            Color colorMoodBoost = new Color(0f, 0.8f, 0f, opacity);

            Color colorNeutralStatus = color05AndLess; // new Color(1f, 1f, 1f, transparancy);

            Color colorNeutralStatusSolid = new Color(colorNeutralStatus.r, colorNeutralStatus.g, colorNeutralStatus.b, 0.5f + opacity * 0.2f);

            Color colorNeutralStatusFade = new Color(colorNeutralStatus.r, colorNeutralStatus.g, colorNeutralStatus.b, opacity / 4);


            Color colorHealthBarGreen = new Color(0f, 0.8f, 0f, opacity * 0.5f);

            Color colorRedAlert = new Color(0.8f, 0, 0, opacityCritical + (1 - opacityCritical) * opacity);

            //            Color colorRedAlert = new Color(color25To21.r, color25To21.g, color25To21.b, opacityCritical + (1 - opacityCritical) * opacity);

            Color colorOrangeAlert = new Color(color20To16.r, color20To16.g, color20To16.b, opacityCritical + (1 - opacityCritical) * opacity);

            Color colorYellowAlert = new Color(color15To11.r, color15To11.g, color15To11.b, opacityCritical + (1 - opacityCritical) * opacity);

            int iconNum = 0;

            PawnStats pawnStats;
            if (colonist.Dead || colonist.holder != null || !_statsDict.TryGetValue(colonist, out pawnStats) ||
                colonist.drafter == null || colonist.skills == null)
                return;

            //Drafted
            if (ColBarSettings.ShowDraft && colonist.drafter.Drafted)
                DrawIcon(rect, ref iconNum, Icons.Draft, colorNeutralStatusSolid, rectAlpha);

            if (pawnStats.MentalSanity != null)
            {
           //   // Berserk
           //   if (ColBarSettings.ShowAggressive && pawnStats.MentalSanity == MentalStateDefOf.Berserk)
           //       DrawIcon(rect, ref iconNum, Icons.Aggressive, colorRedAlert, rectAlpha);

                // Binging on alcohol - needs refinement
                if (ColBarSettings.ShowDrunk)
                {
                    if (pawnStats.MentalSanity == MentalStateDefOf.BingingDrugMajor)
                        DrawIcon(rect, ref iconNum, Icons.Drunk, colorOrangeAlert, rectAlpha);
                    if (pawnStats.MentalSanity == MentalStateDefOf.BingingDrugExtreme)
                        DrawIcon(rect, ref iconNum, Icons.Drunk, colorRedAlert, rectAlpha);
                }

     //         // Give Up Exit
     //         if (ColBarSettings.ShowLeave && pawnStats.MentalSanity == MentalStateDefOf.PanicFlee) // was GiveUpExit
     //             DrawIcon(rect, ref iconNum, Icons.Leave, colorRedAlert, rectAlpha);
     //
     //         //Daze Wander
     //         if (ColBarSettings.ShowDazed && pawnStats.MentalSanity == MentalStateDefOf.WanderSad) // + MentalStateDefOf.WanderPsychotic
     //             DrawIcon(rect, ref iconNum, Icons.Dazed, colorYellowAlert, rectAlpha);

                //PanicFlee
                if (pawnStats.MentalSanity == MentalStateDefOf.PanicFlee)
                    DrawIcon(rect, ref iconNum, Icons.Panic, colorYellowAlert, rectAlpha);
            }

            // Drunknes percent
            if (ColBarSettings.ShowDrunk)
                if (pawnStats.Drunkness > 0.05)
                    DrawIcon_FadeFloatWithThreeColors(rect, ref iconNum, Icons.Drunk, pawnStats.Drunkness, colorYellowAlert, colorOrangeAlert, colorRedAlert, rectAlpha);


            // Pacifc + Unarmed
            if (ColBarSettings.ShowPacific || ColBarSettings.ShowUnarmed)
            {
                if (colonist.skills.GetSkill(SkillDefOf.Melee).TotallyDisabled && colonist.skills.GetSkill(SkillDefOf.Shooting).TotallyDisabled)
                {
                    if (ColBarSettings.ShowPacific)
                        DrawIcon(rect, ref iconNum, Icons.Pacific, colorNeutralStatus, rectAlpha);
                }
                else if (ColBarSettings.ShowUnarmed && colonist.equipment.Primary == null && !colonist.IsPrisonerOfColony)
                    DrawIcon(rect, ref iconNum, Icons.Unarmed, colorNeutralStatus, rectAlpha);
            }
            // Trait Pyromaniac
            if (ColBarSettings.ShowPyromaniac && colonist.story.traits.HasTrait(TraitDef.Named("Pyromaniac")))
                DrawIcon(rect, ref iconNum, Icons.Pyromaniac, colorYellowAlert, rectAlpha);

      //    // Idle
      //    if (ColBarSettings.ShowIdle && colonist.mindState.IsIdle)
      //        DrawIcon(rect, ref iconNum, Icons.Idle, colorNeutralStatus, rectAlpha);

            MentalBreaker mb = !colonist.Dead ? colonist.mindState.mentalBreaker : null;

            // Bad Mood
            if (ColBarSettings.ShowSad && colonist.needs.mood.CurLevelPercentage <= mb?.BreakThresholdMinor)
                DrawIcon_FadeRedAlertToNeutral(rect, ref iconNum, Icons.Sad, colonist.needs.mood.CurLevelPercentage / mb.BreakThresholdMinor, rectAlpha);

            // Bloodloss
            if (ColBarSettings.ShowBloodloss && pawnStats.BleedRate > 0.0f)
                DrawIcon_FadeFloatWithTwoColors(rect, ref iconNum, Icons.Bloodloss, pawnStats.BleedRate, colorRedAlert, colorNeutralStatus, rectAlpha);

            //Health
            if (ColBarSettings.ShowHealth)
            {
                float pawnHealth = colonist.health.summaryHealth.SummaryHealthPercent;
                //Infection
                if (pawnStats.HasLifeThreateningDisease)
                {
                    if (pawnHealth < pawnStats.HealthDisease)
                        DrawIcon_FadeFloatWithFourColorsHB(rect, ref iconNum, Icons.Health, pawnHealth, colorHealthBarGreen, colorYellowAlert, colorOrangeAlert, colorRedAlert, rectAlpha);

                    else DrawIcon_FadeFloatWithFourColorsHB(rect, ref iconNum, Icons.Health, pawnStats.HealthDisease, colorHealthBarGreen, colorYellowAlert, colorOrangeAlert, colorRedAlert, rectAlpha);

                }
                else if (colonist.health.summaryHealth.SummaryHealthPercent < 1f)
                {
                    DrawIcon_FadeFloatWithFourColorsHB(rect, ref iconNum, Icons.Health, pawnHealth, colorHealthBarGreen, colorYellowAlert, colorOrangeAlert, colorRedAlert, rectAlpha);
                }

                //Toxic buildup
                if (pawnStats.ToxicBuildUp > 0.04f)
                    DrawIcon_FadeFloatFiveColors(rect, ref iconNum, Icons.Toxic, pawnStats.ToxicBuildUp, colorNeutralStatusFade, colorHealthBarGreen, colorYellowAlert, colorOrangeAlert, colorRedAlert, rectAlpha);
            }



            // Sickness
            if (ColBarSettings.ShowDisease && pawnStats.IsSick)
            {
                if (pawnStats.DiseaseDisappearance < ColBarSettings.LimitDiseaseLess)
                {
                    DrawIcon_FadeFloatWithFourColorsHB(rect, ref iconNum, Icons.Sickness, pawnStats.DiseaseDisappearance / ColBarSettings.LimitDiseaseLess, colorNeutralStatus, colorYellowAlert, colorOrangeAlert, colorRedAlert, rectAlpha);
                }
                else
                {
                    DrawIcon(rect, ref iconNum, Icons.Sickness, colorNeutralStatus, rectAlpha);
                }

            }

            // Pain

            if (ColBarSettings.ShowPain)
            {
                if (colonist.story.traits.HasTrait(TraitDef.Named("Masochist")))
                {
                    if (pawnStats.PainMoodLevel == 0)
                        DrawIcon(rect, ref iconNum, Icons.Pain, colorMoodBoost * 0.4f, rectAlpha);
                    if (pawnStats.PainMoodLevel == 1)
                        DrawIcon(rect, ref iconNum, Icons.Pain, colorMoodBoost * 0.6f, rectAlpha);
                    if (pawnStats.PainMoodLevel == 2)
                        DrawIcon(rect, ref iconNum, Icons.Pain, colorMoodBoost * 0.8f, rectAlpha);
                    if (pawnStats.PainMoodLevel == 3)
                        DrawIcon(rect, ref iconNum, Icons.Pain, colorMoodBoost, rectAlpha);
                }
                else
                {
                    // pain is always worse, +5 to the icon color
                    if (pawnStats.PainMoodLevel == 0)
                        DrawIcon(rect, ref iconNum, Icons.Pain, color10To06, rectAlpha);
                    if (pawnStats.PainMoodLevel == 1)
                        DrawIcon(rect, ref iconNum, Icons.Pain, color15To11, rectAlpha);
                    if (pawnStats.PainMoodLevel == 2)
                        DrawIcon(rect, ref iconNum, Icons.Pain, color20To16, rectAlpha);
                    if (pawnStats.PainMoodLevel == 3)
                        DrawIcon(rect, ref iconNum, Icons.Pain, color25To21, rectAlpha);
                }

            }

            if (ColBarSettings.ShowDisease)
            {
                if (colonist.health.ShouldBeTendedNow && !colonist.health.ShouldDoSurgeryNow)
                    DrawIcon(rect, ref iconNum, Icons.MedicalAttention, colorOrangeAlert, rectAlpha);
                else if (colonist.health.ShouldBeTendedNow && colonist.health.ShouldDoSurgeryNow)
                {
                    DrawIcon(rect, ref iconNum, Icons.MedicalAttention, colorYellowAlert, rectAlpha);
                    DrawIcon(rect, ref iconNum, Icons.MedicalAttention, colorOrangeAlert, rectAlpha);
                }
                else if (colonist.health.ShouldDoSurgeryNow)
                    DrawIcon(rect, ref iconNum, Icons.MedicalAttention, colorYellowAlert, rectAlpha);
            }

            // Hungry
            if (ColBarSettings.ShowHungry && colonist.needs.food.CurLevel < (double)ColBarSettings.LimitFoodLess)
                DrawIcon_FadeRedAlertToNeutral(rect, ref iconNum, Icons.Hungry,
                    colonist.needs.food.CurLevel / ColBarSettings.LimitFoodLess, rectAlpha);

            //Tired
            if (ColBarSettings.ShowTired && colonist.needs.rest.CurLevel < (double)ColBarSettings.LimitRestLess)
                DrawIcon_FadeRedAlertToNeutral(rect, ref iconNum, Icons.Tired,
                    colonist.needs.rest.CurLevel / ColBarSettings.LimitRestLess, rectAlpha);

            // Too Cold & too hot
            if (ColBarSettings.ShowCold && pawnStats.TooCold > 0f)
            {
                if (pawnStats.TooCold >= 0f)
                {
                    if (pawnStats.TooCold <= 1f)
                        DrawIcon_FadeFloatWithTwoColors(rect, ref iconNum, Icons.Freezing, pawnStats.TooCold, colorNeutralStatusFade, colorYellowAlert, rectAlpha);
                    else if (pawnStats.TooCold <= 1.5f)
                        DrawIcon_FadeFloatWithTwoColors(rect, ref iconNum, Icons.Freezing, (pawnStats.TooCold - 1f) * 2f, colorYellowAlert, colorOrangeAlert, rectAlpha);
                    else
                        DrawIcon_FadeFloatWithTwoColors(rect, ref iconNum, Icons.Freezing, (pawnStats.TooCold - 1.5f) * 2f, colorOrangeAlert, colorRedAlert, rectAlpha);
                }
            }
            else if (ColBarSettings.ShowHot && pawnStats.TooHot > 0f && pawnStats.TooCold >= 0f)
            {
                if (pawnStats.TooHot <= 1f)
                    DrawIcon_FadeFloatWithTwoColors(rect, ref iconNum, Icons.Hot, pawnStats.TooHot, colorNeutralStatusFade, colorYellowAlert, rectAlpha);
                else if (pawnStats.TooHot <= 1.5f)
                    DrawIcon_FadeFloatWithTwoColors(rect, ref iconNum, Icons.Hot, pawnStats.TooHot, colorYellowAlert, colorOrangeAlert, rectAlpha);
                else
                    DrawIcon_FadeFloatWithTwoColors(rect, ref iconNum, Icons.Hot, pawnStats.TooHot - 1f, colorOrangeAlert, colorRedAlert, rectAlpha);
            }


            // Bed status
            if (ColBarSettings.ShowBedroom && !pawnStats.HasBed)
                DrawIcon(rect, ref iconNum, Icons.Bedroom, color10To06, rectAlpha);


            // Usage of bed ...
            if (ColBarSettings.ShowLovers && HasMood(colonist, ThoughtDef.Named("WantToSleepWithSpouseOrLover")))
            {
                DrawIcon(rect, ref iconNum, Icons.Love, colorYellowAlert, rectAlpha);
            }

            //    if (ColBarSettings.ShowLovers && HasMood(colonist, ThoughtDef.Named("GotSomeLovin")))
            //    {
            //        DrawIcon(bodyLoc, iconNum, Icons.Love, colorMoodBoost);
            //    }



            if (ColBarSettings.ShowLovers && HasMood(colonist, ThoughtDef.Named("GotMarried")))
            {
                DrawIcon(rect, ref iconNum, Icons.Marriage, colorMoodBoost, rectAlpha);
            }

            if (ColBarSettings.ShowLovers && HasMood(colonist, ThoughtDef.Named("HoneymoonPhase")))
            {
                DrawIcon(rect, ref iconNum, Icons.Marriage, colorMoodBoost / 2, rectAlpha);
            }

            if (ColBarSettings.ShowLovers && HasMood(colonist, ThoughtDef.Named("AttendedWedding")))
            {
                DrawIcon(rect, ref iconNum, Icons.Marriage, colorMoodBoost / 4, rectAlpha);
            }

            // Naked
            if (ColBarSettings.ShowNaked && HasMood(colonist, ThoughtDef.Named("Naked")))
            {
                DrawIcon(rect, ref iconNum, Icons.Naked, color10To06, rectAlpha);
            }

            // Apparel
            if (ColBarSettings.ShowApparelHealth && pawnStats.ApparelHealth < (double)ColBarSettings.LimitApparelHealthLess)
            {
                double pawnApparelHealth = pawnStats.ApparelHealth / (double)ColBarSettings.LimitApparelHealthLess;
                DrawIcon_FadeRedAlertToNeutral(rect, ref iconNum, Icons.ApparelHealth, (float)pawnApparelHealth, rectAlpha);
            }

            // Moods caused by traits

            if (ColBarSettings.ShowProsthophile && HasMood(colonist, ThoughtDef.Named("ProsthophileNoProsthetic")))
            {
                DrawIcon(rect, ref iconNum, Icons.Prosthophile, color05AndLess, rectAlpha);
            }

            if (ColBarSettings.ShowProsthophobe && HasMood(colonist, ThoughtDef.Named("ProsthophobeUnhappy")))
            {
                DrawIcon(rect, ref iconNum, Icons.Prosthophobe, color10To06, rectAlpha);
            }

            if (ColBarSettings.ShowNightOwl && HasMood(colonist, ThoughtDef.Named("NightOwlDuringTheDay")))
            {
                DrawIcon(rect, ref iconNum, Icons.NightOwl, color10To06, rectAlpha);
            }

            if (ColBarSettings.ShowGreedy && HasMood(colonist, ThoughtDef.Named("Greedy")))
            {
                DrawIcon(rect, ref iconNum, Icons.Greedy, color10To06, rectAlpha);
            }

            if (ColBarSettings.ShowJealous && HasMood(colonist, ThoughtDef.Named("Jealous")))
            {
                DrawIcon(rect, ref iconNum, Icons.Jealous, color10To06, rectAlpha);
            }


            // Effectiveness
            if (ColBarSettings.ShowEffectiveness && pawnStats.TotalEfficiency < (double)ColBarSettings.LimitEfficiencyLess)
                DrawIcon_FadeRedAlertToNeutral(rect, ref iconNum, Icons.Effectiveness,
                    pawnStats.TotalEfficiency / ColBarSettings.LimitEfficiencyLess, rectAlpha);






            // Bad thoughts

            if (ColBarSettings.ShowLeftUnburied && HasMood(colonist, ThoughtDef.Named("ColonistLeftUnburied")))
            {
                DrawIcon(rect, ref iconNum, Icons.LeftUnburied, color10To06, rectAlpha);
            }

            if (ColBarSettings.ShowDeadColonists)
            {
                // Close Family & friends / 25

                // not family, more whiter icon
                if (HasMood(colonist, ThoughtDef.Named("KilledColonist")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color05AndLess, rectAlpha);
                }

                if (HasMood(colonist, ThoughtDef.Named("KilledColonyAnimal")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color05AndLess, rectAlpha);
                }

                #region DeathMemory
                //Deathmemory
                // some of those need staging - to do
                if (HasMood(colonist, ThoughtDef.Named("KnowGuestExecuted")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color05AndLess, rectAlpha);
                }
                if (HasMood(colonist, ThoughtDef.Named("KnowColonistExecuted")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color05AndLess, rectAlpha);
                }
                if (HasMood(colonist, ThoughtDef.Named("KnowPrisonerDiedInnocent")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color10To06, rectAlpha);
                }
                if (HasMood(colonist, ThoughtDef.Named("KnowColonistDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color05AndLess, rectAlpha);
                }
                //Bonded animal died
                if (HasMood(colonist, ThoughtDef.Named("BondedAnimalDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color10To06, rectAlpha);
                }
                // Friend / rival died
                if (HasMood(colonist, ThoughtDef.Named("PawnWithGoodOpinionDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color10To06, rectAlpha);
                }

                if (HasMood(colonist, ThoughtDef.Named("PawnWithBadOpinionDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, colorMoodBoost, rectAlpha);
                }

                #endregion

                #region DeathMemoryFamily

                if (HasMood(colonist, ThoughtDef.Named("MySonDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color25To21, rectAlpha);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyDaughterDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color25To21, rectAlpha);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyHusbandDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color25To21, rectAlpha);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyWifeDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color25To21, rectAlpha);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyFianceDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color20To16, rectAlpha);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyFianceeDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color20To16, rectAlpha);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyLoverDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color20To16, rectAlpha);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyBrotherDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color15To11, rectAlpha);
                }

                if (HasMood(colonist, ThoughtDef.Named("MySisterDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color15To11, rectAlpha);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyGrandchildDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color15To11, rectAlpha);
                }

                // 10

                if (HasMood(colonist, ThoughtDef.Named("MyFatherDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color10To06, rectAlpha);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyMotherDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color10To06, rectAlpha);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyNieceDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color05AndLess, rectAlpha);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyNephewDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color05AndLess, rectAlpha);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyHalfSiblingDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color05AndLess, rectAlpha);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyAuntDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color05AndLess, rectAlpha);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyUncleDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color05AndLess, rectAlpha);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyGrandparentDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color05AndLess, rectAlpha);
                }

                if (HasMood(colonist, ThoughtDef.Named("MyCousinDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color05AndLess, rectAlpha);
                }
                if (HasMood(colonist, ThoughtDef.Named("MyKinDied")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color05AndLess, rectAlpha);
                }

                #endregion

                //Memory misc
                if (HasMood(colonist, ThoughtDef.Named("WitnessedDeathAlly")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color10To06, rectAlpha);
                }
                if (HasMood(colonist, ThoughtDef.Named("WitnessedDeathNonAlly")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color05AndLess, rectAlpha);
                }
                if (HasMood(colonist, ThoughtDef.Named("WitnessedDeathFamily")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, color10To06, rectAlpha);
                }
                if (HasMood(colonist, ThoughtDef.Named("WitnessedDeathBloodlust")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, colorMoodBoost, rectAlpha);
                }
                if (HasMood(colonist, ThoughtDef.Named("KilledHumanlikeBloodlust")))
                {
                    DrawIcon(rect, ref iconNum, Icons.DeadColonist, colorMoodBoost, rectAlpha);
                }

                // Crowded missing since A14?

                if (ColBarSettings.ShowRoomStatus && pawnStats.CrowdedMoodLevel != 0)
                {
                    if (pawnStats.CrowdedMoodLevel == 0)
                        DrawIcon(rect, ref iconNum, Icons.Crowded, colorNeutralStatusFade, rectAlpha);
                    if (pawnStats.CrowdedMoodLevel == 1)
                        DrawIcon(rect, ref iconNum, Icons.Crowded, colorYellowAlert, rectAlpha);
                    if (pawnStats.CrowdedMoodLevel == 2)
                        DrawIcon(rect, ref iconNum, Icons.Crowded, colorOrangeAlert, rectAlpha);
                }
            }

        }


        #endregion
    }
}