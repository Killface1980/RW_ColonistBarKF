using System.Collections.Generic;
using System.Linq;
using ColonistBarKF.Settings;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace ColonistBarKF.Bar
{
    // ReSharper disable once InconsistentNaming
    public class ColBarHelper_KF : IExposable
    {
        #region Public Fields

        [NotNull]
        public readonly List<Pair<Thing, Map>> TmpColonistsWithMap = new List<Pair<Thing, Map>>();

        public float CachedScale;

        public int DisplayGroupForBar;

        public bool EntriesDirty = true;

        [NotNull]
        public List<Pawn> TmpCaravanPawns = new List<Pawn>();

        [NotNull]
        public List<Caravan> TmpCaravans = new List<Caravan>();

        [NotNull]
        public List<Thing> TmpColonists = new List<Thing>();

        [NotNull]
        public List<Pawn> TmpColonistsInOrder = new List<Pawn>();

        [NotNull]
        public List<Thing> TmpMapColonistsOrCorpsesInScreenRect = new List<Thing>();

        #endregion Public Fields

        #region Private Fields

        [NotNull]
        private readonly List<Vector2> _cachedDrawLocs = new List<Vector2>();

        [NotNull]
        private readonly List<EntryKf> _cachedEntries = new List<EntryKf>();

        [NotNull]
        private readonly List<Map> _tmpMaps = new List<Map>();
        [NotNull]
        private List<Pawn> _tmpPawns = new List<Pawn>();

        #endregion Private Fields

        #region Public Properties

        [NotNull]
        public List<Vector2> DrawLocs => _cachedDrawLocs;

        [NotNull]
        public List<EntryKf> Entries
        {
            get
            {
                CheckRecacheEntries();
                return _cachedEntries;
            }
        }

        public bool ShowGroupFrames
        {
            get
            {
                List<EntryKf> entries = Entries;
                int num = -1;
                for (int i = 0; i < entries.Count; i++)
                {
                    num = Mathf.Max(num, entries[i].Group);
                }

                return num >= 1;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public bool AnyBarEntryAt(Vector2 pos)
        {
            if (!TryGetEntryAt(pos, out EntryKf entry))
            {
                return false;
            }

            return entry.GroupCount > 0;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref DisplayGroupForBar, "displayGroupForBar");
        }

        public bool TryGetEntryAt(Vector2 pos, out EntryKf entry)
        {
            List<Vector2> drawLocs = _cachedDrawLocs;
            List<EntryKf> entries = Entries;
            Vector2 size = ColonistBar_Kf.FullSize;
            for (int i = 0; i < drawLocs.Count; i++)
            {
                Rect rect = new Rect(drawLocs[i].x, drawLocs[i].y, size.x, size.y);
                if (rect.Contains(pos))
                {
                    entry = entries[i];
                    return true;
                }
            }

            entry = default(EntryKf);
            return false;
        }

        #endregion Public Methods

        #region Private Methods

        private static void SortCachedColonists([NotNull] ref List<Pawn> tmpColonists)
        {
            List<Pawn> sort = new List<Pawn>();
            List<Pawn> others = new List<Pawn>();

            List<Pawn> orderedEnumerable;
            switch (Settings.Settings.BarSettings.SortBy)
            {
                case SettingsColonistBar.SortByWhat.vanilla:
                    {
                        tmpColonists.SortBy(x => x.thingIDNumber);
                        break;
                    }

                case SettingsColonistBar.SortByWhat.byName:
                    {
                        tmpColonists.SortBy(x => x.LabelCap);
                        break;
                    }

                case SettingsColonistBar.SortByWhat.sexage:
                    {
                        orderedEnumerable = tmpColonists.OrderBy(x => x.gender.GetLabel() != null)
                            .ThenBy(x => x.gender.GetLabel()).ThenBy(x => x?.ageTracker?.AgeBiologicalYears).ToList();
                        tmpColonists = orderedEnumerable;
                        break;
                    }

                case SettingsColonistBar.SortByWhat.health:
                    {
                        tmpColonists.SortBy(x => x?.health?.summaryHealth?.SummaryHealthPercent ?? 0f);
                        break;
                    }

                case SettingsColonistBar.SortByWhat.bleedRate:
                    {
                        tmpColonists.SortByDescending(x => x?.health?.hediffSet?.BleedRateTotal ?? 0f);
                        break;
                    }

                case SettingsColonistBar.SortByWhat.mood:
                    {
                        tmpColonists.SortBy(x => x?.needs?.mood?.CurInstantLevelPercentage ?? 0f);

                        // tmpColonists.SortBy(x => x.needs.mood.CurLevelPercentage);
                        break;
                    }

                case SettingsColonistBar.SortByWhat.weapons:
                    {
                        orderedEnumerable = tmpColonists
                            .OrderByDescending(a => a?.equipment?.Primary?.def?.IsMeleeWeapon == true)
                            .ThenByDescending(c => c?.equipment?.Primary?.def?.IsRangedWeapon).ThenByDescending(
                                b => b?.skills?.AverageOfRelevantSkillsFor(WorkTypeDefOf.Hunting)).ToList();
                        tmpColonists = orderedEnumerable;
                        break;
                    }

                case SettingsColonistBar.SortByWhat.medicTendQuality:
                    {
                        sort = tmpColonists.Where(x => !x?.story?.WorkTypeIsDisabled(WorkTypeDefOf.Doctor) ?? false)
                            .ToList();
                        others = tmpColonists.Where(x => x?.story?.WorkTypeIsDisabled(WorkTypeDefOf.Doctor) ?? true)
                            .ToList();

                        sort.SortByDescending(b => b.GetStatValue(StatDefOf.MedicalTendQuality));
                        others.SortBy(x => x.LabelCap);

                        sort.AddRange(others);

                        tmpColonists = sort;
                        break;
                    }

                case SettingsColonistBar.SortByWhat.medicSurgerySuccess:
                    {
                        sort = tmpColonists.Where(x => !x?.story?.WorkTypeIsDisabled(WorkTypeDefOf.Doctor) ?? false)
                            .ToList();
                        others = tmpColonists.Where(x => x?.story?.WorkTypeIsDisabled(WorkTypeDefOf.Doctor) ?? true)
                            .ToList();

                        sort.SortByDescending(b => b.GetStatValue(StatDefOf.MedicalSurgerySuccessChance));
                        others.SortBy(x => x.LabelCap);

                        sort.AddRange(others);

                        tmpColonists = sort;
                        break;
                    }

                case SettingsColonistBar.SortByWhat.diplomacy:
                    {
                        sort = tmpColonists.Where(x => !x?.story?.WorkTagIsDisabled(WorkTags.Social) ?? false).ToList();
                        others = tmpColonists.Where(x => x?.story?.WorkTagIsDisabled(WorkTags.Social) ?? true).ToList();

                        sort.SortByDescending(b => b.GetStatValue(StatDefOf.DiplomacyPower));
                        others.SortBy(x => x.LabelCap);

                        sort.AddRange(others);

                        tmpColonists = sort;
                        break;
                    }

                case SettingsColonistBar.SortByWhat.tradePrice:
                    {
                        sort = tmpColonists.Where(x => !x?.story?.WorkTagIsDisabled(WorkTags.Social) ?? false).ToList();
                        others = tmpColonists.Where(x => x?.story?.WorkTagIsDisabled(WorkTags.Social) ?? true).ToList();

                        sort.SortByDescending(b => b.GetStatValue(StatDefOf.TradePriceImprovement));
                        others.SortBy(x => x.LabelCap);

                        sort.AddRange(others);

                        tmpColonists = sort;
                        break;
                    }

                default:
                    {
                        tmpColonists.SortBy(x => x.thingIDNumber);
                        break;
                    }
            }

            Settings.Settings.SaveBarSettings();
        }

        private void CheckRecacheEntries()
        {
            if (!EntriesDirty)
            {
                return;
            }

            EntriesDirty = false;
            _cachedEntries.Clear();
            if (Find.PlaySettings.showColonistBar)
            {
                _tmpMaps.Clear();
                _tmpMaps.AddRange(Find.Maps);
                _tmpMaps.SortBy(x => !x.IsPlayerHome, x => x.uniqueID);
                int groupInt = 0;
                for (int index = 0; index < _tmpMaps.Count; index++)
                {
                    Map tempMap = _tmpMaps[index];
                    _tmpPawns.Clear();
                    _tmpPawns.AddRange(tempMap.mapPawns.FreeColonists);
                    List<Thing> list = tempMap.listerThings.ThingsInGroup(ThingRequestGroup.Corpse);
                    for (int i = 0; i < list.Count; i++)
                    {
                        Thing thing = list[i];
                        if (!thing.IsDessicated())
                        {
                            Pawn innerPawn = ((Corpse)thing).InnerPawn;
                            if (innerPawn == null)
                            {
                                continue;
                            }

                            if (innerPawn.IsColonist)
                            {
                                _tmpPawns.Add(innerPawn);
                            }
                        }
                    }

                    List<Pawn> allPawnsSpawned = tempMap.mapPawns.AllPawnsSpawned;
                    foreach (Corpse corpse in allPawnsSpawned
                        .Select(spawnedPawn => spawnedPawn.carryTracker.CarriedThing as Corpse).Where(
                            corpse => corpse != null && !corpse.IsDessicated() && corpse.InnerPawn.IsColonist))
                    {
                        _tmpPawns.Add(corpse.InnerPawn);
                    }

                    // tmpPawns.SortBy((Pawn x) => x.thingIDNumber);
                    SortCachedColonists(ref _tmpPawns);
                    foreach (Pawn tempPawn in _tmpPawns)
                    {
                        _cachedEntries.Add(new EntryKf(tempPawn, tempMap, groupInt, _tmpPawns.Count));

                        if (Settings.Settings.BarSettings.UseGrouping && groupInt != DisplayGroupForBar)
                        {
                            if (_cachedEntries.FindAll(x => x.Group == groupInt).Count > 2)
                            {
                                _cachedEntries.Add(new EntryKf(null, tempMap, groupInt, _tmpPawns.Count));
                                break;
                            }
                        }
                    }

                    if (!_tmpPawns.Any())
                    {
                        _cachedEntries.Add(new EntryKf(null, tempMap, groupInt, 0));
                    }

                    groupInt++;
                }

                TmpCaravans.Clear();
                TmpCaravans.AddRange(Find.WorldObjects.Caravans);
                TmpCaravans.SortBy(x => x.ID);
                foreach (Caravan caravan in TmpCaravans.Where(caravan => caravan.IsPlayerControlled))
                {
                    _tmpPawns.Clear();
                    _tmpPawns.AddRange(caravan.PawnsListForReading);

                    // tmpPawns.SortBy((Pawn x) => x.thingIDNumber);
                    SortCachedColonists(ref _tmpPawns);
                    foreach (Pawn tempPawn in _tmpPawns.Where(tempPawn => tempPawn.IsColonist))
                    {
                        _cachedEntries.Add(
                            new EntryKf(tempPawn, null, groupInt, _tmpPawns.FindAll(x => x.IsColonist).Count));

                        if (Settings.Settings.BarSettings.UseGrouping && groupInt != DisplayGroupForBar)
                        {
                            if (_cachedEntries.FindAll(x => x.Group == groupInt).Count > 2)
                            {
                                _cachedEntries.Add(
                                    new EntryKf(null, null, groupInt, _tmpPawns.FindAll(x => x.IsColonist).Count));
                                break;
                            }
                        }
                    }

                    groupInt++;
                }
            }

            // RecacheDrawLocs();
            ColonistBar_Kf.Drawer.Notify_RecachedEntries();
            _tmpPawns.Clear();
            _tmpMaps.Clear();
            TmpCaravans.Clear();
            ColonistBar_Kf.DrawLocsFinder.CalculateDrawLocs(_cachedDrawLocs, out CachedScale);
        }

        #endregion Private Methods
    }
}