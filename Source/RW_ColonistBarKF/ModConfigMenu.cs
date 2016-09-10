#if !NoCCL
using CommunityCoreLibrary;
using CommunityCoreLibrary.UI;
# endif
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Verse;
using static ColonistBarKF.CBKF;
using static ColonistBarKF.PSI.PSI;
using static ColonistBarKF.BarSettings.SortByWhat;

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



        public override void PreOpen()
        {
            base.PreOpen();
        }




#if NoCCL
        public override void PreClose()
        {
            SaveSettings();
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
            "PSISettings".Translate()
        };
        public string[] psiToolbarStrings =
        {
            "ColonistBarKF.BarSettings.PSIToolbar01".Translate(),
            "ColonistBarKF.BarSettings.PSIToolbar02".Translate(),
            "ColonistBarKF.BarSettings.PSIToolbar03".Translate(),
            "ColonistBarKF.BarSettings.PSIToolbar04".Translate(),
            "ColonistBarKF.BarSettings.PSIToolbar05".Translate()
        };

        public string[] psiPositionStrings =
        {
            "ColonistBarKF.BarSettings.useTop".Translate(),
            "ColonistBarKF.BarSettings.useBottom".Translate(),
            "ColonistBarKF.BarSettings.useLeft".Translate(),
            "ColonistBarKF.BarSettings.useRight".Translate()
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
                SaveSettings();
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
                SaveSettings();
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
                SaveSettings();
                SavePsiSettings();
                psiToolbarInt = value;
            }
        }

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
            rect.xMin += 15f;
            rect.width -= 15f;
            rect.yMin += 20f;
            rect.yMax -= 10f;


            GUILayout.BeginArea(new Rect(0, 0, rect.width, 36f));
            MainToolbarInt = GUILayout.Toolbar(MainToolbarInt, mainToolbarStrings, GUILayout.Width(rect.width));
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(0, 36f, rect.width, 30f));

            switch (MainToolbarInt)
            {
                case 0:
                    {
                        BarPositionInt = GUILayout.Toolbar(BarPositionInt, psiPositionStrings, GUILayout.Width(rect.width));
                        GUILayout.EndArea();
                        GUILayout.BeginArea(new Rect(0, 84f, rect.width, rect.height - 130f));
                        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(rect.width));
                        FillPageOptions();
                    }
                    break;
                case 1:
                    {
                        PSIToolbarInt = GUILayout.Toolbar(PSIToolbarInt, psiToolbarStrings, GUILayout.Width(rect.width));
                        GUILayout.EndArea();
                        GUILayout.BeginArea(new Rect(0, 84f, rect.width, rect.height - 130f));
                        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(rect.width));

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
                                FillPagePSIMain();
                            }
                        }
                        else
                        {
                            FillPagePSIShowHide();
                        }


                    }
                    break;
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();


            GUILayout.BeginArea(new Rect(0, rect.yMax - 10, rect.width, 30f));
            if (GUILayout.Button("ColonistBarKF.BarSettings.RevertSettings".Translate(), GUILayout.Width(rect.width / 2 - 10f)))
            {
                ResetBarSettings();
            }
            if (GUILayout.Button("ColonistBarKF.BarSettings.RevertPSISettings".Translate(), GUILayout.Width(rect.width / 2 - 10f)))
            {
                ResetPSISettings();
            }
            GUILayout.EndArea();

#if !NoCCL
            return 1000f;
