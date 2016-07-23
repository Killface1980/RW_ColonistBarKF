using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RW_ColonistBarKF
{
    [StaticConstructorOnStartup]
    public class ColonistBar_KF
    {


        private const float PawnTextureHorizontalPadding = 1f;


        private const float DoubleClickTime = 0.5f;

        private List<Pawn> cachedColonists = new List<Pawn>();

        private List<Vector2> cachedDrawLocs = new List<Vector2>();

        private bool colonistsDirty = true;

        private Dictionary<string, string> pawnLabelsCache = new Dictionary<string, string>();

        private Pawn clickedColonist;

        private float clickedAt;

        private static readonly Texture2D BGTex = Command.BGTex;

        private static readonly Texture2D SelectedTex = ContentFinder<Texture2D>.Get("UI/Overlays/SelectionBracketGUI", true);

        private static readonly Texture2D DeadColonistTex = ContentFinder<Texture2D>.Get("UI/Misc/DeadColonist", true);

        private static readonly Texture2D Icon_MentalStateNonAggro = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/MentalStateNonAggro", true);

        private static readonly Texture2D Icon_MentalStateAggro = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/MentalStateAggro", true);

        private static readonly Texture2D Icon_MedicalRest = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/MedicalRest", true);

        private static readonly Texture2D Icon_Sleeping = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Sleeping", true);

        private static readonly Texture2D Icon_Fleeing = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Fleeing", true);

        private static readonly Texture2D Icon_Attacking = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Attacking", true);

        private static readonly Texture2D Icon_Idle = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Idle", true);

        private static readonly Texture2D Icon_Burning = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Burning", true);

        private static Vector2 BaseSize = new Vector2(Settings.BaseSizeFloat, Settings.BaseSizeFloat);

        //      public static readonly Vector2 PawnTextureSize = new Vector2(BaseSize.x - 2f, 75f);
        public static Vector2 PawnTextureSize = new Vector2(Settings.BaseSizeFloat - 2f, Settings.BaseSizeFloat * 1.5f);

        private static Vector3 PawnTextureCameraOffset = new Vector3(0f, 0f, 0.3f);

        private float Scale
        {
            get
            {
                if (Settings.useCustomIconSize)
                {
                    return 1f;
                }

                float num = 1f;
                while (true)
                {
                    int allowedRowsCountForScale = GetAllowedRowsCountForScale(num);
                    int num2 = RowsCountAssumingScale(num);
                    if (num2 <= allowedRowsCountForScale)
                    {
                        break;
                    }
                    num *= 0.95f;
                }
                return num;
            }
        }

        private Vector2 Size
        {
            get
            {
                return SizeAssumingScale(Scale);
            }
        }

        private float SpacingHorizontal
        {
            get
            {
                return SpacingHorizontalAssumingScale(Scale);
            }
        }

        private float SpacingVertical
        {
            get
            {
                return SpacingVerticalAssumingScale(Scale);
            }
        }

        private int ColonistsPerRow
        {
            get
            {
                return ColonistsPerRowAssumingScale(Scale);
            }
        }

        private static Vector2 SizeAssumingScale(float scale)
        {
            BaseSize.x = Settings.BaseSizeFloat;
            BaseSize.y = Settings.BaseSizeFloat;
            return BaseSize * scale;
        }

        private int RowsCountAssumingScale(float scale)
        {
            return Mathf.CeilToInt(cachedDrawLocs.Count / (float)ColonistsPerRowAssumingScale(scale));
        }

        private static int ColonistsPerRowAssumingScale(float scale)
        {
            return Mathf.FloorToInt((Settings.MaxColonistBarWidth + SpacingHorizontalAssumingScale(scale)) / (SizeAssumingScale(scale).x + SpacingHorizontalAssumingScale(scale)));
        }

        private static float SpacingHorizontalAssumingScale(float scale)
        {
            if (Settings.useCustomIconSize)
                return Settings.BaseSpacingHorizontal;

            return Settings.BaseSpacingHorizontal * scale;
        }

        private static float SpacingVerticalAssumingScale(float scale)
        {
            if (Settings.useCustomIconSize)
                return Settings.BaseSpacingVertical;
            return Settings.BaseSpacingVertical * scale;
        }

        private static int GetAllowedRowsCountForScale(float scale)
        {
            if (Settings.useCustomIconSize)
                return 4;

            if (scale > 0.58f)
            {
                return 1;
            }
            if (scale > 0.42f)
            {
                return 2;
            }
            return 3;
        }

        private static List<Thing> tmpColonists = new List<Thing>();

        public void ColonistBarOnGUI()
        {
            if (!Find.PlaySettings.showColonistBar)
            {
                return;
            }
            if (Settings.reloadsettings || Settings.firstload)
            {
                BaseSize.x = Settings.BaseSizeFloat;
                BaseSize.y = Settings.BaseSizeFloat;
                PawnTextureSize.x = Settings.BaseSizeFloat - 2f;
                PawnTextureSize.y = Settings.BaseSizeFloat * 1.5f;
                float pawnTextureCameraOffsetNew = Settings.PawnTextureCameraZoom / 1.28205f;
                PawnTextureCameraOffset = new Vector3(0f, 0f, Settings.PawnTextureCameraVerticalOffset / pawnTextureCameraOffsetNew);
                Settings.firstload = false;
                Settings.reloadsettings = false;
            }

            if (Event.current.type == EventType.Layout)
            {
                RecacheDrawLocs();
            }
            else
            {
                for (int i = 0; i < cachedDrawLocs.Count; i++)
                {
                    Rect rect = new Rect(cachedDrawLocs[i].x, cachedDrawLocs[i].y, Size.x, Size.y);
                    Pawn colonist = cachedColonists[i];
                    HandleColonistClicks(rect, colonist);
                    if (Event.current.type == EventType.Repaint)
                    {
                        DrawColonist(rect, colonist);
                    }
                }
            }
        }

        // RimWorld.ColonistBar
        public List<Thing> ColonistsInScreenRect(Rect rect)
        {

            tmpColonists.Clear();
            RecacheDrawLocs();
            for (int i = 0; i < cachedDrawLocs.Count; i++)
            {
                if (rect.Overlaps(new Rect(cachedDrawLocs[i].x, cachedDrawLocs[i].y, Size.x, Size.y)))
                {
                    Thing thing;
                    if (cachedColonists[i].Dead)
                    {
                        thing = cachedColonists[i].corpse;
                    }
                    else
                    {
                        thing = cachedColonists[i];
                    }
                    if (thing != null && thing.Spawned)
                    {
                        tmpColonists.Add(thing);
                    }
                }
            }
            return tmpColonists;
        }

        // RimWorld.ColonistBar
        public Thing ColonistAt(Vector2 pos)
        {
            Pawn pawn = null;
            RecacheDrawLocs();
            for (int i = 0; i < cachedDrawLocs.Count; i++)
            {
                Rect rect = new Rect(cachedDrawLocs[i].x, cachedDrawLocs[i].y, Size.x, Size.y);
                if (rect.Contains(pos))
                {
                    pawn = cachedColonists[i];
                }
            }
            Thing thing;
            if (pawn != null && pawn.Dead)
            {
                thing = pawn.corpse;
            }
            else
            {
                thing = pawn;
            }
            if (thing != null && thing.Spawned)
            {
                return thing;
            }
            return null;
        }


        private void RecacheDrawLocs()
        {
            CheckRecacheColonistsRaw();
            Vector2 size = Size;
            int colonistsPerRow = ColonistsPerRow;
            float spacingHorizontal = SpacingHorizontal;
            float spacingVertical = SpacingVertical;
            float cachedDrawLocs_x = 0f;
            float cachedDrawLocs_y = Settings.MarginTop;
            cachedDrawLocs.Clear();
            for (int i = 0; i < cachedColonists.Count; i++)
            {
                if (i % colonistsPerRow == 0)
                {
                    int maxColInRow = Mathf.Min(colonistsPerRow, cachedColonists.Count - i);
                    float num4 = maxColInRow * size.x + (maxColInRow - 1) * spacingHorizontal;
                    cachedDrawLocs_x = (Screen.width - num4) / 2f;
                    if (i != 0)
                    {
                        cachedDrawLocs_y += size.y + spacingVertical;
                    }
                }
                else
                {
                    cachedDrawLocs_x += size.x + spacingHorizontal;
                }
                cachedDrawLocs.Add(new Vector2(cachedDrawLocs_x, cachedDrawLocs_y));
            }
        }

        private void CheckRecacheColonistsRaw()
        {
            if (!colonistsDirty)
            {
                return;
            }
            cachedColonists.Clear();
            cachedColonists.AddRange(Find.MapPawns.FreeColonists);
            List<Thing> list = Find.ListerThings.ThingsInGroup(ThingRequestGroup.Corpse);
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].IsDessicated())
                {
                    Pawn innerPawn = ((Corpse)list[i]).innerPawn;
                    if (innerPawn.IsColonist)
                    {
                        cachedColonists.Add(innerPawn);
                    }
                }
            }
            List<Pawn> allPawnsSpawned = Find.MapPawns.AllPawnsSpawned;
            for (int j = 0; j < allPawnsSpawned.Count; j++)
            {
                Corpse corpse = allPawnsSpawned[j].carrier.CarriedThing as Corpse;
                if (corpse != null && !corpse.IsDessicated() && corpse.innerPawn.IsColonist)
                {
                    cachedColonists.Add(corpse.innerPawn);
                }
            }
            cachedColonists.SortBy(x => x.thingIDNumber);
            pawnLabelsCache.Clear();
            colonistsDirty = false;
        }

        private void DrawColonist(Rect rect, Pawn colonist)
        {
            float colonistRectAlpha = GetColonistRectAlpha(rect);
            bool flag = !colonist.Dead ? Find.Selector.SelectedObjects.Contains(colonist) : Find.Selector.SelectedObjects.Contains(colonist.corpse);
            Color color = new Color(1f, 1f, 1f, colonistRectAlpha);
            GUI.color = color;
            GUI.DrawTexture(rect, BGTex);
            if (flag)
            {
                DrawSelectionOverlayOnGUI(colonist, rect.ContractedBy(-2f * Scale));
            }
            GUI.DrawTexture(GetPawnTextureRect(rect.x, rect.y), PortraitsCache.Get(colonist, PawnTextureSize, PawnTextureCameraOffset, Settings.PawnTextureCameraZoom));
            GUI.color = new Color(1f, 1f, 1f, colonistRectAlpha * 0.8f);
            DrawIcons(rect, colonist);
            GUI.color = color;
            if (colonist.Dead)
            {
                GUI.DrawTexture(rect, DeadColonistTex);
            }
            float num = 4f * Scale;
            Vector2 pos = new Vector2(rect.center.x, rect.yMax - num);
            GenWorldUI.DrawPawnLabel(colonist, pos, colonistRectAlpha, rect.width + SpacingHorizontal - 2f, pawnLabelsCache);
            GUI.color = Color.white;
        }

        private float GetColonistRectAlpha(Rect rect)
        {
            float t;
            if (Messages.CollidesWithAnyMessage(rect, out t))
            {
                return Mathf.Lerp(1f, 0.2f, t);
            }
            return 1f;
        }

        private Rect GetPawnTextureRect(float x, float y)
        {
            Vector2 vector = PawnTextureSize * Scale;
            return new Rect(x + 1f, y - (vector.y - Size.y) - 1f, vector.x, vector.y);
        }

        private void DrawIcons(Rect rect, Pawn colonist)
        {
            if (colonist.Dead)
            {
                return;
            }
            float num = Settings.BaseIconSize * Scale;
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
                DrawIcon(Icon_MentalStateAggro, ref vector, colonist.MentalStateDef.LabelCap);
            }
            else if (colonist.InMentalState)
            {
                DrawIcon(Icon_MentalStateNonAggro, ref vector, colonist.MentalStateDef.LabelCap);
            }
            else if (colonist.InBed() && colonist.CurrentBed().Medical)
            {
                DrawIcon(Icon_MedicalRest, ref vector, "ActivityIconMedicalRest".Translate());
            }
            else if (colonist.CurJob != null && colonist.jobs.curDriver.asleep)
            {
                DrawIcon(Icon_Sleeping, ref vector, "ActivityIconSleeping".Translate());
            }
            else if (colonist.CurJob != null && colonist.CurJob.def == JobDefOf.FleeAndCower)
            {
                DrawIcon(Icon_Fleeing, ref vector, "ActivityIconFleeing".Translate());
            }
            else if (flag)
            {
                DrawIcon(Icon_Attacking, ref vector, "ActivityIconAttacking".Translate());
            }
            else if (colonist.mindState.IsIdle && GenDate.DaysPassed >= 1)
            {
                DrawIcon(Icon_Idle, ref vector, "ActivityIconIdle".Translate());
            }
            if (colonist.IsBurning())
            {
                DrawIcon(Icon_Burning, ref vector, "ActivityIconBurning".Translate());
            }
        }

        private void DrawIcon(Texture2D icon, ref Vector2 pos, string tooltip)
        {
            float num = Settings.BaseIconSize * Scale;
            Rect rect = new Rect(pos.x, pos.y, num, num);
            GUI.DrawTexture(rect, icon);
            TooltipHandler.TipRegion(rect, tooltip);
            pos.x += num;
        }

        private void HandleColonistClicks(Rect rect, Pawn colonist)
        {

            if (Mouse.IsOver(rect) && Event.current.type == EventType.MouseDown)
            {
                if (clickedColonist == colonist && Time.time - clickedAt < DoubleClickTime)
                {
                    Event.current.Use();
                    JumpToTargetUtility.TryJump(colonist);
                    clickedColonist = null;
                }
                else
                {
                    clickedColonist = colonist;
                    clickedAt = Time.time;
                }
            }
        }

        private void DrawSelectionOverlayOnGUI(Pawn colonist, Rect rect)
        {
            Thing thing = colonist;
            if (colonist.Dead)
            {
                thing = colonist.corpse;
            }
            float num = 0.4f * Scale;
            Vector2 textureSize = new Vector2(SelectedTex.width * num, SelectedTex.height * num);
            Vector3[] array = SelectionDrawer.SelectionBracketPartsPos(thing, rect.center, rect.size, textureSize, Settings.BaseIconSize * Scale);
            int num2 = 90;
            for (int i = 0; i < 4; i++)
            {
                Widgets.DrawTextureRotated(new Vector2(array[i].x, array[i].z), SelectedTex, num2, num);
                num2 += 90;
            }
        }

    }

}
