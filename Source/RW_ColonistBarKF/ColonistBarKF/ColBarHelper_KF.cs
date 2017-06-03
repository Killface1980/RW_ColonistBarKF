using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using static ColonistBarKF.Settings;

namespace ColonistBarKF
{
    public class ColBarHelper_KF
    {
        public bool entriesDirty = true;

        public List<Vector2> cachedDrawLocs = new List<Vector2>();

        public List<Vector2> DrawLocs
        {
            get
            {
                return cachedDrawLocs;
            }
        }

        public float cachedScale;

        public List<ColonistBar.Entry> cachedEntries = new List<ColonistBar.Entry>();

        public List<Pawn> tmpPawns = new List<Pawn>();

        public List<Map> tmpMaps = new List<Map>();

        public List<Caravan> tmpCaravans = new List<Caravan>();

        public List<Pawn> tmpColonistsInOrder = new List<Pawn>();

        public List<Pair<Thing, Map>> tmpColonistsWithMap = new List<Pair<Thing, Map>>();

        public List<Thing> tmpColonists = new List<Thing>();

        public List<Thing> tmpMapColonistsOrCorpsesInScreenRect = new List<Thing>();

        public List<Pawn> tmpCaravanPawns = new List<Pawn>();

        public List<ColonistBar.Entry> Entries
        {
            get
            {
                CheckRecacheEntries();
                return cachedEntries;
            }
        }

        public void CheckRecacheEntries()
        {
            if (!entriesDirty)
            {
                return;
            }
            entriesDirty = false;
            cachedEntries.Clear();
            if (Find.PlaySettings.showColonistBar)
            {
                tmpMaps.Clear();
                tmpMaps.AddRange(Find.Maps);
                tmpMaps.SortBy(x => !x.IsPlayerHome, x => x.uniqueID);
                int num = 0;
                for (int i = 0; i < tmpMaps.Count; i++)
                {
                    tmpPawns.Clear();
                    tmpPawns.AddRange(tmpMaps[i].mapPawns.FreeColonists);
                    List<Thing> list = tmpMaps[i].listerThings.ThingsInGroup(ThingRequestGroup.Corpse);
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (!list[j].IsDessicated())
                        {
                            Pawn innerPawn = ((Corpse)list[j]).InnerPawn;
                            if (innerPawn != null)
                            {
                                if (innerPawn.IsColonist)
                                {
                                    tmpPawns.Add(innerPawn);
                                }
                            }
                        }
                    }
                    List<Pawn> allPawnsSpawned = tmpMaps[i].mapPawns.AllPawnsSpawned;
                    for (int k = 0; k < allPawnsSpawned.Count; k++)
                    {
                        Corpse corpse = allPawnsSpawned[k].carryTracker.CarriedThing as Corpse;
                        if (corpse != null && !corpse.IsDessicated() && corpse.InnerPawn.IsColonist)
                        {
                            tmpPawns.Add(corpse.InnerPawn);
                        }
                    }
                    //         tmpPawns.SortBy((Pawn x) => x.thingIDNumber);
                    SortCachedColonists(ref tmpPawns);
                    for (int l = 0; l < tmpPawns.Count; l++)
                    {
                        cachedEntries.Add(new ColonistBar.Entry(tmpPawns[l], tmpMaps[i], num));
                    }
                    if (!tmpPawns.Any())
                    {
                        cachedEntries.Add(new ColonistBar.Entry(null, tmpMaps[i], num));
                    }
                    num++;
                }
                tmpCaravans.Clear();
                tmpCaravans.AddRange(Find.WorldObjects.Caravans);
                tmpCaravans.SortBy(x => x.ID);
                for (int m = 0; m < tmpCaravans.Count; m++)
                {
                    if (tmpCaravans[m].IsPlayerControlled)
                    {
                        tmpPawns.Clear();
                        tmpPawns.AddRange(tmpCaravans[m].PawnsListForReading);
                        //  tmpPawns.SortBy((Pawn x) => x.thingIDNumber);
                        SortCachedColonists(ref tmpPawns);
                        for (int n = 0; n < tmpPawns.Count; n++)
                        {
                            if (tmpPawns[n].IsColonist)
                            {
                                cachedEntries.Add(new ColonistBar.Entry(tmpPawns[n], null, num));
                            }
                        }
                        num++;
                    }
                }
            }
            //        RecacheDrawLocs();
            ColonistBar_KF.drawer.Notify_RecachedEntries();
            tmpPawns.Clear();
            tmpMaps.Clear();
            tmpCaravans.Clear();
            ColonistBar_KF.drawLocsFinder.CalculateDrawLocs(cachedDrawLocs, out cachedScale);
        }

