#if !NoCCL
using CommunityCoreLibrary;
using CommunityCoreLibrary.UI;
# endif
using System;
using System.Collections.Generic;
using System.IO;
using ColonistBarKF.ColorPicker;
using ColonistBarKF.PSI;
using UnityEngine;
using Verse;
using static ColonistBarKF.CBKF;
using static ColonistBarKF.PSI.PSI;
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

        public ColonistBarKF_Settings()
        {
            forcePause = false;
            doCloseX = true;
            draggable = true;
            drawShadow = true;
            preventCameraMotion = false;
            resizeable = true;
            onlyOneOfTypeAllowed = true;

        }

        public Window OptionsDialog;

        #endregion

        #region Methods

        public static int lastupdate = -5000;

        public override void WindowUpdate()
        {
            ColonistBar_KF.BaseSize = new Vector2(ColBarSettings.BaseSizeFloat, ColBarSettings.BaseSizeFloat);
            ColonistBar_KF.PawnTextureSize = new Vector2(ColBarSettings.BaseSizeFloat - 2f, ColBarSettings.BaseSizeFloat * 1.5f);

            Reinit(false, true);

            //  if (Find.TickManager.TicksGame > lastupdate)
            //  {
            //
            //    //if (ColBarSettings.UseGender)
            //    //    ColonistBarTextures.BGTex = ColonistBarTextures.BGTexGrey;
            //    //else
            //    //{
            //    //    ColonistBarTextures.BGTex = ColonistBarTextures.BGTexVanilla;
            //    //}
            //      lastupdate = Find.TickManager.TicksGame + 1500;
            //  }
        }


#if NoCCL
        public override void PreClose()
        {
            SaveBarSettings();
            SavePsiSettings();
        }



        public override Vector2 InitialSize => new Vector2(512f, 690f);

        private int mainToolbarInt = 0;
        private int psiToolbarInt = 0;
        private int barPositionInt = 0;
        private int psiBarPositionInt = 0;
        private int moodBarPositionInt = 0;
        private int psiPositionInt = 0;

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
            "PSI.Settings.SensitivityButton".Translate(),
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

        private int BarPositionInt
        {
            get
            {
                if (ColBarSettings.ColBarPos == Position.Alignment.Left)
                {
                    barPositionInt = 0;
                }
                if (ColBarSettings.ColBarPos == Position.Alignment.Right)
                {
                    barPositionInt = 1;
                }
                if (ColBarSettings.ColBarPos == Position.Alignment.Top)
                {
                    barPositionInt = 2;
                }
                if (ColBarSettings.ColBarPos == Position.Alignment.Bottom)
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
                        ColBarSettings.ColBarPos = Position.Alignment.Left;
                        break;
                    case 1:
                        ColBarSettings.ColBarPos = Position.Alignment.Right;

                        break;
                    case 2:
                        ColBarSettings.ColBarPos = Position.Alignment.Top;
                        break;
                    case 3:
                        ColBarSettings.ColBarPos = Position.Alignment.Bottom;
                        break;
                    default:
                        ColBarSettings.ColBarPos = Position.Alignment.Left;
                        break;
                }

                barPositionInt = value;
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
            normal = { textColor = Color.white },
            padding = new RectOffset(0, 0, 5, 0),
        };

        readonly GUIStyle _headline = new GUIStyle
        {
            fontStyle = FontStyle.Bold,
            fontSize = 16,
            normal = { textColor = Color.white },
            padding = new RectOffset(0, 0, 12, 6),
        };

        readonly GUIStyle _fondBoxes = new GUIStyle
        {
            normal = { background = ColonistBarTextures.DarkGrayFond },
            hover = { background = ColonistBarTextures.GrayFond },
            padding = new RectOffset(15, 15, 6, 10),
            margin = new RectOffset(0, 0, 10, 10)
        };

        readonly GUIStyle _fondImages = new GUIStyle
        {
            normal = { background = ColonistBarTextures.DarkGrayFond },
            hover = { background = ColonistBarTextures.RedHover },
        };

        readonly GUIStyle _darkGrayBgImage = new GUIStyle
        {
            normal = { background = ColonistBarTextures.GrayFond },
        };

        readonly GUIStyle _grayLines = new GUIStyle
        {
            normal = { background = ColonistBarTextures.GrayLines },
        };


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

            BeginArea(viewRect);
            BeginVertical();

            BeginHorizontal();
            FlexibleSpace();
            Label("Colonist Bar KF 0.15.3", _headline);
            FlexibleSpace();
            EndHorizontal();

            Space(Text.LineHeight / 2);

            BeginHorizontal();
            FlexibleSpace();
            MainToolbarInt = Toolbar(MainToolbarInt, mainToolbarStrings);
            FlexibleSpace();
            EndHorizontal();


            switch (MainToolbarInt)
            {
                case 0:
                    {

                        Space(Text.LineHeight);

                        _scrollPosition = BeginScrollView(_scrollPosition);

                        LabelHeadline("CBKF.Settings.BarPosition".Translate());

                        BeginVertical();

                        BeginHorizontal();
                        BarPositionInt = Toolbar(BarPositionInt, positionStrings);
                        FlexibleSpace();
                        EndHorizontal();

                        FillPageMain();

                        EndVertical();

                        LabelHeadline("CBKF.Settings.Advanced".Translate());

                        BeginVertical();
                        FillPageAdvanced();
                        EndVertical();

                        EndScrollView();

                    }
                    break;
                case 1:
                    {

                        //                 LabelHeadline("PSI.Settings".Translate());
                        Space(Text.LineHeight);


                        int toolbarInt = Mathf.FloorToInt(viewRect.width / 150f);
                        if (toolbarInt == 0)
                        {
                            toolbarInt += 1;
                        }
                        BeginHorizontal();
                        FlexibleSpace();
                        PSIToolbarInt = SelectionGrid(PSIToolbarInt, psiToolbarStrings, toolbarInt > psiToolbarStrings.Length ? psiToolbarStrings.Length : toolbarInt);
                        FlexibleSpace();
                        EndHorizontal();

                        Space(Text.LineHeight / 2);

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
                        //else if (PSIToolbarInt == 4)
                        //{
                        //    {
                        //        FillPagePSILoadIconset();
                        //    }
                        //}
                        else
                        {
                            FillPagePSIIconSet(viewRect);
                        }

                    }
                    break;
            }

            FlexibleSpace();
            Space(Text.LineHeight / 2);
            Label("", _grayLines, Height(1));
            Space(Text.LineHeight / 2);
            BeginHorizontal();
            if (Button("CBKF.Settings.RevertSettings".Translate()))
            {
                ResetBarSettings();
            }
            Space(Text.LineHeight / 2);
            if (Button("PSI.Settings.RevertSettings".Translate()))
            {
                ResetPSISettings();
            }
            FlexibleSpace();
            EndHorizontal();

            EndVertical();
            EndArea();

