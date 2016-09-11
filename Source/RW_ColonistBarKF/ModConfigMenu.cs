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

        public override void WindowUpdate()
        {
            ColonistBar_KF.BaseSize.x = ColSettingsColBar.BaseSizeFloat;
            ColonistBar_KF.BaseSize.y = ColSettingsColBar.BaseSizeFloat;
            ColonistBar_KF.PawnTextureSize.x = ColSettingsColBar.BaseSizeFloat - 2f;
            ColonistBar_KF.PawnTextureSize.y = ColSettingsColBar.BaseSizeFloat * 1.5f;

            if (ColSettingsColBar.UseGender)
                ColonistBarTextures.BGTex = ColonistBarTextures.BGTexGrey;
            else
            {
                ColonistBarTextures.BGTex = ColonistBarTextures.BGTexVanilla;
            }
        }

        public override void PreOpen()
        {
            base.PreOpen();
        }




#if NoCCL
        public override void PreClose()
        {
            SaveBarSettings();
        }

        public override Vector2 InitialSize
        {
            get { return new Vector2(438f, 640f); }
        }
        private int mainToolbarInt = 0;
        private int psiToolbarInt = 0;
        private int barPositionInt = 0;

        public string[] mainToolbarStrings =
            {
            "ColonistBarSettings".Translate(),
            "SettingsPSI".Translate()
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

        public int MainToolbarInt
        {
            get
            {
                LoadBarSettings();
                LoadPsiSettings();
                return mainToolbarInt;
            }

            set
            {
                SaveBarSettings();
                SavePsiSettings();
                mainToolbarInt = value;
            }
        }

        public int BarPositionInt
        {
            get
            {
                LoadBarSettings();
                LoadPsiSettings();
                if (!ColSettingsColBar.UseBottomAlignment && !ColSettingsColBar.UseVerticalAlignment && !ColSettingsColBar.UseRightAlignment)
                {
                    barPositionInt = 0;
                }
                if (ColSettingsColBar.UseBottomAlignment && !ColSettingsColBar.UseVerticalAlignment && !ColSettingsColBar.UseRightAlignment)
                {
                    barPositionInt = 1;
                }
                if (!ColSettingsColBar.UseBottomAlignment && ColSettingsColBar.UseVerticalAlignment && !ColSettingsColBar.UseRightAlignment)
                {
                    barPositionInt = 2;
                }
                if (!ColSettingsColBar.UseBottomAlignment && ColSettingsColBar.UseVerticalAlignment && ColSettingsColBar.UseRightAlignment)
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
                        ColSettingsColBar.UseBottomAlignment = false;
                        ColSettingsColBar.UseVerticalAlignment = false;
                        ColSettingsColBar.UseRightAlignment = false;
                        break;
                    case 1:
                        ColSettingsColBar.UseBottomAlignment = true;
                        ColSettingsColBar.UseVerticalAlignment = false;
                        ColSettingsColBar.UseRightAlignment = false; break;
                    case 2:
                        ColSettingsColBar.UseBottomAlignment = false;
                        ColSettingsColBar.UseVerticalAlignment = true;
                        ColSettingsColBar.UseRightAlignment = false; break;
                    case 3:
                        ColSettingsColBar.UseBottomAlignment = false;
                        ColSettingsColBar.UseVerticalAlignment = true;
                        ColSettingsColBar.UseRightAlignment = true; break;
                    default:
                        ColSettingsColBar.UseBottomAlignment = false;
                        ColSettingsColBar.UseVerticalAlignment = false;
                        ColSettingsColBar.UseRightAlignment = false;
                        break;
                }
                SaveBarSettings();
                SavePsiSettings();
                barPositionInt = value;
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
            padding = new RectOffset(0, 0, 12, 6)
        };
        public ColonistBarKF_Settings()
        {
            forcePause = true;
            doCloseX = true;
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

            GUILayout.Label("Colonist Bar KF 0.15.3", Headline);
            GUILayout.Space(6f);
            GUILayout.BeginVertical(GUILayout.Width(viewRect.width), GUILayout.Height(viewRect.height));
            MainToolbarInt = GUILayout.Toolbar(MainToolbarInt, mainToolbarStrings, GUILayout.Width(viewRect.width));

            switch (MainToolbarInt)
            {
                case 0:
                    {
                        GUILayout.Label("ColonistBarKF.SettingsColBar.Position".Translate(), FontBold);
                        BarPositionInt = GUILayout.Toolbar(BarPositionInt, psiPositionStrings, GUILayout.Width(viewRect.width));
                        GUILayout.Space(18f);
                        FillPagePosition();
                        GUILayout.Space(18f);
                        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(viewRect.width));
                        FillPageOptions();

                    }
                    break;
                case 1:
                    {
                        GUILayout.Label("ColonistBarKF.SettingsPSI.Settings".Translate(), FontBold);
                        PSIToolbarInt = GUILayout.Toolbar(PSIToolbarInt, psiToolbarStrings, GUILayout.Width(viewRect.width));
                        GUILayout.Space(18f);
                        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(viewRect.width));

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
            GUILayout.Space(12);
            GUILayout.EndScrollView();

            GUILayout.FlexibleSpace();
            GUILayout.Space(12f);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("ColonistBarKF.SettingsColBar.RevertSettings".Translate(), GUILayout.Width(viewRect.width / 2 - 10f)))
            {
                ResetBarSettings();
            }
            if (GUILayout.Button("ColonistBarKF.SettingsColBar.RevertPSISettings".Translate(), GUILayout.Width(viewRect.width / 2 - 10f)))
            {
                ResetPSISettings();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

#if !NoCCL
            return 1000f;
#endif
        }

        private void ResetBarSettings()
        {
            // forces the mod to update the values from the sliders. is deactivated by saving the values.
            //ColSettingsColBar.Reloadsettings = true;

            {
                ColSettingsColBar.UseGender = false;
                ColSettingsColBar.UseCustomMarginTopHor = false;
                ColSettingsColBar.UseCustomBaseSpacingHorizontal = false;
                ColSettingsColBar.UseCustomBaseSpacingVertical = false;
                ColSettingsColBar.UseCustomIconSize = false;
                ColSettingsColBar.UseCustomPawnTextureCameraHorizontalOffset = false;
                ColSettingsColBar.UseCustomPawnTextureCameraVerticalOffset = false;
                ColSettingsColBar.UseCustomPawnTextureCameraZoom = false;
                ColSettingsColBar.UseCustomMarginLeftHorTop = false;
                ColSettingsColBar.UseCustomMarginRightHorTop = false;
                ColSettingsColBar.UseCustomMarginLeftHorBottom = false;
                ColSettingsColBar.UseCustomMarginRightHorBottom = false;
                ColSettingsColBar.UseBottomAlignment = false;
                ColSettingsColBar.UseMoodColors = false;
                ColSettingsColBar.UseWeaponIcons = false;
                ColSettingsColBar.UseFixedIconScale = false;

                ColSettingsColBar.MarginBottomHor = 21f;
                ColSettingsColBar.BaseSpacingHorizontal = 24f;
                ColSettingsColBar.BaseSpacingVertical = 32f;
                ColSettingsColBar.BaseSizeFloat = 48f;
                ColSettingsColBar.PawnTextureCameraHorizontalOffset = 0f;
                ColSettingsColBar.PawnTextureCameraVerticalOffset = 0.3f;
                ColonistBar_KF.PawnTextureCameraOffset = new Vector3(0f, 0f, 0.3f);
                ColSettingsColBar.PawnTextureCameraZoom = 1.28205f;
                ColSettingsColBar.MaxColonistBarWidth = Screen.width - 320f;
                ColSettingsColBar.FemaleColor = new Color(1f, 0.64f, 0.8f, 1f);
                ColSettingsColBar.MaleColor = new Color(0.52f, 0.75f, 0.92f, 1f);
#if !NoCCL
                femaleColorField.Value = new Color(1f, 0.64f, 0.8f, 1f);
                maleColorField.Value = new Color(0.52f, 0.75f, 0.92f, 1f);
#endif
                ColSettingsColBar.HorizontalOffset = 0f;
                ColSettingsColBar.VerticalOffset = 0f;
                ColSettingsColBar.UseCustomBaseSpacingVertical = false;
                ColSettingsColBar.UseVerticalAlignment = false;
                ColSettingsColBar.BaseSpacingVertical = 32f;
                ColSettingsColBar.MaxColonistBarHeight = Screen.height - 240f;
                ColSettingsColBar.UseRightAlignment = false;
                ColSettingsColBar.MarginLeftHorTop = 180f;
                ColSettingsColBar.MarginRightHorTop = 180f;
                ColSettingsColBar.MarginLeftHorBottom = 180f;
                ColSettingsColBar.MarginRightHorBottom = 180f;
                ColSettingsColBar.UseCustomDoubleClickTime = false;
                ColSettingsColBar.DoubleClickTime = 0.5f;
                ColSettingsColBar.UseCustomMarginLeftVer = false;
                ColSettingsColBar.MarginLeftVer = 21f;
                ColSettingsColBar.UseCustomMarginTopVerLeft = false;
                ColSettingsColBar.MarginTopVerLeft = 120f;
                ColSettingsColBar.UseCustomMarginBottomVerLeft = false;
                ColSettingsColBar.MarginBottomVerLeft = 120f;

                ColSettingsColBar.UseCustomMarginTopHor = false;
                ColSettingsColBar.UseCustomMarginBottomHor = false;
                ColSettingsColBar.UseCustomMarginLeftHorTop = false;
                ColSettingsColBar.UseCustomMarginRightHorTop = false;

                ColSettingsColBar.UseCustomMarginTopVerLeft = false;
                ColSettingsColBar.UseCustomMarginTopVerRight = false;
                ColSettingsColBar.UseCustomMarginLeftVer = false;
                ColSettingsColBar.UseCustomMarginRightVer = false;
                ColSettingsColBar.UseCustomMarginBottomVerLeft = false;
                ColSettingsColBar.UseCustomMarginBottomVerRight = false;
                ColSettingsColBar.SortBy = vanilla;
                ColSettingsColBar.useZoomToMouse = false;
                ColSettingsColBar.moodRectScale = 0.33f;
            }
        }

        private void ResetPSISettings()
        {
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

        private void FillPagePosition()
        {
            #region Vertical Alignment

            if (ColSettingsColBar.UseVerticalAlignment)
            {
#if !NoCCL
                listing.Indent();
#endif
                if (ColSettingsColBar.UseRightAlignment)
                {
                    ColSettingsColBar.UseCustomMarginRightVer = GUILayout.Toggle(ColSettingsColBar.UseCustomMarginRightVer, "ColonistBarKF.SettingsColBar.MarginEdge".Translate());
                    if (ColSettingsColBar.UseCustomMarginRightVer)
                    {
                        //    listing.Gap(3f);
                        ColSettingsColBar.MarginRightVer = GUILayout.HorizontalSlider(ColSettingsColBar.MarginRightVer, 0f, Screen.width / 12);
                    }
                    else
                    {
                        ColSettingsColBar.MarginRightVer = 21f;
                    }
                    // listing.Gap(3f);

                    ColSettingsColBar.UseCustomMarginTopVerRight = GUILayout.Toggle(ColSettingsColBar.UseCustomMarginTopVerRight, "ColonistBarKF.SettingsColBar.MarginTop".Translate());
                    if (ColSettingsColBar.UseCustomMarginTopVerRight)
                    {
                        ColSettingsColBar.MarginTopVerRight = GUILayout.HorizontalSlider(ColSettingsColBar.MarginTopVerRight, 0f, Screen.height * 2 / 5);
                    }
                    else
                    {
                        ColSettingsColBar.MarginTopVerRight = 120f;
                        ColSettingsColBar.MaxColonistBarHeight = Screen.height - ColSettingsColBar.MarginTopVerRight - ColSettingsColBar.MarginBottomVerRight;
                        ColSettingsColBar.VerticalOffset = ColSettingsColBar.MarginTopVerRight / 2 - ColSettingsColBar.MarginBottomVerRight / 2;

                    }
                    //  listing.Gap(3f);
                    ColSettingsColBar.UseCustomMarginBottomVerRight = GUILayout.Toggle(ColSettingsColBar.UseCustomMarginBottomVerRight, "ColonistBarKF.SettingsColBar.MarginBottom".Translate());
                    if (ColSettingsColBar.UseCustomMarginBottomVerRight)
                    {
                        //     listing.Gap(3f);
                        ColSettingsColBar.MarginBottomVerRight = GUILayout.HorizontalSlider(ColSettingsColBar.MarginBottomVerRight, 0f, Screen.height * 2 / 5);
                    }
                    else
                    {
                        ColSettingsColBar.MarginBottomVerRight = 120f;
                        ColSettingsColBar.MaxColonistBarHeight = Screen.height - ColSettingsColBar.MarginTopVerRight - ColSettingsColBar.MarginBottomVerRight;
                        ColSettingsColBar.VerticalOffset = ColSettingsColBar.MarginTopVerRight / 2 - ColSettingsColBar.MarginBottomVerRight / 2;
                    }
                }
                else
                {
                    ColSettingsColBar.UseCustomMarginLeftVer = GUILayout.Toggle(ColSettingsColBar.UseCustomMarginLeftVer, "ColonistBarKF.SettingsColBar.MarginEdge".Translate());
                    if (ColSettingsColBar.UseCustomMarginLeftVer)
                    {
                        //     listing.Gap(3f);
                        ColSettingsColBar.MarginLeftVer = GUILayout.HorizontalSlider(ColSettingsColBar.MarginLeftVer, 0f, Screen.width / 12);
                    }
                    else
                    {
                        ColSettingsColBar.MarginLeftVer = 21f;
                    }
                    //   listing.Gap(3f);

                    ColSettingsColBar.UseCustomMarginTopVerLeft = GUILayout.Toggle(ColSettingsColBar.UseCustomMarginTopVerLeft, "ColonistBarKF.SettingsColBar.MarginTop".Translate());
                    if (ColSettingsColBar.UseCustomMarginTopVerLeft)
                    {
                        //    listing.Gap(3f);
                        ColSettingsColBar.MarginTopVerLeft = GUILayout.HorizontalSlider(ColSettingsColBar.MarginTopVerLeft, 0f, Screen.height * 2 / 5);
                    }
                    else
                    {
                        ColSettingsColBar.MarginTopVerLeft = 120f;
                        ColSettingsColBar.MaxColonistBarHeight = Screen.height - ColSettingsColBar.MarginTopVerLeft - ColSettingsColBar.MarginBottomVerLeft;
                        ColSettingsColBar.VerticalOffset = ColSettingsColBar.MarginTopVerLeft / 2 - ColSettingsColBar.MarginBottomVerLeft / 2;

                    }
                    //   listing.Gap(3f);

                    ColSettingsColBar.UseCustomMarginBottomVerLeft = GUILayout.Toggle(ColSettingsColBar.UseCustomMarginBottomVerLeft, "ColonistBarKF.SettingsColBar.MarginBottom".Translate());
                    if (ColSettingsColBar.UseCustomMarginBottomVerLeft)
                    {
                        //    listing.Gap(3f);
                        ColSettingsColBar.MarginBottomVerLeft = GUILayout.HorizontalSlider(ColSettingsColBar.MarginBottomVerLeft, 0f, Screen.height * 2 / 5);
                    }
                    else
                    {
                        ColSettingsColBar.MarginBottomVerLeft = 120f;
                        ColSettingsColBar.MaxColonistBarHeight = Screen.height - ColSettingsColBar.MarginTopVerLeft - ColSettingsColBar.MarginBottomVerLeft;
                        ColSettingsColBar.VerticalOffset = ColSettingsColBar.MarginTopVerLeft / 2 - ColSettingsColBar.MarginBottomVerLeft / 2;
                    }

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

                if (ColSettingsColBar.UseBottomAlignment)
                {
                    ColSettingsColBar.UseCustomMarginBottomHor = GUILayout.Toggle(ColSettingsColBar.UseCustomMarginBottomHor, "ColonistBarKF.SettingsColBar.MarginEdge".Translate());
                    if (ColSettingsColBar.UseCustomMarginBottomHor)
                    {
                        //    listing.Gap(3f);
                        ColSettingsColBar.MarginBottomHor = ColSettingsColBar.MarginBottomHor = GUILayout.HorizontalSlider(ColSettingsColBar.MarginBottomHor, 10, Screen.height / 12);
                    }
                    else
                    {
                        ColSettingsColBar.MarginBottomHor = 21f;
                    }
                    //   listing.Gap(3f);


                    ColSettingsColBar.UseCustomMarginLeftHorBottom = GUILayout.Toggle(ColSettingsColBar.UseCustomMarginLeftHorBottom, "ColonistBarKF.SettingsColBar.MarginLeft".Translate());
                    if (ColSettingsColBar.UseCustomMarginLeftHorBottom)
                    {
                        //   listing.Gap(3f);
                        ColSettingsColBar.MarginLeftHorBottom = GUILayout.HorizontalSlider(ColSettingsColBar.MarginLeftHorBottom, 0f, Screen.width * 2 / 5);
                    }
                    else
                    {
                        ColSettingsColBar.MarginLeftHorBottom = 160f;
                        ColSettingsColBar.MaxColonistBarWidth = Screen.width - ColSettingsColBar.MarginLeftHorBottom - ColSettingsColBar.MarginRightHorBottom;
                        ColSettingsColBar.HorizontalOffset = ColSettingsColBar.MarginLeftHorBottom / 2 - ColSettingsColBar.MarginRightHorBottom / 2;
                    }
                    //  listing.Gap(3f);

                    ColSettingsColBar.UseCustomMarginRightHorBottom = GUILayout.Toggle(ColSettingsColBar.UseCustomMarginRightHorBottom, "ColonistBarKF.SettingsColBar.MarginRight".Translate());
                    if (ColSettingsColBar.UseCustomMarginRightHorBottom)
                    {
                        //      listing.Gap(3f);
                        ColSettingsColBar.MarginRightHorBottom = GUILayout.HorizontalSlider(ColSettingsColBar.MarginRightHorBottom, 0f, Screen.width * 2 / 5);
                    }
                    else
                    {
                        ColSettingsColBar.MarginRightHorBottom = 160f;
                        ColSettingsColBar.MaxColonistBarWidth = Screen.width - ColSettingsColBar.MarginLeftHorBottom - ColSettingsColBar.MarginRightHorBottom;
                        ColSettingsColBar.HorizontalOffset = ColSettingsColBar.MarginLeftHorBottom / 2 - ColSettingsColBar.MarginRightHorBottom / 2;
                    }
                    //    listing.Gap(3f);
                }
                else
                {
                    ColSettingsColBar.UseCustomMarginTopHor = GUILayout.Toggle(ColSettingsColBar.UseCustomMarginTopHor, "ColonistBarKF.SettingsColBar.MarginEdge".Translate());
                    if (ColSettingsColBar.UseCustomMarginTopHor)
                    {
                        //    listing.Gap(3f);
                        ColSettingsColBar.MarginTopHor = GUILayout.HorizontalSlider(ColSettingsColBar.MarginTopHor, 10, Screen.height / 12);
                    }
                    else
                    {
                        ColSettingsColBar.MarginTopHor = 21f;
                    }
                    //  listing.Gap(3f);


                    ColSettingsColBar.UseCustomMarginLeftHorTop = GUILayout.Toggle(ColSettingsColBar.UseCustomMarginLeftHorTop, "ColonistBarKF.SettingsColBar.MarginLeft".Translate());
                    if (ColSettingsColBar.UseCustomMarginLeftHorTop)
                    {
                        //    listing.Gap(3f);
                        ColSettingsColBar.MarginLeftHorTop = GUILayout.HorizontalSlider(ColSettingsColBar.MarginLeftHorTop, 0f, Screen.width * 2 / 5);
                    }
                    else
                    {
                        ColSettingsColBar.MarginLeftHorTop = 160f;
                        ColSettingsColBar.MaxColonistBarWidth = Screen.width - ColSettingsColBar.MarginLeftHorTop - ColSettingsColBar.MarginRightHorTop;
                        ColSettingsColBar.HorizontalOffset = ColSettingsColBar.MarginLeftHorTop / 2 - ColSettingsColBar.MarginRightHorTop / 2;
                    }
                    //  listing.Gap(3f);

                    ColSettingsColBar.UseCustomMarginRightHorTop = GUILayout.Toggle(ColSettingsColBar.UseCustomMarginRightHorTop, "ColonistBarKF.SettingsColBar.MarginRight".Translate());
                    if (ColSettingsColBar.UseCustomMarginRightHorTop)
                    {
                        //     listing.Gap(3f);
                        ColSettingsColBar.MarginRightHorTop = GUILayout.HorizontalSlider(ColSettingsColBar.MarginRightHorTop, 0f, Screen.width * 2 / 5);
                    }
                    else
                    {
                        ColSettingsColBar.MarginRightHorTop = 160f;
                        ColSettingsColBar.MaxColonistBarWidth = Screen.width - ColSettingsColBar.MarginLeftHorTop - ColSettingsColBar.MarginRightHorTop;
                        ColSettingsColBar.HorizontalOffset = ColSettingsColBar.MarginLeftHorTop / 2 - ColSettingsColBar.MarginRightHorTop / 2;
                    }
                    //  listing.Gap(3f);
                }
#if !NoCCL
                listing.Undent();
#endif
            }
            #endregion

            GUILayout.Space(12f);
        }

        private void FillPageOptions()
        {
            // forces the mod to update the values from the sliders. is deactivated by saving the values.


            #region Size + Spacing

            ColSettingsColBar.UseCustomIconSize = GUILayout.Toggle(ColSettingsColBar.UseCustomIconSize, "ColonistBarKF.SettingsColBar.BasicSize".Translate());

            if (ColSettingsColBar.UseCustomIconSize)
            {
                //     listing.Gap(3f);
                ColSettingsColBar.UseFixedIconScale = GUILayout.Toggle(ColSettingsColBar.UseFixedIconScale, "ColonistBarKF.SettingsColBar.FixedScale".Translate());
                //    listing.Gap(3f);
                ColSettingsColBar.BaseSizeFloat = GUILayout.HorizontalSlider(ColSettingsColBar.BaseSizeFloat, 16f, 128f);

                //     listing.Gap();
            }
            else
            {
                ColSettingsColBar.BaseSizeFloat = 48f;
                ColSettingsColBar.BaseIconSize = 20f;
                //     listing.Gap(3f);
            }


            ColSettingsColBar.UseCustomBaseSpacingHorizontal = GUILayout.Toggle(ColSettingsColBar.UseCustomBaseSpacingHorizontal, "ColonistBarKF.SettingsColBar.BaseSpacingHorizontal".Translate());
            if (ColSettingsColBar.UseCustomBaseSpacingHorizontal)
            {
                //      listing.Gap(3f);
                ColSettingsColBar.BaseSpacingHorizontal = GUILayout.HorizontalSlider(ColSettingsColBar.BaseSpacingHorizontal, 1f, 72f);
            }
            else
            {
                ColSettingsColBar.BaseSpacingHorizontal = 24f;
                //      listing.Gap(3f);
            }

            ColSettingsColBar.UseCustomBaseSpacingVertical = GUILayout.Toggle(ColSettingsColBar.UseCustomBaseSpacingVertical, "ColonistBarKF.SettingsColBar.BaseSpacingVertical".Translate());
            if (ColSettingsColBar.UseCustomBaseSpacingVertical)
            {
                //      listing.Gap(3f);
                ColSettingsColBar.BaseSpacingVertical = GUILayout.HorizontalSlider(ColSettingsColBar.BaseSpacingVertical, 1f, 96f);
            }
            else
            {
                ColSettingsColBar.BaseSpacingVertical = 32f;
            }

            #endregion

            GUILayout.Space(12f);
            ColSettingsColBar.UseMoodColors = GUILayout.Toggle(ColSettingsColBar.UseMoodColors, "ColonistBarKF.SettingsColBar.UseMoodColors".Translate());
            if (ColSettingsColBar.UseMoodColors)
            {
                //      listing.Gap(3f);
                ColSettingsColBar.moodRectScale = GUILayout.HorizontalSlider(ColSettingsColBar.moodRectScale, 0.33f, 1f);
            }
            GUILayout.Space(12f);
            ColSettingsColBar.UseWeaponIcons = GUILayout.Toggle(ColSettingsColBar.UseWeaponIcons, "ColonistBarKF.SettingsColBar.UseWeaponIcons".Translate());

            GUILayout.Space(12f);
            ColSettingsColBar.UseGender = GUILayout.Toggle(ColSettingsColBar.UseGender, "ColonistBarKF.SettingsColBar.useGender".Translate());
            GUILayout.Space(12f);
            ColSettingsColBar.useZoomToMouse = GUILayout.Toggle(ColSettingsColBar.useZoomToMouse, "ColonistBarKF.SettingsColBar.useZoomToMouse".Translate());

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
            GUILayout.Space(12f);
            ColSettingsColBar.UseCustomPawnTextureCameraZoom = GUILayout.Toggle(ColSettingsColBar.UseCustomPawnTextureCameraZoom, "ColonistBarKF.SettingsColBar.PawnTextureCameraZoom".Translate());
            if (ColSettingsColBar.UseCustomPawnTextureCameraZoom)
            {
                //    listing.Gap(3f);
                ColSettingsColBar.PawnTextureCameraZoom = GUILayout.HorizontalSlider(ColSettingsColBar.PawnTextureCameraZoom, 0.3f, 3f);
            }
            else
            {
                ColSettingsColBar.PawnTextureCameraZoom = 1.28205f;
            }
            //    listing.Gap(3f);

            ColSettingsColBar.UseCustomPawnTextureCameraHorizontalOffset = GUILayout.Toggle(ColSettingsColBar.UseCustomPawnTextureCameraHorizontalOffset, "ColonistBarKF.SettingsColBar.PawnTextureCameraHorizontalOffset".Translate());
            if (ColSettingsColBar.UseCustomPawnTextureCameraHorizontalOffset)
            {
                //        listing.Gap(3f);
                ColSettingsColBar.PawnTextureCameraHorizontalOffset = GUILayout.HorizontalSlider(ColSettingsColBar.PawnTextureCameraHorizontalOffset, 0.7f, -0.7f);
            }
            else
            {
                ColSettingsColBar.PawnTextureCameraHorizontalOffset = 0f;
            }

            ColSettingsColBar.UseCustomPawnTextureCameraVerticalOffset = GUILayout.Toggle(ColSettingsColBar.UseCustomPawnTextureCameraVerticalOffset, "ColonistBarKF.SettingsColBar.PawnTextureCameraVerticalOffset".Translate());
            if (ColSettingsColBar.UseCustomPawnTextureCameraVerticalOffset)
            {
                //       listing.Gap(3f);
                ColSettingsColBar.PawnTextureCameraVerticalOffset = GUILayout.HorizontalSlider(ColSettingsColBar.PawnTextureCameraVerticalOffset, 0f, 1f);
            }
            else
            {
                ColSettingsColBar.PawnTextureCameraVerticalOffset = 0.3f;
            }
            #endregion


            GUILayout.Space(12f);
            ColSettingsColBar.UseCustomDoubleClickTime = GUILayout.Toggle(ColSettingsColBar.UseCustomDoubleClickTime, "ColonistBarKF.SettingsColBar.DoubleClickTime".Translate());
            if (ColSettingsColBar.UseCustomDoubleClickTime)
            {
                //       listing.Gap(3f);
                ColSettingsColBar.DoubleClickTime = GUILayout.HorizontalSlider(ColSettingsColBar.DoubleClickTime, 0.1f, 1.5f);
            }
            else
            {
                ColSettingsColBar.DoubleClickTime = 0.5f;
            }

            //       GUILayout.Toggle("ColonistBarKF.SettingsColBar.useExtraIcons".Translate(), ref ColSettingsColBar.useExtraIcons, null);
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
            if (ColSettingsColBar.UseBottomAlignment)
            {
                ColSettingsColBar.MaxColonistBarWidth = Screen.width - ColSettingsColBar.MarginLeftHorBottom - ColSettingsColBar.MarginRightHorBottom;
                ColSettingsColBar.HorizontalOffset = ColSettingsColBar.MarginLeftHorBottom / 2 - ColSettingsColBar.MarginRightHorBottom / 2;

            }
            else
            {
                ColSettingsColBar.MaxColonistBarWidth = Screen.width - ColSettingsColBar.MarginLeftHorTop - ColSettingsColBar.MarginRightHorTop;
                ColSettingsColBar.HorizontalOffset = ColSettingsColBar.MarginLeftHorTop / 2 - ColSettingsColBar.MarginRightHorTop / 2;

            }
            return result;
        }

        public float SliderMaxBarHeight(Rect rect, float val, float min, float max)
        {
            GUI.skin.horizontalSlider.alignment = TextAnchor.MiddleCenter;
            float result = GUI.HorizontalSlider(rect, val, min, max);
            if (ColSettingsColBar.UseRightAlignment)
            {
                ColSettingsColBar.MaxColonistBarHeight = Screen.height - ColSettingsColBar.MarginTopVerRight - ColSettingsColBar.MarginBottomVerRight;
                ColSettingsColBar.VerticalOffset = ColSettingsColBar.MarginTopVerRight / 2 - ColSettingsColBar.MarginBottomVerRight / 2;
            }
            else
            {
                ColSettingsColBar.MaxColonistBarHeight = Screen.height - ColSettingsColBar.MarginTopVerLeft - ColSettingsColBar.MarginBottomVerLeft;
                ColSettingsColBar.VerticalOffset = ColSettingsColBar.MarginTopVerLeft / 2 - ColSettingsColBar.MarginBottomVerLeft / 2;
            }
            return result;
        }

        public float SliderBaseSize(Rect rect, float val, float min, float max)
        {
            GUI.skin.horizontalSlider.alignment = TextAnchor.MiddleCenter;
            float result = GUI.HorizontalSlider(rect, val, min, max);
            ColSettingsColBar.BaseIconSize = ColSettingsColBar.BaseSizeFloat / 2f - 4f;
            return result;
        }
#if !NoCCL
        private LabeledInput_Color femaleColorField = new LabeledInput_Color(barSettings.FemaleColor, "ColonistBarKF.SettingsColBar.FemaleColor".Translate());
        private LabeledInput_Color maleColorField = new LabeledInput_Color(barSettings.MaleColor, "ColonistBarKF.SettingsColBar.MaleColor".Translate());
#endif

        private void FillPagePSILoadIconset()
        {
            //  if (GUILayout.Button("PSI.SettingsColBar.IconSet".Translate() + PSI.ColSettingsColBar.IconSet))
            //   if (GUILayout.ButtonLabeled("PSI.SettingsColBar.IconSet".Translate() , PSI.ColSettingsColBar.IconSet))
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
            GUILayout.Label("PSI.ColSettingsColBar.LoadPresetButton".Translate(), FontBold);
            if (GUILayout.Button("PSI.SettingsColBar.IconSet".Translate() + SettingsPsi.IconSet))
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
            GUILayout.Label("PSI.Settings.Visibility.Header", FontBold);

            SettingsPsi.ShowTargetPoint = GUILayout.Toggle(SettingsPsi.ShowTargetPoint, "PSI.Settings.Visibility.TargetPoint".Translate());
            SettingsPsi.ShowAggressive = GUILayout.Toggle(SettingsPsi.ShowAggressive, "PSI.Settings.Visibility.Aggressive".Translate());
            SettingsPsi.ShowDazed = GUILayout.Toggle(SettingsPsi.ShowDazed, "PSI.Settings.Visibility.Dazed".Translate());
            SettingsPsi.ShowLeave = GUILayout.Toggle(SettingsPsi.ShowLeave, "PSI.Settings.Visibility.Leave".Translate());
            SettingsPsi.ShowDraft = GUILayout.Toggle(SettingsPsi.ShowDraft, "PSI.Settings.Visibility.Draft".Translate());
          
            SettingsPsi.ShowIdle = GUILayout.Toggle(SettingsPsi.ShowIdle, "PSI.Settings.Visibility.Idle".Translate());
            SettingsPsi.ShowUnarmed = GUILayout.Toggle(SettingsPsi.ShowUnarmed, "PSI.Settings.Visibility.Unarmed".Translate());
            SettingsPsi.ShowHungry = GUILayout.Toggle(SettingsPsi.ShowHungry, "PSI.Settings.Visibility.Hungry".Translate());
            SettingsPsi.ShowSad = GUILayout.Toggle(SettingsPsi.ShowSad, "PSI.Settings.Visibility.Sad".Translate());
            SettingsPsi.ShowTired = GUILayout.Toggle(SettingsPsi.ShowTired, "PSI.Settings.Visibility.Tired".Translate());
            //
            SettingsPsi.ShowDisease = GUILayout.Toggle(SettingsPsi.ShowDisease, "PSI.Settings.Visibility.Sickness".Translate());
            SettingsPsi.ShowPain = GUILayout.Toggle(SettingsPsi.ShowPain, "PSI.Settings.Visibility.Pain".Translate());
            SettingsPsi.ShowHealth = GUILayout.Toggle(SettingsPsi.ShowHealth, "PSI.Settings.Visibility.Health".Translate());
            SettingsPsi.ShowEffectiveness = GUILayout.Toggle(SettingsPsi.ShowEffectiveness, "PSI.Settings.Visibility.Injury".Translate());
            SettingsPsi.ShowBloodloss = GUILayout.Toggle(SettingsPsi.ShowBloodloss, "PSI.Settings.Visibility.Bloodloss".Translate());
            //
            SettingsPsi.ShowHot = GUILayout.Toggle(SettingsPsi.ShowHot, "PSI.Settings.Visibility.Hot".Translate());
            SettingsPsi.ShowCold = GUILayout.Toggle(SettingsPsi.ShowCold, "PSI.Settings.Visibility.Cold".Translate());
            SettingsPsi.ShowNaked = GUILayout.Toggle(SettingsPsi.ShowNaked, "PSI.Settings.Visibility.Naked".Translate());
            SettingsPsi.ShowDrunk = GUILayout.Toggle(SettingsPsi.ShowDrunk, "PSI.Settings.Visibility.Drunk".Translate());
            SettingsPsi.ShowApparelHealth = GUILayout.Toggle(SettingsPsi.ShowApparelHealth, "PSI.Settings.Visibility.ApparelHealth".Translate());
            //
            SettingsPsi.ShowPacific = GUILayout.Toggle(SettingsPsi.ShowPacific, "PSI.Settings.Visibility.Pacific".Translate());
            SettingsPsi.ShowNightOwl = GUILayout.Toggle(SettingsPsi.ShowNightOwl, "PSI.Settings.Visibility.NightOwl".Translate());
            SettingsPsi.ShowGreedy = GUILayout.Toggle(SettingsPsi.ShowGreedy, "PSI.Settings.Visibility.Greedy".Translate());
            SettingsPsi.ShowJealous = GUILayout.Toggle(SettingsPsi.ShowJealous, "PSI.Settings.Visibility.Jealous".Translate());
            SettingsPsi.ShowLovers = GUILayout.Toggle(SettingsPsi.ShowLovers, "PSI.Settings.Visibility.Lovers".Translate());
            //
            SettingsPsi.ShowProsthophile = GUILayout.Toggle(SettingsPsi.ShowProsthophile, "PSI.Settings.Visibility.Prosthophile".Translate());
            SettingsPsi.ShowProsthophobe = GUILayout.Toggle(SettingsPsi.ShowProsthophobe, "PSI.Settings.Visibility.Prosthophobe".Translate());
            SettingsPsi.ShowRoomStatus = GUILayout.Toggle(SettingsPsi.ShowRoomStatus, "PSI.Settings.Visibility.RoomStatus".Translate());
            SettingsPsi.ShowBedroom = GUILayout.Toggle(SettingsPsi.ShowBedroom, "PSI.Settings.Visibility.Bedroom".Translate());

            SettingsPsi.ShowDeadColonists = GUILayout.Toggle(SettingsPsi.ShowDeadColonists, "PSI.Settings.Visibility.ShowDeadColonists".Translate());

            SettingsPsi.ShowPyromaniac = GUILayout.Toggle(SettingsPsi.ShowPyromaniac, "PSI.Settings.Visibility.Pyromaniac".Translate());
        }



        private void FillPagePSIOpacityAndColor()
        {
            GUILayout.Label("PSI.Settings.IconOpacityAndColor.Header".Translate(), FontBold);

            GUILayout.Label("PSI.Settings.IconOpacityAndColor.Opacity".Translate());
            SettingsPsi.IconOpacity = GUILayout.HorizontalSlider(SettingsPsi.IconOpacity, 0.05f, 1f);

            GUILayout.Label("PSI.Settings.IconOpacityAndColor.OpacityCritical".Translate());
            SettingsPsi.IconOpacityCritical = GUILayout.HorizontalSlider(SettingsPsi.IconOpacityCritical, 0f, 1f);

            GUILayout.Toggle(SettingsPsi.UseColoredTarget, "PSI.Settings.IconOpacityAndColor.UseColoredTarget".Translate());


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

            GUILayout.Label("PSI.Settings.Arrangement.Header", FontBold);

            if (GUILayout.Button("PSI.Settings.LoadPresetButton".Translate()))
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

            GUILayout.Label("PSI.Settings.Arrangement.IconSize".Translate() + ("PSI.Settings.SizeLabel." + num).Translate());
            SettingsPsi.IconSize = GUILayout.HorizontalSlider(SettingsPsi.IconSize, 0.5f, 2f);

            GUILayout.Label(string.Concat("PSI.Settings.Arrangement.IconPosition".Translate(), (int)(SettingsPsi.IconDistanceX * 100.0), " , ", (int)(SettingsPsi.IconDistanceY * 100.0)));
            SettingsPsi.IconDistanceX = GUILayout.HorizontalSlider(SettingsPsi.IconDistanceX, -2f, 2f);
            SettingsPsi.IconDistanceY = GUILayout.HorizontalSlider(SettingsPsi.IconDistanceY, -2f, 2f);

            GUILayout.Label(string.Concat("PSI.Settings.Arrangement.IconOffset".Translate(), (int)(SettingsPsi.IconOffsetX * 100.0), " , ", (int)(SettingsPsi.IconOffsetY * 100.0)));
            SettingsPsi.IconOffsetX = GUILayout.HorizontalSlider(SettingsPsi.IconOffsetX, -2f, 2f);
            SettingsPsi.IconOffsetY = GUILayout.HorizontalSlider(SettingsPsi.IconOffsetY, -2f, 2f);

            SettingsPsi.IconsHorizontal = GUILayout.Toggle(SettingsPsi.IconsHorizontal, "PSI.Settings.Arrangement.Horizontal".Translate());

            SettingsPsi.IconsScreenScale = GUILayout.Toggle(SettingsPsi.IconsScreenScale, "PSI.Settings.Arrangement.ScreenScale".Translate());

            GUILayout.Label("PSI.Settings.Arrangement.IconsPerColumn".Translate() + SettingsPsi.IconsInColumn);

            SettingsPsi.IconsInColumn = (int)GUILayout.HorizontalSlider(SettingsPsi.IconsInColumn, 1f, 7f);

            SavePsiSettings();
            Reinit();

            //   if (!listing.DoTextButton("PSI.Settings.ReturnButton".Translate()))
            //       return;
            //
            //   Page = "main";
        }


        private void FillPSIPageLimits()
        {

            GUILayout.Label("PSI.Settings.Sensitivity.Header".Translate(), FontBold);
            if (GUILayout.Button("PSI.Settings.LoadPresetButton".Translate()))
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


            GUILayout.Label("PSI.Settings.Sensitivity.Bleeding".Translate() + ("PSI.Settings.Sensitivity.Bleeding." + Math.Round(SettingsPsi.LimitBleedMult - 0.25)).Translate());
            SettingsPsi.LimitBleedMult = GUILayout.HorizontalSlider(SettingsPsi.LimitBleedMult, 0.5f, 5f);

            GUILayout.Label("PSI.Settings.Sensitivity.Injured".Translate() + (int)(SettingsPsi.LimitEfficiencyLess * 100.0) + "%");
            SettingsPsi.LimitEfficiencyLess = GUILayout.HorizontalSlider(SettingsPsi.LimitEfficiencyLess, 0.01f, 0.99f);

            GUILayout.Label("PSI.Settings.Sensitivity.Food".Translate() + (int)(SettingsPsi.LimitFoodLess * 100.0) + "%");
            SettingsPsi.LimitFoodLess = GUILayout.HorizontalSlider(SettingsPsi.LimitFoodLess, 0.01f, 0.99f);

            GUILayout.Label("PSI.Settings.Sensitivity.Mood".Translate() + (int)(SettingsPsi.LimitMoodLess * 100.0) + "%");
            SettingsPsi.LimitMoodLess = GUILayout.HorizontalSlider(SettingsPsi.LimitMoodLess, 0.01f, 0.99f);

            GUILayout.Label("PSI.Settings.Sensitivity.Rest".Translate() + (int)(SettingsPsi.LimitRestLess * 100.0) + "%");
            SettingsPsi.LimitRestLess = GUILayout.HorizontalSlider(SettingsPsi.LimitRestLess, 0.01f, 0.99f);

            GUILayout.Label("PSI.Settings.Sensitivity.ApparelHealth".Translate() + (int)(SettingsPsi.LimitApparelHealthLess * 100.0) + "%");
            SettingsPsi.LimitApparelHealthLess = GUILayout.HorizontalSlider(SettingsPsi.LimitApparelHealthLess, 0.01f, 0.99f);

            GUILayout.Label("PSI.Settings.Sensitivity.Temperature".Translate() + (int)SettingsPsi.LimitTempComfortOffset + "C");
            SettingsPsi.LimitTempComfortOffset = GUILayout.HorizontalSlider(SettingsPsi.LimitTempComfortOffset, -10f, 10f);

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
            Scribe_Values.LookValue(ref ColSettingsColBar.UseCustomMarginTopHor, "useCustomMarginTopHor", false);
            Scribe_Values.LookValue(ref ColSettingsColBar.UseCustomMarginBottomHor, "UseCustomMarginBottomHor", false);
            Scribe_Values.LookValue(ref ColSettingsColBar.UseCustomMarginLeftHorTop, "useCustomMarginLeftHor", false);
            Scribe_Values.LookValue(ref ColSettingsColBar.UseCustomMarginRightHorTop, "useCustomMarginRightHor", false);
            Scribe_Values.LookValue(ref ColSettingsColBar.UseCustomMarginLeftHorBottom, "UseCustomMarginLeftHorBottom", false);
            Scribe_Values.LookValue(ref ColSettingsColBar.UseCustomMarginRightHorBottom, "UseCustomMarginRightHorBottom", false);

            Scribe_Values.LookValue(ref ColSettingsColBar.UseCustomMarginTopVerLeft, "UseCustomMarginTopVerLeft", false);
            Scribe_Values.LookValue(ref ColSettingsColBar.UseCustomMarginTopVerRight, "UseCustomMarginTopVerRight", false);
            Scribe_Values.LookValue(ref ColSettingsColBar.UseCustomMarginLeftVer, "useCustomMarginLeftRightVer", false);
            Scribe_Values.LookValue(ref ColSettingsColBar.UseCustomMarginRightVer, "UseCustomMarginRightVer", false);
            Scribe_Values.LookValue(ref ColSettingsColBar.UseCustomMarginBottomVerLeft, "UseCustomMarginBottomVerLeft", false);
            Scribe_Values.LookValue(ref ColSettingsColBar.UseCustomMarginBottomVerRight, "UseCustomMarginBottomVerRight", false);


            Scribe_Values.LookValue(ref ColSettingsColBar.UseCustomBaseSpacingHorizontal, "useCustomBaseSpacingHorizontal", false);
            Scribe_Values.LookValue(ref ColSettingsColBar.UseCustomBaseSpacingVertical, "useCustomBaseSpacingVertical", false);
            Scribe_Values.LookValue(ref ColSettingsColBar.UseCustomIconSize, "useCustomIconSize", false);
            Scribe_Values.LookValue(ref ColSettingsColBar.UseFixedIconScale, "useFixedIconScale", false);
            Scribe_Values.LookValue(ref ColSettingsColBar.UseCustomPawnTextureCameraHorizontalOffset, "UseCustomPawnTextureCameraHorizontalOffset", false);
            Scribe_Values.LookValue(ref ColSettingsColBar.UseCustomPawnTextureCameraVerticalOffset, "useCustomPawnTextureCameraVerticalOffset", false);
            Scribe_Values.LookValue(ref ColSettingsColBar.UseCustomPawnTextureCameraZoom, "useCustomPawnTextureCameraZoom", false);
            Scribe_Values.LookValue(ref ColSettingsColBar.UseCustomDoubleClickTime, "useCustomDoubleClick", false);
            Scribe_Values.LookValue(ref ColSettingsColBar.UseGender, "useGender", false);
            Scribe_Values.LookValue(ref ColSettingsColBar.UseVerticalAlignment, "useVerticalAlignment", false);
            Scribe_Values.LookValue(ref ColSettingsColBar.UseRightAlignment, "useRightAlignment", false);
            Scribe_Values.LookValue(ref ColSettingsColBar.UseBottomAlignment, "useBottomAlignment", false);

            Scribe_Values.LookValue(ref ColSettingsColBar.UseMoodColors, "UseMoodColors", false);
            Scribe_Values.LookValue(ref ColSettingsColBar.UseWeaponIcons, "UseWeaponIcons", false);

            Scribe_Values.LookValue(ref ColSettingsColBar.MarginTopHor, "MarginTopHor", 21f);
            Scribe_Values.LookValue(ref ColSettingsColBar.MarginBottomHor, "MarginBottomHor", 21f);
            Scribe_Values.LookValue(ref ColSettingsColBar.MarginLeftHorTop, "MarginLeftHorTop", 160f);
            Scribe_Values.LookValue(ref ColSettingsColBar.MarginRightHorTop, "MarginRightHorTop", 160f);
            Scribe_Values.LookValue(ref ColSettingsColBar.MarginLeftHorBottom, "MarginLeftHorBottom", 160f);
            Scribe_Values.LookValue(ref ColSettingsColBar.MarginRightHorBottom, "MarginRightHorBottom", 160f);

            Scribe_Values.LookValue(ref ColSettingsColBar.MarginTopVerLeft, "MarginTopVerLeft", 120f);
            Scribe_Values.LookValue(ref ColSettingsColBar.MarginBottomVerLeft, "MarginBottomVerLeft", 120f);
            Scribe_Values.LookValue(ref ColSettingsColBar.MarginTopVerRight, "MarginTopVerRight", 120f);
            Scribe_Values.LookValue(ref ColSettingsColBar.MarginBottomVerRight, "MarginBottomVerRight", 120f);
            Scribe_Values.LookValue(ref ColSettingsColBar.MarginLeftVer, "MarginLeftVer", 21f);
            Scribe_Values.LookValue(ref ColSettingsColBar.MarginRightVer, "MarginRightVer", 21f);

            Scribe_Values.LookValue(ref ColSettingsColBar.HorizontalOffset, "HorizontalOffset", 0f);
            Scribe_Values.LookValue(ref ColSettingsColBar.VerticalOffset, "VerticalOffset", 0f);


            Scribe_Values.LookValue(ref ColSettingsColBar.BaseSpacingHorizontal, "BaseSpacingHorizontal", 24f);
            Scribe_Values.LookValue(ref ColSettingsColBar.BaseSpacingVertical, "BaseSpacingVertical", 32f);
            Scribe_Values.LookValue(ref ColSettingsColBar.BaseSizeFloat, "BaseSizeFloat", 48f);
            Scribe_Values.LookValue(ref ColSettingsColBar.BaseIconSize, "BaseIconSize", 20f);
            Scribe_Values.LookValue(ref ColSettingsColBar.PawnTextureCameraHorizontalOffset, "PawnTextureCameraHorizontalOffset", 0f);
            Scribe_Values.LookValue(ref ColSettingsColBar.PawnTextureCameraVerticalOffset, "PawnTextureCameraVerticalOffset", 0.3f);
            Scribe_Values.LookValue(ref ColSettingsColBar.PawnTextureCameraZoom, "PawnTextureCameraZoom", 1.28205f);
            Scribe_Values.LookValue(ref ColSettingsColBar.MaxColonistBarWidth, "MaxColonistBarWidth");
            Scribe_Values.LookValue(ref ColSettingsColBar.MaxColonistBarHeight, "MaxColonistBarHeight");


            Scribe_Values.LookValue(ref ColSettingsColBar.DoubleClickTime, "DoubleClickTime", 0.5f);

            Scribe_Values.LookValue(ref ColSettingsColBar.FemaleColor, "FemaleColor");
            Scribe_Values.LookValue(ref ColSettingsColBar.MaleColor, "MaleColor");

            Scribe_Values.LookValue(ref ColSettingsColBar.MaxRows, "MaxRows");
            Scribe_Values.LookValue(ref ColSettingsColBar.SortBy, "SortBy");
            Scribe_Values.LookValue(ref ColSettingsColBar.useZoomToMouse, "useZoomToMouse");
            Scribe_Values.LookValue(ref ColSettingsColBar.moodRectScale, "moodRectScale");


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
