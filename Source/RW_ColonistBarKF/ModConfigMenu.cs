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
using static RW_ColonistBarKF.CBKF;
using static RW_ColonistBarKF.ModSettings.SortByWhat;

namespace RW_ColonistBarKF
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
            get { return new Vector2(438f, 875f); }
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

            Widgets.Label(rect2c, "RW_ColonistBarKF.ModSettings.BarPosition".Translate());
            var rect2a = new Rect(rect);
            rect2a.yMin = rect2c.yMax;


            var listingMain = new Listing_Standard(rect2a);
            {
                FillPageMain(listingMain, rect2a.width - 15f);
            }
            listingMain.End();


            var rect2 = new Rect(rect);
            rect2.yMin = listingreset.CurHeight + listingMain.CurHeight + rect2c.height;

            var listing2 = new Listing_Standard(rect2);
            {
                FillPageOptions(listing2, rect2.width);
            }
            listing2.End();
#if !NoCCL
            return 1000f;
#endif
        }

        private void FillPageHeader(Listing_Standard listing, float columnwidth)
        {
            // forces the mod to update the values from the sliders. is deactivated by saving the values.
            Settings.Reloadsettings = true;
            listing.ColumnWidth = columnwidth / 2;

            if (listing.ButtonText("RW_ColonistBarKF.ModSettings.RevertSettings".Translate()))
            {
                Settings.UseGender = true;
                Settings.UseCustomMarginTopHor = false;
                Settings.UseCustomBaseSpacingHorizontal = false;
                Settings.UseCustomBaseSpacingVertical = false;
                Settings.UseCustomIconSize = false;
                Settings.UseCustomPawnTextureCameraVerticalOffset = false;
                Settings.UseCustomPawnTextureCameraZoom = false;
                Settings.UseCustomMarginLeftHorTop = false;
                Settings.UseCustomMarginRightHorTop = false;
                Settings.UseCustomMarginLeftHorBottom = false;
                Settings.UseCustomMarginRightHorBottom = false;
                Settings.UseBottomAlignment = false;
                Settings.UseMoodColors = false;
                Settings.UseWeaponIcons = false;
                Settings.UseFixedIconScale = false;

                Settings.MarginBottomHor = 21f;
                Settings.BaseSpacingHorizontal = 24f;
                Settings.BaseSpacingVertical = 32f;
                Settings.BaseSizeFloat = 48f;
                Settings.PawnTextureCameraHorizontalOffset = 0f;
                Settings.PawnTextureCameraVerticalOffset = 0.3f;
                Settings.PawnTextureCameraZoom = 1.28205f;
                Settings.MaxColonistBarWidth = Screen.width - 320f;
                Settings.FemaleColor = new Color(1f, 0.64f, 0.8f, 1f);
                Settings.MaleColor = new Color(0.52f, 0.75f, 0.92f, 1f);
#if !NoCCL
                femaleColorField.Value = new Color(1f, 0.64f, 0.8f, 1f);
                maleColorField.Value = new Color(0.52f, 0.75f, 0.92f, 1f);
#endif
                Settings.HorizontalOffset = 0f;
                Settings.VerticalOffset = 0f;
                Settings.UseCustomBaseSpacingVertical = false;
                Settings.UseVerticalAlignment = false;
                Settings.BaseSpacingVertical = 32f;
                Settings.MaxColonistBarHeight = Screen.height - 240f;
                Settings.UseRightAlignment = false;
                Settings.MarginLeftHorTop = 180f;
                Settings.MarginRightHorTop = 180f;
                Settings.MarginLeftHorBottom = 180f;
                Settings.MarginRightHorBottom = 180f;
                Settings.UseCustomDoubleClickTime = false;
                Settings.DoubleClickTime = 0.5f;
                Settings.UseCustomMarginLeftVer = false;
                Settings.MarginLeftVer = 21f;
                Settings.UseCustomMarginTopVerLeft = false;
                Settings.MarginTopVerLeft = 120f;
                Settings.UseCustomMarginBottomVerLeft = false;
                Settings.MarginBottomVerLeft = 120f;

                Settings.UseCustomMarginTopHor = false;
                Settings.UseCustomMarginBottomHor = false;
                Settings.UseCustomMarginLeftHorTop = false;
                Settings.UseCustomMarginRightHorTop = false;

                Settings.UseCustomMarginTopVerLeft = false;
                Settings.UseCustomMarginTopVerRight = false;
                Settings.UseCustomMarginLeftVer = false;
                Settings.UseCustomMarginRightVer = false;
                Settings.UseCustomMarginBottomVerLeft = false;
                Settings.UseCustomMarginBottomVerRight = false;
                Settings.SortBy = vanilla;
                Settings.useZoomToMouse = false;
            }

      //    listing.NewColumn();
      //    if (listing.ButtonText("RW_ColonistBarKF.ModSettings.SortBy".Translate()))
      //    {
      //        List<FloatMenuOption> floatOptionList = new List<FloatMenuOption>();
      //
      //        floatOptionList.Add(new FloatMenuOption("RW_ColonistBarKF.ModSettings.Vanilla".Translate(), delegate
      //        {
      //            Settings.SortBy = (int)vanilla;
      //            ((UIRootMap)Find.UIRoot).colonistBar.MarkColonistsListDirty();
      //        }));
      //        floatOptionList.Add(new FloatMenuOption("RW_ColonistBarKF.ModSettings.ByName".Translate(), delegate
      //        {
      //            Settings.SortBy = (int)byName;
      //            ((UIRootMap)Find.UIRoot).colonistBar.MarkColonistsListDirty();
      //        }));
      //
      //        floatOptionList.Add(new FloatMenuOption("RW_ColonistBarKF.ModSettings.SexAge".Translate(), delegate
      //        {
      //            Settings.SortBy = (int)sexage;
      //            ((UIRootMap)Find.UIRoot).colonistBar.MarkColonistsListDirty();
      //        }));
      //
      //        floatOptionList.Add(new FloatMenuOption("RW_ColonistBarKF.ModSettings.Mood".Translate(), delegate
      //        {
      //            Settings.SortBy = (int)mood;
      //            ((UIRootMap)Find.UIRoot).colonistBar.MarkColonistsListDirty();
      //        }));
      //        floatOptionList.Add(new FloatMenuOption("RW_ColonistBarKF.ModSettings.Health".Translate(), delegate
      //        {
      //            Settings.SortBy = (int)health;
      //            ((UIRootMap)Find.UIRoot).colonistBar.MarkColonistsListDirty();
      //        }));
      //
      //        floatOptionList.Add(new FloatMenuOption("RW_ColonistBarKF.ModSettings.Weapons".Translate(), delegate
      //        {
      //            Settings.SortBy = (int)weapons;
      //            ((UIRootMap)Find.UIRoot).colonistBar.MarkColonistsListDirty();
      //        }));
      //        floatOptionList.Add(new FloatMenuOption("RW_ColonistBarKF.ModSettings.Settings".Translate(), delegate
      //        {
      //            Find.WindowStack.Add(new ColonistBarKF_Settings());
      //        }));
      //        FloatMenu window = new FloatMenu(floatOptionList, "RW_ColonistBarKF.ModSettings.SortingOptions".Translate());
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
            Settings.Reloadsettings = true;
            listing.ColumnWidth = columnwidth / 2;


            #region Alignment


            if (listing.ButtonText("RW_ColonistBarKF.ModSettings.useTop".Translate()))
            {
                Settings.UseBottomAlignment = false;
                Settings.UseVerticalAlignment = false;
                Settings.UseRightAlignment = false;

            }
            if (listing.ButtonText("RW_ColonistBarKF.ModSettings.useBottom".Translate()))
            {
                Settings.UseBottomAlignment = true;
                Settings.UseVerticalAlignment = false;
                Settings.UseRightAlignment = false;
            }
            listing.NewColumn();
            if (listing.ButtonText("RW_ColonistBarKF.ModSettings.useLeft".Translate()))
            {

                Settings.UseBottomAlignment = false;
                Settings.UseVerticalAlignment = true;
                Settings.UseRightAlignment = false;
            }
            if (listing.ButtonText("RW_ColonistBarKF.ModSettings.useRight".Translate()))
            {

                Settings.UseBottomAlignment = false;
                Settings.UseVerticalAlignment = true;
                Settings.UseRightAlignment = true;
            }

            listing.ColumnWidth = columnwidth;

            #endregion
        }

        private void FillPageOptions(Listing_Standard listing, float columnwidth)
        {
            // forces the mod to update the values from the sliders. is deactivated by saving the values.
            Settings.Reloadsettings = true;

            listing.ColumnWidth = columnwidth;

            listing.Gap(6f);

            #region Vertical Alignment

            if (Settings.UseVerticalAlignment)
            {
#if !NoCCL
                listing.Indent();
#endif
                if (Settings.UseRightAlignment)
                {
                    listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.MarginEdge".Translate(), ref Settings.UseCustomMarginRightVer, null);
                    if (Settings.UseCustomMarginRightVer)
                    {
                        //    listing.Gap(3f);
                        Settings.MarginRightVer = listing.Slider(Settings.MarginRightVer, 0f, Screen.width / 12);
                    }
                    else
                    {
                        Settings.MarginRightVer = 21f;
                    }
                    // listing.Gap(3f);

                    listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.MarginTop".Translate(), ref Settings.UseCustomMarginTopVerRight, null);
                    if (Settings.UseCustomMarginTopVerRight)
                    {
                        //    listing.Gap(3f);
                        Settings.MarginTopVerRight = SliderMaxBarHeight(listing.GetRect(30f), Settings.MarginTopVerRight, 0f, Screen.height * 2 / 5);
                    }
                    else
                    {
                        Settings.MarginTopVerRight = 120f;
                        Settings.MaxColonistBarHeight = Screen.height - Settings.MarginTopVerRight - Settings.MarginBottomVerRight;
                        Settings.VerticalOffset = Settings.MarginTopVerRight / 2 - Settings.MarginBottomVerRight / 2;

                    }
                    //  listing.Gap(3f);
                    listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.MarginBottom".Translate(), ref Settings.UseCustomMarginBottomVerRight, null);
                    if (Settings.UseCustomMarginBottomVerRight)
                    {
                        //     listing.Gap(3f);
                        Settings.MarginBottomVerRight = SliderMaxBarHeight(listing.GetRect(30f), Settings.MarginBottomVerRight, 0f, Screen.height * 2 / 5);
                    }
                    else
                    {
                        Settings.MarginBottomVerRight = 120f;
                        Settings.MaxColonistBarHeight = Screen.height - Settings.MarginTopVerRight - Settings.MarginBottomVerRight;
                        Settings.VerticalOffset = Settings.MarginTopVerRight / 2 - Settings.MarginBottomVerRight / 2;
                    }
                }
                else
                {
                    listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.MarginEdge".Translate(), ref Settings.UseCustomMarginLeftVer, null);
                    if (Settings.UseCustomMarginLeftVer)
                    {
                        //     listing.Gap(3f);
                        Settings.MarginLeftVer = listing.Slider(Settings.MarginLeftVer, 0f, Screen.width / 12);
                    }
                    else
                    {
                        Settings.MarginLeftVer = 21f;
                    }
                    //   listing.Gap(3f);

                    listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.MarginTop".Translate(), ref Settings.UseCustomMarginTopVerLeft, null);
                    if (Settings.UseCustomMarginTopVerLeft)
                    {
                        //    listing.Gap(3f);
                        Settings.MarginTopVerLeft = SliderMaxBarHeight(listing.GetRect(30f), Settings.MarginTopVerLeft, 0f, Screen.height * 2 / 5);
                    }
                    else
                    {
                        Settings.MarginTopVerLeft = 120f;
                        Settings.MaxColonistBarHeight = Screen.height - Settings.MarginTopVerLeft - Settings.MarginBottomVerLeft;
                        Settings.VerticalOffset = Settings.MarginTopVerLeft / 2 - Settings.MarginBottomVerLeft / 2;

                    }
                    //   listing.Gap(3f);

                    listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.MarginBottom".Translate(), ref Settings.UseCustomMarginBottomVerLeft, null);
                    if (Settings.UseCustomMarginBottomVerLeft)
                    {
                        //    listing.Gap(3f);
                        Settings.MarginBottomVerLeft = SliderMaxBarHeight(listing.GetRect(30f), Settings.MarginBottomVerLeft, 0f, Screen.height * 2 / 5);
                    }
                    else
                    {
                        Settings.MarginBottomVerLeft = 120f;
                        Settings.MaxColonistBarHeight = Screen.height - Settings.MarginTopVerLeft - Settings.MarginBottomVerLeft;
                        Settings.VerticalOffset = Settings.MarginTopVerLeft / 2 - Settings.MarginBottomVerLeft / 2;
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

                if (Settings.UseBottomAlignment)
                {
                    listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.MarginEdge".Translate(), ref Settings.UseCustomMarginBottomHor, null);
                    if (Settings.UseCustomMarginBottomHor)
                    {
                        //    listing.Gap(3f);
                        Settings.MarginBottomHor = listing.Slider(Settings.MarginBottomHor, 10, Screen.height / 12);
                    }
                    else
                    {
                        Settings.MarginBottomHor = 21f;
                    }
                    //   listing.Gap(3f);


                    listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.MarginLeft".Translate(), ref Settings.UseCustomMarginLeftHorBottom, null);
                    if (Settings.UseCustomMarginLeftHorBottom)
                    {
                        //   listing.Gap(3f);
                        Settings.MarginLeftHorBottom = SliderMaxBarWidth(listing.GetRect(30f), Settings.MarginLeftHorBottom, 0f, Screen.width * 2 / 5);
                    }
                    else
                    {
                        Settings.MarginLeftHorBottom = 160f;
                        Settings.MaxColonistBarWidth = Screen.width - Settings.MarginLeftHorBottom - Settings.MarginRightHorBottom;
                        Settings.HorizontalOffset = Settings.MarginLeftHorBottom / 2 - Settings.MarginRightHorBottom / 2;
                    }
                    //  listing.Gap(3f);

                    listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.MarginRight".Translate(), ref Settings.UseCustomMarginRightHorBottom, null);
                    if (Settings.UseCustomMarginRightHorBottom)
                    {
                        //      listing.Gap(3f);
                        Settings.MarginRightHorBottom = SliderMaxBarWidth(listing.GetRect(30f), Settings.MarginRightHorBottom, 0f, Screen.width * 2 / 5);
                    }
                    else
                    {
                        Settings.MarginRightHorBottom = 160f;
                        Settings.MaxColonistBarWidth = Screen.width - Settings.MarginLeftHorBottom - Settings.MarginRightHorBottom;
                        Settings.HorizontalOffset = Settings.MarginLeftHorBottom / 2 - Settings.MarginRightHorBottom / 2;
                    }
                    //    listing.Gap(3f);
                }
                else
                {
                    listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.MarginEdge".Translate(), ref Settings.UseCustomMarginTopHor, null);
                    if (Settings.UseCustomMarginTopHor)
                    {
                        //    listing.Gap(3f);
                        Settings.MarginTopHor = listing.Slider(Settings.MarginTopHor, 10, Screen.height / 12);
                    }
                    else
                    {
                        Settings.MarginTopHor = 21f;
                    }
                    //  listing.Gap(3f);


                    listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.MarginLeft".Translate(), ref Settings.UseCustomMarginLeftHorTop, null);
                    if (Settings.UseCustomMarginLeftHorTop)
                    {
                        //    listing.Gap(3f);
                        Settings.MarginLeftHorTop = SliderMaxBarWidth(listing.GetRect(30f), Settings.MarginLeftHorTop, 0f, Screen.width * 2 / 5);
                    }
                    else
                    {
                        Settings.MarginLeftHorTop = 160f;
                        Settings.MaxColonistBarWidth = Screen.width - Settings.MarginLeftHorTop - Settings.MarginRightHorTop;
                        Settings.HorizontalOffset = Settings.MarginLeftHorTop / 2 - Settings.MarginRightHorTop / 2;
                    }
                    //  listing.Gap(3f);

                    listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.MarginRight".Translate(), ref Settings.UseCustomMarginRightHorTop, null);
                    if (Settings.UseCustomMarginRightHorTop)
                    {
                        //     listing.Gap(3f);
                        Settings.MarginRightHorTop = SliderMaxBarWidth(listing.GetRect(30f), Settings.MarginRightHorTop, 0f, Screen.width * 2 / 5);
                    }
                    else
                    {
                        Settings.MarginRightHorTop = 160f;
                        Settings.MaxColonistBarWidth = Screen.width - Settings.MarginLeftHorTop - Settings.MarginRightHorTop;
                        Settings.HorizontalOffset = Settings.MarginLeftHorTop / 2 - Settings.MarginRightHorTop / 2;
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

            listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.BasicSize".Translate(), ref Settings.UseCustomIconSize, null);

            if (Settings.UseCustomIconSize)
            {
                //     listing.Gap(3f);
                listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.FixedScale".Translate(), ref Settings.UseFixedIconScale, null);
                //    listing.Gap(3f);
                Settings.BaseSizeFloat = SliderBaseSize(listing.GetRect(30f), Settings.BaseSizeFloat, 16f, 128f);

                //     listing.Gap();
            }
            else
            {
                Settings.BaseSizeFloat = 48f;
                Settings.BaseIconSize = 20f;
                //     listing.Gap(3f);
            }


            listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.BaseSpacingHorizontal".Translate(), ref Settings.UseCustomBaseSpacingHorizontal, null);
            if (Settings.UseCustomBaseSpacingHorizontal)
            {
                //      listing.Gap(3f);
                Settings.BaseSpacingHorizontal = listing.Slider(Settings.BaseSpacingHorizontal, 1f, 72f);
            }
            else
            {
                Settings.BaseSpacingHorizontal = 24f;
                //      listing.Gap(3f);
            }

            listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.BaseSpacingVertical".Translate(), ref Settings.UseCustomBaseSpacingVertical, null);
            if (Settings.UseCustomBaseSpacingVertical)
            {
                //      listing.Gap(3f);
                Settings.BaseSpacingVertical = listing.Slider(Settings.BaseSpacingVertical, 1f, 96f);
            }
            else
            {
                Settings.BaseSpacingVertical = 32f;
            }

            #endregion

            listing.Gap(3f);
            listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.UseMoodColors".Translate(), ref Settings.UseMoodColors, null);
            listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.UseWeaponIcons".Translate(), ref Settings.UseWeaponIcons, null);

            listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.useGender".Translate(), ref Settings.UseGender, null);
            listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.useZoomToMouse".Translate(), ref Settings.useZoomToMouse, null);

            #region Gender



#if !NoCCL
            if (Settings.UseGender)
            {
            //    listing.Gap(3f);
                float indent = 24f;
                DrawMCMRegion(new Rect(indent, listing.CurHeight, listing.ColumnWidth - indent, 64f));
                listing.Gap(72f);
                listing.ColumnWidth = columnwidth / 2;
                listing.Indent();
                if (listing.ButtonText("RW_ColonistBarKF.ModSettings.ResetColors".Translate()))
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
            listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.PawnTextureCameraZoom".Translate(), ref Settings.UseCustomPawnTextureCameraZoom, null);
            if (Settings.UseCustomPawnTextureCameraZoom)
            {
                //    listing.Gap(3f);
                Settings.PawnTextureCameraZoom = listing.Slider(Settings.PawnTextureCameraZoom, 0.3f, 3f);
            }
            else
            {
                Settings.PawnTextureCameraZoom = 1.28205f;
            }
            //    listing.Gap(3f);

            listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.PawnTextureCameraHorizontalOffset".Translate(), ref Settings.UseCustomPawnTextureCameraHorizontalOffset, null);
            if (Settings.UseCustomPawnTextureCameraHorizontalOffset)
            {
                //        listing.Gap(3f);
                Settings.PawnTextureCameraHorizontalOffset = listing.Slider(Settings.PawnTextureCameraHorizontalOffset, 0.7f, -0.7f);
            }
            else
            {
                Settings.PawnTextureCameraHorizontalOffset = 0f;
            }

            listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.PawnTextureCameraVerticalOffset".Translate(), ref Settings.UseCustomPawnTextureCameraVerticalOffset, null);
            if (Settings.UseCustomPawnTextureCameraVerticalOffset)
            {
                //       listing.Gap(3f);
                Settings.PawnTextureCameraVerticalOffset = listing.Slider(Settings.PawnTextureCameraVerticalOffset, 0f, 1f);
            }
            else
            {
                Settings.PawnTextureCameraVerticalOffset = 0.3f;
            }
            #endregion

            listing.Gap(6f);

            listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.DoubleClickTime".Translate(), ref Settings.UseCustomDoubleClickTime, null);
            if (Settings.UseCustomDoubleClickTime)
            {
                //       listing.Gap(3f);
                Settings.DoubleClickTime = listing.Slider(Settings.DoubleClickTime, 0.1f, 1.5f);
            }
            else
            {
                Settings.DoubleClickTime = 0.5f;
            }

            //       listing.CheckboxLabeled("RW_ColonistBarKF.ModSettings.useExtraIcons".Translate(), ref ModSettings.useExtraIcons, null);
        }

#if !NoCCL
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
#endif
        public float SliderMaxBarWidth(Rect rect, float val, float min, float max)
        {
            GUI.skin.horizontalSlider.alignment = TextAnchor.MiddleCenter;
            float result = GUI.HorizontalSlider(rect, val, min, max);
            if (Settings.UseBottomAlignment)
            {
                Settings.MaxColonistBarWidth = Screen.width - Settings.MarginLeftHorBottom - Settings.MarginRightHorBottom;
                Settings.HorizontalOffset = Settings.MarginLeftHorBottom / 2 - Settings.MarginRightHorBottom / 2;

            }
            else
            {
                Settings.MaxColonistBarWidth = Screen.width - Settings.MarginLeftHorTop - Settings.MarginRightHorTop;
                Settings.HorizontalOffset = Settings.MarginLeftHorTop / 2 - Settings.MarginRightHorTop / 2;

            }
            return result;
        }

        public float SliderMaxBarHeight(Rect rect, float val, float min, float max)
        {
            GUI.skin.horizontalSlider.alignment = TextAnchor.MiddleCenter;
            float result = GUI.HorizontalSlider(rect, val, min, max);
            if (Settings.UseRightAlignment)
            {
                Settings.MaxColonistBarHeight = Screen.height - Settings.MarginTopVerRight - Settings.MarginBottomVerRight;
                Settings.VerticalOffset = Settings.MarginTopVerRight / 2 - Settings.MarginBottomVerRight / 2;
            }
            else
            {
                Settings.MaxColonistBarHeight = Screen.height - Settings.MarginTopVerLeft - Settings.MarginBottomVerLeft;
                Settings.VerticalOffset = Settings.MarginTopVerLeft / 2 - Settings.MarginBottomVerLeft / 2;
            }
            return result;
        }

        public float SliderBaseSize(Rect rect, float val, float min, float max)
        {
            GUI.skin.horizontalSlider.alignment = TextAnchor.MiddleCenter;
            float result = GUI.HorizontalSlider(rect, val, min, max);
            Settings.BaseIconSize = Settings.BaseSizeFloat / 2f - 4f;
            return result;
        }
#if !NoCCL
        private LabeledInput_Color femaleColorField = new LabeledInput_Color(Settings.FemaleColor, "RW_ColonistBarKF.ModSettings.FemaleColor".Translate());
        private LabeledInput_Color maleColorField = new LabeledInput_Color(Settings.MaleColor, "RW_ColonistBarKF.ModSettings.MaleColor".Translate());
#endif
#if NoCCL
        public void ExposeData()
#else
        public override void ExposeData()
#endif
        {
            Scribe_Values.LookValue(ref Settings.UseCustomMarginTopHor, "useCustomMarginTopHor", false, false);
            Scribe_Values.LookValue(ref Settings.UseCustomMarginBottomHor, "UseCustomMarginBottomHor", false, false);
            Scribe_Values.LookValue(ref Settings.UseCustomMarginLeftHorTop, "useCustomMarginLeftHor", false, false);
            Scribe_Values.LookValue(ref Settings.UseCustomMarginRightHorTop, "useCustomMarginRightHor", false, false);
            Scribe_Values.LookValue(ref Settings.UseCustomMarginLeftHorBottom, "UseCustomMarginLeftHorBottom", false, false);
            Scribe_Values.LookValue(ref Settings.UseCustomMarginRightHorBottom, "UseCustomMarginRightHorBottom", false, false);

            Scribe_Values.LookValue(ref Settings.UseCustomMarginTopVerLeft, "UseCustomMarginTopVerLeft", false, false);
            Scribe_Values.LookValue(ref Settings.UseCustomMarginTopVerRight, "UseCustomMarginTopVerRight", false, false);
            Scribe_Values.LookValue(ref Settings.UseCustomMarginLeftVer, "useCustomMarginLeftRightVer", false, false);
            Scribe_Values.LookValue(ref Settings.UseCustomMarginRightVer, "UseCustomMarginRightVer", false, false);
            Scribe_Values.LookValue(ref Settings.UseCustomMarginBottomVerLeft, "UseCustomMarginBottomVerLeft", false, false);
            Scribe_Values.LookValue(ref Settings.UseCustomMarginBottomVerRight, "UseCustomMarginBottomVerRight", false, false);


            Scribe_Values.LookValue(ref Settings.UseCustomBaseSpacingHorizontal, "useCustomBaseSpacingHorizontal", false, false);
            Scribe_Values.LookValue(ref Settings.UseCustomBaseSpacingVertical, "useCustomBaseSpacingVertical", false, false);
            Scribe_Values.LookValue(ref Settings.UseCustomIconSize, "useCustomIconSize", false, false);
            Scribe_Values.LookValue(ref Settings.UseFixedIconScale, "useFixedIconScale", false, false);
            Scribe_Values.LookValue(ref Settings.UseCustomPawnTextureCameraHorizontalOffset, "UseCustomPawnTextureCameraHorizontalOffset", false, false);
            Scribe_Values.LookValue(ref Settings.UseCustomPawnTextureCameraVerticalOffset, "useCustomPawnTextureCameraVerticalOffset", false, false);
            Scribe_Values.LookValue(ref Settings.UseCustomPawnTextureCameraZoom, "useCustomPawnTextureCameraZoom", false, false);
            Scribe_Values.LookValue(ref Settings.UseCustomDoubleClickTime, "useCustomDoubleClick", false, false);
            Scribe_Values.LookValue(ref Settings.UseGender, "useGender", false, false);
            Scribe_Values.LookValue(ref Settings.UseVerticalAlignment, "useVerticalAlignment", false, false);
            Scribe_Values.LookValue(ref Settings.UseRightAlignment, "useRightAlignment", false, false);
            Scribe_Values.LookValue(ref Settings.UseBottomAlignment, "useBottomAlignment", false, false);

            Scribe_Values.LookValue(ref Settings.UseMoodColors, "UseMoodColors", false, false);
            Scribe_Values.LookValue(ref Settings.UseWeaponIcons, "UseWeaponIcons", false, false);

            Scribe_Values.LookValue(ref Settings.MarginTopHor, "MarginTopHor", 21f, false);
            Scribe_Values.LookValue(ref Settings.MarginBottomHor, "MarginBottomHor", 21f, false);
            Scribe_Values.LookValue(ref Settings.MarginLeftHorTop, "MarginLeftHorTop", 160f, false);
            Scribe_Values.LookValue(ref Settings.MarginRightHorTop, "MarginRightHorTop", 160f, false);
            Scribe_Values.LookValue(ref Settings.MarginLeftHorBottom, "MarginLeftHorBottom", 160f, false);
            Scribe_Values.LookValue(ref Settings.MarginRightHorBottom, "MarginRightHorBottom", 160f, false);

            Scribe_Values.LookValue(ref Settings.MarginTopVerLeft, "MarginTopVerLeft", 120f, false);
            Scribe_Values.LookValue(ref Settings.MarginBottomVerLeft, "MarginBottomVerLeft", 120f, false);
            Scribe_Values.LookValue(ref Settings.MarginTopVerRight, "MarginTopVerRight", 120f, false);
            Scribe_Values.LookValue(ref Settings.MarginBottomVerRight, "MarginBottomVerRight", 120f, false);
            Scribe_Values.LookValue(ref Settings.MarginLeftVer, "MarginLeftVer", 21f, false);
            Scribe_Values.LookValue(ref Settings.MarginRightVer, "MarginRightVer", 21f, false);

            Scribe_Values.LookValue(ref Settings.HorizontalOffset, "HorizontalOffset", 0f, false);
            Scribe_Values.LookValue(ref Settings.VerticalOffset, "VerticalOffset", 0f, false);


            Scribe_Values.LookValue(ref Settings.BaseSpacingHorizontal, "BaseSpacingHorizontal", 24f, false);
            Scribe_Values.LookValue(ref Settings.BaseSpacingVertical, "BaseSpacingVertical", 32f, false);
            Scribe_Values.LookValue(ref Settings.BaseSizeFloat, "BaseSizeFloat", 48f, false);
            Scribe_Values.LookValue(ref Settings.BaseIconSize, "BaseIconSize", 20f, false);
            Scribe_Values.LookValue(ref Settings.PawnTextureCameraHorizontalOffset, "PawnTextureCameraHorizontalOffset", 0f, false);
            Scribe_Values.LookValue(ref Settings.PawnTextureCameraVerticalOffset, "PawnTextureCameraVerticalOffset", 0.3f, false);
            Scribe_Values.LookValue(ref Settings.PawnTextureCameraZoom, "PawnTextureCameraZoom", 1.28205f, false);
            Scribe_Values.LookValue(ref Settings.MaxColonistBarWidth, "MaxColonistBarWidth");
            Scribe_Values.LookValue(ref Settings.MaxColonistBarHeight, "MaxColonistBarHeight");


            Scribe_Values.LookValue(ref Settings.DoubleClickTime, "DoubleClickTime", 0.5f, false);

            Scribe_Values.LookValue(ref Settings.FemaleColor, "FemaleColor");
            Scribe_Values.LookValue(ref Settings.MaleColor, "MaleColor");

            Scribe_Values.LookValue(ref Settings.MaxRows, "MaxRows");
            Scribe_Values.LookValue(ref Settings.SortBy, "SortBy");
            Scribe_Values.LookValue(ref Settings.useZoomToMouse, "useZoomToMouse");


            Settings.Reloadsettings = false;
#if !NoCCL
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                femaleColorField.Value = Settings.FemaleColor;
                maleColorField.Value = Settings.MaleColor;
            }
#endif
        }

#endregion

    }
}
