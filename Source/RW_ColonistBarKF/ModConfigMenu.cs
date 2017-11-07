using static ColonistBarKF.PSI.GameComponentPSI;

namespace ColonistBarKF
{
    using ColonistBarKF.Bar;
    using JetBrains.Annotations;
    using System;
    using System.Collections.Generic;
    using System.IO;

    using ColonistBarKF.PSI;

    using UnityEngine;
    using Verse;

    public class ColonistBarKfSettings : Window
    {
        private static readonly string Cbkfversion = "Colonist Bar KF 0.18.0";

        private static int iconLimit;

        [NotNull]
        private readonly GUIStyle darkGrayBgImage =
            new GUIStyle { normal = { background = Textures.GrayFond } };

        [NotNull]
        private readonly GUIStyle fondBoxes =
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
        private readonly GUIStyle fondImages =
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
        private readonly GUIStyle grayLines = new GUIStyle { normal = { background = Textures.GrayLines } };

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
                switch (Settings.barSettings.MoodBarPos)
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
                        Settings.barSettings.MoodBarPos = Position.Alignment.Left;
                        break;

                    case 1:
                        Settings.barSettings.MoodBarPos = Position.Alignment.Right;
                        break;

                    case 2:
                        Settings.barSettings.MoodBarPos = Position.Alignment.Top;
                        break;

                    case 3:
                        Settings.barSettings.MoodBarPos = Position.Alignment.Bottom;
                        break;

                    default:
                        Settings.barSettings.MoodBarPos = Position.Alignment.Right;
                        break;
                }

