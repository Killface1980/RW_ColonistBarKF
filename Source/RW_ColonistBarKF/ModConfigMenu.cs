#if !NoCCL
using CommunityCoreLibrary;
using CommunityCoreLibrary.UI;
# endif
using System;
using System.Collections.Generic;
using System.IO;
using ColonistBarKF.PSI;
using UnityEngine;
using Verse;
using static ColonistBarKF.CBKF;
using static ColonistBarKF.PSI.PSI;
using static ColonistBarKF.SettingsColonistBar.SortByWhat;
using static UnityEngine.GUILayout;

namespace ColonistBarKF
{
#if !NoCCL
    public class ModConfigMenu : ModConfigurationMenu
#else
    public class ColonistBarKF_Settings : Window
#endif
    {
        #region Fields

        public Window OptionsDialog;

        #endregion

        #region Methods

        public static int lastupdate = -5000;

        public override void WindowUpdate()
        {
            Reinit(false, true);
            if (Find.TickManager.TicksGame > lastupdate)
            {
                if (ColBarSettings.UseGender)
                    ColonistBarTextures.BGTex = ColonistBarTextures.BGTexGrey;
                else
                {
                    ColonistBarTextures.BGTex = ColonistBarTextures.BGTexVanilla;
                }
                lastupdate = Find.TickManager.TicksGame + 1500;
            }
        }


#if NoCCL
        public override void PreClose()
        {
            SaveBarSettings();
            SavePsiSettings();
        }



        public override Vector2 InitialSize => new Vector2(438f, 690f);

        private int mainToolbarInt = 0;
        private int psiToolbarInt = 0;
        private int barPositionInt = 0;
        private int psiBarPositionInt = 0;
        private int psiPositionInt = 0;

        public string[] mainToolbarStrings =
            {
            "ColonistBarKF.SettingsColonistBar".Translate(),
            "ColonistBarKF.SettingsPSI".Translate()
        };
        public string[] psiToolbarStrings =
        {
            "PSI.Settings.VisibilityButton".Translate(),
            "PSI.Settings.OpacityButton".Translate(),
            "PSI.Settings.ArrangementButton".Translate(),
            "PSI.Settings.SensitivityButton".Translate(),
            "PSI.Settings.IconSet".Translate()
        };

        public string[] psiPositionStrings =
        {
            "ColonistBarKF.SettingsColonistBar.useTop".Translate(),
            "ColonistBarKF.SettingsColonistBar.useBottom".Translate(),
            "ColonistBarKF.SettingsColonistBar.useLeft".Translate(),
            "ColonistBarKF.SettingsColonistBar.useRight".Translate()
        };

        private string[] psiColBarStrings =
{
            "ColonistBarKF.SettingsColonistBar.useLeft".Translate(),
            "ColonistBarKF.SettingsColonistBar.useRight".Translate(),
            "ColonistBarKF.SettingsColonistBar.useTop".Translate(),
            "ColonistBarKF.SettingsColonistBar.useBottom".Translate()
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

        private int BarPositionInt
        {
            get
            {
                if (!ColBarSettings.UseBottomAlignment && !ColBarSettings.UseVerticalAlignment && !ColBarSettings.UseRightAlignment)
                {
                    barPositionInt = 0;
                }
                if (ColBarSettings.UseBottomAlignment && !ColBarSettings.UseVerticalAlignment && !ColBarSettings.UseRightAlignment)
                {
                    barPositionInt = 1;
                }
                if (!ColBarSettings.UseBottomAlignment && ColBarSettings.UseVerticalAlignment && !ColBarSettings.UseRightAlignment)
                {
                    barPositionInt = 2;
                }
                if (!ColBarSettings.UseBottomAlignment && ColBarSettings.UseVerticalAlignment && ColBarSettings.UseRightAlignment)
                {
                    barPositionInt = 3;
                }
                return barPositionInt;
            }

            set
            {
                switch (value)
                {
                    case 0:
                        ColBarSettings.UseBottomAlignment = false;
                        ColBarSettings.UseVerticalAlignment = false;
                        ColBarSettings.UseRightAlignment = false;
                        break;
                    case 1:
                        ColBarSettings.UseBottomAlignment = true;
                        ColBarSettings.UseVerticalAlignment = false;
                        ColBarSettings.UseRightAlignment = false; break;
                    case 2:
                        ColBarSettings.UseBottomAlignment = false;
                        ColBarSettings.UseVerticalAlignment = true;
                        ColBarSettings.UseRightAlignment = false; break;
                    case 3:
                        ColBarSettings.UseBottomAlignment = false;
                        ColBarSettings.UseVerticalAlignment = true;
                        ColBarSettings.UseRightAlignment = true; break;
                    default:
                        ColBarSettings.UseBottomAlignment = false;
                        ColBarSettings.UseVerticalAlignment = false;
                        ColBarSettings.UseRightAlignment = false;
                        break;
                }

                barPositionInt = value;
            }
        }

        private int PsiBarPositionInt
        {
            get
            {

                if (ColBarSettings.IconAlignment == 0)
                {
                    psiBarPositionInt = 0;
                }
                if (ColBarSettings.IconAlignment == 1)
                {
                    psiBarPositionInt = 1;
                }
                if (ColBarSettings.IconAlignment == 2)
                {
                    psiBarPositionInt = 2;
                }
                if (ColBarSettings.IconAlignment == 3)
                {
                    psiBarPositionInt = 3;
                }
                return psiBarPositionInt;
            }

            set
            {
                if (value == psiBarPositionInt)
                    return;
                switch (value)
                {
                    case 0:
                        ColBarSettings.IconAlignment = value;
                        ColBarSettings.IconDistanceX = 1f;
                        ColBarSettings.IconDistanceY = 1f;
                        ColBarSettings.IconOffsetX = 1f;
                        ColBarSettings.IconOffsetY = 1f;
                        ColBarSettings.IconsHorizontal = false;
                        ColBarSettings.IconsScreenScale = true;
                        ColBarSettings.IconsInColumn = 4;
                        ColBarSettings.IconSize = 1f;
                        ColBarSettings.IconOpacity = 0.7f;
                        ColBarSettings.IconOpacityCritical = 0.6f;
                        break;
                    case 1:
                        ColBarSettings.IconAlignment = value;
                        ColBarSettings.IconDistanceX = -1f;
                        ColBarSettings.IconDistanceY = 1f;
                        ColBarSettings.IconOffsetX = -1f;
                        ColBarSettings.IconOffsetY = 1f;
                        ColBarSettings.IconsHorizontal = false;
                        ColBarSettings.IconsScreenScale = true;
                        ColBarSettings.IconsInColumn = 4;
                        ColBarSettings.IconSize = 1f;
                        ColBarSettings.IconOpacity = 0.7f;
                        ColBarSettings.IconOpacityCritical = 0.6f;
                        break;
                    case 2:
                        ColBarSettings.IconAlignment = value;
                        ColBarSettings.IconDistanceX = 1f;
                        ColBarSettings.IconDistanceY = -1;
                        ColBarSettings.IconOffsetX = -1f;
                        ColBarSettings.IconOffsetY = 1f;
                        ColBarSettings.IconsHorizontal = true;
                        ColBarSettings.IconsScreenScale = true;
                        ColBarSettings.IconsInColumn = 4;
                        ColBarSettings.IconSize = 1f;
                        ColBarSettings.IconOpacity = 0.7f;
                        ColBarSettings.IconOpacityCritical = 0.6f;
                        break;
                    case 3:
                        ColBarSettings.IconAlignment = value;
                        ColBarSettings.IconDistanceX = 1;
                        ColBarSettings.IconDistanceY = 1;
                        ColBarSettings.IconOffsetX = -1;
                        ColBarSettings.IconOffsetY = -1;
                        ColBarSettings.IconsHorizontal = true;
                        ColBarSettings.IconsScreenScale = true;
                        ColBarSettings.IconsInColumn = 4;
                        ColBarSettings.IconSize = 1f;
                        ColBarSettings.IconOpacity = 0.7f;
                        ColBarSettings.IconOpacityCritical = 0.6f;
                        break;
                    default:
                        ColBarSettings.IconAlignment = 0;

                        break;
                }
                psiBarPositionInt = value;
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
                if (value == psiPositionInt)
                    return;
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
                        PsiSettings.IconOpacity = 0.7f;
                        PsiSettings.IconOpacityCritical = 0.6f;
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
                        PsiSettings.IconOpacity = 0.7f;
                        PsiSettings.IconOpacityCritical = 0.6f;
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
                        PsiSettings.IconOpacity = 0.7f;
                        PsiSettings.IconOpacityCritical = 0.6f;
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
                        PsiSettings.IconOpacity = 0.7f;
                        PsiSettings.IconOpacityCritical = 0.6f;
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
                LoadBarSettings();
                LoadPsiSettings();
                return psiToolbarInt;
            }

            set
            {
                SaveBarSettings();
                SavePsiSettings();
                psiToolbarInt = value;
            }
        }

        GUIStyle FontBold = new GUIStyle
        {
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white },
            padding = new RectOffset(0, 0, 12, 6)
        };

        GUIStyle Headline = new GUIStyle
        {
            fontStyle = FontStyle.Bold,
            fontSize = 16,
            normal = { textColor = Color.white },
            padding = new RectOffset(0, 0, 12, 6),
        };

        GUIStyle hoverBox = new GUIStyle
        {
            hover = { background = ColonistBarTextures.HoverBG }
        };

        GUIStyle DarkGrayBG = new GUIStyle
        {
            normal = { background = ColonistBarTextures.PureDarkGray },
        };

        GUIStyle GrayBG = new GUIStyle
        {
            normal = { background = ColonistBarTextures.HoverBG },
        };

        public ColonistBarKF_Settings()
        {
            forcePause = true;
            doCloseX = true;
            draggable = true;
            drawShadow = true;
            preventCameraMotion = false;
            resizeable = true;
        }

        private Vector2 _scrollPosition;


        public override void DoWindowContents(Rect rect)
#else
        public override float DoWindowContents(Rect rect)
#endif
        {
            var viewRect = new Rect(rect);
            viewRect.width -= 15f;
            viewRect.height -= 50f;

            BeginVertical(Width(viewRect.width), Height(viewRect.height));

            BeginHorizontal();
            FlexibleSpace();
            Label("Colonist Bar KF 0.15.3", Headline);
            FlexibleSpace();
            EndHorizontal();

            Space(Text.LineHeight);
            MainToolbarInt = Toolbar(MainToolbarInt, mainToolbarStrings, Width(viewRect.width));
            Space(6f);

            switch (MainToolbarInt)
            {
                case 0:
                    {
                        BeginHorizontal();
                        FlexibleSpace();
                        Label("ColonistBarKF.SettingsColonistBar.BarPosition".Translate(), FontBold);
                        FlexibleSpace();
                        EndHorizontal();

                        Space(6f);
                        BeginVertical(DarkGrayBG);
                        BarPositionInt = Toolbar(BarPositionInt, psiPositionStrings, Width(viewRect.width));
                        FillPagePosition(viewRect.width);
                        EndVertical();
                        Space(6f);
                        Label("", DarkGrayBG, Height(1));
                        Space(6f);
                        BeginHorizontal();
                        FlexibleSpace();
                        Label("ColonistBarKF.SettingsColonistBar.PsiBarPosition".Translate(), FontBold);
                        FlexibleSpace();
                        EndHorizontal();

                        Space(6f);
                        Label("", DarkGrayBG, Height(1));
                        Space(6f);
                        _scrollPosition = BeginScrollView(_scrollPosition, Width(viewRect.width));
                        BeginVertical(DarkGrayBG);
                        FillPageOptions(viewRect.width);
            Space(6f);
                        EndScrollView();
                        EndVertical();
                    }
                    break;
                case 1:
                    {
                        BeginHorizontal();
                        FlexibleSpace();
                        Label("PSI.Settings".Translate(), FontBold);
                        FlexibleSpace();
                        EndHorizontal();
                        PSIToolbarInt = SelectionGrid(PSIToolbarInt, psiToolbarStrings, 2);
                        Space(12);

                        if (PSIToolbarInt == 0)
                        {
                            {
                                FillPagePSIShowHide();
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
                                FillPSIPageArrangement();
                            }
                        }
                        else if (PSIToolbarInt == 3)
                        {
                            {
                                FillPSIPageLimits();
                            }
                        }
                        else if (PSIToolbarInt == 4)
                        {
                            {
                                FillPagePSILoadIconset();
                            }
                        }
                        else
                        {
                            FillPagePSIShowHide();
                        }

                    }
                    break;
            }
            Space(12);

            FlexibleSpace();
            Space(6f);
            Label("", DarkGrayBG, Height(1));
            Space(6f);
            BeginHorizontal();
            if (Button("ColonistBarKF.SettingsColonistBar.RevertSettings".Translate()))
            {
                ResetBarSettings();
            }
            Space(12f);
            if (Button("ColonistBarKF.SettingsColonistBar.RevertPSISettings".Translate()))
            {
                ResetPSISettings();
            }
            EndHorizontal();
            EndVertical();

#if !NoCCL
            return 1000f;
#endif
        }

        private void ResetBarSettings()
        {
            // forces the mod to update the values from the sliders. is deactivated by saving the values.
            //SettingsColonistBar.Reloadsettings = true;

            {
                ColBarSettings.UseGender = false;
                ColBarSettings.UseCustomMarginTopHor = false;
                ColBarSettings.UseCustomBaseSpacingHorizontal = false;
                ColBarSettings.UseCustomBaseSpacingVertical = false;
                ColBarSettings.UseCustomIconSize = false;
                ColBarSettings.UseCustomPawnTextureCameraHorizontalOffset = false;
                ColBarSettings.UseCustomPawnTextureCameraVerticalOffset = false;
                ColBarSettings.UseCustomPawnTextureCameraZoom = false;
                ColBarSettings.UseCustomMarginLeftHorTop = false;
                ColBarSettings.UseCustomMarginRightHorTop = false;
                ColBarSettings.UseCustomMarginLeftHorBottom = false;
                ColBarSettings.UseCustomMarginRightHorBottom = false;
                ColBarSettings.UseBottomAlignment = false;
                ColBarSettings.UseMoodColors = false;
                ColBarSettings.UseWeaponIcons = false;
                ColBarSettings.UseFixedIconScale = false;

                ColBarSettings.MarginBottomHor = 21f;
                ColBarSettings.BaseSpacingHorizontal = 24f;
                ColBarSettings.BaseSpacingVertical = 32f;
                ColBarSettings.BaseSizeFloat = 48f;
                ColBarSettings.PawnTextureCameraHorizontalOffset = 0f;
                ColBarSettings.PawnTextureCameraVerticalOffset = 0.3f;
                ColonistBar_KF.PawnTextureCameraOffset = new Vector3(0f, 0f, 0.3f);
                ColBarSettings.PawnTextureCameraZoom = 1.28205f;
                ColBarSettings.MaxColonistBarWidth = Screen.width - 320f;
                ColBarSettings.FemaleColor = new Color(1f, 0.64f, 0.8f, 1f);
                ColBarSettings.MaleColor = new Color(0.52f, 0.75f, 0.92f, 1f);
#if !NoCCL
                femaleColorField.Value = new Color(1f, 0.64f, 0.8f, 1f);
                maleColorField.Value = new Color(0.52f, 0.75f, 0.92f, 1f);
#endif
                ColBarSettings.HorizontalOffset = 0f;
                ColBarSettings.VerticalOffset = 0f;
                ColBarSettings.UseCustomBaseSpacingVertical = false;
                ColBarSettings.UseVerticalAlignment = false;
                ColBarSettings.BaseSpacingVertical = 32f;
                ColBarSettings.MaxColonistBarHeight = Screen.height - 240f;
                ColBarSettings.UseRightAlignment = false;
                ColBarSettings.MarginLeftHorTop = 180f;
                ColBarSettings.MarginRightHorTop = 180f;
                ColBarSettings.MarginLeftHorBottom = 180f;
                ColBarSettings.MarginRightHorBottom = 180f;
                ColBarSettings.UseCustomDoubleClickTime = false;
                ColBarSettings.DoubleClickTime = 0.5f;
                ColBarSettings.UseCustomMarginLeftVer = false;
                ColBarSettings.MarginLeftVer = 21f;
                ColBarSettings.UseCustomMarginTopVerLeft = false;
                ColBarSettings.MarginTopVerLeft = 120f;
                ColBarSettings.UseCustomMarginBottomVerLeft = false;
                ColBarSettings.MarginBottomVerLeft = 120f;

                ColBarSettings.UseCustomMarginTopHor = false;
                ColBarSettings.UseCustomMarginBottomHor = false;
                ColBarSettings.UseCustomMarginLeftHorTop = false;
                ColBarSettings.UseCustomMarginRightHorTop = false;

                ColBarSettings.UseCustomMarginTopVerLeft = false;
                ColBarSettings.UseCustomMarginTopVerRight = false;
                ColBarSettings.UseCustomMarginLeftVer = false;
                ColBarSettings.UseCustomMarginRightVer = false;
                ColBarSettings.UseCustomMarginBottomVerLeft = false;
                ColBarSettings.UseCustomMarginBottomVerRight = false;
                ColBarSettings.SortBy = vanilla;
                ColBarSettings.useZoomToMouse = false;
                ColBarSettings.moodRectScale = 0.33f;
            }
        }

        private void ResetPSISettings()
        {
            PsiSettings.UsePsi = true;
            PsiSettings.IconSize = 1f;
            PsiSettings.IconSizeMult = 1f;
            PsiSettings.IconDistanceX = 1f;
            PsiSettings.IconDistanceY = 1f;
            PsiSettings.IconOffsetX = 1f;
            PsiSettings.IconOffsetY = 1f;

            PsiSettings.IconsInColumn = 3;
            PsiSettings.IconsHorizontal = false;
            PsiSettings.IconsScreenScale = true;
            PsiSettings.IconSet = "default";

            PsiSettings.ShowTargetPoint = true;
            PsiSettings.ShowAggressive = true;
            PsiSettings.ShowDazed = true;
            PsiSettings.ShowLeave = true;
            PsiSettings.ShowDraft = true;
            PsiSettings.ShowIdle = true;
            PsiSettings.ShowUnarmed = true;
            PsiSettings.ShowHungry = true;
            PsiSettings.ShowSad = true;
            PsiSettings.ShowTired = true;
            PsiSettings.ShowDisease = true;
            PsiSettings.ShowEffectiveness = true;
            PsiSettings.ShowBloodloss = true;
            PsiSettings.ShowHot = true;
            PsiSettings.ShowCold = true;
            PsiSettings.ShowNaked = true;
            PsiSettings.ShowDrunk = true;
            PsiSettings.ShowApparelHealth = true;
            PsiSettings.ShowPacific = true;
            PsiSettings.ShowProsthophile = true;
            PsiSettings.ShowProsthophobe = true;
            PsiSettings.ShowNightOwl = true;
            PsiSettings.ShowGreedy = true;
            PsiSettings.ShowJealous = true;
            PsiSettings.ShowLovers = true;
            PsiSettings.ShowDeadColonists = true;
            PsiSettings.ShowLeftUnburied = true;
            PsiSettings.ShowRoomStatus = true;
            PsiSettings.ShowPain = true;
            PsiSettings.ShowBedroom = true;
            PsiSettings.ShowHealth = true;
            PsiSettings.ShowPyromaniac = true;

      //      PsiSettings.LimitMoodLess = 0.25f;
            PsiSettings.LimitFoodLess = 0.25f;
            PsiSettings.LimitRestLess = 0.25f;
            PsiSettings.LimitEfficiencyLess = 0.33f;
            PsiSettings.LimitDiseaseLess = 1f;
            PsiSettings.LimitBleedMult = 3f;
            PsiSettings.LimitApparelHealthLess = 0.5f;
            PsiSettings.LimitTempComfortOffset = 0f;
            PsiSettings.IconOpacity = 0.7f;
            PsiSettings.IconOpacityCritical = 0.6f;
            PsiSettings.UseColoredTarget = true;

        }

        private void FillPagePosition(float width)
        {
            float colSize = width / 2 - 10f;
            #region Vertical Alignment

            if (ColBarSettings.UseVerticalAlignment)
            {
#if !NoCCL
                listing.Indent();
#endif
                if (ColBarSettings.UseRightAlignment)
                {
                    BeginHorizontal(hoverBox);
                    ColBarSettings.UseCustomMarginRightVer = Toggle(ColBarSettings.UseCustomMarginRightVer, "ColonistBarKF.SettingsColonistBar.MarginEdge".Translate(), Width(colSize));
                    if (ColBarSettings.UseCustomMarginRightVer)
                    {
                        //    listing.Gap(3f);
                        ColBarSettings.MarginRightVer = HorizontalSlider(ColBarSettings.MarginRightVer, 0f, Screen.width / 12, Width(colSize));
                    }
                    else
                    {
                        ColBarSettings.MarginRightVer = 21f;
                    }
                    // listing.Gap(3f);
                    EndHorizontal();
                    BeginHorizontal(hoverBox);
                    ColBarSettings.UseCustomMarginTopVerRight = Toggle(ColBarSettings.UseCustomMarginTopVerRight, "ColonistBarKF.SettingsColonistBar.MarginTop".Translate(), Width(colSize));
                    if (ColBarSettings.UseCustomMarginTopVerRight)
                    {
                        ColBarSettings.MarginTopVerRight = HorizontalSlider(ColBarSettings.MarginTopVerRight, 0f, Screen.height * 2 / 5, Width(colSize));
                    }
                    else
                    {
                        ColBarSettings.MarginTopVerRight = 120f;
                        ColBarSettings.MaxColonistBarHeight = Screen.height - ColBarSettings.MarginTopVerRight - ColBarSettings.MarginBottomVerRight;
                        ColBarSettings.VerticalOffset = ColBarSettings.MarginTopVerRight / 2 - ColBarSettings.MarginBottomVerRight / 2;

                    }
                    EndHorizontal();
                    //  listing.Gap(3f);
                    BeginHorizontal(hoverBox);
                    ColBarSettings.UseCustomMarginBottomVerRight = Toggle(ColBarSettings.UseCustomMarginBottomVerRight, "ColonistBarKF.SettingsColonistBar.MarginBottom".Translate(), Width(colSize));
                    if (ColBarSettings.UseCustomMarginBottomVerRight)
                    {
                        //     listing.Gap(3f);
                        ColBarSettings.MarginBottomVerRight = HorizontalSlider(ColBarSettings.MarginBottomVerRight, 0f, Screen.height * 2 / 5, Width(colSize));
                    }
                    else
                    {
                        ColBarSettings.MarginBottomVerRight = 120f;
                        ColBarSettings.MaxColonistBarHeight = Screen.height - ColBarSettings.MarginTopVerRight - ColBarSettings.MarginBottomVerRight;
                        ColBarSettings.VerticalOffset = ColBarSettings.MarginTopVerRight / 2 - ColBarSettings.MarginBottomVerRight / 2;
                    }
                    EndHorizontal();
                }
                else
                {
                    BeginHorizontal(hoverBox);
                    ColBarSettings.UseCustomMarginLeftVer = Toggle(ColBarSettings.UseCustomMarginLeftVer, "ColonistBarKF.SettingsColonistBar.MarginEdge".Translate(), Width(colSize));
                    if (ColBarSettings.UseCustomMarginLeftVer)
                    {
                        //     listing.Gap(3f);
                        ColBarSettings.MarginLeftVer = HorizontalSlider(ColBarSettings.MarginLeftVer, 0f, Screen.width / 12, Width(colSize));
                    }
                    else
                    {
                        ColBarSettings.MarginLeftVer = 21f;
                    }
                    //   listing.Gap(3f);
                    EndHorizontal();
                    BeginHorizontal(hoverBox);
                    ColBarSettings.UseCustomMarginTopVerLeft = Toggle(ColBarSettings.UseCustomMarginTopVerLeft, "ColonistBarKF.SettingsColonistBar.MarginTop".Translate(), Width(colSize));
                    if (ColBarSettings.UseCustomMarginTopVerLeft)
                    {
                        //    listing.Gap(3f);
                        ColBarSettings.MarginTopVerLeft = HorizontalSlider(ColBarSettings.MarginTopVerLeft, 0f, Screen.height * 2 / 5, Width(colSize));
                    }
                    else
                    {
                        ColBarSettings.MarginTopVerLeft = 120f;
                        ColBarSettings.MaxColonistBarHeight = Screen.height - ColBarSettings.MarginTopVerLeft - ColBarSettings.MarginBottomVerLeft;
                        ColBarSettings.VerticalOffset = ColBarSettings.MarginTopVerLeft / 2 - ColBarSettings.MarginBottomVerLeft / 2;

                    }
                    EndHorizontal();

                    //   listing.Gap(3f);
                    BeginHorizontal(hoverBox);
                    ColBarSettings.UseCustomMarginBottomVerLeft = Toggle(ColBarSettings.UseCustomMarginBottomVerLeft, "ColonistBarKF.SettingsColonistBar.MarginBottom".Translate(), Width(colSize));
                    if (ColBarSettings.UseCustomMarginBottomVerLeft)
                    {
                        //    listing.Gap(3f);
                        ColBarSettings.MarginBottomVerLeft = HorizontalSlider(ColBarSettings.MarginBottomVerLeft, 0f, Screen.height * 2 / 5, Width(colSize));
                    }
                    else
                    {
                        ColBarSettings.MarginBottomVerLeft = 120f;
                        ColBarSettings.MaxColonistBarHeight = Screen.height - ColBarSettings.MarginTopVerLeft - ColBarSettings.MarginBottomVerLeft;
                        ColBarSettings.VerticalOffset = ColBarSettings.MarginTopVerLeft / 2 - ColBarSettings.MarginBottomVerLeft / 2;
                    }
                    EndHorizontal();
                }



                //  listing.Gap(3f);
#if !NoCCL
                listing.Undent();
#endif
            }
            #endregion

            #region Horizontal alignment
            else
            {
#if !NoCCL
                listing.Indent();
#endif

                if (ColBarSettings.UseBottomAlignment)
                {
                    BeginHorizontal(hoverBox);
                    ColBarSettings.UseCustomMarginBottomHor = Toggle(ColBarSettings.UseCustomMarginBottomHor, "ColonistBarKF.SettingsColonistBar.MarginEdge".Translate(), Width(colSize));
                    if (ColBarSettings.UseCustomMarginBottomHor)
                    {
                        //    listing.Gap(3f);
                        ColBarSettings.MarginBottomHor = ColBarSettings.MarginBottomHor = HorizontalSlider(ColBarSettings.MarginBottomHor, 10, Screen.height / 12, Width(colSize));
                    }
                    else
                    {
                        ColBarSettings.MarginBottomHor = 21f;
                    }
                    //   listing.Gap(3f);
                    EndHorizontal();
                    BeginHorizontal(hoverBox);
                    ColBarSettings.UseCustomMarginLeftHorBottom = Toggle(ColBarSettings.UseCustomMarginLeftHorBottom, "ColonistBarKF.SettingsColonistBar.MarginLeft".Translate(), Width(colSize));
                    if (ColBarSettings.UseCustomMarginLeftHorBottom)
                    {
                        //   listing.Gap(3f);
                        ColBarSettings.MarginLeftHorBottom = HorizontalSlider(ColBarSettings.MarginLeftHorBottom, 0f, Screen.width * 2 / 5, Width(colSize));
                    }
                    else
                    {
                        ColBarSettings.MarginLeftHorBottom = 160f;
                        ColBarSettings.MaxColonistBarWidth = Screen.width - ColBarSettings.MarginLeftHorBottom - ColBarSettings.MarginRightHorBottom;
                        ColBarSettings.HorizontalOffset = ColBarSettings.MarginLeftHorBottom / 2 - ColBarSettings.MarginRightHorBottom / 2;
                    }
                    //  listing.Gap(3f);
                    EndHorizontal();
                    BeginHorizontal(hoverBox);
                    ColBarSettings.UseCustomMarginRightHorBottom = Toggle(ColBarSettings.UseCustomMarginRightHorBottom, "ColonistBarKF.SettingsColonistBar.MarginRight".Translate(), Width(colSize));
                    if (ColBarSettings.UseCustomMarginRightHorBottom)
                    {
                        //      listing.Gap(3f);
                        ColBarSettings.MarginRightHorBottom = HorizontalSlider(ColBarSettings.MarginRightHorBottom, 0f, Screen.width * 2 / 5, Width(colSize));
                    }
                    else
                    {
                        ColBarSettings.MarginRightHorBottom = 160f;
                        ColBarSettings.MaxColonistBarWidth = Screen.width - ColBarSettings.MarginLeftHorBottom - ColBarSettings.MarginRightHorBottom;
                        ColBarSettings.HorizontalOffset = ColBarSettings.MarginLeftHorBottom / 2 - ColBarSettings.MarginRightHorBottom / 2;
                    }
                    EndHorizontal();
                    //    listing.Gap(3f);
                }
                else
                {
                    BeginHorizontal(hoverBox);
                    ColBarSettings.UseCustomMarginTopHor = Toggle(ColBarSettings.UseCustomMarginTopHor, "ColonistBarKF.SettingsColonistBar.MarginEdge".Translate(), Width(colSize));
                    if (ColBarSettings.UseCustomMarginTopHor)
                    {
                        //    listing.Gap(3f);
                        ColBarSettings.MarginTopHor = HorizontalSlider(ColBarSettings.MarginTopHor, 10, Screen.height / 12, Width(colSize));
                    }
                    else
                    {
                        ColBarSettings.MarginTopHor = 21f;
                    }
                    //  listing.Gap(3f);
                    EndHorizontal();
                    BeginHorizontal(hoverBox);
                    ColBarSettings.UseCustomMarginLeftHorTop = Toggle(ColBarSettings.UseCustomMarginLeftHorTop, "ColonistBarKF.SettingsColonistBar.MarginLeft".Translate(), Width(colSize));
                    if (ColBarSettings.UseCustomMarginLeftHorTop)
                    {
                        //    listing.Gap(3f);
                        ColBarSettings.MarginLeftHorTop = HorizontalSlider(ColBarSettings.MarginLeftHorTop, 0f, Screen.width * 2 / 5, Width(colSize));
                    }
                    else
                    {
                        ColBarSettings.MarginLeftHorTop = 160f;
                        ColBarSettings.MaxColonistBarWidth = Screen.width - ColBarSettings.MarginLeftHorTop - ColBarSettings.MarginRightHorTop;
                        ColBarSettings.HorizontalOffset = ColBarSettings.MarginLeftHorTop / 2 - ColBarSettings.MarginRightHorTop / 2;
                    }
                    EndHorizontal();
                    BeginHorizontal(hoverBox);
                    //  listing.Gap(3f);

                    ColBarSettings.UseCustomMarginRightHorTop = Toggle(ColBarSettings.UseCustomMarginRightHorTop, "ColonistBarKF.SettingsColonistBar.MarginRight".Translate(), Width(colSize));
                    if (ColBarSettings.UseCustomMarginRightHorTop)
                    {
                        //     listing.Gap(3f);
                        ColBarSettings.MarginRightHorTop = HorizontalSlider(ColBarSettings.MarginRightHorTop, 0f, Screen.width * 2 / 5, Width(colSize));
                    }
                    else
                    {
                        ColBarSettings.MarginRightHorTop = 160f;
                        ColBarSettings.MaxColonistBarWidth = Screen.width - ColBarSettings.MarginLeftHorTop - ColBarSettings.MarginRightHorTop;
                        ColBarSettings.HorizontalOffset = ColBarSettings.MarginLeftHorTop / 2 - ColBarSettings.MarginRightHorTop / 2;
                    }
                    EndHorizontal();
                    //  listing.Gap(3f);
                }
#if !NoCCL
                listing.Undent();
#endif
            }
            #endregion

            Space(12f);
        }

        private void FillPageOptions(float width)
        {
            float colSize = (width - 15f) / 2 - 10f;
            #region Size + Spacing
            BeginHorizontal(hoverBox);
            ColBarSettings.UseCustomIconSize = Toggle(ColBarSettings.UseCustomIconSize, "ColonistBarKF.SettingsColonistBar.BasicSize".Translate(), Width(colSize));

            if (ColBarSettings.UseCustomIconSize)
            {
                //     listing.Gap(3f);
                //    listing.Gap(3f);
                ColBarSettings.BaseSizeFloat = HorizontalSlider(ColBarSettings.BaseSizeFloat, 16f, 128f, Width(colSize));

                //     listing.Gap();
            }
            else
            {
                ColBarSettings.BaseSizeFloat = 48f;
                ColBarSettings.BaseIconSize = 20f;
                //     listing.Gap(3f);
            }
            EndHorizontal();
            ColBarSettings.UseFixedIconScale = Toggle(ColBarSettings.UseFixedIconScale, "ColonistBarKF.SettingsColonistBar.FixedScale".Translate(), Width(colSize));

            BeginHorizontal(hoverBox);
            ColBarSettings.UseCustomBaseSpacingHorizontal = Toggle(ColBarSettings.UseCustomBaseSpacingHorizontal, "ColonistBarKF.SettingsColonistBar.BaseSpacingHorizontal".Translate(), Width(colSize));
            if (ColBarSettings.UseCustomBaseSpacingHorizontal)
            {
                //      listing.Gap(3f);
                ColBarSettings.BaseSpacingHorizontal = HorizontalSlider(ColBarSettings.BaseSpacingHorizontal, 1f, 72f, Width(colSize));
            }
            else
            {
                ColBarSettings.BaseSpacingHorizontal = 24f;
                //      listing.Gap(3f);
            }
            EndHorizontal();
            BeginHorizontal(hoverBox);
            ColBarSettings.UseCustomBaseSpacingVertical = Toggle(ColBarSettings.UseCustomBaseSpacingVertical, "ColonistBarKF.SettingsColonistBar.BaseSpacingVertical".Translate(), Width(colSize));
            if (ColBarSettings.UseCustomBaseSpacingVertical)
            {
                //      listing.Gap(3f);
                ColBarSettings.BaseSpacingVertical = HorizontalSlider(ColBarSettings.BaseSpacingVertical, 1f, 96f, Width(colSize));
            }
            else
            {
                ColBarSettings.BaseSpacingVertical = 32f;
            }
            EndHorizontal();
            #endregion

            Space(12f);
            BeginHorizontal(hoverBox);
            ColBarSettings.UseMoodColors = Toggle(ColBarSettings.UseMoodColors, "ColonistBarKF.SettingsColonistBar.UseMoodColors".Translate(), Width(colSize));
            if (ColBarSettings.UseMoodColors)
            {
                //      listing.Gap(3f);
                ColBarSettings.moodRectScale = HorizontalSlider(ColBarSettings.moodRectScale, 0.33f, 1f, Width(colSize));
            }
            EndHorizontal();
            Space(12f);
            ColBarSettings.UseWeaponIcons = Toggle(ColBarSettings.UseWeaponIcons, "ColonistBarKF.SettingsColonistBar.UseWeaponIcons".Translate());

            Space(12f);
            ColBarSettings.UseGender = Toggle(ColBarSettings.UseGender, "ColonistBarKF.SettingsColonistBar.useGender".Translate());
            Space(12f);
            ColBarSettings.useZoomToMouse = Toggle(ColBarSettings.useZoomToMouse, "ColonistBarKF.SettingsColonistBar.useZoomToMouse".Translate());

            #region Gender



#if !NoCCL
            if (barSettings.UseGender)
            {
            //    listing.Gap(3f);
                float indent = 24f;
                DrawMCMRegion(new Rect(indent, listing.CurHeight, listing.ColumnWidth - indent, 64f));
                listing.Gap(72f);
                listing.ColumnWidth = columnwidth / 2;
                listing.Indent();
                if (GUILayout.Button("ColonistBarKF.SettingsColonistBar.ResetColors".Translate()))
                {
                    femaleColorField.Value = new Color(1f, 0.64f, 0.8f, 1f);
                    maleColorField.Value = new Color(0.52f, 0.75f, 0.92f, 1f);
                }
                listing.Undent();
                listing.ColumnWidth = columnwidth;
                listing.Gap();
            }
#endif
            #endregion


            #region Camera
            Space(12f);
            BeginHorizontal(hoverBox);
            ColBarSettings.UseCustomPawnTextureCameraZoom = Toggle(ColBarSettings.UseCustomPawnTextureCameraZoom, "ColonistBarKF.SettingsColonistBar.PawnTextureCameraZoom".Translate(), Width(colSize));
            if (ColBarSettings.UseCustomPawnTextureCameraZoom)
            {
                //    listing.Gap(3f);
                ColBarSettings.PawnTextureCameraZoom = HorizontalSlider(ColBarSettings.PawnTextureCameraZoom, 0.3f, 3f, Width(colSize));
            }
            else
            {
                ColBarSettings.PawnTextureCameraZoom = 1.28205f;
            }
            //    listing.Gap(3f);
            EndHorizontal();
            BeginHorizontal(hoverBox);
            ColBarSettings.UseCustomPawnTextureCameraHorizontalOffset = Toggle(ColBarSettings.UseCustomPawnTextureCameraHorizontalOffset, "ColonistBarKF.SettingsColonistBar.PawnTextureCameraHorizontalOffset".Translate(), Width(colSize));
            if (ColBarSettings.UseCustomPawnTextureCameraHorizontalOffset)
            {
                //        listing.Gap(3f);
                ColBarSettings.PawnTextureCameraHorizontalOffset = HorizontalSlider(ColBarSettings.PawnTextureCameraHorizontalOffset, 0.7f, -0.7f, Width(colSize));
            }
            else
            {
                ColBarSettings.PawnTextureCameraHorizontalOffset = 0f;
            }
            EndHorizontal();

            BeginHorizontal(hoverBox);
            ColBarSettings.UseCustomPawnTextureCameraVerticalOffset = Toggle(ColBarSettings.UseCustomPawnTextureCameraVerticalOffset, "ColonistBarKF.SettingsColonistBar.PawnTextureCameraVerticalOffset".Translate(), Width(colSize));
            if (ColBarSettings.UseCustomPawnTextureCameraVerticalOffset)
            {
                //       listing.Gap(3f);
                ColBarSettings.PawnTextureCameraVerticalOffset = HorizontalSlider(ColBarSettings.PawnTextureCameraVerticalOffset, 0f, 1f, Width(colSize));
            }
            else
            {
                ColBarSettings.PawnTextureCameraVerticalOffset = 0.3f;
            }
            EndHorizontal();
            #endregion


            Space(12f);
            BeginHorizontal(hoverBox);
            ColBarSettings.UseCustomDoubleClickTime = Toggle(ColBarSettings.UseCustomDoubleClickTime, "ColonistBarKF.SettingsColonistBar.DoubleClickTime".Translate() + ": " + ColBarSettings.DoubleClickTime.ToString("N2") + "s", Width(colSize));
            if (ColBarSettings.UseCustomDoubleClickTime)
            {
                //       listing.Gap(3f);
                ColBarSettings.DoubleClickTime = HorizontalSlider(ColBarSettings.DoubleClickTime, 0.1f, 1.5f, Width(colSize));
            }
            else
            {
                ColBarSettings.DoubleClickTime = 0.5f;
            }
            EndHorizontal();
            //       GUILayout.Toggle("ColonistBarKF.SettingsColonistBar.useExtraIcons".Translate(), ref ColBarSettings.useExtraIcons, null);
        }



#if !NoCCL
        private void DrawMCMRegion(Rect InRect)
        {
            Rect row = InRect;
            row.height = 24f;

            femaleColorField.Draw(row);
            barSettings.FemaleColor = femaleColorField.Value;

            row.y += 30f;

            maleColorField.Draw(row);
            barSettings.MaleColor = maleColorField.Value;
        }
#endif

#if !NoCCL
        private LabeledInput_Color femaleColorField = new LabeledInput_Color(barSettings.FemaleColor, "ColonistBarKF.SettingsColonistBar.FemaleColor".Translate());
        private LabeledInput_Color maleColorField = new LabeledInput_Color(barSettings.MaleColor, "ColonistBarKF.SettingsColonistBar.MaleColor".Translate());
#endif

        private void FillPagePSILoadIconset()
        {
            //  if (GUILayout.Button("PSI.SettingsColonistBar.IconSet".Translate() + PSI.ColBarSettings.IconSet))
            //   if (GUILayout.ButtonLabeled("PSI.SettingsColonistBar.IconSet".Translate() , PSI.ColBarSettings.IconSet))
            //   {
            //       var options = new List<FloatMenuOption>();
            //       foreach (var str in PSI.IconSets)
            //       {
            //           var setname = str;
            //           options.Add(new FloatMenuOption(setname, () =>
            //           {
            //               PSI.SettingsColonistBar.IconSet = setname;
            //               PSI.PSIMaterials = new PSIMaterials(setname);
            //               PSI.PSIMaterials.ReloadTextures(true);
            //           }));
            //       }
            //       Find.WindowStack.Add(new FloatMenu(options));
            //   }
            //   listing.NewColumn();

            //    if (GUILayout.ButtonLabeled("PSI.SettingsColonistBar.LoadPresetButton".Translate()))



            BeginHorizontal();
            FlexibleSpace();
            Label("PSI.SettingsColonistBar.LoadPresetButton".Translate(), FontBold);
            FlexibleSpace();
            EndHorizontal();

            _scrollPosition = BeginScrollView(_scrollPosition);


            if (Button("PSI.SettingsColonistBar.IconSet".Translate() + PsiSettings.IconSet))
            {
                var options = new List<FloatMenuOption>();

                options.Add(new FloatMenuOption("default_settings".Translate(), () =>
                {
                    try
                    {
                        PsiSettings.IconSet = "default";
                        SavePsiSettings();
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.SettingsColonistBar.LoadPreset.UnableToLoad".Translate() + "default_settings");
                    }
                }));
                options.Add(new FloatMenuOption("original_settings".Translate(), () =>
                {
                    try
                    {
                        PsiSettings.IconSet = "original";
                        SavePsiSettings();
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.SettingsColonistBar.LoadPreset.UnableToLoad".Translate() + "original_settings");
                    }
                }));
                options.Add(new FloatMenuOption("text_settings".Translate(), () =>
                {
                    try
                    {
                        PsiSettings.IconSet = "text";
                        SavePsiSettings();
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.SettingsColonistBar.LoadPreset.UnableToLoad".Translate() + "text_settings");
                    }
                }));
                Find.WindowStack.Add(new FloatMenu(options));
            }
            Space(6f);
            EndScrollView();
        }

        private void FillPagePSIShowHide()
        {
            BeginHorizontal();
            FlexibleSpace();
            Label("PSI.Settings.Visibility.Header", FontBold);
            FlexibleSpace();
            EndHorizontal();

            _scrollPosition = BeginScrollView(_scrollPosition);

            PsiSettings.ShowTargetPoint = Toggle(PsiSettings.ShowTargetPoint, "PSI.Settings.Visibility.TargetPoint".Translate());

            DrawCheckboxArea("PSI.Settings.Visibility.Aggressive".Translate(), PSIMaterials[Icons.Aggressive].mainTexture, ref ColBarSettings.ShowAggressive, ref PsiSettings.ShowAggressive);

            DrawCheckboxArea("PSI.Settings.Visibility.Dazed".Translate(), PSIMaterials[Icons.Dazed].mainTexture, ref ColBarSettings.ShowDazed, ref PsiSettings.ShowDazed);

            DrawCheckboxArea("PSI.Settings.Visibility.Leave".Translate(), PSIMaterials[Icons.Leave].mainTexture, ref ColBarSettings.ShowLeave, ref PsiSettings.ShowLeave);

            DrawCheckboxArea("PSI.Settings.Visibility.Draft".Translate(), PSIMaterials[Icons.Draft].mainTexture, ref ColBarSettings.ShowDraft, ref PsiSettings.ShowDraft);

            DrawCheckboxArea("PSI.Settings.Visibility.Idle".Translate(), PSIMaterials[Icons.Idle].mainTexture, ref ColBarSettings.ShowIdle, ref PsiSettings.ShowIdle);
            DrawCheckboxArea("PSI.Settings.Visibility.Unarmed".Translate(), PSIMaterials[Icons.Unarmed].mainTexture, ref ColBarSettings.ShowUnarmed, ref PsiSettings.ShowUnarmed);
            DrawCheckboxArea("PSI.Settings.Visibility.Hungry".Translate(), PSIMaterials[Icons.Hungry].mainTexture, ref ColBarSettings.ShowHungry, ref PsiSettings.ShowHungry);
            DrawCheckboxArea("PSI.Settings.Visibility.Sad".Translate(), PSIMaterials[Icons.Sad].mainTexture, ref ColBarSettings.ShowSad, ref PsiSettings.ShowSad);
            DrawCheckboxArea("PSI.Settings.Visibility.Tired".Translate(), PSIMaterials[Icons.Tired].mainTexture, ref ColBarSettings.ShowTired, ref PsiSettings.ShowTired);

            DrawCheckboxArea("PSI.Settings.Visibility.Sickness".Translate(), PSIMaterials[Icons.Sickness].mainTexture, ref ColBarSettings.ShowDisease, ref PsiSettings.ShowDisease);
            DrawCheckboxArea("PSI.Settings.Visibility.Pain".Translate(), PSIMaterials[Icons.Pain].mainTexture, ref ColBarSettings.ShowPain, ref PsiSettings.ShowPain);
            DrawCheckboxArea("PSI.Settings.Visibility.Health".Translate(), PSIMaterials[Icons.Health].mainTexture, ref ColBarSettings.ShowHealth, ref PsiSettings.ShowHealth);
            DrawCheckboxArea("PSI.Settings.Visibility.Injury".Translate(), PSIMaterials[Icons.Effectiveness].mainTexture, ref ColBarSettings.ShowEffectiveness, ref PsiSettings.ShowEffectiveness);
            DrawCheckboxArea("PSI.Settings.Visibility.Bloodloss".Translate(), PSIMaterials[Icons.Bloodloss].mainTexture, ref ColBarSettings.ShowBloodloss, ref PsiSettings.ShowBloodloss);

            DrawCheckboxArea("PSI.Settings.Visibility.Hot".Translate(), PSIMaterials[Icons.Hot].mainTexture, ref ColBarSettings.ShowHot, ref PsiSettings.ShowHot);
            DrawCheckboxArea("PSI.Settings.Visibility.Cold".Translate(), PSIMaterials[Icons.Freezing].mainTexture, ref ColBarSettings.ShowCold, ref PsiSettings.ShowCold);
            DrawCheckboxArea("PSI.Settings.Visibility.Naked".Translate(), PSIMaterials[Icons.Naked].mainTexture, ref ColBarSettings.ShowNaked, ref PsiSettings.ShowNaked);
            DrawCheckboxArea("PSI.Settings.Visibility.Drunk".Translate(), PSIMaterials[Icons.Drunk].mainTexture, ref ColBarSettings.ShowDrunk, ref PsiSettings.ShowDrunk);
            DrawCheckboxArea("PSI.Settings.Visibility.ApparelHealth".Translate(), PSIMaterials[Icons.ApparelHealth].mainTexture, ref ColBarSettings.ShowApparelHealth, ref PsiSettings.ShowApparelHealth);

            DrawCheckboxArea("PSI.Settings.Visibility.Pacific".Translate(), PSIMaterials[Icons.Pacific].mainTexture, ref ColBarSettings.ShowPacific, ref PsiSettings.ShowPacific);
            DrawCheckboxArea("PSI.Settings.Visibility.NightOwl".Translate(), PSIMaterials[Icons.NightOwl].mainTexture, ref ColBarSettings.ShowNightOwl, ref PsiSettings.ShowNightOwl);
            DrawCheckboxArea("PSI.Settings.Visibility.Greedy".Translate(), PSIMaterials[Icons.Greedy].mainTexture, ref ColBarSettings.ShowGreedy, ref PsiSettings.ShowGreedy);
            DrawCheckboxArea("PSI.Settings.Visibility.Jealous".Translate(), PSIMaterials[Icons.Jealous].mainTexture, ref ColBarSettings.ShowJealous, ref PsiSettings.ShowJealous);
            DrawCheckboxArea("PSI.Settings.Visibility.Lovers".Translate(), PSIMaterials[Icons.Love].mainTexture, ref ColBarSettings.ShowLovers, ref PsiSettings.ShowLovers);

            DrawCheckboxArea("PSI.Settings.Visibility.Prosthophile".Translate(), PSIMaterials[Icons.Prosthophile].mainTexture, ref ColBarSettings.ShowProsthophile, ref PsiSettings.ShowProsthophile);
            DrawCheckboxArea("PSI.Settings.Visibility.Prosthophobe".Translate(), PSIMaterials[Icons.Prosthophobe].mainTexture, ref ColBarSettings.ShowProsthophobe, ref PsiSettings.ShowProsthophobe);
            DrawCheckboxArea("PSI.Settings.Visibility.RoomStatus".Translate(), PSIMaterials[Icons.Crowded].mainTexture, ref ColBarSettings.ShowRoomStatus, ref PsiSettings.ShowRoomStatus);
            DrawCheckboxArea("PSI.Settings.Visibility.Bedroom".Translate(), PSIMaterials[Icons.Bedroom].mainTexture, ref ColBarSettings.ShowBedroom, ref PsiSettings.ShowBedroom);
            DrawCheckboxArea("PSI.Settings.Visibility.ShowDeadColonists".Translate(), PSIMaterials[Icons.DeadColonist].mainTexture, ref ColBarSettings.ShowDeadColonists, ref PsiSettings.ShowDeadColonists);

            DrawCheckboxArea("PSI.Settings.Visibility.Pyromaniac".Translate(), PSIMaterials[Icons.Pyromaniac].mainTexture, ref ColBarSettings.ShowPyromaniac, ref PsiSettings.ShowPyromaniac);

            //  PsiSettings.ShowIdle = Toggle(PsiSettings.ShowIdle, "PSI.Settings.Visibility.Idle".Translate());
            //  PsiSettings.ShowUnarmed = Toggle(PsiSettings.ShowUnarmed, "PSI.Settings.Visibility.Unarmed".Translate());
            //  PsiSettings.ShowHungry = Toggle(PsiSettings.ShowHungry, "PSI.Settings.Visibility.Hungry".Translate());
            //  PsiSettings.ShowSad = Toggle(PsiSettings.ShowSad, "PSI.Settings.Visibility.Sad".Translate());
            //  PsiSettings.ShowTired = Toggle(PsiSettings.ShowTired, "PSI.Settings.Visibility.Tired".Translate());
            //  //
            //  PsiSettings.ShowDisease = Toggle(PsiSettings.ShowDisease, "PSI.Settings.Visibility.Sickness".Translate());
            //  PsiSettings.ShowPain = Toggle(PsiSettings.ShowPain, "PSI.Settings.Visibility.Pain".Translate());
            //  PsiSettings.ShowHealth = Toggle(PsiSettings.ShowHealth, "PSI.Settings.Visibility.Health".Translate());
            //  PsiSettings.ShowEffectiveness = Toggle(PsiSettings.ShowEffectiveness, "PSI.Settings.Visibility.Injury".Translate());
            //  PsiSettings.ShowBloodloss = Toggle(PsiSettings.ShowBloodloss, "PSI.Settings.Visibility.Bloodloss".Translate());
            //  //
            //  PsiSettings.ShowHot = Toggle(PsiSettings.ShowHot, "PSI.Settings.Visibility.Hot".Translate());
            //  PsiSettings.ShowCold = Toggle(PsiSettings.ShowCold, "PSI.Settings.Visibility.Cold".Translate());
            //  PsiSettings.ShowNaked = Toggle(PsiSettings.ShowNaked, "PSI.Settings.Visibility.Naked".Translate());
            //  PsiSettings.ShowDrunk = Toggle(PsiSettings.ShowDrunk, "PSI.Settings.Visibility.Drunk".Translate());
            //  PsiSettings.ShowApparelHealth = Toggle(PsiSettings.ShowApparelHealth, "PSI.Settings.Visibility.ApparelHealth".Translate());
            //  //
            //  PsiSettings.ShowPacific = Toggle(PsiSettings.ShowPacific, "PSI.Settings.Visibility.Pacific".Translate());
            //  PsiSettings.ShowNightOwl = Toggle(PsiSettings.ShowNightOwl, "PSI.Settings.Visibility.NightOwl".Translate());
            //  PsiSettings.ShowGreedy = Toggle(PsiSettings.ShowGreedy, "PSI.Settings.Visibility.Greedy".Translate());
            //  PsiSettings.ShowJealous = Toggle(PsiSettings.ShowJealous, "PSI.Settings.Visibility.Jealous".Translate());
            //  PsiSettings.ShowLovers = Toggle(PsiSettings.ShowLovers, "PSI.Settings.Visibility.Lovers".Translate());
            //  //
            //  PsiSettings.ShowProsthophile = Toggle(PsiSettings.ShowProsthophile, "PSI.Settings.Visibility.Prosthophile".Translate());
            //  PsiSettings.ShowProsthophobe = Toggle(PsiSettings.ShowProsthophobe, "PSI.Settings.Visibility.Prosthophobe".Translate());
            //  PsiSettings.ShowRoomStatus = Toggle(PsiSettings.ShowRoomStatus, "PSI.Settings.Visibility.RoomStatus".Translate());
            //  PsiSettings.ShowBedroom = Toggle(PsiSettings.ShowBedroom, "PSI.Settings.Visibility.Bedroom".Translate());
            //
            //  PsiSettings.ShowDeadColonists = Toggle(PsiSettings.ShowDeadColonists, "PSI.Settings.Visibility.ShowDeadColonists".Translate());
            //  PsiSettings.ShowPyromaniac = Toggle(PsiSettings.ShowPyromaniac, "PSI.Settings.Visibility.Pyromaniac".Translate());
            Space(6f);
            EndScrollView();

        }

        private void DrawCheckboxArea(string translate, Texture mainTexture, ref bool ColBarBool, ref bool PsiBarBool)
        {
            BeginVertical();
            BeginHorizontal(GrayBG);
            FlexibleSpace();
            Label(translate, FontBold);
            FlexibleSpace();
            EndHorizontal();
            BeginHorizontal(DarkGrayBG);
            Space(10);
            FlexibleSpace();
            Label(mainTexture, Height(Text.LineHeight * 3f), Width(Text.LineHeight * 3));
            FlexibleSpace();
            ColBarBool = Toggle(ColBarBool, "ColonistBarKF.Settings.ColBarIconVisibility".Translate());
            FlexibleSpace();
            PsiBarBool = Toggle(PsiBarBool, "ColonistBarKF.Settings.PSIIconVisibility".Translate());
            FlexibleSpace();
            Space(10);
            EndHorizontal();
            EndVertical();
            Space(6f);
        }


        private void FillPagePSIOpacityAndColor()
        {
            BeginHorizontal();
            FlexibleSpace();
            Label("PSI.Settings.IconOpacityAndColor.Header".Translate(), FontBold);
            FlexibleSpace();
            EndHorizontal();
            _scrollPosition = BeginScrollView(_scrollPosition);

            Label("PSI.Settings.IconOpacityAndColor.Opacity".Translate());
            PsiSettings.IconOpacity = HorizontalSlider(PsiSettings.IconOpacity, 0.05f, 1f);

            Label("PSI.Settings.IconOpacityAndColor.OpacityCritical".Translate());
            PsiSettings.IconOpacityCritical = HorizontalSlider(PsiSettings.IconOpacityCritical, 0f, 1f);

            Toggle(PsiSettings.UseColoredTarget, "PSI.Settings.IconOpacityAndColor.UseColoredTarget".Translate());


            //if (listing.DoTextButton("PSI.Settings.ResetColors".Translate()))
            //{
            //    colorRedAlert = baseSettings.ColorRedAlert;
            //    Scribe_Values.LookValue(ref colorRedAlert, "colorRedAlert");
            //    colorInput.Value = colorRedAlert;
            //    PSI.SaveSettings();
            //}
            //
            //Rect row = new Rect(0f, listing.CurHeight, listing.ColumnWidth(), 24f);
            //
            //DrawMCMRegion(row);
            //
            //PSI.Settings.ColorRedAlert = colorInput.Value;
            //
            //listing.DoGap();
            //listing.DoGap();

            Space(6f);
            EndScrollView();

            //  if (listing.DoTextButton("PSI.Settings.ReturnButton".Translate()))
            //      Page = "main";
        }

        private void FillPSIPageArrangement()
        {

            BeginHorizontal();
            FlexibleSpace();
            Label("PSI.Settings.Arrangement.Header".Translate(), FontBold);
            FlexibleSpace();
            EndHorizontal();


            ColBarSettings.UsePsi = Toggle(ColBarSettings.UsePsi, "ColonistBarKF.SettingsColonistBar.UsePsiOnBar".Translate());
            if (ColBarSettings.UsePsi)
            {
                PsiBarPositionInt = Toolbar(PsiBarPositionInt, psiColBarStrings);
                Label("IconsInColumn".Translate() + ColBarSettings.IconsInColumn);
                ColBarSettings.IconsInColumn = (int)HorizontalSlider(ColBarSettings.IconsInColumn, 3f, 7f);
            }
            Space(6f);
            PsiSettings.UsePsi = Toggle(PsiSettings.UsePsi, "PSI.Settings.UsePSI".Translate());
            if (PsiSettings.UsePsi)
            {
                PsiPositionInt = Toolbar(PsiPositionInt, psiColBarStrings);
            }
            Space(6f);

            _scrollPosition = BeginScrollView(_scrollPosition);


            var num = (int)(PsiSettings.IconSize * 4.5);

            if (num > 8)
                num = 8;
            else if (num < 0)
                num = 0;

            Label("PSI.Settings.Arrangement.IconSize".Translate() + ("PSI.Settings.SizeLabel." + num).Translate());
            PsiSettings.IconSize = HorizontalSlider(PsiSettings.IconSize, 0.5f, 2f);

            Label(string.Concat("PSI.Settings.Arrangement.IconPosition".Translate(), (int)(PsiSettings.IconDistanceX * 100.0), " , ", (int)(PsiSettings.IconDistanceY * 100.0)));
            PsiSettings.IconDistanceX = HorizontalSlider(PsiSettings.IconDistanceX, -2f, 2f);
            PsiSettings.IconDistanceY = HorizontalSlider(PsiSettings.IconDistanceY, -2f, 2f);

            Label(string.Concat("PSI.Settings.Arrangement.IconOffset".Translate(), (int)(PsiSettings.IconOffsetX * 100.0), " , ", (int)(PsiSettings.IconOffsetY * 100.0)));
            PsiSettings.IconOffsetX = HorizontalSlider(PsiSettings.IconOffsetX, -2f, 2f);
            PsiSettings.IconOffsetY = HorizontalSlider(PsiSettings.IconOffsetY, -2f, 2f);

            PsiSettings.IconsHorizontal = Toggle(PsiSettings.IconsHorizontal, "PSI.Settings.Arrangement.Horizontal".Translate());

            PsiSettings.IconsScreenScale = Toggle(PsiSettings.IconsScreenScale, "PSI.Settings.Arrangement.ScreenScale".Translate());

            Label("PSI.Settings.Arrangement.IconsPerColumn".Translate() + PsiSettings.IconsInColumn);

            PsiSettings.IconsInColumn = (int)HorizontalSlider(PsiSettings.IconsInColumn, 1f, 7f);


            //   if (!listing.DoTextButton("PSI.Settings.ReturnButton".Translate()))
            //       return;
            //
            //   Page = "main";
            Space(6f);
            EndScrollView();
        }


        private void FillPSIPageLimits()
        {

            BeginHorizontal();
            FlexibleSpace();
            Label("PSI.Settings.Sensitivity.Header".Translate(), FontBold);
            FlexibleSpace();
            EndHorizontal();

            _scrollPosition = BeginScrollView(_scrollPosition);

            if (Button("PSI.Settings.LoadPresetButton".Translate()))
            {
                var options = new List<FloatMenuOption>();
                options.Add(new FloatMenuOption("Less Sensitive", () =>
                {
                    try
                    {
                        PsiSettings.LimitBleedMult = 2f;
                        PsiSettings.LimitDiseaseLess = 1f;
                        PsiSettings.LimitEfficiencyLess = 0.28f;
                        PsiSettings.LimitFoodLess = 0.2f;
                //        PsiSettings.LimitMoodLess = 0.2f;
                        PsiSettings.LimitRestLess = 0.2f;
                        PsiSettings.LimitApparelHealthLess = 0.5f;
                        PsiSettings.LimitTempComfortOffset = 3f;
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.Settings.LoadPreset.UnableToLoad".Translate() + "Less Sensitive");
                    }
                }));
                options.Add(new FloatMenuOption("Standard", () =>
                {
                    try
                    {
                        PsiSettings.LimitBleedMult = 3f;
                        PsiSettings.LimitDiseaseLess = 1f;
                        PsiSettings.LimitEfficiencyLess = 0.33f;
                        PsiSettings.LimitFoodLess = 0.25f;
                 //       PsiSettings.LimitMoodLess = 0.25f;
                        PsiSettings.LimitRestLess = 0.25f;
                        PsiSettings.LimitApparelHealthLess = 0.5f;
                        PsiSettings.LimitTempComfortOffset = 0f;
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.Settings.LoadPreset.UnableToLoad".Translate() + "Standard");
                    }
                }));
                options.Add(new FloatMenuOption("More Sensitive", () =>
                {
                    try
                    {
                        PsiSettings.LimitBleedMult = 4f;
                        PsiSettings.LimitDiseaseLess = 1f;
                        PsiSettings.LimitEfficiencyLess = 0.45f;
                        PsiSettings.LimitFoodLess = 0.3f;
                  //      PsiSettings.LimitMoodLess = 0.3f;
                        PsiSettings.LimitRestLess = 0.3f;
                        PsiSettings.LimitApparelHealthLess = 0.5f;
                        PsiSettings.LimitTempComfortOffset = -3f;
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.Settings.LoadPreset.UnableToLoad".Translate() + "More Sensitive");
                    }
                }));

                Find.WindowStack.Add(new FloatMenu(options));
            }


            Label("PSI.Settings.Sensitivity.Bleeding".Translate() + ("PSI.Settings.Sensitivity.Bleeding." + Math.Round(PsiSettings.LimitBleedMult - 0.25)).Translate());
            PsiSettings.LimitBleedMult = HorizontalSlider(PsiSettings.LimitBleedMult, 0.5f, 5f);

            Label("PSI.Settings.Sensitivity.Injured".Translate() + (int)(PsiSettings.LimitEfficiencyLess * 100.0) + "%");
            PsiSettings.LimitEfficiencyLess = HorizontalSlider(PsiSettings.LimitEfficiencyLess, 0.01f, 0.99f);

            Label("PSI.Settings.Sensitivity.Food".Translate() + (int)(PsiSettings.LimitFoodLess * 100.0) + "%");
            PsiSettings.LimitFoodLess = HorizontalSlider(PsiSettings.LimitFoodLess, 0.01f, 0.99f);

     //     Label("PSI.Settings.Sensitivity.Mood".Translate() + (int)(PsiSettings.LimitMoodLess * 100.0) + "%");
     //     PsiSettings.LimitMoodLess = HorizontalSlider(PsiSettings.LimitMoodLess, 0.01f, 0.99f);

            Label("PSI.Settings.Sensitivity.Rest".Translate() + (int)(PsiSettings.LimitRestLess * 100.0) + "%");
            PsiSettings.LimitRestLess = HorizontalSlider(PsiSettings.LimitRestLess, 0.01f, 0.99f);

            Label("PSI.Settings.Sensitivity.ApparelHealth".Translate() + (int)(PsiSettings.LimitApparelHealthLess * 100.0) + "%");
            PsiSettings.LimitApparelHealthLess = HorizontalSlider(PsiSettings.LimitApparelHealthLess, 0.01f, 0.99f);

            Label("PSI.Settings.Sensitivity.Temperature".Translate() + (int)PsiSettings.LimitTempComfortOffset + "C");
            PsiSettings.LimitTempComfortOffset = HorizontalSlider(PsiSettings.LimitTempComfortOffset, -10f, 10f);

            Space(6f);
            EndScrollView();

            //  if (!listing.DoTextButton("PSI.Settings.ReturnButton".Translate()))
            //      return;
            //
            //  Page = "main";
        }


#if NoCCL
        public void ExposeData()
#else
        public override void ExposeData()
#endif
        {
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginTopHor, "useCustomMarginTopHor", false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginBottomHor, "UseCustomMarginBottomHor", false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginLeftHorTop, "useCustomMarginLeftHor", false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginRightHorTop, "useCustomMarginRightHor", false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginLeftHorBottom, "UseCustomMarginLeftHorBottom", false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginRightHorBottom, "UseCustomMarginRightHorBottom", false);

            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginTopVerLeft, "UseCustomMarginTopVerLeft", false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginTopVerRight, "UseCustomMarginTopVerRight", false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginLeftVer, "useCustomMarginLeftRightVer", false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginRightVer, "UseCustomMarginRightVer", false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginBottomVerLeft, "UseCustomMarginBottomVerLeft", false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginBottomVerRight, "UseCustomMarginBottomVerRight", false);


