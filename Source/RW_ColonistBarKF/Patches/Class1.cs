using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColonistBarKF
{
    using System.Reflection;

    using Harmony;

    using RimWorld;

    using Verse;
    using Verse.AI.Group;

    [HarmonyPatch(typeof(Pawn_AgeTracker), "BirthdayBiological")]
    static class BirthdayBiological_Postfix
    {
        private static readonly FieldInfo pawnField = AccessTools.Field(typeof(Pawn_AgeTracker), "pawn");
        private static readonly FieldInfo lastLordStartTickField = AccessTools.Field(typeof(Pawn_AgeTracker), "lastLordStartTick");

        private static bool startPartyASAP;

        private static int lastLordStartTick;

        [HarmonyPostfix]
        private static void BirtdayParty(Pawn_AgeTracker __instance)
        {
            Pawn __result = (Pawn)pawnField.GetValue(__instance);
            lastLordStartTick = (int)lastLordStartTickField.GetValue(__instance);

            if (!__result.IsFreeColonist) return;

            if (!__result.CurrentlyUsable())
                return;

            if (PartyUtility.AcceptableGameConditionsToStartParty(__result.Map))
            {
                Tick_TryStartParty(__result);
            }
        }

        private static void Tick_TryStartParty(Pawn pawn)
        {
            if (!pawn.Map.IsPlayerHome)
            {
                return;
            }



            if (Rand.MTBEventOccurs(40f, 60000f, 5000f))
            {
                startPartyASAP = true;
            }

            if (startPartyASAP && Find.TickManager.TicksGame - lastLordStartTick >= 600000 && PartyUtility.AcceptableGameConditionsToStartParty(pawn.Map))
            {
                TryStartParty(pawn);
            }
        }

        // RimWorld.VoluntarilyJoinableLordsStarter
        public static bool TryStartParty(Pawn pawn)
        {
            if (pawn == null)
            {
                return false;
            }

            IntVec3 intVec;
            if (!RCellFinder.TryFindPartySpot(pawn, out intVec))
            {
                return false;
            }

            LordMaker.MakeNewLord(pawn.Faction, new LordJob_Joinable_Party(intVec), pawn.Map, null);
            Find.LetterStack.ReceiveLetter("LetterLabelNewBirthdayParty".Translate(), "LetterNewBirthdayParty".Translate(new object[]
            {
        pawn.LabelShort
            }), LetterDefOf.Good, new TargetInfo(intVec, pawn.Map, false), null);
            lastLordStartTick = Find.TickManager.TicksGame;
            startPartyASAP = false;
            return true;
        }

    }
}
