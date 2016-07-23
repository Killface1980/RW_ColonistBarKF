using System.Collections.Generic;
using CommunityCoreLibrary;
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

        public override float DoWindowContents(Rect inRect)
        {
            float curY = 0f;

            inRect.xMin += 15f;
            inRect.width -= 15f;

            Rect contentRect = inRect;
            contentRect.yMin += curY;

            var listing2 = new Listing_Standard(contentRect);

            FillPageMain(listing2, contentRect.width);
            curY += 650f;

            listing2.End();

            return curY;
        }

        private void FillPageMain(Listing_Standard listing, float columnwidth)
        {
            // forces the mod to update the values from the sliders. is deactivated by saving the values.
            Settings.reloadsettings = true;

            listing.ColumnWidth = columnwidth/2;

            if (listing.ButtonText("RW_ColonistBarKF.Settings.RevertSettings".Translate()))
            {
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
                Settings.MaxColonistBarWidth = Screen.width - 320f;
            }
            listing.ColumnWidth = columnwidth;
            listing.Gap();
            
        //  listing.CheckboxLabeled("RW_ColonistBarKF.Settings.CustomScale".Translate(), ref Settings.useCustomScale, null);
        //  if (Settings.useCustomScale)
        //      Settings.Scale = listing.Slider(Settings.Scale, 0.1f, 1f);
        //
        //
        //  listing.ColumnWidth = columnwidth;
        //  listing.Gap();

            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.BasicSize".Translate(), ref Settings.useCustomIconSize, null);

            if (Settings.useCustomIconSize)
            { 
                Settings.BaseSizeFloat = listing.Slider(Settings.BaseSizeFloat, 16f, 128f);
                Settings.BaseIconSize = Settings.BaseSizeFloat/2f - 4f;
            }
            else
            {
                Settings.BaseSizeFloat = 48f;
                Settings.BaseIconSize = 20f;
            }

            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MarginTop".Translate(), ref Settings.useCustomMarginTop, null);
            if (Settings.useCustomMarginTop)
                Settings.MarginTop = listing.Slider(Settings.MarginTop, 8f, 64f);
            else
            {
                Settings.MarginTop = 21f;
            }

            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.BaseSpacingHorizontal".Translate(), ref Settings.useCustomBaseSpacingHorizontal, null);
            if (Settings.useCustomBaseSpacingHorizontal)
                Settings.BaseSpacingHorizontal = listing.Slider(Settings.BaseSpacingHorizontal, 12f, 48f);
            else
            {
                Settings.BaseSpacingHorizontal = 24f;
            }

            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.BaseSpacingVertical".Translate(), ref Settings.useCustomBaseSpacingVertical, null);
            if (Settings.useCustomBaseSpacingVertical)
                Settings.BaseSpacingVertical = listing.Slider(Settings.BaseSpacingVertical, 16f, 64f);
            else
            {
                Settings.BaseSpacingVertical = 32f;
            }
            listing.Gap();

            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.PawnTextureCameraVerticalOffset".Translate(), ref Settings.useCustomPawnTextureCameraVerticalOffset, null);
            if (Settings.useCustomPawnTextureCameraVerticalOffset)
                Settings.PawnTextureCameraVerticalOffset = listing.Slider(Settings.PawnTextureCameraVerticalOffset, 0f, 1f);
            else
            {
                Settings.PawnTextureCameraVerticalOffset = 0.3f;
            }

            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.PawnTextureCameraZoom".Translate(), ref Settings.useCustomPawnTextureCameraZoom, null);
            if (Settings.useCustomPawnTextureCameraZoom)
                Settings.PawnTextureCameraZoom = listing.Slider(Settings.PawnTextureCameraZoom, 0.3f, 3f);
            else
            {
                Settings.PawnTextureCameraZoom = 1.28205f;
            }
            listing.Gap();

            listing.CheckboxLabeled("RW_ColonistBarKF.Settings.MaxColonistBarWidth".Translate(), ref Settings.useCustomMaxColonistBarWidth, null);
            if (Settings.useCustomMaxColonistBarWidth)
                Settings.MaxColonistBarWidth = listing.Slider(Settings.MaxColonistBarWidth, Screen.width / 6, Screen.width);
            else
            {
                Settings.MaxColonistBarWidth = Screen.width - 320f;
            }

            //   TextFieldNumeric(ref Settings.ColonistsPerRow,ref string "test", 0f,20f);


        }

        public override void ExposeData()
        {

            Scribe_Values.LookValue(ref Settings.useCustomMarginTop, "useCustomMarginTop", false, false);
            Scribe_Values.LookValue(ref Settings.useCustomBaseSpacingHorizontal, "useCustomBaseSpacingHorizontal", false, false);
            Scribe_Values.LookValue(ref Settings.useCustomBaseSpacingVertical, "useCustomBaseSpacingVertical", false, false);
            Scribe_Values.LookValue(ref Settings.useCustomIconSize, "useCustomIconSize", false, false);
            Scribe_Values.LookValue(ref Settings.useCustomPawnTextureCameraVerticalOffset, "useCustomPawnTextureCameraVerticalOffset", false, false);
            Scribe_Values.LookValue(ref Settings.useCustomPawnTextureCameraZoom, "useCustomPawnTextureCameraZoom", false, false);
            Scribe_Values.LookValue(ref Settings.useCustomMaxColonistBarWidth, "useCustomMaxColonistBarWidth", false, false);

            Scribe_Values.LookValue(ref Settings.MarginTop, "MarginTop", 21f, false);
            Scribe_Values.LookValue(ref Settings.BaseSpacingHorizontal, "BaseSpacingHorizontal", 24f, false);
            Scribe_Values.LookValue(ref Settings.BaseSpacingVertical, "BaseSpacingVertical", 32f, false);
            Scribe_Values.LookValue(ref Settings.BaseSizeFloat, "BaseSizeFloat", 48f, false);
            Scribe_Values.LookValue(ref Settings.BaseIconSize, "BaseIconSize", 20f, false);
            Scribe_Values.LookValue(ref Settings.PawnTextureCameraVerticalOffset, "PawnTextureCameraVerticalOffset", 0.3f, false);
            Scribe_Values.LookValue(ref Settings.PawnTextureCameraZoom, "PawnTextureCameraZoom", 1.28205f, false);
            Scribe_Values.LookValue(ref Settings.MaxColonistBarWidth, "MaxColonistBarWidth", Screen.width - 320f, false);


            Settings.reloadsettings = false;

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
            }
        }

        #endregion
    }
}