                this.moodBarPositionInt = value;
            }
        }

        private int PsiBarPositionInt
        {
            get
            {
                if (Settings.barSettings.ColBarPsiIconPos == Position.Alignment.Left)
                {
                    this.psiBarPositionInt = 0;
                }

                if (Settings.barSettings.ColBarPsiIconPos == Position.Alignment.Right)
                {
                    this.psiBarPositionInt = 1;
                }

                if (Settings.barSettings.ColBarPsiIconPos == Position.Alignment.Top)
                {
                    this.psiBarPositionInt = 2;
                }

                if (Settings.barSettings.ColBarPsiIconPos == Position.Alignment.Bottom)
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
                        Settings.barSettings.ColBarPsiIconPos = Position.Alignment.Left;
                        Settings.barSettings.IconOffsetX = 1f;
                        Settings.barSettings.IconOffsetY = 1f;
                        Settings.barSettings.IconsHorizontal = false;
                        break;

                    case 1:
                        Settings.barSettings.ColBarPsiIconPos = Position.Alignment.Right;
                        Settings.barSettings.IconOffsetX = -1f;
                        Settings.barSettings.IconOffsetY = 1f;
                        Settings.barSettings.IconsHorizontal = false;
                        break;

                    case 2:
                        Settings.barSettings.ColBarPsiIconPos = Position.Alignment.Top;
                        Settings.barSettings.IconOffsetX = -1f;
                        Settings.barSettings.IconOffsetY = 1f;
                        Settings.barSettings.IconsHorizontal = true;
                        break;

                    case 3:
                        Settings.barSettings.ColBarPsiIconPos = Position.Alignment.Bottom;
                        Settings.barSettings.IconOffsetX = -1;
                        Settings.barSettings.IconOffsetY = -1;
                        Settings.barSettings.IconsHorizontal = true;
                        break;

                    default:
                        Settings.barSettings.ColBarPsiIconPos = 0;

                        break;
                }

                this.psiBarPositionInt = value;
            }
        }

        private int PsiPositionInt
        {
            get
            {
                if (Settings.psiSettings.IconAlignment == 0)
                {
                    this.psiPositionInt = 0;
                }

                if (Settings.psiSettings.IconAlignment == 1)
                {
                    this.psiPositionInt = 1;
                }

                if (Settings.psiSettings.IconAlignment == 2)
                {
                    this.psiPositionInt = 2;
                }

                if (Settings.psiSettings.IconAlignment == 3)
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
                        Settings.psiSettings.IconAlignment = value;
                        Settings.psiSettings.IconMarginX = 1f;
                        Settings.psiSettings.IconMarginY = 1f;
                        Settings.psiSettings.IconOffsetX = 1f;
                        Settings.psiSettings.IconOffsetY = 1f;
                        Settings.psiSettings.IconsHorizontal = false;
                        Settings.psiSettings.IconsScreenScale = true;
                        Settings.psiSettings.IconsInColumn = 3;
                        Settings.psiSettings.IconSize = 1f;
                        Settings.psiSettings.IconOpacity = 0.5f;
                        Settings.psiSettings.IconOpacityCritical = 0.8f;
                        break;

                    case 1:
                        Settings.psiSettings.IconAlignment = value;
                        Settings.psiSettings.IconMarginX = -1f;
                        Settings.psiSettings.IconMarginY = 1f;
                        Settings.psiSettings.IconOffsetX = -1f;
                        Settings.psiSettings.IconOffsetY = 1f;
                        Settings.psiSettings.IconsHorizontal = false;
                        Settings.psiSettings.IconsScreenScale = true;
                        Settings.psiSettings.IconsInColumn = 3;
                        Settings.psiSettings.IconSize = 1f;
                        Settings.psiSettings.IconOpacity = 0.5f;
                        Settings.psiSettings.IconOpacityCritical = 0.8f;
                        break;

                    case 2:
                        Settings.psiSettings.IconAlignment = value;
                        Settings.psiSettings.IconMarginX = 1f;
                        Settings.psiSettings.IconMarginY = -1.63f;
                        Settings.psiSettings.IconOffsetX = -1f;
                        Settings.psiSettings.IconOffsetY = 1f;
                        Settings.psiSettings.IconsHorizontal = true;
                        Settings.psiSettings.IconsScreenScale = true;
                        Settings.psiSettings.IconsInColumn = 3;
                        Settings.psiSettings.IconSize = 1f;
                        Settings.psiSettings.IconOpacity = 0.5f;
                        Settings.psiSettings.IconOpacityCritical = 0.8f;
                        break;

                    case 3:
                        Settings.psiSettings.IconAlignment = value;
                        Settings.psiSettings.IconMarginX = 1.139534f;
                        Settings.psiSettings.IconMarginY = 1.375f;
                        Settings.psiSettings.IconOffsetX = -0.9534883f;
                        Settings.psiSettings.IconOffsetY = -0.9534884f;
                        Settings.psiSettings.IconsHorizontal = true;
                        Settings.psiSettings.IconsScreenScale = true;
                        Settings.psiSettings.IconsInColumn = 4;
                        Settings.psiSettings.IconSize = 1.084302f;
                        Settings.psiSettings.IconOpacity = 0.5f;
                        Settings.psiSettings.IconOpacityCritical = 0.8f;
                        break;

                    default:
                        Settings.psiSettings.IconAlignment = 0;

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
            if (Settings.psiSettings.ShowTargetPoint)
            {
                if (!Settings.psiSettings.UseColoredTarget)
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
            Settings.psiSettings.ShowTargetPoint = GUILayout.Toggle(
                Settings.psiSettings.ShowTargetPoint,
                "PSI.Settings.Visibility.TargetPoint".Translate());
            Settings.psiSettings.UseColoredTarget = GUILayout.Toggle(
                Settings.psiSettings.UseColoredTarget,
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

            Settings.barSettings.UseCustomIconSize = GUILayout.Toggle(
                Settings.barSettings.UseCustomIconSize,
                "CBKF.Settings.BasicSize".Translate() + Settings.barSettings.BaseIconSize.ToString("N0") + " px, "
                + ColonistBar_KF.BarHelperKf.cachedScale.ToStringPercent() + " %, "
                + (int)Settings.barSettings.BaseSpacingHorizontal + " x, " + (int)Settings.barSettings.BaseSpacingVertical + " y");

            if (Settings.barSettings.UseCustomIconSize)
            {
                GUILayout.Space(Text.LineHeight / 2);

                Settings.barSettings.BaseIconSize = GUILayout.HorizontalSlider(Settings.barSettings.BaseIconSize, 24f, 256f);

                Settings.barSettings.BaseSpacingHorizontal =
                    GUILayout.HorizontalSlider(Settings.barSettings.BaseSpacingHorizontal, 1f, 72f);
                Settings.barSettings.BaseSpacingVertical =
                    GUILayout.HorizontalSlider(Settings.barSettings.BaseSpacingVertical, 1f, 96f);
            }
            else
            {
                Settings.barSettings.BaseIconSize = 48f;
                Settings.barSettings.BaseSpacingHorizontal = 24f;
                Settings.barSettings.BaseSpacingVertical = 32f;
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);
            Settings.barSettings.UseCustomPawnTextureCameraOffsets = GUILayout.Toggle(
                Settings.barSettings.UseCustomPawnTextureCameraOffsets,
                "CBKF.Settings.PawnTextureCameraOffsets".Translate()
                + Settings.barSettings.PawnTextureCameraHorizontalOffset.ToString("N2") + " x, "
                + Settings.barSettings.PawnTextureCameraVerticalOffset.ToString("N2") + " y, "
                + Settings.barSettings.PawnTextureCameraZoom.ToString("N2") + " z");
            if (Settings.barSettings.UseCustomPawnTextureCameraOffsets)
            {
                GUILayout.Space(Text.LineHeight / 2);
                Settings.barSettings.PawnTextureCameraHorizontalOffset = GUILayout.HorizontalSlider(
                    Settings.barSettings.PawnTextureCameraHorizontalOffset,
                    0.7f,
                    -0.7f);
                Settings.barSettings.PawnTextureCameraVerticalOffset =
                    GUILayout.HorizontalSlider(Settings.barSettings.PawnTextureCameraVerticalOffset, 0f, 1f);
                Settings.barSettings.PawnTextureCameraZoom =
                    GUILayout.HorizontalSlider(Settings.barSettings.PawnTextureCameraZoom, 0.3f, 3f);
            }
            else
            {
                Settings.barSettings.PawnTextureCameraHorizontalOffset = 0f;
                Settings.barSettings.PawnTextureCameraVerticalOffset = 0.3f;
                Settings.barSettings.PawnTextureCameraZoom = 1.28205f;
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
            Settings.barSettings.UseGrouping = GUILayout.Toggle(
                Settings.barSettings.UseGrouping,
                "CBKF.Settings.UseGrouping".Translate());

            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);
            Settings.barSettings.UseGroupColors = GUILayout.Toggle(
                Settings.barSettings.UseGroupColors,
                "CBKF.Settings.UseGroupColors".Translate());

            GUILayout.EndVertical();
        }

        private void FillPageMain()
        {
            GUILayout.BeginVertical(this.fondBoxes);
            Settings.barSettings.UseCustomMarginTop = GUILayout.Toggle(
                Settings.barSettings.UseCustomMarginTop,
                "CBKF.Settings.ColonistBarOffset".Translate() + (int)Settings.barSettings.MarginTop + " y \n"
                + "CBKF.Settings.MaxColonistBarWidth".Translate() + ": "
                + ((float)UI.screenWidth - (int)Settings.barSettings.MarginHorizontal) + " px");

            if (Settings.barSettings.UseCustomMarginTop)
            {
                GUILayout.Space(Text.LineHeight / 2);
                Settings.barSettings.MarginTop =
                    GUILayout.HorizontalSlider(Settings.barSettings.MarginTop, 0f, (float)UI.screenHeight / 6);
                Settings.barSettings.MarginHorizontal = GUILayout.HorizontalSlider(
                    Settings.barSettings.MarginHorizontal,
                    (float)UI.screenWidth * 3 / 5,
                    0f);
            }
            else
            {
                Settings.barSettings.MarginTop = 21f;
                Settings.barSettings.MarginHorizontal = 520f;
            }

            // listing.Gap(3f);
            GUILayout.EndVertical();

            // listing.Gap(3f);
            GUILayout.BeginVertical(this.fondBoxes);
            Settings.barSettings.UseCustomRowCount = GUILayout.Toggle(
                Settings.barSettings.UseCustomRowCount,
                "PSI.Settings.Arrangement.ColonistsPerColumn".Translate()
                + (Settings.barSettings.UseCustomRowCount ? Settings.barSettings.MaxRowsCustom : 3));
            if (Settings.barSettings.UseCustomRowCount)
            {
                Settings.barSettings.MaxRowsCustom = (int)GUILayout.HorizontalSlider(Settings.barSettings.MaxRowsCustom, 1f, 5f);
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);

            Settings.barSettings.UseWeaponIcons = GUILayout.Toggle(
                Settings.barSettings.UseWeaponIcons,
                "CBKF.Settings.UseWeaponIcons".Translate());

            Settings.barSettings.UseGender = GUILayout.Toggle(
                Settings.barSettings.UseGender,
                "CBKF.Settings.useGender".Translate());

            Settings.barSettings.useZoomToMouse = GUILayout.Toggle(
                Settings.barSettings.useZoomToMouse,
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
            Settings.barSettings.UseNewMood = GUILayout.Toggle(
                Settings.barSettings.UseNewMood,
                "CBKF.Settings.UseNewMood".Translate());

            if (Settings.barSettings.UseNewMood)
            {
                Settings.barSettings.UseExternalMoodBar = GUILayout.Toggle(
                    Settings.barSettings.UseExternalMoodBar,
                    "CBKF.Settings.UseExternalMoodBar".Translate());

                if (Settings.barSettings.UseExternalMoodBar)
                {
                    GUILayout.BeginHorizontal();
                    this.MoodBarPositionInt = GUILayout.Toolbar(this.MoodBarPositionInt, this.positionStrings);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                Settings.barSettings.UseExternalMoodBar = false;
            }

            GUILayout.EndVertical();

            GUILayout.EndVertical();
        }

        private void FillPagePSIIconSet(Rect viewRect)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("PSI.Settings.IconSet".Translate() + Settings.psiSettings.IconSet))
            {
                FloatMenuOption fmoDefault = new FloatMenuOption(
                    "PSI.Settings.Preset.0".Translate(),
                    () =>
                        {
                            try
                            {
                                Settings.psiSettings.IconSet = "default";
                                Settings.psiSettings.UseColoredTarget = true;
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
                                Settings.psiSettings.IconSet = "original";
                                Settings.psiSettings.UseColoredTarget = false;
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
                GameComponentPSI.PSIMaterials[Icon.Target],
                GameComponentPSI.PSIMaterials[Icon.TargetHair],
                GameComponentPSI.PSIMaterials[Icon.TargetSkin],
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Draft".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Draft],
                ref Settings.barSettings.ShowDraft,
                ref Settings.psiSettings.ShowDraft,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Unarmed".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Unarmed],
                ref Settings.barSettings.ShowUnarmed,
                ref Settings.psiSettings.ShowUnarmed,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Idle".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Idle],
                ref Settings.barSettings.ShowIdle,
                ref Settings.psiSettings.ShowIdle,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Sad".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Sad],
                ref Settings.barSettings.ShowSad,
                ref Settings.psiSettings.ShowSad,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Aggressive".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Aggressive],
                ref Settings.barSettings.ShowAggressive,
                ref Settings.psiSettings.ShowAggressive,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Panic".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Panic],
                ref Settings.barSettings.ShowPanic,
                ref Settings.psiSettings.ShowPanic,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Dazed".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Dazed],
                ref Settings.barSettings.ShowDazed,
                ref Settings.psiSettings.ShowDazed,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Leave".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Leave],
                ref Settings.barSettings.ShowLeave,
                ref Settings.psiSettings.ShowLeave,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Hungry".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Hungry],
                ref Settings.barSettings.ShowHungry,
                ref Settings.psiSettings.ShowHungry,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Tired".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Tired],
                ref Settings.barSettings.ShowTired,
                ref Settings.psiSettings.ShowTired,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.TooCold".Translate(),
                GameComponentPSI.PSIMaterials[Icon.TooCold],
                ref Settings.barSettings.ShowTooCold,
                ref Settings.psiSettings.ShowTooCold,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.TooHot".Translate(),
                GameComponentPSI.PSIMaterials[Icon.TooHot],
                ref Settings.barSettings.ShowTooHot,
                ref Settings.psiSettings.ShowTooHot,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.ApparelHealth".Translate(),
                GameComponentPSI.PSIMaterials[Icon.ApparelHealth],
                ref Settings.barSettings.ShowApparelHealth,
                ref Settings.psiSettings.ShowApparelHealth,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Naked".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Naked],
                ref Settings.barSettings.ShowNaked,
                ref Settings.psiSettings.ShowNaked,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Health".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Health],
                ref Settings.barSettings.ShowHealth,
                ref Settings.psiSettings.ShowHealth,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.MedicalAttention".Translate(),
                GameComponentPSI.PSIMaterials[Icon.MedicalAttention],
                ref Settings.barSettings.ShowMedicalAttention,
                ref Settings.psiSettings.ShowMedicalAttention,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Injury".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Effectiveness],
                ref Settings.barSettings.ShowEffectiveness,
                ref Settings.psiSettings.ShowEffectiveness,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Bloodloss".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Bloodloss],
                ref Settings.barSettings.ShowBloodloss,
                ref Settings.psiSettings.ShowBloodloss,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Pain".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Pain],
                ref Settings.barSettings.ShowPain,
                ref Settings.psiSettings.ShowPain,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Drunk".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Drunk],
                ref Settings.barSettings.ShowDrunk,
                ref Settings.psiSettings.ShowDrunk,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Toxicity".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Toxicity],
                ref Settings.barSettings.ShowToxicity,
                ref Settings.psiSettings.ShowToxicity,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.NightOwl".Translate(),
                GameComponentPSI.PSIMaterials[Icon.NightOwl],
                ref Settings.barSettings.ShowNightOwl,
                ref Settings.psiSettings.ShowNightOwl,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.LeftUnburied".Translate(),
                GameComponentPSI.PSIMaterials[Icon.LeftUnburied],
                ref Settings.barSettings.ShowLeftUnburied,
                ref Settings.psiSettings.ShowLeftUnburied,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.CabinFever".Translate(),
                GameComponentPSI.PSIMaterials[Icon.CabinFever],
                ref Settings.barSettings.ShowCabinFever,
                ref Settings.psiSettings.ShowCabinFever,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Bedroom".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Bedroom],
                ref Settings.barSettings.ShowBedroom,
                ref Settings.psiSettings.ShowBedroom,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Greedy".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Greedy],
                ref Settings.barSettings.ShowGreedy,
                ref Settings.psiSettings.ShowGreedy,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Jealous".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Jealous],
                ref Settings.barSettings.ShowJealous,
                ref Settings.psiSettings.ShowJealous,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Pyromaniac".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Pyromaniac],
                ref Settings.barSettings.ShowPyromaniac,
                ref Settings.psiSettings.ShowPyromaniac,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Prosthophile".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Prosthophile],
                ref Settings.barSettings.ShowProsthophile,
                ref Settings.psiSettings.ShowProsthophile,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Prosthophobe".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Prosthophobe],
                ref Settings.barSettings.ShowProsthophobe,
                ref Settings.psiSettings.ShowProsthophobe,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Pacific".Translate(),
                GameComponentPSI.PSIMaterials[Icon.Pacific],
                ref Settings.barSettings.ShowPacific,
                ref Settings.psiSettings.ShowPacific,
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
                "PSI.Settings.IconOpacityAndColor.Opacity".Translate() + (Settings.psiSettings.IconOpacity * 100).ToString("N0")
                + " %");
            Settings.psiSettings.IconOpacity = GUILayout.HorizontalSlider(Settings.psiSettings.IconOpacity, 0.1f, 1f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);
            GUILayout.Label(
                "PSI.Settings.IconOpacityAndColor.OpacityCritical".Translate()
                + (Settings.psiSettings.IconOpacityCritical * 100).ToString("N0") + " %");
            Settings.psiSettings.IconOpacityCritical = GUILayout.HorizontalSlider(Settings.psiSettings.IconOpacityCritical, 0.1f, 1f);
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
                                                                        Settings.psiSettings.LimitBleedMult
                                                                            = 2f;
                                                                        Settings.psiSettings
                                                                                .LimitDiseaseLess =
                                                                            1f;
                                                                        Settings.psiSettings
                                                                                .LimitEfficiencyLess =
                                                                            0.6f;
                                                                        Settings.psiSettings.LimitFoodLess
                                                                            = 0.2f;

                                                                        // PsiSettings.LimitMoodLess = 0.2f;
                                                                        Settings.psiSettings.LimitRestLess
                                                                            = 0.2f;
                                                                        Settings.psiSettings
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
                                                                        Settings.psiSettings.LimitBleedMult
                                                                            = 3f;
                                                                        Settings.psiSettings
                                                                                .LimitDiseaseLess =
                                                                            1f;
                                                                        Settings.psiSettings
                                                                                .LimitEfficiencyLess =
                                                                            0.75f;
                                                                        Settings.psiSettings.LimitFoodLess
                                                                            = 0.25f;

                                                                        // PsiSettings.LimitMoodLess = 0.25f;
                                                                        Settings.psiSettings.LimitRestLess
                                                                            = 0.25f;
                                                                        Settings.psiSettings
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
                                                                        Settings.psiSettings.LimitBleedMult
                                                                            = 4f;
                                                                        Settings.psiSettings
                                                                                .LimitDiseaseLess =
                                                                            1f;
                                                                        Settings.psiSettings
                                                                                .LimitEfficiencyLess =
                                                                            0.9f;
                                                                        Settings.psiSettings.LimitFoodLess
                                                                            = 0.3f;

                                                                        // PsiSettings.LimitMoodLess = 0.3f;
                                                                        Settings.psiSettings.LimitRestLess
                                                                            = 0.3f;
                                                                        Settings.psiSettings
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
                + ("PSI.Settings.Sensitivity.Bleeding." + Math.Round(Settings.psiSettings.LimitBleedMult - 0.25)).Translate());
            Settings.psiSettings.LimitBleedMult = GUILayout.HorizontalSlider(Settings.psiSettings.LimitBleedMult, 0.5f, 5f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Injured".Translate() + (int)(Settings.psiSettings.LimitEfficiencyLess * 100.0) + " %");
            Settings.psiSettings.LimitEfficiencyLess = GUILayout.HorizontalSlider(Settings.psiSettings.LimitEfficiencyLess, 0.01f, 1f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Food".Translate() + (int)(Settings.psiSettings.LimitFoodLess * 100.0) + " %");
            Settings.psiSettings.LimitFoodLess = GUILayout.HorizontalSlider(Settings.psiSettings.LimitFoodLess, 0.01f, 0.99f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Rest".Translate() + (int)(Settings.psiSettings.LimitRestLess * 100.0) + " %");
            Settings.psiSettings.LimitRestLess = GUILayout.HorizontalSlider(Settings.psiSettings.LimitRestLess, 0.01f, 0.99f);
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
                "PSI.Settings.Sensitivity.Temperature".Translate() + (int)Settings.psiSettings.LimitTempComfortOffset + " °C");
            Settings.psiSettings.LimitTempComfortOffset =
                GUILayout.HorizontalSlider(Settings.psiSettings.LimitTempComfortOffset, -10f, 10f);
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
            Settings.barSettings.UsePsi = GUILayout.Toggle(Settings.barSettings.UsePsi, "CBKF.Settings.UsePsiOnBar".Translate());
            if (Settings.barSettings.UsePsi)
            {
                GUILayout.BeginHorizontal();
                this.PsiBarPositionInt = GUILayout.Toolbar(this.PsiBarPositionInt, this.positionStrings);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.Label("PSI.Settings.Arrangement.IconsPerColumn".Translate() + Settings.barSettings.IconsInColumn);
                Settings.barSettings.IconsInColumn = (int)GUILayout.HorizontalSlider(Settings.barSettings.IconsInColumn, 2f, 5f);
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);
            Settings.psiSettings.UsePsi = GUILayout.Toggle(Settings.psiSettings.UsePsi, "PSI.Settings.UsePSI".Translate());
            Settings.psiSettings.ShowRelationsOnStrangers = GUILayout.Toggle(
                Settings.psiSettings.ShowRelationsOnStrangers,
                "PSI.Settings.ShowRelationsOnStrangers".Translate());
            Settings.psiSettings.UsePsiOnPrisoner = GUILayout.Toggle(
                Settings.psiSettings.UsePsiOnPrisoner,
                "PSI.Settings.UsePSIOnPrisoner".Translate());
            Settings.psiSettings.UsePsiOnAnimals = GUILayout.Toggle(
                Settings.psiSettings.UsePsiOnAnimals,
                "PSI.Settings.UsePsiOnAnimals".Translate());

            if (Settings.psiSettings.UsePsi || Settings.psiSettings.UsePsiOnPrisoner)
            {
                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.BeginHorizontal();
                this.PsiPositionInt = GUILayout.Toolbar(this.PsiPositionInt, this.positionStrings);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(Text.LineHeight / 2);

                Settings.psiSettings.IconsHorizontal = GUILayout.Toggle(
                    Settings.psiSettings.IconsHorizontal,
                    "PSI.Settings.Arrangement.Horizontal".Translate());

                Settings.psiSettings.IconsScreenScale = GUILayout.Toggle(
                    Settings.psiSettings.IconsScreenScale,
                    "PSI.Settings.Arrangement.ScreenScale".Translate());

                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.Label("PSI.Settings.Arrangement.IconsPerColumn".Translate() + Settings.psiSettings.IconsInColumn);
                Settings.psiSettings.IconsInColumn = (int)GUILayout.HorizontalSlider(Settings.psiSettings.IconsInColumn, 1f, 7f);

                int num = (int)(Settings.psiSettings.IconSize * 4.5);

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
                Settings.psiSettings.IconSize = GUILayout.HorizontalSlider(Settings.psiSettings.IconSize, 0.5f, 2f);
                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.BeginVertical(this.fondBoxes);
                GUILayout.Label(
                    "PSI.Settings.Arrangement.IconPosition".Translate() + (int)(Settings.psiSettings.IconMarginX * 100.0)
                    + " x, " + (int)(Settings.psiSettings.IconMarginY * 100.0) + " y");
                Settings.psiSettings.IconMarginX = GUILayout.HorizontalSlider(Settings.psiSettings.IconMarginX, -2f, 2f);
                Settings.psiSettings.IconMarginY = GUILayout.HorizontalSlider(Settings.psiSettings.IconMarginY, -2f, 2f);
                GUILayout.EndVertical();

                GUILayout.BeginVertical(this.fondBoxes);
                GUILayout.Label(
                    "PSI.Settings.Arrangement.IconOffset".Translate() + (int)(Settings.psiSettings.IconOffsetX * 100.0) + " x, "
                    + (int)(Settings.psiSettings.IconOffsetY * 100.0) + " y");
                Settings.psiSettings.IconOffsetX = GUILayout.HorizontalSlider(Settings.psiSettings.IconOffsetX, -2f, 2f);
                Settings.psiSettings.IconOffsetY = GUILayout.HorizontalSlider(Settings.psiSettings.IconOffsetY, -2f, 2f);
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
            Settings.barSettings = new SettingsColonistBar();
        }

        private void ResetPSISettings()
        {
            Settings.psiSettings = new SettingsPSI();
        }
    }
}