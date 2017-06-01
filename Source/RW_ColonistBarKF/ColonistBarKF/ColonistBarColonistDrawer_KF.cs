using static ColonistBarKF.Settings;

using static Verse.ColonistBarTextures;

namespace ColonistBarKF
{
    using System;
    using System.Collections.Generic;

    using RimWorld;
    using RimWorld.Planet;

    using UnityEngine;

    using Verse;
    using Verse.AI.Group;
    using Verse.Sound;

    [StaticConstructorOnStartup]
    public class ColonistBarColonistDrawer_KF
    {
        private static readonly Color HighlightColor = new Color(0.5f, 0.5f, 0.5f, 1f);

        private static Vector2[] bracketLocs = new Vector2[4];

        private static Vector3 pawnTextureCameraOffset;

        private Dictionary<string, string> pawnLabelsCache = new Dictionary<string, string>();

        private static Vector3 PawnTextureCameraOffset
        {
            get
            {
                float pawnTextureCameraOffsetNew = ColBarSettings.PawnTextureCameraZoom / 1.28205f;
                pawnTextureCameraOffset =
                    new Vector3(
                        ColBarSettings.PawnTextureCameraHorizontalOffset / pawnTextureCameraOffsetNew,
                        0f,
                        ColBarSettings.PawnTextureCameraVerticalOffset / pawnTextureCameraOffsetNew);
                return pawnTextureCameraOffset;
            }
        }

        private static Vector2 PawnTextureSize
            => new Vector2(ColBarSettings.BaseSizeFloat - 2f, ColBarSettings.BaseSizeFloat * 1.5f);

        private static Pawn SelPawn => Find.Selector.SingleSelectedThing as Pawn;

        public void DrawColonist(Rect outerRect, Pawn colonist, Map pawnMap)
        {
            PawnStats pawnStats = MapComponent_PSI.Get.GetCache(colonist);

            // if (pawnStats.IconCount == 0)
            // outerRect.width
            float entryRectAlpha = ColonistBar_KF.GetEntryRectAlpha(outerRect);
            this.ApplyEntryInAnotherMapAlphaFactor(pawnMap, ref entryRectAlpha);

            bool colonistAlive = (!colonist.Dead)
                                     ? Find.Selector.SelectedObjects.Contains(colonist)
                                     : Find.Selector.SelectedObjects.Contains(colonist.Corpse);

            Color color = new Color(1f, 1f, 1f, entryRectAlpha);
            GUI.color = color;


            // testing
            // GUI.DrawTexture(outerRect, DarkGrayFond);
            Rect pawnRect;
            Rect moodBorderRect;
            Rect psiRect;

            BuildRects(pawnStats, ref outerRect, out pawnRect, out moodBorderRect, out psiRect);

            Color bgColor = color;
            if (ColBarSettings.UseGender)
            {
                bgColor = pawnStats.BGColor;

                bgColor.a = entryRectAlpha;
                GUI.color = bgColor;
            }

            GUI.DrawTexture(pawnRect, ColBarSettings.UseGender ? BGTexGrey : BGTexVanilla);
            GUI.color = color;

            if (colonist.needs?.mood != null)
            {
                if (ColBarSettings.UseExternalMoodBar || ColBarSettings.UseNewMood)
                {
                    if (pawnStats != null)
                    {
                        // string tooltip = colonist.needs.mood.GetTipString();
                        DrawNewMoodRect(moodBorderRect, pawnStats.Mood, pawnStats.Mb);
                    }
                }
                else
                {
                    Rect position = pawnRect.ContractedBy(2f);
                    float num = position.height * colonist.needs.mood.CurLevelPercentage;
                    position.yMin = position.yMax - num;
                    position.height = num;
                    GUI.DrawTexture(position, VanillaMoodBGTex);
                }
            }

            //    Rect rect2 = outerRect.ContractedBy(-2f * ColonistBar_KF.Scale);
            Rect rect2 = outerRect.ContractedBy(-2f);

            if (colonistAlive && !WorldRendererUtility.WorldRenderedNow)
            {
                this.DrawSelectionOverlayOnGUI(colonist, rect2);
            }
            else if (WorldRendererUtility.WorldRenderedNow && colonist.IsCaravanMember()
                     && Find.WorldSelector.IsSelected(colonist.GetCaravan()))
            {
                this.DrawCaravanSelectionOverlayOnGUI(colonist.GetCaravan(), rect2);
            }

            GUI.DrawTexture(
                this.GetPawnTextureRect(pawnRect.x, pawnRect.y),
                PortraitsCache.Get(
                    colonist,
                    PawnTextureSize,
                    PawnTextureCameraOffset,
                    ColBarSettings.PawnTextureCameraZoom));
            if (colonist.CurJob != null) DrawCurrentJobTooltip(colonist, pawnRect);

            if (ColBarSettings.UseWeaponIcons)
            {
                this.DrawWeaponIcon(pawnRect, colonist);
            }

            GUI.color = new Color(1f, 1f, 1f, entryRectAlpha * 0.8f);
            this.DrawIcons(pawnRect, colonist);
            GUI.color = color;
            if (colonist.Dead)
            {
                GUI.DrawTexture(pawnRect, DeadColonistTex);
                pawnStats.BGColor = color;
            }

            // float num = 4f * Scale;
            Vector2 pos = new Vector2(pawnRect.center.x, pawnRect.yMax + 1f * ColonistBar_KF.Scale);
            GenMapUI.DrawPawnLabel(colonist, pos, entryRectAlpha, pawnRect.width, this.pawnLabelsCache);

            GUI.color = Color.white;

            // PSI
            if (ColBarSettings.UsePsi)
            {
                PSI.PSI.DrawColonistIcons(colonist, false, entryRectAlpha, psiRect);
            }
        }