            Scribe_Values.LookValue(ref ColBarSettings.UseCustomBaseSpacingHorizontal, "useCustomBaseSpacingHorizontal", false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomBaseSpacingVertical, "useCustomBaseSpacingVertical", false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomIconSize, "useCustomIconSize", false);
            Scribe_Values.LookValue(ref ColBarSettings.UseFixedIconScale, "useFixedIconScale", false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomPawnTextureCameraHorizontalOffset, "UseCustomPawnTextureCameraHorizontalOffset", false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomPawnTextureCameraVerticalOffset, "useCustomPawnTextureCameraVerticalOffset", false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomPawnTextureCameraZoom, "useCustomPawnTextureCameraZoom", false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomDoubleClickTime, "useCustomDoubleClick", false);
            Scribe_Values.LookValue(ref ColBarSettings.UseGender, "useGender", false);
            Scribe_Values.LookValue(ref ColBarSettings.UseVerticalAlignment, "useVerticalAlignment", false);
            Scribe_Values.LookValue(ref ColBarSettings.UseRightAlignment, "useRightAlignment", false);
            Scribe_Values.LookValue(ref ColBarSettings.UseBottomAlignment, "useBottomAlignment", false);

            Scribe_Values.LookValue(ref ColBarSettings.UseMoodColors, "UseMoodColors", false);
            Scribe_Values.LookValue(ref ColBarSettings.UseWeaponIcons, "UseWeaponIcons", false);

