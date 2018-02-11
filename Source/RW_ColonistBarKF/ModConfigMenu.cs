using System;
using System.Collections.Generic;
using System.IO;
using ColonistBarKF.Bar;
using ColonistBarKF.Settings;
using JetBrains.Annotations;
using UnityEngine;
using Verse;
using static ColonistBarKF.PSI.GameComponentPSI;

namespace ColonistBarKF
{
    public class ColonistBarKfSettings : Window
    {
        private static readonly string Cbkfversion = "Colonist Bar KF 0.18.0";

        private static int _iconLimit;

        [NotNull]
        private readonly GUIStyle _darkGrayBgImage =
            new GUIStyle { normal = { background = Textures.GrayFond } };

        [NotNull]
        private readonly GUIStyle _fondBoxes =
            new GUIStyle
            {
                normal = {
                                background = Textures.DarkGrayFond
                             },
                hover = {
                               background = Textures.GrayFond
                            },
                padding = new RectOffset(15, 15, 6, 10),
                margin = new RectOffset(0, 0, 10, 10)
            };

        [NotNull]
        private readonly GUIStyle _fondImages =
            new GUIStyle
            {
                normal = {
                                background = Textures.DarkGrayFond
                             },
                hover = {
                               background = Textures.RedHover
                            }
            };

        [NotNull]
        private readonly GUIStyle _fontBold =
            new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                normal = {
                                textColor = Color.white
                             },
                padding = new RectOffset(0, 0, 5, 0)
            };

        [NotNull]
        private readonly GUIStyle _grayLines = new GUIStyle { normal = { background = Textures.GrayLines } };