        public void DrawGroupFrame(int group)
        {
            Rect position = this.GroupFrameRect(group);
            List<ColonistBar.Entry> entries = ColonistBar_KF.BarHelperKf.Entries;
            Map map = entries.Find((ColonistBar.Entry x) => x.group == group).map;
            float num;
            if (map == null)
            {
                if (WorldRendererUtility.WorldRenderedNow)
                {
                    num = 1f;
                }
                else
                {
                    num = 0.75f;
                }
            }
            else if (map != Find.VisibleMap || WorldRendererUtility.WorldRenderedNow)
            {
                num = 0.75f;
            }
            else
            {
                num = 1f;
            }

            Widgets.DrawRectFast(position, new Color(0.5f, 0.5f, 0.5f, 0.4f * num));
        }

        public void HandleClicks(Rect rect, Pawn colonist)
        {
            if (Mouse.IsOver(rect))
            {
                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        {
                            switch (Event.current.button)
                            {
                                // LMB
                                // Double click
                                case 0:
                                    if (Event.current.clickCount == 2)
                                    {
                                        // use event so it doesn't bubble through
                                        Event.current.Use();

                                        if (FollowMe.CurrentlyFollowing)
                                        {
                                            FollowMe.StopFollow("Selected another colonist on bar");
                                            FollowMe.TryStartFollow(colonist);
                                        }
                                        else
                                        {
                                            CameraJumper.TryJump(colonist);
                                        }

                                        // clickedColonist = null;
                                    }

                                    // if (Event.current.clickCount == 1)
                                    // {
                                    // clickedColonist = colonist;
                                    // }
                                    break;
                                // RMB
                                case 1:
                                    List<FloatMenuOption> floatOptionList = new List<FloatMenuOption>();

                                    if (colonist != null && SelPawn != null && SelPawn != colonist && colonist.Map != null)
                                    {
                                        foreach (FloatMenuOption choice in
                                            FloatMenuMakerMap.ChoicesAtFor(colonist.TrueCenter(), SelPawn))
                                        {
                                            floatOptionList.Add(choice);
                                        }

                                        if (floatOptionList.Any())
                                            floatOptionList.Add(
                                                new FloatMenuOption("--------------------", delegate { }));
                                    }

                                    if (colonist.Map != null)
                                    {
                                        if (!FollowMe.CurrentlyFollowing)
                                        {
                                            floatOptionList.Add(
                                                new FloatMenuOption(
                                                    "FollowMe.StartFollow".Translate() + " - " + colonist.LabelShort,
                                                    delegate { FollowMe.TryStartFollow(colonist); }));
                                        }
                                        else
                                        {
                                            floatOptionList.Add(
                                                new FloatMenuOption(
                                                    "FollowMe.StopFollow".Translate(),
                                                    delegate { FollowMe.StopFollow("Canceled in dropdown"); }));
                                        }
                                    }

                                    floatOptionList.Add(
                                        new FloatMenuOption(
                                            "CBKF.Settings.Vanilla".Translate(),
                                            delegate
                                                {
                                                    ColBarSettings.SortBy = SettingsColonistBar.SortByWhat.vanilla;
                                                    ColonistBar_KF.MarkColonistsDirty();

                                                    // CheckRecacheEntries();
                                                }));
                                    floatOptionList.Add(
                                        new FloatMenuOption(
                                            "CBKF.Settings.ByName".Translate(),
                                            delegate
                                                {
                                                    ColBarSettings.SortBy = SettingsColonistBar.SortByWhat.byName;
                                                    ColonistBar_KF.MarkColonistsDirty();

                                                    // CheckRecacheEntries();
                                                }));

                                    floatOptionList.Add(
                                        new FloatMenuOption(
                                            "CBKF.Settings.SexAge".Translate(),
                                            delegate
                                                {
                                                    ColBarSettings.SortBy = SettingsColonistBar.SortByWhat.sexage;
                                                    ColonistBar_KF.MarkColonistsDirty();

                                                    // CheckRecacheEntries();
                                                }));

                                    floatOptionList.Add(
                                        new FloatMenuOption(
                                            "CBKF.Settings.Mood".Translate(),
                                            delegate
                                                {
                                                    ColBarSettings.SortBy = SettingsColonistBar.SortByWhat.mood;
                                                    ColonistBar_KF.MarkColonistsDirty();

                                                    // CheckRecacheEntries();
                                                }));
                                    floatOptionList.Add(
                                        new FloatMenuOption(
                                            "CBKF.Settings.Health".Translate(),
                                            delegate
                                                {
                                                    ColBarSettings.SortBy = SettingsColonistBar.SortByWhat.health;
                                                    ColonistBar_KF.MarkColonistsDirty();

                                                    // CheckRecacheEntries();
                                                }));
                                    floatOptionList.Add(
                                        new FloatMenuOption(
                                            "CBKF.Settings.Medic".Translate(),
                                            delegate
                                                {
                                                    ColBarSettings.SortBy = SettingsColonistBar.SortByWhat.medic;
                                                    ColonistBar_KF.MarkColonistsDirty();

                                                    // CheckRecacheEntries();
                                                }));
                                    floatOptionList.Add(
                                        new FloatMenuOption(
                                            "CBKF.Settings.Weapons".Translate(),
                                            delegate
                                                {
                                                    ColBarSettings.SortBy = SettingsColonistBar.SortByWhat.weapons;
                                                    ColonistBar_KF.MarkColonistsDirty();

                                                    // CheckRecacheEntries();
                                                }));

                                    floatOptionList.Add(
                                        new FloatMenuOption(
                                            "CBKF.Settings.SettingsColonistBar".Translate(),
                                            delegate { Find.WindowStack.Add(new ColonistBarKF_Settings()); }));
                                    FloatMenu window = new FloatMenu(
                                                           floatOptionList,
                                                           "CBKF.Settings.SortingOptions".Translate());
                                    Find.WindowStack.Add(window);

                                    // use event so it doesn't bubble through
                                    Event.current.Use();
                                    break;
                            }

                            break;
                        }
                }
                // MMB
                if (Event.current.type == EventType.mouseUp && Event.current.button == 2)
                {
                    // start following
                    if (FollowMe.CurrentlyFollowing)
                    {
                        FollowMe.StopFollow("Canceled by user");
                    }
                    else
                    {
                        FollowMe.TryStartFollow(colonist);
                    }

                    // use event so it doesn't bubble through
                    Event.current.Use();
                }
            }
        }

