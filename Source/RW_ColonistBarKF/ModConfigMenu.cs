#if !NoCCL
using CommunityCoreLibrary;
using CommunityCoreLibrary.UI;
# endif
using System;
using System.Collections.Generic;
using System.IO;
using RW_ColonistBarKF;
using UnityEngine;
using Verse;
using static ColonistBarKF.CBKF;
using static ColonistBarKF.PSI.PSI;
using static ColonistBarKF.SettingsColBar.SortByWhat;
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

        private int lastupdate = -5000;

        public override void WindowUpdate()
        {
            if (Find.TickManager.TicksGame > lastupdate)
            {
                ColonistBar_KF.BaseSize.x = CBKF.SettingsColBar.BaseSizeFloat;
                ColonistBar_KF.BaseSize.y = CBKF.SettingsColBar.BaseSizeFloat;
                ColonistBar_KF.PawnTextureSize.x = CBKF.SettingsColBar.BaseSizeFloat - 2f;
                ColonistBar_KF.PawnTextureSize.y = CBKF.SettingsColBar.BaseSizeFloat * 1.5f;

                if (CBKF.SettingsColBar.UseGender)
                    ColonistBarTextures.BGTex = ColonistBarTextures.BGTexGrey;
                else
                {
                    ColonistBarTextures.BGTex = ColonistBarTextures.BGTexVanilla;
                }
                lastupdate = Find.TickManager.TicksGame + 1000;
            }
        }


