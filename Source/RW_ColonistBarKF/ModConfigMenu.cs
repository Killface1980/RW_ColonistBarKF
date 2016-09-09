#if !NoCCL
using CommunityCoreLibrary;
using CommunityCoreLibrary.UI;
# endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;
using static ColonistBarKF.CBKF;
using static ColonistBarKF.PSI.PSI;
using static ColonistBarKF.BarSettings.SortByWhat;

namespace ColonistBarKF
{
#if !NoCCL
    public class ModConfigMenu : ModConfigurationMenu
#else
    public class ColonistBarKF_Settings : Window
#endif
    {
        #region Fields

        public string Page = "main";
        public Window OptionsDialog;

        #endregion

        #region Methods



        public override void PreOpen()
        {
            base.PreOpen();
        }




#if NoCCL
        public override void PreClose()
        {
            SaveSettings();
        }

        public override Vector2 InitialSize
        {
            get { return new Vector2(438f, 925f); }
        }

        public ColonistBarKF_Settings()
        {
            forcePause = true;
            doCloseX = true;
        }

        public override void DoWindowContents(Rect rect)
#else
        public override float DoWindowContents(Rect rect)
#endif
        {
            rect.xMin += 15f;
            rect.width -= 15f;
            rect.yMin += 10f;
            Listing_Standard listingreset = new Listing_Standard(rect);
            {
                FillPageHeader(listingreset, rect.width - 15f);
                listingreset.Gap(6f);
            }
            listingreset.End();

            var rect2c = new Rect(rect);
            rect2c.yMin = listingreset.CurHeight;
            rect2c.height = Text.LineHeight;

            Widgets.Label(rect2c, "ColonistBarKF.BarSettings.BarPosition".Translate());
            var rect2a = new Rect(rect);
            rect2a.yMin = rect2c.yMax;

            var buttonRect = new Rect(rect2c);
            buttonRect.height = 30f;

            if (Widgets.ButtonText(buttonRect, "ColonistBarKF.BarSettings.Page".Translate()))
            {
                List<FloatMenuOption> floatOptionList = new List<FloatMenuOption>();

                floatOptionList.Add(new FloatMenuOption("ColonistBarKF.BarSettings.PagePosition".Translate(), delegate
                {
                    Page = "main";
                    SaveSettings();
                    SavePsiSettings();
                    LoadBarSettings();
                    LoadPsiSettings();
                }));
                floatOptionList.Add(new FloatMenuOption("ColonistBarKF.BarSettings.PageOptions".Translate(), delegate
                {
                    Page = "options";
                    SaveSettings();
                    SavePsiSettings();
                    LoadBarSettings();
                    LoadPsiSettings();
                }));
                floatOptionList.Add(new FloatMenuOption("ColonistBarKF.BarSettings.psiMain".Translate(), delegate
                {
                    Page = "psiMain";
                    SaveSettings();
                    SavePsiSettings();
                    LoadBarSettings();
                    LoadPsiSettings();
                }));
                floatOptionList.Add(new FloatMenuOption("ColonistBarKF.BarSettings.psiShowHide".Translate(), delegate
                {
                    Page = "psiShowHide";
                    SaveSettings();
                    SavePsiSettings();
                    LoadBarSettings();
                    LoadPsiSettings();
                }));
                floatOptionList.Add(new FloatMenuOption("ColonistBarKF.BarSettings.psiOpacityAndColor".Translate(), delegate
                {
                    Page = "psiOpacityAndColor";
                    SaveSettings();
                    SavePsiSettings();
                    LoadBarSettings();
                    LoadPsiSettings();
                }));
                floatOptionList.Add(new FloatMenuOption("ColonistBarKF.BarSettings.psiArrange".Translate(), delegate
                {
                    Page = "psiArrange";
                    SaveSettings();
                    SavePsiSettings();
                    LoadBarSettings();
                    LoadPsiSettings();
                }));
                floatOptionList.Add(new FloatMenuOption("ColonistBarKF.BarSettings.psiLimits".Translate(), delegate
                {
                    Page = "psiLimits";
                    SaveSettings();
                    SavePsiSettings();
                    LoadBarSettings();
                    LoadPsiSettings();
                }));
                FloatMenu window = new FloatMenu(floatOptionList, "ColonistBarKF.BarSettings.PageSettings".Translate());
                Find.WindowStack.Add(window);

            }
            rect2a.yMin += buttonRect.height;

            var listingMain = new Listing_Standard(rect2a);
            switch (Page)
            {
                case "main":
                    {
                        FillPageMain(listingMain, rect2a.width - 15f);
                        listingMain.End();
                    }
                    break;
                case "options":
                    {
                        FillPageOptions(listingMain, rect2a.width);
                        listingMain.End();
                    }
                    break;
                case "psiMain":
                    {
                        FillPagePSIMain(listingMain);
                        listingMain.End();
                    }
                    break;
                case "psiShowHide":
                    {
                        FillPagePSIShowHide(listingMain);
                        listingMain.End();
                    }
                    break;
                case "psiOpacityAndColor":
                    {
                        FillPagePSIOpacityAndColor(listingMain);
                        listingMain.End();
                    }
                    break;
                case "psiArrange":
                    {
                        FillPSIPageArrangement(listingMain);
                        listingMain.End();
                    }
                    break;
                case "psiLimits":
                    {
                        FillPSIPageLimits(listingMain);
                        listingMain.End();
                    }
                    break;
                default:
                    FillPageMain(listingMain, rect2a.width - 15f);
                    break;
            }




            var rect2 = new Rect(rect);
            rect2.yMin = listingreset.CurHeight + listingMain.CurHeight + rect2c.height;


#if !NoCCL
            return 1000f;
#endif
        }

        private void FillPageHeader(Listing_Standard listing, float columnwidth)
        {
            // forces the mod to update the values from the sliders. is deactivated by saving the values.
            CBKF.BarSettings.Reloadsettings = true;
            listing.ColumnWidth = columnwidth / 2;

            if (listing.ButtonText("ColonistBarKF.BarSettings.RevertSettings".Translate()))
            {
                CBKF.BarSettings.UseGender = true;
                CBKF.BarSettings.UseCustomMarginTopHor = false;
                CBKF.BarSettings.UseCustomBaseSpacingHorizontal = false;
                CBKF.BarSettings.UseCustomBaseSpacingVertical = false;
                CBKF.BarSettings.UseCustomIconSize = false;
                CBKF.BarSettings.UseCustomPawnTextureCameraVerticalOffset = false;
                CBKF.BarSettings.UseCustomPawnTextureCameraZoom = false;
                CBKF.BarSettings.UseCustomMarginLeftHorTop = false;
                CBKF.BarSettings.UseCustomMarginRightHorTop = false;
                CBKF.BarSettings.UseCustomMarginLeftHorBottom = false;
                CBKF.BarSettings.UseCustomMarginRightHorBottom = false;
                CBKF.BarSettings.UseBottomAlignment = false;
                CBKF.BarSettings.UseMoodColors = false;
                CBKF.BarSettings.UseWeaponIcons = false;
                CBKF.BarSettings.UseFixedIconScale = false;

                CBKF.BarSettings.MarginBottomHor = 21f;
                CBKF.BarSettings.BaseSpacingHorizontal = 24f;
                CBKF.BarSettings.BaseSpacingVertical = 32f;
                CBKF.BarSettings.BaseSizeFloat = 48f;
                CBKF.BarSettings.PawnTextureCameraHorizontalOffset = 0f;
                CBKF.BarSettings.PawnTextureCameraVerticalOffset = 0.3f;
                CBKF.BarSettings.PawnTextureCameraZoom = 1.28205f;
                CBKF.BarSettings.MaxColonistBarWidth = Screen.width - 320f;
                CBKF.BarSettings.FemaleColor = new Color(1f, 0.64f, 0.8f, 1f);
                CBKF.BarSettings.MaleColor = new Color(0.52f, 0.75f, 0.92f, 1f);
#if !NoCCL
                femaleColorField.Value = new Color(1f, 0.64f, 0.8f, 1f);
                maleColorField.Value = new Color(0.52f, 0.75f, 0.92f, 1f);
#endif
                CBKF.BarSettings.HorizontalOffset = 0f;
                CBKF.BarSettings.VerticalOffset = 0f;
                CBKF.BarSettings.UseCustomBaseSpacingVertical = false;
                CBKF.BarSettings.UseVerticalAlignment = false;
                CBKF.BarSettings.BaseSpacingVertical = 32f;
                CBKF.BarSettings.MaxColonistBarHeight = Screen.height - 240f;
                CBKF.BarSettings.UseRightAlignment = false;
                CBKF.BarSettings.MarginLeftHorTop = 180f;
                CBKF.BarSettings.MarginRightHorTop = 180f;
                CBKF.BarSettings.MarginLeftHorBottom = 180f;
                CBKF.BarSettings.MarginRightHorBottom = 180f;
                CBKF.BarSettings.UseCustomDoubleClickTime = false;
                CBKF.BarSettings.DoubleClickTime = 0.5f;
                CBKF.BarSettings.UseCustomMarginLeftVer = false;
                CBKF.BarSettings.MarginLeftVer = 21f;
                CBKF.BarSettings.UseCustomMarginTopVerLeft = false;
                CBKF.BarSettings.MarginTopVerLeft = 120f;
                CBKF.BarSettings.UseCustomMarginBottomVerLeft = false;
                CBKF.BarSettings.MarginBottomVerLeft = 120f;

                CBKF.BarSettings.UseCustomMarginTopHor = false;
                CBKF.BarSettings.UseCustomMarginBottomHor = false;
                CBKF.BarSettings.UseCustomMarginLeftHorTop = false;
                CBKF.BarSettings.UseCustomMarginRightHorTop = false;

                CBKF.BarSettings.UseCustomMarginTopVerLeft = false;
                CBKF.BarSettings.UseCustomMarginTopVerRight = false;
                CBKF.BarSettings.UseCustomMarginLeftVer = false;
                CBKF.BarSettings.UseCustomMarginRightVer = false;
                CBKF.BarSettings.UseCustomMarginBottomVerLeft = false;
                CBKF.BarSettings.UseCustomMarginBottomVerRight = false;
                CBKF.BarSettings.SortBy = vanilla;
                CBKF.BarSettings.useZoomToMouse = false;
                CBKF.BarSettings.moodRectScale = 0.33f;
            }

            //    listing.NewColumn();
            //    if (listing.ButtonText("ColonistBarKF.BarSettings.SortBy".Translate()))
            //    {
            //        List<FloatMenuOption> floatOptionList = new List<FloatMenuOption>();
            //
            //        floatOptionList.Add(new FloatMenuOption("ColonistBarKF.BarSettings.Vanilla".Translate(), delegate
            //        {
            //            barSettings.SortBy = (int)vanilla;
            //            ((UIRootMap)Find.UIRoot).colonistBar.MarkColonistsListDirty();
            //        }));
            //        floatOptionList.Add(new FloatMenuOption("ColonistBarKF.BarSettings.ByName".Translate(), delegate
            //        {
            //            barSettings.SortBy = (int)byName;
            //            ((UIRootMap)Find.UIRoot).colonistBar.MarkColonistsListDirty();
            //        }));
            //
            //        floatOptionList.Add(new FloatMenuOption("ColonistBarKF.BarSettings.SexAge".Translate(), delegate
            //        {
            //            barSettings.SortBy = (int)sexage;
            //            ((UIRootMap)Find.UIRoot).colonistBar.MarkColonistsListDirty();
            //        }));
            //
            //        floatOptionList.Add(new FloatMenuOption("ColonistBarKF.BarSettings.Mood".Translate(), delegate
            //        {
            //            barSettings.SortBy = (int)mood;
            //            ((UIRootMap)Find.UIRoot).colonistBar.MarkColonistsListDirty();
            //        }));
            //        floatOptionList.Add(new FloatMenuOption("ColonistBarKF.BarSettings.Health".Translate(), delegate
            //        {
            //            barSettings.SortBy = (int)health;
            //            ((UIRootMap)Find.UIRoot).colonistBar.MarkColonistsListDirty();
            //        }));
            //
            //        floatOptionList.Add(new FloatMenuOption("ColonistBarKF.BarSettings.Weapons".Translate(), delegate
            //        {
            //            barSettings.SortBy = (int)weapons;
            //            ((UIRootMap)Find.UIRoot).colonistBar.MarkColonistsListDirty();
            //        }));
            //        floatOptionList.Add(new FloatMenuOption("ColonistBarKF.BarSettings.barSettings".Translate(), delegate
            //        {
            //            Find.WindowStack.Add(new ColonistBarKF_Settings());
            //        }));
            //        FloatMenu window = new FloatMenu(floatOptionList, "ColonistBarKF.BarSettings.SortingOptions".Translate());
            //        Find.WindowStack.Add(window);
            //
            //    }
            //
            //    listing.ColumnWidth = columnwidth;
            listing.Gap(6f);
        }


        private void FillPageMain(Listing_Standard listing, float columnwidth)
        {
            // forces the mod to update the values from the sliders. is deactivated by saving the values.
            CBKF.BarSettings.Reloadsettings = true;
            listing.ColumnWidth = columnwidth / 2;


            #region Alignment


            if (listing.ButtonText("ColonistBarKF.BarSettings.useTop".Translate()))
            {
                CBKF.BarSettings.UseBottomAlignment = false;
                CBKF.BarSettings.UseVerticalAlignment = false;
                CBKF.BarSettings.UseRightAlignment = false;

            }
            if (listing.ButtonText("ColonistBarKF.BarSettings.useBottom".Translate()))
            {
                CBKF.BarSettings.UseBottomAlignment = true;
                CBKF.BarSettings.UseVerticalAlignment = false;
                CBKF.BarSettings.UseRightAlignment = false;
            }
            listing.NewColumn();
            if (listing.ButtonText("ColonistBarKF.BarSettings.useLeft".Translate()))
            {

                CBKF.BarSettings.UseBottomAlignment = false;
                CBKF.BarSettings.UseVerticalAlignment = true;
                CBKF.BarSettings.UseRightAlignment = false;
            }
            if (listing.ButtonText("ColonistBarKF.BarSettings.useRight".Translate()))
            {

                CBKF.BarSettings.UseBottomAlignment = false;
                CBKF.BarSettings.UseVerticalAlignment = true;
                CBKF.BarSettings.UseRightAlignment = true;
            }

            listing.ColumnWidth = columnwidth;

            #endregion
        }

        private void FillPageOptions(Listing_Standard listing, float columnwidth)
        {
            // forces the mod to update the values from the sliders. is deactivated by saving the values.
            CBKF.BarSettings.Reloadsettings = true;

            listing.ColumnWidth = columnwidth;

            listing.Gap(6f);

            #region Vertical Alignment

            if (CBKF.BarSettings.UseVerticalAlignment)
            {
#if !NoCCL
                listing.Indent();
#endif
                if (CBKF.BarSettings.UseRightAlignment)
                {
                    listing.CheckboxLabeled("ColonistBarKF.BarSettings.MarginEdge".Translate(), ref CBKF.BarSettings.UseCustomMarginRightVer, null);
                    if (CBKF.BarSettings.UseCustomMarginRightVer)
                    {
                        //    listing.Gap(3f);
                        CBKF.BarSettings.MarginRightVer = listing.Slider(CBKF.BarSettings.MarginRightVer, 0f, Screen.width / 12);
                    }
                    else
                    {
                        CBKF.BarSettings.MarginRightVer = 21f;
                    }
                    // listing.Gap(3f);

                    listing.CheckboxLabeled("ColonistBarKF.BarSettings.MarginTop".Translate(), ref CBKF.BarSettings.UseCustomMarginTopVerRight, null);
                    if (CBKF.BarSettings.UseCustomMarginTopVerRight)
                    {
                        //    listing.Gap(3f);
                        CBKF.BarSettings.MarginTopVerRight = SliderMaxBarHeight(listing.GetRect(30f), CBKF.BarSettings.MarginTopVerRight, 0f, Screen.height * 2 / 5);
                    }
                    else
                    {
                        CBKF.BarSettings.MarginTopVerRight = 120f;
                        CBKF.BarSettings.MaxColonistBarHeight = Screen.height - CBKF.BarSettings.MarginTopVerRight - CBKF.BarSettings.MarginBottomVerRight;
                        CBKF.BarSettings.VerticalOffset = CBKF.BarSettings.MarginTopVerRight / 2 - CBKF.BarSettings.MarginBottomVerRight / 2;

                    }
                    //  listing.Gap(3f);
                    listing.CheckboxLabeled("ColonistBarKF.BarSettings.MarginBottom".Translate(), ref CBKF.BarSettings.UseCustomMarginBottomVerRight, null);
                    if (CBKF.BarSettings.UseCustomMarginBottomVerRight)
                    {
                        //     listing.Gap(3f);
                        CBKF.BarSettings.MarginBottomVerRight = SliderMaxBarHeight(listing.GetRect(30f), CBKF.BarSettings.MarginBottomVerRight, 0f, Screen.height * 2 / 5);
                    }
                    else
                    {
                        CBKF.BarSettings.MarginBottomVerRight = 120f;
                        CBKF.BarSettings.MaxColonistBarHeight = Screen.height - CBKF.BarSettings.MarginTopVerRight - CBKF.BarSettings.MarginBottomVerRight;
                        CBKF.BarSettings.VerticalOffset = CBKF.BarSettings.MarginTopVerRight / 2 - CBKF.BarSettings.MarginBottomVerRight / 2;
                    }
                }
                else
                {
                    listing.CheckboxLabeled("ColonistBarKF.BarSettings.MarginEdge".Translate(), ref CBKF.BarSettings.UseCustomMarginLeftVer, null);
                    if (CBKF.BarSettings.UseCustomMarginLeftVer)
                    {
                        //     listing.Gap(3f);
                        CBKF.BarSettings.MarginLeftVer = listing.Slider(CBKF.BarSettings.MarginLeftVer, 0f, Screen.width / 12);
                    }
                    else
                    {
                        CBKF.BarSettings.MarginLeftVer = 21f;
                    }
                    //   listing.Gap(3f);

                    listing.CheckboxLabeled("ColonistBarKF.BarSettings.MarginTop".Translate(), ref CBKF.BarSettings.UseCustomMarginTopVerLeft, null);
                    if (CBKF.BarSettings.UseCustomMarginTopVerLeft)
                    {
                        //    listing.Gap(3f);
                        CBKF.BarSettings.MarginTopVerLeft = SliderMaxBarHeight(listing.GetRect(30f), CBKF.BarSettings.MarginTopVerLeft, 0f, Screen.height * 2 / 5);
                    }
                    else
                    {
                        CBKF.BarSettings.MarginTopVerLeft = 120f;
                        CBKF.BarSettings.MaxColonistBarHeight = Screen.height - CBKF.BarSettings.MarginTopVerLeft - CBKF.BarSettings.MarginBottomVerLeft;
                        CBKF.BarSettings.VerticalOffset = CBKF.BarSettings.MarginTopVerLeft / 2 - CBKF.BarSettings.MarginBottomVerLeft / 2;

                    }
                    //   listing.Gap(3f);

                    listing.CheckboxLabeled("ColonistBarKF.BarSettings.MarginBottom".Translate(), ref CBKF.BarSettings.UseCustomMarginBottomVerLeft, null);
                    if (CBKF.BarSettings.UseCustomMarginBottomVerLeft)
                    {
                        //    listing.Gap(3f);
                        CBKF.BarSettings.MarginBottomVerLeft = SliderMaxBarHeight(listing.GetRect(30f), CBKF.BarSettings.MarginBottomVerLeft, 0f, Screen.height * 2 / 5);
                    }
                    else
                    {
                        CBKF.BarSettings.MarginBottomVerLeft = 120f;
                        CBKF.BarSettings.MaxColonistBarHeight = Screen.height - CBKF.BarSettings.MarginTopVerLeft - CBKF.BarSettings.MarginBottomVerLeft;
                        CBKF.BarSettings.VerticalOffset = CBKF.BarSettings.MarginTopVerLeft / 2 - CBKF.BarSettings.MarginBottomVerLeft / 2;
                    }

                }



                //  listing.Gap(3f);
#if !NoCCL
                listing.Undent();
#endif
            }
            #endregion

            #region Horizontal alignment
            else
            {
#if !NoCCL
                listing.Indent();
#endif

                if (CBKF.BarSettings.UseBottomAlignment)
                {
                    listing.CheckboxLabeled("ColonistBarKF.BarSettings.MarginEdge".Translate(), ref CBKF.BarSettings.UseCustomMarginBottomHor, null);
                    if (CBKF.BarSettings.UseCustomMarginBottomHor)
                    {
                        //    listing.Gap(3f);
                        CBKF.BarSettings.MarginBottomHor = listing.Slider(CBKF.BarSettings.MarginBottomHor, 10, Screen.height / 12);
                    }
                    else
                    {
                        CBKF.BarSettings.MarginBottomHor = 21f;
                    }
                    //   listing.Gap(3f);


                    listing.CheckboxLabeled("ColonistBarKF.BarSettings.MarginLeft".Translate(), ref CBKF.BarSettings.UseCustomMarginLeftHorBottom, null);
                    if (CBKF.BarSettings.UseCustomMarginLeftHorBottom)
                    {
                        //   listing.Gap(3f);
                        CBKF.BarSettings.MarginLeftHorBottom = SliderMaxBarWidth(listing.GetRect(30f), CBKF.BarSettings.MarginLeftHorBottom, 0f, Screen.width * 2 / 5);
                    }
                    else
                    {
                        CBKF.BarSettings.MarginLeftHorBottom = 160f;
                        CBKF.BarSettings.MaxColonistBarWidth = Screen.width - CBKF.BarSettings.MarginLeftHorBottom - CBKF.BarSettings.MarginRightHorBottom;
                        CBKF.BarSettings.HorizontalOffset = CBKF.BarSettings.MarginLeftHorBottom / 2 - CBKF.BarSettings.MarginRightHorBottom / 2;
                    }
                    //  listing.Gap(3f);

                    listing.CheckboxLabeled("ColonistBarKF.BarSettings.MarginRight".Translate(), ref CBKF.BarSettings.UseCustomMarginRightHorBottom, null);
                    if (CBKF.BarSettings.UseCustomMarginRightHorBottom)
                    {
                        //      listing.Gap(3f);
                        CBKF.BarSettings.MarginRightHorBottom = SliderMaxBarWidth(listing.GetRect(30f), CBKF.BarSettings.MarginRightHorBottom, 0f, Screen.width * 2 / 5);
                    }
                    else
                    {
                        CBKF.BarSettings.MarginRightHorBottom = 160f;
                        CBKF.BarSettings.MaxColonistBarWidth = Screen.width - CBKF.BarSettings.MarginLeftHorBottom - CBKF.BarSettings.MarginRightHorBottom;
                        CBKF.BarSettings.HorizontalOffset = CBKF.BarSettings.MarginLeftHorBottom / 2 - CBKF.BarSettings.MarginRightHorBottom / 2;
                    }
                    //    listing.Gap(3f);
                }
                else
                {
                    listing.CheckboxLabeled("ColonistBarKF.BarSettings.MarginEdge".Translate(), ref CBKF.BarSettings.UseCustomMarginTopHor, null);
                    if (CBKF.BarSettings.UseCustomMarginTopHor)
                    {
                        //    listing.Gap(3f);
                        CBKF.BarSettings.MarginTopHor = listing.Slider(CBKF.BarSettings.MarginTopHor, 10, Screen.height / 12);
                    }
                    else
                    {
                        CBKF.BarSettings.MarginTopHor = 21f;
                    }
                    //  listing.Gap(3f);


                    listing.CheckboxLabeled("ColonistBarKF.BarSettings.MarginLeft".Translate(), ref CBKF.BarSettings.UseCustomMarginLeftHorTop, null);
                    if (CBKF.BarSettings.UseCustomMarginLeftHorTop)
                    {
                        //    listing.Gap(3f);
                        CBKF.BarSettings.MarginLeftHorTop = SliderMaxBarWidth(listing.GetRect(30f), CBKF.BarSettings.MarginLeftHorTop, 0f, Screen.width * 2 / 5);
                    }
                    else
                    {
                        CBKF.BarSettings.MarginLeftHorTop = 160f;
                        CBKF.BarSettings.MaxColonistBarWidth = Screen.width - CBKF.BarSettings.MarginLeftHorTop - CBKF.BarSettings.MarginRightHorTop;
                        CBKF.BarSettings.HorizontalOffset = CBKF.BarSettings.MarginLeftHorTop / 2 - CBKF.BarSettings.MarginRightHorTop / 2;
                    }
                    //  listing.Gap(3f);

                    listing.CheckboxLabeled("ColonistBarKF.BarSettings.MarginRight".Translate(), ref CBKF.BarSettings.UseCustomMarginRightHorTop, null);
                    if (CBKF.BarSettings.UseCustomMarginRightHorTop)
                    {
                        //     listing.Gap(3f);
                        CBKF.BarSettings.MarginRightHorTop = SliderMaxBarWidth(listing.GetRect(30f), CBKF.BarSettings.MarginRightHorTop, 0f, Screen.width * 2 / 5);
                    }
                    else
                    {
                        CBKF.BarSettings.MarginRightHorTop = 160f;
                        CBKF.BarSettings.MaxColonistBarWidth = Screen.width - CBKF.BarSettings.MarginLeftHorTop - CBKF.BarSettings.MarginRightHorTop;
                        CBKF.BarSettings.HorizontalOffset = CBKF.BarSettings.MarginLeftHorTop / 2 - CBKF.BarSettings.MarginRightHorTop / 2;
                    }
                    //  listing.Gap(3f);
                }
#if !NoCCL
                listing.Undent();
#endif
            }
            #endregion
            listing.Gap(6f);

            Widgets.DrawLineHorizontal(0, listing.CurHeight, listing.ColumnWidth);

            listing.Gap(6f);

            #region Size + Spacing

            listing.CheckboxLabeled("ColonistBarKF.BarSettings.BasicSize".Translate(), ref CBKF.BarSettings.UseCustomIconSize, null);

            if (CBKF.BarSettings.UseCustomIconSize)
            {
                //     listing.Gap(3f);
                listing.CheckboxLabeled("ColonistBarKF.BarSettings.FixedScale".Translate(), ref CBKF.BarSettings.UseFixedIconScale, null);
                //    listing.Gap(3f);
                CBKF.BarSettings.BaseSizeFloat = SliderBaseSize(listing.GetRect(30f), CBKF.BarSettings.BaseSizeFloat, 16f, 128f);

                //     listing.Gap();
            }
            else
            {
                CBKF.BarSettings.BaseSizeFloat = 48f;
                CBKF.BarSettings.BaseIconSize = 20f;
                //     listing.Gap(3f);
            }


            listing.CheckboxLabeled("ColonistBarKF.BarSettings.BaseSpacingHorizontal".Translate(), ref CBKF.BarSettings.UseCustomBaseSpacingHorizontal, null);
            if (CBKF.BarSettings.UseCustomBaseSpacingHorizontal)
            {
                //      listing.Gap(3f);
                CBKF.BarSettings.BaseSpacingHorizontal = listing.Slider(CBKF.BarSettings.BaseSpacingHorizontal, 1f, 72f);
            }
            else
            {
                CBKF.BarSettings.BaseSpacingHorizontal = 24f;
                //      listing.Gap(3f);
            }

            listing.CheckboxLabeled("ColonistBarKF.BarSettings.BaseSpacingVertical".Translate(), ref CBKF.BarSettings.UseCustomBaseSpacingVertical, null);
            if (CBKF.BarSettings.UseCustomBaseSpacingVertical)
            {
                //      listing.Gap(3f);
                CBKF.BarSettings.BaseSpacingVertical = listing.Slider(CBKF.BarSettings.BaseSpacingVertical, 1f, 96f);
            }
            else
            {
                CBKF.BarSettings.BaseSpacingVertical = 32f;
            }

            #endregion

            listing.Gap(3f);
            listing.CheckboxLabeled("ColonistBarKF.BarSettings.UseMoodColors".Translate(), ref CBKF.BarSettings.UseMoodColors, null);
            if (CBKF.BarSettings.UseMoodColors)
            {
                //      listing.Gap(3f);
                CBKF.BarSettings.moodRectScale = listing.Slider(CBKF.BarSettings.moodRectScale, 0.33f, 1f);
            }
            listing.CheckboxLabeled("ColonistBarKF.BarSettings.UseWeaponIcons".Translate(), ref CBKF.BarSettings.UseWeaponIcons, null);

            listing.CheckboxLabeled("ColonistBarKF.BarSettings.useGender".Translate(), ref CBKF.BarSettings.UseGender, null);
            listing.CheckboxLabeled("ColonistBarKF.BarSettings.useZoomToMouse".Translate(), ref CBKF.BarSettings.useZoomToMouse, null);

            #region Gender



#if !NoCCL
            if (barSettings.UseGender)
            {
            //    listing.Gap(3f);
                float indent = 24f;
                DrawMCMRegion(new Rect(indent, listing.CurHeight, listing.ColumnWidth - indent, 64f));
                listing.Gap(72f);
                listing.ColumnWidth = columnwidth / 2;
                listing.Indent();
                if (listing.ButtonText("ColonistBarKF.BarSettings.ResetColors".Translate()))
                {
                    femaleColorField.Value = new Color(1f, 0.64f, 0.8f, 1f);
                    maleColorField.Value = new Color(0.52f, 0.75f, 0.92f, 1f);
                }
                listing.Undent();
                listing.ColumnWidth = columnwidth;
                listing.Gap();
            }
#endif
            #endregion

            listing.Gap();

            #region Camera
            listing.CheckboxLabeled("ColonistBarKF.BarSettings.PawnTextureCameraZoom".Translate(), ref CBKF.BarSettings.UseCustomPawnTextureCameraZoom, null);
            if (CBKF.BarSettings.UseCustomPawnTextureCameraZoom)
            {
                //    listing.Gap(3f);
                CBKF.BarSettings.PawnTextureCameraZoom = listing.Slider(CBKF.BarSettings.PawnTextureCameraZoom, 0.3f, 3f);
            }
            else
            {
                CBKF.BarSettings.PawnTextureCameraZoom = 1.28205f;
            }
            //    listing.Gap(3f);

            listing.CheckboxLabeled("ColonistBarKF.BarSettings.PawnTextureCameraHorizontalOffset".Translate(), ref CBKF.BarSettings.UseCustomPawnTextureCameraHorizontalOffset, null);
            if (CBKF.BarSettings.UseCustomPawnTextureCameraHorizontalOffset)
            {
                //        listing.Gap(3f);
                CBKF.BarSettings.PawnTextureCameraHorizontalOffset = listing.Slider(CBKF.BarSettings.PawnTextureCameraHorizontalOffset, 0.7f, -0.7f);
            }
            else
            {
                CBKF.BarSettings.PawnTextureCameraHorizontalOffset = 0f;
            }

            listing.CheckboxLabeled("ColonistBarKF.BarSettings.PawnTextureCameraVerticalOffset".Translate(), ref CBKF.BarSettings.UseCustomPawnTextureCameraVerticalOffset, null);
            if (CBKF.BarSettings.UseCustomPawnTextureCameraVerticalOffset)
            {
                //       listing.Gap(3f);
                CBKF.BarSettings.PawnTextureCameraVerticalOffset = listing.Slider(CBKF.BarSettings.PawnTextureCameraVerticalOffset, 0f, 1f);
            }
            else
            {
                CBKF.BarSettings.PawnTextureCameraVerticalOffset = 0.3f;
            }
            #endregion

            listing.Gap(6f);

            listing.CheckboxLabeled("ColonistBarKF.BarSettings.DoubleClickTime".Translate(), ref CBKF.BarSettings.UseCustomDoubleClickTime, null);
            if (CBKF.BarSettings.UseCustomDoubleClickTime)
            {
                //       listing.Gap(3f);
                CBKF.BarSettings.DoubleClickTime = listing.Slider(CBKF.BarSettings.DoubleClickTime, 0.1f, 1.5f);
            }
            else
            {
                CBKF.BarSettings.DoubleClickTime = 0.5f;
            }

            //       listing.CheckboxLabeled("ColonistBarKF.BarSettings.useExtraIcons".Translate(), ref BarSettings.useExtraIcons, null);
        }

#if !NoCCL
        private void DrawMCMRegion(Rect InRect)
        {
            Rect row = InRect;
            row.height = 24f;

            femaleColorField.Draw(row);
            barSettings.FemaleColor = femaleColorField.Value;

            row.y += 30f;

            maleColorField.Draw(row);
            barSettings.MaleColor = maleColorField.Value;
        }
#endif
        public float SliderMaxBarWidth(Rect rect, float val, float min, float max)
        {
            GUI.skin.horizontalSlider.alignment = TextAnchor.MiddleCenter;
            float result = GUI.HorizontalSlider(rect, val, min, max);
            if (CBKF.BarSettings.UseBottomAlignment)
            {
                CBKF.BarSettings.MaxColonistBarWidth = Screen.width - CBKF.BarSettings.MarginLeftHorBottom - CBKF.BarSettings.MarginRightHorBottom;
                CBKF.BarSettings.HorizontalOffset = CBKF.BarSettings.MarginLeftHorBottom / 2 - CBKF.BarSettings.MarginRightHorBottom / 2;

            }
            else
            {
                CBKF.BarSettings.MaxColonistBarWidth = Screen.width - CBKF.BarSettings.MarginLeftHorTop - CBKF.BarSettings.MarginRightHorTop;
                CBKF.BarSettings.HorizontalOffset = CBKF.BarSettings.MarginLeftHorTop / 2 - CBKF.BarSettings.MarginRightHorTop / 2;

            }
            return result;
        }

        public float SliderMaxBarHeight(Rect rect, float val, float min, float max)
        {
            GUI.skin.horizontalSlider.alignment = TextAnchor.MiddleCenter;
            float result = GUI.HorizontalSlider(rect, val, min, max);
            if (CBKF.BarSettings.UseRightAlignment)
            {
                CBKF.BarSettings.MaxColonistBarHeight = Screen.height - CBKF.BarSettings.MarginTopVerRight - CBKF.BarSettings.MarginBottomVerRight;
                CBKF.BarSettings.VerticalOffset = CBKF.BarSettings.MarginTopVerRight / 2 - CBKF.BarSettings.MarginBottomVerRight / 2;
            }
            else
            {
                CBKF.BarSettings.MaxColonistBarHeight = Screen.height - CBKF.BarSettings.MarginTopVerLeft - CBKF.BarSettings.MarginBottomVerLeft;
                CBKF.BarSettings.VerticalOffset = CBKF.BarSettings.MarginTopVerLeft / 2 - CBKF.BarSettings.MarginBottomVerLeft / 2;
            }
            return result;
        }

        public float SliderBaseSize(Rect rect, float val, float min, float max)
        {
            GUI.skin.horizontalSlider.alignment = TextAnchor.MiddleCenter;
            float result = GUI.HorizontalSlider(rect, val, min, max);
            CBKF.BarSettings.BaseIconSize = CBKF.BarSettings.BaseSizeFloat / 2f - 4f;
            return result;
        }
#if !NoCCL
        private LabeledInput_Color femaleColorField = new LabeledInput_Color(barSettings.FemaleColor, "ColonistBarKF.BarSettings.FemaleColor".Translate());
        private LabeledInput_Color maleColorField = new LabeledInput_Color(barSettings.MaleColor, "ColonistBarKF.BarSettings.MaleColor".Translate());
#endif

        private void FillPagePSIMain(Listing_Standard listing)
        {
            //  if (listing.ButtonText("PSI.BarSettings.IconSet".Translate() + PSI.BarSettings.IconSet))
            //   if (listing.ButtonTextLabeled("PSI.BarSettings.IconSet".Translate() , PSI.BarSettings.IconSet))
            //   {
            //       var options = new List<FloatMenuOption>();
            //       foreach (var str in PSI.IconSets)
            //       {
            //           var setname = str;
            //           options.Add(new FloatMenuOption(setname, () =>
            //           {
            //               PSI.BarSettings.IconSet = setname;
            //               PSI.Materials = new Materials(setname);
            //               PSI.Materials.ReloadTextures(true);
            //           }));
            //       }
            //       Find.WindowStack.Add(new FloatMenu(options));
            //   }
            //   listing.NewColumn();

            //    if (listing.ButtonTextLabeled("PSI.BarSettings.LoadPresetButton".Translate()))
            if (listing.ButtonTextLabeled("PSI.BarSettings.IconSet".Translate() + psiSettings.IconSet, "PSI.BarSettings.LoadPresetButton".Translate()))
            {
                var strArray = new string[0];
                var path = GenFilePaths.CoreModsFolderPath + "/RW_PawnStateIcons/Presets/Complete/";
                if (Directory.Exists(path))
                    strArray = Directory.GetFiles(path, "*.cfg");
                var options = new List<FloatMenuOption>();
                foreach (var str in strArray)
                {
                    var setname = str;
                    options.Add(new FloatMenuOption(Path.GetFileNameWithoutExtension(setname), () =>
                    {
                        try
                        {
                            CBKF.PsiSettings = XmlLoader.ItemFromXmlFile<PSISettings>(setname);
                            SavePsiSettings();
                            PSI.PSI.Reinit();
                        }
                        catch (IOException)
                        {
                            Log.Error("PSI.BarSettings.LoadPreset.UnableToLoad".Translate() + setname);
                        }
                    }));
                }
                Find.WindowStack.Add(new FloatMenu(options));
            }

        }

        private void FillPagePSIShowHide(Listing_Standard listing)
        {
            listing.Label("PSI.Settings.Visibility.Header");
            listing.CheckboxLabeled("PSI.Settings.Visibility.TargetPoint".Translate(), ref psiSettings.ShowTargetPoint);
            listing.CheckboxLabeled("PSI.Settings.Visibility.Aggressive".Translate(), ref psiSettings.ShowAggressive);
            listing.CheckboxLabeled("PSI.Settings.Visibility.Dazed".Translate(), ref psiSettings.ShowDazed);
            listing.CheckboxLabeled("PSI.Settings.Visibility.Leave".Translate(), ref psiSettings.ShowLeave);
            listing.CheckboxLabeled("PSI.Settings.Visibility.Draft".Translate(), ref psiSettings.ShowDraft);
            //
            listing.CheckboxLabeled("PSI.Settings.Visibility.Idle".Translate(), ref psiSettings.ShowIdle);
            listing.CheckboxLabeled("PSI.Settings.Visibility.Unarmed".Translate(), ref psiSettings.ShowUnarmed);
            listing.CheckboxLabeled("PSI.Settings.Visibility.Hungry".Translate(), ref psiSettings.ShowHungry);
            listing.CheckboxLabeled("PSI.Settings.Visibility.Sad".Translate(), ref psiSettings.ShowSad);
            listing.CheckboxLabeled("PSI.Settings.Visibility.Tired".Translate(), ref psiSettings.ShowTired);
            //
            listing.CheckboxLabeled("PSI.Settings.Visibility.Sickness".Translate(), ref psiSettings.ShowDisease);
            listing.CheckboxLabeled("PSI.Settings.Visibility.Pain".Translate(), ref psiSettings.ShowPain);
            listing.CheckboxLabeled("PSI.Settings.Visibility.Health".Translate(), ref psiSettings.ShowHealth);
            listing.CheckboxLabeled("PSI.Settings.Visibility.Injury".Translate(), ref psiSettings.ShowEffectiveness);
            listing.CheckboxLabeled("PSI.Settings.Visibility.Bloodloss".Translate(), ref psiSettings.ShowBloodloss);
            //
            listing.CheckboxLabeled("PSI.Settings.Visibility.Hot".Translate(), ref psiSettings.ShowHot);
            listing.CheckboxLabeled("PSI.Settings.Visibility.Cold".Translate(), ref psiSettings.ShowCold);
            listing.CheckboxLabeled("PSI.Settings.Visibility.Naked".Translate(), ref psiSettings.ShowNaked);
            listing.CheckboxLabeled("PSI.Settings.Visibility.Drunk".Translate(), ref psiSettings.ShowDrunk);
            listing.CheckboxLabeled("PSI.Settings.Visibility.ApparelHealth".Translate(), ref psiSettings.ShowApparelHealth);
            //
            listing.CheckboxLabeled("PSI.Settings.Visibility.Pacific".Translate(), ref psiSettings.ShowPacific);
            listing.CheckboxLabeled("PSI.Settings.Visibility.NightOwl".Translate(), ref psiSettings.ShowNightOwl);
            listing.CheckboxLabeled("PSI.Settings.Visibility.Greedy".Translate(), ref psiSettings.ShowGreedy);
            listing.CheckboxLabeled("PSI.Settings.Visibility.Jealous".Translate(), ref psiSettings.ShowJealous);
            listing.CheckboxLabeled("PSI.Settings.Visibility.Lovers".Translate(), ref psiSettings.ShowLovers);
            //
            listing.CheckboxLabeled("PSI.Settings.Visibility.Prosthophile".Translate(), ref psiSettings.ShowProsthophile);
            listing.CheckboxLabeled("PSI.Settings.Visibility.Prosthophobe".Translate(), ref psiSettings.ShowProsthophobe);
            listing.CheckboxLabeled("PSI.Settings.Visibility.RoomStatus".Translate(), ref psiSettings.ShowRoomStatus);
            listing.CheckboxLabeled("PSI.Settings.Visibility.Bedroom".Translate(), ref psiSettings.ShowBedroom);

            listing.CheckboxLabeled("PSI.Settings.Visibility.ShowDeadColonists".Translate(), ref psiSettings.ShowDeadColonists);

            listing.CheckboxLabeled("PSI.Settings.Visibility.Pyromaniac".Translate(), ref psiSettings.ShowPyromaniac);
        }

        private void FillPagePSIOpacityAndColor(Listing_Standard listing)
        {
            listing.Label("PSI.Settings.IconOpacityAndColor.Header");

            listing.Label("PSI.Settings.IconOpacityAndColor.Opacity".Translate());
            psiSettings.IconOpacity = listing.Slider(psiSettings.IconOpacity, 0.05f, 1f);

            listing.Label("PSI.Settings.IconOpacityAndColor.OpacityCritical".Translate());
            psiSettings.IconOpacityCritical = listing.Slider(psiSettings.IconOpacityCritical, 0f, 1f);

            listing.CheckboxLabeled("PSI.Settings.IconOpacityAndColor.UseColoredTarget".Translate(), ref psiSettings.UseColoredTarget);
            listing.Gap();


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


            //  if (listing.DoTextButton("PSI.Settings.ReturnButton".Translate()))
            //      Page = "main";
        }

        private void FillPSIPageArrangement(Listing_Standard listing)
        {

            listing.Label("PSI.Settings.Arrangement.Header");

            if (listing.ButtonText("PSI.Settings.LoadPresetButton".Translate()))
            {
                var strArray = new string[0];
                var path = GenFilePaths.CoreModsFolderPath + "/RW_PawnStateIcons/Presets/Position/";
                if (Directory.Exists(path))
                    strArray = Directory.GetFiles(path, "*.cfg");
                var options = new List<FloatMenuOption>();

                    options.Add(new FloatMenuOption("Left (Default)", () =>
                    {
                        try
                        {
                            psiSettings.IconDistanceX = 1f;
                            psiSettings.IconDistanceY = 1f;
                            psiSettings.IconOffsetX = 1f;
                            psiSettings.IconOffsetY = 1f;
                            psiSettings.IconsHorizontal = false;
                            psiSettings.IconsScreenScale = true;
                            psiSettings.IconsInColumn = 3;
                            psiSettings.IconSize = 1f;
                            psiSettings.IconOpacity = 0.7f;
                            psiSettings.IconOpacityCritical = 0.6f;
                        }
                        catch (IOException)
                        {
                            Log.Error("PSI.Settings.LoadPreset.UnableToLoad".Translate() + "Left");
                        }
                    }));
                options.Add(new FloatMenuOption("Right", () =>
                {
                    try
                    {
                        psiSettings.IconDistanceX = -1f;
                        psiSettings.IconDistanceY = 1f;
                        psiSettings.IconOffsetX = -1f;
                        psiSettings.IconOffsetY = 1f;
                        psiSettings.IconsHorizontal = false;
                        psiSettings.IconsScreenScale = true;
                        psiSettings.IconsInColumn = 3;
                        psiSettings.IconSize = 1f;
                        psiSettings.IconOpacity = 0.7f;
                        psiSettings.IconOpacityCritical = 0.6f;
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.Settings.LoadPreset.UnableToLoad".Translate() + "Right");
                    }
                }));

                options.Add(new FloatMenuOption("Top", () =>
                {
                    try
                    {
                        psiSettings.IconDistanceX = 1f;
                        psiSettings.IconDistanceY = -1.63f;
                        psiSettings.IconOffsetX = -1f;
                        psiSettings.IconOffsetY = 1f;
                        psiSettings.IconsHorizontal = true;
                        psiSettings.IconsScreenScale = true;
                        psiSettings.IconsInColumn = 3;
                        psiSettings.IconSize = 1f;
                        psiSettings.IconOpacity = 0.7f;
                        psiSettings.IconOpacityCritical = 0.6f;
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.Settings.LoadPreset.UnableToLoad".Translate() + "Top");
                    }
                }));

                options.Add(new FloatMenuOption("Bottom", () =>
                {
                    try
                    {
                        psiSettings.IconDistanceX = 1.139534f;
                        psiSettings.IconDistanceY = 1.375f;
                        psiSettings.IconOffsetX = -0.9534883f;
                        psiSettings.IconOffsetY = -0.9534884f;
                        psiSettings.IconsHorizontal = true;
                        psiSettings.IconsScreenScale = true;
                        psiSettings.IconsInColumn = 4;
                        psiSettings.IconSize = 1.084302f;
                        psiSettings.IconOpacity = 0.7f;
                        psiSettings.IconOpacityCritical = 0.6f;
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.Settings.LoadPreset.UnableToLoad".Translate() + "Bottom");
                    }
                }));

                Find.WindowStack.Add(new FloatMenu(options));
            }

            var num = (int)(psiSettings.IconSize * 4.5);

            if (num > 8)
                num = 8;
            else if (num < 0)
                num = 0;

            listing.Label("PSI.Settings.Arrangement.IconSize".Translate() + ("PSI.Settings.SizeLabel." + num).Translate());
            psiSettings.IconSize = listing.Slider(psiSettings.IconSize, 0.5f, 2f);

            listing.Label(string.Concat("PSI.Settings.Arrangement.IconPosition".Translate(), (int)(psiSettings.IconDistanceX * 100.0), " , ", (int)(psiSettings.IconDistanceY * 100.0)));
            psiSettings.IconDistanceX = listing.Slider(psiSettings.IconDistanceX, -2f, 2f);
            psiSettings.IconDistanceY = listing.Slider(psiSettings.IconDistanceY, -2f, 2f);

            listing.Label(string.Concat("PSI.Settings.Arrangement.IconOffset".Translate(), (int)(psiSettings.IconOffsetX * 100.0), " , ", (int)(psiSettings.IconOffsetY * 100.0)));
            psiSettings.IconOffsetX = listing.Slider(psiSettings.IconOffsetX, -2f, 2f);
            psiSettings.IconOffsetY = listing.Slider(psiSettings.IconOffsetY, -2f, 2f);

            listing.CheckboxLabeled("PSI.Settings.Arrangement.Horizontal".Translate(), ref psiSettings.IconsHorizontal);

            listing.CheckboxLabeled("PSI.Settings.Arrangement.ScreenScale".Translate(), ref psiSettings.IconsScreenScale);

            listing.Label("PSI.Settings.Arrangement.IconsPerColumn".Translate() + psiSettings.IconsInColumn);

            psiSettings.IconsInColumn = (int)listing.Slider(psiSettings.IconsInColumn, 1f, 7f);

            SavePsiSettings();
            PSI.PSI.Reinit();

            //   if (!listing.DoTextButton("PSI.Settings.ReturnButton".Translate()))
            //       return;
            //
            //   Page = "main";
        }

        private void FillPSIPageLimits(Listing_Standard listing)
        {

            listing.Label("PSI.Settings.Sensitivity.Header");
            if (listing.ButtonText("PSI.Settings.LoadPresetButton".Translate()))
            {
                var options = new List<FloatMenuOption>();
                    options.Add(new FloatMenuOption("Less Sensitive", () =>
                    {
                        try
                        {
                            psiSettings.LimitBleedMult = 2f;
                            psiSettings.LimitDiseaseLess = 1f;
                            psiSettings.LimitEfficiencyLess = 0.28f;
                            psiSettings.LimitFoodLess = 0.2f;
                            psiSettings.LimitMoodLess = 0.2f;
                            psiSettings.LimitRestLess = 0.2f;
                            psiSettings.LimitApparelHealthLess = 0.5f;
                            psiSettings.LimitTempComfortOffset = 3f;
                        }
                        catch (IOException)
                        {
                            Log.Error("PSI.Settings.LoadPreset.UnableToLoad".Translate() + "Less Sensitive");
                        }
                    }));
                options.Add(new FloatMenuOption("Standard", () =>
                {
                    try
                    {
                        psiSettings.LimitBleedMult = 3f;
                        psiSettings.LimitDiseaseLess = 1f;
                        psiSettings.LimitEfficiencyLess = 0.33f;
                        psiSettings.LimitFoodLess = 0.25f;
                        psiSettings.LimitMoodLess = 0.25f;
                        psiSettings.LimitRestLess = 0.25f;
                        psiSettings.LimitApparelHealthLess = 0.5f;
                        psiSettings.LimitTempComfortOffset = 0f;
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.Settings.LoadPreset.UnableToLoad".Translate() + "Standard");
                    }
                }));
                options.Add(new FloatMenuOption("More Sensitive", () =>
                {
                    try
                    {
                        psiSettings.LimitBleedMult = 4f;
                        psiSettings.LimitDiseaseLess = 1f;
                        psiSettings.LimitEfficiencyLess = 0.45f;
                        psiSettings.LimitFoodLess = 0.3f;
                        psiSettings.LimitMoodLess = 0.3f;
                        psiSettings.LimitRestLess = 0.3f;
                        psiSettings.LimitApparelHealthLess = 0.5f;
                        psiSettings.LimitTempComfortOffset = -3f;
                    }
                    catch (IOException)
                    {
                        Log.Error("PSI.Settings.LoadPreset.UnableToLoad".Translate() + "More Sensitive");
                    }
                }));

                Find.WindowStack.Add(new FloatMenu(options));
            }

            listing.Gap();

            listing.Label("PSI.Settings.Sensitivity.Bleeding".Translate() + ("PSI.Settings.Sensitivity.Bleeding." + Math.Round(psiSettings.LimitBleedMult - 0.25)).Translate());
            psiSettings.LimitBleedMult = listing.Slider(psiSettings.LimitBleedMult, 0.5f, 5f);

            listing.Label("PSI.Settings.Sensitivity.Injured".Translate() + (int)(psiSettings.LimitEfficiencyLess * 100.0) + "%");
            psiSettings.LimitEfficiencyLess = listing.Slider(psiSettings.LimitEfficiencyLess, 0.01f, 0.99f);

            listing.Label("PSI.Settings.Sensitivity.Food".Translate() + (int)(psiSettings.LimitFoodLess * 100.0) + "%");
            psiSettings.LimitFoodLess = listing.Slider(psiSettings.LimitFoodLess, 0.01f, 0.99f);

            listing.Label("PSI.Settings.Sensitivity.Mood".Translate() + (int)(psiSettings.LimitMoodLess * 100.0) + "%");
            psiSettings.LimitMoodLess = listing.Slider(psiSettings.LimitMoodLess, 0.01f, 0.99f);

            listing.Label("PSI.Settings.Sensitivity.Rest".Translate() + (int)(psiSettings.LimitRestLess * 100.0) + "%");
            psiSettings.LimitRestLess = listing.Slider(psiSettings.LimitRestLess, 0.01f, 0.99f);

            listing.Label("PSI.Settings.Sensitivity.ApparelHealth".Translate() + (int)(psiSettings.LimitApparelHealthLess * 100.0) + "%");
            psiSettings.LimitApparelHealthLess = listing.Slider(psiSettings.LimitApparelHealthLess, 0.01f, 0.99f);

            listing.Label("PSI.Settings.Sensitivity.Temperature".Translate() + (int)psiSettings.LimitTempComfortOffset + "C");
            psiSettings.LimitTempComfortOffset = listing.Slider(psiSettings.LimitTempComfortOffset, -10f, 10f);

            //  if (!listing.DoTextButton("PSI.Settings.ReturnButton".Translate()))
            //      return;
            //
            //  Page = "main";
        }


#if NoCCL
        public void ExposeData()
#else
        public override void ExposeData()
#endif
        {
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseCustomMarginTopHor, "useCustomMarginTopHor", false, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseCustomMarginBottomHor, "UseCustomMarginBottomHor", false, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseCustomMarginLeftHorTop, "useCustomMarginLeftHor", false, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseCustomMarginRightHorTop, "useCustomMarginRightHor", false, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseCustomMarginLeftHorBottom, "UseCustomMarginLeftHorBottom", false, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseCustomMarginRightHorBottom, "UseCustomMarginRightHorBottom", false, false);

            Scribe_Values.LookValue(ref CBKF.BarSettings.UseCustomMarginTopVerLeft, "UseCustomMarginTopVerLeft", false, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseCustomMarginTopVerRight, "UseCustomMarginTopVerRight", false, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseCustomMarginLeftVer, "useCustomMarginLeftRightVer", false, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseCustomMarginRightVer, "UseCustomMarginRightVer", false, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseCustomMarginBottomVerLeft, "UseCustomMarginBottomVerLeft", false, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseCustomMarginBottomVerRight, "UseCustomMarginBottomVerRight", false, false);


            Scribe_Values.LookValue(ref CBKF.BarSettings.UseCustomBaseSpacingHorizontal, "useCustomBaseSpacingHorizontal", false, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseCustomBaseSpacingVertical, "useCustomBaseSpacingVertical", false, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseCustomIconSize, "useCustomIconSize", false, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseFixedIconScale, "useFixedIconScale", false, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseCustomPawnTextureCameraHorizontalOffset, "UseCustomPawnTextureCameraHorizontalOffset", false, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseCustomPawnTextureCameraVerticalOffset, "useCustomPawnTextureCameraVerticalOffset", false, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseCustomPawnTextureCameraZoom, "useCustomPawnTextureCameraZoom", false, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseCustomDoubleClickTime, "useCustomDoubleClick", false, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseGender, "useGender", false, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseVerticalAlignment, "useVerticalAlignment", false, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseRightAlignment, "useRightAlignment", false, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseBottomAlignment, "useBottomAlignment", false, false);

            Scribe_Values.LookValue(ref CBKF.BarSettings.UseMoodColors, "UseMoodColors", false, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.UseWeaponIcons, "UseWeaponIcons", false, false);

            Scribe_Values.LookValue(ref CBKF.BarSettings.MarginTopHor, "MarginTopHor", 21f, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.MarginBottomHor, "MarginBottomHor", 21f, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.MarginLeftHorTop, "MarginLeftHorTop", 160f, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.MarginRightHorTop, "MarginRightHorTop", 160f, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.MarginLeftHorBottom, "MarginLeftHorBottom", 160f, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.MarginRightHorBottom, "MarginRightHorBottom", 160f, false);

            Scribe_Values.LookValue(ref CBKF.BarSettings.MarginTopVerLeft, "MarginTopVerLeft", 120f, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.MarginBottomVerLeft, "MarginBottomVerLeft", 120f, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.MarginTopVerRight, "MarginTopVerRight", 120f, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.MarginBottomVerRight, "MarginBottomVerRight", 120f, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.MarginLeftVer, "MarginLeftVer", 21f, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.MarginRightVer, "MarginRightVer", 21f, false);

            Scribe_Values.LookValue(ref CBKF.BarSettings.HorizontalOffset, "HorizontalOffset", 0f, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.VerticalOffset, "VerticalOffset", 0f, false);


            Scribe_Values.LookValue(ref CBKF.BarSettings.BaseSpacingHorizontal, "BaseSpacingHorizontal", 24f, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.BaseSpacingVertical, "BaseSpacingVertical", 32f, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.BaseSizeFloat, "BaseSizeFloat", 48f, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.BaseIconSize, "BaseIconSize", 20f, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.PawnTextureCameraHorizontalOffset, "PawnTextureCameraHorizontalOffset", 0f, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.PawnTextureCameraVerticalOffset, "PawnTextureCameraVerticalOffset", 0.3f, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.PawnTextureCameraZoom, "PawnTextureCameraZoom", 1.28205f, false);
            Scribe_Values.LookValue(ref CBKF.BarSettings.MaxColonistBarWidth, "MaxColonistBarWidth");
            Scribe_Values.LookValue(ref CBKF.BarSettings.MaxColonistBarHeight, "MaxColonistBarHeight");


            Scribe_Values.LookValue(ref CBKF.BarSettings.DoubleClickTime, "DoubleClickTime", 0.5f, false);

            Scribe_Values.LookValue(ref CBKF.BarSettings.FemaleColor, "FemaleColor");
            Scribe_Values.LookValue(ref CBKF.BarSettings.MaleColor, "MaleColor");

            Scribe_Values.LookValue(ref CBKF.BarSettings.MaxRows, "MaxRows");
            Scribe_Values.LookValue(ref CBKF.BarSettings.SortBy, "SortBy");
            Scribe_Values.LookValue(ref CBKF.BarSettings.useZoomToMouse, "useZoomToMouse");
            Scribe_Values.LookValue(ref CBKF.BarSettings.moodRectScale, "moodRectScale");


            CBKF.BarSettings.Reloadsettings = false;
#if !NoCCL
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                femaleColorField.Value = barSettings.FemaleColor;
                maleColorField.Value = barSettings.MaleColor;
            }
#endif
        }

        #endregion

    }
}