            Scribe_Values.LookValue(ref ColBarSettings.MarginTopHor, "MarginTopHor", 21f);
            Scribe_Values.LookValue(ref ColBarSettings.MarginBottomHor, "MarginBottomHor", 21f);
            Scribe_Values.LookValue(ref ColBarSettings.MarginLeftHorTop, "MarginLeftHorTop", 160f);
            Scribe_Values.LookValue(ref ColBarSettings.MarginRightHorTop, "MarginRightHorTop", 160f);
            Scribe_Values.LookValue(ref ColBarSettings.MarginLeftHorBottom, "MarginLeftHorBottom", 160f);
            Scribe_Values.LookValue(ref ColBarSettings.MarginRightHorBottom, "MarginRightHorBottom", 160f);

            Scribe_Values.LookValue(ref ColBarSettings.MarginTopVerLeft, "MarginTopVerLeft", 120f);
            Scribe_Values.LookValue(ref ColBarSettings.MarginBottomVerLeft, "MarginBottomVerLeft", 120f);
            Scribe_Values.LookValue(ref ColBarSettings.MarginTopVerRight, "MarginTopVerRight", 120f);
            Scribe_Values.LookValue(ref ColBarSettings.MarginBottomVerRight, "MarginBottomVerRight", 120f);
            Scribe_Values.LookValue(ref ColBarSettings.MarginLeftVer, "MarginLeftVer", 21f);
            Scribe_Values.LookValue(ref ColBarSettings.MarginRightVer, "MarginRightVer", 21f);

