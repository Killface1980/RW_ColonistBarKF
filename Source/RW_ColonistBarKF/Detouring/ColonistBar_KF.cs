using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColonistBarKF.PSI;
using RimWorld;
using UnityEngine;
using Verse;
using static ColonistBarKF.SettingsColonistBar.SortByWhat;
using static ColonistBarKF.CBKF;

namespace ColonistBarKF
{
    public class ColonistBar_KF
    {

        private const float PawnTextureHorizontalPadding = 1f;

        private List<Pawn> cachedColonists = new List<Pawn>();

        private List<Vector2> cachedDrawLocs = new List<Vector2>();

        private bool colonistsDirty = true;

        private Dictionary<string, string> pawnLabelsCache = new Dictionary<string, string>();

        private Pawn clickedColonist;

        private float clickedAt;


        // custom test

        public static Vector2 BaseSize = new Vector2(ColBarSettings.BaseSizeFloat, ColBarSettings.BaseSizeFloat);

        //      public static readonly Vector2 PawnTextureSize = new Vector2(BaseSize.x - 2f, 75f);
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

        public static float CurrentScale;

        private float Scale
        {
            get
            {
                CurrentScale = 1f;

                if (ColBarSettings.UseFixedIconScale)
                {
                    return ColBarSettings.FixedIconScaleFloat;
                }

                if (ColBarSettings.ColBarPos == SettingsColonistBar.Alignment.Left || ColBarSettings.ColBarPos == SettingsColonistBar.Alignment.Right)
                {
                    while (true)
                    {
                        int allowedColumnsCountForScale = GetAllowedRowsCountForScale(CurrentScale);
                        int num2 = ColumnsCountAssumingScale(CurrentScale);
                        if (num2 <= allowedColumnsCountForScale)
                        {
                            break;
                        }
                        CurrentScale *= 0.95f;
                    }
                    return CurrentScale;
                }

                while (true)
                {
                    int allowedRowsCountForScale = GetAllowedRowsCountForScale(CurrentScale);

                    int rowsCountAssumingScale = RowsCountAssumingScale(CurrentScale);
                    if (rowsCountAssumingScale <= allowedRowsCountForScale)
                    {
                        break;
                    }
                    CurrentScale *= 0.95f;
                }
                return CurrentScale;
            }
        }

        private Vector2 Size => SizeAssumingScale(Scale);

        private float SpacingHorizontal => SpacingHorizontalAssumingScale(Scale);

        private float SpacingVertical => SpacingVerticalAssumingScale(Scale);

        private float SpacingPSIHorizontal => SpacingHorizontalPSIAssumingScale(Scale);

        private float SpacingPSIVertical => SpacingVerticalPSIAssumingScale(Scale);

        private float SpacingMoodBarHorizontal => SpacingHorizontalMoodBarAssumingScale(Scale);

        private static float SpacingHorizontalMoodBarAssumingScale(float scale)
        {
            if (ColBarSettings.UseMoodColors)
                return ColBarSettings.BaseSizeFloat / 4 * scale;

            return 0f;
        }


        private int ColonistsPerRow => ColonistsPerRowAssumingScale(Scale);

        private int ColonistsPerColumn => ColonistsPerColumnAssumingScale(Scale);

        private static Vector2 SizeAssumingScale(float scale)
        {
            BaseSize.x = ColBarSettings.BaseSizeFloat;
            BaseSize.y = ColBarSettings.BaseSizeFloat;
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
            if (ColBarSettings.ColBarPos == SettingsColonistBar.Alignment.Bottom)
            {
                ColBarSettings.MaxColonistBarWidth = Screen.width - ColBarSettings.MarginLeftHorBottom - ColBarSettings.MarginRightHorBottom;
                ColBarSettings.HorizontalOffset = ColBarSettings.MarginLeftHorBottom / 2 - ColBarSettings.MarginRightHorBottom / 2;

            }
            else
            {
                ColBarSettings.MaxColonistBarWidth = Screen.width - ColBarSettings.MarginLeftHorTop - ColBarSettings.MarginRightHorTop;
                ColBarSettings.HorizontalOffset = ColBarSettings.MarginLeftHorTop / 2 - ColBarSettings.MarginRightHorTop / 2;

            }
            return Mathf.FloorToInt((ColBarSettings.MaxColonistBarWidth) / (SizeAssumingScale(scale).x + SpacingHorizontalAssumingScale(scale) + SpacingHorizontalPSIAssumingScale(scale) + SpacingHorizontalMoodBarAssumingScale(scale)));
        }

