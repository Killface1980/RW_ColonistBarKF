using RimWorld.Planet;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using static ColonistBarKF.CBKF;
using static ColonistBarKF.Position;
using static ColonistBarKF.SettingsColonistBar.SortByWhat;
namespace ColonistBarKF
{
    using RimWorld;

    [StaticConstructorOnStartup]
    public class ColonistBarColonistDrawerKF
    {
        private const float PawnTextureCameraZoom = 1.28205f;

        private const float PawnTextureHorizontalPadding = 1f;

        private const float BaseIconSize = 20f;

        private const float BaseGroupFrameMargin = 12f;

        public const float DoubleClickTime = 0.5f;

        private Dictionary<string, string> pawnLabelsCache = new Dictionary<string, string>();

        private Pawn clickedColonist;

        private float clickedAt;

        private static readonly Texture2D MoodBGTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.4f, 0.47f, 0.53f, 0.44f));

        private static readonly Texture2D DeadColonistTex = ContentFinder<Texture2D>.Get("UI/Misc/DeadColonist", true);

        private static readonly Texture2D Icon_MentalStateNonAggro = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/MentalStateNonAggro", true);

        private static readonly Texture2D Icon_MentalStateAggro = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/MentalStateAggro", true);

        private static readonly Texture2D Icon_MedicalRest = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/MedicalRest", true);

        private static readonly Texture2D Icon_Sleeping = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Sleeping", true);

        private static readonly Texture2D Icon_Fleeing = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Fleeing", true);

        private static readonly Texture2D Icon_Attacking = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Attacking", true);

        private static readonly Texture2D Icon_Idle = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Idle", true);

        private static readonly Texture2D Icon_Burning = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Burning", true);

        public static Vector2 PawnTextureSize = new Vector2(ColBarSettings.BaseSizeFloat - 2f, ColBarSettings.BaseSizeFloat * 1.5f);

        private static Vector3 _pawnTextureCameraOffset;

        public static Vector3 PawnTextureCameraOffset
        {
            get
            {
                float pawnTextureCameraOffsetNew = ColBarSettings.PawnTextureCameraZoom / 1.28205f;
                _pawnTextureCameraOffset = new Vector3(ColBarSettings.PawnTextureCameraHorizontalOffset / pawnTextureCameraOffsetNew, 0f, ColBarSettings.PawnTextureCameraVerticalOffset / pawnTextureCameraOffsetNew);
                return _pawnTextureCameraOffset;
            }

            set { _pawnTextureCameraOffset = value; }
        }
        private static Vector2[] bracketLocs = new Vector2[4];

        private ColonistBar ColonistBar
        {
            get
            {
                return Find.ColonistBar;
            }
        }

        public void DrawColonist(Rect rect, Pawn colonist, Map pawnMap)
        {
            float entryRectAlpha = ColonistBar.GetEntryRectAlpha(rect);
            ApplyEntryInAnotherMapAlphaFactor(pawnMap, ref entryRectAlpha);
            bool flag = (!colonist.Dead) ? Find.Selector.SelectedObjects.Contains(colonist) : Find.Selector.SelectedObjects.Contains(colonist.Corpse);
            Color color = new Color(1f, 1f, 1f, entryRectAlpha);
            GUI.color = color;
            GUI.DrawTexture(rect, ColonistBarTextures.BGTex);
            if (colonist.needs != null && colonist.needs.mood != null)
            {
                Rect position = rect.ContractedBy(2f);
                float num = position.height * colonist.needs.mood.CurLevelPercentage;
                position.yMin = position.yMax - num;
                position.height = num;
                GUI.DrawTexture(position, ColonistBarColonistDrawerKF.MoodBGTex);
            }
            Rect rect2 = rect.ContractedBy(-2f * ColonistBar.Scale);
            if (flag && !WorldRendererUtility.WorldRenderedNow)
            {
                DrawSelectionOverlayOnGUI(colonist, rect2);
            }
            else if (WorldRendererUtility.WorldRenderedNow && colonist.IsCaravanMember() && Find.WorldSelector.IsSelected(colonist.GetCaravan()))
            {
                DrawCaravanSelectionOverlayOnGUI(colonist.GetCaravan(), rect2);
            }
            GUI.DrawTexture(GetPawnTextureRect(rect.x, rect.y), PortraitsCache.Get(colonist, ColonistBarColonistDrawerKF.PawnTextureSize, ColonistBarColonistDrawerKF.PawnTextureCameraOffset, 1.28205f));
            //PSI
            if (ColBarSettings.UseWeaponIcons)
            {
                DrawWeapon(rect, colonist, entryRectAlpha);
            }
            if (ColBarSettings.UsePsi)
            {
                PSI.PSI.DrawColonistIconsOnBar(rect, colonist, entryRectAlpha);
            }
            //PSI end
            GUI.color = new Color(1f, 1f, 1f, entryRectAlpha * 0.8f);
            DrawIcons(rect, colonist);
            GUI.color = color;
            if (colonist.Dead)
            {
                GUI.DrawTexture(rect, ColonistBarColonistDrawerKF.DeadColonistTex);
            }
            float num2 = 4f * ColonistBar.Scale;
            Vector2 pos = new Vector2(rect.center.x, rect.yMax - num2);
            GenMapUI.DrawPawnLabel(colonist, pos, entryRectAlpha, rect.width + ColonistBar.SpaceBetweenColonistsHorizontal - 2f, pawnLabelsCache, GameFont.Tiny, true, true);
            Text.Font = GameFont.Small;
            GUI.color = Color.white;
        }

        private Rect GroupFrameRect(int group)
        {
            float num = 99999f;
            float num2 = 0f;
            float num3 = 0f;
            List<ColonistBar.Entry> entries = ColonistBar.Entries;
            List<Vector2> drawLocs = ColonistBar.DrawLocs;
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].group == group)
                {
                    num = Mathf.Min(num, drawLocs[i].x);
                    num2 = Mathf.Max(num2, drawLocs[i].x + ColonistBar.Size.x);
                    num3 = Mathf.Max(num3, drawLocs[i].y + ColonistBar.Size.y);
                }
            }
            return new Rect(num, 0f, num2 - num, num3).ContractedBy(-12f * ColonistBar.Scale);
        }

        public void DrawGroupFrame(int group)
        {
            Rect position = GroupFrameRect(group);
            List<ColonistBar.Entry> entries = ColonistBar.Entries;
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
            Widgets.DrawRectFast(position, new Color(0.5f, 0.5f, 0.5f, 0.4f * num), null);
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

        public void HandleClicks(Rect rect, Pawn colonist)
        {
            if (Mouse.IsOver(rect) && Event.current.type == EventType.MouseDown)
            {
                if (clickedColonist == colonist && Time.time - clickedAt < 0.5f)
                {
                    Event.current.Use();
                    CameraJumper.TryJump(colonist);
                    clickedColonist = null;
                }
                else
                {
                    clickedColonist = colonist;
                    clickedAt = Time.time;
                }
            }
            if (Mouse.IsOver(rect) && Event.current.button == 1)
            {

                if (Event.current.type == EventType.MouseDown)
                {
                    List<FloatMenuOption> floatOptionList = new List<FloatMenuOption>();

                    if (clickedColonist != null && SelPawn != null && SelPawn != clickedColonist)
                    {
                        foreach (FloatMenuOption choice in FloatMenuMakerMap.ChoicesAtFor(clickedColonist.TrueCenter(), SelPawn))
                        {
                            floatOptionList.Add(choice);
                        }
                        floatOptionList.Add(new FloatMenuOption("--------------------", delegate
                        {
                        }));

                    }

                    floatOptionList.Add(new FloatMenuOption("CBKF.Settings.Vanilla".Translate(), delegate
                    {
                        ColBarSettings.SortBy = vanilla;
                        Find.ColonistBar.MarkColonistsDirty();
                    }));
                    floatOptionList.Add(new FloatMenuOption("CBKF.Settings.ByName".Translate(), delegate
                    {
                        ColBarSettings.SortBy = byName;
                        Find.ColonistBar.MarkColonistsDirty();
                    }));

                    floatOptionList.Add(new FloatMenuOption("CBKF.Settings.SexAge".Translate(), delegate
                    {
                        ColBarSettings.SortBy = sexage;
                        Find.ColonistBar.MarkColonistsDirty();
                    }));

                    floatOptionList.Add(new FloatMenuOption("CBKF.Settings.Mood".Translate(), delegate
                    {
                        ColBarSettings.SortBy = mood;
                        Find.ColonistBar.MarkColonistsDirty();
                    }));
                    floatOptionList.Add(new FloatMenuOption("CBKF.Settings.Health".Translate(), delegate
                    {
                        ColBarSettings.SortBy = health;
                        Find.ColonistBar.MarkColonistsDirty();
                    }));
                    floatOptionList.Add(new FloatMenuOption("CBKF.Settings.Medic".Translate(), delegate
                    {
                        ColBarSettings.SortBy = medic;
                        Find.ColonistBar.MarkColonistsDirty();
                    }));
                    floatOptionList.Add(new FloatMenuOption("CBKF.Settings.Weapons".Translate(), delegate
                    {
                        ColBarSettings.SortBy = weapons;
                        Find.ColonistBar.MarkColonistsDirty();
                    }));

                    floatOptionList.Add(new FloatMenuOption("CBKF.Settings.SettingsColonistBar".Translate(), delegate { Find.WindowStack.Add(new ColonistBarKF_Settings()); }));
                    FloatMenu window = new FloatMenu(floatOptionList, "CBKF.Settings.SortingOptions".Translate());
                    Find.WindowStack.Add(window);

                    // use event so it doesn't bubble through
                    Event.current.Use();
                }
            }
        }

        private Pawn SelPawn => Find.Selector.SingleSelectedThing as Pawn;

        public void HandleGroupFrameClicks(int group)
        {
            Rect rect = GroupFrameRect(group);
            bool worldRenderedNow = WorldRendererUtility.WorldRenderedNow;
            if (Mouse.IsOver(rect) && Event.current.type == EventType.MouseUp && !ColonistBar.AnyColonistOrCorpseAt(UI.MousePositionOnUIInverted) && ((!worldRenderedNow && !Find.Selector.dragBox.IsValid) || (worldRenderedNow && !Find.WorldSelector.dragBox.IsValid)))
            {
                Find.Selector.dragBox.active = false;
                Find.WorldSelector.dragBox.active = false;
                ColonistBar.Entry entry = ColonistBar.Entries.Find((ColonistBar.Entry x) => x.group == group);
                Map map = entry.map;
                if (map == null)
                {
                    if (Find.MainTabsRoot.OpenTab == MainTabDefOf.World)
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
                    if (!CameraJumper.CloseWorldTab() && Current.Game.VisibleMap != map)
                    {
                        SoundDefOf.MapSelected.PlayOneShotOnCamera();
                    }
                    Current.Game.VisibleMap = map;
                }
            }
        }

        public void Notify_RecachedEntries()
        {
            pawnLabelsCache.Clear();
        }

        private Rect GetPawnTextureRect(float x, float y)
        {
            Vector2 vector = ColonistBarColonistDrawerKF.PawnTextureSize * ColonistBar.Scale;
            Rect rect = new Rect(x + 1f, y - (vector.y - ColonistBar.Size.y) - 1f, vector.x, vector.y);
            rect = rect.ContractedBy(1f);
            return rect;
        }

        private void DrawIcons(Rect rect, Pawn colonist)
        {
            if (colonist.Dead)
            {
                return;
            }
            float num = 20f * ColonistBar.Scale;
            Vector2 vector = new Vector2(rect.x + 1f, rect.yMax - num - 1f);
            bool flag = false;
            if (colonist.CurJob != null)
            {
                JobDef def = colonist.CurJob.def;
                if (def == JobDefOf.AttackMelee || def == JobDefOf.AttackStatic)
                {
                    flag = true;
                }
                else if (def == JobDefOf.WaitCombat)
                {
                    Stance_Busy stance_Busy = colonist.stances.curStance as Stance_Busy;
                    if (stance_Busy != null && stance_Busy.focusTarg.IsValid)
                    {
                        flag = true;
                    }
                }
            }
            if (colonist.InAggroMentalState)
            {
                DrawIcon(ColonistBarColonistDrawerKF.Icon_MentalStateAggro, ref vector, colonist.MentalStateDef.LabelCap);
            }
            else if (colonist.InMentalState)
            {
                DrawIcon(ColonistBarColonistDrawerKF.Icon_MentalStateNonAggro, ref vector, colonist.MentalStateDef.LabelCap);
            }
            else if (colonist.InBed() && colonist.CurrentBed().Medical)
            {
                DrawIcon(ColonistBarColonistDrawerKF.Icon_MedicalRest, ref vector, "ActivityIconMedicalRest".Translate());
            }
            else if (colonist.CurJob != null && colonist.jobs.curDriver.asleep)
            {
                DrawIcon(ColonistBarColonistDrawerKF.Icon_Sleeping, ref vector, "ActivityIconSleeping".Translate());
            }
            else if (colonist.CurJob != null && colonist.CurJob.def == JobDefOf.FleeAndCower)
            {
                DrawIcon(ColonistBarColonistDrawerKF.Icon_Fleeing, ref vector, "ActivityIconFleeing".Translate());
            }
            else if (flag)
            {
                DrawIcon(ColonistBarColonistDrawerKF.Icon_Attacking, ref vector, "ActivityIconAttacking".Translate());
            }
            else if (colonist.mindState.IsIdle && GenDate.DaysPassed >= 1)
            {
                DrawIcon(ColonistBarColonistDrawerKF.Icon_Idle, ref vector, "ActivityIconIdle".Translate());
            }
            if (colonist.IsBurning())
            {
                DrawIcon(ColonistBarColonistDrawerKF.Icon_Burning, ref vector, "ActivityIconBurning".Translate());
            }
        }

        private void DrawIcon(Texture2D icon, ref Vector2 pos, string tooltip)
        {
            float num = 20f * ColonistBar.Scale;
            Rect rect = new Rect(pos.x, pos.y, num, num);
            GUI.DrawTexture(rect, icon);
            TooltipHandler.TipRegion(rect, tooltip);
            pos.x += num;
        }

        private void DrawWeapon(Rect rect, Pawn colonist, float entryRectAlpha)
        {
            Color color = new Color(1f, 1f, 1f, entryRectAlpha);
            GUI.color = color;
            if (colonist?.equipment.Primary != null)
            {
                var thing = colonist.equipment.Primary;
                Rect rect2 = rect.ContractedBy(rect.width / 3);

                rect2.x = rect.xMax - rect2.width - rect.width / 12;
                rect2.y = rect.yMax - rect2.height - rect.height / 12;

                GUI.color = color;
                Texture2D resolvedIcon;
                if (!thing.def.uiIconPath.NullOrEmpty())
                {
                    resolvedIcon = thing.def.uiIcon;
                }
                else
                {
                    resolvedIcon = thing.Graphic.ExtractInnerGraphicFor(thing).MatSingle.mainTexture as Texture2D;
                }
                // color label by thing
                Color iconcolor = new Color();

                if (thing.def.IsMeleeWeapon)
                {
                    GUI.color = new Color(0.85f, 0.2f, 0.2f, entryRectAlpha);
                    iconcolor = new Color(0.2f, 0.05f, 0.05f, 0.75f * entryRectAlpha);
                }
                if (thing.def.IsRangedWeapon)
                {
                    GUI.color = new Color(0.15f, 0.3f, 0.85f, entryRectAlpha);
                    iconcolor = new Color(0.03f, 0.075f, 0.2f, 0.75f * entryRectAlpha);
                }
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

        private static readonly Color HighlightColor = new Color(0.5f, 0.5f, 0.5f, 1f);

        private void DrawSelectionOverlayOnGUI(Pawn colonist, Rect rect)
        {
            Thing obj = colonist;
            if (colonist.Dead)
            {
                obj = colonist.Corpse;
            }
            float num = 0.4f * ColonistBar.Scale;
            Vector2 textureSize = new Vector2((float)SelectionDrawerUtility.SelectedTexGUI.width * num, (float)SelectionDrawerUtility.SelectedTexGUI.height * num);
            SelectionDrawerUtility.CalculateSelectionBracketPositionsUI<object>(ColonistBarColonistDrawerKF.bracketLocs, obj, rect, SelectionDrawer.SelectTimes, textureSize, 20f * ColonistBar.Scale);
            DrawSelectionOverlayOnGUI(ColonistBarColonistDrawerKF.bracketLocs, num);
        }

        private void DrawCaravanSelectionOverlayOnGUI(Caravan caravan, Rect rect)
        {
            float num = 0.4f * ColonistBar.Scale;
            Vector2 textureSize = new Vector2((float)SelectionDrawerUtility.SelectedTexGUI.width * num, (float)SelectionDrawerUtility.SelectedTexGUI.height * num);
            SelectionDrawerUtility.CalculateSelectionBracketPositionsUI<WorldObject>(ColonistBarColonistDrawerKF.bracketLocs, caravan, rect, WorldSelectionDrawer.SelectTimes, textureSize, 20f * ColonistBar.Scale);
            DrawSelectionOverlayOnGUI(ColonistBarColonistDrawerKF.bracketLocs, num);
        }

        private void DrawSelectionOverlayOnGUI(Vector2[] bracketLocs, float selectedTexScale)
        {
            int num = 90;
            for (int i = 0; i < 4; i++)
            {
                Widgets.DrawTextureRotated(bracketLocs[i], SelectionDrawerUtility.SelectedTexGUI, (float)num, selectedTexScale);
                num += 90;
            }
        }
    }
}
