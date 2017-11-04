using static ColonistBarKF.PSI.GameComponentPSI;

namespace ColonistBarKF
{
    using ColonistBarKF.Bar;
    using JetBrains.Annotations;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using Verse;

    public class ColonistBarKfSettings : Window
    {
        private static readonly string Cbkfversion = "Colonist Bar KF 0.18.0";

        private static int iconLimit;

        [NotNull]
        private readonly GUIStyle darkGrayBgImage =
            new GUIStyle { normal = { background = ColonistBarTextures.GrayFond } };

        [NotNull]
        private readonly GUIStyle fondBoxes =
            new GUIStyle
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

        [NotNull]
        private readonly GUIStyle fondImages =
            new GUIStyle
            {
                normal = {
                                background = ColonistBarTextures.DarkGrayFond
                             },
                hover = {
                               background = ColonistBarTextures.RedHover
                            }
            };

        [NotNull]
        private readonly GUIStyle fontBold =
            new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                normal = {
                                textColor = Color.white
                             },
                padding = new RectOffset(0, 0, 5, 0)
            };

        [NotNull]
        private readonly GUIStyle grayLines = new GUIStyle { normal = { background = ColonistBarTextures.GrayLines } };

        [NotNull]
        private readonly GUIStyle headline =
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
        private readonly string[] mainToolbarStrings =
            { "CBKF.Settings.ColonistBar".Translate(), "CBKF.Settings.PSI".Translate() };

        [NotNull]
        private readonly string[] positionStrings =
            {
                "CBKF.Settings.useLeft".Translate(), "CBKF.Settings.useRight".Translate(),
                "CBKF.Settings.useTop".Translate(), "CBKF.Settings.useBottom".Translate()
            };

        [NotNull]
        private readonly string[] psiToolbarStrings =
            {
                "PSI.Settings.ArrangementButton".Translate(), "PSI.Settings.OpacityButton".Translate(),
                "PSI.Settings.IconButton".Translate(), "PSI.Settings.SensitivityButton".Translate()
            };

        private int moodBarPositionInt;

        private int psiBarPositionInt;

        private int psiPositionInt;

        private Vector2 scrollPositionBase;

        private Vector2 scrollPositionPSI;

        private Vector2 scrollPositionPSIOp;

        private Vector2 scrollPositionPSISens;

        private Vector2 scrollPositionPSISize;

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
                switch (Settings.ColBarSettings.MoodBarPos)
                {
                    case Position.Alignment.Left:
                        this.moodBarPositionInt = 0;
                        break;

                    case Position.Alignment.Right:
                        this.moodBarPositionInt = 1;
                        break;

                    case Position.Alignment.Top:
                        this.moodBarPositionInt = 2;
                        break;

                    case Position.Alignment.Bottom:
                        this.moodBarPositionInt = 3;
                        break;
                }

