#if !NoCCL
using CommunityCoreLibrary;
using CommunityCoreLibrary.UI;
#endif

using System;
using System.Collections.Generic;
using System.IO;
using ColonistBarKF.PSI;
using UnityEngine;
using Verse;
using static ColonistBarKF.CBKF;
using static ColonistBarKF.PSI.PSI;

namespace ColonistBarKF
{
#if !NoCCL
    public class ModConfigMenu : ModConfigurationMenu
#else
    public class ColonistBarKF_Settings : Window
#endif
    {
        public static int lastupdate = -5000;

        private static int _iconLimit;

        private static ColorWrapper colourWrapper;

        public Window OptionsDialog;

        public ColonistBarKF_Settings()
        {
            forcePause = false;
            doCloseX = true;
            draggable = true;
            drawShadow = true;
            preventCameraMotion = false;
            resizeable = true;
            onlyOneOfTypeAllowed = true;
            Reinit(false, true);
        }


        private void DrawCheckboxArea(
            string iconName,
            Material iconMaterial,
            ref bool colBarBool,
            ref bool psiBarBool,
            ref int iconInRow)
        {
            if (iconInRow > _iconLimit - 1)
            {
                GUILayout.EndHorizontal();

                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.BeginHorizontal();
                iconInRow = 0;
            }

            if (iconInRow > 0 && _iconLimit != 1) GUILayout.Space(Text.LineHeight / 2);

            GUILayout.BeginVertical(_fondImages);

            GUILayout.BeginHorizontal(_darkGrayBgImage, GUILayout.Height(Text.LineHeight * 1.2f));
            GUILayout.FlexibleSpace();
            GUILayout.Label(iconName, _fontBold);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical();
            GUILayout.Space(Text.LineHeight / 2);

            if (iconMaterial != null)
            {
                GUILayout.Label(iconMaterial.mainTexture, GUILayout.Width(Text.LineHeight * 2.5f), GUILayout.Height(Text.LineHeight * 2.5f));
            }
            else
            {
                if (psiBarBool || colBarBool)
                {
                    GUI.color = Color.red;
                    GUILayout.Label(
                        "PSI.Settings.IconSet.IconNotAvailable".Translate(), GUILayout.Width(Text.LineHeight * 2.5f), GUILayout.Height(Text.LineHeight * 2.5f));
                    GUI.color = Color.white;
                }
                else
                {
                    GUILayout.Label(string.Empty, GUILayout.Width(Text.LineHeight * 2.5f), GUILayout.Height(Text.LineHeight * 2.5f));
                }
            }

            GUILayout.Space(Text.LineHeight / 2);

            // Label("PSI.Settings.VisibilityButton".Translate(), FontBold);
            colBarBool = GUILayout.Toggle(colBarBool, "CBKF.Settings.ColBarIconVisibility".Translate());
            psiBarBool = GUILayout.Toggle(psiBarBool, "CBKF.Settings.PSIIconVisibility".Translate());
            GUILayout.Space(Text.LineHeight / 2);

            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            iconInRow++;
        }

        private void DrawCheckboxAreaTarget(
            string iconName,
            Material targetSingle,
            Material targetHair,
            Material targetSkin,
            ref int iconInRow)
        {
            GUILayout.BeginVertical(_fondImages);

            GUILayout.BeginHorizontal(_darkGrayBgImage, GUILayout.Height(Text.LineHeight * 1.2f));
            GUILayout.FlexibleSpace();
            GUILayout.Label(iconName, _fontBold);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical();
            GUILayout.Space(Text.LineHeight / 2);
            if (PsiSettings.ShowTargetPoint)
            {
                if (!PsiSettings.UseColoredTarget)
                {
                    if (targetSingle != null)
                    {
                        GUILayout.Label(targetSingle.mainTexture, GUILayout.Width(Text.LineHeight * 2.5f), GUILayout.Height(Text.LineHeight * 2.5f));
                    }
                    else
                    {
                        GUI.color = Color.red;
                        GUILayout.Label(
                            "PSI.Settings.IconSet.IconNotAvailable".Translate(), GUILayout.Width(Text.LineHeight * 2.5f), GUILayout.Height(Text.LineHeight * 2.5f));
                        GUI.color = Color.white;
                    }
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    if (targetHair != null)
                    {
                        GUILayout.Label(targetHair.mainTexture, GUILayout.Width(Text.LineHeight * 2.5f), GUILayout.Height(Text.LineHeight * 2.5f));
                    }
                    else
                    {
                        GUI.color = Color.red;
                        GUILayout.Label(
                            "PSI.Settings.IconSet.IconNotAvailable".Translate() + " HairTarget", GUILayout.Width(Text.LineHeight * 2.5f), GUILayout.Height(Text.LineHeight * 2.5f));
                        GUI.color = Color.white;
                    }

                    if (targetSkin != null)
                    {
                        GUILayout.Label(targetSkin.mainTexture, GUILayout.Width(Text.LineHeight * 2.5f), GUILayout.Height(Text.LineHeight * 2.5f));
                    }
                    else
                    {
                        GUI.color = Color.red;
                        GUILayout.Label(
                            "PSI.Settings.IconSet.IconNotAvailable".Translate() + " SkinTarget", GUILayout.Width(Text.LineHeight * 2.5f), GUILayout.Height(Text.LineHeight * 2.5f));
                        GUI.color = Color.white;
                    }

                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.Label(string.Empty, GUILayout.Width(Text.LineHeight * 2.5f), GUILayout.Height(Text.LineHeight * 2.5f));
            }

            GUILayout.Space(Text.LineHeight / 2);

            // Label("PSI.Settings.VisibilityButton".Translate(), FontBold);
            PsiSettings.ShowTargetPoint = GUILayout.Toggle(
                PsiSettings.ShowTargetPoint,
                "PSI.Settings.Visibility.TargetPoint".Translate());
            PsiSettings.UseColoredTarget = GUILayout.Toggle(
                PsiSettings.UseColoredTarget,
                "PSI.Settings.IconOpacityAndColor.UseColoredTarget".Translate());

            GUILayout.Space(Text.LineHeight / 2);

            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            iconInRow += 2;
        }

        private void FillPageAdvanced()
        {
            GUILayout.BeginVertical(_fondBoxes);

            ColBarSettings.UseCustomIconSize = GUILayout.Toggle(
                ColBarSettings.UseCustomIconSize,
                "CBKF.Settings.BasicSize".Translate() + ColBarSettings.BaseSizeFloat.ToString("N0") + " px, "
                + (ColBarSettings.UseFixedIconScale
                       ? (ColBarSettings.FixedIconScaleFloat * 100).ToString("N0") + " %, "
                       : (ColonistBar_KF.helper.cachedScale * 100).ToString("N0") + " %, ")
                + (int)ColBarSettings.BaseSpacingHorizontal + " x, " + (int)ColBarSettings.BaseSpacingVertical + " y");

            if (ColBarSettings.UseCustomIconSize)
            {
                GUILayout.Space(Text.LineHeight / 2);

                ColBarSettings.BaseSizeFloat = GUILayout.HorizontalSlider(ColBarSettings.BaseSizeFloat, 24f, 128f);

                ColBarSettings.BaseSpacingHorizontal = GUILayout.HorizontalSlider(ColBarSettings.BaseSpacingHorizontal, 1f, 72f);
                ColBarSettings.BaseSpacingVertical = GUILayout.HorizontalSlider(ColBarSettings.BaseSpacingVertical, 1f, 96f);
            }
            else
            {
                ColBarSettings.BaseSizeFloat = 48f;
                ColBarSettings.BaseSpacingHorizontal = 24f;
                ColBarSettings.BaseSpacingVertical = 32f;
            }

            #region Fixed Scaling

            ColBarSettings.UseFixedIconScale = GUILayout.Toggle(
                ColBarSettings.UseFixedIconScale,
                "CBKF.Settings.FixedScale".Translate());
            if (ColBarSettings.UseFixedIconScale)
            {
                ColBarSettings.FixedIconScaleFloat = GUILayout.HorizontalSlider(ColBarSettings.FixedIconScaleFloat, 0.2f, 2.5f);
            }
            else
            {
                ColBarSettings.FixedIconScaleFloat = 1f;
            }

            #endregion

            GUILayout.EndVertical();



            #region Camera

            GUILayout.BeginVertical(_fondBoxes);
            ColBarSettings.UseCustomPawnTextureCameraOffsets = GUILayout.Toggle(
                ColBarSettings.UseCustomPawnTextureCameraOffsets,
                "CBKF.Settings.PawnTextureCameraOffsets".Translate()
                + ColBarSettings.PawnTextureCameraHorizontalOffset.ToString("N2") + " x, "
                + ColBarSettings.PawnTextureCameraVerticalOffset.ToString("N2") + " y, "
                + ColBarSettings.PawnTextureCameraZoom.ToString("N2") + " z");
            if (ColBarSettings.UseCustomPawnTextureCameraOffsets)
            {
                GUILayout.Space(Text.LineHeight / 2);
                ColBarSettings.PawnTextureCameraHorizontalOffset = GUILayout.HorizontalSlider(ColBarSettings.PawnTextureCameraHorizontalOffset, 0.7f, -0.7f);
                ColBarSettings.PawnTextureCameraVerticalOffset = GUILayout.HorizontalSlider(ColBarSettings.PawnTextureCameraVerticalOffset, 0f, 1f);
                ColBarSettings.PawnTextureCameraZoom = GUILayout.HorizontalSlider(ColBarSettings.PawnTextureCameraZoom, 0.3f, 3f);
            }
            else
            {
                ColBarSettings.PawnTextureCameraHorizontalOffset = 0f;
                ColBarSettings.PawnTextureCameraVerticalOffset = 0.3f;
                ColBarSettings.PawnTextureCameraZoom = 1.28205f;
            }

            GUILayout.EndVertical();

            #endregion

            #region Gender

            // if (ColBarSettings.UseGender)
            // {
            // if (Button("CBKF.Settings.FemaleColor".Translate()))
            // {
            // while (Find.WindowStack.TryRemove(typeof(Dialog_ColorPicker)))
            // {
            // }
            // Find.WindowStack.Add(new Dialog_ColorPicker(colourWrapper, delegate
            // {
            // ColBarSettings.FemaleColor = colourWrapper.Color;
            // }, false, true)
            // {
            // initialPosition = new Vector2(windowRect.xMax + 10f, windowRect.yMin),
            // });
            // }
            // if (Button("CBKF.Settings.MaleColor".Translate()))
            // {
            // while (Find.WindowStack.TryRemove(typeof(Dialog_ColorPicker)))
            // {
            // }
            // Find.WindowStack.Add(new Dialog_ColorPicker(colourWrapper, delegate
            // {
            // ColBarSettings.MaleColor = colourWrapper.Color;
            // }, false, true)
            // {
            // initialPosition = new Vector2(windowRect.xMax + 10f, windowRect.yMin),
            // });
            // }
            // if (Button("CBKF.Settings.ResetColors".Translate()))
            // {
            // ColBarSettings.FemaleColor = new Color(1f, 0.64f, 0.8f, 1f);
            // ColBarSettings.MaleColor = new Color(0.52f, 0.75f, 0.92f, 1f);
            // }
            // }
            #endregion
        }

        private void FillPageMain()
        {

            #region Horizontal alignment

            GUILayout.BeginVertical(_fondBoxes);
            ColBarSettings.UseCustomMarginTopHor = GUILayout.Toggle(
                ColBarSettings.UseCustomMarginTopHor,
                "CBKF.Settings.ColonistBarOffset".Translate() + (int)ColBarSettings.MarginTop + " yMin, "
                + (int)ColBarSettings.MarginLeft + " xMin, " + (int)ColBarSettings.MarginRight + " xMax");

            if (ColBarSettings.UseCustomMarginTopHor)
            {
                GUILayout.Space(Text.LineHeight / 2);
                ColBarSettings.MarginTop = GUILayout.HorizontalSlider(ColBarSettings.MarginTop, 0f, Screen.height / 6);
                ColBarSettings.MarginLeft = GUILayout.HorizontalSlider(
                    ColBarSettings.MarginLeft,
                    0f,
                    Screen.width * 2 / 5);
                ColBarSettings.MarginRight = GUILayout.HorizontalSlider(
                    ColBarSettings.MarginRight,
                    0f,
                    Screen.width * 2 / 5);
            }
            else
            {
                ColBarSettings.MarginTop = 21f;
                ColBarSettings.MarginLeft = 160f;
                ColBarSettings.MarginRight = 160f;
                ColBarSettings.MaxColonistBarWidth = Screen.width - ColBarSettings.MarginLeft
                                                     - ColBarSettings.MarginRight;
                ColBarSettings.HorizontalOffset = ColBarSettings.MarginLeft / 2
                                                  - ColBarSettings.MarginRight / 2;
            }

            // listing.Gap(3f);
            GUILayout.EndVertical();

            // listing.Gap(3f);


#if !NoCCL
                listing.Undent();
#endif

            #endregion



            #region Max Rows

            GUILayout.BeginVertical(_fondBoxes);
            ColBarSettings.UseCustomRowCount = GUILayout.Toggle(
                ColBarSettings.UseCustomRowCount,
                "PSI.Settings.Arrangement.ColonistsPerColumn".Translate()
                + (ColBarSettings.UseCustomRowCount ? ColBarSettings.MaxRowsCustom : 3));
            if (ColBarSettings.UseCustomRowCount)
            {
                ColBarSettings.MaxRowsCustom = (int) GUILayout.HorizontalSlider(ColBarSettings.MaxRowsCustom, 1f, 5f);
            }

            GUILayout.EndVertical();

            #endregion

            #region Various

            GUILayout.BeginVertical(_fondBoxes);

            ColBarSettings.UseWeaponIcons = GUILayout.Toggle(
                ColBarSettings.UseWeaponIcons,
                "CBKF.Settings.UseWeaponIcons".Translate());

            ColBarSettings.UseGender = GUILayout.Toggle(ColBarSettings.UseGender, "CBKF.Settings.useGender".Translate());

            ColBarSettings.useZoomToMouse = GUILayout.Toggle(
                ColBarSettings.useZoomToMouse,
                "CBKF.Settings.useZoomToMouse".Translate());

            GUILayout.Label("FollowMe.MiddleClick".Translate());

          //#region DoubleClickTime
          //
          //ColBarSettings.UseCustomDoubleClickTime = GUILayout.Toggle(
          //    ColBarSettings.UseCustomDoubleClickTime,
          //    "CBKF.Settings.DoubleClickTime".Translate() + ": " + ColBarSettings.DoubleClickTime.ToString("N2")
          //    + " s");
          //if (ColBarSettings.UseCustomDoubleClickTime)
          //{
          //    // listing.Gap(3f);
          //    GUILayout.Space(Text.LineHeight / 2);
          //    ColBarSettings.DoubleClickTime = GUILayout.HorizontalSlider(ColBarSettings.DoubleClickTime, 0.1f, 1.5f);
          //}
          //else
          //{
          //    ColBarSettings.DoubleClickTime = 0.5f;
          //}
          //
          //#endregion


            #region Mood Bar

            GUILayout.BeginVertical(_fondBoxes);
            ColBarSettings.UseNewMood = GUILayout.Toggle(
                ColBarSettings.UseNewMood,
                "CBKF.Settings.UseNewMood".Translate());

            if (ColBarSettings.UseNewMood || ColBarSettings.UseExternalMoodBar)
            {
                ColBarSettings.UseExternalMoodBar = GUILayout.Toggle(
                ColBarSettings.UseExternalMoodBar,
                "CBKF.Settings.UseExternalMoodBar".Translate());

                if (ColBarSettings.UseExternalMoodBar)
                {
                    GUILayout.BeginHorizontal();
                    MoodBarPositionInt = GUILayout.Toolbar(MoodBarPositionInt, positionStrings);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndVertical();

            #endregion

            GUILayout.EndVertical();

            #endregion
        }

        private void FillPagePSIIconSet(Rect viewRect)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("PSI.Settings.IconSet".Translate() + PsiSettings.IconSet))
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>();

                options.Add(
                    new FloatMenuOption(
                        "PSI.Settings.Preset.0".Translate(),
                        () =>
                            {
                                try
                                {
                                    PsiSettings.IconSet = "default";
                                    PsiSettings.UseColoredTarget = true;
                                    SavePsiSettings();
                                }
                                catch (IOException)
                                {
                                    Log.Error("PSI.Settings.LoadPreset.UnableToLoad".Translate() + "default");
                                }
                            }));
                options.Add(
                    new FloatMenuOption(
                        "PSI.Settings.Preset.1".Translate(),
                        () =>
                            {
                                try
                                {
                                    PsiSettings.IconSet = "original";
                                    PsiSettings.UseColoredTarget = false;
                                    SavePsiSettings();
                                }
                                catch (IOException)
                                {
                                    Log.Error("PSI.Settings.LoadPreset.UnableToLoad".Translate() + "default");
                                }
                            }));

                Find.WindowStack.Add(new FloatMenu(options));
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            _iconLimit = Mathf.FloorToInt(viewRect.width / 125f);

            GUILayout.Space(Text.LineHeight / 2);

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            int num = 0;
            GUILayout.BeginHorizontal();

            DrawCheckboxAreaTarget(
                "PSI.Settings.Visibility.TargetPoint".Translate(),
                PSIMaterials[Icons.Target],
                PSIMaterials[Icons.TargetHair],
                PSIMaterials[Icons.TargetSkin],
                ref num);

            DrawCheckboxArea(
                "PSI.Settings.Visibility.Draft".Translate(),
                PSIMaterials[Icons.Draft],
                ref ColBarSettings.ShowDraft,
                ref PsiSettings.ShowDraft,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Unarmed".Translate(),
                PSIMaterials[Icons.Unarmed],
                ref ColBarSettings.ShowUnarmed,
                ref PsiSettings.ShowUnarmed,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Idle".Translate(),
                PSIMaterials[Icons.Idle],
                ref ColBarSettings.ShowIdle,
                ref PsiSettings.ShowIdle,
                ref num);

            DrawCheckboxArea(
                "PSI.Settings.Visibility.Sad".Translate(),
                PSIMaterials[Icons.Sad],
                ref ColBarSettings.ShowSad,
                ref PsiSettings.ShowSad,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Aggressive".Translate(),
                PSIMaterials[Icons.Aggressive],
                ref ColBarSettings.ShowAggressive,
                ref PsiSettings.ShowAggressive,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Panic".Translate(),
                PSIMaterials[Icons.Panic],
                ref ColBarSettings.ShowPanic,
                ref PsiSettings.ShowPanic,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Dazed".Translate(),
                PSIMaterials[Icons.Dazed],
                ref ColBarSettings.ShowDazed,
                ref PsiSettings.ShowDazed,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Leave".Translate(),
                PSIMaterials[Icons.Leave],
                ref ColBarSettings.ShowLeave,
                ref PsiSettings.ShowLeave,
                ref num);

            DrawCheckboxArea(
                "PSI.Settings.Visibility.Hungry".Translate(),
                PSIMaterials[Icons.Hungry],
                ref ColBarSettings.ShowHungry,
                ref PsiSettings.ShowHungry,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Tired".Translate(),
                PSIMaterials[Icons.Tired],
                ref ColBarSettings.ShowTired,
                ref PsiSettings.ShowTired,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.TooCold".Translate(),
                PSIMaterials[Icons.TooCold],
                ref ColBarSettings.ShowTooCold,
                ref PsiSettings.ShowTooCold,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.TooHot".Translate(),
                PSIMaterials[Icons.TooHot],
                ref ColBarSettings.ShowTooHot,
                ref PsiSettings.ShowTooHot,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.ApparelHealth".Translate(),
                PSIMaterials[Icons.ApparelHealth],
                ref ColBarSettings.ShowApparelHealth,
                ref PsiSettings.ShowApparelHealth,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Naked".Translate(),
                PSIMaterials[Icons.Naked],
                ref ColBarSettings.ShowNaked,
                ref PsiSettings.ShowNaked,
                ref num);

            DrawCheckboxArea(
                "PSI.Settings.Visibility.Health".Translate(),
                PSIMaterials[Icons.Health],
                ref ColBarSettings.ShowHealth,
                ref PsiSettings.ShowHealth,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.MedicalAttention".Translate(),
                PSIMaterials[Icons.MedicalAttention],
                ref ColBarSettings.ShowMedicalAttention,
                ref PsiSettings.ShowMedicalAttention,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Injury".Translate(),
                PSIMaterials[Icons.Effectiveness],
                ref ColBarSettings.ShowEffectiveness,
                ref PsiSettings.ShowEffectiveness,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Bloodloss".Translate(),
                PSIMaterials[Icons.Bloodloss],
                ref ColBarSettings.ShowBloodloss,
                ref PsiSettings.ShowBloodloss,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Sickness".Translate(),
                PSIMaterials[Icons.Sickness],
                ref ColBarSettings.ShowMedicalAttention,
                ref PsiSettings.ShowMedicalAttention,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Pain".Translate(),
                PSIMaterials[Icons.Pain],
                ref ColBarSettings.ShowPain,
                ref PsiSettings.ShowPain,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Drunk".Translate(),
                PSIMaterials[Icons.Drunk],
                ref ColBarSettings.ShowDrunk,
                ref PsiSettings.ShowDrunk,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Toxicity".Translate(),
                PSIMaterials[Icons.Toxicity],
                ref ColBarSettings.ShowToxicity,
                ref PsiSettings.ShowToxicity,
                ref num);

            DrawCheckboxArea(
                "PSI.Settings.Visibility.NightOwl".Translate(),
                PSIMaterials[Icons.NightOwl],
                ref ColBarSettings.ShowNightOwl,
                ref PsiSettings.ShowNightOwl,
                ref num);

            DrawCheckboxArea(
                "PSI.Settings.Visibility.LeftUnburied".Translate(),
                PSIMaterials[Icons.LeftUnburied],
                ref ColBarSettings.ShowLeftUnburied,
                ref PsiSettings.ShowLeftUnburied,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.CabinFever".Translate(),
                PSIMaterials[Icons.CabinFever],
                ref ColBarSettings.ShowCabinFever,
                ref PsiSettings.ShowCabinFever,
                ref num);

            DrawCheckboxArea(
                "PSI.Settings.Visibility.Bedroom".Translate(),
                PSIMaterials[Icons.Bedroom],
                ref ColBarSettings.ShowBedroom,
                ref PsiSettings.ShowBedroom,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Greedy".Translate(),
                PSIMaterials[Icons.Greedy],
                ref ColBarSettings.ShowGreedy,
                ref PsiSettings.ShowGreedy,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.DeadColonists".Translate(),
                PSIMaterials[Icons.DeadColonist],
                ref ColBarSettings.ShowDeadColonists,
                ref PsiSettings.ShowDeadColonists,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Jealous".Translate(),
                PSIMaterials[Icons.Jealous],
                ref ColBarSettings.ShowJealous,
                ref PsiSettings.ShowJealous,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Pyromaniac".Translate(),
                PSIMaterials[Icons.Pyromaniac],
                ref ColBarSettings.ShowPyromaniac,
                ref PsiSettings.ShowPyromaniac,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Prosthophile".Translate(),
                PSIMaterials[Icons.Prosthophile],
                ref ColBarSettings.ShowProsthophile,
                ref PsiSettings.ShowProsthophile,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Prosthophobe".Translate(),
                PSIMaterials[Icons.Prosthophobe],
                ref ColBarSettings.ShowProsthophobe,
                ref PsiSettings.ShowProsthophobe,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Pacific".Translate(),
                PSIMaterials[Icons.Pacific],
                ref ColBarSettings.ShowPacific,
                ref PsiSettings.ShowPacific,
                ref num);

            DrawCheckboxArea(
                "PSI.Settings.Visibility.Lovers".Translate(),
                PSIMaterials[Icons.Love],
                ref ColBarSettings.ShowLove,
                ref PsiSettings.ShowLove,
                ref num);

            // DrawCheckboxArea("PSI.Settings.Visibility.Marriage".Translate(), PSIMaterials[Icons.Marriage], ref ColBarSettings.ShowMarriage, ref PsiSettings.ShowMarriage, ref num);
            GUILayout.EndHorizontal();

            // PsiSettings.ShowIdle = Toggle(PsiSettings.ShowIdle, "PSI.Settings.Visibility.Idle".Translate());
            // PsiSettings.ShowUnarmed = Toggle(PsiSettings.ShowUnarmed, "PSI.Settings.Visibility.Unarmed".Translate());
            // PsiSettings.ShowHungry = Toggle(PsiSettings.ShowHungry, "PSI.Settings.Visibility.Hungry".Translate());
            // PsiSettings.ShowSad = Toggle(PsiSettings.ShowSad, "PSI.Settings.Visibility.Sad".Translate());
            // PsiSettings.ShowTired = Toggle(PsiSettings.ShowTired, "PSI.Settings.Visibility.Tired".Translate());
            // //
            // PsiSettings.ShowMedicalAttention = Toggle(PsiSettings.ShowMedicalAttention, "PSI.Settings.Visibility.Sickness".Translate());
            // PsiSettings.ShowPain = Toggle(PsiSettings.ShowPain, "PSI.Settings.Visibility.Pain".Translate());
            // PsiSettings.ShowHealth = Toggle(PsiSettings.ShowHealth, "PSI.Settings.Visibility.Health".Translate());
            // PsiSettings.ShowEffectiveness = Toggle(PsiSettings.ShowEffectiveness, "PSI.Settings.Visibility.Injury".Translate());
            // PsiSettings.ShowBloodloss = Toggle(PsiSettings.ShowBloodloss, "PSI.Settings.Visibility.Bloodloss".Translate());
            // //
            // PsiSettings.ShowTooHot = Toggle(PsiSettings.ShowTooHot, "PSI.Settings.Visibility.TooHot".Translate());
            // PsiSettings.ShowTooCold = Toggle(PsiSettings.ShowTooCold, "PSI.Settings.Visibility.TooCold".Translate());
            // PsiSettings.ShowNaked = Toggle(PsiSettings.ShowNaked, "PSI.Settings.Visibility.Naked".Translate());
            // PsiSettings.ShowDrunk = Toggle(PsiSettings.ShowDrunk, "PSI.Settings.Visibility.Drunk".Translate());
            // PsiSettings.ShowApparelHealth = Toggle(PsiSettings.ShowApparelHealth, "PSI.Settings.Visibility.ApparelHealth".Translate());
            // //
            // PsiSettings.ShowPacific = Toggle(PsiSettings.ShowPacific, "PSI.Settings.Visibility.Pacific".Translate());
            // PsiSettings.ShowNightOwl = Toggle(PsiSettings.ShowNightOwl, "PSI.Settings.Visibility.NightOwl".Translate());
            // PsiSettings.ShowGreedy = Toggle(PsiSettings.ShowGreedy, "PSI.Settings.Visibility.Greedy".Translate());
            // PsiSettings.ShowJealous = Toggle(PsiSettings.ShowJealous, "PSI.Settings.Visibility.Jealous".Translate());
            // PsiSettings.ShowLove = Toggle(PsiSettings.ShowLove, "PSI.Settings.Visibility.Lovers".Translate());
            // //
            // PsiSettings.ShowProsthophile = Toggle(PsiSettings.ShowProsthophile, "PSI.Settings.Visibility.Prosthophile".Translate());
            // PsiSettings.ShowProsthophobe = Toggle(PsiSettings.ShowProsthophobe, "PSI.Settings.Visibility.Prosthophobe".Translate());
            // PsiSettings.ShowCabinFever = Toggle(PsiSettings.ShowCabinFever, "PSI.Settings.Visibility.CabinFever".Translate());
            // PsiSettings.ShowBedroom = Toggle(PsiSettings.ShowBedroom, "PSI.Settings.Visibility.Bedroom".Translate());
            // PsiSettings.ShowDeadColonists = Toggle(PsiSettings.ShowDeadColonists, "PSI.Settings.Visibility.ShowDeadColonists".Translate());
            // PsiSettings.ShowPyromaniac = Toggle(PsiSettings.ShowPyromaniac, "PSI.Settings.Visibility.Pyromaniac".Translate());
            GUILayout.Space(Text.LineHeight / 2);
            GUILayout.EndScrollView();
        }

        private void FillPagePSIOpacityAndColor()
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            GUILayout.BeginVertical(_fondBoxes);
            GUILayout.Label(
                "PSI.Settings.IconOpacityAndColor.Opacity".Translate() + (PsiSettings.IconOpacity * 100).ToString("N0")
                + " %");
            PsiSettings.IconOpacity = GUILayout.HorizontalSlider(PsiSettings.IconOpacity, 0.1f, 1f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(_fondBoxes);
            GUILayout.Label(
                "PSI.Settings.IconOpacityAndColor.OpacityCritical".Translate()
                + (PsiSettings.IconOpacityCritical * 100).ToString("N0") + " %");
            PsiSettings.IconOpacityCritical = GUILayout.HorizontalSlider(PsiSettings.IconOpacityCritical, 0.1f, 1f);
            GUILayout.EndVertical();

            // if (listing.DoTextButton("PSI.Settings.ResetColors".Translate()))
            // {
            // colorRedAlert = baseSettings.ColorRedAlert;
            // Scribe_Values.LookValue(ref colorRedAlert, "colorRedAlert");
            // colorInput.Value = colorRedAlert;
            // PSI.SaveSettings();
            // }
            // Rect row = new Rect(0f, listing.CurHeight, listing.ColumnWidth(), 24f);
            // DrawMCMRegion(row);
            // PSI.Settings.ColorRedAlert = colorInput.Value;
            // listing.DoGap();
            // listing.DoGap();
            GUILayout.EndScrollView();

            // if (listing.DoTextButton("PSI.Settings.ReturnButton".Translate()))
            // Page = "main";
        }

        private void FillPSIPageSensitivity()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("PSI.Settings.LoadPresetButton".Translate()))
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>
                                                    {
                                                        new FloatMenuOption(
                                                            "Less Sensitive",
                                                            () =>
                                                                {
                                                                    try
                                                                    {
                                                                        PsiSettings.LimitBleedMult = 2f;
                                                                        PsiSettings.LimitDiseaseLess = 1f;
                                                                        PsiSettings.LimitEfficiencyLess = 0.28f;
                                                                        PsiSettings.LimitFoodLess = 0.2f;

                                                                        // PsiSettings.LimitMoodLess = 0.2f;
                                                                        PsiSettings.LimitRestLess = 0.2f;
                                                                        PsiSettings.LimitApparelHealthLess = 0.5f;
                                                                        PsiSettings.LimitTempComfortOffset = 3f;
                                                                    }
                                                                    catch (IOException)
                                                                    {
                                                                        Log.Error(
                                                                            "PSI.Settings.LoadPreset.UnableToLoad"
                                                                                .Translate()
                                                                            + "Less Sensitive");
                                                                    }
                                                                }),
                                                        new FloatMenuOption(
                                                            "Standard",
                                                            () =>
                                                                {
                                                                    try
                                                                    {
                                                                        PsiSettings.LimitBleedMult = 3f;
                                                                        PsiSettings.LimitDiseaseLess = 1f;
                                                                        PsiSettings.LimitEfficiencyLess = 0.33f;
                                                                        PsiSettings.LimitFoodLess = 0.25f;

                                                                        // PsiSettings.LimitMoodLess = 0.25f;
                                                                        PsiSettings.LimitRestLess = 0.25f;
                                                                        PsiSettings.LimitApparelHealthLess = 0.5f;
                                                                        PsiSettings.LimitTempComfortOffset = 0f;
                                                                    }
                                                                    catch (IOException)
                                                                    {
                                                                        Log.Error(
                                                                            "PSI.Settings.LoadPreset.UnableToLoad".Translate
                                                                                () + "Standard");
                                                                    }
                                                                }),
                                                        new FloatMenuOption(
                                                            "More Sensitive",
                                                            () =>
                                                                {
                                                                    try
                                                                    {
                                                                        PsiSettings.LimitBleedMult = 4f;
                                                                        PsiSettings.LimitDiseaseLess = 1f;
                                                                        PsiSettings.LimitEfficiencyLess = 0.45f;
                                                                        PsiSettings.LimitFoodLess = 0.3f;

                                                                        // PsiSettings.LimitMoodLess = 0.3f;
                                                                        PsiSettings.LimitRestLess = 0.3f;
                                                                        PsiSettings.LimitApparelHealthLess = 0.5f;
                                                                        PsiSettings.LimitTempComfortOffset = -3f;
                                                                    }
                                                                    catch (IOException)
                                                                    {
                                                                        Log.Error(
                                                                            "PSI.Settings.LoadPreset.UnableToLoad".Translate
                                                                                () + "More Sensitive");
                                                                    }
                                                                })
                                                    };

                Find.WindowStack.Add(new FloatMenu(options));
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            GUILayout.BeginVertical(_fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Bleeding".Translate()
                + ("PSI.Settings.Sensitivity.Bleeding." + Math.Round(PsiSettings.LimitBleedMult - 0.25)).Translate());
            PsiSettings.LimitBleedMult = GUILayout.HorizontalSlider(PsiSettings.LimitBleedMult, 0.5f, 5f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(_fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Injured".Translate() + (int)(PsiSettings.LimitEfficiencyLess * 100.0) + " %");
            PsiSettings.LimitEfficiencyLess = GUILayout.HorizontalSlider(PsiSettings.LimitEfficiencyLess, 0.01f, 0.99f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(_fondBoxes);
            GUILayout.Label("PSI.Settings.Sensitivity.Food".Translate() + (int)(PsiSettings.LimitFoodLess * 100.0) + " %");
            PsiSettings.LimitFoodLess = GUILayout.HorizontalSlider(PsiSettings.LimitFoodLess, 0.01f, 0.99f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(_fondBoxes);
            GUILayout.Label("PSI.Settings.Sensitivity.Rest".Translate() + (int)(PsiSettings.LimitRestLess * 100.0) + " %");
            PsiSettings.LimitRestLess = GUILayout.HorizontalSlider(PsiSettings.LimitRestLess, 0.01f, 0.99f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(_fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.ApparelHealth".Translate() + (int)(PsiSettings.LimitApparelHealthLess * 100.0)
                + " %");
            PsiSettings.LimitApparelHealthLess = GUILayout.HorizontalSlider(PsiSettings.LimitApparelHealthLess, 0.01f, 0.99f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(_fondBoxes);
            GUILayout.Label("PSI.Settings.Sensitivity.Temperature".Translate() + (int)PsiSettings.LimitTempComfortOffset + " °C");
            PsiSettings.LimitTempComfortOffset = GUILayout.HorizontalSlider(PsiSettings.LimitTempComfortOffset, -10f, 10f);
            GUILayout.EndVertical();

            GUILayout.EndScrollView();

            // if (!listing.DoTextButton("PSI.Settings.ReturnButton".Translate()))
            // return;
            // Page = "main";
        }

        private void FillPSIPageSizeArrangement()
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);



            #region PSI on Bar

            GUILayout.BeginVertical(_fondBoxes);
            ColBarSettings.UsePsi = GUILayout.Toggle(ColBarSettings.UsePsi, "CBKF.Settings.UsePsiOnBar".Translate());
            if (ColBarSettings.UsePsi)
            {
                GUILayout.BeginHorizontal();
                PsiBarPositionInt = GUILayout.Toolbar(PsiBarPositionInt, positionStrings);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.Label("PSI.Settings.Arrangement.IconsPerColumn".Translate() + ColBarSettings.IconsInColumn);
                ColBarSettings.IconsInColumn = (int) GUILayout.HorizontalSlider(ColBarSettings.IconsInColumn, 2f, 5f);
            }
            GUILayout.EndVertical();

            #endregion

            GUILayout.BeginVertical(_fondBoxes);
            PsiSettings.UsePsi = GUILayout.Toggle(PsiSettings.UsePsi, "PSI.Settings.UsePSI".Translate());
            PsiSettings.UsePsiOnPrisoner = GUILayout.Toggle(
                PsiSettings.UsePsiOnPrisoner,
                "PSI.Settings.UsePSIOnPrisoner".Translate());

            if (PsiSettings.UsePsi || PsiSettings.UsePsiOnPrisoner)
            {
                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.BeginHorizontal();
                PsiPositionInt = GUILayout.Toolbar(PsiPositionInt, positionStrings);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(Text.LineHeight / 2);

                PsiSettings.IconsHorizontal = GUILayout.Toggle(
                    PsiSettings.IconsHorizontal,
                    "PSI.Settings.Arrangement.Horizontal".Translate());

                PsiSettings.IconsScreenScale = GUILayout.Toggle(
                    PsiSettings.IconsScreenScale,
                    "PSI.Settings.Arrangement.ScreenScale".Translate());

                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.Label("PSI.Settings.Arrangement.IconsPerColumn".Translate() + PsiSettings.IconsInColumn);
                PsiSettings.IconsInColumn = (int) GUILayout.HorizontalSlider(PsiSettings.IconsInColumn, 1f, 7f);

                int num = (int)(PsiSettings.IconSize * 4.5);

                if (num > 8) num = 8;
                else if (num < 0) num = 0;

                GUILayout.Space(Text.LineHeight / 2);
                GUILayout.Label("PSI.Settings.Arrangement.IconSize".Translate() + ("PSI.Settings.SizeLabel." + num).Translate());
                PsiSettings.IconSize = GUILayout.HorizontalSlider(PsiSettings.IconSize, 0.5f, 2f);
                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.BeginVertical(_fondBoxes);
                GUILayout.Label(
                    "PSI.Settings.Arrangement.IconPosition".Translate() + (int)(PsiSettings.IconDistanceX * 100.0)
                    + " x, " + (int)(PsiSettings.IconDistanceY * 100.0) + " y");
                PsiSettings.IconDistanceX = GUILayout.HorizontalSlider(PsiSettings.IconDistanceX, -2f, 2f);
                PsiSettings.IconDistanceY = GUILayout.HorizontalSlider(PsiSettings.IconDistanceY, -2f, 2f);
                GUILayout.EndVertical();

                GUILayout.BeginVertical(_fondBoxes);
                GUILayout.Label(
                    "PSI.Settings.Arrangement.IconOffset".Translate() + (int)(PsiSettings.IconOffsetX * 100.0) + " x, "
                    + (int)(PsiSettings.IconOffsetY * 100.0) + " y");
                PsiSettings.IconOffsetX = GUILayout.HorizontalSlider(PsiSettings.IconOffsetX, -2f, 2f);
                PsiSettings.IconOffsetY = GUILayout.HorizontalSlider(PsiSettings.IconOffsetY, -2f, 2f);
                GUILayout.EndVertical();

                // if (!listing.DoTextButton("PSI.Settings.ReturnButton".Translate()))
                // return;
                // Page = "main";
            }

            GUILayout.EndVertical();


            GUILayout.EndScrollView();
        }

        private void LabelHeadline(string labelstring)
        {
            GUILayout.Space(Text.LineHeight / 2);
            GUILayout.Label(string.Empty, _grayLines, GUILayout.Height(1));
            GUILayout.Space(Text.LineHeight / 4);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(labelstring, _fontBold);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(Text.LineHeight / 2);
            GUILayout.Label(string.Empty, _grayLines, GUILayout.Height(1));
            GUILayout.Space(Text.LineHeight / 2);
        }

        private void ResetBarSettings()
        {
            ColBarSettings = new SettingsColonistBar();
        }

        private void ResetPSISettings()
        {
            PsiSettings = new SettingsPSI();
        }

#if NoCCL
        public override void PreClose()
        {
            SaveBarSettings();
            SavePsiSettings();
        }

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(512f, 590f);
            }
        }

        private int mainToolbarInt;

        private int psiToolbarInt;

        private int barPositionInt = 0;

        private int psiBarPositionInt;

        private int moodBarPositionInt;

        private int psiPositionInt;

        private readonly string[] mainToolbarStrings =
            {
                "CBKF.Settings.ColonistBar".Translate(),
                "CBKF.Settings.PSI".Translate()
            };

        private readonly string[] psiToolbarStrings =
            {
                "PSI.Settings.ArrangementButton".Translate(),
                "PSI.Settings.OpacityButton".Translate(),
                "PSI.Settings.IconButton".Translate(),
                "PSI.Settings.SensitivityButton".Translate()
            };

        private readonly string[] positionStrings =
            {
                "CBKF.Settings.useLeft".Translate(),
                "CBKF.Settings.useRight".Translate(),
                "CBKF.Settings.useTop".Translate(),
                "CBKF.Settings.useBottom".Translate()
            };

        private int MainToolbarInt
        {
            get
            {
                return mainToolbarInt;
            }

            set
            {
                mainToolbarInt = value;
            }
        }

        private int PsiBarPositionInt
        {
            get
            {
                if (ColBarSettings.ColBarPsiIconPos == Position.Alignment.Left)
                {
                    psiBarPositionInt = 0;
                }

                if (ColBarSettings.ColBarPsiIconPos == Position.Alignment.Right)
                {
                    psiBarPositionInt = 1;
                }

                if (ColBarSettings.ColBarPsiIconPos == Position.Alignment.Top)
                {
                    psiBarPositionInt = 2;
                }

                if (ColBarSettings.ColBarPsiIconPos == Position.Alignment.Bottom)
                {
                    psiBarPositionInt = 3;
                }

                return psiBarPositionInt;
            }

            set
            {
                switch (value)
                {
                    case 0:
                        ColBarSettings.ColBarPsiIconPos = Position.Alignment.Left;
                        ColBarSettings.IconOffsetX = 1f;
                        ColBarSettings.IconOffsetY = 1f;
                        ColBarSettings.IconsHorizontal = false;
                        ColBarSettings.IconsScreenScale = true;
                        break;
                    case 1:
                        ColBarSettings.ColBarPsiIconPos = Position.Alignment.Right;
                        ColBarSettings.IconOffsetX = -1f;
                        ColBarSettings.IconOffsetY = 1f;
                        ColBarSettings.IconsHorizontal = false;
                        break;
                    case 2:
                        ColBarSettings.ColBarPsiIconPos = Position.Alignment.Top;
                        ColBarSettings.IconOffsetX = -1f;
                        ColBarSettings.IconOffsetY = 1f;
                        ColBarSettings.IconsHorizontal = true;
                        break;
                    case 3:
                        ColBarSettings.ColBarPsiIconPos = Position.Alignment.Bottom;
                        ColBarSettings.IconOffsetX = -1;
                        ColBarSettings.IconOffsetY = -1;
                        ColBarSettings.IconsHorizontal = true;
                        break;
                    default:
                        ColBarSettings.ColBarPsiIconPos = 0;

                        break;
                }

                psiBarPositionInt = value;
            }
        }

        private int MoodBarPositionInt
        {
            get
            {
                if (ColBarSettings.MoodBarPos == Position.Alignment.Left)
                {
                    moodBarPositionInt = 0;
                }

                if (ColBarSettings.MoodBarPos == Position.Alignment.Right)
                {
                    moodBarPositionInt = 1;
                }

                if (ColBarSettings.MoodBarPos == Position.Alignment.Top)
                {
                    moodBarPositionInt = 2;
                }

                if (ColBarSettings.MoodBarPos == Position.Alignment.Bottom)
                {
                    moodBarPositionInt = 3;
                }

                return moodBarPositionInt;
            }

            set
            {
                switch (value)
                {
                    case 0:
                        ColBarSettings.MoodBarPos = Position.Alignment.Left;
                        break;
                    case 1:
                        ColBarSettings.MoodBarPos = Position.Alignment.Right;
                        break;
                    case 2:
                        ColBarSettings.MoodBarPos = Position.Alignment.Top;
                        break;
                    case 3:
                        ColBarSettings.MoodBarPos = Position.Alignment.Bottom;
                        break;
                    default:
                        ColBarSettings.MoodBarPos = Position.Alignment.Right;
                        break;
                }

                moodBarPositionInt = value;
            }
        }

        private int PsiPositionInt
        {
            get
            {
                if (PsiSettings.IconAlignment == 0)
                {
                    psiPositionInt = 0;
                }

                if (PsiSettings.IconAlignment == 1)
                {
                    psiPositionInt = 1;
                }

                if (PsiSettings.IconAlignment == 2)
                {
                    psiPositionInt = 2;
                }

                if (PsiSettings.IconAlignment == 3)
                {
                    psiPositionInt = 3;
                }

                return psiPositionInt;
            }

            set
            {
                if (value == psiPositionInt) return;
                switch (value)
                {
                    case 0:
                        PsiSettings.IconAlignment = value;
                        PsiSettings.IconDistanceX = 1f;
                        PsiSettings.IconDistanceY = 1f;
                        PsiSettings.IconOffsetX = 1f;
                        PsiSettings.IconOffsetY = 1f;
                        PsiSettings.IconsHorizontal = false;
                        PsiSettings.IconsScreenScale = true;
                        PsiSettings.IconsInColumn = 3;
                        PsiSettings.IconSize = 1f;
                        PsiSettings.IconOpacity = 0.5f;
                        PsiSettings.IconOpacityCritical = 0.8f;
                        break;
                    case 1:
                        PsiSettings.IconAlignment = value;
                        PsiSettings.IconDistanceX = -1f;
                        PsiSettings.IconDistanceY = 1f;
                        PsiSettings.IconOffsetX = -1f;
                        PsiSettings.IconOffsetY = 1f;
                        PsiSettings.IconsHorizontal = false;
                        PsiSettings.IconsScreenScale = true;
                        PsiSettings.IconsInColumn = 3;
                        PsiSettings.IconSize = 1f;
                        PsiSettings.IconOpacity = 0.5f;
                        PsiSettings.IconOpacityCritical = 0.8f;
                        break;
                    case 2:
                        PsiSettings.IconAlignment = value;
                        PsiSettings.IconDistanceX = 1f;
                        PsiSettings.IconDistanceY = -1.63f;
                        PsiSettings.IconOffsetX = -1f;
                        PsiSettings.IconOffsetY = 1f;
                        PsiSettings.IconsHorizontal = true;
                        PsiSettings.IconsScreenScale = true;
                        PsiSettings.IconsInColumn = 3;
                        PsiSettings.IconSize = 1f;
                        PsiSettings.IconOpacity = 0.5f;
                        PsiSettings.IconOpacityCritical = 0.8f;
                        break;
                    case 3:
                        PsiSettings.IconAlignment = value;
                        PsiSettings.IconDistanceX = 1.139534f;
                        PsiSettings.IconDistanceY = 1.375f;
                        PsiSettings.IconOffsetX = -0.9534883f;
                        PsiSettings.IconOffsetY = -0.9534884f;
                        PsiSettings.IconsHorizontal = true;
                        PsiSettings.IconsScreenScale = true;
                        PsiSettings.IconsInColumn = 4;
                        PsiSettings.IconSize = 1.084302f;
                        PsiSettings.IconOpacity = 0.5f;
                        PsiSettings.IconOpacityCritical = 0.8f;
                        break;
                    default:
                        PsiSettings.IconAlignment = 0;

                        break;
                }

                psiPositionInt = value;
            }
        }

        private int PSIToolbarInt
        {
            get
            {
                return psiToolbarInt;
            }

            set
            {
                psiToolbarInt = value;
            }
        }

        readonly GUIStyle _fontBold = new GUIStyle
        {
            fontStyle = FontStyle.Bold,
            normal = {
                                                          textColor = Color.white
                                                       },
            padding = new RectOffset(0, 0, 5, 0)
        };

        readonly GUIStyle _headline = new GUIStyle
        {
            fontStyle = FontStyle.Bold,
            fontSize = 16,
            normal = {
                                                          textColor = Color.white
                                                       },
            padding = new RectOffset(0, 0, 12, 6)
        };

        readonly GUIStyle _fondBoxes = new GUIStyle
        {
            normal = {
                                                           background = ColonistBarTextures.DarkGrayFond
                                                        },
            hover = {
                                                          background = ColonistBarTextures.GrayFond
                                                       },
            padding = new RectOffset(15, 15, 6, 10),
            margin = new RectOffset(0, 0, 10, 10)
        };

        readonly GUIStyle _fondImages = new GUIStyle
        {
            normal = {
                                                            background = ColonistBarTextures.DarkGrayFond
                                                         },
            hover = {
                                                           background = ColonistBarTextures.RedHover
                                                        }
        };

        readonly GUIStyle _darkGrayBgImage = new GUIStyle { normal = { background = ColonistBarTextures.GrayFond } };

        readonly GUIStyle _grayLines = new GUIStyle { normal = { background = ColonistBarTextures.GrayLines } };

        private Vector2 _scrollPosition;

        public override void DoWindowContents(Rect rect)
#else
        public override float DoWindowContents(Rect rect)
#endif
        {
            Rect viewRect = new Rect(rect);
            viewRect.x += 15f;
            viewRect.width -= 30f;
            viewRect.height -= 15f;

            GUILayout.BeginArea(viewRect);
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Colonist Bar KF 0.16.0", _headline);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(Text.LineHeight / 2);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            MainToolbarInt = GUILayout.Toolbar(MainToolbarInt, mainToolbarStrings);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            switch (MainToolbarInt)
            {
                case 0:
                    {
                        GUILayout.Space(Text.LineHeight);

                        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

                        LabelHeadline("CBKF.Settings.BarPosition".Translate());

                        GUILayout.BeginVertical();

                        FillPageMain();

                        GUILayout.EndVertical();

                        LabelHeadline("CBKF.Settings.Advanced".Translate());

                        GUILayout.BeginVertical();
                        FillPageAdvanced();
                        GUILayout.EndVertical();

                        GUILayout.EndScrollView();
                    }

                    break;
                case 1:
                    {
                        // LabelHeadline("PSI.Settings".Translate());
                        GUILayout.Space(Text.LineHeight);

                        int toolbarInt = Mathf.FloorToInt(viewRect.width / 150f);
                        if (toolbarInt == 0)
                        {
                            toolbarInt += 1;
                        }

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        PSIToolbarInt = GUILayout.SelectionGrid(
                            PSIToolbarInt,
                            psiToolbarStrings,
                            toolbarInt > psiToolbarStrings.Length ? psiToolbarStrings.Length : toolbarInt);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        GUILayout.Space(Text.LineHeight / 2);

                        if (PSIToolbarInt == 0)
                        {
                            {
                                FillPSIPageSizeArrangement();
                            }
                        }
                        else if (PSIToolbarInt == 1)
                        {
                            {
                                FillPagePSIOpacityAndColor();
                            }
                        }
                        else if (PSIToolbarInt == 2)
                        {
                            {
                                FillPagePSIIconSet(viewRect);
                            }
                        }
                        else if (PSIToolbarInt == 3)
                        {
                            {
                                FillPSIPageSensitivity();
                            }
                        }

                        // else if (PSIToolbarInt == 4)
                        // {
                        // {
                        // FillPagePSILoadIconset();
                        // }
                        // }
                        else
                        {
                            FillPagePSIIconSet(viewRect);
                        }
                    }

                    break;
            }

            GUILayout.FlexibleSpace();
            GUILayout.Space(Text.LineHeight / 2);
            GUILayout.Label(string.Empty, _grayLines, GUILayout.Height(1));
            GUILayout.Space(Text.LineHeight / 2);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("CBKF.Settings.RevertSettings".Translate()))
            {
                ResetBarSettings();
            }

            GUILayout.Space(Text.LineHeight / 2);
            if (GUILayout.Button("PSI.Settings.RevertSettings".Translate()))
            {
                ResetPSISettings();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();

            if (GUI.changed)
                ColonistBar_KF.MarkColonistsDirty();

#if !NoCCL
            return 1000f;
#endif
        }
    }
}