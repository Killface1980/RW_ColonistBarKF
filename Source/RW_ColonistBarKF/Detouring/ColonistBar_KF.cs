using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;
using static RW_ColonistBarKF.CBKF;

namespace RW_ColonistBarKF
{
    [StaticConstructorOnStartup]
    public class ColonistBar_KF
    {

        private const float PawnTextureHorizontalPadding = 1f;

        private List<Pawn> cachedColonists = new List<Pawn>();

        private List<Vector2> cachedDrawLocs = new List<Vector2>();

        private bool colonistsDirty = true;

        private Dictionary<string, string> pawnLabelsCache = new Dictionary<string, string>();

        private Pawn clickedColonist;

        private float clickedAt;

        private static readonly Texture2D MoodTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.8f, 0.85f, 0.5f));
        private static readonly Texture2D MoodMinorCrossedTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.85f, 0.85f, 0.2f, 0.5f));
        private static readonly Texture2D MoodMajorCrossedTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.95f, 0.55f, 0.05f, 0.75f));
        private static readonly Texture2D MoodExtremeCrossedTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.95f, 0.15f, 0.00f, 0.8f));
        private static readonly Texture2D MoodExtremeCrossedBGTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.9f, 0.1f, 0.00f, 0.45f));

        private static readonly Texture2D MoodTargetTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.7f, 0.9f, 0.95f, 0.7f));
        private static readonly Texture2D MoodBreakTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.1f, 0.2f, 0.22f, 0.8f));


        //   private static Texture2D BGTex = ContentFinder<Texture2D>.Get("UI/Widgets/DesButBG", true);
        //   private static readonly Texture2D BGTex = Command.BGTex;
        private static Texture2D BGTex = Command.BGTex;

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

        // custom test





        private static Vector2 BaseSize = new Vector2(Settings.BaseSizeFloat, Settings.BaseSizeFloat);

        //      public static readonly Vector2 PawnTextureSize = new Vector2(BaseSize.x - 2f, 75f);
        public static Vector2 PawnTextureSize = new Vector2(Settings.BaseSizeFloat - 2f, Settings.BaseSizeFloat * 1.5f);

        private static Vector3 PawnTextureCameraOffset = new Vector3(0f, 0f, 0.3f);

        private float Scale
        {
            get
            {
                float num = 1f;

                if (Settings.UseFixedIconScale)
                {
                    return 1f;
                }

                if (Settings.UseVerticalAlignment)
                {
                    while (true)
                    {
                        int allowedColumnsCountForScale = GetAllowedColumnsCountForScale(num);
                        int num2 = ColumnsCountAssumingScale(num);
                        if (num2 <= allowedColumnsCountForScale)
                        {
                            break;
                        }
                        num *= 0.95f;
                    }
                    return num;
                }

                if (Settings.UseCustomIconSize)
                {

                    while (true)
                    {
                        int allowedRowsCountForScale = GetAllowedRowsCountForScaleModded(num);
                        int num2 = RowsCountAssumingScale(num);
                        if (num2 <= allowedRowsCountForScale)
                        {
                            break;
                        }
                        num *= 0.95f;
                    }
                    return num;
                }


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

        private int ColonistsPerColumn
        {
            get
            {
                return ColonistsPerColumnAssumingScale(Scale);
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
        private int ColumnsCountAssumingScale(float scale)
        {
            return Mathf.CeilToInt(cachedDrawLocs.Count / (float)ColonistsPerColumnAssumingScale(scale));
        }
        private static int ColonistsPerRowAssumingScale(float scale)
        {
            return Mathf.FloorToInt((Settings.MaxColonistBarWidth + SpacingHorizontalAssumingScale(scale)) / (SizeAssumingScale(scale).x + SpacingHorizontalAssumingScale(scale)));
        }

        private static int ColonistsPerColumnAssumingScale(float scale)
        {
            return Mathf.FloorToInt((Settings.MaxColonistBarHeight + SpacingVerticalAssumingScale(scale)) / (SizeAssumingScale(scale).y + SpacingVerticalAssumingScale(scale)));
        }

        private static float SpacingHorizontalAssumingScale(float scale)
        {

            return Settings.BaseSpacingHorizontal * scale;
        }

        private static float SpacingVerticalAssumingScale(float scale)
        {
            return Settings.BaseSpacingVertical * scale;
        }

        private static int GetAllowedRowsCountForScale(float scale)
        {
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

        private static int GetAllowedColumnsCountForScale(float scale)
        {

            if (scale > 0.7f)
            {
                return 4;
            }
            if (scale > 0.6f)
            {
                return 5;
            }
            if (scale > 0.5f)
            {
                return 6;
            }

            return 7;

        }

        private static int GetAllowedRowsCountForScaleModded(float scale)
        {
            if (scale > 0.67f)
            {
                return 2;
            }
            if (scale > 0.34f)
            {
                return 3;
            }
            return 4;
        }

        private static List<Thing> tmpColonists = new List<Thing>();

        [Detour(typeof(ColonistBar), bindingFlags = (BindingFlags.Instance | BindingFlags.Public))]
        public void ColonistBarOnGUI()
        {
            if (!Find.PlaySettings.showColonistBar)
            {
                return;
            }
            if (Settings.Reloadsettings || Settings.Firstload)
            {
                BaseSize.x = Settings.BaseSizeFloat;
                BaseSize.y = Settings.BaseSizeFloat;
                PawnTextureSize.x = Settings.BaseSizeFloat - 2f;
                PawnTextureSize.y = Settings.BaseSizeFloat * 1.5f;
                float pawnTextureCameraOffsetNew = Settings.PawnTextureCameraZoom / 1.28205f;
                PawnTextureCameraOffset = new Vector3(Settings.PawnTextureCameraHorizontalOffset / pawnTextureCameraOffsetNew, 0f, Settings.PawnTextureCameraVerticalOffset / pawnTextureCameraOffsetNew);
                Settings.Firstload = false;
                Settings.Reloadsettings = false;
                if (Settings.UseGender)
                    BGTex = ContentFinder<Texture2D>.Get("UI/Widgets/DesButBG_grey", true);
                else
                {
                    BGTex = ContentFinder<Texture2D>.Get("UI/Widgets/DesButBG_vanilla", true);
                }
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
                        //Widgets.DrawShadowAround(rect);
                        DrawColonist(rect, colonist);
                    }
                }
            }

        }

        // RimWorld.ColonistBar
        [Detour(typeof(ColonistBar), bindingFlags = (BindingFlags.Instance | BindingFlags.Public))]
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

        [Detour(typeof(ColonistBar), bindingFlags = (BindingFlags.Instance | BindingFlags.Public))]
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


        public void RecacheDrawLocs()
        {
            CheckRecacheColonistsRaw();
            Vector2 size = Size;
            int colonistsPerRow = ColonistsPerRow;
            int colonistsPerColumn = ColonistsPerColumn;
            float spacingHorizontal = SpacingHorizontal;
            float spacingVertical = SpacingVertical;
            float cachedDrawLocs_x = 0f + Settings.MarginLeftHorTop;
            float cachedDrawLocs_y = Settings.MarginTopHor;
            if (Settings.UseVerticalAlignment)
            {
                cachedDrawLocs_x = 0f + Settings.MarginLeftVer;
                if (Settings.UseRightAlignment)
                    cachedDrawLocs_x = Screen.width - size.x - Settings.MarginRightVer;
            }
            else if (Settings.UseBottomAlignment)
            {
                cachedDrawLocs_y = Screen.height - size.y - Settings.MarginBottomHor - 30f - 12f;
            }
            cachedDrawLocs.Clear();
            if (Settings.UseVerticalAlignment)
            {
                for (int i = 0; i < cachedColonists.Count; i++)
                {
                    //         Debug.Log("Colonists count: " + i);
                    if (i % colonistsPerColumn == 0)
                    {
                        int maxColInColumn = Mathf.Min(colonistsPerColumn, cachedColonists.Count - i);
                        float num4 = maxColInColumn * size.y + (maxColInColumn - 1) * spacingVertical;
                        cachedDrawLocs_y = (Screen.height - num4) / 2f + Settings.VerticalOffset;
                        if (i != 0)
                        {
                            if (Settings.UseRightAlignment)
                            {
                                cachedDrawLocs_x -= size.x + spacingHorizontal;
                            }
                            else
                            {
                                cachedDrawLocs_x += size.x + spacingHorizontal;
                            }
                        }
                        //         Debug.Log("maxColInColumn " + maxColInColumn);
                    }
                    else
                    {
                        cachedDrawLocs_y += size.y + spacingVertical;
                    }
                    cachedDrawLocs.Add(new Vector2(cachedDrawLocs_x, cachedDrawLocs_y));

                    //      Debug.Log("MaxColonistBarHeight:" + ModSettings.MaxColonistBarHeight+ " + SpacingVerticalAssumingScale(1f): "+ SpacingVerticalAssumingScale(1f) + " / (SizeAssumingScale(1f).y: "+ SizeAssumingScale(1f).y + " + SpacingVerticalAssumingScale(1f): "+ SpacingVerticalAssumingScale(1f));
                    //
                    //      Debug.Log("colonistsPerRow " + colonistsPerRow);
                    //      Debug.Log("colonistsPerColumn " + colonistsPerColumn);
                    //      Debug.Log("cachedDrawLocs_x: " + cachedDrawLocs_x);
                    //      Debug.Log("cachedDrawLocs_y: " + cachedDrawLocs_y);
                    //      Debug.Log("cachedColonists: " + i);

                }
            }
            else
            {
                for (int i = 0; i < cachedColonists.Count; i++)
                {
                    if (i % colonistsPerRow == 0)
                    {
                        int maxColInRow = Mathf.Min(colonistsPerRow, cachedColonists.Count - i);
                        float num4 = maxColInRow * size.x + (maxColInRow - 1) * spacingHorizontal;
                        cachedDrawLocs_x = (Screen.width - num4) / 2f + Settings.HorizontalOffset;
                        if (i != 0)
                        {
                            if (Settings.UseBottomAlignment)
                            {
                                cachedDrawLocs_y -= size.y + spacingVertical;
                            }
                            else
                            {
                                cachedDrawLocs_y += size.y + spacingVertical;
                            }
                        }
                    }
                    else
                    {
                        cachedDrawLocs_x += size.x + spacingHorizontal;
                    }
                    cachedDrawLocs.Add(new Vector2(cachedDrawLocs_x, cachedDrawLocs_y));
                }
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

            SortCachedColonists();

            pawnLabelsCache.Clear();
            colonistsDirty = false;
        }

        public void SortCachedColonists()
        {
            IOrderedEnumerable<Pawn> orderedEnumerable;
            switch (Settings.SortBy)
            {
                case "vanilla":
                    cachedColonists.SortBy(x => x.thingIDNumber);
                    break;

                case "sexage":
                    orderedEnumerable = cachedColonists.OrderBy(x => x.gender.GetLabel()).ThenBy(x => x.ageTracker.AgeBiologicalYears);
                    cachedColonists = orderedEnumerable.ToList();
                    break;

                case "health":
                    cachedColonists.SortBy(x => x.health.summaryHealth.SummaryHealthPercent);
                    break;

                case "health2":
                    cachedColonists.SortByDescending(x => x.health.summaryHealth.SummaryHealthPercent);
                    break;
                case "mood":
                    cachedColonists.SortByDescending(x => x.needs.mood.CurInstantLevel);
                    break;
                case "health3":
                    orderedEnumerable = cachedColonists.OrderByDescending(
                        x =>
                        {
                            return x.equipment.Primary != null && x.equipment.Primary.def.IsMeleeWeapon;
                        }).ThenByDescending(
                        x =>
                        {
                            return x.equipment.Primary != null && x.equipment.Primary.def.IsRangedWeapon;
                        });
                    cachedColonists = orderedEnumerable.ToList();
                    break;

                default:
                    cachedColonists.SortBy(x => x.thingIDNumber);
                    break;
            }

        }

        private void DrawColonist(Rect rect, Pawn colonist)
        {
            float colonistRectAlpha = GetColonistRectAlpha(rect);
            bool flag = !colonist.Dead ? Find.Selector.SelectedObjects.Contains(colonist) : Find.Selector.SelectedObjects.Contains(colonist.corpse);
            Color color = new Color(1f, 1f, 1f, colonistRectAlpha);
            GUI.color = color;

            Color BGColor = new Color();

            Need_Mood mood = (!colonist.Dead) ? colonist.needs.mood : null;
            MentalBreaker mb = (!colonist.Dead) ? colonist.mindState.mentalBreaker : null;

            if (Settings.UseMoodColors)
            {
                Rect moodBorderRect = rect.ContractedBy(-1f);

                if (mood != null && mb != null)
                {
                    if (mood.CurLevelPercentage <= mb.BreakThresholdExtreme)
                    {
                        GUI.DrawTexture(moodBorderRect, MoodExtremeCrossedTex);
                    }
                    else if (mood.CurLevelPercentage <= mb.BreakThresholdMajor)
                    {
                        GUI.DrawTexture(moodBorderRect, MoodMajorCrossedTex);
                    }
                    else if (mood.CurLevelPercentage <= mb.BreakThresholdMinor)
                    {
                        GUI.DrawTexture(moodBorderRect, MoodMinorCrossedTex);
                    }
                }
            }
            if (Settings.UseGender)
            {
                if (colonist.gender == Gender.Male)
                {
                    BGColor = Settings.MaleColor;
                }
                if (colonist.gender == Gender.Female)
                {
                    BGColor = Settings.FemaleColor;
                }
            }
            if (colonist.Dead)
                BGColor = BGColor * Color.gray;

            // else if (colonist.needs.mood.CurLevel < colonist.mindState.mentalBreaker.BreakThresholdMinor)
            // {
            //     BGColor = Color.Lerp(Color.red, BGColor, colonist.needs.mood.CurLevel / colonist.mindState.mentalBreaker.BreakThresholdMinor);
            // }
            BGColor.a = colonistRectAlpha;
            if (Settings.UseGender)
            {
                GUI.color = BGColor;
            }


            // adding color overlay

            GUI.DrawTexture(rect, BGTex);
            GUI.color = color;
            if (Settings.UseMoodColors)
            {
                // draw mood thingie
                Rect moodRect = rect.ContractedBy(2.0f);

                if (mood != null && mb != null)
                {
                    if (mood.CurLevelPercentage > mb.BreakThresholdMinor)
                    {
                        GUI.DrawTexture(moodRect.BottomPart(mood.CurLevelPercentage), MoodTex);
                    }
                    else if (mood.CurLevelPercentage > mb.BreakThresholdMajor)
                    {
                        GUI.DrawTexture(moodRect.BottomPart(mood.CurLevelPercentage), MoodMinorCrossedTex);
                    }
                    else if (mood.CurLevelPercentage > mb.BreakThresholdExtreme)
                    {
                        GUI.DrawTexture(moodRect.BottomPart(mood.CurLevelPercentage), MoodMajorCrossedTex);
                    }
                    else
                    {
                        GUI.DrawTexture(moodRect, MoodExtremeCrossedBGTex);
                        GUI.DrawTexture(moodRect.BottomPart(mood.CurLevelPercentage), MoodExtremeCrossedTex);
                    }

                    DrawMentalThreshold(moodRect, mb.BreakThresholdExtreme, mood.CurLevelPercentage);
                    DrawMentalThreshold(moodRect, mb.BreakThresholdMajor, mood.CurLevelPercentage);
                    DrawMentalThreshold(moodRect, mb.BreakThresholdMinor, mood.CurLevelPercentage);

                    GUI.DrawTexture(new Rect(moodRect.x, moodRect.yMax - moodRect.height * mood.CurInstantLevelPercentage, moodRect.width, 1), MoodTargetTex);

                    GUI.DrawTexture(new Rect(moodRect.xMax + 1, moodRect.yMax - moodRect.height * mood.CurInstantLevelPercentage - 1, 2, 3), MoodTargetTex);
                }

            }
            if (flag)
            {
                DrawSelectionOverlayOnGUI(colonist, rect.ContractedBy(-2f * Scale));
            }

            GUI.DrawTexture(GetPawnTextureRect(rect.x, rect.y), PortraitsCache.Get(colonist, PawnTextureSize, PawnTextureCameraOffset, Settings.PawnTextureCameraZoom));

            if (true)
            {


                DrawWeapon(rect, colonist);
                if (!Settings.UseCustomPawnTextureCameraHorizontalOffset)
                {
                    Settings.PawnTextureCameraHorizontalOffset = 0.3f;
                }
            }

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
        private static readonly Color _highlightColor = new Color(0.5f, 0.5f, 0.5f, 1f);

        private void DrawWeapon(Rect rect, Pawn colonist)
        {
            foreach (ThingWithComps thing in colonist.equipment.AllEquipment)
            {
                var rect2 = rect.ContractedBy(rect.width / 3);

                rect2.x += rect.width / 3 - (rect.width / 12);
                rect2.y += rect.height / 3 - (rect.height / 12);

                if (Mouse.IsOver(rect2))
                {
                    GUI.color = _highlightColor;
                    GUI.DrawTexture(rect2, TexUI.HighlightTex);
                }

                GUI.color = Color.white;
                Texture2D resolvedIcon;
                if (!thing.def.uiIconPath.NullOrEmpty())
                {
                    resolvedIcon = thing.def.uiIcon;
                }
                else
                {
                    resolvedIcon = thing.Graphic.ExtractInnerGraphicFor(thing).MatSingle.mainTexture as Texture2D;
                }
                // color labe by thing

                var iconcolor = new Color(0.8f, 0.8f, 0.8f, 0.75f);
                if (thing.def.IsMeleeWeapon)
                {
                    GUI.color = new Color(0.8f, 0.1f, 0.1f, 0.9f);
                }
                if (thing.def.IsRangedWeapon)
                {
                    GUI.color = new Color(0.8f, 0.8f, 0.1f, 0.9f);
                }
                Widgets.DrawBoxSolid(rect2, iconcolor);
                Widgets.DrawBox(rect2);
                GUI.color = Color.white;
                var rect3 = rect2.ContractedBy(rect2.width / 8);

                Widgets.DrawTextureRotated(rect3, resolvedIcon, -90f);


            }
        }



        internal static void DrawMentalThreshold(Rect moodRect, float threshold, float currentMood)
        {
            GUI.DrawTexture(new Rect(moodRect.x, moodRect.yMax - moodRect.height * threshold, moodRect.width, 1), MoodBreakTex);
            /*if (currentMood <= threshold)
			{
				GUI.DrawTexture(new Rect(moodRect.xMax-4, moodRect.yMax - moodRect.height * threshold, 8, 2), MoodBreakCrossedTex);
			}*/
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
            // custom 

            //       if (ModSettings.useExtraIcons)
            //       {
            //           if (colonist.needs.mood.CurLevel < colonist.mindState.mentalBreaker.BreakThresholdMinor)
            //           {
            //               GUI.color = Color.Lerp(Color.red, Color.grey, colonist.needs.mood.CurLevel / colonist.mindState.mentalBreaker.BreakThresholdMinor);
            //               Icon_Sad = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar_KF/Sad", true);
            //               DrawIcon(Icon_Sad, ref vector, "Sad".Translate());
            //               GUI.color = Color.white;
            //           } 
            //       }

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
                if (clickedColonist == colonist && Time.time - clickedAt < Settings.DoubleClickTime)
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
            if (Mouse.IsOver(rect) && Event.current.button == 2)
            {
                if (Event.current.type == EventType.MouseDown)
                    Find.WindowStack.Add(new ColonistBarKF_Settings());
                //    Find.WindowStack.Add(new Dialog_InfoCard(colonist));
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