#if !NoCCL
            return 1000f;
#endif
        }

        private void LabelHeadline(string labelstring)
        {
            Space(Text.LineHeight / 2);
            Label("", _grayLines, Height(1));
            Space(Text.LineHeight / 4);
            BeginHorizontal();
            FlexibleSpace();
            Label(labelstring, _fontBold);
            FlexibleSpace();
            EndHorizontal();
            Space(Text.LineHeight / 2);
            Label("", _grayLines, Height(1));
            Space(Text.LineHeight / 2);
        }

        private void ResetBarSettings()
        {
            ColBarSettings = new SettingsColonistBar();
        }

        private void ResetPSISettings()
        {
            PsiSettings = new SettingsPSI();
        }

        private void FillPageMain()
        {
            #region Colonist bar position

            #region Vertical Alignment

            if (ColBarSettings.ColBarPos == Position.Alignment.Right)
            {
                BeginVertical(_fondBoxes);
                ColBarSettings.UseCustomMarginRight = Toggle(ColBarSettings.UseCustomMarginRight, "CBKF.Settings.ColonistBarOffset".Translate() +
                   (int)ColBarSettings.MarginRightVer + " xMax, " +
                   (int)ColBarSettings.MarginTopVerRight + " yMin, " +
                   (int)ColBarSettings.MarginBottomVerRight + " yMax"
                   );
                if (ColBarSettings.UseCustomMarginRight)
                {
                    Space(Text.LineHeight / 2);
                    ColBarSettings.MarginRightVer = HorizontalSlider(ColBarSettings.MarginRightVer, 0f, Screen.width / 6);
                    ColBarSettings.MarginTopVerRight = HorizontalSlider(ColBarSettings.MarginTopVerRight, 0f, Screen.height * 2 / 5);
                    ColBarSettings.MarginBottomVerRight = HorizontalSlider(ColBarSettings.MarginBottomVerRight, 0f, Screen.height * 2 / 5);
                }
                else
                {
                    ColBarSettings.MarginRightVer = 21f;
                    ColBarSettings.MarginTopVerRight = 120f;
                    ColBarSettings.MarginBottomVerRight = 120f;
                    ColBarSettings.MaxColonistBarHeight = Screen.height - ColBarSettings.MarginTopVerRight - ColBarSettings.MarginBottomVerRight;
                    ColBarSettings.VerticalOffset = ColBarSettings.MarginTopVerRight / 2 - ColBarSettings.MarginBottomVerRight / 2;
                }
                EndVertical();

            }
            if (ColBarSettings.ColBarPos == Position.Alignment.Left)
            {
                BeginVertical(_fondBoxes);
                ColBarSettings.UseCustomMarginLeft = Toggle(ColBarSettings.UseCustomMarginLeft, "CBKF.Settings.ColonistBarOffset".Translate() +
                   (int)ColBarSettings.MarginLeftVer + " xMin, " +
                   (int)ColBarSettings.MarginTopVerLeft + " yMin, " +
                   (int)ColBarSettings.MarginBottomVerLeft + " yMax"
                   );
                if (ColBarSettings.UseCustomMarginLeft)
                {
                    Space(Text.LineHeight / 2);
                    ColBarSettings.MarginLeftVer = HorizontalSlider(ColBarSettings.MarginLeftVer, 0f, Screen.width / 6);
                    ColBarSettings.MarginTopVerLeft = HorizontalSlider(ColBarSettings.MarginTopVerLeft, 0f, Screen.height * 2 / 5);
                    ColBarSettings.MarginBottomVerLeft = HorizontalSlider(ColBarSettings.MarginBottomVerLeft, 0f, Screen.height * 2 / 5);
                }
                else
                {
                    ColBarSettings.MarginLeftVer = 21f;
                    ColBarSettings.MarginTopVerLeft = 120f;
                    ColBarSettings.MarginBottomVerLeft = 120f;
                    ColBarSettings.MaxColonistBarHeight = Screen.height - ColBarSettings.MarginTopVerLeft - ColBarSettings.MarginBottomVerLeft;
                    ColBarSettings.VerticalOffset = ColBarSettings.MarginTopVerLeft / 2 - ColBarSettings.MarginBottomVerLeft / 2;
                }
                EndVertical();
            }



            //  listing.Gap(3f);
#if !NoCCL
                listing.Undent();
#endif

            #endregion

            #region Horizontal alignment


            if (ColBarSettings.ColBarPos == Position.Alignment.Bottom)
            {
                BeginVertical(_fondBoxes);
                ColBarSettings.UseCustomMarginBottom = Toggle(ColBarSettings.UseCustomMarginBottom, "CBKF.Settings.ColonistBarOffset".Translate() +
                   (int)ColBarSettings.MarginBottomHor + " yMax, " +
                   (int)ColBarSettings.MarginLeftHorBottom + " xMin, " +
                   (int)ColBarSettings.MarginRightHorBottom + " xMax"
                   );

                if (ColBarSettings.UseCustomMarginBottom)
                {
                    Space(Text.LineHeight / 2);
                    ColBarSettings.MarginBottomHor = ColBarSettings.MarginBottomHor = HorizontalSlider(ColBarSettings.MarginBottomHor, 10, Screen.height / 6);
                    ColBarSettings.MarginLeftHorBottom = HorizontalSlider(ColBarSettings.MarginLeftHorBottom, 0f, Screen.width * 2 / 5);
                    ColBarSettings.MarginRightHorBottom = HorizontalSlider(ColBarSettings.MarginRightHorBottom, 0f, Screen.width * 2 / 5);
                }
                else
                {
                    ColBarSettings.MarginBottomHor = 21f;
                    ColBarSettings.MarginLeftHorBottom = 160f;
                    ColBarSettings.MarginRightHorBottom = 160f;
                    ColBarSettings.MaxColonistBarWidth = Screen.width - ColBarSettings.MarginLeftHorBottom - ColBarSettings.MarginRightHorBottom;
                    ColBarSettings.HorizontalOffset = ColBarSettings.MarginLeftHorBottom / 2 - ColBarSettings.MarginRightHorBottom / 2;
                }
                EndVertical();
            }
            if (ColBarSettings.ColBarPos == Position.Alignment.Top)
            {
                BeginVertical(_fondBoxes);
                ColBarSettings.UseCustomMarginTopHor = Toggle(ColBarSettings.UseCustomMarginTopHor, "CBKF.Settings.ColonistBarOffset".Translate() +
                   (int)ColBarSettings.MarginTopHor + " yMin, " +
                   (int)ColBarSettings.MarginLeftHorTop + " xMin, " +
                   (int)ColBarSettings.MarginRightHorTop + " xMax"
                   );

                if (ColBarSettings.UseCustomMarginTopHor)
                {
                    Space(Text.LineHeight / 2);
                    ColBarSettings.MarginTopHor = HorizontalSlider(ColBarSettings.MarginTopHor, 10, Screen.height / 6);
                    ColBarSettings.MarginLeftHorTop = HorizontalSlider(ColBarSettings.MarginLeftHorTop, 0f, Screen.width * 2 / 5);
                    ColBarSettings.MarginRightHorTop = HorizontalSlider(ColBarSettings.MarginRightHorTop, 0f, Screen.width * 2 / 5);
                }
                else
                {
                    ColBarSettings.MarginTopHor = 21f;
                    ColBarSettings.MarginLeftHorTop = 160f;
                    ColBarSettings.MarginRightHorTop = 160f;
                    ColBarSettings.MaxColonistBarWidth = Screen.width - ColBarSettings.MarginLeftHorTop - ColBarSettings.MarginRightHorTop;
                    ColBarSettings.HorizontalOffset = ColBarSettings.MarginLeftHorTop / 2 - ColBarSettings.MarginRightHorTop / 2;
                }
                //  listing.Gap(3f);
                EndVertical();


                //  listing.Gap(3f);
            }
#if !NoCCL
                listing.Undent();
#endif

            #endregion
            #endregion



            #region Max Rows
            BeginVertical(_fondBoxes);
            ColBarSettings.UseCustomRowCount = Toggle(ColBarSettings.UseCustomRowCount, "PSI.Settings.Arrangement.IconsPerColumn".Translate() + (ColBarSettings.UseCustomRowCount ? ColBarSettings.MaxRowsCustom : 3));
            if (ColBarSettings.UseCustomRowCount)
            {
                ColBarSettings.MaxRowsCustom = (int)HorizontalSlider(ColBarSettings.MaxRowsCustom, 1f, 5f);
            }
            EndVertical();
            #endregion

            #region Various



            BeginVertical(_fondBoxes);

            ColBarSettings.UseWeaponIcons = Toggle(ColBarSettings.UseWeaponIcons, "CBKF.Settings.UseWeaponIcons".Translate());

            ColBarSettings.UseGender = Toggle(ColBarSettings.UseGender, "CBKF.Settings.useGender".Translate());

            ColBarSettings.useZoomToMouse = Toggle(ColBarSettings.useZoomToMouse, "CBKF.Settings.useZoomToMouse".Translate());

            #region DoubleClickTime

            ColBarSettings.UseCustomDoubleClickTime = Toggle(ColBarSettings.UseCustomDoubleClickTime, "CBKF.Settings.DoubleClickTime".Translate() + ": " + ColBarSettings.DoubleClickTime.ToString("N2") + " s");
            if (ColBarSettings.UseCustomDoubleClickTime)
            {
                //       listing.Gap(3f);
                Space(Text.LineHeight / 2);
                ColBarSettings.DoubleClickTime = HorizontalSlider(ColBarSettings.DoubleClickTime, 0.1f, 1.5f);
            }
            else
            {
                ColBarSettings.DoubleClickTime = 0.5f;
            }

            #endregion

            EndVertical();

            #endregion

        }

        private void FillPageAdvanced()
        {
            #region Size + Spacing
            BeginVertical(_fondBoxes);

            ColBarSettings.UseCustomIconSize = Toggle(ColBarSettings.UseCustomIconSize, "CBKF.Settings.BasicSize".Translate() +
                (ColBarSettings.BaseSizeFloat).ToString("N0") + " px, " +
                (ColBarSettings.UseFixedIconScale ? (ColBarSettings.FixedIconScaleFloat * 100).ToString("N0") + " %, " : (ColonistBar_KF.CurrentScale * 100).ToString("N0") + " %, ") +
                (int)ColBarSettings.BaseSpacingHorizontal + " x, " +
            (int)ColBarSettings.BaseSpacingVertical + " y"
                );

            if (ColBarSettings.UseCustomIconSize)
            {
                Space(Text.LineHeight / 2);

                ColBarSettings.BaseSizeFloat = HorizontalSlider(ColBarSettings.BaseSizeFloat, 24f, 128f);

                ColBarSettings.BaseSpacingHorizontal = HorizontalSlider(ColBarSettings.BaseSpacingHorizontal, 1f, 72f);
                ColBarSettings.BaseSpacingVertical = HorizontalSlider(ColBarSettings.BaseSpacingVertical, 1f, 96f);
            }
            else
            {
                ColBarSettings.BaseSizeFloat = 48f;
                ColBarSettings.BaseSpacingHorizontal = 24f;
                ColBarSettings.BaseSpacingVertical = 32f;
            }

            #region Fixed Scaling
            ColBarSettings.UseFixedIconScale = Toggle(ColBarSettings.UseFixedIconScale, "CBKF.Settings.FixedScale".Translate());
            if (ColBarSettings.UseFixedIconScale)
            {
                ColBarSettings.FixedIconScaleFloat = HorizontalSlider(ColBarSettings.FixedIconScaleFloat, 0.2f, 2.5f);
            }
            else
            {
                ColBarSettings.FixedIconScaleFloat = 1f;
            }
            #endregion

            EndVertical();

            #endregion

            #region Camera

            BeginVertical(_fondBoxes);
            ColBarSettings.UseCustomPawnTextureCameraOffsets = Toggle(ColBarSettings.UseCustomPawnTextureCameraOffsets, "CBKF.Settings.PawnTextureCameraOffsets".Translate() +
                ColBarSettings.PawnTextureCameraHorizontalOffset.ToString("N2") + " x, " +
                ColBarSettings.PawnTextureCameraVerticalOffset.ToString("N2") + " y, " +
                ColBarSettings.PawnTextureCameraZoom.ToString("N2") + " z"
                );
            if (ColBarSettings.UseCustomPawnTextureCameraOffsets)
            {
                Space(Text.LineHeight / 2);
                ColBarSettings.PawnTextureCameraHorizontalOffset = HorizontalSlider(ColBarSettings.PawnTextureCameraHorizontalOffset, 0.7f, -0.7f);
                ColBarSettings.PawnTextureCameraVerticalOffset = HorizontalSlider(ColBarSettings.PawnTextureCameraVerticalOffset, 0f, 1f);
                ColBarSettings.PawnTextureCameraZoom = HorizontalSlider(ColBarSettings.PawnTextureCameraZoom, 0.3f, 3f);
            }
            else
            {
                ColBarSettings.PawnTextureCameraHorizontalOffset = 0f;
                ColBarSettings.PawnTextureCameraVerticalOffset = 0.3f;
                ColBarSettings.PawnTextureCameraZoom = 1.28205f;
            }

            EndVertical();

            #endregion



            #region Mood Bar

            BeginVertical(_fondBoxes);
            ColBarSettings.UseMoodColors = Toggle(ColBarSettings.UseMoodColors, "CBKF.Settings.UseMoodColors".Translate() +
                (ColBarSettings.moodRectScale * 100.0).ToString("N0") + " %, " +
                ColBarSettings.moodRectAlpha.ToString("N2") + " a"
                );

            if (ColBarSettings.UseMoodColors)
            {

                    Space(Text.LineHeight / 2);

                    BeginHorizontal();
                    MoodBarPositionInt = Toolbar(MoodBarPositionInt, positionStrings);
                    FlexibleSpace();
                    EndHorizontal();

                    Space(Text.LineHeight / 2);

                    Label("PSI.Settings.Arrangement.IconsPerColumn".Translate() + ColBarSettings.IconsInColumn);
                    ColBarSettings.IconsInColumn = (int)HorizontalSlider(ColBarSettings.IconsInColumn, 2f, 5f);
                
                Space(Text.LineHeight / 2);
                ColBarSettings.moodRectScale = HorizontalSlider(ColBarSettings.moodRectScale, 0.33f, 1f);
                ColBarSettings.moodRectAlpha = HorizontalSlider(ColBarSettings.moodRectAlpha, 0, 1f);
            }
            EndVertical();

            #endregion


            #region Gender


            if (ColBarSettings.UseGender)
            {

                if (Button("CBKF.Settings.FemaleColor".Translate()))
                {
                    while (Find.WindowStack.TryRemove(typeof(Dialog_ColorPicker)))
                    {
                    }
                    Find.WindowStack.Add(new Dialog_ColorPicker(colourWrapper, delegate
                    {
                        ColBarSettings.FemaleColor = colourWrapper.Color;
                    }, false, true)
                    {
                        initialPosition = new Vector2(windowRect.xMax + 10f, windowRect.yMin),
                    });
                }

                if (Button("CBKF.Settings.MaleColor".Translate()))
                {
                    while (Find.WindowStack.TryRemove(typeof(Dialog_ColorPicker)))
                    {
                    }
                    Find.WindowStack.Add(new Dialog_ColorPicker(colourWrapper, delegate
                    {
                        ColBarSettings.MaleColor = colourWrapper.Color;
                    }, false, true)
                    {
                        initialPosition = new Vector2(windowRect.xMax + 10f, windowRect.yMin),
                    });
                }

                if (Button("CBKF.Settings.ResetColors".Translate()))
                {
                    ColBarSettings.FemaleColor = new Color(1f, 0.64f, 0.8f, 1f);
                    ColBarSettings.MaleColor = new Color(0.52f, 0.75f, 0.92f, 1f);
                }
            }
            #endregion


        }

        private void FillPagePSIIconSet(Rect viewRect)
        {

            BeginHorizontal();
            if (Button("PSI.Settings.IconSet".Translate() + PsiSettings.IconSet))
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>();

                options.Add(new FloatMenuOption("PSI.Settings.Preset.0".Translate(), () =>
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
                options.Add(new FloatMenuOption("PSI.Settings.Preset.1".Translate(), () =>
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
            FlexibleSpace();
            EndHorizontal();


            _iconLimit = Mathf.FloorToInt(viewRect.width / 125f);


            Space(Text.LineHeight / 2);

            _scrollPosition = BeginScrollView(_scrollPosition);

            int num = 0;
            BeginHorizontal();

            DrawCheckboxAreaTarget("PSI.Settings.Visibility.TargetPoint".Translate(), PSIMaterials[Icons.Target], PSIMaterials[Icons.TargetHair], PSIMaterials[Icons.TargetSkin], ref num);

            DrawCheckboxArea("PSI.Settings.Visibility.Draft".Translate(), PSIMaterials[Icons.Draft], ref ColBarSettings.ShowDraft, ref PsiSettings.ShowDraft, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.Unarmed".Translate(), PSIMaterials[Icons.Unarmed], ref ColBarSettings.ShowUnarmed, ref PsiSettings.ShowUnarmed, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.Idle".Translate(), PSIMaterials[Icons.Idle], ref ColBarSettings.ShowIdle, ref PsiSettings.ShowIdle, ref num);

            DrawCheckboxArea("PSI.Settings.Visibility.Sad".Translate(), PSIMaterials[Icons.Sad], ref ColBarSettings.ShowSad, ref PsiSettings.ShowSad, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.Aggressive".Translate(), PSIMaterials[Icons.Aggressive], ref ColBarSettings.ShowAggressive, ref PsiSettings.ShowAggressive, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.Panic".Translate(), PSIMaterials[Icons.Panic], ref ColBarSettings.ShowPanic, ref PsiSettings.ShowPanic, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.Dazed".Translate(), PSIMaterials[Icons.Dazed], ref ColBarSettings.ShowDazed, ref PsiSettings.ShowDazed, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.Leave".Translate(), PSIMaterials[Icons.Leave], ref ColBarSettings.ShowLeave, ref PsiSettings.ShowLeave, ref num);

            DrawCheckboxArea("PSI.Settings.Visibility.Hungry".Translate(), PSIMaterials[Icons.Hungry], ref ColBarSettings.ShowHungry, ref PsiSettings.ShowHungry, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.Tired".Translate(), PSIMaterials[Icons.Tired], ref ColBarSettings.ShowTired, ref PsiSettings.ShowTired, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.TooCold".Translate(), PSIMaterials[Icons.TooCold], ref ColBarSettings.ShowTooCold, ref PsiSettings.ShowTooCold, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.TooHot".Translate(), PSIMaterials[Icons.TooHot], ref ColBarSettings.ShowTooHot, ref PsiSettings.ShowTooHot, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.ApparelHealth".Translate(), PSIMaterials[Icons.ApparelHealth], ref ColBarSettings.ShowApparelHealth, ref PsiSettings.ShowApparelHealth, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.Naked".Translate(), PSIMaterials[Icons.Naked], ref ColBarSettings.ShowNaked, ref PsiSettings.ShowNaked, ref num);

            DrawCheckboxArea("PSI.Settings.Visibility.Health".Translate(), PSIMaterials[Icons.Health], ref ColBarSettings.ShowHealth, ref PsiSettings.ShowHealth, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.MedicalAttention".Translate(), PSIMaterials[Icons.MedicalAttention], ref ColBarSettings.ShowMedicalAttention, ref PsiSettings.ShowMedicalAttention, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.Injury".Translate(), PSIMaterials[Icons.Effectiveness], ref ColBarSettings.ShowEffectiveness, ref PsiSettings.ShowEffectiveness, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.Bloodloss".Translate(), PSIMaterials[Icons.Bloodloss], ref ColBarSettings.ShowBloodloss, ref PsiSettings.ShowBloodloss, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.Sickness".Translate(), PSIMaterials[Icons.Sickness], ref ColBarSettings.ShowMedicalAttention, ref PsiSettings.ShowMedicalAttention, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.Pain".Translate(), PSIMaterials[Icons.Pain], ref ColBarSettings.ShowPain, ref PsiSettings.ShowPain, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.Drunk".Translate(), PSIMaterials[Icons.Drunk], ref ColBarSettings.ShowDrunk, ref PsiSettings.ShowDrunk, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.Toxicity".Translate(), PSIMaterials[Icons.Toxicity], ref ColBarSettings.ShowToxicity, ref PsiSettings.ShowToxicity, ref num);

            DrawCheckboxArea("PSI.Settings.Visibility.NightOwl".Translate(), PSIMaterials[Icons.NightOwl], ref ColBarSettings.ShowNightOwl, ref PsiSettings.ShowNightOwl, ref num);

            DrawCheckboxArea("PSI.Settings.Visibility.LeftUnburied".Translate(), PSIMaterials[Icons.LeftUnburied], ref ColBarSettings.ShowLeftUnburied, ref PsiSettings.ShowLeftUnburied, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.Crowded".Translate(), PSIMaterials[Icons.Crowded], ref ColBarSettings.ShowCrowded, ref PsiSettings.ShowRoomStatus, ref num);

            DrawCheckboxArea("PSI.Settings.Visibility.Bedroom".Translate(), PSIMaterials[Icons.Bedroom], ref ColBarSettings.ShowBedroom, ref PsiSettings.ShowBedroom, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.Greedy".Translate(), PSIMaterials[Icons.Greedy], ref ColBarSettings.ShowGreedy, ref PsiSettings.ShowGreedy, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.DeadColonists".Translate(), PSIMaterials[Icons.DeadColonist], ref ColBarSettings.ShowDeadColonists, ref PsiSettings.ShowDeadColonists, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.Jealous".Translate(), PSIMaterials[Icons.Jealous], ref ColBarSettings.ShowJealous, ref PsiSettings.ShowJealous, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.Pyromaniac".Translate(), PSIMaterials[Icons.Pyromaniac], ref ColBarSettings.ShowPyromaniac, ref PsiSettings.ShowPyromaniac, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.Prosthophile".Translate(), PSIMaterials[Icons.Prosthophile], ref ColBarSettings.ShowProsthophile, ref PsiSettings.ShowProsthophile, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.Prosthophobe".Translate(), PSIMaterials[Icons.Prosthophobe], ref ColBarSettings.ShowProsthophobe, ref PsiSettings.ShowProsthophobe, ref num);
            DrawCheckboxArea("PSI.Settings.Visibility.Pacific".Translate(), PSIMaterials[Icons.Pacific], ref ColBarSettings.ShowPacific, ref PsiSettings.ShowPacific, ref num);

            DrawCheckboxArea("PSI.Settings.Visibility.Lovers".Translate(), PSIMaterials[Icons.Love], ref ColBarSettings.ShowLove, ref PsiSettings.ShowLove, ref num);
            //      DrawCheckboxArea("PSI.Settings.Visibility.Marriage".Translate(), PSIMaterials[Icons.Marriage], ref ColBarSettings.ShowMarriage, ref PsiSettings.ShowMarriage, ref num);

            EndHorizontal();

            //  PsiSettings.ShowIdle = Toggle(PsiSettings.ShowIdle, "PSI.Settings.Visibility.Idle".Translate());
            //  PsiSettings.ShowUnarmed = Toggle(PsiSettings.ShowUnarmed, "PSI.Settings.Visibility.Unarmed".Translate());
            //  PsiSettings.ShowHungry = Toggle(PsiSettings.ShowHungry, "PSI.Settings.Visibility.Hungry".Translate());
            //  PsiSettings.ShowSad = Toggle(PsiSettings.ShowSad, "PSI.Settings.Visibility.Sad".Translate());
            //  PsiSettings.ShowTired = Toggle(PsiSettings.ShowTired, "PSI.Settings.Visibility.Tired".Translate());
            //  //
            //  PsiSettings.ShowMedicalAttention = Toggle(PsiSettings.ShowMedicalAttention, "PSI.Settings.Visibility.Sickness".Translate());
            //  PsiSettings.ShowPain = Toggle(PsiSettings.ShowPain, "PSI.Settings.Visibility.Pain".Translate());
            //  PsiSettings.ShowHealth = Toggle(PsiSettings.ShowHealth, "PSI.Settings.Visibility.Health".Translate());
            //  PsiSettings.ShowEffectiveness = Toggle(PsiSettings.ShowEffectiveness, "PSI.Settings.Visibility.Injury".Translate());
            //  PsiSettings.ShowBloodloss = Toggle(PsiSettings.ShowBloodloss, "PSI.Settings.Visibility.Bloodloss".Translate());
            //  //
            //  PsiSettings.ShowTooHot = Toggle(PsiSettings.ShowTooHot, "PSI.Settings.Visibility.TooHot".Translate());
            //  PsiSettings.ShowTooCold = Toggle(PsiSettings.ShowTooCold, "PSI.Settings.Visibility.TooCold".Translate());
            //  PsiSettings.ShowNaked = Toggle(PsiSettings.ShowNaked, "PSI.Settings.Visibility.Naked".Translate());
            //  PsiSettings.ShowDrunk = Toggle(PsiSettings.ShowDrunk, "PSI.Settings.Visibility.Drunk".Translate());
            //  PsiSettings.ShowApparelHealth = Toggle(PsiSettings.ShowApparelHealth, "PSI.Settings.Visibility.ApparelHealth".Translate());
            //  //
            //  PsiSettings.ShowPacific = Toggle(PsiSettings.ShowPacific, "PSI.Settings.Visibility.Pacific".Translate());
            //  PsiSettings.ShowNightOwl = Toggle(PsiSettings.ShowNightOwl, "PSI.Settings.Visibility.NightOwl".Translate());
            //  PsiSettings.ShowGreedy = Toggle(PsiSettings.ShowGreedy, "PSI.Settings.Visibility.Greedy".Translate());
            //  PsiSettings.ShowJealous = Toggle(PsiSettings.ShowJealous, "PSI.Settings.Visibility.Jealous".Translate());
            //  PsiSettings.ShowLove = Toggle(PsiSettings.ShowLove, "PSI.Settings.Visibility.Lovers".Translate());
            //  //
            //  PsiSettings.ShowProsthophile = Toggle(PsiSettings.ShowProsthophile, "PSI.Settings.Visibility.Prosthophile".Translate());
            //  PsiSettings.ShowProsthophobe = Toggle(PsiSettings.ShowProsthophobe, "PSI.Settings.Visibility.Prosthophobe".Translate());
            //  PsiSettings.ShowCrowded = Toggle(PsiSettings.ShowCrowded, "PSI.Settings.Visibility.Crowded".Translate());
            //  PsiSettings.ShowBedroom = Toggle(PsiSettings.ShowBedroom, "PSI.Settings.Visibility.Bedroom".Translate());
            //
            //  PsiSettings.ShowDeadColonists = Toggle(PsiSettings.ShowDeadColonists, "PSI.Settings.Visibility.ShowDeadColonists".Translate());
            //  PsiSettings.ShowPyromaniac = Toggle(PsiSettings.ShowPyromaniac, "PSI.Settings.Visibility.Pyromaniac".Translate());
            Space(Text.LineHeight / 2);
            EndScrollView();

        }

        private static int _iconLimit;
        private static ColorWrapper colourWrapper;

        private void DrawCheckboxArea(string iconName, Material iconMaterial, ref bool colBarBool, ref bool psiBarBool, ref int iconInRow)
        {
            if (iconInRow > _iconLimit - 1)
            {
                EndHorizontal();

                Space(Text.LineHeight / 2);

                BeginHorizontal();
                iconInRow = 0;
            }
            if (iconInRow > 0 && _iconLimit != 1)
                Space(Text.LineHeight / 2);

            BeginVertical(_fondImages);

            BeginHorizontal(_darkGrayBgImage, Height(Text.LineHeight * 1.2f));
            FlexibleSpace();
            Label(iconName, _fontBold);
            FlexibleSpace();
            EndHorizontal();

            BeginHorizontal();
            FlexibleSpace();

            BeginVertical();
            Space(Text.LineHeight / 2);

            if (iconMaterial != null)
            {
                Label(iconMaterial.mainTexture, Width(Text.LineHeight * 2.5f), Height(Text.LineHeight * 2.5f));
            }
            else
            {
                if (psiBarBool || colBarBool)
                {
                    GUI.color = Color.red;
                    Label("PSI.Settings.IconSet.IconNotAvailable".Translate(), Width(Text.LineHeight * 2.5f), Height(Text.LineHeight * 2.5f));
                    GUI.color = Color.white;
                }
                else
                {
                    Label("", Width(Text.LineHeight * 2.5f), Height(Text.LineHeight * 2.5f));
                }
            }
            Space(Text.LineHeight / 2);
            //      Label("PSI.Settings.VisibilityButton".Translate(), FontBold);
            colBarBool = Toggle(colBarBool, "CBKF.Settings.ColBarIconVisibility".Translate());
            psiBarBool = Toggle(psiBarBool, "CBKF.Settings.PSIIconVisibility".Translate());
            Space(Text.LineHeight / 2);

            EndVertical();

            FlexibleSpace();
            EndHorizontal();

            EndVertical();

            iconInRow++;
        }

        private void DrawCheckboxAreaTarget(string iconName, Material targetSingle, Material targetHair, Material targetSkin, ref int iconInRow)
        {

            BeginVertical(_fondImages);

            BeginHorizontal(_darkGrayBgImage, Height(Text.LineHeight * 1.2f));
            FlexibleSpace();
            Label(iconName, _fontBold);
            FlexibleSpace();
            EndHorizontal();

            BeginHorizontal();
            FlexibleSpace();

            BeginVertical();
            Space(Text.LineHeight / 2);
            if (PsiSettings.ShowTargetPoint)
            {
                if (!PsiSettings.UseColoredTarget)
                {
                    if (targetSingle != null)
                    {
                        Label(targetSingle.mainTexture, Width(Text.LineHeight * 2.5f), Height(Text.LineHeight * 2.5f));
                    }
                    else
                    {
                        GUI.color = Color.red;
                        Label("PSI.Settings.IconSet.IconNotAvailable".Translate(), Width(Text.LineHeight * 2.5f), Height(Text.LineHeight * 2.5f));
                        GUI.color = Color.white;
                    }
                }
                else
                {
                    BeginHorizontal();
                    if (targetHair != null)
                    {
                        Label(targetHair.mainTexture, Width(Text.LineHeight * 2.5f), Height(Text.LineHeight * 2.5f));
                    }
                    else
                    {
                        GUI.color = Color.red;
                        Label("PSI.Settings.IconSet.IconNotAvailable".Translate() + " HairTarget", Width(Text.LineHeight * 2.5f), Height(Text.LineHeight * 2.5f));
                        GUI.color = Color.white;
                    }
                    if (targetSkin != null)
                    {
                        Label(targetSkin.mainTexture, Width(Text.LineHeight * 2.5f), Height(Text.LineHeight * 2.5f));
                    }
                    else
                    {
                        GUI.color = Color.red;
                        Label("PSI.Settings.IconSet.IconNotAvailable".Translate() + " SkinTarget", Width(Text.LineHeight * 2.5f), Height(Text.LineHeight * 2.5f));
                        GUI.color = Color.white;
                    }
                    EndHorizontal();
                }

            }
            else
            {
                Label("", Width(Text.LineHeight * 2.5f), Height(Text.LineHeight * 2.5f));

            }
            Space(Text.LineHeight / 2);
            //      Label("PSI.Settings.VisibilityButton".Translate(), FontBold);
            PsiSettings.ShowTargetPoint = Toggle(PsiSettings.ShowTargetPoint, "PSI.Settings.Visibility.TargetPoint".Translate());
            PsiSettings.UseColoredTarget = Toggle(PsiSettings.UseColoredTarget, "PSI.Settings.IconOpacityAndColor.UseColoredTarget".Translate());

            Space(Text.LineHeight / 2);

            EndVertical();

            FlexibleSpace();
            EndHorizontal();

            EndVertical();

            iconInRow += 2;
        }


        private void FillPagePSIOpacityAndColor()
        {

            _scrollPosition = BeginScrollView(_scrollPosition);

            BeginVertical(_fondBoxes);
            Label("PSI.Settings.IconOpacityAndColor.Opacity".Translate() + (PsiSettings.IconOpacity * 100).ToString("N0") + " %");
            PsiSettings.IconOpacity = HorizontalSlider(PsiSettings.IconOpacity, 0.05f, 1f);
            EndVertical();

            BeginVertical(_fondBoxes);
            Label("PSI.Settings.IconOpacityAndColor.OpacityCritical".Translate() + (PsiSettings.IconOpacityCritical * 100).ToString("N0") + " %");
            PsiSettings.IconOpacityCritical = HorizontalSlider(PsiSettings.IconOpacityCritical, 0f, 1f);
            EndVertical();



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

            EndScrollView();

            //  if (listing.DoTextButton("PSI.Settings.ReturnButton".Translate()))
            //      Page = "main";
        }

        private void FillPSIPageSizeArrangement()
        {

            _scrollPosition = BeginScrollView(_scrollPosition);

            #region PSI on Bar
            BeginVertical(_fondBoxes);
            ColBarSettings.UsePsi = Toggle(ColBarSettings.UsePsi, "CBKF.Settings.UsePsiOnBar".Translate());
            if (ColBarSettings.UsePsi)
            {
                Space(Text.LineHeight / 2);

                BeginHorizontal();
                PsiBarPositionInt = Toolbar(PsiBarPositionInt, positionStrings);
                FlexibleSpace();
                EndHorizontal();

                Space(Text.LineHeight / 2);

                Label("PSI.Settings.Arrangement.IconsPerColumn".Translate() + ColBarSettings.IconsInColumn);
                ColBarSettings.IconsInColumn = (int)HorizontalSlider(ColBarSettings.IconsInColumn, 2f, 5f);
            }
            EndVertical();

            #endregion

            #region PSI on Colonist

            BeginVertical(_fondBoxes);
            PsiSettings.UsePsi = Toggle(PsiSettings.UsePsi, "PSI.Settings.UsePSI".Translate());
            PsiSettings.UsePsiOnPrisoner = Toggle(PsiSettings.UsePsiOnPrisoner, "PSI.Settings.UsePSIOnPrisoner".Translate());

            if ( PsiSettings.UsePsi || PsiSettings.UsePsiOnPrisoner)
            {
                
                Space(Text.LineHeight / 2);

                BeginHorizontal();
                PsiPositionInt = Toolbar(PsiPositionInt, positionStrings);
                FlexibleSpace();
                EndHorizontal();

                Space(Text.LineHeight / 2);

                PsiSettings.IconsHorizontal = Toggle(PsiSettings.IconsHorizontal, "PSI.Settings.Arrangement.Horizontal".Translate());

                PsiSettings.IconsScreenScale = Toggle(PsiSettings.IconsScreenScale, "PSI.Settings.Arrangement.ScreenScale".Translate());

                Space(Text.LineHeight / 2);

                Label("PSI.Settings.Arrangement.IconsPerColumn".Translate() + PsiSettings.IconsInColumn);
                PsiSettings.IconsInColumn = (int)HorizontalSlider(PsiSettings.IconsInColumn, 1f, 7f);

                int num = (int)(PsiSettings.IconSize * 4.5);

                if (num > 8)
                    num = 8;
                else if (num < 0)
                    num = 0;

                Space(Text.LineHeight / 2);
                Label("PSI.Settings.Arrangement.IconSize".Translate() + ("PSI.Settings.SizeLabel." + num).Translate());
                PsiSettings.IconSize = HorizontalSlider(PsiSettings.IconSize, 0.5f, 2f);
                Space(Text.LineHeight / 2);



                BeginVertical(_fondBoxes);
                Label("PSI.Settings.Arrangement.IconPosition".Translate() +
                    (int)(PsiSettings.IconDistanceX * 100.0) + " x, " +
                    (int)(PsiSettings.IconDistanceY * 100.0) + " y"
                    );
                PsiSettings.IconDistanceX = HorizontalSlider(PsiSettings.IconDistanceX, -2f, 2f);
                PsiSettings.IconDistanceY = HorizontalSlider(PsiSettings.IconDistanceY, -2f, 2f);
                EndVertical();

                BeginVertical(_fondBoxes);
                Label("PSI.Settings.Arrangement.IconOffset".Translate() +
                    (int)(PsiSettings.IconOffsetX * 100.0) + " x, " +
                    (int)(PsiSettings.IconOffsetY * 100.0) + " y"
                    );
                PsiSettings.IconOffsetX = HorizontalSlider(PsiSettings.IconOffsetX, -2f, 2f);
                PsiSettings.IconOffsetY = HorizontalSlider(PsiSettings.IconOffsetY, -2f, 2f);
                EndVertical();


                //   if (!listing.DoTextButton("PSI.Settings.ReturnButton".Translate()))
                //       return;
                //
                //   Page = "main";
            }
            EndVertical();
            #endregion

            EndScrollView();
        }


        private void FillPSIPageSensitivity()
        {

            BeginHorizontal();
            if (Button("PSI.Settings.LoadPresetButton".Translate()))
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>
                {
                    new FloatMenuOption("Less Sensitive", () =>
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
                    }),
                    new FloatMenuOption("Standard", () =>
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
                    }),
                    new FloatMenuOption("More Sensitive", () =>
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
                    })
                };

                Find.WindowStack.Add(new FloatMenu(options));
            }

            FlexibleSpace();
            EndHorizontal();

            _scrollPosition = BeginScrollView(_scrollPosition);

            BeginVertical(_fondBoxes);
            Label("PSI.Settings.Sensitivity.Bleeding".Translate() + ("PSI.Settings.Sensitivity.Bleeding." + Math.Round(PsiSettings.LimitBleedMult - 0.25)).Translate());
            PsiSettings.LimitBleedMult = HorizontalSlider(PsiSettings.LimitBleedMult, 0.5f, 5f);
            EndVertical();


            BeginVertical(_fondBoxes);
            Label("PSI.Settings.Sensitivity.Injured".Translate() + (int)(PsiSettings.LimitEfficiencyLess * 100.0) + " %");
            PsiSettings.LimitEfficiencyLess = HorizontalSlider(PsiSettings.LimitEfficiencyLess, 0.01f, 0.99f);
            EndVertical();


            BeginVertical(_fondBoxes);
            Label("PSI.Settings.Sensitivity.Food".Translate() + (int)(PsiSettings.LimitFoodLess * 100.0) + " %");
            PsiSettings.LimitFoodLess = HorizontalSlider(PsiSettings.LimitFoodLess, 0.01f, 0.99f);
            EndVertical();


            BeginVertical(_fondBoxes);
            Label("PSI.Settings.Sensitivity.Rest".Translate() + (int)(PsiSettings.LimitRestLess * 100.0) + " %");
            PsiSettings.LimitRestLess = HorizontalSlider(PsiSettings.LimitRestLess, 0.01f, 0.99f);
            EndVertical();


            BeginVertical(_fondBoxes);
            Label("PSI.Settings.Sensitivity.ApparelHealth".Translate() + (int)(PsiSettings.LimitApparelHealthLess * 100.0) + " %");
            PsiSettings.LimitApparelHealthLess = HorizontalSlider(PsiSettings.LimitApparelHealthLess, 0.01f, 0.99f);
            EndVertical();


            BeginVertical(_fondBoxes);
            Label("PSI.Settings.Sensitivity.Temperature".Translate() + (int)PsiSettings.LimitTempComfortOffset + " °C");
            PsiSettings.LimitTempComfortOffset = HorizontalSlider(PsiSettings.LimitTempComfortOffset, -10f, 10f);
            EndVertical();

            EndScrollView();

            //  if (!listing.DoTextButton("PSI.Settings.ReturnButton".Translate()))
            //      return;
            //
            //  Page = "main";
        }

        #endregion

    }
}
