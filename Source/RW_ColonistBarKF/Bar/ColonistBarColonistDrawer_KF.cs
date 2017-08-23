namespace ColonistBarKF.Bar
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using ColonistBarKF.Menus;
    using ColonistBarKF.PSI;
    using FacialStuff.Detouring;

    using Harmony;

    using RimWorld;
    using RimWorld.Planet;

    using static Settings;

    using static SettingsColonistBar;

    using UnityEngine;

    using Verse;
    using Verse.AI.Group;
    using Verse.Sound;

    [StaticConstructorOnStartup]
    public class ColonistBarColonistDrawer_KF
    {
        #region Private Fields

        private static readonly Vector2[] bracketLocs = new Vector2[4];

        private static readonly Color HighlightColor = new Color(0.5f, 0.5f, 0.5f, 1f);

        private static Vector3 pawnTextureCameraOffset;

        private readonly Dictionary<string, string> pawnLabelsCache = new Dictionary<string, string>();

        #endregion Private Fields

        #region Private Properties

        private static Vector3 PawnTextureCameraOffset
        {
            get
            {
                float pawnTextureCameraOffsetNew = ColBarSettings.PawnTextureCameraZoom / 1.28205f;
                float posx = ColBarSettings.PawnTextureCameraHorizontalOffset / pawnTextureCameraOffsetNew;
                float posz = ColBarSettings.PawnTextureCameraVerticalOffset / pawnTextureCameraOffsetNew;
                pawnTextureCameraOffset = new Vector3(posx, 0f, posz);
                return pawnTextureCameraOffset;
            }
        }

        private static Vector2 PawnTextureSize => new Vector2(
            ColBarSettings.BaseSizeFloat - 2f,
            ColBarSettings.BaseSizeFloat * 1.5f);

        [CanBeNull]
        private static Pawn SelPawn => Find.Selector.SingleSelectedThing as Pawn;

        #endregion Private Properties

        #region Public Methods

        public void DrawColonist(Rect outerRect, [NotNull] Pawn colonist, [CanBeNull] Map pawnMap)
        {
            CompPSI psiComp = colonist.GetComp<CompPSI>();
            Rect pawnRect = new Rect(outerRect.x, outerRect.y, ColonistBar_KF.PawnSize.x, ColonistBar_KF.PawnSize.y);

            // if (pawnStats.IconCount == 0)
            // outerRect.width
            float entryRectAlpha = ColonistBar_KF.GetEntryRectAlpha(outerRect);
            this.ApplyEntryInAnotherMapAlphaFactor(pawnMap, outerRect, ref entryRectAlpha);

            bool colonistAlive = !colonist.Dead
                                     ? Find.Selector.SelectedObjects.Contains(colonist)
                                     : Find.Selector.SelectedObjects.Contains(colonist.Corpse);

            Color color = new Color(1f, 1f, 1f, entryRectAlpha);
            GUI.color = color;

            // testing
            // Widgets.DrawBox(outerRect);
            if (psiComp != null)
            {
                BuildRects(
                    psiComp.thisColCount,
                    ref outerRect,
                    ref pawnRect,
                    out Rect moodBorderRect,
                    out Rect psiRect);

                // Widgets.DrawBoxSolid(outerRect, new Color(0.5f, 1f, 0.5f, 0.5f));
                Color background = color;
                Texture2D tex2 = ColonistBarTextures.BgTexVanilla;
                if (ColBarSettings.UseGender)
                {
                    background = psiComp.BGColor;
                    tex2 = ColonistBarTextures.BgTexGrey;
                    background.a = entryRectAlpha;
                    GUI.color = background;
                }

                GUI.DrawTexture(pawnRect, tex2);

                GUI.color = color;

                if (colonist.needs?.mood != null)
                {
                    if (ColBarSettings.UseExternalMoodBar || ColBarSettings.UseNewMood)
                    {
                        if (psiComp.Mood != null && psiComp.Mb != null)
                        {
                            // string tooltip = colonist.needs.mood.GetTipString();
                            DrawNewMoodRect(moodBorderRect, psiComp.Mood, psiComp.Mb);
                        }
                    }
                    else
                    {
                        Rect position = pawnRect.ContractedBy(2f);
                        float num = position.height * colonist.needs.mood.CurLevelPercentage;
                        position.yMin = position.yMax - num;
                        position.height = num;
                        GUI.DrawTexture(position, ColonistBarTextures.VanillaMoodBgTex);
                    }
                }

                // PSI
                if (ColBarSettings.UsePsi)
                {
                    GameComponentPSI.DrawColonistIconsBar(colonist, psiRect, entryRectAlpha);
                }
            }
            else
            {
                GUI.color = color;
                GUI.DrawTexture(pawnRect, ColonistBarTextures.BgTexVanilla);
            }

            GUI.color = color;

            // Rect rect2 = outerRect.ContractedBy(-2f * ColonistBar_KF.Scale);
            Rect rect2 = outerRect.ContractedBy(-2f);

            if (colonistAlive && !WorldRendererUtility.WorldRenderedNow)
            {
                if (FollowMe.CurrentlyFollowing)
                {
                    Color col = ColonistBarTextures.ColBlueishGreen;

                    Pawn follow = FollowMe._followedThing as Pawn;
                    if (follow != null)
                    {
                        if (follow == colonist)
                        {
                            col = ColonistBarTextures.ColSkyBlue;
                        }
                    }

                    col.a = color.a;
                    GUI.color = col;
                }

                this.DrawSelectionOverlayOnGUI(colonist, rect2);
            }
            else if (WorldRendererUtility.WorldRenderedNow && colonist.IsCaravanMember()
                     && Find.WorldSelector.IsSelected(colonist.GetCaravan()))
            {
                this.DrawCaravanSelectionOverlayOnGUI(colonist.GetCaravan(), rect2);
            }

            GUI.color = color;

            GUI.DrawTexture(
                this.GetPawnTextureRect(pawnRect.x, pawnRect.y),
                PortraitsCache.Get(
                    colonist,
                    PawnTextureSize,
                    PawnTextureCameraOffset,
                    ColBarSettings.PawnTextureCameraZoom));
            if (colonist.CurJob != null)
            {
                DrawCurrentJobTooltip(colonist, pawnRect);
            }

            if (ColBarSettings.UseWeaponIcons)
            {
                this.DrawWeaponIcon(pawnRect, entryRectAlpha, colonist);
            }

            GUI.color = new Color(1f, 1f, 1f, entryRectAlpha * 0.8f);
            this.DrawIcons(pawnRect, colonist);
            GUI.color = color;
            if (colonist.Dead)
            {
                GUI.DrawTexture(pawnRect, ColonistBarTextures.DeadColonistTex);
            }

            // float num = 4f * Scale;
            Vector2 pos = new Vector2(pawnRect.center.x, pawnRect.yMax + 1f * ColonistBar_KF.Scale);
            GenMapUI.DrawPawnLabel(colonist, pos, entryRectAlpha, pawnRect.width, this.pawnLabelsCache);

            GUI.color = Color.white;
        }

        public void DrawEmptyFrame(Rect outerRect, [CanBeNull] Map pawnMap, int groupCount)
        {
            Rect pawnRect = new Rect(outerRect.x, outerRect.y, ColonistBar_KF.PawnSize.x, ColonistBar_KF.PawnSize.y);
            pawnRect.x += (outerRect.width - pawnRect.width) / 2;

            // if (pawnStats.IconCount == 0)
            // outerRect.width
            float entryRectAlpha = ColonistBar_KF.GetEntryRectAlpha(outerRect);
            this.ApplyEntryInAnotherMapAlphaFactor(pawnMap, outerRect, ref entryRectAlpha);

            Color color = new Color(1f, 1f, 1f, entryRectAlpha);
            GUI.color = color;

            // testing
            // Widgets.DrawBox(outerRect);

            // Widgets.DrawBoxSolid(outerRect, new Color(0.5f, 1f, 0.5f, 0.5f));
            GUI.DrawTexture(pawnRect, ColonistBarTextures.BgTexGrey);
            GUI.color = color;

            GUI.color = color;
            GUI.Label(pawnRect, groupCount.ToString() + " in group");
            outerRect = pawnRect;

            GUI.color = Color.white;
        }

        public void DrawGroupFrame(int group)
        {
            Rect position = this.GroupFrameRect(group);
            List<EntryKF> entries = ColonistBar_KF.BarHelperKf.Entries;
            Map map = entries.Find(x => x.group == group).map;
            float num;
            Color color = new Color(0.5f, 0.5f, 0.5f, 0.4f);

            bool flag = Mouse.IsOver(position);

            // Caravan on world map
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

                if (ColBarSettings.UseGroupColors)
                {
                    color = new Color(0.2f, 0.5f, 0.47f, 0.4f);
                }
            }
            else
            {
                // other pawns, on map
                if (map != Find.VisibleMap || WorldRendererUtility.WorldRenderedNow)
                {
                    num = 0.75f;
                }
                else
                {
                    num = 1f;
                }

                if (ColBarSettings.UseGroupColors && !map.IsPlayerHome)
                {
                    color = new Color(0.2f, 0.25f, 0.5f, 0.4f);
                }
            }

            if (flag && num < 1f)
            {
                num = 1f;
            }

            color.a *= num;
            Widgets.DrawRectFast(position, color);
        }

        public void HandleClicks(Rect rect, [CanBeNull] Pawn colonist, int showThisMap)
        {
            if (Mouse.IsOver(rect) && Event.current.type == EventType.MouseDown)
            {
                switch (Event.current.button)
                {
                    // Left Mouse Button
                    case 0:
                        {
                            if (Event.current.clickCount == 1)
                            {
                                // Single click on "more colonists"
                                if (colonist == null)
                                {
                                    // use event so it doesn't bubble through
                                    Event.current.Use();
                                    ColonistBar_KF.BarHelperKf.displayGroupForBar = showThisMap;
                                    HarmonyPatches.MarkColonistsDirty_Postfix();
                                }
                            }

                            if (Event.current.clickCount == 2)
                            {
                                // Double click
                                // use event so it doesn't bubble through
                                Event.current.Use();
                                bool flag = false;
                                if (colonist == null)
                                {
                                }
                                else
                                {
                                    if (FollowMe.CurrentlyFollowing)
                                    {
                                        FollowMe.StopFollow("Selected another colonist on bar");
                                        if (colonist?.Map != null)
                                        {
                                            FollowMe.TryStartFollow(colonist);
                                        }
                                        else
                                        {
                                            flag = true;
                                        }
                                    }
                                    else
                                    {
                                        flag = true;
                                    }

                                    if (flag)
                                    {
                                        CameraJumper.TryJump(colonist);
                                    }
                                }

                                // clickedColonist = null;
                            }

                            // if (Event.current.clickCount == 1)
                            // {
                            // clickedColonist = colonist;
                            // }
                            break;
                        }

                    // Right Mouse Button
                    case 1:
                        List<FloatMenuOption> choicesList = new List<FloatMenuOption>();
                        List<FloatMenuOption> fluffyStart = new List<FloatMenuOption>();
                        List<FloatMenuOption> fluffyStop = new List<FloatMenuOption>();

                        if (colonist != null && SelPawn != null && SelPawn != colonist && SelPawn.Map != null
                            && colonist.Map == SelPawn.Map && SelPawn.IsColonistPlayerControlled)
                        {
                            foreach (FloatMenuOption choice in FloatMenuMakerMap.ChoicesAtFor(
                                colonist.TrueCenter(),
                                SelPawn))
                            {
                                choicesList.Add(choice);

                                // floatOptionList.Add(choice);
                            }
                        }

                        if (colonist?.Map != null)
                        {
                            FloatMenuOption fluffyStopAction;

                            FloatMenuOption fluffyFollowAction = new FloatMenuOption(
                                "FollowMe.StartFollow".Translate() + " - " + colonist.LabelShort,
                                delegate { FollowMe.TryStartFollow(colonist); });

                            bool flag = !FollowMe.CurrentlyFollowing
                                        || FollowMe.CurrentlyFollowing && FollowMe._followedThing != colonist;
                            if (flag)
                            {
                                fluffyStart.Add(fluffyFollowAction);

                                // foreach (var pawn in colonist.Map.mapPawns.FreeColonistsSpawned.OrderBy(pawn => pawn.LabelShort))
                                // {
                                // if (pawn != colonist)
                                // {
                                // FloatMenuOption fluffyFollowAlsoAction = new FloatMenuOption(
                                // "FollowMe.StartFollow".Translate() + " - " + pawn.LabelShort,
                                // delegate { FollowMe.TryStartFollow(pawn); });
                                // fluffyStart.Add(fluffyFollowAlsoAction);
                                // }
                                // }
                            }

                            if (FollowMe.CurrentlyFollowing)
                            {
                                fluffyStopAction = new FloatMenuOption(
                                    "FollowMe.StopFollow".Translate() + " - " + FollowMe._followedThing.LabelShort,
                                    delegate { FollowMe.StopFollow("Canceled in dropdown"); });

                                fluffyStop.Add(fluffyStopAction);
                            }
                        }

                        this.GetSortList(out List<FloatMenuOption> sortList);

                        // this.GetSortExtraList(out List<FloatMenuOption> extraSortList);
                        Dictionary<string, List<FloatMenuOption>> labeledSortingActions =
                            new Dictionary<string, List<FloatMenuOption>>();

                        FloatMenuOption options = new FloatMenuOption(
                            "CBKF.Settings.SettingsColonistBar".Translate(),
                            delegate { Find.WindowStack.Add(new ColonistBarKfSettings()); });

                        List<FloatMenuOption> floatOptionList = new List<FloatMenuOption>
                        {
                            options
                        };

                        if (!fluffyStart.NullOrEmpty())
                        {
                            labeledSortingActions.Add(fluffyStart[0].Label, fluffyStart);
                        }

                        if (!fluffyStop.NullOrEmpty())
                        {
                            labeledSortingActions.Add(fluffyStop[0].Label, fluffyStop);
                        }

                        if (!choicesList.NullOrEmpty())
                        {
                            labeledSortingActions.Add(
                                "CBKF.Settings.ChoicesForPawn".Translate(SelPawn, colonist),
                                choicesList);
                        }

                        labeledSortingActions.Add("CBKF.Settings.OrderingOptions".Translate(), sortList);

                        // labeledSortingActions.Add("CBKF.Settings.AllStatsSortingOptions".Translate(), extraSortList);
                        labeledSortingActions.Add("CBKF.Settings.SettingsColonistBar".Translate(), floatOptionList);

                        List<FloatMenuOption> items = labeledSortingActions.Keys.Select(
                            label =>
                                {
                                    List<FloatMenuOption> fmo = labeledSortingActions[label];
                                    return Tools.MakeMenuItemForLabel(label, fmo);
                                }).ToList();

                        Tools.labelMenu = new FloatMenuLabels(items);
                        Find.WindowStack.Add(Tools.labelMenu);

                        // use event so it doesn't bubble through
                        Event.current.Use();
                        break;
                }

                // Middle Mouse Button
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

        private void GetSortList([NotNull] out List<FloatMenuOption> sortList)
        {
            sortList = new List<FloatMenuOption>();

            FloatMenuOption sortByVanilla = new FloatMenuOption(
                "CBKF.Settings.Vanilla".Translate(),
                delegate
                    {
                        ColBarSettings.SortBy = SortByWhat.vanilla;
                        HarmonyPatches.MarkColonistsDirty_Postfix();
                    });
            FloatMenuOption sortByWeapons = new FloatMenuOption(
                "CBKF.Settings.Weapons".Translate(),
                delegate
                    {
                        ColBarSettings.SortBy = SortByWhat.weapons;
                        HarmonyPatches.MarkColonistsDirty_Postfix();

                    });
            FloatMenuOption sortByName = new FloatMenuOption(
                "CBKF.Settings.ByName".Translate(),
                delegate
                    {
                        ColBarSettings.SortBy = SortByWhat.byName;
                        HarmonyPatches.MarkColonistsDirty_Postfix();

                    });

            FloatMenuOption sortbySexAge = new FloatMenuOption(
                "CBKF.Settings.SexAge".Translate(),
                delegate
                    {
                        ColBarSettings.SortBy = SortByWhat.sexage;
                        HarmonyPatches.MarkColonistsDirty_Postfix();
                    });
            FloatMenuOption sortByMood = new FloatMenuOption(
                "CBKF.Settings.Mood".Translate(),
                delegate
                    {
                        ColBarSettings.SortBy = SortByWhat.mood;
                        HarmonyPatches.MarkColonistsDirty_Postfix();

                        // CheckRecacheEntries();
                    });
            FloatMenuOption sortByHealth = new FloatMenuOption(
                "CBKF.Settings.Health".Translate(),
                delegate
                    {
                        ColBarSettings.SortBy = SortByWhat.health;
                        HarmonyPatches.MarkColonistsDirty_Postfix();

                        // CheckRecacheEntries();
                    });

            FloatMenuOption sortByMedic = new FloatMenuOption(
                StatDefOf.MedicalTendQuality.LabelCap,
                delegate
                    {
                        ColBarSettings.SortBy = SortByWhat.medicTendQuality;
                        HarmonyPatches.MarkColonistsDirty_Postfix();
                    });
            FloatMenuOption sortByMedic2 = new FloatMenuOption(
                StatDefOf.MedicalSurgerySuccessChance.LabelCap,
                delegate
                    {
                        ColBarSettings.SortBy = SortByWhat.medicSurgerySuccess;
                        HarmonyPatches.MarkColonistsDirty_Postfix();
                    });

            FloatMenuOption sortByTrade = new FloatMenuOption(
                StatDefOf.TradePriceImprovement.LabelCap,
                delegate
                    {
                        ColBarSettings.SortBy = SortByWhat.tradePrice;
                        HarmonyPatches.MarkColonistsDirty_Postfix();
                    });
            sortList.Add(sortByVanilla);
            sortList.Add(sortByWeapons);
            sortList.Add(sortByName);
            sortList.Add(sortbySexAge);
            sortList.Add(sortByMood);
            sortList.Add(sortByHealth);
            sortList.Add(sortByMedic);
            sortList.Add(sortByMedic2);
            sortList.Add(sortByTrade);

        }

        // RimWorld.ColonistBarColonistDrawer
        public void HandleGroupFrameClicks(int group)
        {
            Rect rect = this.GroupFrameRect(group);

            // Using Mouse Down instead of Up to not interfere with HandleClicks
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Mouse.IsOver(rect) && Event.current.clickCount == 1)
            {
                bool worldRenderedNow = WorldRendererUtility.WorldRenderedNow;
                EntryKF entry = ColonistBar_KF.BarHelperKf.Entries.Find(x => x.group == group);
                Map map = entry.map;

                if (!ColonistBar_KF.BarHelperKf.AnyBarEntryAt(UI.MousePositionOnUIInverted))
                {
                    if (!worldRenderedNow && !Find.Selector.dragBox.IsValidAndActive
                        || worldRenderedNow && !Find.WorldSelector.dragBox.IsValidAndActive)
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
            // if (Event.current.button == 1 && Widgets.ButtonInvisible(rect, false))
            // {
            // ColonistBar.Entry entry2 = ColonistBar_KF.BarHelperKf.Entries.Find((ColonistBar.Entry x) => x.group == group);
            // if (entry2.map != null)
            // {
            // CameraJumper.TryJumpAndSelect(CameraJumper.GetWorldTargetOfMap(entry2.map));
            // }
            // else if (entry2.pawn != null)
            // {
            // CameraJumper.TryJumpAndSelect(entry2.pawn);
            // }
            // }
        }

        public void Notify_RecachedEntries()
        {
            this.pawnLabelsCache.Clear();
        }

        #endregion Public Methods

        #region Private Methods

        private static void BuildRects(
            int thisColCount,
            ref Rect outerRect,
            ref Rect pawnRect,
            out Rect moodRect,
            out Rect psiRect)
        {
            float widthMoodFloat = pawnRect.width;
            float heightMoodFloat = pawnRect.height;

            float modifier = 1;

            bool psiHorizontal = ColBarSettings.ColBarPsiIconPos == Position.Alignment.Left
                                 || ColBarSettings.ColBarPsiIconPos == Position.Alignment.Right;

            bool moodHorizontal = ColBarSettings.MoodBarPos == Position.Alignment.Left
                                  || ColBarSettings.MoodBarPos == Position.Alignment.Right;

            float widthPsiFloat;
            float heightPsiFloat;
            float heightFullPsiFloat;

            if (psiHorizontal)
            {
                widthPsiFloat = ColonistBar_KF.WidthPSIHorizontal * ColonistBar_KF.Scale;
                heightPsiFloat = outerRect.height - ColonistBar_KF.SpacingLabel;
                heightFullPsiFloat = outerRect.height - ColonistBar_KF.SpacingLabel;
            }
            else
            {
                widthPsiFloat = outerRect.width;
                heightPsiFloat = ColonistBar_KF.HeightPSIVertical * ColonistBar_KF.Scale;
                heightFullPsiFloat = ColonistBar_KF.HeightPSIVertical * ColonistBar_KF.Scale;
            }

            if (ColBarSettings.UsePsi)
            {
                // If lesser rows, move the rect
                if (thisColCount < ColonistBar_KF.PsiRowsOnBar)
                {
                    CalculateSizePSI(thisColCount, modifier, psiHorizontal, ref widthPsiFloat, ref heightPsiFloat);
                }
            }

            if (ColBarSettings.UseExternalMoodBar)
            {
                if (moodHorizontal)
                {
                    widthMoodFloat /= 4;
                }
                else
                {
                    heightMoodFloat /= 4;
                }
            }

            psiRect = new Rect(outerRect.x, outerRect.y, widthPsiFloat, heightPsiFloat);

            // Widgets.DrawBoxSolid(psiRect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
            switch (ColBarSettings.ColBarPsiIconPos)
            {
                case Position.Alignment.Left:
                    pawnRect.x += widthPsiFloat;
                    break;

                case Position.Alignment.Right:
                    psiRect.x = pawnRect.xMax;
                    break;

                case Position.Alignment.Top:
                    pawnRect.y += heightFullPsiFloat;
                    psiRect.y += heightFullPsiFloat - heightPsiFloat;
                    break;

                case Position.Alignment.Bottom:
                    psiRect.y = pawnRect.yMax + ColonistBar_KF.SpacingLabel;
                    break;

                default: throw new ArgumentOutOfRangeException();
            }

            moodRect = new Rect(pawnRect.x, pawnRect.y, widthMoodFloat, heightMoodFloat);

            if (ColBarSettings.UseExternalMoodBar)
            {
                switch (ColBarSettings.MoodBarPos)
                {
                    case Position.Alignment.Left:
                        pawnRect.x += widthMoodFloat;
                        if (ColBarSettings.ColBarPsiIconPos != Position.Alignment.Left)
                        {
                            psiRect.x += widthMoodFloat;
                        }

                        if (!psiHorizontal)
                        {
                            psiRect.width -= widthMoodFloat;
                        }

                        break;

                    case Position.Alignment.Right:
                        moodRect.x = pawnRect.xMax;
                        psiRect.x += ColBarSettings.ColBarPsiIconPos == Position.Alignment.Right ? widthMoodFloat : 0f;
                        break;

                    case Position.Alignment.Top:
                        pawnRect.y += heightMoodFloat;
                        psiRect.y += ColBarSettings.ColBarPsiIconPos == Position.Alignment.Bottom
                                         ? heightMoodFloat
                                         : 0f;
                        break;

                    case Position.Alignment.Bottom:
                        moodRect.y = pawnRect.yMax + ColonistBar_KF.SpacingLabel;
                        psiRect.y += ColBarSettings.ColBarPsiIconPos == Position.Alignment.Bottom
                                         ? heightMoodFloat
                                         : 0f;
                        if (psiHorizontal)
                        {
                            psiRect.height -= heightMoodFloat;
                        }

                        break;

                    default: throw new ArgumentOutOfRangeException();
                }
            }

            float offsetX = outerRect.x - Mathf.Min(psiRect.x, moodRect.x, pawnRect.x);
            offsetX += outerRect.xMax - Mathf.Max(psiRect.xMax, moodRect.xMax, pawnRect.xMax);
            offsetX /= 2;

            float height = Mathf.Max(psiRect.yMax, moodRect.yMax, pawnRect.yMax);

            psiRect.x += offsetX;
            moodRect.x += offsetX;
            pawnRect.x += offsetX;

            outerRect.x += offsetX;
            outerRect.width -= offsetX * 2;
            outerRect.yMax =
                ColBarSettings.ColBarPsiIconPos == Position.Alignment.Bottom
                || ColBarSettings.MoodBarPos == Position.Alignment.Bottom
                    ? height
                    : height + ColonistBar_KF.SpacingLabel;
        }

        private static void CalculateSizePSI(
            int thisColCount,
            float modifier,
            bool psiHorizontal,
            ref float widthPsiFloat,
            ref float heightPsiFloat)
        {
            switch (thisColCount)
            {
                case 0:
                    modifier = 0f;
                    break;

                case 1:
                    modifier = 0.5f;
                    break;

                default: break;
            }

            if (psiHorizontal)
            {
                widthPsiFloat *= modifier;
            }
            else
            {
                heightPsiFloat *= modifier;
            }
        }

        private static void DrawCurrentJobTooltip([NotNull] Pawn colonist, Rect pawnRect)
        {
            string jobDescription = null;
            Lord lord = colonist.GetLord();
            if (lord?.LordJob != null)
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

        private static void DrawCurrentMood(
            Rect moodRect,
            [NotNull] Texture2D moodTex,
            float moodPercent,
            [NotNull] Need mood,
            out Rect rect1,
            out Rect rect2)
        {
            float x = moodRect.x + moodRect.width * mood.CurInstantLevelPercentage;
            float y = moodRect.yMax - moodRect.height * mood.CurInstantLevelPercentage;
            rect1 = new Rect(moodRect.x, y, moodRect.width, 1);
            rect2 = new Rect(moodRect.xMax + 1, y - 1, 2, 3);

            if (ColBarSettings.UseExternalMoodBar)
            {
                switch (ColBarSettings.MoodBarPos)
                {
                    default:
                        GUI.DrawTexture(moodRect.BottomPart(moodPercent), moodTex);
                        break;

                    case Position.Alignment.Top:
                        rect1 = new Rect(x, moodRect.y, 1, moodRect.height);
                        rect2 = new Rect(x - 1, moodRect.yMin - 1, 3, 2);
                        GUI.DrawTexture(moodRect.LeftPart(moodPercent), moodTex);
                        break;

                    case Position.Alignment.Bottom:
                        rect1 = new Rect(x, moodRect.y, 1, moodRect.height);
                        rect2 = new Rect(x - 1, moodRect.yMax + 1, 3, 2);
                        GUI.DrawTexture(moodRect.LeftPart(moodPercent), moodTex);
                        break;
                }
            }
            else
            {
                GUI.DrawTexture(moodRect.BottomPart(moodPercent), moodTex);
            }
        }

        private static void DrawMentalThreshold(Rect moodRect, float threshold)
        {
            if (ColBarSettings.UseExternalMoodBar && (ColBarSettings.MoodBarPos == Position.Alignment.Top
                                                      || ColBarSettings.MoodBarPos == Position.Alignment.Bottom))
            {
                GUI.DrawTexture(
                    new Rect(moodRect.x + moodRect.width * threshold, moodRect.y, 1, moodRect.height),
                    ColonistBarTextures.MoodBreakTex);
            }
            else
            {
                GUI.DrawTexture(
                    new Rect(moodRect.x, moodRect.yMax - moodRect.height * threshold, moodRect.width, 1),
                    ColonistBarTextures.MoodBreakTex);
            }

            /*if (currentMood <= threshold)
            {
                GUI.DrawTexture(new Rect(moodRect.xMax-4, moodRect.yMax - moodRect.height * threshold, 8, 2), MoodBreakCrossedTex);
            }*/
        }

        private static void DrawNewMoodRect(Rect moodBorderRect, [NotNull] Need mood, [NotNull] MentalBreaker mb)
        {
            Rect moodRect = moodBorderRect.ContractedBy(2f);

            Color color = GUI.color;
            Color moodCol;

            Color critColor = Color.clear;
            bool showCritical = false;

            float moodPercent;
            float curMood = mood.CurLevelPercentage;

            GUI.DrawTexture(moodBorderRect, ColonistBarTextures.MoodBgTex);

            if (curMood > mb.BreakThresholdMinor)
            {
                moodPercent = Mathf.InverseLerp(mb.BreakThresholdMinor, 1f, curMood);
                moodCol = ColonistBarTextures.ColBlue;
                if (moodPercent < 0.3f)
                {
                    critColor = Color.Lerp(
                        ColonistBarTextures.ColorNeutralSoft,
                        ColonistBarTextures.ColorNeutralStatus,
                        Mathf.InverseLerp(0.3f, 0f, moodPercent));
                    critColor *= ColonistBarTextures.ColYellow;
                    showCritical = true;
                }
            }
            else if (curMood > mb.BreakThresholdMajor)
            {
                moodPercent = Mathf.InverseLerp(mb.BreakThresholdMajor, mb.BreakThresholdMinor, curMood);
                moodCol = ColonistBarTextures.ColYellow;
                if (moodPercent < 0.4f)
                {
                    critColor = Color.Lerp(
                        ColonistBarTextures.ColorNeutralSoft,
                        ColonistBarTextures.ColorNeutralStatus,
                        Mathf.InverseLerp(0.4f, 0f, moodPercent));
                    critColor *= ColonistBarTextures.ColOrange;
                    showCritical = true;
                }
            }
            else if (curMood > mb.BreakThresholdExtreme)
            {
                moodPercent = Mathf.InverseLerp(mb.BreakThresholdExtreme, mb.BreakThresholdMajor, curMood);
                moodCol = ColonistBarTextures.ColOrange;
                if (moodPercent < 0.5f)
                {
                    critColor = Color.Lerp(
                        ColonistBarTextures.ColorNeutralSoft,
                        ColonistBarTextures.ColorNeutralStatus,
                        Mathf.InverseLerp(0.5f, 0f, moodPercent));
                    critColor *= ColonistBarTextures.ColVermillion;
                    showCritical = true;
                }
            }
            else
            {
                // moodPercent = mb.BreakThresholdExtreme > 0.01f ? Mathf.InverseLerp(0f, mb.BreakThresholdExtreme, curMood) : 1f;
                moodPercent = 1f;
                moodCol = ColonistBarTextures.ColVermillion;
            }

            moodCol.a = color.a;

            GUI.color = moodCol;
            GUI.DrawTexture(moodRect, ColonistBarTextures.MoodNeutralBgTex);
            if (showCritical)
            {
                critColor.a *= color.a;
                GUI.color = critColor;
                GUI.DrawTexture(moodRect, ColonistBarTextures.MoodNeutralTex);
                GUI.color = moodCol;
            }

            DrawCurrentMood(
                moodRect,
                ColonistBarTextures.MoodNeutralTex,
                moodPercent,
                mood,
                out Rect rect1,
                out Rect rect2);
            GUI.color = color;

            DrawMentalThreshold(moodRect, mb.BreakThresholdExtreme);
            DrawMentalThreshold(moodRect, mb.BreakThresholdMajor);
            DrawMentalThreshold(moodRect, mb.BreakThresholdMinor);

            GUI.DrawTexture(rect1, ColonistBarTextures.MoodTargetTex);
            GUI.DrawTexture(rect2, ColonistBarTextures.MoodTargetTex);

            // TooltipHandler.TipRegion(moodRect, tooltip);
        }

        private void ApplyEntryInAnotherMapAlphaFactor([CanBeNull] Map map, Rect rect, ref float alpha)
        {
            bool flag = Mouse.IsOver(rect);

            if (map == null)
            {
                if (!WorldRendererUtility.WorldRenderedNow)
                {
                    alpha = Mathf.Min(alpha, flag ? 1f : 0.4f);
                }
            }
            else if (map != Find.VisibleMap || WorldRendererUtility.WorldRenderedNow)
            {
                alpha = Mathf.Min(alpha, flag ? 1f : 0.4f);
            }
        }

        private void DrawCaravanSelectionOverlayOnGUI([NotNull] Caravan caravan, Rect rect)
        {
            float num = 0.4f * ColonistBar_KF.Scale;
            float x = SelectionDrawerUtility.SelectedTexGUI.width * num;
            float y = SelectionDrawerUtility.SelectedTexGUI.height * num;
            Vector2 textureSize = new Vector2(x, y);
            SelectionDrawerUtility.CalculateSelectionBracketPositionsUI(
                bracketLocs,
                caravan,
                rect,
                WorldSelectionDrawer.SelectTimes,
                textureSize,
                ColBarSettings.BaseSizeFloat * ColonistBar_KF.Scale);
            this.DrawSelectionOverlayOnGUI(bracketLocs, num);
        }

        private void DrawIcon([NotNull] Texture2D icon, ref Vector2 pos, [NotNull] string tooltip)
        {
            float num = ColBarSettings.BaseSizeFloat * 0.4f * ColonistBar_KF.Scale;
            Rect rect = new Rect(pos.x, pos.y, num, num);
            GUI.DrawTexture(rect, icon);
            TooltipHandler.TipRegion(rect, tooltip);
            pos.x += num;
        }

        private void DrawIcons(Rect rect, [JetBrains.Annotations.NotNull] Pawn colonist)
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
                this.DrawIcon(ColonistBarTextures.IconMentalStateAggro, ref vector, colonist.MentalStateDef.LabelCap);
            }
            else if (colonist.InMentalState)
            {
                // DrawIcon(PSI.PSI.PSIMaterials[Icons.Dazed].mainTexture as Texture2D, ref vector, colonist.MentalStateDef.LabelCap);
                this.DrawIcon(
                    ColonistBarTextures.IconMentalStateNonAggro,
                    ref vector,
                    colonist.MentalStateDef.LabelCap);
            }
            else if (colonist.InBed() && colonist.CurrentBed().Medical)
            {
                // DrawIcon(PSI.PSI.PSIMaterials[Icons.Health].mainTexture as Texture2D, ref vector, "ActivityIconMedicalRest".Translate());
                this.DrawIcon(ColonistBarTextures.IconMedicalRest, ref vector, "ActivityIconMedicalRest".Translate());
            }
            else if (colonist.CurJob != null && colonist.jobs.curDriver.asleep)
            {
                // DrawIcon(PSI.PSI.PSIMaterials[Icons.Tired].mainTexture as Texture2D, ref vector, "ActivityIconSleeping".Translate());
                this.DrawIcon(ColonistBarTextures.IconSleeping, ref vector, "ActivityIconSleeping".Translate());
            }
            else if (colonist.CurJob != null && colonist.CurJob.def == JobDefOf.FleeAndCower)
            {
                // DrawIcon(PSI.PSI.PSIMaterials[Icons.Leave].mainTexture as Texture2D, ref vector, "ActivityIconFleeing".Translate());
                this.DrawIcon(ColonistBarTextures.IconFleeing, ref vector, "ActivityIconFleeing".Translate());
            }
            else if (attacking)
            {
                this.DrawIcon(ColonistBarTextures.IconAttacking, ref vector, "ActivityIconAttacking".Translate());
            }
            else if (colonist.mindState.IsIdle && GenDate.DaysPassed >= 0.1)
            {
                // DrawIcon(PSI.PSI.PSIMaterials[Icons.Idle].mainTexture as Texture2D, ref vector, "ActivityIconIdle".Translate());
                this.DrawIcon(ColonistBarTextures.IconIdle, ref vector, "ActivityIconIdle".Translate());
            }

            if (colonist.IsBurning())
            {
                this.DrawIcon(ColonistBarTextures.IconBurning, ref vector, "ActivityIconBurning".Translate());
            }
        }

        private void DrawSelectionOverlayOnGUI([NotNull] Pawn colonist, Rect rect)
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
                ColBarSettings.BaseSizeFloat * ColonistBar_KF.Scale);
            this.DrawSelectionOverlayOnGUI(bracketLocs, num);
        }

        private void DrawSelectionOverlayOnGUI([NotNull] Vector2[] bracketLocs, float selectedTexScale)
        {
            int num = 90;
            for (int i = 0; i < 4; i++)
            {
                Widgets.DrawTextureRotated(
                    bracketLocs[i],
                    SelectionDrawerUtility.SelectedTexGUI,
                    num,
                    selectedTexScale);
                num += 90;
            }
        }

        private void DrawWeaponIcon(Rect rect, float entryRectAlpha, [NotNull] Pawn colonist)
        {
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
                    ColonistBarTextures.ResolvedIcon = thing.def.uiIcon;
                }
                else
                {
                    ColonistBarTextures.ResolvedIcon =
                        thing.Graphic.ExtractInnerGraphicFor(thing).MatSingle.mainTexture as Texture2D;
                }

                Color weaponColor = new Color();

                // color labe by thing
                if (thing.def.IsMeleeWeapon)
                {
                    weaponColor = ColonistBarTextures.ColVermillion;
                    weaponColor.a = entryRectAlpha;
                    GUI.color = weaponColor;
                }

                if (thing.def.IsRangedWeapon)
                {
                    weaponColor = ColonistBarTextures.ColBlue;
                    weaponColor.a = entryRectAlpha;
                    GUI.color = weaponColor;
                }

                Color iconcolor = new Color(0.5f, 0.5f, 0.5f, 0.8f * entryRectAlpha);
                Widgets.DrawBoxSolid(rect2, iconcolor);
                Widgets.DrawBox(rect2);
                GUI.color = color;
                Rect rect3 = rect2.ContractedBy(rect2.width / 8);

                Widgets.DrawTextureRotated(rect3, ColonistBarTextures.ResolvedIcon, 0);

                // Not visible, deactivated
                // if (Mouse.IsOver(rect2))
                // {
                // GUI.color = HighlightColor;
                // GUI.DrawTexture(rect2, TexUI.HighlightTex);
                // }
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
            float posX = 99999f;
            float posY = 21f;
            float num2 = 0f;
            float height = 0f;
            List<EntryKF> entries = ColonistBar_KF.BarHelperKf.Entries;
            List<Vector2> drawLocs = ColonistBar_KF.BarHelperKf.DrawLocs;
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].group == group)
                {
                    posX = Mathf.Min(posX, drawLocs[i].x);
                    num2 = Mathf.Max(num2, drawLocs[i].x + ColonistBar_KF.FullSize.x);
                    height = Mathf.Max(height, drawLocs[i].y + ColonistBar_KF.FullSize.y);
                }
            }

            if (ColBarSettings.UseCustomMarginTop)
            {
                posY = ColBarSettings.MarginTop;
                height -= ColBarSettings.MarginTop;
            }

            height += ColonistBar_KF.SpacingLabel;

            return new Rect(posX, posY, num2 - posX, height).ContractedBy(-12f * ColonistBar_KF.Scale);
        }

        #endregion Private Methods
    }
}