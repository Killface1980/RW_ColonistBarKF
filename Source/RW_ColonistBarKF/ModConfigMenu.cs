using CommunityCoreLibrary;
using CommunityCoreLibrary.UI;
using System;
using System.Collections.Generic;
using System.IO;
using RimWorld;
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

            Rect headerRect = inRect;
            Rect headerRect2 = inRect;

            var headerListing = new Listing_Standard(headerRect);

            //       DoHeading(listing, "Pawn State Icons", false);

            headerListing.ColumnWidth = inRect.width;
            //    headerListing.ColumnWidth = inRect.width / 2 - 10f;

            FillPageMain(headerListing);

            headerListing.End();

            curY += headerListing.CurHeight;


            headerRect2.yMin += curY;
            var listinghead = new Listing_Standard(headerRect2);

            listinghead.ColumnWidth = headerListing.ColumnWidth / 2 - 10f;
            FillPageMain(listinghead);

            listinghead.End();

            curY += listinghead.CurHeight;

            curY += 15f;

            Rect contentRect = inRect;
            contentRect.yMin += curY;

            var listing2 = new Listing_Standard(contentRect);

            FillPageShowHide(listing2, contentRect.width);
            curY += 27 * 30f;

            listing2.End();

            return curY;
        }

        public static void DoCheckbox(Rect rect, ref bool value, string labelKey, string tipKey)
        {
            GameFont font = Text.Font;
            TextAnchor anchor = Text.Anchor;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            string text = Translator.Translate(labelKey);
            Vector2 vector = new Vector2(rect.x, rect.y + (rect.height - 24f) / 2f);
            float x = Text.CalcSize(text).x;
            Rect rect2 = new Rect(rect.x + 24f + 4f, rect.y, x, rect.height);
            Widgets.Checkbox(vector, ref value, 24f, false);
            DoLabel(rect2, text, Translator.Translate(tipKey));
            Text.Anchor = anchor;
            Text.Font = font;
        }

        public static void DoLabel(Rect rect, string label, string tipText = "")
        {
            GameFont font = Text.Font;
            TextAnchor anchor = Text.Anchor;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            if (!GenText.NullOrEmpty(tipText))
            {
                Widgets.DrawHighlightIfMouseover(rect);
                if (Mouse.IsOver(rect))
                {
                    GUI.DrawTexture(rect, TexUI.HighlightTex);
                }
                TooltipHandler.TipRegion(rect, tipText);
            }
            Widgets.Label(rect, label);
            Text.Anchor = anchor;
            Text.Font = font;
        }

        private void DoHeading(Listing_Standard listing, string translatorKey, bool translate = true)
        {
            Text.Font = GameFont.Medium;
            listing.Label(translate ? translatorKey.Translate() : translatorKey);
            Text.Font = GameFont.Small;
        }

        private void FillPageMain(Listing_Standard listing)
        {

            listing.ColumnWidth = listing.ColumnWidth;
            Page = "showhide";
        }

        private void FillPageShowHide(Listing_Standard listing, float columnwidth)
        {

            listing.ColumnWidth = columnwidth;
            DoHeading(listing, "PSI.Settings.Visibility.Header");

            listing.Label("PSI.Settings.IconOpacityAndColor.Opacity".Translate());
            ColonistBarSettings.BaseSizeFloat = listing.Slider(ColonistBarSettings.BaseSizeFloat, 24f, 96f);
         //   TextFieldNumeric(ref ColonistBarSettings.ColonistsPerRow,ref string "test", 0f,20f);


        }

        public override void ExposeData()
        {

            Scribe_Values.LookValue(ref ColonistBarSettings.BaseSizeFloat, "BaseSizeFloat");

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
            }
        }

        #endregion
    }
}