#endif
        }

        private void ResetBarSettings()
        {
            // forces the mod to update the values from the sliders. is deactivated by saving the values.
            //ColBarSettings.Reloadsettings = true;

            {
                ColBarSettings.UseGender = true;
                ColBarSettings.UseCustomMarginTopHor = false;
                ColBarSettings.UseCustomBaseSpacingHorizontal = false;
                ColBarSettings.UseCustomBaseSpacingVertical = false;
                ColBarSettings.UseCustomIconSize = false;
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

            PsiSettings.LimitMoodLess = 0.25f;
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

        private void FillPageOptions()
        {
            // forces the mod to update the values from the sliders. is deactivated by saving the values.
            if (false)
                ColBarSettings.Reloadsettings = true;

            #region Vertical Alignment

            if (ColBarSettings.UseVerticalAlignment)
            {
#if !NoCCL
                listing.Indent();
#endif
                if (ColBarSettings.UseRightAlignment)
                {
                    ColBarSettings.UseCustomMarginRightVer = GUILayout.Toggle(ColBarSettings.UseCustomMarginRightVer, "ColonistBarKF.BarSettings.MarginEdge".Translate());
                    if (ColBarSettings.UseCustomMarginRightVer)
                    {
                        //    listing.Gap(3f);
                        ColBarSettings.MarginRightVer = GUILayout.HorizontalSlider(ColBarSettings.MarginRightVer, 0f, Screen.width / 12);
                    }
                    else
                    {
                        ColBarSettings.MarginRightVer = 21f;
                    }
                    // listing.Gap(3f);

                    ColBarSettings.UseCustomMarginTopVerRight = GUILayout.Toggle(ColBarSettings.UseCustomMarginTopVerRight, "ColonistBarKF.BarSettings.MarginTop".Translate());
                    if (ColBarSettings.UseCustomMarginTopVerRight)
                    {
                        ColBarSettings.MarginTopVerRight = GUILayout.HorizontalSlider(ColBarSettings.MarginTopVerRight, 0f, Screen.height * 2 / 5);
                    }
                    else
                    {
                        ColBarSettings.MarginTopVerRight = 120f;
                        ColBarSettings.MaxColonistBarHeight = Screen.height - ColBarSettings.MarginTopVerRight - ColBarSettings.MarginBottomVerRight;
                        ColBarSettings.VerticalOffset = ColBarSettings.MarginTopVerRight / 2 - ColBarSettings.MarginBottomVerRight / 2;

                    }
                    //  listing.Gap(3f);
                    ColBarSettings.UseCustomMarginBottomVerRight = GUILayout.Toggle(ColBarSettings.UseCustomMarginBottomVerRight, "ColonistBarKF.BarSettings.MarginBottom".Translate());
                    if (ColBarSettings.UseCustomMarginBottomVerRight)
                    {
                        //     listing.Gap(3f);
                        ColBarSettings.MarginBottomVerRight = GUILayout.HorizontalSlider(ColBarSettings.MarginBottomVerRight, 0f, Screen.height * 2 / 5);
                    }
                    else
                    {
                        ColBarSettings.MarginBottomVerRight = 120f;
                        ColBarSettings.MaxColonistBarHeight = Screen.height - ColBarSettings.MarginTopVerRight - ColBarSettings.MarginBottomVerRight;
                        ColBarSettings.VerticalOffset = ColBarSettings.MarginTopVerRight / 2 - ColBarSettings.MarginBottomVerRight / 2;
                    }
                }
                else
                {
                    ColBarSettings.UseCustomMarginLeftVer = GUILayout.Toggle(ColBarSettings.UseCustomMarginLeftVer, "ColonistBarKF.BarSettings.MarginEdge".Translate());
                    if (ColBarSettings.UseCustomMarginLeftVer)
                    {
                        //     listing.Gap(3f);
                        ColBarSettings.MarginLeftVer = GUILayout.HorizontalSlider(ColBarSettings.MarginLeftVer, 0f, Screen.width / 12);
                    }
                    else
                    {
                        ColBarSettings.MarginLeftVer = 21f;
                    }
                    //   listing.Gap(3f);

                    ColBarSettings.UseCustomMarginTopVerLeft = GUILayout.Toggle(ColBarSettings.UseCustomMarginTopVerLeft, "ColonistBarKF.BarSettings.MarginTop".Translate());
                    if (ColBarSettings.UseCustomMarginTopVerLeft)
                    {
                        //    listing.Gap(3f);
                        ColBarSettings.MarginTopVerLeft = GUILayout.HorizontalSlider(ColBarSettings.MarginTopVerLeft, 0f, Screen.height * 2 / 5);
                    }
                    else
                    {
                        ColBarSettings.MarginTopVerLeft = 120f;
                        ColBarSettings.MaxColonistBarHeight = Screen.height - ColBarSettings.MarginTopVerLeft - ColBarSettings.MarginBottomVerLeft;
                        ColBarSettings.VerticalOffset = ColBarSettings.MarginTopVerLeft / 2 - ColBarSettings.MarginBottomVerLeft / 2;

                    }
                    //   listing.Gap(3f);

                    ColBarSettings.UseCustomMarginBottomVerLeft = GUILayout.Toggle(ColBarSettings.UseCustomMarginBottomVerLeft, "ColonistBarKF.BarSettings.MarginBottom".Translate());
                    if (ColBarSettings.UseCustomMarginBottomVerLeft)
                    {
                        //    listing.Gap(3f);
                        ColBarSettings.MarginBottomVerLeft = GUILayout.HorizontalSlider(ColBarSettings.MarginBottomVerLeft, 0f, Screen.height * 2 / 5);
                    }
                    else
                    {
                        ColBarSettings.MarginBottomVerLeft = 120f;
                        ColBarSettings.MaxColonistBarHeight = Screen.height - ColBarSettings.MarginTopVerLeft - ColBarSettings.MarginBottomVerLeft;
                        ColBarSettings.VerticalOffset = ColBarSettings.MarginTopVerLeft / 2 - ColBarSettings.MarginBottomVerLeft / 2;
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

                if (ColBarSettings.UseBottomAlignment)
                {
                    ColBarSettings.UseCustomMarginBottomHor = GUILayout.Toggle(ColBarSettings.UseCustomMarginBottomHor, "ColonistBarKF.BarSettings.MarginEdge".Translate());
                    if (ColBarSettings.UseCustomMarginBottomHor)
                    {
                        //    listing.Gap(3f);
                        ColBarSettings.MarginBottomHor = ColBarSettings.MarginBottomHor = GUILayout.HorizontalSlider(ColBarSettings.MarginBottomHor, 10, Screen.height / 12);
                    }
                    else
                    {
                        ColBarSettings.MarginBottomHor = 21f;
                    }
                    //   listing.Gap(3f);


                    ColBarSettings.UseCustomMarginLeftHorBottom = GUILayout.Toggle(ColBarSettings.UseCustomMarginLeftHorBottom, "ColonistBarKF.BarSettings.MarginLeft".Translate());
                    if (ColBarSettings.UseCustomMarginLeftHorBottom)
                    {
                        //   listing.Gap(3f);
                        ColBarSettings.MarginLeftHorBottom = GUILayout.HorizontalSlider(ColBarSettings.MarginLeftHorBottom, 0f, Screen.width * 2 / 5);
                    }
                    else
                    {
                        ColBarSettings.MarginLeftHorBottom = 160f;
                        ColBarSettings.MaxColonistBarWidth = Screen.width - ColBarSettings.MarginLeftHorBottom - ColBarSettings.MarginRightHorBottom;
                        ColBarSettings.HorizontalOffset = ColBarSettings.MarginLeftHorBottom / 2 - ColBarSettings.MarginRightHorBottom / 2;
                    }
                    //  listing.Gap(3f);

                    ColBarSettings.UseCustomMarginRightHorBottom = GUILayout.Toggle(ColBarSettings.UseCustomMarginRightHorBottom, "ColonistBarKF.BarSettings.MarginRight".Translate());
                    if (ColBarSettings.UseCustomMarginRightHorBottom)
                    {
                        //      listing.Gap(3f);
                        ColBarSettings.MarginRightHorBottom = GUILayout.HorizontalSlider(ColBarSettings.MarginRightHorBottom, 0f, Screen.width * 2 / 5);
                    }
                    else
                    {
                        ColBarSettings.MarginRightHorBottom = 160f;
                        ColBarSettings.MaxColonistBarWidth = Screen.width - ColBarSettings.MarginLeftHorBottom - ColBarSettings.MarginRightHorBottom;
                        ColBarSettings.HorizontalOffset = ColBarSettings.MarginLeftHorBottom / 2 - ColBarSettings.MarginRightHorBottom / 2;
                    }
                    //    listing.Gap(3f);
                }
                else
                {
                    ColBarSettings.UseCustomMarginTopHor = GUILayout.Toggle(ColBarSettings.UseCustomMarginTopHor, "ColonistBarKF.BarSettings.MarginEdge".Translate());
                    if (ColBarSettings.UseCustomMarginTopHor)
                    {
                        //    listing.Gap(3f);
                        ColBarSettings.MarginTopHor = GUILayout.HorizontalSlider(ColBarSettings.MarginTopHor, 10, Screen.height / 12);
                    }
                    else
                    {
                        ColBarSettings.MarginTopHor = 21f;
                    }
                    //  listing.Gap(3f);


                    ColBarSettings.UseCustomMarginLeftHorTop = GUILayout.Toggle(ColBarSettings.UseCustomMarginLeftHorTop, "ColonistBarKF.BarSettings.MarginLeft".Translate());
                    if (ColBarSettings.UseCustomMarginLeftHorTop)
                    {
                        //    listing.Gap(3f);
                        ColBarSettings.MarginLeftHorTop = GUILayout.HorizontalSlider(ColBarSettings.MarginLeftHorTop, 0f, Screen.width * 2 / 5);
                    }
                    else
                    {
                        ColBarSettings.MarginLeftHorTop = 160f;
                        ColBarSettings.MaxColonistBarWidth = Screen.width - ColBarSettings.MarginLeftHorTop - ColBarSettings.MarginRightHorTop;
                        ColBarSettings.HorizontalOffset = ColBarSettings.MarginLeftHorTop / 2 - ColBarSettings.MarginRightHorTop / 2;
                    }
                    //  listing.Gap(3f);

                    ColBarSettings.UseCustomMarginRightHorTop = GUILayout.Toggle(ColBarSettings.UseCustomMarginRightHorTop, "ColonistBarKF.BarSettings.MarginRight".Translate());
                    if (ColBarSettings.UseCustomMarginRightHorTop)
                    {
                        //     listing.Gap(3f);
                        ColBarSettings.MarginRightHorTop = GUILayout.HorizontalSlider(ColBarSettings.MarginRightHorTop, 0f, Screen.width * 2 / 5);
                    }
                    else
                    {
                        ColBarSettings.MarginRightHorTop = 160f;
                        ColBarSettings.MaxColonistBarWidth = Screen.width - ColBarSettings.MarginLeftHorTop - ColBarSettings.MarginRightHorTop;
                        ColBarSettings.HorizontalOffset = ColBarSettings.MarginLeftHorTop / 2 - ColBarSettings.MarginRightHorTop / 2;
                    }
                    //  listing.Gap(3f);
                }
#if !NoCCL
                listing.Undent();
#endif
            }
            #endregion

            GUILayout.Space(12f);

            #region Size + Spacing

            ColBarSettings.UseCustomIconSize = GUILayout.Toggle(ColBarSettings.UseCustomIconSize, "ColonistBarKF.BarSettings.BasicSize".Translate());

            if (ColBarSettings.UseCustomIconSize)
            {
                //     listing.Gap(3f);
                ColBarSettings.UseFixedIconScale = GUILayout.Toggle(ColBarSettings.UseFixedIconScale, "ColonistBarKF.BarSettings.FixedScale".Translate());
                //    listing.Gap(3f);
                ColBarSettings.BaseSizeFloat = GUILayout.HorizontalSlider(ColBarSettings.BaseSizeFloat, 16f, 128f);

                //     listing.Gap();
            }
            else
            {
                ColBarSettings.BaseSizeFloat = 48f;
                ColBarSettings.BaseIconSize = 20f;
                //     listing.Gap(3f);
            }


            ColBarSettings.UseCustomBaseSpacingHorizontal = GUILayout.Toggle(ColBarSettings.UseCustomBaseSpacingHorizontal, "ColonistBarKF.BarSettings.BaseSpacingHorizontal".Translate());
            if (ColBarSettings.UseCustomBaseSpacingHorizontal)
            {
                //      listing.Gap(3f);
                ColBarSettings.BaseSpacingHorizontal = GUILayout.HorizontalSlider(ColBarSettings.BaseSpacingHorizontal, 1f, 72f);
            }
            else
            {
                ColBarSettings.BaseSpacingHorizontal = 24f;
                //      listing.Gap(3f);
            }

            ColBarSettings.UseCustomBaseSpacingVertical = GUILayout.Toggle(ColBarSettings.UseCustomBaseSpacingVertical, "ColonistBarKF.BarSettings.BaseSpacingVertical".Translate());
            if (ColBarSettings.UseCustomBaseSpacingVertical)
            {
                //      listing.Gap(3f);
                ColBarSettings.BaseSpacingVertical = GUILayout.HorizontalSlider(ColBarSettings.BaseSpacingVertical, 1f, 96f);
            }
            else
            {
                ColBarSettings.BaseSpacingVertical = 32f;
            }

            #endregion

            GUILayout.Space(12f);
            ColBarSettings.UseMoodColors = GUILayout.Toggle(ColBarSettings.UseMoodColors, "ColonistBarKF.BarSettings.UseMoodColors".Translate());
            if (ColBarSettings.UseMoodColors)
            {
                //      listing.Gap(3f);
                ColBarSettings.moodRectScale = GUILayout.HorizontalSlider(ColBarSettings.moodRectScale, 0.33f, 1f);
            }
            GUILayout.Space(12f);
            ColBarSettings.UseWeaponIcons = GUILayout.Toggle(ColBarSettings.UseWeaponIcons, "ColonistBarKF.BarSettings.UseWeaponIcons".Translate());

            GUILayout.Space(12f);
            ColBarSettings.UseGender = GUILayout.Toggle(ColBarSettings.UseGender, "ColonistBarKF.BarSettings.useGender".Translate());
            GUILayout.Space(12f);
            ColBarSettings.useZoomToMouse = GUILayout.Toggle(ColBarSettings.useZoomToMouse, "ColonistBarKF.BarSettings.useZoomToMouse".Translate());

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
                if (GUILayout.Button("ColonistBarKF.BarSettings.ResetColors".Translate()))
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
            ColBarSettings.UseCustomPawnTextureCameraZoom = GUILayout.Toggle(ColBarSettings.UseCustomPawnTextureCameraZoom, "ColonistBarKF.BarSettings.PawnTextureCameraZoom".Translate());
            if (ColBarSettings.UseCustomPawnTextureCameraZoom)
            {
                //    listing.Gap(3f);
                ColBarSettings.PawnTextureCameraZoom = GUILayout.HorizontalSlider(ColBarSettings.PawnTextureCameraZoom, 0.3f, 3f);
            }
            else
            {
                ColBarSettings.PawnTextureCameraZoom = 1.28205f;
            }
            //    listing.Gap(3f);

            ColBarSettings.UseCustomPawnTextureCameraHorizontalOffset = GUILayout.Toggle(ColBarSettings.UseCustomPawnTextureCameraHorizontalOffset, "ColonistBarKF.BarSettings.PawnTextureCameraHorizontalOffset".Translate());
            if (ColBarSettings.UseCustomPawnTextureCameraHorizontalOffset)
            {
                //        listing.Gap(3f);
                ColBarSettings.PawnTextureCameraHorizontalOffset = GUILayout.HorizontalSlider(ColBarSettings.PawnTextureCameraHorizontalOffset, 0.7f, -0.7f);
            }
            else
            {
                ColBarSettings.PawnTextureCameraHorizontalOffset = 0f;
            }

            ColBarSettings.UseCustomPawnTextureCameraVerticalOffset = GUILayout.Toggle(ColBarSettings.UseCustomPawnTextureCameraVerticalOffset, "ColonistBarKF.BarSettings.PawnTextureCameraVerticalOffset".Translate());
            if (ColBarSettings.UseCustomPawnTextureCameraVerticalOffset)
            {
                //       listing.Gap(3f);
                ColBarSettings.PawnTextureCameraVerticalOffset = GUILayout.HorizontalSlider(ColBarSettings.PawnTextureCameraVerticalOffset, 0f, 1f);
            }
            else
            {
                ColBarSettings.PawnTextureCameraVerticalOffset = 0.3f;
            }
            #endregion


            GUILayout.Space(12f);
            ColBarSettings.UseCustomDoubleClickTime = GUILayout.Toggle(ColBarSettings.UseCustomDoubleClickTime, "ColonistBarKF.BarSettings.DoubleClickTime".Translate());
            if (ColBarSettings.UseCustomDoubleClickTime)
            {
                //       listing.Gap(3f);
                ColBarSettings.DoubleClickTime = GUILayout.HorizontalSlider(ColBarSettings.DoubleClickTime, 0.1f, 1.5f);
            }
            else
            {
                ColBarSettings.DoubleClickTime = 0.5f;
            }

            //       GUILayout.Toggle("ColonistBarKF.BarSettings.useExtraIcons".Translate(), ref ColBarSettings.useExtraIcons, null);
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
            if (ColBarSettings.UseBottomAlignment)
            {
                ColBarSettings.MaxColonistBarWidth = Screen.width - ColBarSettings.MarginLeftHorBottom - ColBarSettings.MarginRightHorBottom;
                ColBarSettings.HorizontalOffset = ColBarSettings.MarginLeftHorBottom / 2 - ColBarSettings.MarginRightHorBottom / 2;

            }
            else
            {
                ColBarSettings.MaxColonistBarWidth = Screen.width - ColBarSettings.MarginLeftHorTop - ColBarSettings.MarginRightHorTop;
                ColBarSettings.HorizontalOffset = ColBarSettings.MarginLeftHorTop / 2 - ColBarSettings.MarginRightHorTop / 2;

            }
            return result;
        }

        public float SliderMaxBarHeight(Rect rect, float val, float min, float max)
        {
            GUI.skin.horizontalSlider.alignment = TextAnchor.MiddleCenter;
            float result = GUI.HorizontalSlider(rect, val, min, max);
            if (ColBarSettings.UseRightAlignment)
            {
                ColBarSettings.MaxColonistBarHeight = Screen.height - ColBarSettings.MarginTopVerRight - ColBarSettings.MarginBottomVerRight;
                ColBarSettings.VerticalOffset = ColBarSettings.MarginTopVerRight / 2 - ColBarSettings.MarginBottomVerRight / 2;
            }
            else
            {
                ColBarSettings.MaxColonistBarHeight = Screen.height - ColBarSettings.MarginTopVerLeft - ColBarSettings.MarginBottomVerLeft;
                ColBarSettings.VerticalOffset = ColBarSettings.MarginTopVerLeft / 2 - ColBarSettings.MarginBottomVerLeft / 2;
            }
            return result;
        }

        public float SliderBaseSize(Rect rect, float val, float min, float max)
        {
            GUI.skin.horizontalSlider.alignment = TextAnchor.MiddleCenter;
            float result = GUI.HorizontalSlider(rect, val, min, max);
            ColBarSettings.BaseIconSize = ColBarSettings.BaseSizeFloat / 2f - 4f;
            return result;
        }
#if !NoCCL
        private LabeledInput_Color femaleColorField = new LabeledInput_Color(barSettings.FemaleColor, "ColonistBarKF.BarSettings.FemaleColor".Translate());
        private LabeledInput_Color maleColorField = new LabeledInput_Color(barSettings.MaleColor, "ColonistBarKF.BarSettings.MaleColor".Translate());
#endif

        private void FillPagePSIMain()
        {
            //  if (GUILayout.Button("PSI.BarSettings.IconSet".Translate() + PSI.ColBarSettings.IconSet))
            //   if (GUILayout.ButtonLabeled("PSI.BarSettings.IconSet".Translate() , PSI.ColBarSettings.IconSet))
            //   {
            //       var options = new List<FloatMenuOption>();
            //       foreach (var str in PSI.IconSets)
            //       {
            //           var setname = str;
            //           options.Add(new FloatMenuOption(setname, () =>
            //           {
            //               PSI.BarSettings.IconSet = setname;
            //               PSI.Materials = new Materials(setname);
            //               PSI.Materials.ReloadTextures(true);
            //           }));
            //       }
            //       Find.WindowStack.Add(new FloatMenu(options));
            //   }
            //   listing.NewColumn();

            //    if (GUILayout.ButtonLabeled("PSI.BarSettings.LoadPresetButton".Translate()))
            GUILayout.Label("PSI.ColBarSettings.LoadPresetButton".Translate());
            if (GUILayout.Button("PSI.BarSettings.IconSet".Translate() + PsiSettings.IconSet))
            {
                var options = new List<FloatMenuOption>();

                options.Add(new FloatMenuOption("default_settings".Translate(), () =>
                {
                    try
                    {
                        PsiSettings.IconSet = "default";
                        SavePsiSettings();
                        Reinit();
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.BarSettings.LoadPreset.UnableToLoad".Translate() + "default_settings");
                    }
                }));
                options.Add(new FloatMenuOption("original_settings".Translate(), () =>
                {
                    try
                    {
                        PsiSettings.IconSet = "original";
                        SavePsiSettings();
                        Reinit();
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.BarSettings.LoadPreset.UnableToLoad".Translate() + "original_settings");
                    }
                }));
                options.Add(new FloatMenuOption("text_settings".Translate(), () =>
                {
                    try
                    {
                        PsiSettings.IconSet = "text";
                        SavePsiSettings();
                        Reinit();
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.BarSettings.LoadPreset.UnableToLoad".Translate() + "text_settings");
                    }
                }));
                Find.WindowStack.Add(new FloatMenu(options));
            }


        }


        private void FillPagePSIShowHide()
        {
            GUILayout.Label("PSI.Settings.Visibility.Header");

            PsiSettings.ShowTargetPoint = GUILayout.Toggle(PsiSettings.ShowTargetPoint, "PSI.Settings.Visibility.TargetPoint".Translate());
            PsiSettings.ShowAggressive = GUILayout.Toggle(PsiSettings.ShowAggressive, "PSI.Settings.Visibility.Aggressive".Translate());
            PsiSettings.ShowDazed = GUILayout.Toggle(PsiSettings.ShowDazed, "PSI.Settings.Visibility.Dazed".Translate());
            PsiSettings.ShowLeave = GUILayout.Toggle(PsiSettings.ShowLeave, "PSI.Settings.Visibility.Leave".Translate());
            PsiSettings.ShowDraft = GUILayout.Toggle(PsiSettings.ShowDraft, "PSI.Settings.Visibility.Draft".Translate());
            //
            PsiSettings.ShowIdle = GUILayout.Toggle(PsiSettings.ShowIdle, "PSI.Settings.Visibility.Idle".Translate());
            PsiSettings.ShowUnarmed = GUILayout.Toggle(PsiSettings.ShowUnarmed, "PSI.Settings.Visibility.Unarmed".Translate());
            PsiSettings.ShowHungry = GUILayout.Toggle(PsiSettings.ShowHungry, "PSI.Settings.Visibility.Hungry".Translate());
            PsiSettings.ShowSad = GUILayout.Toggle(PsiSettings.ShowSad, "PSI.Settings.Visibility.Sad".Translate());
            PsiSettings.ShowTired = GUILayout.Toggle(PsiSettings.ShowTired, "PSI.Settings.Visibility.Tired".Translate());
            //
            PsiSettings.ShowDisease = GUILayout.Toggle(PsiSettings.ShowDisease, "PSI.Settings.Visibility.Sickness".Translate());
            PsiSettings.ShowPain = GUILayout.Toggle(PsiSettings.ShowPain, "PSI.Settings.Visibility.Pain".Translate());
            PsiSettings.ShowHealth = GUILayout.Toggle(PsiSettings.ShowHealth, "PSI.Settings.Visibility.Health".Translate());
            PsiSettings.ShowEffectiveness = GUILayout.Toggle(PsiSettings.ShowEffectiveness, "PSI.Settings.Visibility.Injury".Translate());
            PsiSettings.ShowBloodloss = GUILayout.Toggle(PsiSettings.ShowBloodloss, "PSI.Settings.Visibility.Bloodloss".Translate());
            //
            PsiSettings.ShowHot = GUILayout.Toggle(PsiSettings.ShowHot, "PSI.Settings.Visibility.Hot".Translate());
            PsiSettings.ShowCold = GUILayout.Toggle(PsiSettings.ShowCold, "PSI.Settings.Visibility.Cold".Translate());
            PsiSettings.ShowNaked = GUILayout.Toggle(PsiSettings.ShowNaked, "PSI.Settings.Visibility.Naked".Translate());
            PsiSettings.ShowDrunk = GUILayout.Toggle(PsiSettings.ShowDrunk, "PSI.Settings.Visibility.Drunk".Translate());
            PsiSettings.ShowApparelHealth = GUILayout.Toggle(PsiSettings.ShowApparelHealth, "PSI.Settings.Visibility.ApparelHealth".Translate());
            //
            PsiSettings.ShowPacific = GUILayout.Toggle(PsiSettings.ShowPacific, "PSI.Settings.Visibility.Pacific".Translate());
            PsiSettings.ShowNightOwl = GUILayout.Toggle(PsiSettings.ShowNightOwl, "PSI.Settings.Visibility.NightOwl".Translate());
            PsiSettings.ShowGreedy = GUILayout.Toggle(PsiSettings.ShowGreedy, "PSI.Settings.Visibility.Greedy".Translate());
            PsiSettings.ShowJealous = GUILayout.Toggle(PsiSettings.ShowJealous, "PSI.Settings.Visibility.Jealous".Translate());
            PsiSettings.ShowLovers = GUILayout.Toggle(PsiSettings.ShowLovers, "PSI.Settings.Visibility.Lovers".Translate());
            //
            PsiSettings.ShowProsthophile = GUILayout.Toggle(PsiSettings.ShowProsthophile, "PSI.Settings.Visibility.Prosthophile".Translate());
            PsiSettings.ShowProsthophobe = GUILayout.Toggle(PsiSettings.ShowProsthophobe, "PSI.Settings.Visibility.Prosthophobe".Translate());
            PsiSettings.ShowRoomStatus = GUILayout.Toggle(PsiSettings.ShowRoomStatus, "PSI.Settings.Visibility.RoomStatus".Translate());
            PsiSettings.ShowBedroom = GUILayout.Toggle(PsiSettings.ShowBedroom, "PSI.Settings.Visibility.Bedroom".Translate());

            PsiSettings.ShowDeadColonists = GUILayout.Toggle(PsiSettings.ShowDeadColonists, "PSI.Settings.Visibility.ShowDeadColonists".Translate());

            PsiSettings.ShowPyromaniac = GUILayout.Toggle(PsiSettings.ShowPyromaniac, "PSI.Settings.Visibility.Pyromaniac".Translate());
        }

        private void FillPagePSIOpacityAndColor()
        {
            GUILayout.Label("PSI.Settings.IconOpacityAndColor.Header".Translate());

            GUILayout.Label("PSI.Settings.IconOpacityAndColor.Opacity".Translate());
            PsiSettings.IconOpacity = GUILayout.HorizontalSlider(PsiSettings.IconOpacity, 0.05f, 1f);

            GUILayout.Label("PSI.Settings.IconOpacityAndColor.OpacityCritical".Translate());
            PsiSettings.IconOpacityCritical = GUILayout.HorizontalSlider(PsiSettings.IconOpacityCritical, 0f, 1f);

            GUILayout.Toggle(PsiSettings.UseColoredTarget, "PSI.Settings.IconOpacityAndColor.UseColoredTarget".Translate());


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

            GUILayout.Label("PSI.Settings.Arrangement.Header");

            if (GUILayout.Button("PSI.Settings.LoadPresetButton".Translate()))
            {
                var options = new List<FloatMenuOption>();

                options.Add(new FloatMenuOption("Left_Default)".Translate(), () =>
                {
                    try
                    {
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
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.Settings.LoadPreset.UnableToLoad".Translate() + "Bottom");
                    }
                }));

                Find.WindowStack.Add(new FloatMenu(options));
            }

            var num = (int)(PsiSettings.IconSize * 4.5);

            if (num > 8)
                num = 8;
            else if (num < 0)
                num = 0;

            GUILayout.Label("PSI.Settings.Arrangement.IconSize".Translate() + ("PSI.Settings.SizeLabel." + num).Translate());
            PsiSettings.IconSize = GUILayout.HorizontalSlider(PsiSettings.IconSize, 0.5f, 2f);

            GUILayout.Label(string.Concat("PSI.Settings.Arrangement.IconPosition".Translate(), (int)(PsiSettings.IconDistanceX * 100.0), " , ", (int)(PsiSettings.IconDistanceY * 100.0)));
            PsiSettings.IconDistanceX = GUILayout.HorizontalSlider(PsiSettings.IconDistanceX, -2f, 2f);
            PsiSettings.IconDistanceY = GUILayout.HorizontalSlider(PsiSettings.IconDistanceY, -2f, 2f);

            GUILayout.Label(string.Concat("PSI.Settings.Arrangement.IconOffset".Translate(), (int)(PsiSettings.IconOffsetX * 100.0), " , ", (int)(PsiSettings.IconOffsetY * 100.0)));
            PsiSettings.IconOffsetX = GUILayout.HorizontalSlider(PsiSettings.IconOffsetX, -2f, 2f);
            PsiSettings.IconOffsetY = GUILayout.HorizontalSlider(PsiSettings.IconOffsetY, -2f, 2f);

            PsiSettings.IconsHorizontal = GUILayout.Toggle(PsiSettings.IconsHorizontal, "PSI.Settings.Arrangement.Horizontal".Translate());

            PsiSettings.IconsScreenScale = GUILayout.Toggle(PsiSettings.IconsScreenScale, "PSI.Settings.Arrangement.ScreenScale".Translate());

            GUILayout.Label("PSI.Settings.Arrangement.IconsPerColumn".Translate() + PsiSettings.IconsInColumn);

            PsiSettings.IconsInColumn = (int)GUILayout.HorizontalSlider(PsiSettings.IconsInColumn, 1f, 7f);

            SavePsiSettings();
            Reinit();

            //   if (!listing.DoTextButton("PSI.Settings.ReturnButton".Translate()))
            //       return;
            //
            //   Page = "main";
        }


        private void FillPSIPageLimits()
        {

            GUILayout.Label("PSI.Settings.Sensitivity.Header");
            if (GUILayout.Button("PSI.Settings.LoadPresetButton".Translate()))
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
                        PsiSettings.LimitMoodLess = 0.2f;
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
                        PsiSettings.LimitMoodLess = 0.25f;
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
                        PsiSettings.LimitMoodLess = 0.3f;
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


            GUILayout.Label("PSI.Settings.Sensitivity.Bleeding".Translate() + ("PSI.Settings.Sensitivity.Bleeding." + Math.Round(PsiSettings.LimitBleedMult - 0.25)).Translate());
            PsiSettings.LimitBleedMult = GUILayout.HorizontalSlider(PsiSettings.LimitBleedMult, 0.5f, 5f);

            GUILayout.Label("PSI.Settings.Sensitivity.Injured".Translate() + (int)(PsiSettings.LimitEfficiencyLess * 100.0) + "%");
            PsiSettings.LimitEfficiencyLess = GUILayout.HorizontalSlider(PsiSettings.LimitEfficiencyLess, 0.01f, 0.99f);

            GUILayout.Label("PSI.Settings.Sensitivity.Food".Translate() + (int)(PsiSettings.LimitFoodLess * 100.0) + "%");
            PsiSettings.LimitFoodLess = GUILayout.HorizontalSlider(PsiSettings.LimitFoodLess, 0.01f, 0.99f);

            GUILayout.Label("PSI.Settings.Sensitivity.Mood".Translate() + (int)(PsiSettings.LimitMoodLess * 100.0) + "%");
            PsiSettings.LimitMoodLess = GUILayout.HorizontalSlider(PsiSettings.LimitMoodLess, 0.01f, 0.99f);

            GUILayout.Label("PSI.Settings.Sensitivity.Rest".Translate() + (int)(PsiSettings.LimitRestLess * 100.0) + "%");
            PsiSettings.LimitRestLess = GUILayout.HorizontalSlider(PsiSettings.LimitRestLess, 0.01f, 0.99f);

            GUILayout.Label("PSI.Settings.Sensitivity.ApparelHealth".Translate() + (int)(PsiSettings.LimitApparelHealthLess * 100.0) + "%");
            PsiSettings.LimitApparelHealthLess = GUILayout.HorizontalSlider(PsiSettings.LimitApparelHealthLess, 0.01f, 0.99f);

            GUILayout.Label("PSI.Settings.Sensitivity.Temperature".Translate() + (int)PsiSettings.LimitTempComfortOffset + "C");
            PsiSettings.LimitTempComfortOffset = GUILayout.HorizontalSlider(PsiSettings.LimitTempComfortOffset, -10f, 10f);

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
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginTopHor, "useCustomMarginTopHor", false, false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginBottomHor, "UseCustomMarginBottomHor", false, false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginLeftHorTop, "useCustomMarginLeftHor", false, false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginRightHorTop, "useCustomMarginRightHor", false, false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginLeftHorBottom, "UseCustomMarginLeftHorBottom", false, false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginRightHorBottom, "UseCustomMarginRightHorBottom", false, false);

            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginTopVerLeft, "UseCustomMarginTopVerLeft", false, false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginTopVerRight, "UseCustomMarginTopVerRight", false, false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginLeftVer, "useCustomMarginLeftRightVer", false, false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginRightVer, "UseCustomMarginRightVer", false, false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginBottomVerLeft, "UseCustomMarginBottomVerLeft", false, false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomMarginBottomVerRight, "UseCustomMarginBottomVerRight", false, false);


            Scribe_Values.LookValue(ref ColBarSettings.UseCustomBaseSpacingHorizontal, "useCustomBaseSpacingHorizontal", false, false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomBaseSpacingVertical, "useCustomBaseSpacingVertical", false, false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomIconSize, "useCustomIconSize", false, false);
            Scribe_Values.LookValue(ref ColBarSettings.UseFixedIconScale, "useFixedIconScale", false, false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomPawnTextureCameraHorizontalOffset, "UseCustomPawnTextureCameraHorizontalOffset", false, false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomPawnTextureCameraVerticalOffset, "useCustomPawnTextureCameraVerticalOffset", false, false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomPawnTextureCameraZoom, "useCustomPawnTextureCameraZoom", false, false);
            Scribe_Values.LookValue(ref ColBarSettings.UseCustomDoubleClickTime, "useCustomDoubleClick", false, false);
            Scribe_Values.LookValue(ref ColBarSettings.UseGender, "useGender", false, false);
            Scribe_Values.LookValue(ref ColBarSettings.UseVerticalAlignment, "useVerticalAlignment", false, false);
            Scribe_Values.LookValue(ref ColBarSettings.UseRightAlignment, "useRightAlignment", false, false);
            Scribe_Values.LookValue(ref ColBarSettings.UseBottomAlignment, "useBottomAlignment", false, false);

            Scribe_Values.LookValue(ref ColBarSettings.UseMoodColors, "UseMoodColors", false, false);
            Scribe_Values.LookValue(ref ColBarSettings.UseWeaponIcons, "UseWeaponIcons", false, false);

            Scribe_Values.LookValue(ref ColBarSettings.MarginTopHor, "MarginTopHor", 21f, false);
            Scribe_Values.LookValue(ref ColBarSettings.MarginBottomHor, "MarginBottomHor", 21f, false);
            Scribe_Values.LookValue(ref ColBarSettings.MarginLeftHorTop, "MarginLeftHorTop", 160f, false);
            Scribe_Values.LookValue(ref ColBarSettings.MarginRightHorTop, "MarginRightHorTop", 160f, false);
            Scribe_Values.LookValue(ref ColBarSettings.MarginLeftHorBottom, "MarginLeftHorBottom", 160f, false);
            Scribe_Values.LookValue(ref ColBarSettings.MarginRightHorBottom, "MarginRightHorBottom", 160f, false);

            Scribe_Values.LookValue(ref ColBarSettings.MarginTopVerLeft, "MarginTopVerLeft", 120f, false);
            Scribe_Values.LookValue(ref ColBarSettings.MarginBottomVerLeft, "MarginBottomVerLeft", 120f, false);
            Scribe_Values.LookValue(ref ColBarSettings.MarginTopVerRight, "MarginTopVerRight", 120f, false);
            Scribe_Values.LookValue(ref ColBarSettings.MarginBottomVerRight, "MarginBottomVerRight", 120f, false);
            Scribe_Values.LookValue(ref ColBarSettings.MarginLeftVer, "MarginLeftVer", 21f, false);
            Scribe_Values.LookValue(ref ColBarSettings.MarginRightVer, "MarginRightVer", 21f, false);

            Scribe_Values.LookValue(ref ColBarSettings.HorizontalOffset, "HorizontalOffset", 0f, false);
            Scribe_Values.LookValue(ref ColBarSettings.VerticalOffset, "VerticalOffset", 0f, false);


            Scribe_Values.LookValue(ref ColBarSettings.BaseSpacingHorizontal, "BaseSpacingHorizontal", 24f, false);
            Scribe_Values.LookValue(ref ColBarSettings.BaseSpacingVertical, "BaseSpacingVertical", 32f, false);
            Scribe_Values.LookValue(ref ColBarSettings.BaseSizeFloat, "BaseSizeFloat", 48f, false);
            Scribe_Values.LookValue(ref ColBarSettings.BaseIconSize, "BaseIconSize", 20f, false);
            Scribe_Values.LookValue(ref ColBarSettings.PawnTextureCameraHorizontalOffset, "PawnTextureCameraHorizontalOffset", 0f, false);
            Scribe_Values.LookValue(ref ColBarSettings.PawnTextureCameraVerticalOffset, "PawnTextureCameraVerticalOffset", 0.3f, false);
            Scribe_Values.LookValue(ref ColBarSettings.PawnTextureCameraZoom, "PawnTextureCameraZoom", 1.28205f, false);
            Scribe_Values.LookValue(ref ColBarSettings.MaxColonistBarWidth, "MaxColonistBarWidth");
            Scribe_Values.LookValue(ref ColBarSettings.MaxColonistBarHeight, "MaxColonistBarHeight");


            Scribe_Values.LookValue(ref ColBarSettings.DoubleClickTime, "DoubleClickTime", 0.5f, false);

            Scribe_Values.LookValue(ref ColBarSettings.FemaleColor, "FemaleColor");
            Scribe_Values.LookValue(ref ColBarSettings.MaleColor, "MaleColor");

            Scribe_Values.LookValue(ref ColBarSettings.MaxRows, "MaxRows");
            Scribe_Values.LookValue(ref ColBarSettings.SortBy, "SortBy");
            Scribe_Values.LookValue(ref ColBarSettings.useZoomToMouse, "useZoomToMouse");
            Scribe_Values.LookValue(ref ColBarSettings.moodRectScale, "moodRectScale");


            ColBarSettings.Reloadsettings = false;
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
