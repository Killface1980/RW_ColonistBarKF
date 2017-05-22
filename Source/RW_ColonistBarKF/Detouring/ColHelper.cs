using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace ColonistBarKF
{
    public class ColHelper
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

        private static void SortCachedColonists(ref List<Pawn> tmpColonists)
        {
            IOrderedEnumerable<Pawn> orderedEnumerable = null;
            switch (CBKF.ColBarSettings.SortBy)
            {
                case SettingsColonistBar.SortByWhat.vanilla:
                    tmpColonists.SortBy(x => x.thingIDNumber);
                    CBKF.SaveBarSettings();
                    break;

                case SettingsColonistBar.SortByWhat.byName:
                    orderedEnumerable = tmpColonists.OrderBy(x => x?.LabelCap != null).ThenBy(x => x.LabelCap);
                    tmpColonists = orderedEnumerable.ToList();
                    CBKF.SaveBarSettings();
                    break;

                case SettingsColonistBar.SortByWhat.sexage:
                    orderedEnumerable = tmpColonists.OrderBy(x => x?.gender.GetLabel() != null).ThenBy(x => x?.gender.GetLabel()).ThenBy(x => x?.ageTracker.AgeBiologicalYears);
                    tmpColonists = orderedEnumerable.ToList();
                    CBKF.SaveBarSettings();
                    break;

                case SettingsColonistBar.SortByWhat.health:
                    orderedEnumerable = tmpColonists.OrderBy(x => x?.health?.summaryHealth?.SummaryHealthPercent);
                    tmpColonists = orderedEnumerable.ToList();
                    CBKF.SaveBarSettings();
                    break;

                case SettingsColonistBar.SortByWhat.mood:
                    orderedEnumerable = tmpColonists.OrderBy(x => x?.needs?.mood?.CurLevelPercentage);
                    tmpColonists = orderedEnumerable.ToList();
                    CBKF.SaveBarSettings();
                    break;

                case SettingsColonistBar.SortByWhat.weapons:
                    orderedEnumerable = tmpColonists.OrderByDescending(a => a?.equipment?.Primary != null && a.equipment.Primary.def.IsMeleeWeapon)
                        .ThenByDescending(c => c?.equipment?.Primary != null && c.equipment.Primary.def.IsRangedWeapon).ThenByDescending(b => b.skills.AverageOfRelevantSkillsFor(WorkTypeDefOf.Hunting));
                    tmpColonists = orderedEnumerable.ToList();
                    CBKF.SaveBarSettings();
                    break;

                case SettingsColonistBar.SortByWhat.medic:
                    orderedEnumerable = tmpColonists.OrderByDescending(b => b?.skills?.AverageOfRelevantSkillsFor(WorkTypeDefOf.Doctor));
                    tmpColonists = orderedEnumerable.ToList();
                    CBKF.SaveBarSettings();
                    break;

                default:
                    tmpColonists.SortBy(x => x.thingIDNumber);
                    CBKF.SaveBarSettings();
                    break;
            }
        }

    }
}