        // RimWorld.ColonistBarColonistDrawer
        public void HandleGroupFrameClicks(int group)
        {
            Rect rect = this.GroupFrameRect(group);
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && Mouse.IsOver(rect))
            {
                bool worldRenderedNow = WorldRendererUtility.WorldRenderedNow;
                ColonistBar.Entry entry =
                    ColonistBar_KF.BarHelperKf.Entries.Find((ColonistBar.Entry x) => x.@group == @group);
                Map map = entry.map;

                if (!ColonistBar_KF.BarHelperKf.AnyColonistOrCorpseAt(UI.MousePositionOnUIInverted))
                {
                    if ((!worldRenderedNow && !Find.Selector.dragBox.IsValidAndActive)
                        || (worldRenderedNow && !Find.WorldSelector.dragBox.IsValidAndActive))
                    {
                        Find.Selector.dragBox.active = false;
                        Find.WorldSelector.dragBox.active = false;
                        if (map == null)
                        {
                            if (worldRenderedNow)
                            {
                                CameraJumper.TrySelect(entry.pawn);
                            }
                            else
                            {
                                CameraJumper.TryJumpAndSelect(entry.pawn);
                            }
                        }
                        else
                        {
                            if (!CameraJumper.TryHideWorld() && Current.Game.VisibleMap != map)
                            {
                                SoundDefOf.MapSelected.PlayOneShotOnCamera(null);
                            }
                            Current.Game.VisibleMap = map;
                        }
                    }
                }
            }
            // RMB vanilla - not wanted
            //         if (Event.current.button == 1 && Widgets.ButtonInvisible(rect, false))
            //         {
            //             ColonistBar.Entry entry2 = ColonistBar_KF.BarHelperKf.Entries.Find((ColonistBar.Entry x) => x.group == group);
            //             if (entry2.map != null)
            //             {
            //                 CameraJumper.TryJumpAndSelect(CameraJumper.GetWorldTargetOfMap(entry2.map));
            //             }
            //             else if (entry2.pawn != null)
            //             {
            //                 CameraJumper.TryJumpAndSelect(entry2.pawn);
            //             }
            //         }
        }

        public void Notify_RecachedEntries()
        {
            this.pawnLabelsCache.Clear();
        }

        private static void BuildRects(PawnStats pawnStats, ref Rect outerRect, out Rect pawnRect, out Rect moodBorderRect, out Rect psiRect)
        {
            float offsetX = 0f;
            bool flag = false;

            float widthPsiFloatX = ColonistBar_KF.WidthPSIHorizontal * ColonistBar_KF.Scale;

            if (ColBarSettings.UsePsi)
            {
                if (pawnStats.thisColCount < ColonistBar_KF.PsiRowsOnBar)
                {
                    if (ColonistBar_KF.PsiRowsOnBar == 2)
                    {
                        if (pawnStats.thisColCount == 1)
                        {
                            offsetX = widthPsiFloatX / 2;
                        }
                        if (pawnStats.thisColCount == 0)
                        {
                            offsetX = widthPsiFloatX;
                        }
                    }
                    else if (ColonistBar_KF.PsiRowsOnBar == 1)
                    {
                        offsetX = widthPsiFloatX;
             //           flag = true;
                    }

                    switch (ColBarSettings.ColBarPsiIconPos)
                    {
                        case Position.Alignment.Left:
                        case Position.Alignment.Right:
                            if (!flag)
                            {
                                outerRect.x += offsetX / 2;
                            }
                            outerRect.width -= offsetX;
                            break;
                    }
                }
            }

            pawnRect = new Rect(outerRect.x, outerRect.y, ColonistBar_KF.PawnSize.x, ColonistBar_KF.PawnSize.y);

            moodBorderRect = new Rect(pawnRect);
            psiRect = new Rect(pawnRect);

            if (ColBarSettings.UseExternalMoodBar)
            {
                // draw mood border
                switch (ColBarSettings.MoodBarPos)
                {
                    case Position.Alignment.Left:
                        moodBorderRect.width /= 4;
                        pawnRect.x += moodBorderRect.width;
                        break;
                    case Position.Alignment.Right:
                        moodBorderRect.width /= 4;
                        moodBorderRect.x = pawnRect.xMax;
                        break;
                    case Position.Alignment.Top:
                        moodBorderRect.height /= 4;
                        pawnRect.y += moodBorderRect.height;
                        break;
                    case Position.Alignment.Bottom:
                        moodBorderRect.height /= 4;
                        moodBorderRect.y = pawnRect.yMax + ColonistBar_KF.SpacingLabel;
                        break;
                }

                psiRect = new Rect(pawnRect);

                if (ColBarSettings.UsePsi)
                {
                    switch (ColBarSettings.ColBarPsiIconPos)
                    {
                        case Position.Alignment.Left:
                            pawnRect.x += widthPsiFloatX;
                            moodBorderRect.x += widthPsiFloatX;
                            psiRect.x = Mathf.Min(moodBorderRect.xMin, pawnRect.xMin) - psiRect.width;
                            break;
                        case Position.Alignment.Right:
                            psiRect.x = Mathf.Max(pawnRect.xMax, moodBorderRect.xMax);
                            break;
                        case Position.Alignment.Top:
                            pawnRect.y += ColonistBar_KF.HeightPSIVertical * ColonistBar_KF.Scale;
                            moodBorderRect.y += ColonistBar_KF.HeightPSIVertical * ColonistBar_KF.Scale;
                            psiRect.yMax = Mathf.Min(pawnRect.yMin, moodBorderRect.yMin);
                            break;
                        case Position.Alignment.Bottom:
                            psiRect.y = Math.Max(pawnRect.yMax + ColonistBar_KF.SpacingLabel, moodBorderRect.yMax);
                            break;
                    }
                }
            }
            else
            {
                if (ColBarSettings.UsePsi)
                {
                    switch (ColBarSettings.ColBarPsiIconPos)
                    {
                        case Position.Alignment.Left:
                            pawnRect.x += widthPsiFloatX;
                            break;
                        case Position.Alignment.Right:
                            psiRect.x = pawnRect.xMax;
                            break;
                        case Position.Alignment.Top:
                            pawnRect.y += ColonistBar_KF.HeightPSIVertical * ColonistBar_KF.Scale;
                            break;
                        case Position.Alignment.Bottom:
                            psiRect.y = pawnRect.yMax + ColonistBar_KF.SpacingLabel;
                            break;
                    }

                    moodBorderRect = new Rect(pawnRect);
                }
            }
        }

        private static void DrawCurrentJobTooltip(Pawn colonist, Rect pawnRect)
        {
            string jobDescription = null;
            Lord lord = colonist.GetLord();
            if (lord != null && lord.LordJob != null)
            {
                jobDescription = lord.LordJob.GetReport();
            }

            if (colonist.jobs.curJob != null)
            {
                try
                {
                    string text2 = colonist.jobs.curDriver.GetReport().CapitalizeFirst();
                    if (!jobDescription.NullOrEmpty())
                    {
                        jobDescription = jobDescription + ": " + text2;
                    }
                    else
                    {
                        jobDescription = text2;
                    }
                }
                catch (Exception arg)
                {
                    Log.Message("JobDriver.GetReport() exception: " + arg);
                }
            }

            if (!jobDescription.NullOrEmpty())
            {
                TooltipHandler.TipRegion(pawnRect, jobDescription);
            }
        }

        private static void DrawMentalThreshold(Rect moodRect, float threshold, float currentMood)
        {
            if (ColBarSettings.MoodBarPos == Position.Alignment.Left
                || ColBarSettings.MoodBarPos == Position.Alignment.Right)
                GUI.DrawTexture(
                    new Rect(moodRect.x, moodRect.yMax - moodRect.height * threshold, moodRect.width, 1),
                    MoodBreakTex);
            else
                GUI.DrawTexture(
                    new Rect(moodRect.x + moodRect.width * threshold, moodRect.y, 1, moodRect.height),
                    MoodBreakTex);

            /*if (currentMood <= threshold)
			{
				GUI.DrawTexture(new Rect(moodRect.xMax-4, moodRect.yMax - moodRect.height * threshold, 8, 2), MoodBreakCrossedTex);
			}*/
        }

        private static void DrawNewMoodRect(Rect moodBorderRect, Need mood, MentalBreaker mb, string tooltip = null)
        {
            Rect moodRect = moodBorderRect.ContractedBy(2f);
            Rect rect1 = new Rect();
            Rect rect2 = new Rect();

            Color color = GUI.color;
            Color moodCol = new Color();

            Color critColor = Color.clear;
            bool showCritical = false;

            float moodPercent;
            float curMood = mood.CurLevelPercentage;

            GUI.DrawTexture(moodBorderRect, MoodBGTex);

            if (curMood > mb.BreakThresholdMinor)
            {
                moodPercent = Mathf.InverseLerp(mb.BreakThresholdMinor, 1f, curMood);
                moodCol = ColBlue;
                if (moodPercent < 0.3f)
                {
                    critColor = Color.Lerp(ColorNeutralSoft, ColorNeutralStatusSolid, Mathf.InverseLerp(0.3f, 0f, moodPercent));
                    critColor *= ColYellow;
                    showCritical = true;
                }
            }
            else if (curMood > mb.BreakThresholdMajor)
            {
                moodPercent = Mathf.InverseLerp(mb.BreakThresholdMajor, mb.BreakThresholdMinor, curMood);
                moodCol = ColYellow;
                if (moodPercent < 0.4f)
                {
                    critColor = Color.Lerp(ColorNeutralSoft, ColorNeutralStatusSolid, Mathf.InverseLerp(0.4f, 0f, moodPercent));
                    critColor *= ColOrange;
                    showCritical = true;
                }
            }
            else if (curMood > mb.BreakThresholdExtreme)
            {
                moodPercent = Mathf.InverseLerp(mb.BreakThresholdExtreme, mb.BreakThresholdMajor, curMood);
                moodCol = ColOrange;
                if (moodPercent < 0.5f)
                {
                    critColor = Color.Lerp(ColorNeutralSoft, ColorNeutralStatusSolid, Mathf.InverseLerp(0.5f, 0f, moodPercent));
                    critColor *= ColVermillion;
                    showCritical = true;
                }
            }
            else
            {
                // moodPercent = mb.BreakThresholdExtreme > 0.01f ? Mathf.InverseLerp(0f, mb.BreakThresholdExtreme, curMood) : 1f;
                moodPercent = 1f;
                moodCol = ColVermillion;
            }


            moodCol.a = color.a;

            GUI.color = moodCol;
            GUI.DrawTexture(moodRect, MoodNeutralBGTex);
            if (showCritical)
            {
                critColor.a *= color.a;
                GUI.color = critColor;
                GUI.DrawTexture(moodRect, MoodNeutralTex);
                GUI.color = moodCol;
            }
            DrawCurrentMood(moodRect, MoodNeutralTex, moodPercent, mood, out rect1, out rect2);
            GUI.color = color;

            DrawMentalThreshold(moodRect, mb.BreakThresholdExtreme, mood.CurLevelPercentage);
            DrawMentalThreshold(moodRect, mb.BreakThresholdMajor, mood.CurLevelPercentage);
            DrawMentalThreshold(moodRect, mb.BreakThresholdMinor, mood.CurLevelPercentage);

            GUI.DrawTexture(rect1, MoodTargetTex);
            GUI.DrawTexture(rect2, MoodTargetTex);

            // TooltipHandler.TipRegion(moodRect, tooltip);
        }

        private static void DrawCurrentMood(Rect moodRect, Texture2D moodTex, float moodPercent, Need mood, out Rect rect1, out Rect rect2)
        {
            switch (ColBarSettings.MoodBarPos)
            {
                default:
                    rect1 = new Rect(
                                moodRect.x,
                                moodRect.yMax - moodRect.height * mood.CurInstantLevelPercentage,
                                moodRect.width,
                                1);
                    rect2 = new Rect(
                                moodRect.xMax + 1,
                                moodRect.yMax - moodRect.height * mood.CurInstantLevelPercentage - 1,
                                2,
                                3);
                    GUI.DrawTexture(moodRect.BottomPart(moodPercent), moodTex);
                    break;

                case Position.Alignment.Top:
                    rect1 = new Rect(
                                moodRect.x + moodRect.width * mood.CurInstantLevelPercentage,
                                moodRect.y,
                                1,
                                moodRect.height);
                    rect2 = new Rect(
                                moodRect.x + moodRect.width * mood.CurInstantLevelPercentage - 1,
                                moodRect.yMin - 1,
                                3,
                                2);
                    GUI.DrawTexture(moodRect.LeftPart(moodPercent), moodTex);
                    break;
                case Position.Alignment.Bottom:
                    rect1 = new Rect(
                                moodRect.x + moodRect.width * mood.CurInstantLevelPercentage,
                                moodRect.y,
                                1,
                                moodRect.height);
                    rect2 = new Rect(
                                moodRect.x + moodRect.width * mood.CurInstantLevelPercentage - 1,
                                moodRect.yMax + 1,
                                3,
                                2);
                    GUI.DrawTexture(moodRect.LeftPart(moodPercent), moodTex);
                    break;
            }
        }

        private void ApplyEntryInAnotherMapAlphaFactor(Map map, ref float alpha)
        {
            if (map == null)
            {
                if (!WorldRendererUtility.WorldRenderedNow)
                {
                    alpha = Mathf.Min(alpha, 0.4f);
                }
            }
            else if (map != Find.VisibleMap || WorldRendererUtility.WorldRenderedNow)
            {
                alpha = Mathf.Min(alpha, 0.4f);
            }
        }

        private void DrawCaravanSelectionOverlayOnGUI(Caravan caravan, Rect rect)
        {
            float num = 0.4f * ColonistBar_KF.Scale;
            Vector2 textureSize = new Vector2(
                                      SelectionDrawerUtility.SelectedTexGUI.width * num,
                                      SelectionDrawerUtility.SelectedTexGUI.height * num);
            SelectionDrawerUtility.CalculateSelectionBracketPositionsUI(
                bracketLocs,
                caravan,
                rect,
                WorldSelectionDrawer.SelectTimes,
                textureSize,
                20f * ColonistBar_KF.Scale);
            this.DrawSelectionOverlayOnGUI(bracketLocs, num);
        }

        private void DrawIcon(Texture2D icon, ref Vector2 pos, string tooltip)
        {
            float num = ColBarSettings.BaseSizeFloat * 0.4f * ColonistBar_KF.Scale;
            Rect rect = new Rect(pos.x, pos.y, num, num);
            GUI.DrawTexture(rect, icon);
            TooltipHandler.TipRegion(rect, tooltip);
            pos.x += num;
        }

        private void DrawIcons(Rect rect, Pawn colonist)
        {
            if (colonist.Dead)
            {
                return;
            }

            Vector2 vector = new Vector2(rect.x + 1f, rect.yMax - rect.width / 5 * 2 - 1f);
            bool attacking = false;
            if (colonist.CurJob != null)
            {
                JobDef def = colonist.CurJob.def;
                if (def == JobDefOf.AttackMelee || def == JobDefOf.AttackStatic)
                {
                    attacking = true;
                }
                else if (def == JobDefOf.WaitCombat)
                {
                    Stance_Busy stanceBusy = colonist.stances.curStance as Stance_Busy;
                    if (stanceBusy != null && stanceBusy.focusTarg.IsValid)
                    {
                        attacking = true;
                    }
                }
            }

            if (colonist.InAggroMentalState)
            {
                // DrawIcon(PSI.PSI.PSIMaterials[Icons.Aggressive].mainTexture as Texture2D, ref vector, colonist.MentalStateDef.LabelCap);
                this.DrawIcon(Icon_MentalStateAggro, ref vector, colonist.MentalStateDef.LabelCap);
            }
            else if (colonist.InMentalState)
            {
                // DrawIcon(PSI.PSI.PSIMaterials[Icons.Dazed].mainTexture as Texture2D, ref vector, colonist.MentalStateDef.LabelCap);
                this.DrawIcon(Icon_MentalStateNonAggro, ref vector, colonist.MentalStateDef.LabelCap);
            }
            else if (colonist.InBed() && colonist.CurrentBed().Medical)
            {
                // DrawIcon(PSI.PSI.PSIMaterials[Icons.Health].mainTexture as Texture2D, ref vector, "ActivityIconMedicalRest".Translate());
                this.DrawIcon(Icon_MedicalRest, ref vector, "ActivityIconMedicalRest".Translate());
            }
            else if (colonist.CurJob != null && colonist.jobs.curDriver.asleep)
            {
                // DrawIcon(PSI.PSI.PSIMaterials[Icons.Tired].mainTexture as Texture2D, ref vector, "ActivityIconSleeping".Translate());
                this.DrawIcon(Icon_Sleeping, ref vector, "ActivityIconSleeping".Translate());
            }
            else if (colonist.CurJob != null && colonist.CurJob.def == JobDefOf.FleeAndCower)
            {
                // DrawIcon(PSI.PSI.PSIMaterials[Icons.Leave].mainTexture as Texture2D, ref vector, "ActivityIconFleeing".Translate());
                this.DrawIcon(Icon_Fleeing, ref vector, "ActivityIconFleeing".Translate());
            }
            else if (attacking)
            {
                this.DrawIcon(Icon_Attacking, ref vector, "ActivityIconAttacking".Translate());
            }
            else if (colonist.mindState.IsIdle && GenDate.DaysPassed >= 1)
            {
                // DrawIcon(PSI.PSI.PSIMaterials[Icons.Idle].mainTexture as Texture2D, ref vector, "ActivityIconIdle".Translate());
                this.DrawIcon(Icon_Idle, ref vector, "ActivityIconIdle".Translate());
            }

            if (colonist.IsBurning())
            {
                this.DrawIcon(Icon_Burning, ref vector, "ActivityIconBurning".Translate());
            }
        }

        private void DrawSelectionOverlayOnGUI(Pawn colonist, Rect rect)
        {
            Thing obj = colonist;
            if (colonist.Dead)
            {
                obj = colonist.Corpse;
            }

            float num = 0.4f * ColonistBar_KF.Scale;
            Vector2 textureSize = new Vector2(
                                      SelectionDrawerUtility.SelectedTexGUI.width * num,
                                      SelectionDrawerUtility.SelectedTexGUI.height * num);
            SelectionDrawerUtility.CalculateSelectionBracketPositionsUI(
                bracketLocs,
                obj,
                rect,
                SelectionDrawer.SelectTimes,
                textureSize,
                ColBarSettings.BaseSizeFloat * 0.4f * ColonistBar_KF.Scale);
            this.DrawSelectionOverlayOnGUI(bracketLocs, num);
        }

        private void DrawSelectionOverlayOnGUI(Vector2[] bracketLocs, float selectedTexScale)
        {
            int num = 90;
            for (int i = 0; i < 4; i++)
            {
                Widgets.DrawTextureRotated(bracketLocs[i], SelectionDrawerUtility.SelectedTexGUI, num, selectedTexScale);
                num += 90;
            }
        }

        private void DrawWeaponIcon(Rect rect, Pawn colonist)
        {
            float entryRectAlpha = ColonistBar_KF.GetEntryRectAlpha(rect);
            this.ApplyEntryInAnotherMapAlphaFactor(colonist.Map, ref entryRectAlpha);
            Color color = new Color(1f, 1f, 1f, entryRectAlpha);
            GUI.color = color;
            if (colonist.equipment.Primary != null)
            {
                ThingWithComps thing = colonist.equipment.Primary;
                Rect rect2 = rect.ContractedBy(rect.width / 3);

                rect2.x = rect.xMax - rect2.width - rect.width / 12;
                rect2.y = rect.yMax - rect2.height - rect.height / 12;

                GUI.color = color;
                if (!thing.def.uiIconPath.NullOrEmpty())
                {
                    resolvedIcon = thing.def.uiIcon;
                }
                else
                {
                    resolvedIcon = thing.Graphic.ExtractInnerGraphicFor(thing).MatSingle.mainTexture as Texture2D;
                }

                // color labe by thing
                if (thing.def.IsMeleeWeapon)
                {
                    GUI.color = new Color(ColVermillion.r, ColVermillion.g, ColVermillion.b, entryRectAlpha);
                }

                if (thing.def.IsRangedWeapon)
                {
                    GUI.color = new Color(ColBlue.r, ColBlue.g, ColBlue.b, entryRectAlpha);
                }

                Color iconcolor = new Color(0.5f, 0.5f, 0.5f, 0.8f * entryRectAlpha);
                Widgets.DrawBoxSolid(rect2, iconcolor);
                Widgets.DrawBox(rect2);
                GUI.color = color;
                Rect rect3 = rect2.ContractedBy(rect2.width / 8);

                Widgets.DrawTextureRotated(rect3, resolvedIcon, 0);

                if (Mouse.IsOver(rect2))
                {
                    GUI.color = HighlightColor;
                    GUI.DrawTexture(rect2, TexUI.HighlightTex);
                }

                TooltipHandler.TipRegion(rect2, thing.def.LabelCap);
            }
        }

        private Rect GetPawnTextureRect(float x, float y)
        {
            Vector2 size = PawnTextureSize * ColonistBar_KF.Scale;

            return new Rect(x + 1f, y - (size.y - ColonistBar_KF.PawnSize.y) - 1f, size.x, size.y);
        }

        // RimWorld.ColonistBarColonistDrawer
        private Rect GroupFrameRect(int group)
        {
            float pos_x = 99999f;
            float pos_y = 21f;
            float num2 = 0f;
            float height = 0f;
            List<ColonistBar.Entry> entries = ColonistBar_KF.BarHelperKf.Entries;
            List<Vector2> drawLocs = ColonistBar_KF.BarHelperKf.DrawLocs;
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].group == group)
                {
                    pos_x = Mathf.Min(pos_x, drawLocs[i].x);
                    num2 = Mathf.Max(num2, drawLocs[i].x + ColonistBar_KF.FullSize.x);
                    height = Mathf.Max(height, drawLocs[i].y + ColonistBar_KF.FullSize.y);
                }
            }

            if (ColBarSettings.UseCustomMarginTop)
            {
                pos_y = ColBarSettings.MarginTop;
                height -= ColBarSettings.MarginTop;
            }

            height += ColonistBar_KF.SpacingLabel;

            return new Rect(pos_x, pos_y, num2 - pos_x, height).ContractedBy(-12f * ColonistBar_KF.Scale);
        }
    }
}