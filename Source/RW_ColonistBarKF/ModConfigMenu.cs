using CommunityCoreLibrary;
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
            Reloadsettings = true;
            listing.ColumnWidth = columnwidth / 2;

            if (listing.ButtonText("RW_ColonistBarKF.Settings.RevertSettings".Translate()))
            {
                UseGender = false;
                UseCustomMarginTopHor = false;
                UseCustomBaseSpacingHorizontal = false;
                UseCustomBaseSpacingVertical = false;
                UseCustomIconSize = false;
                UseCustomPawnTextureCameraVerticalOffset = false;
                UseCustomPawnTextureCameraZoom = false;
                UseCustomMarginLeftHorTop = false;
                UseCustomMarginRightHorTop = false;
                UseCustomMarginLeftHorBottom = false;
                UseCustomMarginRightHorBottom = false;
                UseBottomAlignment = false;
                UseMoodColors = false;
                UseFixedIconScale = false;

                MarginBottomHor = 21f;
                BaseSpacingHorizontal = 24f;
                BaseSpacingVertical = 32f;
                BaseSizeFloat = 48f;
                PawnTextureCameraHorizontalOffset = 0f;
                PawnTextureCameraVerticalOffset = 0.3f;
                PawnTextureCameraZoom = 1.28205f;
                MaxColonistBarWidth = Screen.width - 320f;
                FemaleColor = new Color(1f, 0.64f, 0.8f, 1f);
                MaleColor = new Color(0.52f, 0.75f, 0.92f, 1f);
                femaleColorField.Value = new Color(1f, 0.64f, 0.8f, 1f);
                maleColorField.Value = new Color(0.52f, 0.75f, 0.92f, 1f);

                HorizontalOffset = 0f;
                VerticalOffset = 0f;
                UseCustomBaseSpacingVertical = false;
                UseVerticalAlignment = false;
                BaseSpacingVertical = 32f;
                MaxColonistBarHeight = Screen.height - 240f;
                UseRightAlignment = false;
                MarginLeftHorTop = 180f;
                MarginRightHorTop = 180f;
                MarginLeftHorBottom = 180f;
                MarginRightHorBottom = 180f;
                UseCustomDoubleClickTime = false;
                DoubleClickTime = 0.5f;
                UseCustomMarginLeftVer = false;
                MarginLeftVer = 21f;
                UseCustomMarginTopVerLeft = false;
                MarginTopVerLeft = 120f;
                UseCustomMarginBottomVerLeft = false;
                MarginBottomVerLeft = 120f;

                UseCustomMarginTopHor = false;
                UseCustomMarginBottomHor = false;
                UseCustomMarginLeftHorTop = false;
                UseCustomMarginRightHorTop = false;

                UseCustomMarginTopVerLeft = false;
                UseCustomMarginTopVerRight = false;
                UseCustomMarginLeftVer = false;
                UseCustomMarginRightVer = false;
                UseCustomMarginBottomVerLeft = false;
                UseCustomMarginBottomVerRight = false;
            }
            listing.ColumnWidth = columnwidth;
            listing.Gap();
        }

        private void FillPageMain(Listing_Standard listing, float columnwidth)
        {
            // forces the mod to update the values from the sliders. is deactivated by saving the values.
            Reloadsettings = true;
            listing.ColumnWidth = columnwidth / 2;


            #region Alignment


            if (listing.ButtonText("RW_ColonistBarKF.Settings.useTop".Translate()))
            {
                UseBottomAlignment = false;
                UseVerticalAlignment = false;
                UseRightAlignment = false;

            }
            if (listing.ButtonText("RW_ColonistBarKF.Settings.useBottom".Translate()))
            {
                UseBottomAlignment = true;
                UseVerticalAlignment = false;
                UseRightAlignment = false;
            }
            listing.NewColumn();
            if (listing.ButtonText("RW_ColonistBarKF.Settings.useLeft".Translate()))
            {

                UseBottomAlignment = false;
                UseVerticalAlignment = true;
                UseRightAlignment = false;
            }
            if (listing.ButtonText("RW_ColonistBarKF.Settings.useRight".Translate()))
            {

                UseBottomAlignment = false;
                UseVerticalAlignment = true;
                UseRightAlignment = true;
            }

            listing.ColumnWidth = columnwidth;
            #endregion
        }

        private void FillPageOptions(Listing_Standard listing, float columnwidth)
        {
            // forces the mod to update the values from the sliders. is deactivated by saving the values.
            Reloadsettings = true;

            listing.ColumnWidth = columnwidth;

            listing.Gap();

            #region Vertical Alignment

            if (UseVerticalAlignment)
            {
                listing.Indent();
                if (UseRightAlignment)
                {
                    listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MarginEdge".Translate(), ref UseCustomMarginRightVer, null);
                    if (UseCustomMarginRightVer)
                    {
                        listing.Gap(3f);
                        MarginRightVer = listing.Slider(MarginRightVer, 0f, Screen.width / 12);
                    }
                    else
                    {
                        MarginRightVer = 21f;
                    }
                    listing.Gap(3f);

                    listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MarginTop".Translate(), ref UseCustomMarginTopVerRight, null);
                    if (UseCustomMarginTopVerRight)
                    {
                        listing.Gap(3f);
                        MarginTopVerRight = SliderMaxBarHeight(listing.GetRect(30f), MarginTopVerRight, 0f, Screen.height * 2 / 5);
                    }
                    else
                    {
                        MarginTopVerRight = 120f;
                        MaxColonistBarHeight = Screen.height - MarginTopVerRight - MarginBottomVerRight;
                        VerticalOffset = MarginTopVerRight / 2 - MarginBottomVerRight / 2;

                    }
                    listing.Gap(3f);
                    listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MarginBottom".Translate(), ref UseCustomMarginBottomVerRight, null);
                    if (UseCustomMarginBottomVerRight)
                    {
                        listing.Gap(3f);
                        MarginBottomVerRight = SliderMaxBarHeight(listing.GetRect(30f), MarginBottomVerRight, 0f, Screen.height * 2 / 5);
                    }
                    else
                    {
                        MarginBottomVerRight = 120f;
                        MaxColonistBarHeight = Screen.height - MarginTopVerRight - MarginBottomVerRight;
                        VerticalOffset = MarginTopVerRight / 2 - MarginBottomVerRight / 2;
                    }
                }
                else
                {
                    listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MarginEdge".Translate(), ref UseCustomMarginLeftVer, null);
                    if (UseCustomMarginLeftVer)
                    {
                        listing.Gap(3f);
                        MarginLeftVer = listing.Slider(MarginLeftVer, 0f, Screen.width / 12);
                    }
                    else
                    {
                        MarginLeftVer = 21f;
                    }
                    listing.Gap(3f);

                    listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MarginTop".Translate(), ref UseCustomMarginTopVerLeft, null);
                    if (UseCustomMarginTopVerLeft)
                    {
                        listing.Gap(3f);
                        MarginTopVerLeft = SliderMaxBarHeight(listing.GetRect(30f), MarginTopVerLeft, 0f, Screen.height * 2 / 5);
                    }
                    else
                    {
                        MarginTopVerLeft = 120f;
                        MaxColonistBarHeight = Screen.height - MarginTopVerLeft - MarginBottomVerLeft;
                        VerticalOffset = MarginTopVerLeft / 2 - MarginBottomVerLeft / 2;

                    }
                    listing.Gap(3f);

                    listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MarginBottom".Translate(), ref UseCustomMarginBottomVerLeft, null);
                    if (UseCustomMarginBottomVerLeft)
                    {
                        listing.Gap(3f);
                        MarginBottomVerLeft = SliderMaxBarHeight(listing.GetRect(30f), MarginBottomVerLeft, 0f, Screen.height * 2 / 5);
                    }
                    else
                    {
                        MarginBottomVerLeft = 120f;
                        MaxColonistBarHeight = Screen.height - MarginTopVerLeft - MarginBottomVerLeft;
                        VerticalOffset = MarginTopVerLeft / 2 - MarginBottomVerLeft / 2;
                    }

                }



                listing.Gap(3f);

                listing.Undent();
            }
            #endregion

            #region Horizontal alignment
            else
            {
                listing.Indent();


                if (UseBottomAlignment)
                {
                    listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MarginEdge".Translate(), ref UseCustomMarginBottomHor, null);
                    if (UseCustomMarginBottomHor)
                    {
                        listing.Gap(3f);
                        MarginBottomHor = listing.Slider(MarginBottomHor, 10, Screen.height / 12);
                    }
                    else
                    {
                        MarginBottomHor = 21f;
                    }
                    listing.Gap(3f);


                    listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MarginLeft".Translate(), ref UseCustomMarginLeftHorBottom, null);
                    if (UseCustomMarginLeftHorBottom)
                    {
                        listing.Gap(3f);
                        MarginLeftHorBottom = SliderMaxBarWidth(listing.GetRect(30f), MarginLeftHorBottom, 0f, Screen.width * 2 / 5);
                    }
                    else
                    {
                        MarginLeftHorBottom = 160f;
                        MaxColonistBarWidth = Screen.width - MarginLeftHorBottom - MarginRightHorBottom;
                        HorizontalOffset = MarginLeftHorBottom / 2 - MarginRightHorBottom / 2;
                    }
                    listing.Gap(3f);

                    listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MarginRight".Translate(), ref UseCustomMarginRightHorBottom, null);
                    if (UseCustomMarginRightHorBottom)
                    {
                        listing.Gap(3f);
                        MarginRightHorBottom = SliderMaxBarWidth(listing.GetRect(30f), MarginRightHorBottom, 0f, Screen.width * 2 / 5);
                    }
                    else
                    {
                        MarginRightHorBottom = 160f;
                        MaxColonistBarWidth = Screen.width - MarginLeftHorBottom - MarginRightHorBottom;
                        HorizontalOffset = MarginLeftHorBottom / 2 - MarginRightHorBottom / 2;
                    }
                    listing.Gap(3f);
                }
                else
                {
                    listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MarginEdge".Translate(), ref UseCustomMarginTopHor, null);
                    if (UseCustomMarginTopHor)
                    {
                        listing.Gap(3f);
                        MarginTopHor = listing.Slider(MarginTopHor, 10, Screen.height / 12);
                    }
                    else
                    {
                        MarginTopHor = 21f;
                    }
                    listing.Gap(3f);


                    listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MarginLeft".Translate(), ref UseCustomMarginLeftHorTop, null);
                    if (UseCustomMarginLeftHorTop)
                    {
                        listing.Gap(3f);
                        MarginLeftHorTop = SliderMaxBarWidth(listing.GetRect(30f), MarginLeftHorTop, 0f, Screen.width * 2 / 5);
                    }
                    else
                    {
                        MarginLeftHorTop = 160f;
                        MaxColonistBarWidth = Screen.width - MarginLeftHorTop - MarginRightHorTop;
                        HorizontalOffset = MarginLeftHorTop / 2 - MarginRightHorTop / 2;
                    }
                    listing.Gap(3f);

                    listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MarginRight".Translate(), ref UseCustomMarginRightHorTop, null);
                    if (UseCustomMarginRightHorTop)
                    {
                        listing.Gap(3f);
                        MarginRightHorTop = SliderMaxBarWidth(listing.GetRect(30f), MarginRightHorTop, 0f, Screen.width * 2 / 5);
                    }
                    else
                    {
                        MarginRightHorTop = 160f;
                        MaxColonistBarWidth = Screen.width - MarginLeftHorTop - MarginRightHorTop;
                        HorizontalOffset = MarginLeftHorTop / 2 - MarginRightHorTop / 2;
                    }
                    listing.Gap(3f);
                }
                listing.Undent();

            }
            #endregion

            listing.Gap();

            #region Size + Spacing

            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.BasicSize".Translate(), ref UseCustomIconSize, null);

            if (UseCustomIconSize)
            {
                listing.Gap(3f);
                listing.CheckboxLabeled("RW_ColonistBarKF.Settings.FixedScale".Translate(), ref UseFixedIconScale, null);
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


            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.BaseSpacingHorizontal".Translate(), ref UseCustomBaseSpacingHorizontal, null);
            if (UseCustomBaseSpacingHorizontal)
            {
                listing.Gap(3f);
                BaseSpacingHorizontal = listing.Slider(BaseSpacingHorizontal, 1f, 72f);
            }
            else
            {
                BaseSpacingHorizontal = 24f;
                listing.Gap(3f);
            }

            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.BaseSpacingVertical".Translate(), ref UseCustomBaseSpacingVertical, null);
            if (UseCustomBaseSpacingVertical)
            {
                listing.Gap(3f);
                BaseSpacingVertical = listing.Slider(BaseSpacingVertical, 1f, 96f);
            }
            else
            {
                BaseSpacingVertical = 32f;
            }

            #endregion

            listing.Gap();
            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.UseMoodColors".Translate(), ref UseMoodColors, null);

            listing.Gap();
            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.useGender".Translate(), ref UseGender, null);

            #region Gender



            if (UseGender)
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
            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.PawnTextureCameraZoom".Translate(), ref UseCustomPawnTextureCameraZoom, null);
            if (UseCustomPawnTextureCameraZoom)
            {
                listing.Gap(3f);
                PawnTextureCameraZoom = listing.Slider(PawnTextureCameraZoom, 0.3f, 3f);
            }
            else
            {
                PawnTextureCameraZoom = 1.28205f;
            }
            listing.Gap(3f);

            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.PawnTextureCameraHorizontalOffset".Translate(), ref UseCustomPawnTextureCameraHorizontalOffset, null);
            if (UseCustomPawnTextureCameraHorizontalOffset)
            {
                listing.Gap(3f);
                PawnTextureCameraHorizontalOffset = listing.Slider(PawnTextureCameraHorizontalOffset, 0.7f, -0.7f);
            }
            else
            {
                PawnTextureCameraHorizontalOffset = 0f;
            }

            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.PawnTextureCameraVerticalOffset".Translate(), ref UseCustomPawnTextureCameraVerticalOffset, null);
            if (UseCustomPawnTextureCameraVerticalOffset)
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

            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.DoubleClickTime".Translate(), ref UseCustomDoubleClickTime, null);
            if (UseCustomDoubleClickTime)
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
            if (UseBottomAlignment)
            {
                MaxColonistBarWidth = Screen.width - MarginLeftHorBottom - MarginRightHorBottom;
                HorizontalOffset = MarginLeftHorBottom / 2 - MarginRightHorBottom / 2;

            }
            else
            {
                MaxColonistBarWidth = Screen.width - MarginLeftHorTop - MarginRightHorTop;
                HorizontalOffset = MarginLeftHorTop / 2 - MarginRightHorTop / 2;

            }
            return result;
        }

        public float SliderMaxBarHeight(Rect rect, float val, float min, float max)
        {
            GUI.skin.horizontalSlider.alignment = TextAnchor.MiddleCenter;
            float result = GUI.HorizontalSlider(rect, val, min, max);
            if (UseRightAlignment)
            {
                MaxColonistBarHeight = Screen.height - MarginTopVerRight - MarginBottomVerRight;
                VerticalOffset = MarginTopVerRight / 2 - MarginBottomVerRight / 2;
            }
            else
            {
                MaxColonistBarHeight = Screen.height - MarginTopVerLeft - MarginBottomVerLeft;
                VerticalOffset = MarginTopVerLeft / 2 - MarginBottomVerLeft / 2;
            }
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
            Scribe_Values.LookValue(ref UseCustomMarginTopHor, "useCustomMarginTopHor", false, false);
            Scribe_Values.LookValue(ref UseCustomMarginBottomHor, "UseCustomMarginBottomHor", false, false);
            Scribe_Values.LookValue(ref UseCustomMarginLeftHorTop, "useCustomMarginLeftHor", false, false);
            Scribe_Values.LookValue(ref UseCustomMarginRightHorTop, "useCustomMarginRightHor", false, false);
            Scribe_Values.LookValue(ref UseCustomMarginLeftHorBottom, "UseCustomMarginLeftHorBottom", false, false);
            Scribe_Values.LookValue(ref UseCustomMarginRightHorBottom, "UseCustomMarginRightHorBottom", false, false);

            Scribe_Values.LookValue(ref UseCustomMarginTopVerLeft, "UseCustomMarginTopVerLeft", false, false);
            Scribe_Values.LookValue(ref UseCustomMarginTopVerRight, "UseCustomMarginTopVerRight", false, false);
            Scribe_Values.LookValue(ref UseCustomMarginLeftVer, "useCustomMarginLeftRightVer", false, false);
            Scribe_Values.LookValue(ref UseCustomMarginRightVer, "UseCustomMarginRightVer", false, false);
            Scribe_Values.LookValue(ref UseCustomMarginBottomVerLeft, "UseCustomMarginBottomVerLeft", false, false);
            Scribe_Values.LookValue(ref UseCustomMarginBottomVerRight, "UseCustomMarginBottomVerRight", false, false);


            Scribe_Values.LookValue(ref UseCustomBaseSpacingHorizontal, "useCustomBaseSpacingHorizontal", false, false);
            Scribe_Values.LookValue(ref UseCustomBaseSpacingVertical, "useCustomBaseSpacingVertical", false, false);
            Scribe_Values.LookValue(ref UseCustomIconSize, "useCustomIconSize", false, false);
            Scribe_Values.LookValue(ref UseFixedIconScale, "useFixedIconScale", false, false);
            Scribe_Values.LookValue(ref UseCustomPawnTextureCameraHorizontalOffset, "UseCustomPawnTextureCameraHorizontalOffset", false, false);
            Scribe_Values.LookValue(ref UseCustomPawnTextureCameraVerticalOffset, "useCustomPawnTextureCameraVerticalOffset", false, false);
            Scribe_Values.LookValue(ref UseCustomPawnTextureCameraZoom, "useCustomPawnTextureCameraZoom", false, false);
            Scribe_Values.LookValue(ref UseCustomDoubleClickTime, "useCustomDoubleClick", false, false);
            Scribe_Values.LookValue(ref UseGender, "useGender", false, false);
            Scribe_Values.LookValue(ref UseVerticalAlignment, "useVerticalAlignment", false, false);
            Scribe_Values.LookValue(ref UseRightAlignment, "useRightAlignment", false, false);
            Scribe_Values.LookValue(ref UseBottomAlignment, "useBottomAlignment", false, false);

            Scribe_Values.LookValue(ref UseMoodColors, "UseMoodColors", false, false);

            Scribe_Values.LookValue(ref MarginTopHor, "MarginTopHor", 21f, false);
            Scribe_Values.LookValue(ref MarginBottomHor, "MarginBottomHor", 21f, false);
            Scribe_Values.LookValue(ref MarginLeftHorTop, "MarginLeftHorTop", 160f, false);
            Scribe_Values.LookValue(ref MarginRightHorTop, "MarginRightHorTop", 160f, false);
            Scribe_Values.LookValue(ref MarginLeftHorBottom, "MarginLeftHorBottom", 160f, false);
            Scribe_Values.LookValue(ref MarginRightHorBottom, "MarginRightHorBottom", 160f, false);

            Scribe_Values.LookValue(ref MarginTopVerLeft, "MarginTopVerLeft", 120f, false);
            Scribe_Values.LookValue(ref MarginBottomVerLeft, "MarginBottomVerLeft", 120f, false);
            Scribe_Values.LookValue(ref MarginTopVerRight, "MarginTopVerRight", 120f, false);
            Scribe_Values.LookValue(ref MarginBottomVerRight, "MarginBottomVerRight", 120f, false);
            Scribe_Values.LookValue(ref MarginLeftVer, "MarginLeftVer", 21f, false);
            Scribe_Values.LookValue(ref MarginRightVer, "MarginRightVer", 21f, false);

            Scribe_Values.LookValue(ref HorizontalOffset, "HorizontalOffset", 0f, false);
            Scribe_Values.LookValue(ref VerticalOffset, "VerticalOffset", 0f, false);


            Scribe_Values.LookValue(ref BaseSpacingHorizontal, "BaseSpacingHorizontal", 24f, false);
            Scribe_Values.LookValue(ref BaseSpacingVertical, "BaseSpacingVertical", 32f, false);
            Scribe_Values.LookValue(ref BaseSizeFloat, "BaseSizeFloat", 48f, false);
            Scribe_Values.LookValue(ref BaseIconSize, "BaseIconSize", 20f, false);
            Scribe_Values.LookValue(ref PawnTextureCameraHorizontalOffset, "PawnTextureCameraHorizontalOffset", 0f, false);
            Scribe_Values.LookValue(ref PawnTextureCameraVerticalOffset, "PawnTextureCameraVerticalOffset", 0.3f, false);
            Scribe_Values.LookValue(ref PawnTextureCameraZoom, "PawnTextureCameraZoom", 1.28205f, false);
            Scribe_Values.LookValue(ref MaxColonistBarWidth, "MaxColonistBarWidth");
            Scribe_Values.LookValue(ref MaxColonistBarHeight, "MaxColonistBarHeight");


            Scribe_Values.LookValue(ref DoubleClickTime, "DoubleClickTime", 0.5f, false);

            Scribe_Values.LookValue(ref FemaleColor, "FemaleColor");
            Scribe_Values.LookValue(ref MaleColor, "MaleColor");

            Scribe_Values.LookValue(ref MaxRows, "MaxRows");


            Reloadsettings = false;
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                femaleColorField.Value = FemaleColor;
                maleColorField.Value = MaleColor;
            }
        }

        #endregion

    }
}