            Scribe_Values.LookValue(ref ColBarSettings.HorizontalOffset, "HorizontalOffset", 0f);
            Scribe_Values.LookValue(ref ColBarSettings.VerticalOffset, "VerticalOffset", 0f);


            Scribe_Values.LookValue(ref ColBarSettings.BaseSpacingHorizontal, "BaseSpacingHorizontal", 24f);
            Scribe_Values.LookValue(ref ColBarSettings.BaseSpacingVertical, "BaseSpacingVertical", 32f);
            Scribe_Values.LookValue(ref ColBarSettings.BaseSizeFloat, "BaseSizeFloat", 48f);
            Scribe_Values.LookValue(ref ColBarSettings.BaseIconSize, "BaseIconSize", 20f);
            Scribe_Values.LookValue(ref ColBarSettings.PawnTextureCameraHorizontalOffset, "PawnTextureCameraHorizontalOffset", 0f);
            Scribe_Values.LookValue(ref ColBarSettings.PawnTextureCameraVerticalOffset, "PawnTextureCameraVerticalOffset", 0.3f);
            Scribe_Values.LookValue(ref ColBarSettings.PawnTextureCameraZoom, "PawnTextureCameraZoom", 1.28205f);
            Scribe_Values.LookValue(ref ColBarSettings.MaxColonistBarWidth, "MaxColonistBarWidth");
            Scribe_Values.LookValue(ref ColBarSettings.MaxColonistBarHeight, "MaxColonistBarHeight");


            Scribe_Values.LookValue(ref ColBarSettings.DoubleClickTime, "DoubleClickTime", 0.5f);

            Scribe_Values.LookValue(ref ColBarSettings.FemaleColor, "FemaleColor");
            Scribe_Values.LookValue(ref ColBarSettings.MaleColor, "MaleColor");

            Scribe_Values.LookValue(ref ColBarSettings.MaxRows, "MaxRows");
            Scribe_Values.LookValue(ref ColBarSettings.SortBy, "SortBy");
            Scribe_Values.LookValue(ref ColBarSettings.useZoomToMouse, "useZoomToMouse");
            Scribe_Values.LookValue(ref ColBarSettings.moodRectScale, "moodRectScale");


#if !NoCCL
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                femaleColorField.Value = barSettings.FemaleColor;
                maleColorField.Value = barSettings.MaleColor;
            }
#endif
        }

        #endregion

    }
}