                return this.moodBarPositionInt;
            }

            set
            {
                switch (value)
                {
                    case 0:
                        Settings.ColBarSettings.MoodBarPos = Position.Alignment.Left;
                        break;

                    case 1:
                        Settings.ColBarSettings.MoodBarPos = Position.Alignment.Right;
                        break;

                    case 2:
                        Settings.ColBarSettings.MoodBarPos = Position.Alignment.Top;
                        break;

                    case 3:
                        Settings.ColBarSettings.MoodBarPos = Position.Alignment.Bottom;
                        break;

                    default:
                        Settings.ColBarSettings.MoodBarPos = Position.Alignment.Right;
                        break;
                }

                this.moodBarPositionInt = value;
            }
        }

        private int PsiBarPositionInt
        {
            get
            {
                if (Settings.ColBarSettings.ColBarPsiIconPos == Position.Alignment.Left)
                {
                    this.psiBarPositionInt = 0;
                }

                if (Settings.ColBarSettings.ColBarPsiIconPos == Position.Alignment.Right)
                {
                    this.psiBarPositionInt = 1;
                }

                if (Settings.ColBarSettings.ColBarPsiIconPos == Position.Alignment.Top)
                {
                    this.psiBarPositionInt = 2;
                }

                if (Settings.ColBarSettings.ColBarPsiIconPos == Position.Alignment.Bottom)
                {
                    this.psiBarPositionInt = 3;
                }

                return this.psiBarPositionInt;
            }

            set
            {
                switch (value)
                {
                    case 0:
                        Settings.ColBarSettings.ColBarPsiIconPos = Position.Alignment.Left;
                        Settings.ColBarSettings.IconOffsetX = 1f;
                        Settings.ColBarSettings.IconOffsetY = 1f;
                        Settings.ColBarSettings.IconsHorizontal = false;
                        break;

                    case 1:
                        Settings.ColBarSettings.ColBarPsiIconPos = Position.Alignment.Right;
                        Settings.ColBarSettings.IconOffsetX = -1f;
                        Settings.ColBarSettings.IconOffsetY = 1f;
                        Settings.ColBarSettings.IconsHorizontal = false;
                        break;

                    case 2:
                        Settings.ColBarSettings.ColBarPsiIconPos = Position.Alignment.Top;
                        Settings.ColBarSettings.IconOffsetX = -1f;
                        Settings.ColBarSettings.IconOffsetY = 1f;
                        Settings.ColBarSettings.IconsHorizontal = true;
                        break;

                    case 3:
                        Settings.ColBarSettings.ColBarPsiIconPos = Position.Alignment.Bottom;
                        Settings.ColBarSettings.IconOffsetX = -1;
                        Settings.ColBarSettings.IconOffsetY = -1;
                        Settings.ColBarSettings.IconsHorizontal = true;
                        break;

                    default:
                        Settings.ColBarSettings.ColBarPsiIconPos = 0;

                        break;
                }

                this.psiBarPositionInt = value;
            }
        }

        private int PsiPositionInt
        {
            get
            {
                if (Settings.PsiSettings.IconAlignment == 0)
                {
                    this.psiPositionInt = 0;
                }

                if (Settings.PsiSettings.IconAlignment == 1)
                {
                    this.psiPositionInt = 1;
                }

                if (Settings.PsiSettings.IconAlignment == 2)
                {
                    this.psiPositionInt = 2;
                }

                if (Settings.PsiSettings.IconAlignment == 3)
                {
                    this.psiPositionInt = 3;
                }

                return this.psiPositionInt;
            }

            set
            {
                if (value == this.psiPositionInt)
                {
                    return;
                }

                switch (value)
                {
                    case 0:
                        Settings.PsiSettings.IconAlignment = value;
                        Settings.PsiSettings.IconMarginX = 1f;
                        Settings.PsiSettings.IconMarginY = 1f;
                        Settings.PsiSettings.IconOffsetX = 1f;
                        Settings.PsiSettings.IconOffsetY = 1f;
                        Settings.PsiSettings.IconsHorizontal = false;
                        Settings.PsiSettings.IconsScreenScale = true;
                        Settings.PsiSettings.IconsInColumn = 3;
                        Settings.PsiSettings.IconSize = 1f;
                        Settings.PsiSettings.IconOpacity = 0.5f;
                        Settings.PsiSettings.IconOpacityCritical = 0.8f;
                        break;

                    case 1:
                        Settings.PsiSettings.IconAlignment = value;
                        Settings.PsiSettings.IconMarginX = -1f;
                        Settings.PsiSettings.IconMarginY = 1f;
                        Settings.PsiSettings.IconOffsetX = -1f;
                        Settings.PsiSettings.IconOffsetY = 1f;
                        Settings.PsiSettings.IconsHorizontal = false;
                        Settings.PsiSettings.IconsScreenScale = true;
                        Settings.PsiSettings.IconsInColumn = 3;
                        Settings.PsiSettings.IconSize = 1f;
                        Settings.PsiSettings.IconOpacity = 0.5f;
                        Settings.PsiSettings.IconOpacityCritical = 0.8f;
                        break;

                    case 2:
                        Settings.PsiSettings.IconAlignment = value;
                        Settings.PsiSettings.IconMarginX = 1f;
                        Settings.PsiSettings.IconMarginY = -1.63f;
                        Settings.PsiSettings.IconOffsetX = -1f;
                        Settings.PsiSettings.IconOffsetY = 1f;
                        Settings.PsiSettings.IconsHorizontal = true;
                        Settings.PsiSettings.IconsScreenScale = true;
                        Settings.PsiSettings.IconsInColumn = 3;
                        Settings.PsiSettings.IconSize = 1f;
                        Settings.PsiSettings.IconOpacity = 0.5f;
                        Settings.PsiSettings.IconOpacityCritical = 0.8f;
                        break;

                    case 3:
                        Settings.PsiSettings.IconAlignment = value;
                        Settings.PsiSettings.IconMarginX = 1.139534f;
                        Settings.PsiSettings.IconMarginY = 1.375f;
                        Settings.PsiSettings.IconOffsetX = -0.9534883f;
                        Settings.PsiSettings.IconOffsetY = -0.9534884f;
                        Settings.PsiSettings.IconsHorizontal = true;
                        Settings.PsiSettings.IconsScreenScale = true;
                        Settings.PsiSettings.IconsInColumn = 4;
                        Settings.PsiSettings.IconSize = 1.084302f;
                        Settings.PsiSettings.IconOpacity = 0.5f;
                        Settings.PsiSettings.IconOpacityCritical = 0.8f;
                        break;

                    default:
                        Settings.PsiSettings.IconAlignment = 0;

                        break;
                }

                this.psiPositionInt = value;
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
            GUILayout.Label(Cbkfversion, this.headline);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(Text.LineHeight / 2);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            this.MainToolbarInt = GUILayout.Toolbar(this.MainToolbarInt, this.mainToolbarStrings);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            switch (this.MainToolbarInt)
            {
                case 0:
                    {
                        GUILayout.Space(Text.LineHeight / 2);

                        this.scrollPositionBase = GUILayout.BeginScrollView(this.scrollPositionBase);

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
                        this.PSIToolbarInt = GUILayout.SelectionGrid(
                            this.PSIToolbarInt,
                            this.psiToolbarStrings,
                            toolbarInt > this.psiToolbarStrings.Length ? this.psiToolbarStrings.Length : toolbarInt);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        GUILayout.Space(Text.LineHeight / 2);

                        if (this.PSIToolbarInt == 0)
                        {
                            {
                                this.FillPSIPageSizeArrangement();
                            }
                        }
                        else if (this.PSIToolbarInt == 1)
                        {
                            {
                                this.FillPagePSIOpacityAndColor();
                            }
                        }
                        else if (this.PSIToolbarInt == 2)
                        {
                            {
                                this.FillPagePSIIconSet(viewRect);
                            }
                        }
                        else if (this.PSIToolbarInt == 3)
                        {
                            {
                                this.FillPSIPageSensitivity();
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
                            this.FillPagePSIIconSet(viewRect);
                        }
                    }

                    break;
            }

            GUILayout.FlexibleSpace();
            GUILayout.Space(Text.LineHeight / 2);
            GUILayout.Label(string.Empty, this.grayLines, GUILayout.Height(1));
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
            Settings.SavePsiSettings();
        }

        private void DrawCheckboxArea(
            [NotNull] string iconName,
            [NotNull] Material iconMaterial,
            ref bool colBarBool,
            ref bool psiBarBool,
            ref int iconInRow)
        {
            if (iconInRow > iconLimit - 1)
            {
                GUILayout.EndHorizontal();

                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.BeginHorizontal();
                iconInRow = 0;
            }

            if (iconInRow > 0 && iconLimit != 1)
            {
                GUILayout.Space(Text.LineHeight / 2);
            }

            GUILayout.BeginVertical(this.fondImages);

            GUILayout.BeginHorizontal(this.darkGrayBgImage, GUILayout.Height(Text.LineHeight * 1.2f));
            GUILayout.FlexibleSpace();
            GUILayout.Label(iconName, this.fontBold);
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
            GUILayout.BeginVertical(this.fondImages);

            GUILayout.BeginHorizontal(this.darkGrayBgImage, GUILayout.Height(Text.LineHeight * 1.2f));
            GUILayout.FlexibleSpace();
            GUILayout.Label(iconName, this.fontBold);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical();
            GUILayout.Space(Text.LineHeight / 2);
            if (Settings.PsiSettings.ShowTargetPoint)
            {
                if (!Settings.PsiSettings.UseColoredTarget)
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
            Settings.PsiSettings.ShowTargetPoint = GUILayout.Toggle(
                Settings.PsiSettings.ShowTargetPoint,
                "PSI.Settings.Visibility.TargetPoint".Translate());
            Settings.PsiSettings.UseColoredTarget = GUILayout.Toggle(
                Settings.PsiSettings.UseColoredTarget,
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
            GUILayout.BeginVertical(this.fondBoxes);

            Settings.ColBarSettings.UseCustomIconSize = GUILayout.Toggle(
                Settings.ColBarSettings.UseCustomIconSize,
                "CBKF.Settings.BasicSize".Translate() + Settings.ColBarSettings.BaseSizeFloat.ToString("N0") + " px, "
                + ColonistBar_KF.BarHelperKf.cachedScale.ToStringPercent() + " %, "
                + (int)Settings.ColBarSettings.BaseSpacingHorizontal + " x, " + (int)Settings.ColBarSettings.BaseSpacingVertical + " y");

            if (Settings.ColBarSettings.UseCustomIconSize)
            {
                GUILayout.Space(Text.LineHeight / 2);

                Settings.ColBarSettings.BaseSizeFloat = GUILayout.HorizontalSlider(Settings.ColBarSettings.BaseSizeFloat, 24f, 256f);

                Settings.ColBarSettings.BaseSpacingHorizontal =
                    GUILayout.HorizontalSlider(Settings.ColBarSettings.BaseSpacingHorizontal, 1f, 72f);
                Settings.ColBarSettings.BaseSpacingVertical =
                    GUILayout.HorizontalSlider(Settings.ColBarSettings.BaseSpacingVertical, 1f, 96f);
            }
            else
            {
                Settings.ColBarSettings.BaseSizeFloat = 48f;
                Settings.ColBarSettings.BaseSpacingHorizontal = 24f;
                Settings.ColBarSettings.BaseSpacingVertical = 32f;
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);
            Settings.ColBarSettings.UseCustomPawnTextureCameraOffsets = GUILayout.Toggle(
                Settings.ColBarSettings.UseCustomPawnTextureCameraOffsets,
                "CBKF.Settings.PawnTextureCameraOffsets".Translate()
                + Settings.ColBarSettings.PawnTextureCameraHorizontalOffset.ToString("N2") + " x, "
                + Settings.ColBarSettings.PawnTextureCameraVerticalOffset.ToString("N2") + " y, "
                + Settings.ColBarSettings.PawnTextureCameraZoom.ToString("N2") + " z");
            if (Settings.ColBarSettings.UseCustomPawnTextureCameraOffsets)
            {
                GUILayout.Space(Text.LineHeight / 2);
                Settings.ColBarSettings.PawnTextureCameraHorizontalOffset = GUILayout.HorizontalSlider(
                    Settings.ColBarSettings.PawnTextureCameraHorizontalOffset,
                    0.7f,
                    -0.7f);
                Settings.ColBarSettings.PawnTextureCameraVerticalOffset =
                    GUILayout.HorizontalSlider(Settings.ColBarSettings.PawnTextureCameraVerticalOffset, 0f, 1f);
                Settings.ColBarSettings.PawnTextureCameraZoom =
                    GUILayout.HorizontalSlider(Settings.ColBarSettings.PawnTextureCameraZoom, 0.3f, 3f);
            }
            else
            {
                Settings.ColBarSettings.PawnTextureCameraHorizontalOffset = 0f;
                Settings.ColBarSettings.PawnTextureCameraVerticalOffset = 0.3f;
                Settings.ColBarSettings.PawnTextureCameraZoom = 1.28205f;
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
            GUILayout.BeginVertical(this.fondBoxes);
            Settings.ColBarSettings.UseGrouping = GUILayout.Toggle(
                Settings.ColBarSettings.UseGrouping,
                "CBKF.Settings.UseGrouping".Translate());

            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);
            Settings.ColBarSettings.UseGroupColors = GUILayout.Toggle(
                Settings.ColBarSettings.UseGroupColors,
                "CBKF.Settings.UseGroupColors".Translate());

            GUILayout.EndVertical();
        }

        private void FillPageMain()
        {
            GUILayout.BeginVertical(this.fondBoxes);
            Settings.ColBarSettings.UseCustomMarginTop = GUILayout.Toggle(
                Settings.ColBarSettings.UseCustomMarginTop,
                "CBKF.Settings.ColonistBarOffset".Translate() + (int)Settings.ColBarSettings.MarginTop + " y \n"
                + "CBKF.Settings.MaxColonistBarWidth".Translate() + ": "
                + ((float)UI.screenWidth - (int)Settings.ColBarSettings.MarginHorizontal) + " px");

            if (Settings.ColBarSettings.UseCustomMarginTop)
            {
                GUILayout.Space(Text.LineHeight / 2);
                Settings.ColBarSettings.MarginTop =
                    GUILayout.HorizontalSlider(Settings.ColBarSettings.MarginTop, 0f, (float)UI.screenHeight / 6);
                Settings.ColBarSettings.MarginHorizontal = GUILayout.HorizontalSlider(
                    Settings.ColBarSettings.MarginHorizontal,
                    (float)UI.screenWidth * 3 / 5,
                    0f);
            }
            else
            {
                Settings.ColBarSettings.MarginTop = 21f;
                Settings.ColBarSettings.MarginHorizontal = 520f;
            }

            // listing.Gap(3f);
            GUILayout.EndVertical();

            // listing.Gap(3f);
            GUILayout.BeginVertical(this.fondBoxes);
            Settings.ColBarSettings.UseCustomRowCount = GUILayout.Toggle(
                Settings.ColBarSettings.UseCustomRowCount,
                "PSI.Settings.Arrangement.ColonistsPerColumn".Translate()
                + (Settings.ColBarSettings.UseCustomRowCount ? Settings.ColBarSettings.MaxRowsCustom : 3));
            if (Settings.ColBarSettings.UseCustomRowCount)
            {
                Settings.ColBarSettings.MaxRowsCustom = (int)GUILayout.HorizontalSlider(Settings.ColBarSettings.MaxRowsCustom, 1f, 5f);
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);

            Settings.ColBarSettings.UseWeaponIcons = GUILayout.Toggle(
                Settings.ColBarSettings.UseWeaponIcons,
                "CBKF.Settings.UseWeaponIcons".Translate());

            Settings.ColBarSettings.UseGender = GUILayout.Toggle(
                Settings.ColBarSettings.UseGender,
                "CBKF.Settings.useGender".Translate());

            Settings.ColBarSettings.useZoomToMouse = GUILayout.Toggle(
                Settings.ColBarSettings.useZoomToMouse,
                "CBKF.Settings.useZoomToMouse".Translate());

            GUILayout.Label("FollowMe.MiddleClick".Translate());

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
            GUILayout.BeginVertical(this.fondBoxes);
            Settings.ColBarSettings.UseNewMood = GUILayout.Toggle(
                Settings.ColBarSettings.UseNewMood,
                "CBKF.Settings.UseNewMood".Translate());

            if (Settings.ColBarSettings.UseNewMood)
            {
                Settings.ColBarSettings.UseExternalMoodBar = GUILayout.Toggle(
                    Settings.ColBarSettings.UseExternalMoodBar,
                    "CBKF.Settings.UseExternalMoodBar".Translate());

                if (Settings.ColBarSettings.UseExternalMoodBar)
                {
                    GUILayout.BeginHorizontal();
                    this.MoodBarPositionInt = GUILayout.Toolbar(this.MoodBarPositionInt, this.positionStrings);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                Settings.ColBarSettings.UseExternalMoodBar = false;
            }

            GUILayout.EndVertical();

            GUILayout.EndVertical();
        }

        private void FillPagePSIIconSet(Rect viewRect)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("PSI.Settings.IconSet".Translate() + Settings.PsiSettings.IconSet))
            {
                FloatMenuOption fmoDefault = new FloatMenuOption(
                    "PSI.Settings.Preset.0".Translate(),
                    () =>
                        {
                            try
                            {
                                Settings.PsiSettings.IconSet = "default";
                                Settings.PsiSettings.UseColoredTarget = true;
                                Settings.SavePsiSettings();
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
                                Settings.PsiSettings.IconSet = "original";
                                Settings.PsiSettings.UseColoredTarget = false;
                                Settings.SavePsiSettings();
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

            iconLimit = Mathf.FloorToInt(viewRect.width / 125f);

            GUILayout.Space(Text.LineHeight / 2);

            this.scrollPositionPSI = GUILayout.BeginScrollView(this.scrollPositionPSI);

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
                ref Settings.ColBarSettings.ShowDraft,
                ref Settings.PsiSettings.ShowDraft,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Unarmed".Translate(),
                PSIMaterials[Icon.Unarmed],
                ref Settings.ColBarSettings.ShowUnarmed,
                ref Settings.PsiSettings.ShowUnarmed,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Idle".Translate(),
                PSIMaterials[Icon.Idle],
                ref Settings.ColBarSettings.ShowIdle,
                ref Settings.PsiSettings.ShowIdle,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Sad".Translate(),
                PSIMaterials[Icon.Sad],
                ref Settings.ColBarSettings.ShowSad,
                ref Settings.PsiSettings.ShowSad,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Aggressive".Translate(),
                PSIMaterials[Icon.Aggressive],
                ref Settings.ColBarSettings.ShowAggressive,
                ref Settings.PsiSettings.ShowAggressive,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Panic".Translate(),
                PSIMaterials[Icon.Panic],
                ref Settings.ColBarSettings.ShowPanic,
                ref Settings.PsiSettings.ShowPanic,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Dazed".Translate(),
                PSIMaterials[Icon.Dazed],
                ref Settings.ColBarSettings.ShowDazed,
                ref Settings.PsiSettings.ShowDazed,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Leave".Translate(),
                PSIMaterials[Icon.Leave],
                ref Settings.ColBarSettings.ShowLeave,
                ref Settings.PsiSettings.ShowLeave,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Hungry".Translate(),
                PSIMaterials[Icon.Hungry],
                ref Settings.ColBarSettings.ShowHungry,
                ref Settings.PsiSettings.ShowHungry,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Tired".Translate(),
                PSIMaterials[Icon.Tired],
                ref Settings.ColBarSettings.ShowTired,
                ref Settings.PsiSettings.ShowTired,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.TooCold".Translate(),
                PSIMaterials[Icon.TooCold],
                ref Settings.ColBarSettings.ShowTooCold,
                ref Settings.PsiSettings.ShowTooCold,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.TooHot".Translate(),
                PSIMaterials[Icon.TooHot],
                ref Settings.ColBarSettings.ShowTooHot,
                ref Settings.PsiSettings.ShowTooHot,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.ApparelHealth".Translate(),
                PSIMaterials[Icon.ApparelHealth],
                ref Settings.ColBarSettings.ShowApparelHealth,
                ref Settings.PsiSettings.ShowApparelHealth,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Naked".Translate(),
                PSIMaterials[Icon.Naked],
                ref Settings.ColBarSettings.ShowNaked,
                ref Settings.PsiSettings.ShowNaked,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Health".Translate(),
                PSIMaterials[Icon.Health],
                ref Settings.ColBarSettings.ShowHealth,
                ref Settings.PsiSettings.ShowHealth,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.MedicalAttention".Translate(),
                PSIMaterials[Icon.MedicalAttention],
                ref Settings.ColBarSettings.ShowMedicalAttention,
                ref Settings.PsiSettings.ShowMedicalAttention,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Injury".Translate(),
                PSIMaterials[Icon.Effectiveness],
                ref Settings.ColBarSettings.ShowEffectiveness,
                ref Settings.PsiSettings.ShowEffectiveness,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Bloodloss".Translate(),
                PSIMaterials[Icon.Bloodloss],
                ref Settings.ColBarSettings.ShowBloodloss,
                ref Settings.PsiSettings.ShowBloodloss,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Pain".Translate(),
                PSIMaterials[Icon.Pain],
                ref Settings.ColBarSettings.ShowPain,
                ref Settings.PsiSettings.ShowPain,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Drunk".Translate(),
                PSIMaterials[Icon.Drunk],
                ref Settings.ColBarSettings.ShowDrunk,
                ref Settings.PsiSettings.ShowDrunk,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Toxicity".Translate(),
                PSIMaterials[Icon.Toxicity],
                ref Settings.ColBarSettings.ShowToxicity,
                ref Settings.PsiSettings.ShowToxicity,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.NightOwl".Translate(),
                PSIMaterials[Icon.NightOwl],
                ref Settings.ColBarSettings.ShowNightOwl,
                ref Settings.PsiSettings.ShowNightOwl,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.LeftUnburied".Translate(),
                PSIMaterials[Icon.LeftUnburied],
                ref Settings.ColBarSettings.ShowLeftUnburied,
                ref Settings.PsiSettings.ShowLeftUnburied,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.CabinFever".Translate(),
                PSIMaterials[Icon.CabinFever],
                ref Settings.ColBarSettings.ShowCabinFever,
                ref Settings.PsiSettings.ShowCabinFever,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Bedroom".Translate(),
                PSIMaterials[Icon.Bedroom],
                ref Settings.ColBarSettings.ShowBedroom,
                ref Settings.PsiSettings.ShowBedroom,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Greedy".Translate(),
                PSIMaterials[Icon.Greedy],
                ref Settings.ColBarSettings.ShowGreedy,
                ref Settings.PsiSettings.ShowGreedy,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Jealous".Translate(),
                PSIMaterials[Icon.Jealous],
                ref Settings.ColBarSettings.ShowJealous,
                ref Settings.PsiSettings.ShowJealous,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Pyromaniac".Translate(),
                PSIMaterials[Icon.Pyromaniac],
                ref Settings.ColBarSettings.ShowPyromaniac,
                ref Settings.PsiSettings.ShowPyromaniac,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Prosthophile".Translate(),
                PSIMaterials[Icon.Prosthophile],
                ref Settings.ColBarSettings.ShowProsthophile,
                ref Settings.PsiSettings.ShowProsthophile,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Prosthophobe".Translate(),
                PSIMaterials[Icon.Prosthophobe],
                ref Settings.ColBarSettings.ShowProsthophobe,
                ref Settings.PsiSettings.ShowProsthophobe,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Pacific".Translate(),
                PSIMaterials[Icon.Pacific],
                ref Settings.ColBarSettings.ShowPacific,
                ref Settings.PsiSettings.ShowPacific,
                ref num);

            // DrawCheckboxArea("PSI.Settings.Visibility.Marriage".Translate(), PSIMaterials[Icons.Marriage], ref ColBarSettings.ShowMarriage, ref PsiSettings.ShowMarriage, ref num);
            GUILayout.EndHorizontal();

            GUILayout.Space(Text.LineHeight / 2);
            GUILayout.EndScrollView();
        }

        private void FillPagePSIOpacityAndColor()
        {
            this.scrollPositionPSIOp = GUILayout.BeginScrollView(this.scrollPositionPSIOp);

            GUILayout.BeginVertical(this.fondBoxes);
            GUILayout.Label(
                "PSI.Settings.IconOpacityAndColor.Opacity".Translate() + (Settings.PsiSettings.IconOpacity * 100).ToString("N0")
                + " %");
            Settings.PsiSettings.IconOpacity = GUILayout.HorizontalSlider(Settings.PsiSettings.IconOpacity, 0.1f, 1f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);
            GUILayout.Label(
                "PSI.Settings.IconOpacityAndColor.OpacityCritical".Translate()
                + (Settings.PsiSettings.IconOpacityCritical * 100).ToString("N0") + " %");
            Settings.PsiSettings.IconOpacityCritical = GUILayout.HorizontalSlider(Settings.PsiSettings.IconOpacityCritical, 0.1f, 1f);
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
                                                                        Settings.PsiSettings.LimitBleedMult
                                                                            = 2f;
                                                                        Settings.PsiSettings
                                                                                .LimitDiseaseLess =
                                                                            1f;
                                                                        Settings.PsiSettings
                                                                                .LimitEfficiencyLess =
                                                                            0.6f;
                                                                        Settings.PsiSettings.LimitFoodLess
                                                                            = 0.2f;

                                                                        // PsiSettings.LimitMoodLess = 0.2f;
                                                                        Settings.PsiSettings.LimitRestLess
                                                                            = 0.2f;
                                                                        Settings.PsiSettings
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
                                                                        Settings.PsiSettings.LimitBleedMult
                                                                            = 3f;
                                                                        Settings.PsiSettings
                                                                                .LimitDiseaseLess =
                                                                            1f;
                                                                        Settings.PsiSettings
                                                                                .LimitEfficiencyLess =
                                                                            0.75f;
                                                                        Settings.PsiSettings.LimitFoodLess
                                                                            = 0.25f;

                                                                        // PsiSettings.LimitMoodLess = 0.25f;
                                                                        Settings.PsiSettings.LimitRestLess
                                                                            = 0.25f;
                                                                        Settings.PsiSettings
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
                                                                        Settings.PsiSettings.LimitBleedMult
                                                                            = 4f;
                                                                        Settings.PsiSettings
                                                                                .LimitDiseaseLess =
                                                                            1f;
                                                                        Settings.PsiSettings
                                                                                .LimitEfficiencyLess =
                                                                            0.9f;
                                                                        Settings.PsiSettings.LimitFoodLess
                                                                            = 0.3f;

                                                                        // PsiSettings.LimitMoodLess = 0.3f;
                                                                        Settings.PsiSettings.LimitRestLess
                                                                            = 0.3f;
                                                                        Settings.PsiSettings
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

            this.scrollPositionPSISens = GUILayout.BeginScrollView(this.scrollPositionPSISens);

            GUILayout.BeginVertical(this.fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Bleeding".Translate()
                + ("PSI.Settings.Sensitivity.Bleeding." + Math.Round(Settings.PsiSettings.LimitBleedMult - 0.25)).Translate());
            Settings.PsiSettings.LimitBleedMult = GUILayout.HorizontalSlider(Settings.PsiSettings.LimitBleedMult, 0.5f, 5f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Injured".Translate() + (int)(Settings.PsiSettings.LimitEfficiencyLess * 100.0) + " %");
            Settings.PsiSettings.LimitEfficiencyLess = GUILayout.HorizontalSlider(Settings.PsiSettings.LimitEfficiencyLess, 0.01f, 1f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Food".Translate() + (int)(Settings.PsiSettings.LimitFoodLess * 100.0) + " %");
            Settings.PsiSettings.LimitFoodLess = GUILayout.HorizontalSlider(Settings.PsiSettings.LimitFoodLess, 0.01f, 0.99f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Rest".Translate() + (int)(Settings.PsiSettings.LimitRestLess * 100.0) + " %");
            Settings.PsiSettings.LimitRestLess = GUILayout.HorizontalSlider(Settings.PsiSettings.LimitRestLess, 0.01f, 0.99f);
            GUILayout.EndVertical();

            // Replaced with thought check => human leather, dead man's apparel
            // GUILayout.BeginVertical(this._fondBoxes);
            // GUILayout.Label(
            // "PSI.Settings.Sensitivity.ApparelHealth".Translate() + (int)(PsiSettings.LimitApparelHealthLess * 100.0)
            // + " %");
            // PsiSettings.LimitApparelHealthLess =
            // GUILayout.HorizontalSlider(PsiSettings.LimitApparelHealthLess, 0.01f, 0.99f);
            // GUILayout.EndVertical();
            GUILayout.BeginVertical(this.fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Temperature".Translate() + (int)Settings.PsiSettings.LimitTempComfortOffset + " °C");
            Settings.PsiSettings.LimitTempComfortOffset =
                GUILayout.HorizontalSlider(Settings.PsiSettings.LimitTempComfortOffset, -10f, 10f);
            GUILayout.EndVertical();

            GUILayout.EndScrollView();

            // if (!listing.DoTextButton("PSI.Settings.ReturnButton".Translate()))
            // return;
            // Page = "main";
        }

        private void FillPSIPageSizeArrangement()
        {
            this.scrollPositionPSISize = GUILayout.BeginScrollView(this.scrollPositionPSISize);

            GUILayout.BeginVertical(this.fondBoxes);
            Settings.ColBarSettings.UsePsi = GUILayout.Toggle(Settings.ColBarSettings.UsePsi, "CBKF.Settings.UsePsiOnBar".Translate());
            if (Settings.ColBarSettings.UsePsi)
            {
                GUILayout.BeginHorizontal();
                this.PsiBarPositionInt = GUILayout.Toolbar(this.PsiBarPositionInt, this.positionStrings);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.Label("PSI.Settings.Arrangement.IconsPerColumn".Translate() + Settings.ColBarSettings.IconsInColumn);
                Settings.ColBarSettings.IconsInColumn = (int)GUILayout.HorizontalSlider(Settings.ColBarSettings.IconsInColumn, 2f, 5f);
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);
            Settings.PsiSettings.UsePsi = GUILayout.Toggle(Settings.PsiSettings.UsePsi, "PSI.Settings.UsePSI".Translate());
            Settings.PsiSettings.ShowRelationsOnStrangers = GUILayout.Toggle(
                Settings.PsiSettings.ShowRelationsOnStrangers,
                "PSI.Settings.ShowRelationsOnStrangers".Translate());
            Settings.PsiSettings.UsePsiOnPrisoner = GUILayout.Toggle(
                Settings.PsiSettings.UsePsiOnPrisoner,
                "PSI.Settings.UsePSIOnPrisoner".Translate());
            Settings.PsiSettings.UsePsiOnAnimals = GUILayout.Toggle(
                Settings.PsiSettings.UsePsiOnAnimals,
                "PSI.Settings.UsePsiOnAnimals".Translate());

            if (Settings.PsiSettings.UsePsi || Settings.PsiSettings.UsePsiOnPrisoner)
            {
                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.BeginHorizontal();
                this.PsiPositionInt = GUILayout.Toolbar(this.PsiPositionInt, this.positionStrings);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(Text.LineHeight / 2);

                Settings.PsiSettings.IconsHorizontal = GUILayout.Toggle(
                    Settings.PsiSettings.IconsHorizontal,
                    "PSI.Settings.Arrangement.Horizontal".Translate());

                Settings.PsiSettings.IconsScreenScale = GUILayout.Toggle(
                    Settings.PsiSettings.IconsScreenScale,
                    "PSI.Settings.Arrangement.ScreenScale".Translate());

                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.Label("PSI.Settings.Arrangement.IconsPerColumn".Translate() + Settings.PsiSettings.IconsInColumn);
                Settings.PsiSettings.IconsInColumn = (int)GUILayout.HorizontalSlider(Settings.PsiSettings.IconsInColumn, 1f, 7f);

                int num = (int)(Settings.PsiSettings.IconSize * 4.5);

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
                Settings.PsiSettings.IconSize = GUILayout.HorizontalSlider(Settings.PsiSettings.IconSize, 0.5f, 2f);
                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.BeginVertical(this.fondBoxes);
                GUILayout.Label(
                    "PSI.Settings.Arrangement.IconPosition".Translate() + (int)(Settings.PsiSettings.IconMarginX * 100.0)
                    + " x, " + (int)(Settings.PsiSettings.IconMarginY * 100.0) + " y");
                Settings.PsiSettings.IconMarginX = GUILayout.HorizontalSlider(Settings.PsiSettings.IconMarginX, -2f, 2f);
                Settings.PsiSettings.IconMarginY = GUILayout.HorizontalSlider(Settings.PsiSettings.IconMarginY, -2f, 2f);
                GUILayout.EndVertical();

                GUILayout.BeginVertical(this.fondBoxes);
                GUILayout.Label(
                    "PSI.Settings.Arrangement.IconOffset".Translate() + (int)(Settings.PsiSettings.IconOffsetX * 100.0) + " x, "
                    + (int)(Settings.PsiSettings.IconOffsetY * 100.0) + " y");
                Settings.PsiSettings.IconOffsetX = GUILayout.HorizontalSlider(Settings.PsiSettings.IconOffsetX, -2f, 2f);
                Settings.PsiSettings.IconOffsetY = GUILayout.HorizontalSlider(Settings.PsiSettings.IconOffsetY, -2f, 2f);
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
            GUILayout.Label(string.Empty, this.grayLines, GUILayout.Height(1));
            GUILayout.Space(Text.LineHeight / 4);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(labelstring, this.fontBold);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(Text.LineHeight / 2);
            GUILayout.Label(string.Empty, this.grayLines, GUILayout.Height(1));
            GUILayout.Space(Text.LineHeight / 2);
        }

        private void ResetBarSettings()
        {
            Settings.ColBarSettings = new SettingsColonistBar();
        }

        private void ResetPSISettings()
        {
            Settings.PsiSettings = new SettingsPSI();
        }
    }
}