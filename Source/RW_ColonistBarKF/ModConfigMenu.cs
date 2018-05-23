using System;
using System.Collections.Generic;
using System.IO;
using ColonistBarKF.Bar;
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
            this.forcePause = false;
            this.doCloseX = true;
            this.draggable = true;
            this.drawShadow = true;
            this.preventCameraMotion = false;
            this.resizeable = true;
            this.onlyOneOfTypeAllowed = true;
            Reinit(false);
        }

        public override Vector2 InitialSize => new Vector2(540f, 650f);

        private int MainToolbarInt { get; set; }

        private int MoodBarPositionInt
        {
            get
            {
                switch (Settings.BarSettings.MoodBarPos)
                {
                    case Position.Alignment.Left:
                        this._moodBarPositionInt = 0;
                        break;

                    case Position.Alignment.Right:
                        this._moodBarPositionInt = 1;
                        break;

                    case Position.Alignment.Top:
                        this._moodBarPositionInt = 2;
                        break;

                    case Position.Alignment.Bottom:
                        this._moodBarPositionInt = 3;
                        break;
                }

                return this._moodBarPositionInt;
            }

            set
            {
                switch (value)
                {
                    case 0:
                        Settings.BarSettings.MoodBarPos = Position.Alignment.Left;
                        break;

                    case 1:
                        Settings.BarSettings.MoodBarPos = Position.Alignment.Right;
                        break;

                    case 2:
                        Settings.BarSettings.MoodBarPos = Position.Alignment.Top;
                        break;

                    case 3:
                        Settings.BarSettings.MoodBarPos = Position.Alignment.Bottom;
                        break;

                    default:
                        Settings.BarSettings.MoodBarPos = Position.Alignment.Right;
                        break;
                }

                this._moodBarPositionInt = value;
            }
        }

        private int PSIBarPositionInt
        {
            get
            {
                if (Settings.BarSettings.ColBarPSIIconPos == Position.Alignment.Left)
                {
                    this._psiBarPositionInt = 0;
                }

                if (Settings.BarSettings.ColBarPSIIconPos == Position.Alignment.Right)
                {
                    this._psiBarPositionInt = 1;
                }

                if (Settings.BarSettings.ColBarPSIIconPos == Position.Alignment.Top)
                {
                    this._psiBarPositionInt = 2;
                }

                if (Settings.BarSettings.ColBarPSIIconPos == Position.Alignment.Bottom)
                {
                    this._psiBarPositionInt = 3;
                }

                return this._psiBarPositionInt;
            }

            set
            {
                switch (value)
                {
                    case 0:
                        Settings.BarSettings.ColBarPSIIconPos = Position.Alignment.Left;
                        Settings.BarSettings.IconOffsetX = 1f;
                        Settings.BarSettings.IconOffsetY = 1f;
                        Settings.BarSettings.IconsHorizontal = false;
                        break;

                    case 1:
                        Settings.BarSettings.ColBarPSIIconPos = Position.Alignment.Right;
                        Settings.BarSettings.IconOffsetX = -1f;
                        Settings.BarSettings.IconOffsetY = 1f;
                        Settings.BarSettings.IconsHorizontal = false;
                        break;

                    case 2:
                        Settings.BarSettings.ColBarPSIIconPos = Position.Alignment.Top;
                        Settings.BarSettings.IconOffsetX = -1f;
                        Settings.BarSettings.IconOffsetY = 1f;
                        Settings.BarSettings.IconsHorizontal = true;
                        break;

                    case 3:
                        Settings.BarSettings.ColBarPSIIconPos = Position.Alignment.Bottom;
                        Settings.BarSettings.IconOffsetX = -1;
                        Settings.BarSettings.IconOffsetY = -1;
                        Settings.BarSettings.IconsHorizontal = true;
                        break;

                    default:
                        Settings.BarSettings.ColBarPSIIconPos = 0;

                        break;
                }

                this._psiBarPositionInt = value;
            }
        }

        private int PSIPositionInt
        {
            get
            {
                if (Settings.PSISettings.IconAlignment == 0)
                {
                    this._psiPositionInt = 0;
                }

                if (Settings.PSISettings.IconAlignment == 1)
                {
                    this._psiPositionInt = 1;
                }

                if (Settings.PSISettings.IconAlignment == 2)
                {
                    this._psiPositionInt = 2;
                }

                if (Settings.PSISettings.IconAlignment == 3)
                {
                    this._psiPositionInt = 3;
                }

                return this._psiPositionInt;
            }

            set
            {
                if (value == this._psiPositionInt)
                {
                    return;
                }

                switch (value)
                {
                    case 0:
                        Settings.PSISettings.IconAlignment = value;
                        Settings.PSISettings.IconMarginX = 1f;
                        Settings.PSISettings.IconMarginY = 1f;
                        Settings.PSISettings.IconOffsetX = 1f;
                        Settings.PSISettings.IconOffsetY = 1f;
                        Settings.PSISettings.IconsHorizontal = false;
                        Settings.PSISettings.IconsScreenScale = true;
                        Settings.PSISettings.IconsInColumn = 3;
                        Settings.PSISettings.IconSize = 1f;
                        Settings.PSISettings.IconOpacity = 0.5f;
                        Settings.PSISettings.IconOpacityCritical = 0.8f;
                        break;

                    case 1:
                        Settings.PSISettings.IconAlignment = value;
                        Settings.PSISettings.IconMarginX = -1f;
                        Settings.PSISettings.IconMarginY = 1f;
                        Settings.PSISettings.IconOffsetX = -1f;
                        Settings.PSISettings.IconOffsetY = 1f;
                        Settings.PSISettings.IconsHorizontal = false;
                        Settings.PSISettings.IconsScreenScale = true;
                        Settings.PSISettings.IconsInColumn = 3;
                        Settings.PSISettings.IconSize = 1f;
                        Settings.PSISettings.IconOpacity = 0.5f;
                        Settings.PSISettings.IconOpacityCritical = 0.8f;
                        break;

                    case 2:
                        Settings.PSISettings.IconAlignment = value;
                        Settings.PSISettings.IconMarginX = 1f;
                        Settings.PSISettings.IconMarginY = -1.63f;
                        Settings.PSISettings.IconOffsetX = -1f;
                        Settings.PSISettings.IconOffsetY = 1f;
                        Settings.PSISettings.IconsHorizontal = true;
                        Settings.PSISettings.IconsScreenScale = true;
                        Settings.PSISettings.IconsInColumn = 3;
                        Settings.PSISettings.IconSize = 1f;
                        Settings.PSISettings.IconOpacity = 0.5f;
                        Settings.PSISettings.IconOpacityCritical = 0.8f;
                        break;

                    case 3:
                        Settings.PSISettings.IconAlignment = value;
                        Settings.PSISettings.IconMarginX = 1.139534f;
                        Settings.PSISettings.IconMarginY = 1.375f;
                        Settings.PSISettings.IconOffsetX = -0.9534883f;
                        Settings.PSISettings.IconOffsetY = -0.9534884f;
                        Settings.PSISettings.IconsHorizontal = true;
                        Settings.PSISettings.IconsScreenScale = true;
                        Settings.PSISettings.IconsInColumn = 4;
                        Settings.PSISettings.IconSize = 1.084302f;
                        Settings.PSISettings.IconOpacity = 0.5f;
                        Settings.PSISettings.IconOpacityCritical = 0.8f;
                        break;

                    default:
                        Settings.PSISettings.IconAlignment = 0;

                        break;
                }

                this._psiPositionInt = value;
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
            GUILayout.Label(Cbkfversion, this._headline);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(Text.LineHeight / 2);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            this.MainToolbarInt = GUILayout.Toolbar(this.MainToolbarInt, this._mainToolbarStrings);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            switch (this.MainToolbarInt)
            {
                case 0:
                    {
                        GUILayout.Space(Text.LineHeight / 2);

                        this._scrollPositionBase = GUILayout.BeginScrollView(this._scrollPositionBase);

                        this.LabelHeadline("CBKF.Settings.BarPosition".Translate());
                        GUILayout.BeginVertical();
                        this.FillPageMain();
                        GUILayout.EndVertical();

                        this.LabelHeadline("CBKF.Settings.Advanced".Translate());
                        GUILayout.BeginVertical();
                        this.FillPageAdvanced();
                        GUILayout.EndVertical();

                        this.LabelHeadline("CBKF.Settings.Grouping".Translate());
                        GUILayout.BeginVertical();
                        this.FillPageCaravanSettings();
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
                        this.PSIToolbarInt = GUILayout.SelectionGrid(this.PSIToolbarInt, this._psiToolbarStrings,
                            toolbarInt > this._psiToolbarStrings.Length ? this._psiToolbarStrings.Length : toolbarInt);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        GUILayout.Space(Text.LineHeight / 2);

                        switch (this.PSIToolbarInt)
                        {
                            case 0:
                            {
                                this.FillPSIPageSizeArrangement();
                                break;
                            }
                            case 1:
                            {
                                this.FillPagePSIOpacityAndColor();
                                break;
                            }
                            case 2:
                            {
                                this.FillPagePSIIconSet(viewRect);
                                break;
                            }
                            case 3:
                            {
                                this.FillPSIPageSensitivity();
                                break;
                            }
                            default:
                                this.FillPagePSIIconSet(viewRect);
                                break;
                        }
                    }

                    break;
            }

            GUILayout.FlexibleSpace();
            GUILayout.Space(Text.LineHeight / 2);
            GUILayout.Label(string.Empty, this._grayLines, GUILayout.Height(1));
            GUILayout.Space(Text.LineHeight / 2);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("CBKF.Settings.RevertSettings".Translate()))
            {
                this.ResetBarSettings();
            }

            GUILayout.Space(Text.LineHeight / 2);
            if (GUILayout.Button("PSI.Settings.RevertSettings".Translate()))
            {
                this.ResetPSISettings();
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
            Settings.SaveBarSettings();
            Settings.SavePSISettings();
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

            GUILayout.BeginVertical(this._fondImages);

            GUILayout.BeginHorizontal(this._darkGrayBgImage, GUILayout.Height(Text.LineHeight * 1.2f));
            GUILayout.FlexibleSpace();
            GUILayout.Label(iconName, this._fontBold);
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
            GUILayout.BeginVertical(this._fondImages);

            GUILayout.BeginHorizontal(this._darkGrayBgImage, GUILayout.Height(Text.LineHeight * 1.2f));
            GUILayout.FlexibleSpace();
            GUILayout.Label(iconName, this._fontBold);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical();
            GUILayout.Space(Text.LineHeight / 2);
            if (Settings.PSISettings.ShowTargetPoint)
            {
                if (!Settings.PSISettings.UseColoredTarget)
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
            Settings.PSISettings.ShowTargetPoint = GUILayout.Toggle(
                Settings.PSISettings.ShowTargetPoint,
                "PSI.Settings.Visibility.TargetPoint".Translate());
            Settings.PSISettings.UseColoredTarget = GUILayout.Toggle(
                Settings.PSISettings.UseColoredTarget,
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
            GUILayout.BeginVertical(this._fondBoxes);

            Settings.BarSettings.UseCustomIconSize = GUILayout.Toggle(
                Settings.BarSettings.UseCustomIconSize,
                "CBKF.Settings.BasicSize".Translate() + Settings.BarSettings.BaseIconSize.ToString("N0") + " px, "
                + ColonistBar_KF.BarHelperKF.CachedScale.ToStringPercent() + " %, "
                + (int)Settings.BarSettings.BaseSpacingHorizontal + " x, " + (int)Settings.BarSettings.BaseSpacingVertical + " y");

            if (Settings.BarSettings.UseCustomIconSize)
            {
                GUILayout.Space(Text.LineHeight / 2);

                Settings.BarSettings.BaseIconSize = GUILayout.HorizontalSlider(Settings.BarSettings.BaseIconSize, 24f, 256f);

                Settings.BarSettings.BaseSpacingHorizontal =
                    GUILayout.HorizontalSlider(Settings.BarSettings.BaseSpacingHorizontal, 1f, 72f);
                Settings.BarSettings.BaseSpacingVertical =
                    GUILayout.HorizontalSlider(Settings.BarSettings.BaseSpacingVertical, 1f, 96f);
            }
            else
            {
                Settings.BarSettings.BaseIconSize = 48f;
                Settings.BarSettings.BaseSpacingHorizontal = 24f;
                Settings.BarSettings.BaseSpacingVertical = 32f;
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical(this._fondBoxes);
            Settings.BarSettings.UseCustomPawnTextureCameraOffsets = GUILayout.Toggle(
                Settings.BarSettings.UseCustomPawnTextureCameraOffsets,
                "CBKF.Settings.PawnTextureCameraOffsets".Translate()
                + Settings.BarSettings.PawnTextureCameraHorizontalOffset.ToString("N2") + " x, "
                + Settings.BarSettings.PawnTextureCameraVerticalOffset.ToString("N2") + " y, "
                + Settings.BarSettings.PawnTextureCameraZoom.ToString("N2") + " z");
            if (Settings.BarSettings.UseCustomPawnTextureCameraOffsets)
            {
                GUILayout.Space(Text.LineHeight / 2);
                Settings.BarSettings.PawnTextureCameraHorizontalOffset = GUILayout.HorizontalSlider(
                    Settings.BarSettings.PawnTextureCameraHorizontalOffset,
                    0.7f,
                    -0.7f);
                Settings.BarSettings.PawnTextureCameraVerticalOffset =
                    GUILayout.HorizontalSlider(Settings.BarSettings.PawnTextureCameraVerticalOffset, 0f, 1f);
                Settings.BarSettings.PawnTextureCameraZoom =
                    GUILayout.HorizontalSlider(Settings.BarSettings.PawnTextureCameraZoom, 0.3f, 3f);
            }
            else
            {
                Settings.BarSettings.PawnTextureCameraHorizontalOffset = 0f;
                Settings.BarSettings.PawnTextureCameraVerticalOffset = 0.3f;
                Settings.BarSettings.PawnTextureCameraZoom = 1.28205f;
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
            GUILayout.BeginVertical(this._fondBoxes);
            Settings.BarSettings.UseGrouping = GUILayout.Toggle(
                Settings.BarSettings.UseGrouping,
                "CBKF.Settings.UseGrouping".Translate());

            GUILayout.EndVertical();

            GUILayout.BeginVertical(this._fondBoxes);
            Settings.BarSettings.UseGroupColors = GUILayout.Toggle(
                Settings.BarSettings.UseGroupColors,
                "CBKF.Settings.UseGroupColors".Translate());

            GUILayout.EndVertical();
        }

        private void FillPageMain()
        {
            GUILayout.BeginVertical(this._fondBoxes);
            Settings.BarSettings.UseCustomMarginTop = GUILayout.Toggle(
                Settings.BarSettings.UseCustomMarginTop,
                "CBKF.Settings.ColonistBarOffset".Translate() + (int)Settings.BarSettings.MarginTop + " y \n"
                + "CBKF.Settings.MaxColonistBarWidth".Translate() + ": "
                + ((float)UI.screenWidth - (int)Settings.BarSettings.MarginHorizontal) + " px");

            if (Settings.BarSettings.UseCustomMarginTop)
            {
                GUILayout.Space(Text.LineHeight / 2);
                Settings.BarSettings.MarginTop =
                    GUILayout.HorizontalSlider(Settings.BarSettings.MarginTop, 0f, (float)UI.screenHeight / 6);
                Settings.BarSettings.MarginHorizontal = GUILayout.HorizontalSlider(
                    Settings.BarSettings.MarginHorizontal,
                    (float)UI.screenWidth * 3 / 5,
                    0f);
            }
            else
            {
                Settings.BarSettings.MarginTop = 21f;
                Settings.BarSettings.MarginHorizontal = 520f;
            }

            // listing.Gap(3f);
            GUILayout.EndVertical();

            // listing.Gap(3f);
            GUILayout.BeginVertical(this._fondBoxes);
            Settings.BarSettings.UseCustomRowCount = GUILayout.Toggle(
                Settings.BarSettings.UseCustomRowCount,
                "PSI.Settings.Arrangement.ColonistsPerColumn".Translate()
                + (Settings.BarSettings.UseCustomRowCount ? Settings.BarSettings.MaxRowsCustom : 3));
            if (Settings.BarSettings.UseCustomRowCount)
            {
                Settings.BarSettings.MaxRowsCustom = (int)GUILayout.HorizontalSlider(Settings.BarSettings.MaxRowsCustom, 1f, 11f);
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical(this._fondBoxes);

            Settings.BarSettings.UseWeaponIcons = GUILayout.Toggle(
                Settings.BarSettings.UseWeaponIcons,
                "CBKF.Settings.UseWeaponIcons".Translate());

            Settings.BarSettings.UseGender = GUILayout.Toggle(
                Settings.BarSettings.UseGender,
                "CBKF.Settings.useGender".Translate());

            Settings.BarSettings.useZoomToMouse = GUILayout.Toggle(
                Settings.BarSettings.useZoomToMouse,
                "CBKF.Settings.useZoomToMouse".Translate());

            GUILayout.Label("FollowMe.MiddleClick".Translate());

            Settings.BarSettings.useFollowMessage = GUILayout.Toggle(
                Settings.BarSettings.useFollowMessage,
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
            GUILayout.BeginVertical(this._fondBoxes);
            Settings.BarSettings.UseNewMood = GUILayout.Toggle(
                Settings.BarSettings.UseNewMood,
                "CBKF.Settings.UseNewMood".Translate());

            if (Settings.BarSettings.UseNewMood)
            {
                Settings.BarSettings.UseExternalMoodBar = GUILayout.Toggle(
                    Settings.BarSettings.UseExternalMoodBar,
                    "CBKF.Settings.UseExternalMoodBar".Translate());

                if (Settings.BarSettings.UseExternalMoodBar)
                {
                    GUILayout.BeginHorizontal();
                    this.MoodBarPositionInt = GUILayout.Toolbar(this.MoodBarPositionInt, this._positionStrings);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                Settings.BarSettings.UseExternalMoodBar = false;
            }

            GUILayout.EndVertical();

            GUILayout.EndVertical();
        }

        private void FillPagePSIIconSet(Rect viewRect)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("PSI.Settings.IconSet".Translate() + Settings.PSISettings.IconSet))
            {
                FloatMenuOption fmoDefault = new FloatMenuOption(
                    "PSI.Settings.Preset.0".Translate(),
                    () =>
                        {
                            try
                            {
                                Settings.PSISettings.IconSet = "default";
                                Settings.PSISettings.UseColoredTarget = true;
                                Settings.SavePSISettings();
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
                                Settings.PSISettings.IconSet = "original";
                                Settings.PSISettings.UseColoredTarget = false;
                                Settings.SavePSISettings();
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

            this._scrollPositionPSI = GUILayout.BeginScrollView(this._scrollPositionPSI);

            int num = 0;
            GUILayout.BeginHorizontal();

            this.DrawCheckboxAreaTarget(
                "PSI.Settings.Visibility.TargetPoint".Translate(),
                PSIMaterials[Icon.Target],
                PSIMaterials[Icon.TargetHair],
                PSIMaterials[Icon.TargetSkin],
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Draft".Translate(),
                PSIMaterials[Icon.Draft],
                ref Settings.BarSettings.ShowDraft,
                ref Settings.PSISettings.ShowDraft,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Unarmed".Translate(),
                PSIMaterials[Icon.Unarmed],
                ref Settings.BarSettings.ShowUnarmed,
                ref Settings.PSISettings.ShowUnarmed,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Idle".Translate(),
                PSIMaterials[Icon.Idle],
                ref Settings.BarSettings.ShowIdle,
                ref Settings.PSISettings.ShowIdle,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Sad".Translate(),
                PSIMaterials[Icon.Sad],
                ref Settings.BarSettings.ShowSad,
                ref Settings.PSISettings.ShowSad,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Aggressive".Translate(),
                PSIMaterials[Icon.Aggressive],
                ref Settings.BarSettings.ShowAggressive,
                ref Settings.PSISettings.ShowAggressive,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Panic".Translate(),
                PSIMaterials[Icon.Panic],
                ref Settings.BarSettings.ShowPanic,
                ref Settings.PSISettings.ShowPanic,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Dazed".Translate(),
                PSIMaterials[Icon.Dazed],
                ref Settings.BarSettings.ShowDazed,
                ref Settings.PSISettings.ShowDazed,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Leave".Translate(),
                PSIMaterials[Icon.Leave],
                ref Settings.BarSettings.ShowLeave,
                ref Settings.PSISettings.ShowLeave,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Hungry".Translate(),
                PSIMaterials[Icon.Hungry],
                ref Settings.BarSettings.ShowHungry,
                ref Settings.PSISettings.ShowHungry,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Tired".Translate(),
                PSIMaterials[Icon.Tired],
                ref Settings.BarSettings.ShowTired,
                ref Settings.PSISettings.ShowTired,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.TooCold".Translate(),
                PSIMaterials[Icon.TooCold],
                ref Settings.BarSettings.ShowTooCold,
                ref Settings.PSISettings.ShowTooCold,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.TooHot".Translate(),
                PSIMaterials[Icon.TooHot],
                ref Settings.BarSettings.ShowTooHot,
                ref Settings.PSISettings.ShowTooHot,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.ApparelHealth".Translate(),
                PSIMaterials[Icon.ApparelHealth],
                ref Settings.BarSettings.ShowApparelHealth,
                ref Settings.PSISettings.ShowApparelHealth,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Naked".Translate(),
                PSIMaterials[Icon.Naked],
                ref Settings.BarSettings.ShowNaked,
                ref Settings.PSISettings.ShowNaked,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Health".Translate(),
                PSIMaterials[Icon.Health],
                ref Settings.BarSettings.ShowHealth,
                ref Settings.PSISettings.ShowHealth,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.MedicalAttention".Translate(),
                PSIMaterials[Icon.MedicalAttention],
                ref Settings.BarSettings.ShowMedicalAttention,
                ref Settings.PSISettings.ShowMedicalAttention,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Injury".Translate(),
                PSIMaterials[Icon.Effectiveness],
                ref Settings.BarSettings.ShowEffectiveness,
                ref Settings.PSISettings.ShowEffectiveness,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Bloodloss".Translate(),
                PSIMaterials[Icon.Bloodloss],
                ref Settings.BarSettings.ShowBloodloss,
                ref Settings.PSISettings.ShowBloodloss,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Pain".Translate(),
                PSIMaterials[Icon.Pain],
                ref Settings.BarSettings.ShowPain,
                ref Settings.PSISettings.ShowPain,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Drunk".Translate(),
                PSIMaterials[Icon.Drunk],
                ref Settings.BarSettings.ShowDrunk,
                ref Settings.PSISettings.ShowDrunk,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Toxicity".Translate(),
                PSIMaterials[Icon.Toxicity],
                ref Settings.BarSettings.ShowToxicity,
                ref Settings.PSISettings.ShowToxicity,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.NightOwl".Translate(),
                PSIMaterials[Icon.NightOwl],
                ref Settings.BarSettings.ShowNightOwl,
                ref Settings.PSISettings.ShowNightOwl,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.LeftUnburied".Translate(),
                PSIMaterials[Icon.LeftUnburied],
                ref Settings.BarSettings.ShowLeftUnburied,
                ref Settings.PSISettings.ShowLeftUnburied,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.CabinFever".Translate(),
                PSIMaterials[Icon.CabinFever],
                ref Settings.BarSettings.ShowCabinFever,
                ref Settings.PSISettings.ShowCabinFever,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Bedroom".Translate(),
                PSIMaterials[Icon.Bedroom],
                ref Settings.BarSettings.ShowBedroom,
                ref Settings.PSISettings.ShowBedroom,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Greedy".Translate(),
                PSIMaterials[Icon.Greedy],
                ref Settings.BarSettings.ShowGreedy,
                ref Settings.PSISettings.ShowGreedy,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Jealous".Translate(),
                PSIMaterials[Icon.Jealous],
                ref Settings.BarSettings.ShowJealous,
                ref Settings.PSISettings.ShowJealous,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Pyromaniac".Translate(),
                PSIMaterials[Icon.Pyromaniac],
                ref Settings.BarSettings.ShowPyromaniac,
                ref Settings.PSISettings.ShowPyromaniac,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Prosthophile".Translate(),
                PSIMaterials[Icon.Prosthophile],
                ref Settings.BarSettings.ShowProsthophile,
                ref Settings.PSISettings.ShowProsthophile,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Prosthophobe".Translate(),
                PSIMaterials[Icon.Prosthophobe],
                ref Settings.BarSettings.ShowProsthophobe,
                ref Settings.PSISettings.ShowProsthophobe,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Pacific".Translate(),
                PSIMaterials[Icon.Pacific],
                ref Settings.BarSettings.ShowPacific,
                ref Settings.PSISettings.ShowPacific,
                ref num);

            // DrawCheckboxArea("PSI.Settings.Visibility.Marriage".Translate(), PSIMaterials[Icons.Marriage], ref ColBarSettings.ShowMarriage, ref PSISettings.ShowMarriage, ref num);
            GUILayout.EndHorizontal();

            GUILayout.Space(Text.LineHeight / 2);
            GUILayout.EndScrollView();
        }

        private void FillPagePSIOpacityAndColor()
        {
            this._scrollPositionPSIOp = GUILayout.BeginScrollView(this._scrollPositionPSIOp);

            GUILayout.BeginVertical(this._fondBoxes);
            GUILayout.Label(
                "PSI.Settings.IconOpacityAndColor.Opacity".Translate() + (Settings.PSISettings.IconOpacity * 100).ToString("N0")
                + " %");
            Settings.PSISettings.IconOpacity = GUILayout.HorizontalSlider(Settings.PSISettings.IconOpacity, 0.1f, 1f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(this._fondBoxes);
            GUILayout.Label(
                "PSI.Settings.IconOpacityAndColor.OpacityCritical".Translate()
                + (Settings.PSISettings.IconOpacityCritical * 100).ToString("N0") + " %");
            Settings.PSISettings.IconOpacityCritical = GUILayout.HorizontalSlider(Settings.PSISettings.IconOpacityCritical, 0.1f, 1f);
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
                                                                        Settings.PSISettings.LimitBleedMult
                                                                            = 2f;
                                                                        Settings.PSISettings
                                                                                .LimitDiseaseLess =
                                                                            1f;
                                                                        Settings.PSISettings
                                                                                .LimitEfficiencyLess =
                                                                            0.6f;
                                                                        Settings.PSISettings.LimitFoodLess
                                                                            = 0.2f;

                                                                        // PSISettings.LimitMoodLess = 0.2f;
                                                                        Settings.PSISettings.LimitRestLess
                                                                            = 0.2f;
                                                                        Settings.PSISettings
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
                                                                        Settings.PSISettings.LimitBleedMult
                                                                            = 3f;
                                                                        Settings.PSISettings
                                                                                .LimitDiseaseLess =
                                                                            1f;
                                                                        Settings.PSISettings
                                                                                .LimitEfficiencyLess =
                                                                            0.75f;
                                                                        Settings.PSISettings.LimitFoodLess
                                                                            = 0.25f;

                                                                        // PSISettings.LimitMoodLess = 0.25f;
                                                                        Settings.PSISettings.LimitRestLess
                                                                            = 0.25f;
                                                                        Settings.PSISettings
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
                                                                        Settings.PSISettings.LimitBleedMult
                                                                            = 4f;
                                                                        Settings.PSISettings
                                                                                .LimitDiseaseLess =
                                                                            1f;
                                                                        Settings.PSISettings
                                                                                .LimitEfficiencyLess =
                                                                            0.9f;
                                                                        Settings.PSISettings.LimitFoodLess
                                                                            = 0.3f;

                                                                        // PSISettings.LimitMoodLess = 0.3f;
                                                                        Settings.PSISettings.LimitRestLess
                                                                            = 0.3f;
                                                                        Settings.PSISettings
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

            this._scrollPositionPSISens = GUILayout.BeginScrollView(this._scrollPositionPSISens);

            GUILayout.BeginVertical(this._fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Bleeding".Translate()
                + ("PSI.Settings.Sensitivity.Bleeding." + Math.Round(Settings.PSISettings.LimitBleedMult - 0.25)).Translate());
            Settings.PSISettings.LimitBleedMult = GUILayout.HorizontalSlider(Settings.PSISettings.LimitBleedMult, 0.5f, 5f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(this._fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Injured".Translate() + (int)(Settings.PSISettings.LimitEfficiencyLess * 100.0) + " %");
            Settings.PSISettings.LimitEfficiencyLess = GUILayout.HorizontalSlider(Settings.PSISettings.LimitEfficiencyLess, 0.01f, 1f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(this._fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Food".Translate() + (int)(Settings.PSISettings.LimitFoodLess * 100.0) + " %");
            Settings.PSISettings.LimitFoodLess = GUILayout.HorizontalSlider(Settings.PSISettings.LimitFoodLess, 0.01f, 0.99f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(this._fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Rest".Translate() + (int)(Settings.PSISettings.LimitRestLess * 100.0) + " %");
            Settings.PSISettings.LimitRestLess = GUILayout.HorizontalSlider(Settings.PSISettings.LimitRestLess, 0.01f, 0.99f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(this._fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Temperature".Translate() + (int)Settings.PSISettings.LimitTempComfortOffset + " °C");
            Settings.PSISettings.LimitTempComfortOffset =
                GUILayout.HorizontalSlider(Settings.PSISettings.LimitTempComfortOffset, -10f, 10f);
            GUILayout.EndVertical();

            GUILayout.EndScrollView();

            // if (!listing.DoTextButton("PSI.Settings.ReturnButton".Translate()))
            // return;
            // Page = "main";
        }

        private void FillPSIPageSizeArrangement()
        {
            this._scrollPositionPSISize = GUILayout.BeginScrollView(this._scrollPositionPSISize);

            GUILayout.BeginVertical(this._fondBoxes);
            Settings.BarSettings.UsePSI = GUILayout.Toggle(Settings.BarSettings.UsePSI, "CBKF.Settings.UsePSIOnBar".Translate());
            if (Settings.BarSettings.UsePSI)
            {
                GUILayout.BeginHorizontal();
                this.PSIBarPositionInt = GUILayout.Toolbar(this.PSIBarPositionInt, this._positionStrings);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.Label("PSI.Settings.Arrangement.IconsPerColumn".Translate() + Settings.BarSettings.IconsInColumn);
                Settings.BarSettings.IconsInColumn = (int)GUILayout.HorizontalSlider(Settings.BarSettings.IconsInColumn, 2f, 5f);
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical(this._fondBoxes);
            Settings.PSISettings.UsePSI = GUILayout.Toggle(Settings.PSISettings.UsePSI, "PSI.Settings.UsePSI".Translate());
            Settings.PSISettings.ShowRelationsOnStrangers = GUILayout.Toggle(
                Settings.PSISettings.ShowRelationsOnStrangers,
                "PSI.Settings.ShowRelationsOnStrangers".Translate());
            Settings.PSISettings.UsePSIOnPrisoner = GUILayout.Toggle(
                Settings.PSISettings.UsePSIOnPrisoner,
                "PSI.Settings.UsePSIOnPrisoner".Translate());
            Settings.PSISettings.UsePSIOnAnimals = GUILayout.Toggle(
                Settings.PSISettings.UsePSIOnAnimals,
                "PSI.Settings.UsePSIOnAnimals".Translate());

            if (Settings.PSISettings.UsePSI || Settings.PSISettings.UsePSIOnPrisoner)
            {
                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.BeginHorizontal();
                this.PSIPositionInt = GUILayout.Toolbar(this.PSIPositionInt, this._positionStrings);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(Text.LineHeight / 2);

                Settings.PSISettings.IconsHorizontal = GUILayout.Toggle(
                    Settings.PSISettings.IconsHorizontal,
                    "PSI.Settings.Arrangement.Horizontal".Translate());

                Settings.PSISettings.IconsScreenScale = GUILayout.Toggle(
                    Settings.PSISettings.IconsScreenScale,
                    "PSI.Settings.Arrangement.ScreenScale".Translate());

                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.Label("PSI.Settings.Arrangement.IconsPerColumn".Translate() + Settings.PSISettings.IconsInColumn);
                Settings.PSISettings.IconsInColumn = (int)GUILayout.HorizontalSlider(Settings.PSISettings.IconsInColumn, 1f, 7f);

                int num = (int)(Settings.PSISettings.IconSize * 4.5);

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
                Settings.PSISettings.IconSize = GUILayout.HorizontalSlider(Settings.PSISettings.IconSize, 0.5f, 2f);
                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.BeginVertical(this._fondBoxes);
                GUILayout.Label(
                    "PSI.Settings.Arrangement.IconPosition".Translate() + (int)(Settings.PSISettings.IconMarginX * 100.0)
                    + " x, " + (int)(Settings.PSISettings.IconMarginY * 100.0) + " y");
                Settings.PSISettings.IconMarginX = GUILayout.HorizontalSlider(Settings.PSISettings.IconMarginX, -2f, 2f);
                Settings.PSISettings.IconMarginY = GUILayout.HorizontalSlider(Settings.PSISettings.IconMarginY, -2f, 2f);
                GUILayout.EndVertical();

                GUILayout.BeginVertical(this._fondBoxes);
                GUILayout.Label(
                    "PSI.Settings.Arrangement.IconOffset".Translate() + (int)(Settings.PSISettings.IconOffsetX * 100.0) + " x, "
                    + (int)(Settings.PSISettings.IconOffsetY * 100.0) + " y");
                Settings.PSISettings.IconOffsetX = GUILayout.HorizontalSlider(Settings.PSISettings.IconOffsetX, -2f, 2f);
                Settings.PSISettings.IconOffsetY = GUILayout.HorizontalSlider(Settings.PSISettings.IconOffsetY, -2f, 2f);
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
            GUILayout.Label(string.Empty, this._grayLines, GUILayout.Height(1));
            GUILayout.Space(Text.LineHeight / 4);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(labelstring, this._fontBold);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(Text.LineHeight / 2);
            GUILayout.Label(string.Empty, this._grayLines, GUILayout.Height(1));
            GUILayout.Space(Text.LineHeight / 2);
        }

        private void ResetBarSettings()
        {
            Settings.BarSettings = new SettingsColonistBar();
        }

        private void ResetPSISettings()
        {
            Settings.PSISettings = new SettingsPSI();
        }
    }
}