        [NotNull]
        private readonly GUIStyle _headline =
            new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                fontSize = 16,
                normal = {
                                textColor = Color.white
                             },
                padding = new RectOffset(0, 0, 12, 6)
            };

        [NotNull]
        private readonly string[] _mainToolbarStrings =
            { "CBKF.Settings.ColonistBar".Translate(), "CBKF.Settings.PSI".Translate() };

        [NotNull]
        private readonly string[] _positionStrings =
            {
                "CBKF.Settings.useLeft".Translate(), "CBKF.Settings.useRight".Translate(),
                "CBKF.Settings.useTop".Translate(), "CBKF.Settings.useBottom".Translate()
            };

        [NotNull]
        private readonly string[] _psiToolbarStrings =
            {
                "PSI.Settings.ArrangementButton".Translate(), "PSI.Settings.OpacityButton".Translate(),
                "PSI.Settings.IconButton".Translate(), "PSI.Settings.SensitivityButton".Translate()
            };

        private int _moodBarPositionInt;

        private int _psiBarPositionInt;

        private int _psiPositionInt;

        private Vector2 _scrollPositionBase;

        private Vector2 _scrollPositionPSI;

        private Vector2 _scrollPositionPSIOp;

        private Vector2 _scrollPositionPSISens;

        private Vector2 _scrollPositionPSISize;

        public ColonistBarKfSettings()
        {
            forcePause = false;
            doCloseX = true;
            draggable = true;
            drawShadow = true;
            preventCameraMotion = false;
            resizeable = true;
            onlyOneOfTypeAllowed = true;
            Reinit(false);
        }

        public override Vector2 InitialSize => new Vector2(540f, 650f);

        private int MainToolbarInt { get; set; }

        private int MoodBarPositionInt
        {
            get
            {
                switch (Settings.Settings.BarSettings.MoodBarPos)
                {
                    case Position.Alignment.Left:
                        _moodBarPositionInt = 0;
                        break;

                    case Position.Alignment.Right:
                        _moodBarPositionInt = 1;
                        break;

                    case Position.Alignment.Top:
                        _moodBarPositionInt = 2;
                        break;

                    case Position.Alignment.Bottom:
                        _moodBarPositionInt = 3;
                        break;
                }

                return _moodBarPositionInt;
            }

            set
            {
                switch (value)
                {
                    case 0:
                        Settings.Settings.BarSettings.MoodBarPos = Position.Alignment.Left;
                        break;

                    case 1:
                        Settings.Settings.BarSettings.MoodBarPos = Position.Alignment.Right;
                        break;

                    case 2:
                        Settings.Settings.BarSettings.MoodBarPos = Position.Alignment.Top;
                        break;

                    case 3:
                        Settings.Settings.BarSettings.MoodBarPos = Position.Alignment.Bottom;
                        break;

                    default:
                        Settings.Settings.BarSettings.MoodBarPos = Position.Alignment.Right;
                        break;
                }

                _moodBarPositionInt = value;
            }
        }

        private int PsiBarPositionInt
        {
            get
            {
                if (Settings.Settings.BarSettings.ColBarPsiIconPos == Position.Alignment.Left)
                {
                    _psiBarPositionInt = 0;
                }

                if (Settings.Settings.BarSettings.ColBarPsiIconPos == Position.Alignment.Right)
                {
                    _psiBarPositionInt = 1;
                }

                if (Settings.Settings.BarSettings.ColBarPsiIconPos == Position.Alignment.Top)
                {
                    _psiBarPositionInt = 2;
                }

                if (Settings.Settings.BarSettings.ColBarPsiIconPos == Position.Alignment.Bottom)
                {
                    _psiBarPositionInt = 3;
                }

                return _psiBarPositionInt;
            }

            set
            {
                switch (value)
                {
                    case 0:
                        Settings.Settings.BarSettings.ColBarPsiIconPos = Position.Alignment.Left;
                        Settings.Settings.BarSettings.IconOffsetX = 1f;
                        Settings.Settings.BarSettings.IconOffsetY = 1f;
                        Settings.Settings.BarSettings.IconsHorizontal = false;
                        break;

                    case 1:
                        Settings.Settings.BarSettings.ColBarPsiIconPos = Position.Alignment.Right;
                        Settings.Settings.BarSettings.IconOffsetX = -1f;
                        Settings.Settings.BarSettings.IconOffsetY = 1f;
                        Settings.Settings.BarSettings.IconsHorizontal = false;
                        break;

                    case 2:
                        Settings.Settings.BarSettings.ColBarPsiIconPos = Position.Alignment.Top;
                        Settings.Settings.BarSettings.IconOffsetX = -1f;
                        Settings.Settings.BarSettings.IconOffsetY = 1f;
                        Settings.Settings.BarSettings.IconsHorizontal = true;
                        break;

                    case 3:
                        Settings.Settings.BarSettings.ColBarPsiIconPos = Position.Alignment.Bottom;
                        Settings.Settings.BarSettings.IconOffsetX = -1;
                        Settings.Settings.BarSettings.IconOffsetY = -1;
                        Settings.Settings.BarSettings.IconsHorizontal = true;
                        break;

                    default:
                        Settings.Settings.BarSettings.ColBarPsiIconPos = 0;

                        break;
                }

                _psiBarPositionInt = value;
            }
        }

        private int PsiPositionInt
        {
            get
            {
                if (Settings.Settings.PSISettings.IconAlignment == 0)
                {
                    _psiPositionInt = 0;
                }

                if (Settings.Settings.PSISettings.IconAlignment == 1)
                {
                    _psiPositionInt = 1;
                }

                if (Settings.Settings.PSISettings.IconAlignment == 2)
                {
                    _psiPositionInt = 2;
                }

                if (Settings.Settings.PSISettings.IconAlignment == 3)
                {
                    _psiPositionInt = 3;
                }

                return _psiPositionInt;
            }

            set
            {
                if (value == _psiPositionInt)
                {
                    return;
                }

                switch (value)
                {
                    case 0:
                        Settings.Settings.PSISettings.IconAlignment = value;
                        Settings.Settings.PSISettings.IconMarginX = 1f;
                        Settings.Settings.PSISettings.IconMarginY = 1f;
                        Settings.Settings.PSISettings.IconOffsetX = 1f;
                        Settings.Settings.PSISettings.IconOffsetY = 1f;
                        Settings.Settings.PSISettings.IconsHorizontal = false;
                        Settings.Settings.PSISettings.IconsScreenScale = true;
                        Settings.Settings.PSISettings.IconsInColumn = 3;
                        Settings.Settings.PSISettings.IconSize = 1f;
                        Settings.Settings.PSISettings.IconOpacity = 0.5f;
                        Settings.Settings.PSISettings.IconOpacityCritical = 0.8f;
                        break;

                    case 1:
                        Settings.Settings.PSISettings.IconAlignment = value;
                        Settings.Settings.PSISettings.IconMarginX = -1f;
                        Settings.Settings.PSISettings.IconMarginY = 1f;
                        Settings.Settings.PSISettings.IconOffsetX = -1f;
                        Settings.Settings.PSISettings.IconOffsetY = 1f;
                        Settings.Settings.PSISettings.IconsHorizontal = false;
                        Settings.Settings.PSISettings.IconsScreenScale = true;
                        Settings.Settings.PSISettings.IconsInColumn = 3;
                        Settings.Settings.PSISettings.IconSize = 1f;
                        Settings.Settings.PSISettings.IconOpacity = 0.5f;
                        Settings.Settings.PSISettings.IconOpacityCritical = 0.8f;
                        break;

                    case 2:
                        Settings.Settings.PSISettings.IconAlignment = value;
                        Settings.Settings.PSISettings.IconMarginX = 1f;
                        Settings.Settings.PSISettings.IconMarginY = -1.63f;
                        Settings.Settings.PSISettings.IconOffsetX = -1f;
                        Settings.Settings.PSISettings.IconOffsetY = 1f;
                        Settings.Settings.PSISettings.IconsHorizontal = true;
                        Settings.Settings.PSISettings.IconsScreenScale = true;
                        Settings.Settings.PSISettings.IconsInColumn = 3;
                        Settings.Settings.PSISettings.IconSize = 1f;
                        Settings.Settings.PSISettings.IconOpacity = 0.5f;
                        Settings.Settings.PSISettings.IconOpacityCritical = 0.8f;
                        break;

                    case 3:
                        Settings.Settings.PSISettings.IconAlignment = value;
                        Settings.Settings.PSISettings.IconMarginX = 1.139534f;
                        Settings.Settings.PSISettings.IconMarginY = 1.375f;
                        Settings.Settings.PSISettings.IconOffsetX = -0.9534883f;
                        Settings.Settings.PSISettings.IconOffsetY = -0.9534884f;
                        Settings.Settings.PSISettings.IconsHorizontal = true;
                        Settings.Settings.PSISettings.IconsScreenScale = true;
                        Settings.Settings.PSISettings.IconsInColumn = 4;
                        Settings.Settings.PSISettings.IconSize = 1.084302f;
                        Settings.Settings.PSISettings.IconOpacity = 0.5f;
                        Settings.Settings.PSISettings.IconOpacityCritical = 0.8f;
                        break;

                    default:
                        Settings.Settings.PSISettings.IconAlignment = 0;

                        break;
                }

                _psiPositionInt = value;
            }
        }

        private int PSIToolbarInt { get; set; }

        public override void DoWindowContents(Rect rect)
        {
            Rect viewRect = new Rect(rect);
            viewRect.x += 15f;
            viewRect.width -= 30f;
            viewRect.height -= 15f;

            GUILayout.BeginArea(viewRect);
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(Cbkfversion, _headline);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(Text.LineHeight / 2);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            MainToolbarInt = GUILayout.Toolbar(MainToolbarInt, _mainToolbarStrings);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            switch (MainToolbarInt)
            {
                case 0:
                    {
                        GUILayout.Space(Text.LineHeight / 2);

                        _scrollPositionBase = GUILayout.BeginScrollView(_scrollPositionBase);

                        LabelHeadline("CBKF.Settings.BarPosition".Translate());
                        GUILayout.BeginVertical();
                        FillPageMain();
                        GUILayout.EndVertical();

                        LabelHeadline("CBKF.Settings.Advanced".Translate());
                        GUILayout.BeginVertical();
                        FillPageAdvanced();
                        GUILayout.EndVertical();

                        LabelHeadline("CBKF.Settings.Grouping".Translate());
                        GUILayout.BeginVertical();
                        FillPageCaravanSettings();
                        GUILayout.EndVertical();

                        GUILayout.EndScrollView();
                    }

                    break;

                case 1:
                    {
                        // LabelHeadline("PSI.Settings.Title".Translate());
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
                            _psiToolbarStrings,
                            toolbarInt > _psiToolbarStrings.Length ? _psiToolbarStrings.Length : toolbarInt);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        GUILayout.Space(Text.LineHeight / 2);

                        switch (PSIToolbarInt)
                        {
                            case 0:
                            {
                                FillPSIPageSizeArrangement();
                                break;
                            }
                            case 1:
                            {
                                FillPagePSIOpacityAndColor();
                                break;
                            }
                            case 2:
                            {
                                FillPagePSIIconSet(viewRect);
                                break;
                            }
                            case 3:
                            {
                                FillPSIPageSensitivity();
                                break;
                            }
                            default:
                                FillPagePSIIconSet(viewRect);
                                break;
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
            {
                HarmonyPatches.MarkColonistsDirty_Postfix();
                Reinit(false, false);
            }
        }

        public override void PreClose()
        {
            Settings.Settings.SaveBarSettings();
            Settings.Settings.SavePsiSettings();
        }

        private void DrawCheckboxArea(
            [NotNull] string iconName,
            [NotNull] Material iconMaterial,
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

            if (iconInRow > 0 && _iconLimit != 1)
            {
                GUILayout.Space(Text.LineHeight / 2);
            }

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
                GUILayout.Label(
                    iconMaterial.mainTexture,
                    GUILayout.Width(Text.LineHeight * 2.5f),
                    GUILayout.Height(Text.LineHeight * 2.5f));
            }
            else
            {
                if (psiBarBool || colBarBool)
                {
                    GUI.color = Color.red;
                    GUILayout.Label(
                        "PSI.Settings.IconSet.IconNotAvailable".Translate(),
                        GUILayout.Width(Text.LineHeight * 2.5f),
                        GUILayout.Height(Text.LineHeight * 2.5f));
                    GUI.color = Color.white;
                }
                else
                {
                    GUILayout.Label(
                        string.Empty,
                        GUILayout.Width(Text.LineHeight * 2.5f),
                        GUILayout.Height(Text.LineHeight * 2.5f));
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
            [NotNull] string iconName,
            [NotNull] Material targetSingle,
            [NotNull] Material targetHair,
            [NotNull] Material targetSkin,
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
            if (Settings.Settings.PSISettings.ShowTargetPoint)
            {
                if (!Settings.Settings.PSISettings.UseColoredTarget)
                {
                    if (targetSingle != null)
                    {
                        GUILayout.Label(
                            targetSingle.mainTexture,
                            GUILayout.Width(Text.LineHeight * 2.5f),
                            GUILayout.Height(Text.LineHeight * 2.5f));
                    }
                    else
                    {
                        GUI.color = Color.red;
                        GUILayout.Label(
                            "PSI.Settings.IconSet.IconNotAvailable".Translate(),
                            GUILayout.Width(Text.LineHeight * 2.5f),
                            GUILayout.Height(Text.LineHeight * 2.5f));
                        GUI.color = Color.white;
                    }
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    if (targetHair != null)
                    {
                        GUILayout.Label(
                            targetHair.mainTexture,
                            GUILayout.Width(Text.LineHeight * 2.5f),
                            GUILayout.Height(Text.LineHeight * 2.5f));
                    }
                    else
                    {
                        GUI.color = Color.red;
                        GUILayout.Label(
                            "PSI.Settings.IconSet.IconNotAvailable".Translate() + " HairTarget",
                            GUILayout.Width(Text.LineHeight * 2.5f),
                            GUILayout.Height(Text.LineHeight * 2.5f));
                        GUI.color = Color.white;
                    }

                    if (targetSkin != null)
                    {
                        GUILayout.Label(
                            targetSkin.mainTexture,
                            GUILayout.Width(Text.LineHeight * 2.5f),
                            GUILayout.Height(Text.LineHeight * 2.5f));
                    }
                    else
                    {
                        GUI.color = Color.red;
                        GUILayout.Label(
                            "PSI.Settings.IconSet.IconNotAvailable".Translate() + " SkinTarget",
                            GUILayout.Width(Text.LineHeight * 2.5f),
                            GUILayout.Height(Text.LineHeight * 2.5f));
                        GUI.color = Color.white;
                    }

                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.Label(
                    string.Empty,
                    GUILayout.Width(Text.LineHeight * 2.5f),
                    GUILayout.Height(Text.LineHeight * 2.5f));
            }

            GUILayout.Space(Text.LineHeight / 2);

            // Label("PSI.Settings.VisibilityButton".Translate(), FontBold);
            Settings.Settings.PSISettings.ShowTargetPoint = GUILayout.Toggle(
                Settings.Settings.PSISettings.ShowTargetPoint,
                "PSI.Settings.Visibility.TargetPoint".Translate());
            Settings.Settings.PSISettings.UseColoredTarget = GUILayout.Toggle(
                Settings.Settings.PSISettings.UseColoredTarget,
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

            Settings.Settings.BarSettings.UseCustomIconSize = GUILayout.Toggle(
                Settings.Settings.BarSettings.UseCustomIconSize,
                "CBKF.Settings.BasicSize".Translate() + Settings.Settings.BarSettings.BaseIconSize.ToString("N0") + " px, "
                + ColonistBar_Kf.BarHelperKf.CachedScale.ToStringPercent() + " %, "
                + (int)Settings.Settings.BarSettings.BaseSpacingHorizontal + " x, " + (int)Settings.Settings.BarSettings.BaseSpacingVertical + " y");

            if (Settings.Settings.BarSettings.UseCustomIconSize)
            {
                GUILayout.Space(Text.LineHeight / 2);

                Settings.Settings.BarSettings.BaseIconSize = GUILayout.HorizontalSlider(Settings.Settings.BarSettings.BaseIconSize, 24f, 256f);

                Settings.Settings.BarSettings.BaseSpacingHorizontal =
                    GUILayout.HorizontalSlider(Settings.Settings.BarSettings.BaseSpacingHorizontal, 1f, 72f);
                Settings.Settings.BarSettings.BaseSpacingVertical =
                    GUILayout.HorizontalSlider(Settings.Settings.BarSettings.BaseSpacingVertical, 1f, 96f);
            }
            else
            {
                Settings.Settings.BarSettings.BaseIconSize = 48f;
                Settings.Settings.BarSettings.BaseSpacingHorizontal = 24f;
                Settings.Settings.BarSettings.BaseSpacingVertical = 32f;
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical(_fondBoxes);
            Settings.Settings.BarSettings.UseCustomPawnTextureCameraOffsets = GUILayout.Toggle(
                Settings.Settings.BarSettings.UseCustomPawnTextureCameraOffsets,
                "CBKF.Settings.PawnTextureCameraOffsets".Translate()
                + Settings.Settings.BarSettings.PawnTextureCameraHorizontalOffset.ToString("N2") + " x, "
                + Settings.Settings.BarSettings.PawnTextureCameraVerticalOffset.ToString("N2") + " y, "
                + Settings.Settings.BarSettings.PawnTextureCameraZoom.ToString("N2") + " z");
            if (Settings.Settings.BarSettings.UseCustomPawnTextureCameraOffsets)
            {
                GUILayout.Space(Text.LineHeight / 2);
                Settings.Settings.BarSettings.PawnTextureCameraHorizontalOffset = GUILayout.HorizontalSlider(
                    Settings.Settings.BarSettings.PawnTextureCameraHorizontalOffset,
                    0.7f,
                    -0.7f);
                Settings.Settings.BarSettings.PawnTextureCameraVerticalOffset =
                    GUILayout.HorizontalSlider(Settings.Settings.BarSettings.PawnTextureCameraVerticalOffset, 0f, 1f);
                Settings.Settings.BarSettings.PawnTextureCameraZoom =
                    GUILayout.HorizontalSlider(Settings.Settings.BarSettings.PawnTextureCameraZoom, 0.3f, 3f);
            }
            else
            {
                Settings.Settings.BarSettings.PawnTextureCameraHorizontalOffset = 0f;
                Settings.Settings.BarSettings.PawnTextureCameraVerticalOffset = 0.3f;
                Settings.Settings.BarSettings.PawnTextureCameraZoom = 1.28205f;
            }

            GUILayout.EndVertical();

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
        }

        private void FillPageCaravanSettings()
        {
            GUILayout.BeginVertical(_fondBoxes);
            Settings.Settings.BarSettings.UseGrouping = GUILayout.Toggle(
                Settings.Settings.BarSettings.UseGrouping,
                "CBKF.Settings.UseGrouping".Translate());

            GUILayout.EndVertical();

            GUILayout.BeginVertical(_fondBoxes);
            Settings.Settings.BarSettings.UseGroupColors = GUILayout.Toggle(
                Settings.Settings.BarSettings.UseGroupColors,
                "CBKF.Settings.UseGroupColors".Translate());

            GUILayout.EndVertical();
        }

        private void FillPageMain()
        {
            GUILayout.BeginVertical(_fondBoxes);
            Settings.Settings.BarSettings.UseCustomMarginTop = GUILayout.Toggle(
                Settings.Settings.BarSettings.UseCustomMarginTop,
                "CBKF.Settings.ColonistBarOffset".Translate() + (int)Settings.Settings.BarSettings.MarginTop + " y \n"
                + "CBKF.Settings.MaxColonistBarWidth".Translate() + ": "
                + ((float)UI.screenWidth - (int)Settings.Settings.BarSettings.MarginHorizontal) + " px");

            if (Settings.Settings.BarSettings.UseCustomMarginTop)
            {
                GUILayout.Space(Text.LineHeight / 2);
                Settings.Settings.BarSettings.MarginTop =
                    GUILayout.HorizontalSlider(Settings.Settings.BarSettings.MarginTop, 0f, (float)UI.screenHeight / 6);
                Settings.Settings.BarSettings.MarginHorizontal = GUILayout.HorizontalSlider(
                    Settings.Settings.BarSettings.MarginHorizontal,
                    (float)UI.screenWidth * 3 / 5,
                    0f);
            }
            else
            {
                Settings.Settings.BarSettings.MarginTop = 21f;
                Settings.Settings.BarSettings.MarginHorizontal = 520f;
            }

            // listing.Gap(3f);
            GUILayout.EndVertical();

            // listing.Gap(3f);
            GUILayout.BeginVertical(_fondBoxes);
            Settings.Settings.BarSettings.UseCustomRowCount = GUILayout.Toggle(
                Settings.Settings.BarSettings.UseCustomRowCount,
                "PSI.Settings.Arrangement.ColonistsPerColumn".Translate()
                + (Settings.Settings.BarSettings.UseCustomRowCount ? Settings.Settings.BarSettings.MaxRowsCustom : 3));
            if (Settings.Settings.BarSettings.UseCustomRowCount)
            {
                Settings.Settings.BarSettings.MaxRowsCustom = (int)GUILayout.HorizontalSlider(Settings.Settings.BarSettings.MaxRowsCustom, 1f, 5f);
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical(_fondBoxes);

            Settings.Settings.BarSettings.UseWeaponIcons = GUILayout.Toggle(
                Settings.Settings.BarSettings.UseWeaponIcons,
                "CBKF.Settings.UseWeaponIcons".Translate());

            Settings.Settings.BarSettings.UseGender = GUILayout.Toggle(
                Settings.Settings.BarSettings.UseGender,
                "CBKF.Settings.useGender".Translate());

            Settings.Settings.BarSettings.useZoomToMouse = GUILayout.Toggle(
                Settings.Settings.BarSettings.useZoomToMouse,
                "CBKF.Settings.useZoomToMouse".Translate());

            GUILayout.Label("FollowMe.MiddleClick".Translate());

            Settings.Settings.BarSettings.useFollowMessage = GUILayout.Toggle(
                Settings.Settings.BarSettings.useFollowMessage,
                "CBKF.Settings.useFollowMessage".Translate());
            // #region DoubleClickTime
            // ColBarSettings.UseCustomDoubleClickTime = GUILayout.Toggle(
            // ColBarSettings.UseCustomDoubleClickTime,
            // "CBKF.Settings.DoubleClickTime".Translate() + ": " + ColBarSettings.DoubleClickTime.ToString("N2")
            // + " s");
            // if (ColBarSettings.UseCustomDoubleClickTime)
            // {
            // // listing.Gap(3f);
            // GUILayout.Space(Text.LineHeight / 2);
            // ColBarSettings.DoubleClickTime = GUILayout.HorizontalSlider(ColBarSettings.DoubleClickTime, 0.1f, 1.5f);
            // }
            // else
            // {
            // ColBarSettings.DoubleClickTime = 0.5f;
            // }
            // #endregion
            GUILayout.BeginVertical(_fondBoxes);
            Settings.Settings.BarSettings.UseNewMood = GUILayout.Toggle(
                Settings.Settings.BarSettings.UseNewMood,
                "CBKF.Settings.UseNewMood".Translate());

            if (Settings.Settings.BarSettings.UseNewMood)
            {
                Settings.Settings.BarSettings.UseExternalMoodBar = GUILayout.Toggle(
                    Settings.Settings.BarSettings.UseExternalMoodBar,
                    "CBKF.Settings.UseExternalMoodBar".Translate());

                if (Settings.Settings.BarSettings.UseExternalMoodBar)
                {
                    GUILayout.BeginHorizontal();
                    MoodBarPositionInt = GUILayout.Toolbar(MoodBarPositionInt, _positionStrings);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                Settings.Settings.BarSettings.UseExternalMoodBar = false;
            }

            GUILayout.EndVertical();

            GUILayout.EndVertical();
        }

        private void FillPagePSIIconSet(Rect viewRect)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("PSI.Settings.IconSet".Translate() + Settings.Settings.PSISettings.IconSet))
            {
                FloatMenuOption fmoDefault = new FloatMenuOption(
                    "PSI.Settings.Preset.0".Translate(),
                    () =>
                        {
                            try
                            {
                                Settings.Settings.PSISettings.IconSet = "default";
                                Settings.Settings.PSISettings.UseColoredTarget = true;
                                Settings.Settings.SavePsiSettings();
                                Reinit(false, true, false);
                            }
                            catch (IOException)
                            {
                                Log.Error("PSI.Settings.LoadPreset.UnableToLoad".Translate() + "default");
                            }
                        });

                FloatMenuOption fmoPreset1 = new FloatMenuOption(
                    "PSI.Settings.Preset.1".Translate(),
                    () =>
                        {
                            try
                            {
                                Settings.Settings.PSISettings.IconSet = "original";
                                Settings.Settings.PSISettings.UseColoredTarget = false;
                                Settings.Settings.SavePsiSettings();
                                Reinit(false, true, false);
                            }
                            catch (IOException)
                            {
                                Log.Error("PSI.Settings.LoadPreset.UnableToLoad".Translate() + "default");
                            }
                        });

                List<FloatMenuOption> options = new List<FloatMenuOption> { fmoDefault, fmoPreset1 };

                Find.WindowStack.Add(new FloatMenu(options));
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            _iconLimit = Mathf.FloorToInt(viewRect.width / 125f);

            GUILayout.Space(Text.LineHeight / 2);

            _scrollPositionPSI = GUILayout.BeginScrollView(_scrollPositionPSI);

            int num = 0;
            GUILayout.BeginHorizontal();

            DrawCheckboxAreaTarget(
                "PSI.Settings.Visibility.TargetPoint".Translate(),
                PSIMaterials[Icon.Target],
                PSIMaterials[Icon.TargetHair],
                PSIMaterials[Icon.TargetSkin],
                ref num);

            DrawCheckboxArea(
                "PSI.Settings.Visibility.Draft".Translate(),
                PSIMaterials[Icon.Draft],
                ref Settings.Settings.BarSettings.ShowDraft,
                ref Settings.Settings.PSISettings.ShowDraft,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Unarmed".Translate(),
                PSIMaterials[Icon.Unarmed],
                ref Settings.Settings.BarSettings.ShowUnarmed,
                ref Settings.Settings.PSISettings.ShowUnarmed,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Idle".Translate(),
                PSIMaterials[Icon.Idle],
                ref Settings.Settings.BarSettings.ShowIdle,
                ref Settings.Settings.PSISettings.ShowIdle,
                ref num);

            DrawCheckboxArea(
                "PSI.Settings.Visibility.Sad".Translate(),
                PSIMaterials[Icon.Sad],
                ref Settings.Settings.BarSettings.ShowSad,
                ref Settings.Settings.PSISettings.ShowSad,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Aggressive".Translate(),
                PSIMaterials[Icon.Aggressive],
                ref Settings.Settings.BarSettings.ShowAggressive,
                ref Settings.Settings.PSISettings.ShowAggressive,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Panic".Translate(),
                PSIMaterials[Icon.Panic],
                ref Settings.Settings.BarSettings.ShowPanic,
                ref Settings.Settings.PSISettings.ShowPanic,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Dazed".Translate(),
                PSIMaterials[Icon.Dazed],
                ref Settings.Settings.BarSettings.ShowDazed,
                ref Settings.Settings.PSISettings.ShowDazed,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Leave".Translate(),
                PSIMaterials[Icon.Leave],
                ref Settings.Settings.BarSettings.ShowLeave,
                ref Settings.Settings.PSISettings.ShowLeave,
                ref num);

            DrawCheckboxArea(
                "PSI.Settings.Visibility.Hungry".Translate(),
                PSIMaterials[Icon.Hungry],
                ref Settings.Settings.BarSettings.ShowHungry,
                ref Settings.Settings.PSISettings.ShowHungry,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Tired".Translate(),
                PSIMaterials[Icon.Tired],
                ref Settings.Settings.BarSettings.ShowTired,
                ref Settings.Settings.PSISettings.ShowTired,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.TooCold".Translate(),
                PSIMaterials[Icon.TooCold],
                ref Settings.Settings.BarSettings.ShowTooCold,
                ref Settings.Settings.PSISettings.ShowTooCold,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.TooHot".Translate(),
                PSIMaterials[Icon.TooHot],
                ref Settings.Settings.BarSettings.ShowTooHot,
                ref Settings.Settings.PSISettings.ShowTooHot,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.ApparelHealth".Translate(),
                PSIMaterials[Icon.ApparelHealth],
                ref Settings.Settings.BarSettings.ShowApparelHealth,
                ref Settings.Settings.PSISettings.ShowApparelHealth,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Naked".Translate(),
                PSIMaterials[Icon.Naked],
                ref Settings.Settings.BarSettings.ShowNaked,
                ref Settings.Settings.PSISettings.ShowNaked,
                ref num);

            DrawCheckboxArea(
                "PSI.Settings.Visibility.Health".Translate(),
                PSIMaterials[Icon.Health],
                ref Settings.Settings.BarSettings.ShowHealth,
                ref Settings.Settings.PSISettings.ShowHealth,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.MedicalAttention".Translate(),
                PSIMaterials[Icon.MedicalAttention],
                ref Settings.Settings.BarSettings.ShowMedicalAttention,
                ref Settings.Settings.PSISettings.ShowMedicalAttention,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Injury".Translate(),
                PSIMaterials[Icon.Effectiveness],
                ref Settings.Settings.BarSettings.ShowEffectiveness,
                ref Settings.Settings.PSISettings.ShowEffectiveness,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Bloodloss".Translate(),
                PSIMaterials[Icon.Bloodloss],
                ref Settings.Settings.BarSettings.ShowBloodloss,
                ref Settings.Settings.PSISettings.ShowBloodloss,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Pain".Translate(),
                PSIMaterials[Icon.Pain],
                ref Settings.Settings.BarSettings.ShowPain,
                ref Settings.Settings.PSISettings.ShowPain,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Drunk".Translate(),
                PSIMaterials[Icon.Drunk],
                ref Settings.Settings.BarSettings.ShowDrunk,
                ref Settings.Settings.PSISettings.ShowDrunk,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Toxicity".Translate(),
                PSIMaterials[Icon.Toxicity],
                ref Settings.Settings.BarSettings.ShowToxicity,
                ref Settings.Settings.PSISettings.ShowToxicity,
                ref num);

            DrawCheckboxArea(
                "PSI.Settings.Visibility.NightOwl".Translate(),
                PSIMaterials[Icon.NightOwl],
                ref Settings.Settings.BarSettings.ShowNightOwl,
                ref Settings.Settings.PSISettings.ShowNightOwl,
                ref num);

            DrawCheckboxArea(
                "PSI.Settings.Visibility.LeftUnburied".Translate(),
                PSIMaterials[Icon.LeftUnburied],
                ref Settings.Settings.BarSettings.ShowLeftUnburied,
                ref Settings.Settings.PSISettings.ShowLeftUnburied,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.CabinFever".Translate(),
                PSIMaterials[Icon.CabinFever],
                ref Settings.Settings.BarSettings.ShowCabinFever,
                ref Settings.Settings.PSISettings.ShowCabinFever,
                ref num);

            DrawCheckboxArea(
                "PSI.Settings.Visibility.Bedroom".Translate(),
                PSIMaterials[Icon.Bedroom],
                ref Settings.Settings.BarSettings.ShowBedroom,
                ref Settings.Settings.PSISettings.ShowBedroom,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Greedy".Translate(),
                PSIMaterials[Icon.Greedy],
                ref Settings.Settings.BarSettings.ShowGreedy,
                ref Settings.Settings.PSISettings.ShowGreedy,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Jealous".Translate(),
                PSIMaterials[Icon.Jealous],
                ref Settings.Settings.BarSettings.ShowJealous,
                ref Settings.Settings.PSISettings.ShowJealous,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Pyromaniac".Translate(),
                PSIMaterials[Icon.Pyromaniac],
                ref Settings.Settings.BarSettings.ShowPyromaniac,
                ref Settings.Settings.PSISettings.ShowPyromaniac,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Prosthophile".Translate(),
                PSIMaterials[Icon.Prosthophile],
                ref Settings.Settings.BarSettings.ShowProsthophile,
                ref Settings.Settings.PSISettings.ShowProsthophile,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Prosthophobe".Translate(),
                PSIMaterials[Icon.Prosthophobe],
                ref Settings.Settings.BarSettings.ShowProsthophobe,
                ref Settings.Settings.PSISettings.ShowProsthophobe,
                ref num);
            DrawCheckboxArea(
                "PSI.Settings.Visibility.Pacific".Translate(),
                PSIMaterials[Icon.Pacific],
                ref Settings.Settings.BarSettings.ShowPacific,
                ref Settings.Settings.PSISettings.ShowPacific,
                ref num);

            // DrawCheckboxArea("PSI.Settings.Visibility.Marriage".Translate(), PSIMaterials[Icons.Marriage], ref ColBarSettings.ShowMarriage, ref PsiSettings.ShowMarriage, ref num);
            GUILayout.EndHorizontal();

            GUILayout.Space(Text.LineHeight / 2);
            GUILayout.EndScrollView();
        }

        private void FillPagePSIOpacityAndColor()
        {
            _scrollPositionPSIOp = GUILayout.BeginScrollView(_scrollPositionPSIOp);

            GUILayout.BeginVertical(_fondBoxes);
            GUILayout.Label(
                "PSI.Settings.IconOpacityAndColor.Opacity".Translate() + (Settings.Settings.PSISettings.IconOpacity * 100).ToString("N0")
                + " %");
            Settings.Settings.PSISettings.IconOpacity = GUILayout.HorizontalSlider(Settings.Settings.PSISettings.IconOpacity, 0.1f, 1f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(_fondBoxes);
            GUILayout.Label(
                "PSI.Settings.IconOpacityAndColor.OpacityCritical".Translate()
                + (Settings.Settings.PSISettings.IconOpacityCritical * 100).ToString("N0") + " %");
            Settings.Settings.PSISettings.IconOpacityCritical = GUILayout.HorizontalSlider(Settings.Settings.PSISettings.IconOpacityCritical, 0.1f, 1f);
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
                                                                        Settings.Settings.PSISettings.LimitBleedMult
                                                                            = 2f;
                                                                        Settings.Settings.PSISettings
                                                                                .LimitDiseaseLess =
                                                                            1f;
                                                                        Settings.Settings.PSISettings
                                                                                .LimitEfficiencyLess =
                                                                            0.6f;
                                                                        Settings.Settings.PSISettings.LimitFoodLess
                                                                            = 0.2f;

                                                                        // PsiSettings.LimitMoodLess = 0.2f;
                                                                        Settings.Settings.PSISettings.LimitRestLess
                                                                            = 0.2f;
                                                                        Settings.Settings.PSISettings
                                                                                .LimitTempComfortOffset
                                                                            = 3f;
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
                                                                        Settings.Settings.PSISettings.LimitBleedMult
                                                                            = 3f;
                                                                        Settings.Settings.PSISettings
                                                                                .LimitDiseaseLess =
                                                                            1f;
                                                                        Settings.Settings.PSISettings
                                                                                .LimitEfficiencyLess =
                                                                            0.75f;
                                                                        Settings.Settings.PSISettings.LimitFoodLess
                                                                            = 0.25f;

                                                                        // PsiSettings.LimitMoodLess = 0.25f;
                                                                        Settings.Settings.PSISettings.LimitRestLess
                                                                            = 0.25f;
                                                                        Settings.Settings.PSISettings
                                                                                .LimitTempComfortOffset
                                                                            = 0f;
                                                                    }
                                                                    catch (IOException)
                                                                    {
                                                                        Log.Error(
                                                                            "PSI.Settings.LoadPreset.UnableToLoad"
                                                                                .Translate()
                                                                            + "Standard");
                                                                    }
                                                                }),
                                                        new FloatMenuOption(
                                                            "More Sensitive",
                                                            () =>
                                                                {
                                                                    try
                                                                    {
                                                                        Settings.Settings.PSISettings.LimitBleedMult
                                                                            = 4f;
                                                                        Settings.Settings.PSISettings
                                                                                .LimitDiseaseLess =
                                                                            1f;
                                                                        Settings.Settings.PSISettings
                                                                                .LimitEfficiencyLess =
                                                                            0.9f;
                                                                        Settings.Settings.PSISettings.LimitFoodLess
                                                                            = 0.3f;

                                                                        // PsiSettings.LimitMoodLess = 0.3f;
                                                                        Settings.Settings.PSISettings.LimitRestLess
                                                                            = 0.3f;
                                                                        Settings.Settings.PSISettings
                                                                                .LimitTempComfortOffset
                                                                            = -3f;
                                                                    }
                                                                    catch (IOException)
                                                                    {
                                                                        Log.Error(
                                                                            "PSI.Settings.LoadPreset.UnableToLoad"
                                                                                .Translate()
                                                                            + "More Sensitive");
                                                                    }
                                                                })
                                                    };

                Find.WindowStack.Add(new FloatMenu(options));
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            _scrollPositionPSISens = GUILayout.BeginScrollView(_scrollPositionPSISens);

            GUILayout.BeginVertical(_fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Bleeding".Translate()
                + ("PSI.Settings.Sensitivity.Bleeding." + Math.Round(Settings.Settings.PSISettings.LimitBleedMult - 0.25)).Translate());
            Settings.Settings.PSISettings.LimitBleedMult = GUILayout.HorizontalSlider(Settings.Settings.PSISettings.LimitBleedMult, 0.5f, 5f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(_fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Injured".Translate() + (int)(Settings.Settings.PSISettings.LimitEfficiencyLess * 100.0) + " %");
            Settings.Settings.PSISettings.LimitEfficiencyLess = GUILayout.HorizontalSlider(Settings.Settings.PSISettings.LimitEfficiencyLess, 0.01f, 1f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(_fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Food".Translate() + (int)(Settings.Settings.PSISettings.LimitFoodLess * 100.0) + " %");
            Settings.Settings.PSISettings.LimitFoodLess = GUILayout.HorizontalSlider(Settings.Settings.PSISettings.LimitFoodLess, 0.01f, 0.99f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(_fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Rest".Translate() + (int)(Settings.Settings.PSISettings.LimitRestLess * 100.0) + " %");
            Settings.Settings.PSISettings.LimitRestLess = GUILayout.HorizontalSlider(Settings.Settings.PSISettings.LimitRestLess, 0.01f, 0.99f);
            GUILayout.EndVertical();

            // Replaced with thought check => human leather, dead man's apparel
            // GUILayout.BeginVertical(this._fondBoxes);
            // GUILayout.Label(
            // "PSI.Settings.Sensitivity.ApparelHealth".Translate() + (int)(PsiSettings.LimitApparelHealthLess * 100.0)
            // + " %");
            // PsiSettings.LimitApparelHealthLess =
            // GUILayout.HorizontalSlider(PsiSettings.LimitApparelHealthLess, 0.01f, 0.99f);
            // GUILayout.EndVertical();
            GUILayout.BeginVertical(_fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Temperature".Translate() + (int)Settings.Settings.PSISettings.LimitTempComfortOffset + " °C");
            Settings.Settings.PSISettings.LimitTempComfortOffset =
                GUILayout.HorizontalSlider(Settings.Settings.PSISettings.LimitTempComfortOffset, -10f, 10f);
            GUILayout.EndVertical();

            GUILayout.EndScrollView();

            // if (!listing.DoTextButton("PSI.Settings.ReturnButton".Translate()))
            // return;
            // Page = "main";
        }

        private void FillPSIPageSizeArrangement()
        {
            _scrollPositionPSISize = GUILayout.BeginScrollView(_scrollPositionPSISize);

            GUILayout.BeginVertical(_fondBoxes);
            Settings.Settings.BarSettings.UsePsi = GUILayout.Toggle(Settings.Settings.BarSettings.UsePsi, "CBKF.Settings.UsePsiOnBar".Translate());
            if (Settings.Settings.BarSettings.UsePsi)
            {
                GUILayout.BeginHorizontal();
                PsiBarPositionInt = GUILayout.Toolbar(PsiBarPositionInt, _positionStrings);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.Label("PSI.Settings.Arrangement.IconsPerColumn".Translate() + Settings.Settings.BarSettings.IconsInColumn);
                Settings.Settings.BarSettings.IconsInColumn = (int)GUILayout.HorizontalSlider(Settings.Settings.BarSettings.IconsInColumn, 2f, 5f);
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical(_fondBoxes);
            Settings.Settings.PSISettings.UsePsi = GUILayout.Toggle(Settings.Settings.PSISettings.UsePsi, "PSI.Settings.UsePSI".Translate());
            Settings.Settings.PSISettings.ShowRelationsOnStrangers = GUILayout.Toggle(
                Settings.Settings.PSISettings.ShowRelationsOnStrangers,
                "PSI.Settings.ShowRelationsOnStrangers".Translate());
            Settings.Settings.PSISettings.UsePsiOnPrisoner = GUILayout.Toggle(
                Settings.Settings.PSISettings.UsePsiOnPrisoner,
                "PSI.Settings.UsePSIOnPrisoner".Translate());
            Settings.Settings.PSISettings.UsePsiOnAnimals = GUILayout.Toggle(
                Settings.Settings.PSISettings.UsePsiOnAnimals,
                "PSI.Settings.UsePsiOnAnimals".Translate());

            if (Settings.Settings.PSISettings.UsePsi || Settings.Settings.PSISettings.UsePsiOnPrisoner)
            {
                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.BeginHorizontal();
                PsiPositionInt = GUILayout.Toolbar(PsiPositionInt, _positionStrings);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(Text.LineHeight / 2);

                Settings.Settings.PSISettings.IconsHorizontal = GUILayout.Toggle(
                    Settings.Settings.PSISettings.IconsHorizontal,
                    "PSI.Settings.Arrangement.Horizontal".Translate());

                Settings.Settings.PSISettings.IconsScreenScale = GUILayout.Toggle(
                    Settings.Settings.PSISettings.IconsScreenScale,
                    "PSI.Settings.Arrangement.ScreenScale".Translate());

                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.Label("PSI.Settings.Arrangement.IconsPerColumn".Translate() + Settings.Settings.PSISettings.IconsInColumn);
                Settings.Settings.PSISettings.IconsInColumn = (int)GUILayout.HorizontalSlider(Settings.Settings.PSISettings.IconsInColumn, 1f, 7f);

                int num = (int)(Settings.Settings.PSISettings.IconSize * 4.5);

                if (num > 8)
                {
                    num = 8;
                }
                else if (num < 0)
                {
                    num = 0;
                }

                GUILayout.Space(Text.LineHeight / 2);
                GUILayout.Label(
                    "PSI.Settings.Arrangement.IconSize".Translate() + ("PSI.Settings.SizeLabel." + num).Translate());
                Settings.Settings.PSISettings.IconSize = GUILayout.HorizontalSlider(Settings.Settings.PSISettings.IconSize, 0.5f, 2f);
                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.BeginVertical(_fondBoxes);
                GUILayout.Label(
                    "PSI.Settings.Arrangement.IconPosition".Translate() + (int)(Settings.Settings.PSISettings.IconMarginX * 100.0)
                    + " x, " + (int)(Settings.Settings.PSISettings.IconMarginY * 100.0) + " y");
                Settings.Settings.PSISettings.IconMarginX = GUILayout.HorizontalSlider(Settings.Settings.PSISettings.IconMarginX, -2f, 2f);
                Settings.Settings.PSISettings.IconMarginY = GUILayout.HorizontalSlider(Settings.Settings.PSISettings.IconMarginY, -2f, 2f);
                GUILayout.EndVertical();

                GUILayout.BeginVertical(_fondBoxes);
                GUILayout.Label(
                    "PSI.Settings.Arrangement.IconOffset".Translate() + (int)(Settings.Settings.PSISettings.IconOffsetX * 100.0) + " x, "
                    + (int)(Settings.Settings.PSISettings.IconOffsetY * 100.0) + " y");
                Settings.Settings.PSISettings.IconOffsetX = GUILayout.HorizontalSlider(Settings.Settings.PSISettings.IconOffsetX, -2f, 2f);
                Settings.Settings.PSISettings.IconOffsetY = GUILayout.HorizontalSlider(Settings.Settings.PSISettings.IconOffsetY, -2f, 2f);
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
            Settings.Settings.BarSettings = new SettingsColonistBar();
        }

        private void ResetPSISettings()
        {
            Settings.Settings.PSISettings = new SettingsPSI();
        }
    }
}