        private static int ColonistsPerColumnAssumingScale(float scale)
        {
            if (ColBarSettings.ColBarPos == SettingsColonistBar.Alignment.Right)
            {
                ColBarSettings.MaxColonistBarHeight = Screen.height - ColBarSettings.MarginTopVerRight - ColBarSettings.MarginBottomVerRight;
                ColBarSettings.VerticalOffset = ColBarSettings.MarginTopVerRight / 2 - ColBarSettings.MarginBottomVerRight / 2;
            }
            else
            {
                ColBarSettings.MaxColonistBarHeight = Screen.height - ColBarSettings.MarginTopVerLeft - ColBarSettings.MarginBottomVerLeft;
                ColBarSettings.VerticalOffset = ColBarSettings.MarginTopVerLeft / 2 - ColBarSettings.MarginBottomVerLeft / 2;
            }
            return Mathf.FloorToInt((ColBarSettings.MaxColonistBarHeight) / (SizeAssumingScale(scale).y + SpacingVerticalAssumingScale(scale) + SpacingVerticalPSIAssumingScale(scale)));
        }


        private static float SpacingHorizontalAssumingScale(float scale)
        {
            return ColBarSettings.BaseSpacingHorizontal * scale;
        }

        private static float SpacingVerticalAssumingScale(float scale)
        {
            return ColBarSettings.BaseSpacingVertical * scale;
        }

        private static float SpacingHorizontalPSIAssumingScale(float scale)
        {
            if (ColBarSettings.UsePsi)
                if (ColBarSettings.ColBarPsiIconPos == SettingsColonistBar.Alignment.Left || ColBarSettings.ColBarPsiIconPos == SettingsColonistBar.Alignment.Right)
                {

                    return ColBarSettings.BaseSizeFloat / ColBarSettings.IconsInColumn * scale * PsiRowsOnBar;
                }
            return 0f;
        }

        private static float SpacingVerticalPSIAssumingScale(float scale)
        {
            if (ColBarSettings.UsePsi)
                if (ColBarSettings.ColBarPsiIconPos == SettingsColonistBar.Alignment.Bottom || ColBarSettings.ColBarPsiIconPos == SettingsColonistBar.Alignment.Top)
                {
                    return ColBarSettings.BaseSizeFloat / ColBarSettings.IconsInColumn * scale * PsiRowsOnBar;
                }
            return 0f;
        }

        private static int PsiRowsOnBar
        {
            get
            {
                int maxCount = 0;
                foreach (KeyValuePair<Pawn, PawnStats> colonist in PSI.PSI._statsDict)
                {
                    if (colonist.Value.IconCount > maxCount)
                        maxCount = colonist.Value.IconCount;
                }
                int psiRowsOnBar = Mathf.CeilToInt((float)maxCount / ColBarSettings.IconsInColumn);
                return psiRowsOnBar;
            }
        }

