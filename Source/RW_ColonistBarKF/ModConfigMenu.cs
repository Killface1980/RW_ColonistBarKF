using System;
using CommunityCoreLibrary;
using CommunityCoreLibrary.ColorPicker;
using CommunityCoreLibrary.UI;
using UnityEngine;
using Verse;

namespace RW_ColonistBarKF
{
    public class ModConfigMenu : ModConfigurationMenu
    {
        #region Fields

        public string Page = "main";
        public Window OptionsDialog;

        #endregion

        #region Methods

        public override float DoWindowContents(Rect rect)
        {
            rect.xMin += 15f;
            rect.width -= 15f;

            var listing = new Listing_Standard(rect);
            {
                FillPageMain(listing, rect.width);
            }
            listing.End();
            return 850f;
        }

        private void FillPageMain(Listing_Standard listing, float columnwidth)
        {
            // forces the mod to update the values from the sliders. is deactivated by saving the values.
            Settings.reloadsettings = true;
            listing.ColumnWidth = columnwidth / 2;

            if (listing.ButtonText("RW_ColonistBarKF.Settings.RevertSettings".Translate()))
            {
                Settings.useGender = false;
                Settings.useCustomMarginTop = false;
                Settings.useCustomBaseSpacingHorizontal = false;
                Settings.useCustomBaseSpacingVertical = false;
                Settings.useCustomIconSize = false;
                Settings.useCustomPawnTextureCameraVerticalOffset = false;
                Settings.useCustomPawnTextureCameraZoom = false;
                Settings.useCustomMaxColonistBarWidth = false;

                Settings.MarginTop = 21f;
                Settings.BaseSpacingHorizontal = 24f;
                Settings.BaseSpacingVertical = 32f;
                Settings.BaseSizeFloat = 48f;
                Settings.PawnTextureCameraVerticalOffset = 0.3f;
                Settings.PawnTextureCameraZoom = 1.28205f;
                Settings.MaxColonistBarWidth = Screen.width - 320f - Math.Abs(Settings.HorizontalOffset);
                Settings.FemaleColor = new Color(1f, 0.64f, 0.8f, 1f);
                Settings.MaleColor = new Color(0.52f, 0.75f, 0.92f, 1f);
                femaleColorField.Value = new Color(1f, 0.64f, 0.8f, 1f);
                maleColorField.Value = new Color(0.52f, 0.75f, 0.92f, 1f);

                Settings.VerticalOffset = 0f;
                Settings.MaxColonistBarHeight = Screen.height - 200f - Math.Abs(Settings.VerticalOffset);

                Settings.useCustomBaseSpacingVertical = false;
                Settings.useVerticalAlignment = false;
                Settings.useVerticalOffset = false;
                Settings.useCustomMaxColonistBarHeight = false;
                Settings.BaseSpacingVertical = 32f;
                Settings.MarginHorizontal = 21f;
                Settings.VerticalOffset = 0f;
                Settings.MaxColonistBarHeight = Screen.height - 200f;
                Settings.useRightAlignment= true;
            }

            listing.ColumnWidth = columnwidth;
            listing.Gap();
            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.useGender".Translate(), ref Settings.useGender, null);
            if (Settings.useGender)
            {
                listing.Gap(6f);
                float indent = 24f;
                DrawMCMRegion(new Rect(indent, listing.CurHeight, listing.ColumnWidth-indent, 64f));
                listing.Gap(72f);
                listing.ColumnWidth = columnwidth / 2;
                listing.Indent();
                if (listing.ButtonText("RW_ColonistBarKF.Settings.ResetColors".Translate()))
                {
                    femaleColorField.Value = new Color(1f, 0.64f, 0.8f, 1f);
                    maleColorField.Value = new Color(0.52f, 0.75f, 0.92f, 1f);
                }
                listing.Undent();
                listing.ColumnWidth = columnwidth;
                listing.Gap();
            }
            listing.Gap();

            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.BasicSize".Translate(), ref Settings.useCustomIconSize, null);

            if (Settings.useCustomIconSize)
            {
                listing.Gap(6f);
                Settings.BaseSizeFloat = listing.Slider(Settings.BaseSizeFloat, 16f, 128f);
                Settings.BaseIconSize = Settings.BaseSizeFloat / 2f - 4f;
            }
            else
            {
                Settings.BaseSizeFloat = 48f;
                Settings.BaseIconSize = 20f;
            }
            listing.Gap(6f);
            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.BaseSpacingHorizontal".Translate(), ref Settings.useCustomBaseSpacingHorizontal, null);
            if (Settings.useCustomBaseSpacingHorizontal)
            {
                listing.Gap(6f);
                Settings.BaseSpacingHorizontal = listing.Slider(Settings.BaseSpacingHorizontal, 1f, 36f);
            }
            else
            {
                Settings.BaseSpacingHorizontal = 24f;
            listing.Gap(6f);
            }

            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.BaseSpacingVertical".Translate(), ref Settings.useCustomBaseSpacingVertical, null);
            if (Settings.useCustomBaseSpacingVertical)
            {
                listing.Gap(6f);
                Settings.BaseSpacingVertical = listing.Slider(Settings.BaseSpacingVertical, 1f, 48f);
            }
            else
            {
                Settings.BaseSpacingVertical = 32f;
            }
            listing.Gap();
            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.useVerticalAlignment".Translate(), ref Settings.useVerticalAlignment, null);

            if (Settings.useVerticalAlignment)
            {
                listing.Indent();
                listing.CheckboxLabeled("RW_ColonistBarKF.Settings.useRightAlignment".Translate(), ref Settings.useRightAlignment, null);

                listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MarginHorizontal".Translate(), ref Settings.useCustomMarginHorizontal, null);
                if (Settings.useCustomMarginHorizontal)
                {
                    listing.Gap(6f);
                    Settings.MarginHorizontal = listing.Slider(Settings.MarginHorizontal, 0f, Screen.width / 6);
                }
                else
                {
                    Settings.MarginHorizontal = 21f;
                }

                listing.CheckboxLabeled("RW_ColonistBarKF.Settings.VerticalOffset".Translate(), ref Settings.useVerticalOffset, null);
                if (Settings.useVerticalOffset)
                {
                listing.Gap(6f);
                    Settings.VerticalOffset = listing.Slider(Settings.VerticalOffset, -Screen.height / 2, Screen.height / 2);
                }
                else
                {
                    Settings.VerticalOffset = 0f;
                }
                listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MaxColonistBarHeight".Translate(), ref Settings.useCustomMaxColonistBarHeight, null);
                if (Settings.useCustomMaxColonistBarHeight)
                {
                listing.Gap(6f);
                Settings.MaxColonistBarHeight = listing.Slider(Settings.MaxColonistBarHeight, Screen.height / 6, Screen.height);
                }
                else
                {
                    Settings.MaxColonistBarHeight = Screen.height - 200f;
                }
                listing.Undent();
            }
            else
            {
                listing.Gap();
                listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MarginTop".Translate(), ref Settings.useCustomMarginTop, null);
                if (Settings.useCustomMarginTop)
                {
                    listing.Gap(6f);
                    Settings.MarginTop = listing.Slider(Settings.MarginTop, 10, Screen.height / 4);
                }
                else
                {
                    Settings.MarginTop = 21f;
                }
                listing.Gap(6f);

                listing.CheckboxLabeled("RW_ColonistBarKF.Settings.HorizontalOffset".Translate(), ref Settings.useHorizontalOffset, null);
                if (Settings.useHorizontalOffset)
                {
                    listing.Gap(6f);
                    Settings.HorizontalOffset = listing.Slider(Settings.HorizontalOffset, -Screen.width / 2, Screen.width / 2);
                }
                else
                {
                    Settings.HorizontalOffset = 0f;
                }

                listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MaxColonistBarWidth".Translate(), ref Settings.useCustomMaxColonistBarWidth, null);
                if (Settings.useCustomMaxColonistBarWidth)
                {
                    listing.Gap(6f);
                    Settings.MaxColonistBarWidth = listing.Slider(Settings.MaxColonistBarWidth, Screen.width / 6, Screen.width);
                }
                else
                {
                    Settings.MaxColonistBarWidth = Screen.width - 320f;
                }
            }

            listing.Gap();

            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.PawnTextureCameraZoom".Translate(), ref Settings.useCustomPawnTextureCameraZoom, null);
            if (Settings.useCustomPawnTextureCameraZoom)
            {
                listing.Gap(6f);
            Settings.PawnTextureCameraZoom = listing.Slider(Settings.PawnTextureCameraZoom, 0.3f, 3f);
            }
            else
            {
                Settings.PawnTextureCameraZoom = 1.28205f;
            }
            listing.Gap(6f);

            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.PawnTextureCameraVerticalOffset".Translate(), ref Settings.useCustomPawnTextureCameraVerticalOffset, null);
            if (Settings.useCustomPawnTextureCameraVerticalOffset)
            {               
            listing.Gap(6f);
                Settings.PawnTextureCameraVerticalOffset = listing.Slider(Settings.PawnTextureCameraVerticalOffset, 0f, 1f);
            }
            else
            {
                Settings.PawnTextureCameraVerticalOffset = 0.3f;
            }

            listing.Gap();

            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.DoubleClickTime".Translate(), ref Settings.useCustomDoubleClickTime, null);
            if (Settings.useCustomDoubleClickTime)
            {
                listing.Gap(6f);
                Settings.DoubleClickTime = listing.Slider(Settings.DoubleClickTime, 0.1f, 1.5f);
            }
            else
            {
                Settings.DoubleClickTime = 0.5f;
            }

            //       listing.CheckboxLabeled("RW_ColonistBarKF.Settings.useExtraIcons".Translate(), ref Settings.useExtraIcons, null);
        }