        public bool AnyColonistOrCorpseAt(Vector2 pos)
        {
            ColonistBar.Entry entry;
            return TryGetEntryAt(pos, out entry) && entry.pawn != null;
        }

        public bool TryGetEntryAt(Vector2 pos, out ColonistBar.Entry entry)
        {
            List<Vector2> drawLocs = cachedDrawLocs;
            List<ColonistBar.Entry> entries = Entries;
            Vector2 size = ColonistBar_KF.FullSize;
            for (int i = 0; i < drawLocs.Count; i++)
            {
                Rect rect = new Rect(drawLocs[i].x, drawLocs[i].y, size.x, size.y);
                if (rect.Contains(pos))
                {
                    entry = entries[i];
                    return true;
                }
            }

            entry = default(ColonistBar.Entry);
            return false;
        }


        private static void SortCachedColonists(ref List<Pawn> tmpColonists)
        {
            IOrderedEnumerable<Pawn> orderedEnumerable = null;
            switch (ColBarSettings.SortBy)
            {
                case SettingsColonistBar.SortByWhat.vanilla:
                    tmpColonists.SortBy(x => x.thingIDNumber);
                    SaveBarSettings();
                    break;

                case SettingsColonistBar.SortByWhat.byName:
                    tmpColonists.SortBy(x => x.LabelCap);
                    SaveBarSettings();
                    break;

                case SettingsColonistBar.SortByWhat.sexage:
                    orderedEnumerable = tmpColonists.OrderBy(x => x?.gender.GetLabel() != null).ThenBy(x => x?.gender.GetLabel()).ThenBy(x => x?.ageTracker?.AgeBiologicalYears);
                    tmpColonists = orderedEnumerable.ToList();
                    SaveBarSettings();
                    break;

                case SettingsColonistBar.SortByWhat.health:
                    tmpColonists.SortBy(x => x.health.summaryHealth.SummaryHealthPercent);
                    SaveBarSettings();
                    break;

                case SettingsColonistBar.SortByWhat.mood:
                    orderedEnumerable = tmpColonists.OrderBy(x => x?.needs?.mood.CurInstantLevelPercentage);
                    tmpColonists = orderedEnumerable.ToList();
                    //    tmpColonists.SortBy(x => x.needs.mood.CurLevelPercentage);
                    SaveBarSettings();
                    break;

                case SettingsColonistBar.SortByWhat.weapons:
                    orderedEnumerable = tmpColonists.OrderByDescending(a => a?.equipment?.Primary?.def != null && a.equipment.Primary.def.IsMeleeWeapon)
                        .ThenByDescending(c => c?.equipment?.Primary?.def?.IsRangedWeapon).ThenByDescending(b => b?.skills?.AverageOfRelevantSkillsFor(WorkTypeDefOf.Hunting));
                    tmpColonists = orderedEnumerable.ToList();
                    SaveBarSettings();
                    break;

                case SettingsColonistBar.SortByWhat.medic:
                    orderedEnumerable = tmpColonists.OrderBy(b => b?.skills != null).ThenByDescending(b => b?.skills.AverageOfRelevantSkillsFor(WorkTypeDefOf.Doctor));
                    tmpColonists = orderedEnumerable.ToList();
                    SaveBarSettings();
                    break;

                case SettingsColonistBar.SortByWhat.medicSurgerySuccess:
                    tmpColonists.SortByDescending(b => b.GetStatValue(StatDefOf.MedicalSurgerySuccessChance));
                    SaveBarSettings();
                    break;

                default:
                    tmpColonists.SortBy(x => x.thingIDNumber);
                    SaveBarSettings();
                    break;
            }
        }

    }
}