        private static int GetAllowedRowsCountForScale(float scale)
        {
            if (ColBarSettings.UseCustomRowCount)
            {
                switch (ColBarSettings.MaxRowsCustom)
                {
                    case 1:
                        {
                            return 1;
                        }
                    case 2:
                        {
                            if (scale > 0.58f)
                            {
                                return 1;
                            }
                            return 2;
                        }
                    case 3:
                        {
                            break;
                        }
                    case 4:
                        {
                            if (scale > 0.58f)
                            {
                                return 1;
                            }
                            if (scale > 0.5f)
                            {
                                return 2;
                            }
                            if (scale > 0.42f)
                            {
                                return 3;
                            }
                            return 4;
                        }
                    case 5:
                        {
                            if (scale > 0.58f)
                            {
                                return 1;
                            }
                            if (scale > 0.5f)
                            {
                                return 2;
                            }
                            if (scale > 0.42f)
                            {
                                return 3;
                            }
                            if (scale > 0.34f)
                            {
                                return 4;
                            }
                            return 5;
                        }

                }
            }


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

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once InconsistentNaming
        public void ColonistBarOnGUI()
        {
            if (!Find.PlaySettings.showColonistBar)
            {
                return;
            }


            if (Event.current.type == EventType.Layout)
            {
                BaseSize.x = ColBarSettings.BaseSizeFloat;
                BaseSize.y = ColBarSettings.BaseSizeFloat;
                PawnTextureSize.x = ColBarSettings.BaseSizeFloat - 2f;
                PawnTextureSize.y = ColBarSettings.BaseSizeFloat * 1.5f;

                RecacheDrawLocs();
            }
            else
            {
                for (int i = 0; i < cachedDrawLocs.Count; i++)
                {
                    Rect rect = new Rect(cachedDrawLocs[i].x, cachedDrawLocs[i].y, Size.x, Size.y);
                    //
                    Pawn colonist = cachedColonists[i];

                    //
                    HandleColonistClicks(rect, colonist);
                    if (Event.current.type == EventType.Repaint)
                    {
                        //Widgets.DrawShadowAround(rect);
                        DrawColonist(rect, colonist);

                        if (ColBarSettings.UsePsi)
                        {
                            float colonistRectAlpha = GetColonistRectAlpha(rect);
                            PSI.PSI.DrawColonistIconsOnBar(rect, colonist, colonistRectAlpha);
                        }
                    }
                }
            }


        }


        // RimWorld.ColonistBar
        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
        // ReSharper disable once UnusedMember.Global
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

        [Detour(typeof(ColonistBar), bindingFlags = BindingFlags.Instance | BindingFlags.Public)]
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
            float spacingHorizontal = SpacingHorizontal + SpacingPSIHorizontal + SpacingMoodBarHorizontal;
            float spacingVertical = SpacingVertical + SpacingPSIVertical;

            float cachedDrawLocs_x = 0f + ColBarSettings.MarginLeftHorTop * Scale;
            float cachedDrawLocs_y = ColBarSettings.MarginTopHor * Scale;


            switch (ColBarSettings.ColBarPos)
            {
                case SettingsColonistBar.Alignment.Left:
                    cachedDrawLocs_x = 0f + ColBarSettings.MarginLeftVer;
                    break;

                case SettingsColonistBar.Alignment.Right:
                    cachedDrawLocs_x = Screen.width - size.x - ColBarSettings.MarginRightVer;
                    break;

                case SettingsColonistBar.Alignment.Bottom:
                    cachedDrawLocs_y = Screen.height - size.y - ColBarSettings.MarginBottomHor - 30f - 12f;
                    break;
                case SettingsColonistBar.Alignment.Top:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            cachedDrawLocs.Clear();

            #region Horizontal Alignment

            if (ColBarSettings.ColBarPos == SettingsColonistBar.Alignment.Left || ColBarSettings.ColBarPos == SettingsColonistBar.Alignment.Right)
                for (int i = 0; i < cachedColonists.Count; i++)
                {
                    //         Debug.Log("Colonists count: " + i);
                    if (i % colonistsPerColumn == 0)
                    {
                        int maxColInColumn = Mathf.Min(colonistsPerColumn, cachedColonists.Count - i);
                        float num4 = maxColInColumn * size.y + (maxColInColumn - 1) * spacingVertical;
                        cachedDrawLocs_y = (Screen.height - num4) / 2f + ColBarSettings.VerticalOffset;

                        if (ColBarSettings.UsePsi)
                            ModifyBasicDrawLocsForPsi(size, ref cachedDrawLocs_x, ref cachedDrawLocs_y);
                        if (ColBarSettings.UseMoodColors)
                            cachedDrawLocs_x -= size.x / 8;

                        if (i != 0)
                        {
                            if (ColBarSettings.ColBarPos == SettingsColonistBar.Alignment.Right)
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

                    //      Debug.Log("MaxColonistBarHeight:" + SettingsColonistBar.MaxColonistBarHeight+ " + SpacingVerticalAssumingScale(1f): "+ SpacingVerticalAssumingScale(1f) + " / (SizeAssumingScale(1f).y: "+ SizeAssumingScale(1f).y + " + SpacingVerticalAssumingScale(1f): "+ SpacingVerticalAssumingScale(1f));
                    //
                    //      Debug.Log("colonistsPerRow " + colonistsPerRow);
                    //      Debug.Log("colonistsPerColumn " + colonistsPerColumn);
                    //      Debug.Log("cachedDrawLocs_x: " + cachedDrawLocs_x);
                    //      Debug.Log("cachedDrawLocs_y: " + cachedDrawLocs_y);
                    //      Debug.Log("cachedColonists: " + i);
                }
            #endregion
            #region Vertical
            else
                for (int i = 0; i < cachedColonists.Count; i++)
                {
                    if (i % colonistsPerRow == 0)
                    {
                        int maxColInRow = Mathf.Min(colonistsPerRow, cachedColonists.Count - i);
                        float num4 = maxColInRow * size.x + (maxColInRow - 1) * spacingHorizontal;
                        cachedDrawLocs_x = (Screen.width - num4) / 2f + ColBarSettings.HorizontalOffset;

                        if (ColBarSettings.UsePsi)
                            ModifyBasicDrawLocsForPsi(size, ref cachedDrawLocs_x, ref cachedDrawLocs_y);

                        if (ColBarSettings.UseMoodColors)
                            cachedDrawLocs_x -= size.x / 8;


                        if (i != 0)
                        {
                            if (ColBarSettings.ColBarPos == SettingsColonistBar.Alignment.Bottom)
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


            #endregion


        }

        private static void ModifyBasicDrawLocsForPsi(Vector2 size, ref float cachedDrawLocs_x, ref float cachedDrawLocs_y)
        {
            switch (ColBarSettings.ColBarPsiIconPos)
            {
                case SettingsColonistBar.Alignment.Left:
                    cachedDrawLocs_x += size.x / ColBarSettings.IconsInColumn * PsiRowsOnBar / 2;
                    break;

                case SettingsColonistBar.Alignment.Right:
                    cachedDrawLocs_x -= size.x / ColBarSettings.IconsInColumn * PsiRowsOnBar / 2;
                    break;

                case SettingsColonistBar.Alignment.Bottom:
                    //      cachedDrawLocs_y -= size.y/ColBarSettings.IconsInColumn*PsiRowsOnBar;
                    break;
                case SettingsColonistBar.Alignment.Top:
                    cachedDrawLocs_y += size.y / ColBarSettings.IconsInColumn * PsiRowsOnBar / 2;
                    break;
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
            foreach (Thing t in list)
            {
                if (t.IsDessicated()) continue;
                Pawn innerPawn = ((Corpse)t).innerPawn;
                if (innerPawn.IsColonist)
                {
                    cachedColonists.Add(innerPawn);
                }
            }
            List<Pawn> allPawnsSpawned = Find.MapPawns.AllPawnsSpawned;
            foreach (Pawn pawn in allPawnsSpawned)
            {
                Corpse corpse = pawn.carrier.CarriedThing as Corpse;
                if (corpse != null && !corpse.IsDessicated() && corpse.innerPawn.IsColonist)
                {
                    cachedColonists.Add(corpse.innerPawn);
                }
            }

            SortCachedColonists();

            pawnLabelsCache.Clear();
            colonistsDirty = false;
        }

        private void SortCachedColonists()
        {
            IOrderedEnumerable<Pawn> orderedEnumerable = null;
            switch (ColBarSettings.SortBy)
            {
                case vanilla:
                    cachedColonists.SortBy(x => x.thingIDNumber);
                    break;

                case byName:
                    orderedEnumerable = cachedColonists.OrderBy(x => x.LabelCap != null).ThenBy(x => x.LabelCap);
                    cachedColonists = orderedEnumerable.ToList();
                    break;

                case sexage:
                    orderedEnumerable = cachedColonists.OrderBy(x => x.gender.GetLabel() != null).ThenBy(x => x.gender.GetLabel()).ThenBy(x => x.ageTracker.AgeBiologicalYears);
                    cachedColonists = orderedEnumerable.ToList();
                    break;

                case health:
                    orderedEnumerable = cachedColonists.OrderBy(x => x?.health?.summaryHealth?.SummaryHealthPercent);
                    cachedColonists = orderedEnumerable.ToList();
                    break;

                case mood:
                    orderedEnumerable = cachedColonists.OrderByDescending(x => x?.needs?.mood?.CurLevelPercentage);
                    cachedColonists = orderedEnumerable.ToList();
                    break;

                case weapons:
                    //orderedEnumerable = cachedColonists
                    //    .OrderByDescending(x => x.equipment.Primary != null && x.equipment.Primary.def.IsMeleeWeapon)
                    //    .ThenByDescending(x => x.equipment.Primary != null && x.equipment.Primary.def.IsRangedWeapon);

                    orderedEnumerable = cachedColonists.OrderByDescending(a => a.equipment.Primary != null && a.equipment.Primary.def.IsMeleeWeapon)
                        //.GetSkill(SkillDefOf.Melee).level)
                        .ThenByDescending(c => c.equipment.Primary != null && c.equipment.Primary.def.IsRangedWeapon).ThenByDescending(b => b.skills.AverageOfRelevantSkillsFor(WorkTypeDefOf.Hunting));
                    //                    .ThenByDescending(d => d.skills.GetSkill(SkillDefOf.Shooting).level);

                    cachedColonists = orderedEnumerable.ToList();
                    break;

                case medic:

                    orderedEnumerable = cachedColonists.OrderByDescending(b => b.skills.AverageOfRelevantSkillsFor(WorkTypeDefOf.Doctor));

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
            bool colonistAlive = !colonist.Dead ? Find.Selector.SelectedObjects.Contains(colonist) : Find.Selector.SelectedObjects.Contains(colonist.corpse);
            Color color = new Color(1f, 1f, 1f, colonistRectAlpha);
            GUI.color = color;

            Color BGColor = new Color();

            Need_Mood mood;
            if (!colonist.Dead) mood = colonist.needs.mood;
            else mood = null;
            MentalBreaker mb;
            if (!colonist.Dead) mb = colonist.mindState.mentalBreaker;
            else mb = null;

            if (ColBarSettings.UseMoodColors)
            {
                // draw mood border
                Rect moodBorderRect = new Rect(rect.x, rect.y, rect.width * 1.25f, rect.height);

                if (mood != null && mb != null)
                {
                    if (mood.CurLevelPercentage <= mb.BreakThresholdExtreme)
                    {
                        GUI.DrawTexture(moodBorderRect, ColonistBarTextures.MoodNeutral);
                        GUI.DrawTexture(moodBorderRect, ColonistBarTextures.MoodExtremeCrossedTex);
                    }
                    else if (mood.CurLevelPercentage <= mb.BreakThresholdMajor)
                    {
                        GUI.DrawTexture(moodBorderRect, ColonistBarTextures.MoodNeutral);
                        GUI.DrawTexture(moodBorderRect, ColonistBarTextures.MoodMajorCrossedTex);
                    }
                    else if (mood.CurLevelPercentage <= mb.BreakThresholdMinor)
                    {
                        GUI.DrawTexture(moodBorderRect, ColonistBarTextures.MoodNeutral);
                        GUI.DrawTexture(moodBorderRect, ColonistBarTextures.MoodMinorCrossedTex);
                    }
                    else
                    {
                        GUI.DrawTexture(moodBorderRect, ColonistBarTextures.MoodNeutral);
                    }
                }
            }


            if (ColBarSettings.UseGender)
            {
                if (colonist.gender == Gender.Male)
                {
                    BGColor = ColBarSettings.MaleColor;
                }
                if (colonist.gender == Gender.Female)
                {
                    BGColor = ColBarSettings.FemaleColor;
                }

                if (colonist.Dead)
                    BGColor = BGColor * Color.gray;

                BGColor.a = colonistRectAlpha;

                GUI.color = BGColor;
            }


            GUI.DrawTexture(rect, ColBarSettings.UseGender ? ColonistBarTextures.BGTexGrey : ColonistBarTextures.BGTexVanilla);

            GUI.color = color;

            if (ColBarSettings.UseMoodColors)
            {
                //         Rect moodRect = new Rect(rect.xMax, rect.y, rect.width/4, rect.height);
                Rect moodRect = rect.ContractedBy(2.0f);
                moodRect.x = rect.xMax;
                moodRect.width /= 4;

                if (mood != null && mb != null)
                {
                    if (mood.CurLevelPercentage > mb.BreakThresholdMinor)
                    {
                        GUI.DrawTexture(moodRect.BottomPart(mood.CurLevelPercentage), ColonistBarTextures.MoodTex);
                    }
                    else if (mood.CurLevelPercentage > mb.BreakThresholdMajor)
                    {
                        GUI.DrawTexture(moodRect.BottomPart(mood.CurLevelPercentage), ColonistBarTextures.MoodMinorCrossedTex);
                    }
                    else if (mood.CurLevelPercentage > mb.BreakThresholdExtreme)
                    {
                        GUI.DrawTexture(moodRect.BottomPart(mood.CurLevelPercentage), ColonistBarTextures.MoodMajorCrossedTex);
                    }
                    else
                    {
                        GUI.DrawTexture(moodRect, ColonistBarTextures.MoodExtremeCrossedBGTex);
                        GUI.DrawTexture(moodRect.BottomPart(mood.CurLevelPercentage), ColonistBarTextures.MoodExtremeCrossedTex);
                    }

                    DrawMentalThreshold(moodRect, mb.BreakThresholdExtreme, mood.CurLevelPercentage);
                    DrawMentalThreshold(moodRect, mb.BreakThresholdMajor, mood.CurLevelPercentage);
                    DrawMentalThreshold(moodRect, mb.BreakThresholdMinor, mood.CurLevelPercentage);

                    GUI.DrawTexture(new Rect(moodRect.x, moodRect.yMax - moodRect.height * mood.CurInstantLevelPercentage, moodRect.width, 1), ColonistBarTextures.MoodTargetTex);

                    GUI.DrawTexture(new Rect(moodRect.xMax + 1, moodRect.yMax - moodRect.height * mood.CurInstantLevelPercentage - 1, 2, 3), ColonistBarTextures.MoodTargetTex);
                }

            }


            if (colonistAlive)
            {
                DrawSelectionOverlayOnGUI(colonist, rect.ContractedBy(-2f * Scale));
            }

            GUI.DrawTexture(GetPawnTextureRect(rect.x, rect.y), PortraitsCache.Get(colonist, PawnTextureSize, PawnTextureCameraOffset, ColBarSettings.PawnTextureCameraZoom));

            if (ColBarSettings.UseWeaponIcons)
            {
                DrawWeapon(rect, colonist);
            }

            GUI.color = new Color(1f, 1f, 1f, colonistRectAlpha * 0.8f);
            DrawIcons(rect, colonist);
            GUI.color = color;
            if (colonist.Dead)
            {
                GUI.DrawTexture(rect, ColonistBarTextures.DeadColonistTex);
            }
            //       float num = 4f * Scale;
            Vector2 pos = new Vector2(rect.center.x, rect.yMax + 1f * Scale);
            GenWorldUI.DrawPawnLabel(colonist, pos, colonistRectAlpha, rect.width + SpacingHorizontal - 2f, pawnLabelsCache);
            GUI.color = Color.white;
        }

        private static readonly Color HighlightColor = new Color(0.5f, 0.5f, 0.5f, 1f);


        internal static void DrawMentalThreshold(Rect moodRect, float threshold, float currentMood)
        {
            GUI.DrawTexture(new Rect(moodRect.x, moodRect.yMax - moodRect.height * threshold, moodRect.width, 1), ColonistBarTextures.MoodBreakTex);
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
            Vector2 vector = new Vector2(rect.x + 1f, rect.yMax - rect.width / 5 * 2 - 1f);
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
                //        DrawIcon(PSI.PSI.PSIMaterials[Icons.Aggressive].mainTexture as Texture2D, ref vector, colonist.MentalStateDef.LabelCap);
                DrawIcon(ColonistBarTextures.Icon_MentalStateAggro, ref vector, colonist.MentalStateDef.LabelCap);
            }
            else if (colonist.InMentalState)
            {
                //        DrawIcon(PSI.PSI.PSIMaterials[Icons.Dazed].mainTexture as Texture2D, ref vector, colonist.MentalStateDef.LabelCap);
                DrawIcon(ColonistBarTextures.Icon_MentalStateNonAggro, ref vector, colonist.MentalStateDef.LabelCap);
            }
            else if (colonist.InBed() && colonist.CurrentBed().Medical)
            {
                //        DrawIcon(PSI.PSI.PSIMaterials[Icons.Health].mainTexture as Texture2D, ref vector, "ActivityIconMedicalRest".Translate());
                DrawIcon(ColonistBarTextures.Icon_MedicalRest, ref vector, "ActivityIconMedicalRest".Translate());
            }
            else if (colonist.CurJob != null && colonist.jobs.curDriver.asleep)
            {
                //      DrawIcon(PSI.PSI.PSIMaterials[Icons.Tired].mainTexture as Texture2D, ref vector, "ActivityIconSleeping".Translate());
                DrawIcon(ColonistBarTextures.Icon_Sleeping, ref vector, "ActivityIconSleeping".Translate());
            }
            else if (colonist.CurJob != null && colonist.CurJob.def == JobDefOf.FleeAndCower)
            {
                //      DrawIcon(PSI.PSI.PSIMaterials[Icons.Leave].mainTexture as Texture2D, ref vector, "ActivityIconFleeing".Translate());
                DrawIcon(ColonistBarTextures.Icon_Fleeing, ref vector, "ActivityIconFleeing".Translate());
            }
            else if (flag)
            {
                DrawIcon(ColonistBarTextures.Icon_Attacking, ref vector, "ActivityIconAttacking".Translate());
            }
            else if (colonist.mindState.IsIdle && GenDate.DaysPassed >= 1)
            {
                //  DrawIcon(PSI.PSI.PSIMaterials[Icons.Idle].mainTexture as Texture2D, ref vector, "ActivityIconIdle".Translate());
                DrawIcon(ColonistBarTextures.Icon_Idle, ref vector, "ActivityIconIdle".Translate());
            }
            if (false)
            {
                PawnStats pawnStats;
                if (colonist.Dead || colonist.holder != null || !PSI.PSI._statsDict.TryGetValue(colonist, out pawnStats) ||
                    colonist.drafter == null || colonist.skills == null)
                    return;

                if (pawnStats.MentalSanity == MentalStateDefOf.BingingDrugMajor || pawnStats.MentalSanity == MentalStateDefOf.BingingDrugExtreme)
                {
                    Material material = PSI.PSI.PSIMaterials[Icons.Drunk];
                    DrawIcon(material.mainTexture as Texture2D, ref vector, colonist.MentalStateDef.LabelCap);
                }
            }

            if (colonist.IsBurning())
            {
                DrawIcon(ColonistBarTextures.Icon_Burning, ref vector, "ActivityIconBurning".Translate());
            }
        }

        private void DrawIcon(Texture2D icon, ref Vector2 pos, string tooltip)
        {
            float num = ColBarSettings.BaseSizeFloat * 0.4f * Scale;
            Rect rect = new Rect(pos.x, pos.y, num, num);
            GUI.DrawTexture(rect, icon);
            TooltipHandler.TipRegion(rect, tooltip);
            pos.x += num;
        }

        private void DrawWeapon(Rect rect, Pawn colonist)
        {
            float colonistRectAlpha = GetColonistRectAlpha(rect);
            Color color = new Color(1f, 1f, 1f, colonistRectAlpha);
            GUI.color = color;
            Color iconcolor = new Color(0.8f, 0.8f, 0.8f, 0.75f * colonistRectAlpha);
            foreach (ThingWithComps thing in colonist.equipment.AllEquipment)
            {
                Rect rect2 = rect.ContractedBy(rect.width / 3);

                rect2.x += rect.width / 3 - rect.width / 8;
                rect2.y += rect.height / 3 - rect.height / 8;

                if (Mouse.IsOver(rect2))
                {
                    GUI.color = HighlightColor;
                    GUI.DrawTexture(rect2, TexUI.HighlightTex);
                }

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
                // color labe by thing

                if (thing.def.IsMeleeWeapon)
                {
                    GUI.color = new Color(0.7f, 0.0f, 0.0f, colonistRectAlpha);
                }
                if (thing.def.IsRangedWeapon)
                {
                    GUI.color = new Color(0.0f, 0.7f, 0.0f, colonistRectAlpha);
                }
                Widgets.DrawBoxSolid(rect2, iconcolor);
                Widgets.DrawBox(rect2);
                GUI.color = color;
                Rect rect3 = rect2.ContractedBy(rect2.width / 8);

                Widgets.DrawTextureRotated(rect3, resolvedIcon, 0);
            }
        }

        private Pawn SelPawn => Find.Selector.SingleSelectedThing as Pawn;

        private void HandleColonistClicks(Rect rect, Pawn colonist)
        {
            if (Mouse.IsOver(rect) && Event.current.type == EventType.MouseDown)
            {
                if (clickedColonist == colonist && Time.time - clickedAt < ColBarSettings.DoubleClickTime)
                {
                    // use event so it doesn't bubble through
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
                        ((UIRootMap)Find.UIRoot).colonistBar.MarkColonistsListDirty();
                    }));
                    floatOptionList.Add(new FloatMenuOption("CBKF.Settings.ByName".Translate(), delegate
                    {
                        ColBarSettings.SortBy = byName;
                        ((UIRootMap)Find.UIRoot).colonistBar.MarkColonistsListDirty();
                    }));

                    floatOptionList.Add(new FloatMenuOption("CBKF.Settings.SexAge".Translate(), delegate
                    {
                        ColBarSettings.SortBy = sexage;
                        ((UIRootMap)Find.UIRoot).colonistBar.MarkColonistsListDirty();
                    }));

                    floatOptionList.Add(new FloatMenuOption("CBKF.Settings.Mood".Translate(), delegate
                    {
                        ColBarSettings.SortBy = mood;
                        ((UIRootMap)Find.UIRoot).colonistBar.MarkColonistsListDirty();
                    }));
                    floatOptionList.Add(new FloatMenuOption("CBKF.Settings.Health".Translate(), delegate
                    {
                        ColBarSettings.SortBy = health;
                        ((UIRootMap)Find.UIRoot).colonistBar.MarkColonistsListDirty();
                    }));
                    floatOptionList.Add(new FloatMenuOption("CBKF.Settings.Medic".Translate(), delegate
                    {
                        ColBarSettings.SortBy = medic;
                        ((UIRootMap)Find.UIRoot).colonistBar.MarkColonistsListDirty();
                    }));
                    floatOptionList.Add(new FloatMenuOption("CBKF.Settings.Weapons".Translate(), delegate
                    {
                        ColBarSettings.SortBy = weapons;
                        ((UIRootMap)Find.UIRoot).colonistBar.MarkColonistsListDirty();
                    }));

                    floatOptionList.Add(new FloatMenuOption("CBKF.Settings.SettingsColonistBar".Translate(), delegate { Find.WindowStack.Add(new ColonistBarKF_Settings()); }));
                    FloatMenu window = new FloatMenu(floatOptionList, "CBKF.Settings.SortingOptions".Translate());
                    Find.WindowStack.Add(window);

                    // use event so it doesn't bubble through
                    Event.current.Use();
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
            Vector2 textureSize = new Vector2(ColonistBarTextures.SelectedTex.width * num, ColonistBarTextures.SelectedTex.height * num);
            Vector3[] array = SelectionDrawer.SelectionBracketPartsPos(thing, rect.center, rect.size, textureSize, ColBarSettings.BaseSizeFloat * 0.4f * Scale);
            int num2 = 90;
            for (int i = 0; i < 4; i++)
            {
                Widgets.DrawTextureRotated(new Vector2(array[i].x, array[i].z), ColonistBarTextures.SelectedTex, num2, num);
                num2 += 90;
            }
        }



    }
}
