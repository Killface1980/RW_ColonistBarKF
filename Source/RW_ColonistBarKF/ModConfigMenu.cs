using System;
using CommunityCoreLibrary;
using CommunityCoreLibrary.ColorPicker;
using CommunityCoreLibrary.UI;
using UnityEngine;
using Verse;
using static RW_ColonistBarKF.Settings;

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

            var listingreset = new Listing_Standard(rect);
            {
                FillPageReset(listingreset, rect.width - 15f);
                listingreset.Gap();
                listingreset.Label("RW_ColonistBarKF.Settings.BarPosition".Translate());
            }
            listingreset.End();

            var rect2a = new Rect(rect);
            rect2a.yMin = listingreset.CurHeight;

            var listingMain = new Listing_Standard(rect2a);
            {
                FillPageMain(listingMain, rect2a.width - 15f);
            }
            listingMain.End();

            var rect2 = new Rect(rect);
            rect2.yMin = listingreset.CurHeight + listingMain.CurHeight;

            var listing2 = new Listing_Standard(rect2);
            {
                FillPageOptions(listing2, rect2.width);
            }
            listing2.End();

            return 1000f;
        }
        private void FillPageReset(Listing_Standard listing, float columnwidth)
        {
            // forces the mod to update the values from the sliders. is deactivated by saving the values.
            reloadsettings = true;
            listing.ColumnWidth = columnwidth / 2;

            if (listing.ButtonText("RW_ColonistBarKF.Settings.RevertSettings".Translate()))
            {
                useGender = false;
                useCustomMarginTopHor = false;
                useCustomBaseSpacingHorizontal = false;
                useCustomBaseSpacingVertical = false;
                useCustomIconSize = false;
                useCustomPawnTextureCameraVerticalOffset = false;
                useCustomPawnTextureCameraZoom = false;
                useCustomMaxColonistBarWidth = false;
                useCustomMarginLeftHor = false;
                useCustomMarginRightHor = false;
                useBottomAlignment = false;
                useFixedIconScale = false;

                MarginTopBottomHor = 21f;
                BaseSpacingHorizontal = 24f;
                BaseSpacingVertical = 32f;
                BaseSizeFloat = 48f;
                PawnTextureCameraVerticalOffset = 0.3f;
                PawnTextureCameraZoom = 1.28205f;
                MaxColonistBarWidth = Screen.width - 320f;
                FemaleColor = new Color(1f, 0.64f, 0.8f, 1f);
                MaleColor = new Color(0.52f, 0.75f, 0.92f, 1f);
                femaleColorField.Value = new Color(1f, 0.64f, 0.8f, 1f);
                maleColorField.Value = new Color(0.52f, 0.75f, 0.92f, 1f);

                VerticalOffset = 0f;
                useCustomBaseSpacingVertical = false;
                useVerticalAlignment = false;
                useCustomMaxColonistBarHeight = false;
                BaseSpacingVertical = 32f;
                VerticalOffset = 0f;
                MaxColonistBarHeight = Screen.height - 240f;
                useRightAlignment = false;
                MarginLeftHor = 180f;
                MarginRightHor = 180f;
                useCustomDoubleClickTime = false;
                DoubleClickTime = 0.5f;
                useCustomMarginLeftRightVer = false;
                MarginLeftRightVer = 21f;
                useCustomMarginTopVer = false;
                MarginTopVer = 120f;
                useCustomMarginBottomVer = false;
                MarginBottomVer = 120f;
            }
            listing.ColumnWidth = columnwidth;
            listing.Gap();
        }

        private void FillPageMain(Listing_Standard listing, float columnwidth)
        {
            // forces the mod to update the values from the sliders. is deactivated by saving the values.
            reloadsettings = true;
            listing.ColumnWidth = columnwidth / 2;


            #region Alignment


            if (listing.ButtonText("RW_ColonistBarKF.Settings.useTop".Translate()))
            {
                useBottomAlignment = false;
                useVerticalAlignment = false;
                useRightAlignment = false;

            }
            if (listing.ButtonText("RW_ColonistBarKF.Settings.useBottom".Translate()))
            {
                useBottomAlignment = true;
                useVerticalAlignment = false;
                useRightAlignment = false;
            }
            listing.NewColumn();
            if (listing.ButtonText("RW_ColonistBarKF.Settings.useLeft".Translate()))
            {

                useBottomAlignment = false;
                useVerticalAlignment = true;
                useRightAlignment = false;
            }
            if (listing.ButtonText("RW_ColonistBarKF.Settings.useRight".Translate()))
            {

                useBottomAlignment = false;
                useVerticalAlignment = true;
                useRightAlignment = true;
            }

            listing.ColumnWidth = columnwidth;
            #endregion
        }

        private void FillPageOptions(Listing_Standard listing, float columnwidth)
        {
            // forces the mod to update the values from the sliders. is deactivated by saving the values.
            reloadsettings = true;

            listing.ColumnWidth = columnwidth;

            listing.Gap();

            #region Vertical Alignment

            if (useVerticalAlignment)
            {
                listing.Indent();

                listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MarginEdge".Translate(), ref useCustomMarginLeftRightVer, null);
                if (useCustomMarginLeftRightVer)
                {
                    listing.Gap(3f);
                    MarginLeftRightVer = listing.Slider(MarginLeftRightVer, 0f, Screen.width / 12);
                }
                else
                {
                    MarginLeftRightVer = 21f;
                }
                listing.Gap(3f);

                listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MarginTop".Translate(), ref useCustomMarginTopVer, null);
                if (useCustomMarginTopVer)
                {
                    listing.Gap(3f);
                    MarginTopVer = SliderMaxBarHeight(listing.GetRect(30f), MarginTopVer, 0f, Screen.height * 2 / 5);
                }
                else
                {
                    MarginTopVer = 120f;
                    MaxColonistBarHeight = Screen.height - MarginTopVer - MarginBottomVer;
                    VerticalOffset = MarginTopVer / 2 - MarginBottomVer / 2;

                }
                listing.Gap(3f);

                listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MarginBottom".Translate(), ref useCustomMarginBottomVer, null);
                if (useCustomMarginBottomVer)
                {
                    listing.Gap(3f);
                    MarginBottomVer = SliderMaxBarHeight(listing.GetRect(30f), MarginBottomVer, 0f, Screen.height * 2 / 5);
                }
                else
                {
                    MarginBottomVer = 120f;
                    MaxColonistBarHeight = Screen.height - MarginTopVer - MarginBottomVer;
                    VerticalOffset = MarginTopVer / 2 - MarginBottomVer / 2;
                }
                listing.Gap(3f);

                listing.Undent();
            }
            #endregion

            #region Horizontal alignment
            else
            {
                listing.Indent();

                listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MarginEdge".Translate(), ref useCustomMarginTopHor, null);
                if (useCustomMarginTopHor)
                {
                    listing.Gap(3f);
                    MarginTopBottomHor = listing.Slider(MarginTopBottomHor, 10, Screen.height / 12);
                }
                else
                {
                    MarginTopBottomHor = 21f;
                }
                listing.Gap(3f);


                listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MarginLeft".Translate(), ref useCustomMarginLeftHor, null);
                if (useCustomMarginLeftHor)
                {
                    listing.Gap(3f);
                    MarginLeftHor = SliderMaxBarWidth(listing.GetRect(30f), MarginLeftHor, 0f, Screen.width * 2 / 5);
                }
                else
                {
                    MarginLeftHor = 160f;
                    MaxColonistBarWidth = Screen.width - MarginLeftHor - MarginRightHor;
                    HorizontalOffset = MarginLeftHor / 2 - MarginRightHor / 2;
                }
                listing.Gap(3f);

                listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MarginRight".Translate(), ref useCustomMarginRightHor, null);
                if (useCustomMarginRightHor)
                {
                    listing.Gap(3f);
                    MarginRightHor = SliderMaxBarWidth(listing.GetRect(30f), MarginRightHor, 0f, Screen.width * 2 / 5);
                }
                else
                {
                    MarginRightHor = 160f;
                    MaxColonistBarWidth = Screen.width - MarginLeftHor - MarginRightHor;
                    HorizontalOffset = MarginLeftHor / 2 - MarginRightHor / 2;
                }
                listing.Gap(3f);
                listing.Undent();

            }
            #endregion

            listing.Gap();

            #region Size + Spacing

            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.BasicSize".Translate(), ref useCustomIconSize, null);

            if (useCustomIconSize)
            {
                listing.Gap(3f);
                listing.CheckboxLabeled("RW_ColonistBarKF.Settings.FixedScale".Translate(), ref useFixedIconScale, null);
                listing.Gap(3f);
                BaseSizeFloat = SliderBaseSize(listing.GetRect(30f), BaseSizeFloat, 16f, 128f);

                listing.Gap();
            }
            else
            {
                BaseSizeFloat = 48f;
                BaseIconSize = 20f;
                listing.Gap(3f);
            }


            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.BaseSpacingHorizontal".Translate(), ref useCustomBaseSpacingHorizontal, null);
            if (useCustomBaseSpacingHorizontal)
            {
                listing.Gap(3f);
                BaseSpacingHorizontal = listing.Slider(BaseSpacingHorizontal, 1f, 36f);
            }
            else
            {
                BaseSpacingHorizontal = 24f;
                listing.Gap(3f);
            }

            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.BaseSpacingVertical".Translate(), ref useCustomBaseSpacingVertical, null);
            if (useCustomBaseSpacingVertical)
            {
                listing.Gap(3f);
                BaseSpacingVertical = listing.Slider(BaseSpacingVertical, 1f, 48f);
            }
            else
            {
                BaseSpacingVertical = 32f;
            }

            #endregion

            listing.Gap();
            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.useGender".Translate(), ref useGender, null);

            #region Gender
            if (useGender)
            {
                listing.Gap(3f);
                float indent = 24f;
                DrawMCMRegion(new Rect(indent, listing.CurHeight, listing.ColumnWidth - indent, 64f));
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
            #endregion

            listing.Gap();

            #region Camera
            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.PawnTextureCameraZoom".Translate(), ref useCustomPawnTextureCameraZoom, null);
            if (useCustomPawnTextureCameraZoom)
            {
                listing.Gap(3f);
                PawnTextureCameraZoom = listing.Slider(PawnTextureCameraZoom, 0.3f, 3f);
            }
            else
            {
                PawnTextureCameraZoom = 1.28205f;
            }
            listing.Gap(3f);

            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.PawnTextureCameraVerticalOffset".Translate(), ref useCustomPawnTextureCameraVerticalOffset, null);
            if (useCustomPawnTextureCameraVerticalOffset)
            {
                listing.Gap(3f);
                PawnTextureCameraVerticalOffset = listing.Slider(PawnTextureCameraVerticalOffset, 0f, 1f);
            }
            else
            {
                PawnTextureCameraVerticalOffset = 0.3f;
            }
            #endregion

            listing.Gap();

            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.DoubleClickTime".Translate(), ref useCustomDoubleClickTime, null);
            if (useCustomDoubleClickTime)
            {
                listing.Gap(3f);
                DoubleClickTime = listing.Slider(DoubleClickTime, 0.1f, 1.5f);
            }
            else
            {
                DoubleClickTime = 0.5f;
            }

            //       listing.CheckboxLabeled("RW_ColonistBarKF.Settings.useExtraIcons".Translate(), ref Settings.useExtraIcons, null);
        }


        private void DrawMCMRegion(Rect InRect)
        {
            Rect row = InRect;
            row.height = 24f;

            femaleColorField.Draw(row);
            FemaleColor = femaleColorField.Value;

            row.y += 30f;

            maleColorField.Draw(row);
            MaleColor = maleColorField.Value;
        }

        public float SliderMaxBarWidth(Rect rect, float val, float min, float max)
        {
            GUI.skin.horizontalSlider.alignment = TextAnchor.MiddleCenter;
            float result = GUI.HorizontalSlider(rect, val, min, max);
            MaxColonistBarWidth = Screen.width - MarginLeftHor - MarginRightHor;
            HorizontalOffset = MarginLeftHor / 2 - MarginRightHor / 2;
            return result;
        }

        public float SliderMaxBarHeight(Rect rect, float val, float min, float max)
        {
            GUI.skin.horizontalSlider.alignment = TextAnchor.MiddleCenter;
            float result = GUI.HorizontalSlider(rect, val, min, max);
            MaxColonistBarHeight = Screen.height - MarginTopVer - MarginBottomVer;
            VerticalOffset = MarginTopVer / 2 - MarginBottomVer / 2;
            return result;
        }

        public float SliderBaseSize(Rect rect, float val, float min, float max)
        {
            GUI.skin.horizontalSlider.alignment = TextAnchor.MiddleCenter;
            float result = GUI.HorizontalSlider(rect, val, min, max);
            BaseIconSize = BaseSizeFloat / 2f - 4f;
            return result;
        }

        private LabeledInput_Color femaleColorField = new LabeledInput_Color(FemaleColor, "RW_ColonistBarKF.Settings.FemaleColor".Translate());
        private LabeledInput_Color maleColorField = new LabeledInput_Color(MaleColor, "RW_ColonistBarKF.Settings.MaleColor".Translate());

        public override void ExposeData()
        {
            Scribe_Values.LookValue(ref useCustomMarginTopHor, "useCustomMarginTopHor", false, false);
            Scribe_Values.LookValue(ref useCustomMarginLeftHor, "useCustomMarginLeftHor", false, false);
            Scribe_Values.LookValue(ref useCustomMarginRightHor, "useCustomMarginRightHor", false, false);

            Scribe_Values.LookValue(ref useCustomMarginTopVer, "useCustomMarginTopVer", false, false);
            Scribe_Values.LookValue(ref useCustomMarginLeftRightVer, "useCustomMarginLeftRightVer", false, false);
            Scribe_Values.LookValue(ref useCustomMarginBottomVer, "useCustomMarginBottomVer", false, false);


            Scribe_Values.LookValue(ref useCustomBaseSpacingHorizontal, "useCustomBaseSpacingHorizontal", false, false);
            Scribe_Values.LookValue(ref useCustomBaseSpacingVertical, "useCustomBaseSpacingVertical", false, false);
            Scribe_Values.LookValue(ref useCustomIconSize, "useCustomIconSize", false, false);
            Scribe_Values.LookValue(ref useFixedIconScale, "useFixedIconScale", false, false);
            Scribe_Values.LookValue(ref useCustomPawnTextureCameraVerticalOffset, "useCustomPawnTextureCameraVerticalOffset", false, false);
            Scribe_Values.LookValue(ref useCustomPawnTextureCameraZoom, "useCustomPawnTextureCameraZoom", false, false);
            Scribe_Values.LookValue(ref useCustomMaxColonistBarWidth, "useCustomMaxColonistBarWidth", false, false);
            Scribe_Values.LookValue(ref useCustomMaxColonistBarHeight, "useCustomMaxColonistBarHeight", false, false);
            Scribe_Values.LookValue(ref useCustomDoubleClickTime, "useCustomDoubleClick", false, false);
            Scribe_Values.LookValue(ref useGender, "useGender", false, false);
            Scribe_Values.LookValue(ref useVerticalAlignment, "useVerticalAlignment", false, false);
            Scribe_Values.LookValue(ref useRightAlignment, "useRightAlignment", false, false);
            Scribe_Values.LookValue(ref useBottomAlignment, "useBottomAlignment", false, false);

            Scribe_Values.LookValue(ref MarginLeftHor, "MarginLeftHor", 21f, false);
            Scribe_Values.LookValue(ref MarginTopBottomHor, "MarginTopBottomHor", 21f, false);
            Scribe_Values.LookValue(ref MarginRightHor, "MarginRightHor", 21f, false);

            Scribe_Values.LookValue(ref MarginTopVer, "MarginTopVer", 120f, false);
            Scribe_Values.LookValue(ref MarginLeftRightVer, "MarginLeftRightVer", 21f, false);
            Scribe_Values.LookValue(ref MarginBottomVer, "MarginBottomVer", 120f, false);

            Scribe_Values.LookValue(ref HorizontalOffset, "HorizontalOffset", 0f, false);
            Scribe_Values.LookValue(ref VerticalOffset, "MarginBottomVer", 0f, false);


            Scribe_Values.LookValue(ref BaseSpacingHorizontal, "BaseSpacingHorizontal", 24f, false);
            Scribe_Values.LookValue(ref BaseSpacingVertical, "BaseSpacingVertical", 32f, false);
            Scribe_Values.LookValue(ref BaseSizeFloat, "BaseSizeFloat", 48f, false);
            Scribe_Values.LookValue(ref BaseIconSize, "BaseIconSize", 20f, false);
            Scribe_Values.LookValue(ref PawnTextureCameraVerticalOffset, "PawnTextureCameraVerticalOffset", 0.3f, false);
            Scribe_Values.LookValue(ref PawnTextureCameraZoom, "PawnTextureCameraZoom", 1.28205f, false);
            Scribe_Values.LookValue(ref MaxColonistBarWidth, "MaxColonistBarWidth");


            Scribe_Values.LookValue(ref DoubleClickTime, "DoubleClickTime", 0.5f, false);

            Scribe_Values.LookValue(ref FemaleColor, "FemaleColor");
            Scribe_Values.LookValue(ref MaleColor, "MaleColor");


            reloadsettings = false;
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                femaleColorField.Value = FemaleColor;
                maleColorField.Value = MaleColor;
            }
        }

        #endregion

    }
}