#if NoCCL
        public override void PreClose()
        {
            SaveBarSettings();
            SavePsiSettings();
        }

        public override Vector2 InitialSize
        {
            get { return new Vector2(438f, 640f); }
        }
        private int mainToolbarInt = 0;
        private int psiToolbarInt = 0;
        private int barPositionInt = 0;
        private int psiBarPositionInt = 0;

        public string[] mainToolbarStrings =
            {
            "ColonistBarKF.SettingsColBar".Translate(),
            "ColonistBarKF.SettingsPSI".Translate()
        };
        public string[] psiToolbarStrings =
        {
            "ColonistBarKF.SettingsColBar.PSIToolbar01".Translate(),
            "ColonistBarKF.SettingsColBar.PSIToolbar02".Translate(),
            "ColonistBarKF.SettingsColBar.PSIToolbar03".Translate(),
            "ColonistBarKF.SettingsColBar.PSIToolbar04".Translate(),
            "ColonistBarKF.SettingsColBar.PSIToolbar05".Translate()
        };

        public string[] psiPositionStrings =
        {
            "ColonistBarKF.SettingsColBar.useTop".Translate(),
            "ColonistBarKF.SettingsColBar.useBottom".Translate(),
            "ColonistBarKF.SettingsColBar.useLeft".Translate(),
            "ColonistBarKF.SettingsColBar.useRight".Translate()
        };

        public string[] psiColBarStrings =
{
            "ColonistBarKF.SettingsColBar.useLeft".Translate(),
            "ColonistBarKF.SettingsColBar.useRight".Translate(),
            "ColonistBarKF.SettingsColBar.useTop".Translate(),
            "ColonistBarKF.SettingsColBar.useBottom".Translate()
        };

        public int MainToolbarInt
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

        public int BarPositionInt
        {
            get
            {
                if (!CBKF.SettingsColBar.UseBottomAlignment && !CBKF.SettingsColBar.UseVerticalAlignment && !CBKF.SettingsColBar.UseRightAlignment)
                {
                    barPositionInt = 0;
                }
                if (CBKF.SettingsColBar.UseBottomAlignment && !CBKF.SettingsColBar.UseVerticalAlignment && !CBKF.SettingsColBar.UseRightAlignment)
                {
                    barPositionInt = 1;
                }
                if (!CBKF.SettingsColBar.UseBottomAlignment && CBKF.SettingsColBar.UseVerticalAlignment && !CBKF.SettingsColBar.UseRightAlignment)
                {
                    barPositionInt = 2;
                }
                if (!CBKF.SettingsColBar.UseBottomAlignment && CBKF.SettingsColBar.UseVerticalAlignment && CBKF.SettingsColBar.UseRightAlignment)
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
                        CBKF.SettingsColBar.UseBottomAlignment = false;
                        CBKF.SettingsColBar.UseVerticalAlignment = false;
                        CBKF.SettingsColBar.UseRightAlignment = false;
                        break;
                    case 1:
                        CBKF.SettingsColBar.UseBottomAlignment = true;
                        CBKF.SettingsColBar.UseVerticalAlignment = false;
                        CBKF.SettingsColBar.UseRightAlignment = false; break;
                    case 2:
                        CBKF.SettingsColBar.UseBottomAlignment = false;
                        CBKF.SettingsColBar.UseVerticalAlignment = true;
                        CBKF.SettingsColBar.UseRightAlignment = false; break;
                    case 3:
                        CBKF.SettingsColBar.UseBottomAlignment = false;
                        CBKF.SettingsColBar.UseVerticalAlignment = true;
                        CBKF.SettingsColBar.UseRightAlignment = true; break;
                    default:
                        CBKF.SettingsColBar.UseBottomAlignment = false;
                        CBKF.SettingsColBar.UseVerticalAlignment = false;
                        CBKF.SettingsColBar.UseRightAlignment = false;
                        break;
                }

                barPositionInt = value;
            }
        }

        public int PsiBarPositionInt
        {
            get
            {

                if (CBKF.SettingsColBar.IconAlignment == 0)
                {
                    psiBarPositionInt = 0;
                }
                if (CBKF.SettingsColBar.IconAlignment == 1)
                {
                    psiBarPositionInt = 1;
                }
                if (CBKF.SettingsColBar.IconAlignment == 2)
                {
                    psiBarPositionInt = 2;
                }
                if (CBKF.SettingsColBar.IconAlignment == 3)
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
                        CBKF.SettingsColBar.IconAlignment = 0;
                        CBKF.SettingsColBar.IconDistanceX = 1f;
                        CBKF.SettingsColBar.IconDistanceY = 1f;
                        CBKF.SettingsColBar.IconOffsetX = 1f;
                        CBKF.SettingsColBar.IconOffsetY = 1f;
                        CBKF.SettingsColBar.IconsHorizontal = false;
                        CBKF.SettingsColBar.IconsScreenScale = true;
                        CBKF.SettingsColBar.IconsInColumn = 4;
                        CBKF.SettingsColBar.IconSize = 1f;
                        CBKF.SettingsColBar.IconOpacity = 0.7f;
                        CBKF.SettingsColBar.IconOpacityCritical = 0.6f;
                        Reinit();
                        break;
                    case 1:
                        CBKF.SettingsColBar.IconAlignment = 1;
                        CBKF.SettingsColBar.IconDistanceX = -1f;
                        CBKF.SettingsColBar.IconDistanceY = 1f;
                        CBKF.SettingsColBar.IconOffsetX = -1f;
                        CBKF.SettingsColBar.IconOffsetY = 1f;
                        CBKF.SettingsColBar.IconsHorizontal = false;
                        CBKF.SettingsColBar.IconsScreenScale = true;
                        CBKF.SettingsColBar.IconsInColumn = 4;
                        CBKF.SettingsColBar.IconSize = 1f;
                        CBKF.SettingsColBar.IconOpacity = 0.7f;
                        CBKF.SettingsColBar.IconOpacityCritical = 0.6f;
                        Reinit();
                        break;
                    case 2:
                        CBKF.SettingsColBar.IconAlignment = 2;
                        CBKF.SettingsColBar.IconDistanceX = 1f;
                        CBKF.SettingsColBar.IconDistanceY = -1;
                        CBKF.SettingsColBar.IconOffsetX = -1f;
                        CBKF.SettingsColBar.IconOffsetY = 1f;
                        CBKF.SettingsColBar.IconsHorizontal = true;
                        CBKF.SettingsColBar.IconsScreenScale = true;
                        CBKF.SettingsColBar.IconsInColumn = 4;
                        CBKF.SettingsColBar.IconSize = 1f;
                        CBKF.SettingsColBar.IconOpacity = 0.7f;
                        CBKF.SettingsColBar.IconOpacityCritical = 0.6f;
                        Reinit();
                        break;
                    case 3:
                        CBKF.SettingsColBar.IconAlignment = 3;
                        CBKF.SettingsColBar.IconDistanceX = 1;
                        CBKF.SettingsColBar.IconDistanceY = 1;
                        CBKF.SettingsColBar.IconOffsetX = -1;
                        CBKF.SettingsColBar.IconOffsetY = -1;
                        CBKF.SettingsColBar.IconsHorizontal = true;
                        CBKF.SettingsColBar.IconsScreenScale = true;
                        CBKF.SettingsColBar.IconsInColumn = 4;
                        CBKF.SettingsColBar.IconSize = 1f;
                        CBKF.SettingsColBar.IconOpacity = 0.7f;
                        CBKF.SettingsColBar.IconOpacityCritical = 0.6f;
                        Reinit();
                        break;
                    default:
                        CBKF.SettingsColBar.IconAlignment = 0;

                        break;
                }
                psiBarPositionInt = value;
            }
        }


        public int PSIToolbarInt
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
            hover = { background = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.2f, 1)) }
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

            Label("Colonist Bar KF 0.15.3", Headline);
            Space(6f);
            BeginVertical(Width(viewRect.width), Height(viewRect.height));
            MainToolbarInt = Toolbar(MainToolbarInt, mainToolbarStrings, Width(viewRect.width));

            switch (MainToolbarInt)
            {
                case 0:
                    {
                        Label("ColonistBarKF.SettingsColBar.Position".Translate(), FontBold);
                        BarPositionInt = Toolbar(BarPositionInt, psiPositionStrings,  Width(viewRect.width));
                        Label("ColonistBarKF.SettingsColBar.PsiBarPosition".Translate(), FontBold);
                        CBKF.SettingsColBar.UsePsi = Toggle(CBKF.SettingsColBar.UsePsi, "UsePsiOnBar".Translate());
                        if (CBKF.SettingsColBar.UsePsi)
                        {
                            PsiBarPositionInt = Toolbar(PsiBarPositionInt, psiColBarStrings, Width(viewRect.width));
                        }
                        Space(18f);
                        FillPagePosition(viewRect.width);
                        Space(18f);
                        _scrollPosition = BeginScrollView(_scrollPosition, Width(viewRect.width));
                        FillPageOptions(viewRect.width);

                    }
                    break;
                case 1:
                    {
                        Label("ColonistBarKF.SettingsPSI.Settings".Translate(), FontBold);
                        PSIToolbarInt = Toolbar(PSIToolbarInt, psiToolbarStrings, Width(viewRect.width));
                        Space(12);
                        SettingsPsi.UsePsi = Toggle(SettingsPsi.UsePsi, "UsePsiOnPawn".Translate());
                        Space(18f);
                        _scrollPosition = BeginScrollView(_scrollPosition, Width(viewRect.width));

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
            EndScrollView();

            FlexibleSpace();
            Space(12f);
            BeginHorizontal(hoverBox);
            if (Button("ColonistBarKF.SettingsColBar.RevertSettings".Translate(), Width(viewRect.width / 2 - 10f)))
            {
                ResetBarSettings();
            }
            if (Button("ColonistBarKF.SettingsColBar.RevertPSISettings".Translate(), Width(viewRect.width / 2 - 10f)))
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
            //SettingsColBar.Reloadsettings = true;

            {
                CBKF.SettingsColBar.UseGender = false;
                CBKF.SettingsColBar.UseCustomMarginTopHor = false;
                CBKF.SettingsColBar.UseCustomBaseSpacingHorizontal = false;
                CBKF.SettingsColBar.UseCustomBaseSpacingVertical = false;
                CBKF.SettingsColBar.UseCustomIconSize = false;
                CBKF.SettingsColBar.UseCustomPawnTextureCameraHorizontalOffset = false;
                CBKF.SettingsColBar.UseCustomPawnTextureCameraVerticalOffset = false;
                CBKF.SettingsColBar.UseCustomPawnTextureCameraZoom = false;
                CBKF.SettingsColBar.UseCustomMarginLeftHorTop = false;
                CBKF.SettingsColBar.UseCustomMarginRightHorTop = false;
                CBKF.SettingsColBar.UseCustomMarginLeftHorBottom = false;
                CBKF.SettingsColBar.UseCustomMarginRightHorBottom = false;
                CBKF.SettingsColBar.UseBottomAlignment = false;
                CBKF.SettingsColBar.UseMoodColors = false;
                CBKF.SettingsColBar.UseWeaponIcons = false;
                CBKF.SettingsColBar.UseFixedIconScale = false;

                CBKF.SettingsColBar.MarginBottomHor = 21f;
                CBKF.SettingsColBar.BaseSpacingHorizontal = 24f;
                CBKF.SettingsColBar.BaseSpacingVertical = 32f;
                CBKF.SettingsColBar.BaseSizeFloat = 48f;
                CBKF.SettingsColBar.PawnTextureCameraHorizontalOffset = 0f;
                CBKF.SettingsColBar.PawnTextureCameraVerticalOffset = 0.3f;
                ColonistBar_KF.PawnTextureCameraOffset = new Vector3(0f, 0f, 0.3f);
                CBKF.SettingsColBar.PawnTextureCameraZoom = 1.28205f;
                CBKF.SettingsColBar.MaxColonistBarWidth = Screen.width - 320f;
                CBKF.SettingsColBar.FemaleColor = new Color(1f, 0.64f, 0.8f, 1f);
                CBKF.SettingsColBar.MaleColor = new Color(0.52f, 0.75f, 0.92f, 1f);
#if !NoCCL
                femaleColorField.Value = new Color(1f, 0.64f, 0.8f, 1f);
                maleColorField.Value = new Color(0.52f, 0.75f, 0.92f, 1f);
#endif
                CBKF.SettingsColBar.HorizontalOffset = 0f;
                CBKF.SettingsColBar.VerticalOffset = 0f;
                CBKF.SettingsColBar.UseCustomBaseSpacingVertical = false;
                CBKF.SettingsColBar.UseVerticalAlignment = false;
                CBKF.SettingsColBar.BaseSpacingVertical = 32f;
                CBKF.SettingsColBar.MaxColonistBarHeight = Screen.height - 240f;
                CBKF.SettingsColBar.UseRightAlignment = false;
                CBKF.SettingsColBar.MarginLeftHorTop = 180f;
                CBKF.SettingsColBar.MarginRightHorTop = 180f;
                CBKF.SettingsColBar.MarginLeftHorBottom = 180f;
                CBKF.SettingsColBar.MarginRightHorBottom = 180f;
                CBKF.SettingsColBar.UseCustomDoubleClickTime = false;
                CBKF.SettingsColBar.DoubleClickTime = 0.5f;
                CBKF.SettingsColBar.UseCustomMarginLeftVer = false;
                CBKF.SettingsColBar.MarginLeftVer = 21f;
                CBKF.SettingsColBar.UseCustomMarginTopVerLeft = false;
                CBKF.SettingsColBar.MarginTopVerLeft = 120f;
                CBKF.SettingsColBar.UseCustomMarginBottomVerLeft = false;
                CBKF.SettingsColBar.MarginBottomVerLeft = 120f;

                CBKF.SettingsColBar.UseCustomMarginTopHor = false;
                CBKF.SettingsColBar.UseCustomMarginBottomHor = false;
                CBKF.SettingsColBar.UseCustomMarginLeftHorTop = false;
                CBKF.SettingsColBar.UseCustomMarginRightHorTop = false;

                CBKF.SettingsColBar.UseCustomMarginTopVerLeft = false;
                CBKF.SettingsColBar.UseCustomMarginTopVerRight = false;
                CBKF.SettingsColBar.UseCustomMarginLeftVer = false;
                CBKF.SettingsColBar.UseCustomMarginRightVer = false;
                CBKF.SettingsColBar.UseCustomMarginBottomVerLeft = false;
                CBKF.SettingsColBar.UseCustomMarginBottomVerRight = false;
                CBKF.SettingsColBar.SortBy = vanilla;
                CBKF.SettingsColBar.useZoomToMouse = false;
                CBKF.SettingsColBar.moodRectScale = 0.33f;
            }
        }

        private void ResetPSISettings()
        {
            SettingsPsi.UsePsi = true;
            SettingsPsi.IconSize = 1f;
            SettingsPsi.IconSizeMult = 1f;
            SettingsPsi.IconDistanceX = 1f;
            SettingsPsi.IconDistanceY = 1f;
            SettingsPsi.IconOffsetX = 1f;
            SettingsPsi.IconOffsetY = 1f;

            SettingsPsi.IconsInColumn = 3;
            SettingsPsi.IconsHorizontal = false;
            SettingsPsi.IconsScreenScale = true;
            SettingsPsi.IconSet = "default";

            SettingsPsi.ShowTargetPoint = true;
            SettingsPsi.ShowAggressive = true;
            SettingsPsi.ShowDazed = true;
            SettingsPsi.ShowLeave = true;
            SettingsPsi.ShowDraft = true;
            SettingsPsi.ShowIdle = true;
            SettingsPsi.ShowUnarmed = true;
            SettingsPsi.ShowHungry = true;
            SettingsPsi.ShowSad = true;
            SettingsPsi.ShowTired = true;
            SettingsPsi.ShowDisease = true;
            SettingsPsi.ShowEffectiveness = true;
            SettingsPsi.ShowBloodloss = true;
            SettingsPsi.ShowHot = true;
            SettingsPsi.ShowCold = true;
            SettingsPsi.ShowNaked = true;
            SettingsPsi.ShowDrunk = true;
            SettingsPsi.ShowApparelHealth = true;
            SettingsPsi.ShowPacific = true;
            SettingsPsi.ShowProsthophile = true;
            SettingsPsi.ShowProsthophobe = true;
            SettingsPsi.ShowNightOwl = true;
            SettingsPsi.ShowGreedy = true;
            SettingsPsi.ShowJealous = true;
            SettingsPsi.ShowLovers = true;
            SettingsPsi.ShowDeadColonists = true;
            SettingsPsi.ShowLeftUnburied = true;
            SettingsPsi.ShowRoomStatus = true;
            SettingsPsi.ShowPain = true;
            SettingsPsi.ShowBedroom = true;
            SettingsPsi.ShowHealth = true;
            SettingsPsi.ShowPyromaniac = true;

            SettingsPsi.LimitMoodLess = 0.25f;
            SettingsPsi.LimitFoodLess = 0.25f;
            SettingsPsi.LimitRestLess = 0.25f;
            SettingsPsi.LimitEfficiencyLess = 0.33f;
            SettingsPsi.LimitDiseaseLess = 1f;
            SettingsPsi.LimitBleedMult = 3f;
            SettingsPsi.LimitApparelHealthLess = 0.5f;
            SettingsPsi.LimitTempComfortOffset = 0f;
            SettingsPsi.IconOpacity = 0.7f;
            SettingsPsi.IconOpacityCritical = 0.6f;
            SettingsPsi.UseColoredTarget = true;

        }

        private void FillPagePosition(float width)
        {
            float colSize = width / 2 - 10f;
            #region Vertical Alignment

            if (CBKF.SettingsColBar.UseVerticalAlignment)
            {
#if !NoCCL
                listing.Indent();
#endif
                if (CBKF.SettingsColBar.UseRightAlignment)
                {
                    BeginHorizontal(hoverBox);
                    CBKF.SettingsColBar.UseCustomMarginRightVer = Toggle(CBKF.SettingsColBar.UseCustomMarginRightVer, "ColonistBarKF.SettingsColBar.MarginEdge".Translate(), Width(colSize));
                    if (CBKF.SettingsColBar.UseCustomMarginRightVer)
                    {
                        //    listing.Gap(3f);
                        CBKF.SettingsColBar.MarginRightVer = HorizontalSlider(CBKF.SettingsColBar.MarginRightVer, 0f, Screen.width / 12, Width(colSize));
                    }
                    else
                    {
                        CBKF.SettingsColBar.MarginRightVer = 21f;
                    }
                    // listing.Gap(3f);
                    EndHorizontal();
                    BeginHorizontal(hoverBox);
                    CBKF.SettingsColBar.UseCustomMarginTopVerRight = Toggle(CBKF.SettingsColBar.UseCustomMarginTopVerRight, "ColonistBarKF.SettingsColBar.MarginTop".Translate(), Width(colSize));
                    if (CBKF.SettingsColBar.UseCustomMarginTopVerRight)
                    {
                        CBKF.SettingsColBar.MarginTopVerRight = HorizontalSlider(CBKF.SettingsColBar.MarginTopVerRight, 0f, Screen.height * 2 / 5, Width(colSize));
                    }
                    else
                    {
                        CBKF.SettingsColBar.MarginTopVerRight = 120f;
                        CBKF.SettingsColBar.MaxColonistBarHeight = Screen.height - CBKF.SettingsColBar.MarginTopVerRight - CBKF.SettingsColBar.MarginBottomVerRight;
                        CBKF.SettingsColBar.VerticalOffset = CBKF.SettingsColBar.MarginTopVerRight / 2 - CBKF.SettingsColBar.MarginBottomVerRight / 2;

                    }
                    EndHorizontal();
                    //  listing.Gap(3f);
                    BeginHorizontal(hoverBox);
                    CBKF.SettingsColBar.UseCustomMarginBottomVerRight = Toggle(CBKF.SettingsColBar.UseCustomMarginBottomVerRight, "ColonistBarKF.SettingsColBar.MarginBottom".Translate(),  Width(colSize));
                    if (CBKF.SettingsColBar.UseCustomMarginBottomVerRight)
                    {
                        //     listing.Gap(3f);
                        CBKF.SettingsColBar.MarginBottomVerRight = HorizontalSlider(CBKF.SettingsColBar.MarginBottomVerRight, 0f, Screen.height * 2 / 5, Width(colSize));
                    }
                    else
                    {
                        CBKF.SettingsColBar.MarginBottomVerRight = 120f;
                        CBKF.SettingsColBar.MaxColonistBarHeight = Screen.height - CBKF.SettingsColBar.MarginTopVerRight - CBKF.SettingsColBar.MarginBottomVerRight;
                        CBKF.SettingsColBar.VerticalOffset = CBKF.SettingsColBar.MarginTopVerRight / 2 - CBKF.SettingsColBar.MarginBottomVerRight / 2;
                    }
                    EndHorizontal();
                }
                else
                {
                    BeginHorizontal(hoverBox);
                    CBKF.SettingsColBar.UseCustomMarginLeftVer = Toggle(CBKF.SettingsColBar.UseCustomMarginLeftVer, "ColonistBarKF.SettingsColBar.MarginEdge".Translate(),  Width(colSize));
                    if (CBKF.SettingsColBar.UseCustomMarginLeftVer)
                    {
                        //     listing.Gap(3f);
                        CBKF.SettingsColBar.MarginLeftVer = HorizontalSlider(CBKF.SettingsColBar.MarginLeftVer, 0f, Screen.width / 12, Width(colSize));
                    }
                    else
                    {
                        CBKF.SettingsColBar.MarginLeftVer = 21f;
                    }
                    //   listing.Gap(3f);
                    EndHorizontal();
                    BeginHorizontal(hoverBox);
                    CBKF.SettingsColBar.UseCustomMarginTopVerLeft = Toggle(CBKF.SettingsColBar.UseCustomMarginTopVerLeft, "ColonistBarKF.SettingsColBar.MarginTop".Translate(),  Width(colSize));
                    if (CBKF.SettingsColBar.UseCustomMarginTopVerLeft)
                    {
                        //    listing.Gap(3f);
                        CBKF.SettingsColBar.MarginTopVerLeft = HorizontalSlider(CBKF.SettingsColBar.MarginTopVerLeft, 0f, Screen.height * 2 / 5, Width(colSize));
                    }
                    else
                    {
                        CBKF.SettingsColBar.MarginTopVerLeft = 120f;
                        CBKF.SettingsColBar.MaxColonistBarHeight = Screen.height - CBKF.SettingsColBar.MarginTopVerLeft - CBKF.SettingsColBar.MarginBottomVerLeft;
                        CBKF.SettingsColBar.VerticalOffset = CBKF.SettingsColBar.MarginTopVerLeft / 2 - CBKF.SettingsColBar.MarginBottomVerLeft / 2;

                    }
                    EndHorizontal();

                    //   listing.Gap(3f);
                    BeginHorizontal(hoverBox);
                    CBKF.SettingsColBar.UseCustomMarginBottomVerLeft = Toggle(CBKF.SettingsColBar.UseCustomMarginBottomVerLeft, "ColonistBarKF.SettingsColBar.MarginBottom".Translate(),  Width(colSize));
                    if (CBKF.SettingsColBar.UseCustomMarginBottomVerLeft)
                    {
                        //    listing.Gap(3f);
                        CBKF.SettingsColBar.MarginBottomVerLeft = HorizontalSlider(CBKF.SettingsColBar.MarginBottomVerLeft, 0f, Screen.height * 2 / 5, Width(colSize));
                    }
                    else
                    {
                        CBKF.SettingsColBar.MarginBottomVerLeft = 120f;
                        CBKF.SettingsColBar.MaxColonistBarHeight = Screen.height - CBKF.SettingsColBar.MarginTopVerLeft - CBKF.SettingsColBar.MarginBottomVerLeft;
                        CBKF.SettingsColBar.VerticalOffset = CBKF.SettingsColBar.MarginTopVerLeft / 2 - CBKF.SettingsColBar.MarginBottomVerLeft / 2;
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

                if (CBKF.SettingsColBar.UseBottomAlignment)
                {
                    BeginHorizontal(hoverBox);
                    CBKF.SettingsColBar.UseCustomMarginBottomHor = Toggle(CBKF.SettingsColBar.UseCustomMarginBottomHor, "ColonistBarKF.SettingsColBar.MarginEdge".Translate(),  Width(colSize));
                    if (CBKF.SettingsColBar.UseCustomMarginBottomHor)
                    {
                        //    listing.Gap(3f);
                        CBKF.SettingsColBar.MarginBottomHor = CBKF.SettingsColBar.MarginBottomHor = HorizontalSlider(CBKF.SettingsColBar.MarginBottomHor, 10, Screen.height / 12, Width(colSize));
                    }
                    else
                    {
                        CBKF.SettingsColBar.MarginBottomHor = 21f;
                    }
                    //   listing.Gap(3f);
                    EndHorizontal();
                    BeginHorizontal(hoverBox);
                    CBKF.SettingsColBar.UseCustomMarginLeftHorBottom = Toggle(CBKF.SettingsColBar.UseCustomMarginLeftHorBottom, "ColonistBarKF.SettingsColBar.MarginLeft".Translate(),  Width(colSize));
                    if (CBKF.SettingsColBar.UseCustomMarginLeftHorBottom)
                    {
                        //   listing.Gap(3f);
                        CBKF.SettingsColBar.MarginLeftHorBottom = HorizontalSlider(CBKF.SettingsColBar.MarginLeftHorBottom, 0f, Screen.width * 2 / 5, Width(colSize));
                    }
                    else
                    {
                        CBKF.SettingsColBar.MarginLeftHorBottom = 160f;
                        CBKF.SettingsColBar.MaxColonistBarWidth = Screen.width - CBKF.SettingsColBar.MarginLeftHorBottom - CBKF.SettingsColBar.MarginRightHorBottom;
                        CBKF.SettingsColBar.HorizontalOffset = CBKF.SettingsColBar.MarginLeftHorBottom / 2 - CBKF.SettingsColBar.MarginRightHorBottom / 2;
                    }
                    //  listing.Gap(3f);
                    EndHorizontal();
                    BeginHorizontal(hoverBox);
                    CBKF.SettingsColBar.UseCustomMarginRightHorBottom = Toggle(CBKF.SettingsColBar.UseCustomMarginRightHorBottom, "ColonistBarKF.SettingsColBar.MarginRight".Translate(),  Width(colSize));
                    if (CBKF.SettingsColBar.UseCustomMarginRightHorBottom)
                    {
                        //      listing.Gap(3f);
                        CBKF.SettingsColBar.MarginRightHorBottom = HorizontalSlider(CBKF.SettingsColBar.MarginRightHorBottom, 0f, Screen.width * 2 / 5, Width(colSize));
                    }
                    else
                    {
                        CBKF.SettingsColBar.MarginRightHorBottom = 160f;
                        CBKF.SettingsColBar.MaxColonistBarWidth = Screen.width - CBKF.SettingsColBar.MarginLeftHorBottom - CBKF.SettingsColBar.MarginRightHorBottom;
                        CBKF.SettingsColBar.HorizontalOffset = CBKF.SettingsColBar.MarginLeftHorBottom / 2 - CBKF.SettingsColBar.MarginRightHorBottom / 2;
                    }
                    EndHorizontal();
                    //    listing.Gap(3f);
                }
                else
                {
                    BeginHorizontal(hoverBox);
                    CBKF.SettingsColBar.UseCustomMarginTopHor = Toggle(CBKF.SettingsColBar.UseCustomMarginTopHor, "ColonistBarKF.SettingsColBar.MarginEdge".Translate(),  Width(colSize));
                    if (CBKF.SettingsColBar.UseCustomMarginTopHor)
                    {
                        //    listing.Gap(3f);
                        CBKF.SettingsColBar.MarginTopHor = HorizontalSlider(CBKF.SettingsColBar.MarginTopHor, 10, Screen.height / 12, Width(colSize));
                    }
                    else
                    {
                        CBKF.SettingsColBar.MarginTopHor = 21f;
                    }
                    //  listing.Gap(3f);
                    EndHorizontal();
                    BeginHorizontal(hoverBox);
                    CBKF.SettingsColBar.UseCustomMarginLeftHorTop = Toggle(CBKF.SettingsColBar.UseCustomMarginLeftHorTop, "ColonistBarKF.SettingsColBar.MarginLeft".Translate(),  Width(colSize));
                    if (CBKF.SettingsColBar.UseCustomMarginLeftHorTop)
                    {
                        //    listing.Gap(3f);
                        CBKF.SettingsColBar.MarginLeftHorTop = HorizontalSlider(CBKF.SettingsColBar.MarginLeftHorTop, 0f, Screen.width * 2 / 5, Width(colSize));
                    }
                    else
                    {
                        CBKF.SettingsColBar.MarginLeftHorTop = 160f;
                        CBKF.SettingsColBar.MaxColonistBarWidth = Screen.width - CBKF.SettingsColBar.MarginLeftHorTop - CBKF.SettingsColBar.MarginRightHorTop;
                        CBKF.SettingsColBar.HorizontalOffset = CBKF.SettingsColBar.MarginLeftHorTop / 2 - CBKF.SettingsColBar.MarginRightHorTop / 2;
                    }
                    EndHorizontal();
                    BeginHorizontal(hoverBox);
                    //  listing.Gap(3f);

                    CBKF.SettingsColBar.UseCustomMarginRightHorTop = Toggle(CBKF.SettingsColBar.UseCustomMarginRightHorTop, "ColonistBarKF.SettingsColBar.MarginRight".Translate(),  Width(colSize));
                    if (CBKF.SettingsColBar.UseCustomMarginRightHorTop)
                    {
                        //     listing.Gap(3f);
                        CBKF.SettingsColBar.MarginRightHorTop = HorizontalSlider(CBKF.SettingsColBar.MarginRightHorTop, 0f, Screen.width * 2 / 5, Width(colSize));
                    }
                    else
                    {
                        CBKF.SettingsColBar.MarginRightHorTop = 160f;
                        CBKF.SettingsColBar.MaxColonistBarWidth = Screen.width - CBKF.SettingsColBar.MarginLeftHorTop - CBKF.SettingsColBar.MarginRightHorTop;
                        CBKF.SettingsColBar.HorizontalOffset = CBKF.SettingsColBar.MarginLeftHorTop / 2 - CBKF.SettingsColBar.MarginRightHorTop / 2;
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
            CBKF.SettingsColBar.UseCustomIconSize = Toggle(CBKF.SettingsColBar.UseCustomIconSize, "ColonistBarKF.SettingsColBar.BasicSize".Translate(), Width(colSize));

            if (CBKF.SettingsColBar.UseCustomIconSize)
            {
                //     listing.Gap(3f);
                //    listing.Gap(3f);
                CBKF.SettingsColBar.BaseSizeFloat = HorizontalSlider(CBKF.SettingsColBar.BaseSizeFloat, 16f, 128f, Width(colSize));

                //     listing.Gap();
            }
            else
            {
                CBKF.SettingsColBar.BaseSizeFloat = 48f;
                CBKF.SettingsColBar.BaseIconSize = 20f;
                //     listing.Gap(3f);
            }
            EndHorizontal();
            CBKF.SettingsColBar.UseFixedIconScale = Toggle(CBKF.SettingsColBar.UseFixedIconScale, "ColonistBarKF.SettingsColBar.FixedScale".Translate(), Width(colSize));

            BeginHorizontal(hoverBox);
            CBKF.SettingsColBar.UseCustomBaseSpacingHorizontal = Toggle(CBKF.SettingsColBar.UseCustomBaseSpacingHorizontal, "ColonistBarKF.SettingsColBar.BaseSpacingHorizontal".Translate(), Width(colSize));
            if (CBKF.SettingsColBar.UseCustomBaseSpacingHorizontal)
            {
                //      listing.Gap(3f);
                CBKF.SettingsColBar.BaseSpacingHorizontal = HorizontalSlider(CBKF.SettingsColBar.BaseSpacingHorizontal, 1f, 72f, Width(colSize));
            }
            else
            {
                CBKF.SettingsColBar.BaseSpacingHorizontal = 24f;
                //      listing.Gap(3f);
            }
            EndHorizontal();
            BeginHorizontal(hoverBox);
            CBKF.SettingsColBar.UseCustomBaseSpacingVertical = Toggle(CBKF.SettingsColBar.UseCustomBaseSpacingVertical, "ColonistBarKF.SettingsColBar.BaseSpacingVertical".Translate(), Width(colSize));
            if (CBKF.SettingsColBar.UseCustomBaseSpacingVertical)
            {
                //      listing.Gap(3f);
                CBKF.SettingsColBar.BaseSpacingVertical = HorizontalSlider(CBKF.SettingsColBar.BaseSpacingVertical, 1f, 96f, Width(colSize));
            }
            else
            {
                CBKF.SettingsColBar.BaseSpacingVertical = 32f;
            }
            EndHorizontal();
            #endregion

            Space(12f);
            BeginHorizontal(hoverBox);
            CBKF.SettingsColBar.UseMoodColors = Toggle(CBKF.SettingsColBar.UseMoodColors, "ColonistBarKF.SettingsColBar.UseMoodColors".Translate(), Width(colSize));
            if (CBKF.SettingsColBar.UseMoodColors)
            {
                //      listing.Gap(3f);
                CBKF.SettingsColBar.moodRectScale = HorizontalSlider(CBKF.SettingsColBar.moodRectScale, 0.33f, 1f, Width(colSize));
            }
            EndHorizontal();
            Space(12f);
            CBKF.SettingsColBar.UseWeaponIcons = Toggle(CBKF.SettingsColBar.UseWeaponIcons, "ColonistBarKF.SettingsColBar.UseWeaponIcons".Translate());

            Space(12f);
            CBKF.SettingsColBar.UseGender = Toggle(CBKF.SettingsColBar.UseGender, "ColonistBarKF.SettingsColBar.useGender".Translate());
            Space(12f);
            CBKF.SettingsColBar.useZoomToMouse = Toggle(CBKF.SettingsColBar.useZoomToMouse, "ColonistBarKF.SettingsColBar.useZoomToMouse".Translate());

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
                if (GUILayout.Button("ColonistBarKF.SettingsColBar.ResetColors".Translate()))
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
            CBKF.SettingsColBar.UseCustomPawnTextureCameraZoom = Toggle(CBKF.SettingsColBar.UseCustomPawnTextureCameraZoom, "ColonistBarKF.SettingsColBar.PawnTextureCameraZoom".Translate(), Width(colSize));
            if (CBKF.SettingsColBar.UseCustomPawnTextureCameraZoom)
            {
                //    listing.Gap(3f);
                CBKF.SettingsColBar.PawnTextureCameraZoom = HorizontalSlider(CBKF.SettingsColBar.PawnTextureCameraZoom, 0.3f, 3f, Width(colSize));
            }
            else
            {
                CBKF.SettingsColBar.PawnTextureCameraZoom = 1.28205f;
            }
            //    listing.Gap(3f);
            EndHorizontal();
            BeginHorizontal(hoverBox);
            CBKF.SettingsColBar.UseCustomPawnTextureCameraHorizontalOffset = Toggle(CBKF.SettingsColBar.UseCustomPawnTextureCameraHorizontalOffset, "ColonistBarKF.SettingsColBar.PawnTextureCameraHorizontalOffset".Translate(), Width(colSize));
            if (CBKF.SettingsColBar.UseCustomPawnTextureCameraHorizontalOffset)
            {
                //        listing.Gap(3f);
                CBKF.SettingsColBar.PawnTextureCameraHorizontalOffset = HorizontalSlider(CBKF.SettingsColBar.PawnTextureCameraHorizontalOffset, 0.7f, -0.7f, Width(colSize));
            }
            else
            {
                CBKF.SettingsColBar.PawnTextureCameraHorizontalOffset = 0f;
            }
            EndHorizontal();

            BeginHorizontal(hoverBox);
            CBKF.SettingsColBar.UseCustomPawnTextureCameraVerticalOffset = Toggle(CBKF.SettingsColBar.UseCustomPawnTextureCameraVerticalOffset, "ColonistBarKF.SettingsColBar.PawnTextureCameraVerticalOffset".Translate(), Width(colSize));
            if (CBKF.SettingsColBar.UseCustomPawnTextureCameraVerticalOffset)
            {
                //       listing.Gap(3f);
                CBKF.SettingsColBar.PawnTextureCameraVerticalOffset = HorizontalSlider(CBKF.SettingsColBar.PawnTextureCameraVerticalOffset, 0f, 1f, Width(colSize));
            }
            else
            {
                CBKF.SettingsColBar.PawnTextureCameraVerticalOffset = 0.3f;
            }
            EndHorizontal();
            #endregion


            Space(12f);
            BeginHorizontal(hoverBox);
            CBKF.SettingsColBar.UseCustomDoubleClickTime = Toggle(CBKF.SettingsColBar.UseCustomDoubleClickTime, "ColonistBarKF.SettingsColBar.DoubleClickTime".Translate() + ": " + CBKF.SettingsColBar.DoubleClickTime.ToString("N2") + "s", Width(colSize));
            if (CBKF.SettingsColBar.UseCustomDoubleClickTime)
            {
                //       listing.Gap(3f);
                CBKF.SettingsColBar.DoubleClickTime = HorizontalSlider(CBKF.SettingsColBar.DoubleClickTime, 0.1f, 1.5f, Width(colSize));
            }
            else
            {
                CBKF.SettingsColBar.DoubleClickTime = 0.5f;
            }
            EndHorizontal();
            //       GUILayout.Toggle("ColonistBarKF.SettingsColBar.useExtraIcons".Translate(), ref SettingsColBar.useExtraIcons, null);
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
        public float SliderMaxBarWidth(Rect rect, float val, float min, float max)
        {
            GUI.skin.horizontalSlider.alignment = TextAnchor.MiddleCenter;
            float result = GUI.HorizontalSlider(rect, val, min, max);
            if (CBKF.SettingsColBar.UseBottomAlignment)
            {
                CBKF.SettingsColBar.MaxColonistBarWidth = Screen.width - CBKF.SettingsColBar.MarginLeftHorBottom - CBKF.SettingsColBar.MarginRightHorBottom;
                CBKF.SettingsColBar.HorizontalOffset = CBKF.SettingsColBar.MarginLeftHorBottom / 2 - CBKF.SettingsColBar.MarginRightHorBottom / 2;

            }
            else
            {
                CBKF.SettingsColBar.MaxColonistBarWidth = Screen.width - CBKF.SettingsColBar.MarginLeftHorTop - CBKF.SettingsColBar.MarginRightHorTop;
                CBKF.SettingsColBar.HorizontalOffset = CBKF.SettingsColBar.MarginLeftHorTop / 2 - CBKF.SettingsColBar.MarginRightHorTop / 2;

            }
            return result;
        }

        public float SliderMaxBarHeight(Rect rect, float val, float min, float max)
        {
            GUI.skin.horizontalSlider.alignment = TextAnchor.MiddleCenter;
            float result = GUI.HorizontalSlider(rect, val, min, max);
            if (CBKF.SettingsColBar.UseRightAlignment)
            {
                CBKF.SettingsColBar.MaxColonistBarHeight = Screen.height - CBKF.SettingsColBar.MarginTopVerRight - CBKF.SettingsColBar.MarginBottomVerRight;
                CBKF.SettingsColBar.VerticalOffset = CBKF.SettingsColBar.MarginTopVerRight / 2 - CBKF.SettingsColBar.MarginBottomVerRight / 2;
            }
            else
            {
                CBKF.SettingsColBar.MaxColonistBarHeight = Screen.height - CBKF.SettingsColBar.MarginTopVerLeft - CBKF.SettingsColBar.MarginBottomVerLeft;
                CBKF.SettingsColBar.VerticalOffset = CBKF.SettingsColBar.MarginTopVerLeft / 2 - CBKF.SettingsColBar.MarginBottomVerLeft / 2;
            }
            return result;
        }

        public float SliderBaseSize(Rect rect, float val, float min, float max)
        {
            GUI.skin.horizontalSlider.alignment = TextAnchor.MiddleCenter;
            float result = GUI.HorizontalSlider(rect, val, min, max);
            CBKF.SettingsColBar.BaseIconSize = CBKF.SettingsColBar.BaseSizeFloat / 2f - 4f;
            return result;
        }
#if !NoCCL
        private LabeledInput_Color femaleColorField = new LabeledInput_Color(barSettings.FemaleColor, "ColonistBarKF.SettingsColBar.FemaleColor".Translate());
        private LabeledInput_Color maleColorField = new LabeledInput_Color(barSettings.MaleColor, "ColonistBarKF.SettingsColBar.MaleColor".Translate());
#endif

        private void FillPagePSILoadIconset()
        {
            //  if (GUILayout.Button("PSI.SettingsColBar.IconSet".Translate() + PSI.SettingsColBar.IconSet))
            //   if (GUILayout.ButtonLabeled("PSI.SettingsColBar.IconSet".Translate() , PSI.SettingsColBar.IconSet))
            //   {
            //       var options = new List<FloatMenuOption>();
            //       foreach (var str in PSI.IconSets)
            //       {
            //           var setname = str;
            //           options.Add(new FloatMenuOption(setname, () =>
            //           {
            //               PSI.SettingsColBar.IconSet = setname;
            //               PSI.Materials = new Materials(setname);
            //               PSI.Materials.ReloadTextures(true);
            //           }));
            //       }
            //       Find.WindowStack.Add(new FloatMenu(options));
            //   }
            //   listing.NewColumn();

            //    if (GUILayout.ButtonLabeled("PSI.SettingsColBar.LoadPresetButton".Translate()))
            Label("PSI.SettingsColBar.LoadPresetButton".Translate(), FontBold);
            if (Button("PSI.SettingsColBar.IconSet".Translate() + SettingsPsi.IconSet))
            {
                var options = new List<FloatMenuOption>();

                options.Add(new FloatMenuOption("default_settings".Translate(), () =>
                {
                    try
                    {
                        SettingsPsi.IconSet = "default";
                        SavePsiSettings();
                        Reinit();
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.SettingsColBar.LoadPreset.UnableToLoad".Translate() + "default_settings");
                    }
                }));
                options.Add(new FloatMenuOption("original_settings".Translate(), () =>
                {
                    try
                    {
                        SettingsPsi.IconSet = "original";
                        SavePsiSettings();
                        Reinit();
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.SettingsColBar.LoadPreset.UnableToLoad".Translate() + "original_settings");
                    }
                }));
                options.Add(new FloatMenuOption("text_settings".Translate(), () =>
                {
                    try
                    {
                        SettingsPsi.IconSet = "text";
                        SavePsiSettings();
                        Reinit();
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.SettingsColBar.LoadPreset.UnableToLoad".Translate() + "text_settings");
                    }
                }));
                Find.WindowStack.Add(new FloatMenu(options));
            }


        }

        private void FillPagePSIShowHide()
        {
            Label("PSI.Settings.Visibility.Header", FontBold);

            SettingsPsi.ShowTargetPoint = Toggle(SettingsPsi.ShowTargetPoint, "PSI.Settings.Visibility.TargetPoint".Translate());
            SettingsPsi.ShowAggressive = Toggle(SettingsPsi.ShowAggressive, "PSI.Settings.Visibility.Aggressive".Translate());
            SettingsPsi.ShowDazed = Toggle(SettingsPsi.ShowDazed, "PSI.Settings.Visibility.Dazed".Translate());
            SettingsPsi.ShowLeave = Toggle(SettingsPsi.ShowLeave, "PSI.Settings.Visibility.Leave".Translate());
            SettingsPsi.ShowDraft = Toggle(SettingsPsi.ShowDraft, "PSI.Settings.Visibility.Draft".Translate());

            SettingsPsi.ShowIdle = Toggle(SettingsPsi.ShowIdle, "PSI.Settings.Visibility.Idle".Translate());
            SettingsPsi.ShowUnarmed = Toggle(SettingsPsi.ShowUnarmed, "PSI.Settings.Visibility.Unarmed".Translate());
            SettingsPsi.ShowHungry = Toggle(SettingsPsi.ShowHungry, "PSI.Settings.Visibility.Hungry".Translate());
            SettingsPsi.ShowSad = Toggle(SettingsPsi.ShowSad, "PSI.Settings.Visibility.Sad".Translate());
            SettingsPsi.ShowTired = Toggle(SettingsPsi.ShowTired, "PSI.Settings.Visibility.Tired".Translate());
            //
            SettingsPsi.ShowDisease = Toggle(SettingsPsi.ShowDisease, "PSI.Settings.Visibility.Sickness".Translate());
            SettingsPsi.ShowPain = Toggle(SettingsPsi.ShowPain, "PSI.Settings.Visibility.Pain".Translate());
            SettingsPsi.ShowHealth = Toggle(SettingsPsi.ShowHealth, "PSI.Settings.Visibility.Health".Translate());
            SettingsPsi.ShowEffectiveness = Toggle(SettingsPsi.ShowEffectiveness, "PSI.Settings.Visibility.Injury".Translate());
            SettingsPsi.ShowBloodloss = Toggle(SettingsPsi.ShowBloodloss, "PSI.Settings.Visibility.Bloodloss".Translate());
            //
            SettingsPsi.ShowHot = Toggle(SettingsPsi.ShowHot, "PSI.Settings.Visibility.Hot".Translate());
            SettingsPsi.ShowCold = Toggle(SettingsPsi.ShowCold, "PSI.Settings.Visibility.Cold".Translate());
            SettingsPsi.ShowNaked = Toggle(SettingsPsi.ShowNaked, "PSI.Settings.Visibility.Naked".Translate());
            SettingsPsi.ShowDrunk = Toggle(SettingsPsi.ShowDrunk, "PSI.Settings.Visibility.Drunk".Translate());
            SettingsPsi.ShowApparelHealth = Toggle(SettingsPsi.ShowApparelHealth, "PSI.Settings.Visibility.ApparelHealth".Translate());
            //
            SettingsPsi.ShowPacific = Toggle(SettingsPsi.ShowPacific, "PSI.Settings.Visibility.Pacific".Translate());
            SettingsPsi.ShowNightOwl = Toggle(SettingsPsi.ShowNightOwl, "PSI.Settings.Visibility.NightOwl".Translate());
            SettingsPsi.ShowGreedy = Toggle(SettingsPsi.ShowGreedy, "PSI.Settings.Visibility.Greedy".Translate());
            SettingsPsi.ShowJealous = Toggle(SettingsPsi.ShowJealous, "PSI.Settings.Visibility.Jealous".Translate());
            SettingsPsi.ShowLovers = Toggle(SettingsPsi.ShowLovers, "PSI.Settings.Visibility.Lovers".Translate());
            //
            SettingsPsi.ShowProsthophile = Toggle(SettingsPsi.ShowProsthophile, "PSI.Settings.Visibility.Prosthophile".Translate());
            SettingsPsi.ShowProsthophobe = Toggle(SettingsPsi.ShowProsthophobe, "PSI.Settings.Visibility.Prosthophobe".Translate());
            SettingsPsi.ShowRoomStatus = Toggle(SettingsPsi.ShowRoomStatus, "PSI.Settings.Visibility.RoomStatus".Translate());
            SettingsPsi.ShowBedroom = Toggle(SettingsPsi.ShowBedroom, "PSI.Settings.Visibility.Bedroom".Translate());

            SettingsPsi.ShowDeadColonists = Toggle(SettingsPsi.ShowDeadColonists, "PSI.Settings.Visibility.ShowDeadColonists".Translate());

            SettingsPsi.ShowPyromaniac = Toggle(SettingsPsi.ShowPyromaniac, "PSI.Settings.Visibility.Pyromaniac".Translate());
        }



        private void FillPagePSIOpacityAndColor()
        {
            Label("PSI.Settings.IconOpacityAndColor.Header".Translate(), FontBold);

            Label("PSI.Settings.IconOpacityAndColor.Opacity".Translate());
            SettingsPsi.IconOpacity = HorizontalSlider(SettingsPsi.IconOpacity, 0.05f, 1f);

            Label("PSI.Settings.IconOpacityAndColor.OpacityCritical".Translate());
            SettingsPsi.IconOpacityCritical = HorizontalSlider(SettingsPsi.IconOpacityCritical, 0f, 1f);

            Toggle(SettingsPsi.UseColoredTarget, "PSI.Settings.IconOpacityAndColor.UseColoredTarget".Translate());


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


            //  if (listing.DoTextButton("PSI.Settings.ReturnButton".Translate()))
            //      Page = "main";
        }

        private void FillPSIPageArrangement()
        {

            Label("PSI.Settings.Arrangement.Header", FontBold);

            if (Button("PSI.Settings.LoadPresetButton".Translate()))
            {
                var options = new List<FloatMenuOption>();

                options.Add(new FloatMenuOption("Left_Default)".Translate(), () =>
                {
                    try
                    {
                        SettingsPsi.IconDistanceX = 1f;
                        SettingsPsi.IconDistanceY = 1f;
                        SettingsPsi.IconOffsetX = 1f;
                        SettingsPsi.IconOffsetY = 1f;
                        SettingsPsi.IconsHorizontal = false;
                        SettingsPsi.IconsScreenScale = true;
                        SettingsPsi.IconsInColumn = 3;
                        SettingsPsi.IconSize = 1f;
                        SettingsPsi.IconOpacity = 0.7f;
                        SettingsPsi.IconOpacityCritical = 0.6f;
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.Settings.LoadPreset.UnableToLoad".Translate() + "Left");
                    }
                }));
                options.Add(new FloatMenuOption("Right".Translate(), () =>
                {
                    try
                    {
                        SettingsPsi.IconDistanceX = -1f;
                        SettingsPsi.IconDistanceY = 1f;
                        SettingsPsi.IconOffsetX = -1f;
                        SettingsPsi.IconOffsetY = 1f;
                        SettingsPsi.IconsHorizontal = false;
                        SettingsPsi.IconsScreenScale = true;
                        SettingsPsi.IconsInColumn = 3;
                        SettingsPsi.IconSize = 1f;
                        SettingsPsi.IconOpacity = 0.7f;
                        SettingsPsi.IconOpacityCritical = 0.6f;
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.Settings.LoadPreset.UnableToLoad".Translate() + "Right");
                    }
                }));

                options.Add(new FloatMenuOption("Top".Translate(), () =>
                {
                    try
                    {
                        SettingsPsi.IconDistanceX = 1f;
                        SettingsPsi.IconDistanceY = -1.63f;
                        SettingsPsi.IconOffsetX = -1f;
                        SettingsPsi.IconOffsetY = 1f;
                        SettingsPsi.IconsHorizontal = true;
                        SettingsPsi.IconsScreenScale = true;
                        SettingsPsi.IconsInColumn = 3;
                        SettingsPsi.IconSize = 1f;
                        SettingsPsi.IconOpacity = 0.7f;
                        SettingsPsi.IconOpacityCritical = 0.6f;
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.Settings.LoadPreset.UnableToLoad".Translate() + "Top");
                    }
                }));

                options.Add(new FloatMenuOption("Bottom".Translate(), () =>
                {
                    try
                    {
                        SettingsPsi.IconDistanceX = 1.139534f;
                        SettingsPsi.IconDistanceY = 1.375f;
                        SettingsPsi.IconOffsetX = -0.9534883f;
                        SettingsPsi.IconOffsetY = -0.9534884f;
                        SettingsPsi.IconsHorizontal = true;
                        SettingsPsi.IconsScreenScale = true;
                        SettingsPsi.IconsInColumn = 4;
                        SettingsPsi.IconSize = 1.084302f;
                        SettingsPsi.IconOpacity = 0.7f;
                        SettingsPsi.IconOpacityCritical = 0.6f;
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.Settings.LoadPreset.UnableToLoad".Translate() + "Bottom");
                    }
                }));

                Find.WindowStack.Add(new FloatMenu(options));
            }

            var num = (int)(SettingsPsi.IconSize * 4.5);

            if (num > 8)
                num = 8;
            else if (num < 0)
                num = 0;

            Label("PSI.Settings.Arrangement.IconSize".Translate() + ("PSI.Settings.SizeLabel." + num).Translate());
            SettingsPsi.IconSize = HorizontalSlider(SettingsPsi.IconSize, 0.5f, 2f);

            Label(string.Concat("PSI.Settings.Arrangement.IconPosition".Translate(), (int)(SettingsPsi.IconDistanceX * 100.0), " , ", (int)(SettingsPsi.IconDistanceY * 100.0)));
            SettingsPsi.IconDistanceX = HorizontalSlider(SettingsPsi.IconDistanceX, -2f, 2f);
            SettingsPsi.IconDistanceY = HorizontalSlider(SettingsPsi.IconDistanceY, -2f, 2f);

            Label(string.Concat("PSI.Settings.Arrangement.IconOffset".Translate(), (int)(SettingsPsi.IconOffsetX * 100.0), " , ", (int)(SettingsPsi.IconOffsetY * 100.0)));
            SettingsPsi.IconOffsetX = HorizontalSlider(SettingsPsi.IconOffsetX, -2f, 2f);
            SettingsPsi.IconOffsetY = HorizontalSlider(SettingsPsi.IconOffsetY, -2f, 2f);

            SettingsPsi.IconsHorizontal = Toggle(SettingsPsi.IconsHorizontal, "PSI.Settings.Arrangement.Horizontal".Translate());

            SettingsPsi.IconsScreenScale = Toggle(SettingsPsi.IconsScreenScale, "PSI.Settings.Arrangement.ScreenScale".Translate());

            Label("PSI.Settings.Arrangement.IconsPerColumn".Translate() + SettingsPsi.IconsInColumn);

            SettingsPsi.IconsInColumn = (int)HorizontalSlider(SettingsPsi.IconsInColumn, 1f, 7f);

            SavePsiSettings();
            Reinit();

            //   if (!listing.DoTextButton("PSI.Settings.ReturnButton".Translate()))
            //       return;
            //
            //   Page = "main";
        }


        private void FillPSIPageLimits()
        {

            Label("PSI.Settings.Sensitivity.Header".Translate(), FontBold);
            if (Button("PSI.Settings.LoadPresetButton".Translate()))
            {
                var options = new List<FloatMenuOption>();
                options.Add(new FloatMenuOption("Less Sensitive", () =>
                {
                    try
                    {
                        SettingsPsi.LimitBleedMult = 2f;
                        SettingsPsi.LimitDiseaseLess = 1f;
                        SettingsPsi.LimitEfficiencyLess = 0.28f;
                        SettingsPsi.LimitFoodLess = 0.2f;
                        SettingsPsi.LimitMoodLess = 0.2f;
                        SettingsPsi.LimitRestLess = 0.2f;
                        SettingsPsi.LimitApparelHealthLess = 0.5f;
                        SettingsPsi.LimitTempComfortOffset = 3f;
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
                        SettingsPsi.LimitBleedMult = 3f;
                        SettingsPsi.LimitDiseaseLess = 1f;
                        SettingsPsi.LimitEfficiencyLess = 0.33f;
                        SettingsPsi.LimitFoodLess = 0.25f;
                        SettingsPsi.LimitMoodLess = 0.25f;
                        SettingsPsi.LimitRestLess = 0.25f;
                        SettingsPsi.LimitApparelHealthLess = 0.5f;
                        SettingsPsi.LimitTempComfortOffset = 0f;
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
                        SettingsPsi.LimitBleedMult = 4f;
                        SettingsPsi.LimitDiseaseLess = 1f;
                        SettingsPsi.LimitEfficiencyLess = 0.45f;
                        SettingsPsi.LimitFoodLess = 0.3f;
                        SettingsPsi.LimitMoodLess = 0.3f;
                        SettingsPsi.LimitRestLess = 0.3f;
                        SettingsPsi.LimitApparelHealthLess = 0.5f;
                        SettingsPsi.LimitTempComfortOffset = -3f;
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.Settings.LoadPreset.UnableToLoad".Translate() + "More Sensitive");
                    }
                }));

                Find.WindowStack.Add(new FloatMenu(options));
            }


            Label("PSI.Settings.Sensitivity.Bleeding".Translate() + ("PSI.Settings.Sensitivity.Bleeding." + Math.Round(SettingsPsi.LimitBleedMult - 0.25)).Translate());
            SettingsPsi.LimitBleedMult = HorizontalSlider(SettingsPsi.LimitBleedMult, 0.5f, 5f);

            Label("PSI.Settings.Sensitivity.Injured".Translate() + (int)(SettingsPsi.LimitEfficiencyLess * 100.0) + "%");
            SettingsPsi.LimitEfficiencyLess = HorizontalSlider(SettingsPsi.LimitEfficiencyLess, 0.01f, 0.99f);

            Label("PSI.Settings.Sensitivity.Food".Translate() + (int)(SettingsPsi.LimitFoodLess * 100.0) + "%");
            SettingsPsi.LimitFoodLess = HorizontalSlider(SettingsPsi.LimitFoodLess, 0.01f, 0.99f);

            Label("PSI.Settings.Sensitivity.Mood".Translate() + (int)(SettingsPsi.LimitMoodLess * 100.0) + "%");
            SettingsPsi.LimitMoodLess = HorizontalSlider(SettingsPsi.LimitMoodLess, 0.01f, 0.99f);

            Label("PSI.Settings.Sensitivity.Rest".Translate() + (int)(SettingsPsi.LimitRestLess * 100.0) + "%");
            SettingsPsi.LimitRestLess = HorizontalSlider(SettingsPsi.LimitRestLess, 0.01f, 0.99f);

            Label("PSI.Settings.Sensitivity.ApparelHealth".Translate() + (int)(SettingsPsi.LimitApparelHealthLess * 100.0) + "%");
            SettingsPsi.LimitApparelHealthLess = HorizontalSlider(SettingsPsi.LimitApparelHealthLess, 0.01f, 0.99f);

            Label("PSI.Settings.Sensitivity.Temperature".Translate() + (int)SettingsPsi.LimitTempComfortOffset + "C");
            SettingsPsi.LimitTempComfortOffset = HorizontalSlider(SettingsPsi.LimitTempComfortOffset, -10f, 10f);

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
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseCustomMarginTopHor, "useCustomMarginTopHor", false);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseCustomMarginBottomHor, "UseCustomMarginBottomHor", false);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseCustomMarginLeftHorTop, "useCustomMarginLeftHor", false);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseCustomMarginRightHorTop, "useCustomMarginRightHor", false);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseCustomMarginLeftHorBottom, "UseCustomMarginLeftHorBottom", false);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseCustomMarginRightHorBottom, "UseCustomMarginRightHorBottom", false);

            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseCustomMarginTopVerLeft, "UseCustomMarginTopVerLeft", false);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseCustomMarginTopVerRight, "UseCustomMarginTopVerRight", false);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseCustomMarginLeftVer, "useCustomMarginLeftRightVer", false);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseCustomMarginRightVer, "UseCustomMarginRightVer", false);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseCustomMarginBottomVerLeft, "UseCustomMarginBottomVerLeft", false);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseCustomMarginBottomVerRight, "UseCustomMarginBottomVerRight", false);


            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseCustomBaseSpacingHorizontal, "useCustomBaseSpacingHorizontal", false);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseCustomBaseSpacingVertical, "useCustomBaseSpacingVertical", false);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseCustomIconSize, "useCustomIconSize", false);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseFixedIconScale, "useFixedIconScale", false);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseCustomPawnTextureCameraHorizontalOffset, "UseCustomPawnTextureCameraHorizontalOffset", false);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseCustomPawnTextureCameraVerticalOffset, "useCustomPawnTextureCameraVerticalOffset", false);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseCustomPawnTextureCameraZoom, "useCustomPawnTextureCameraZoom", false);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseCustomDoubleClickTime, "useCustomDoubleClick", false);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseGender, "useGender", false);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseVerticalAlignment, "useVerticalAlignment", false);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseRightAlignment, "useRightAlignment", false);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseBottomAlignment, "useBottomAlignment", false);

            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseMoodColors, "UseMoodColors", false);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.UseWeaponIcons, "UseWeaponIcons", false);

            Scribe_Values.LookValue(ref CBKF.SettingsColBar.MarginTopHor, "MarginTopHor", 21f);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.MarginBottomHor, "MarginBottomHor", 21f);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.MarginLeftHorTop, "MarginLeftHorTop", 160f);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.MarginRightHorTop, "MarginRightHorTop", 160f);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.MarginLeftHorBottom, "MarginLeftHorBottom", 160f);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.MarginRightHorBottom, "MarginRightHorBottom", 160f);

            Scribe_Values.LookValue(ref CBKF.SettingsColBar.MarginTopVerLeft, "MarginTopVerLeft", 120f);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.MarginBottomVerLeft, "MarginBottomVerLeft", 120f);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.MarginTopVerRight, "MarginTopVerRight", 120f);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.MarginBottomVerRight, "MarginBottomVerRight", 120f);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.MarginLeftVer, "MarginLeftVer", 21f);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.MarginRightVer, "MarginRightVer", 21f);

            Scribe_Values.LookValue(ref CBKF.SettingsColBar.HorizontalOffset, "HorizontalOffset", 0f);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.VerticalOffset, "VerticalOffset", 0f);


            Scribe_Values.LookValue(ref CBKF.SettingsColBar.BaseSpacingHorizontal, "BaseSpacingHorizontal", 24f);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.BaseSpacingVertical, "BaseSpacingVertical", 32f);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.BaseSizeFloat, "BaseSizeFloat", 48f);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.BaseIconSize, "BaseIconSize", 20f);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.PawnTextureCameraHorizontalOffset, "PawnTextureCameraHorizontalOffset", 0f);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.PawnTextureCameraVerticalOffset, "PawnTextureCameraVerticalOffset", 0.3f);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.PawnTextureCameraZoom, "PawnTextureCameraZoom", 1.28205f);
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.MaxColonistBarWidth, "MaxColonistBarWidth");
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.MaxColonistBarHeight, "MaxColonistBarHeight");


            Scribe_Values.LookValue(ref CBKF.SettingsColBar.DoubleClickTime, "DoubleClickTime", 0.5f);

            Scribe_Values.LookValue(ref CBKF.SettingsColBar.FemaleColor, "FemaleColor");
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.MaleColor, "MaleColor");

            Scribe_Values.LookValue(ref CBKF.SettingsColBar.MaxRows, "MaxRows");
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.SortBy, "SortBy");
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.useZoomToMouse, "useZoomToMouse");
            Scribe_Values.LookValue(ref CBKF.SettingsColBar.moodRectScale, "moodRectScale");


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
