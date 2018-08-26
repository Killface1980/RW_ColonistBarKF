namespace ColonistBarKF.Bar
{
    using JetBrains.Annotations;
    using RimWorld;
    using RimWorld.Planet;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Verse;

    // ReSharper disable once InconsistentNaming
    public class ColBarHelper_KF : IExposable
    {
        #region Public Fields

        [NotNull]
        private List<Vector2> _cachedDrawLocs = new List<Vector2>();

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

        public float CachedScale;

        public int DisplayGroupForBar;

        public bool EntriesDirty = true;

        [NotNull]
        public readonly List<Pair<Thing, Map>> TmpColonistsWithMap = new List<Pair<Thing, Map>>();

        #endregion Public Fields

        #region Private Fields

        [NotNull]
        private readonly List<EntryKF> _cachedEntries = new List<EntryKF>();

        [NotNull]
        private readonly List<Map> _tmpMaps = new List<Map>();

        [NotNull]
        private List<Pawn> _tmpPawns = new List<Pawn>();


        #endregion Private Fields

        #region Public Properties

        [NotNull]
        public List<Vector2> DrawLocs => this._cachedDrawLocs;

        [NotNull]
        public List<EntryKF> Entries
        {
            get
            {
                this.CheckRecacheEntries();
                return this._cachedEntries;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public bool AnyBarEntryAt(Vector2 pos)
        {
            if (!this.TryGetEntryAt(pos, out EntryKF entry))
            {
                return false;
            }

            return entry.GroupCount > 0;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref this.DisplayGroupForBar, "displayGroupForBar");
        }

        public bool TryGetEntryAt(Vector2 pos, out EntryKF entry)
        {
            List<Vector2> drawLocs = this._cachedDrawLocs;
            List<EntryKF> entries = this.Entries;
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

            entry = default(EntryKF);
            return false;
        }

        #endregion Public Methods

        #region Private Methods

        private static void SortCachedColonists([NotNull] ref List<Pawn> tmpColonists)
        {
            List<Pawn> sort;
            List<Pawn> others;

            List<Pawn> orderedEnumerable;
            switch (Settings.BarSettings.SortBy)
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
                        .ThenBy(x => x.gender.GetLabel()).ThenBy(x => x.ageTracker?.AgeBiologicalYears).ToList();
                        tmpColonists = orderedEnumerable;
                        break;
                    }

                case SettingsColonistBar.SortByWhat.health:
                    {
                        tmpColonists.SortBy(x => x.health.summaryHealth.SummaryHealthPercent);
                        break;
                    }

                case SettingsColonistBar.SortByWhat.bleedRate:
                    {
                        tmpColonists.SortByDescending(x => x.health.hediffSet.BleedRateTotal);
                        break;
                    }

                case SettingsColonistBar.SortByWhat.mood:
                    {
                        tmpColonists.SortBy(x => x.needs?.mood?.CurInstantLevelPercentage ?? 0f);

                        // tmpColonists.SortBy(x => x.needs.mood.CurLevelPercentage);
                        break;
                    }
                //inverted the order, the melee goes to the end of the list
                case SettingsColonistBar.SortByWhat.weapons:
                    {
                        orderedEnumerable = tmpColonists
                        .OrderByDescending(a => a?.equipment?.Primary?.def?.IsRangedWeapon).ThenByDescending(
                            b => b?.skills?.AverageOfRelevantSkillsFor(WorkTypeDefOf.Hunting))
                        .ThenByDescending(c => c?.equipment?.Primary?.def?.IsMeleeWeapon == true).ToList();
                        tmpColonists = orderedEnumerable;
                        break;
                    }


                case SettingsColonistBar.SortByWhat.medicTendQuality:
                    {
                        sort = tmpColonists.Where(x => !x?.story?.WorkTypeIsDisabled(WorkTypeDefOf.Doctor) ?? false).ToList();
                        others = tmpColonists.Where(x => x?.story?.WorkTypeIsDisabled(WorkTypeDefOf.Doctor) ?? true).ToList();

                        sort.SortByDescending(b => b.GetStatValue(StatDefOf.MedicalTendQuality));
                        others.SortBy(x => x.LabelCap);

                        sort.AddRange(others);

                        tmpColonists = sort;
                        break;
                    }

                case SettingsColonistBar.SortByWhat.medicSurgerySuccess:
                    {

                        sort = tmpColonists.Where(x => !x?.story?.WorkTypeIsDisabled(WorkTypeDefOf.Doctor) ?? false).ToList();
                        others = tmpColonists.Where(x => x?.story?.WorkTypeIsDisabled(WorkTypeDefOf.Doctor) ?? true).ToList();

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

                        sort.SortByDescending(b => b.GetStatValue(StatDefOf.NegotiationAbility));
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
                //shooting accuracy
                case SettingsColonistBar.SortByWhat.shootingAccuracy:
                    {

                        sort = tmpColonists.Where(x => !x?.story?.WorkTypeIsDisabled(WorkTypeDefOf.Hunting) ?? false).ToList();
                        others = tmpColonists.Where(x => x?.story?.WorkTypeIsDisabled(WorkTypeDefOf.Hunting) ?? true).ToList();

                        sort.SortByDescending(b => b.GetStatValue(StatDefOf.ShootingAccuracyPawn));
                        others.SortBy(x => x.LabelCap);

                        sort.AddRange(others);

                        tmpColonists = sort;
                        break;
                    }
                //sort by shotting skill
                case SettingsColonistBar.SortByWhat.shootingSkill:
                    {
                        orderedEnumerable = tmpColonists.OrderByDescending(b => b?.skills?.AverageOfRelevantSkillsFor(WorkTypeDefOf.Hunting)).ToList();
                        tmpColonists = orderedEnumerable;
                        break;
                    }

                    }
                default:
                    {
                        tmpColonists.SortBy(x => x.thingIDNumber);
                        break;
                    }
            }

            Settings.SaveBarSettings();
        }

        public bool ShowGroupFrames
        {
            get
            {
                List<EntryKF> entries = this.Entries;
                int num = -1;
                for (int i = 0; i < entries.Count; i++)
                {
                    num = Mathf.Max(num, entries[i].Group);
                }

                return num >= 1;
            }
        }
        private void CheckRecacheEntries()
        {
            if (!this.EntriesDirty)
            {
                return;
            }

            this.EntriesDirty = false;
            this._cachedEntries.Clear();
            if (Find.PlaySettings.showColonistBar)
            {
                this._tmpMaps.Clear();
                this._tmpMaps.AddRange(Find.Maps);
                this._tmpMaps.SortBy(x => !x.IsPlayerHome, x => x.uniqueID);
                int groupInt = 0;
                foreach (Map tempMap in this._tmpMaps)
                {
                    this._tmpPawns.Clear();
                    this._tmpPawns.AddRange(tempMap.mapPawns.FreeColonists);
                    List<Thing> list = tempMap.listerThings.ThingsInGroup(ThingRequestGroup.Corpse);
                    foreach (Thing thing in list)
                    {
                        if (thing.IsDessicated())
                        {
                            continue;
                        }

                        Pawn innerPawn = ((Corpse)thing).InnerPawn;
                        if (innerPawn == null)
                        {
                            continue;
                        }
                        if (innerPawn.IsColonist)
                        {
                            this._tmpPawns.Add(innerPawn);
                        }
                    }

                    List<Pawn> allPawnsSpawned = tempMap.mapPawns.AllPawnsSpawned;
                    foreach (Corpse corpse in allPawnsSpawned
                                             .Select(spawnedPawn => spawnedPawn.carryTracker.CarriedThing as Corpse).Where(
                                                                                                                           corpse => corpse != null && !corpse.IsDessicated() && corpse.InnerPawn.IsColonist))
                    {
                        this._tmpPawns.Add(corpse.InnerPawn);
                    }

                    // tmpPawns.SortBy((Pawn x) => x.thingIDNumber);
                    SortCachedColonists(ref this._tmpPawns);
                    foreach (Pawn tempPawn in this._tmpPawns)
                    {
                        this._cachedEntries.Add(new EntryKF(tempPawn, tempMap, groupInt, this._tmpPawns.Count));

                        if (Settings.BarSettings.UseGrouping && groupInt != this.DisplayGroupForBar)
                        {
                            if (this._cachedEntries.FindAll(x => x.Group == groupInt).Count > 2)
                            {
                                this._cachedEntries.Add(new EntryKF(null, tempMap, groupInt, this._tmpPawns.Count));
                                break;
                            }
                        }
                    }

                    if (!this._tmpPawns.Any())
                    {
                        this._cachedEntries.Add(new EntryKF(null, tempMap, groupInt, 0));
                    }

                    groupInt++;
                }

                this.TmpCaravans.Clear();
                this.TmpCaravans.AddRange(Find.WorldObjects.Caravans);
                this.TmpCaravans.SortBy(x => x.ID);
                foreach (Caravan caravan in this.TmpCaravans.Where(caravan => caravan.IsPlayerControlled))
                {
                    this._tmpPawns.Clear();
                    this._tmpPawns.AddRange(caravan.PawnsListForReading);

                    // tmpPawns.SortBy((Pawn x) => x.thingIDNumber);
                    SortCachedColonists(ref this._tmpPawns);
                    foreach (Pawn tempPawn in this._tmpPawns.Where(tempPawn => tempPawn.IsColonist))
                    {
                        this._cachedEntries.Add(
                            new EntryKF(
                                tempPawn,
                                null,
                                groupInt,
                                this._tmpPawns.FindAll(x => x.IsColonist).Count));

                        if (Settings.BarSettings.UseGrouping && groupInt != this.DisplayGroupForBar)
                        {
                            if (this._cachedEntries.FindAll(x => x.Group == groupInt).Count > 2)
                            {
                                this._cachedEntries.Add(
                                    new EntryKF(
                                        null,
                                        null,
                                        groupInt,
                                        this._tmpPawns.FindAll(x => x.IsColonist).Count));
                                break;
                            }
                        }
                    }

                    groupInt++;
                }
            }

            // RecacheDrawLocs();
            ColonistBar_KF.Drawer.Notify_RecachedEntries();
            this._tmpPawns.Clear();
            this._tmpMaps.Clear();
            this.TmpCaravans.Clear();
            ColonistBar_KF.DrawLocsFinder.CalculateDrawLocs(this._cachedDrawLocs, out this.CachedScale);
        }

        #endregion Private Methods
    }
}