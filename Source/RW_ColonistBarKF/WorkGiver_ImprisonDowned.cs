using System;
using Verse;
using Verse.AI;

namespace ColonistBarKF
{
    using JetBrains.Annotations;

    using RimWorld;

    public class WorkGiver_ImprisonDowned : WorkGiver_TakeToBed
    {
        private const float MinDistFromEnemy = 40f;

        public override PathEndMode PathEndMode => PathEndMode.OnCell;

        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Pawn);

        public override bool HasJobOnThing([NotNull] Pawn pawn, [NotNull] Thing t, bool forced = false)
        {
            Pawn victim = t as Pawn;
            if (victim == null || !victim.Downed || victim.InBed() || !pawn.CanReserve(victim, 1, -1, null, forced) || GenAI.EnemyIsNear(victim, 10f))
            {
                return false;
            }

            bool flag = false;
            foreach (Building building in pawn.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("MedicalBeacon")))
            {
                if (victim.Position.InHorDistOf(building.Position, building.def.specialDisplayRadius))
                {
                    flag = true;
                    break;
                }
            }

            if (flag == false)
            {
                return false;
            }

            if (victim.Faction.HostileTo(Faction.OfPlayer))
            {
                Building_Bed building_Bed = RestUtility.FindBedFor(victim, pawn, true, false, false);
                if (building_Bed == null)
                {
                    building_Bed = RestUtility.FindBedFor(victim, pawn, true, false, true);
                }

                return building_Bed != null && victim.CanReserve(building_Bed, 1, -1, null, false);
            }
            else
            {
                Building_Bed building_Bed = RestUtility.FindBedFor(victim, pawn, victim.HostFaction == pawn.Faction, false, false);
                return building_Bed != null && victim.CanReserve(building_Bed, 1, -1, null, false);
            }
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Pawn victim = t as Pawn;

            bool hostile = victim.Faction.HostileTo(Faction.OfPlayer);

            if (hostile)
            {
                Building_Bed building_Bed = RestUtility.FindBedFor(victim, pawn, true, false, false);
                if (building_Bed == null)
                {
                    building_Bed = RestUtility.FindBedFor(victim, pawn, true, false, true);
                }

                if (building_Bed == null)
                {
                    Messages.Message("CannotCapture".Translate() + ": " + "NoPrisonerBed".Translate(), victim, MessageSound.RejectInput);
                    return null;
                }

                Job job = new Job(JobDefOf.Capture, victim, building_Bed) { count = 1 };

                return job;
            }
            else
            {
                Building_Bed building_Bed = RestUtility.FindBedFor(victim, pawn, victim.HostFaction == pawn.Faction, false, false);
                if (building_Bed == null)
                {
                    Messages.Message("CannotTreat".Translate() + ": " + "NoGuestBed".Translate(), victim, MessageSound.RejectInput);
                    return null;
                }

                Job job = new Job(JobDefOf.Rescue, victim, building_Bed) { count = 1 };

                return job;

            }

        }
    }
}