        private void DrawMCMRegion(Rect InRect)
        {
            Rect row = InRect;
            row.height = 24f;

            femaleColorField.Draw(row);
            Settings.FemaleColor = femaleColorField.Value;

            row.y += 30f;

            maleColorField.Draw(row);
            Settings.MaleColor = maleColorField.Value;
        }

        private LabeledInput_Color femaleColorField = new LabeledInput_Color(Settings.FemaleColor, "RW_ColonistBarKF.Settings.FemaleColor".Translate());
        private LabeledInput_Color maleColorField = new LabeledInput_Color(Settings.MaleColor, "RW_ColonistBarKF.Settings.MaleColor".Translate());

        public override void ExposeData()
        {
            Scribe_Values.LookValue(ref Settings.useCustomMarginTop, "useCustomMarginTop", false, false);
            Scribe_Values.LookValue(ref Settings.useCustomMarginHorizontal, "useCustomMarginHorizontal", false, false);
            Scribe_Values.LookValue(ref Settings.useCustomBaseSpacingHorizontal, "useCustomBaseSpacingHorizontal", false, false);
            Scribe_Values.LookValue(ref Settings.useCustomBaseSpacingVertical, "useCustomBaseSpacingVertical", false, false);
            Scribe_Values.LookValue(ref Settings.useCustomIconSize, "useCustomIconSize", false, false);
            Scribe_Values.LookValue(ref Settings.useCustomPawnTextureCameraVerticalOffset, "useCustomPawnTextureCameraVerticalOffset", false, false);
            Scribe_Values.LookValue(ref Settings.useCustomPawnTextureCameraZoom, "useCustomPawnTextureCameraZoom", false, false);
            Scribe_Values.LookValue(ref Settings.useCustomMaxColonistBarWidth, "useCustomMaxColonistBarWidth", false, false);
            Scribe_Values.LookValue(ref Settings.useCustomMaxColonistBarHeight, "useCustomMaxColonistBarHeight", false, false);
            Scribe_Values.LookValue(ref Settings.useCustomDoubleClickTime, "useCustomDoubleClick", false, false);
            Scribe_Values.LookValue(ref Settings.useGender, "useGender", false, false);
            Scribe_Values.LookValue(ref Settings.useVerticalAlignment, "useVerticalAlignment", false, false);
            Scribe_Values.LookValue(ref Settings.useRightAlignment, "useRightAlignment", false, false);
            Scribe_Values.LookValue(ref Settings.useVerticalOffset, "useVerticalOffset", false, false);
            Scribe_Values.LookValue(ref Settings.useHorizontalOffset, "useHorizontalOffset", false, false);

            Scribe_Values.LookValue(ref Settings.MarginTop, "MarginTop", 21f, false);
            Scribe_Values.LookValue(ref Settings.MarginHorizontal, "MarginHorizontal", 21f, false);
            Scribe_Values.LookValue(ref Settings.BaseSpacingHorizontal, "BaseSpacingHorizontal", 24f, false);
            Scribe_Values.LookValue(ref Settings.BaseSpacingVertical, "BaseSpacingVertical", 32f, false);
            Scribe_Values.LookValue(ref Settings.BaseSizeFloat, "BaseSizeFloat", 48f, false);
            Scribe_Values.LookValue(ref Settings.BaseIconSize, "BaseIconSize", 20f, false);
            Scribe_Values.LookValue(ref Settings.PawnTextureCameraVerticalOffset, "PawnTextureCameraVerticalOffset", 0.3f, false);
            Scribe_Values.LookValue(ref Settings.PawnTextureCameraZoom, "PawnTextureCameraZoom", 1.28205f, false);
            Scribe_Values.LookValue(ref Settings.MaxColonistBarWidth, "MaxColonistBarWidth");
            Scribe_Values.LookValue(ref Settings.VerticalOffset, "VerticalOffset");
            Scribe_Values.LookValue(ref Settings.HorizontalOffset, "HorizontalOffset");

            Scribe_Values.LookValue(ref Settings.DoubleClickTime, "DoubleClickTime", 0.5f, false);

            Scribe_Values.LookValue(ref Settings.FemaleColor, "FemaleColor");
            Scribe_Values.LookValue(ref Settings.MaleColor, "MaleColor");


            Settings.reloadsettings = false;
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                femaleColorField.Value = Settings.FemaleColor;
                maleColorField.Value = Settings.MaleColor;
            }
        }

        #endregion

    }
}