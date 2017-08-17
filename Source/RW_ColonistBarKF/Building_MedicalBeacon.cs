namespace Outfitter
{
    using RimWorld;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Verse;
    using Verse.AI;

    [StaticConstructorOnStartup]
    public class Building_MedicalBeacon : Building
    {

        private int ticksToDespawn;

        int prisonerCount = 0;
        int guestCount = 0;

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            string inspectString = base.GetInspectString();
            if (!inspectString.NullOrEmpty())
            {
                stringBuilder.AppendLine(inspectString);
            }
            if (base.Spawned)
            {

                if (this.prisonerCount + this.guestCount > 0)
                {
                    stringBuilder.AppendLine("PotentialPrisoners".Translate() + ": " + prisonerCount);
                    stringBuilder.AppendLine("PotentialGuests".Translate() + ": " + guestCount);

                }
                else if (this.ticksToDespawn > 0)
                {
                    stringBuilder.AppendLine("WillDespawnIn".Translate() + ": " + this.ticksToDespawn.TicksToSecondsString());
                }
            }

            return stringBuilder.ToString().TrimEndNewlines();
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            this.ticksToDespawn = 3000;
        }

        public override void Tick()
        {
            base.Tick();

            this.prisonerCount = 0;
            this.guestCount = 0;

            foreach (Pawn pawn in this.Map.mapPawns.AllPawnsSpawned)
            {
                if (this.Position.InHorDistOf(pawn.Position, this.def.specialDisplayRadius))
                {
                    if (!pawn.RaceProps.Humanlike)
                    {
                        continue;
                    }
                    if (pawn.Downed && !pawn.InBed())
                    {
                        if (pawn.Faction.HostileTo(Faction.OfPlayer))
                        {
                            prisonerCount++;
                        }
                        else if (pawn.Faction!= Faction.OfPlayer)
                        {
                            guestCount++;
                        }
                    }
                }
            }

            if (this.prisonerCount + this.guestCount > 0)
            {
                return;
            }

            this.ticksToDespawn--;

            if (this.ticksToDespawn == 0)
            {
                this.Destroy(DestroyMode.Deconstruct);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.ticksToDespawn, "ticksToDespawn");
        }
    }
}