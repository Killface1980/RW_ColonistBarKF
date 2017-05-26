using System;
using System.Reflection;
using Harmony;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace ColonistBarKF
{
  //// patching not working - JIT?!?
  //[HarmonyPatch(typeof(PawnRenderer), "RenderPawnAt", new Type[] { typeof(Vector3) })]
  //static class RenderPawnAt_Postfix
  //{
  //    private static readonly FieldInfo pawnField = AccessTools.Field(typeof(PawnRenderer), "pawn");
  //
  //    [HarmonyPostfix]
  //    private static void MarkColonistsDirty(PawnRenderer __instance)
  //    {
  //        Pawn __result = (Pawn)pawnField.GetValue(__instance);
  //        if (__result.RaceProps.Animal)
  //            PSI.PSI.DrawAnimalIcons(__result);
  //
  //        else if ( (Settings.PsiSettings.UsePsi && __result.IsColonist) || (Settings.PsiSettings.UsePsiOnPrisoner && __result.IsPrisoner))
  //        {
  //            PSI.PSI.DrawColonistIcons(__result, true);
  //        }
  //
  //        //           Log.Message("Colonists marked dirty.x02");
  //    }
  //}

    // patching not working - JIT?!?
    [HarmonyPatch(typeof(ColonistBar), "MarkColonistsDirty")]
    static class MarkColonistsDirty_Prefix
    {
        [HarmonyPrefix]
        private static void MarkColonistsDirty()
        {
            ColonistBar_KF.BarHelperKf.entriesDirty = true;
            //           Log.Message("Colonists marked dirty.x02");
        }
    }

    static class MarkDirty_Helper
    {
        public static void Dirty()
        {
            ColonistBar_KF.BarHelperKf.entriesDirty = true;
        }
    }

    [HarmonyPatch(typeof(Caravan), "Notify_PawnAdded")]
    static class Notify_PawnAdded_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            MarkDirty_Helper.Dirty();
            //          Log.Message("Colonists marked dirty.x02");
        }
    }

    [HarmonyPatch(typeof(Caravan), "Notify_PawnRemoved")]
    static class Notify_PawnRemoved_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            MarkDirty_Helper.Dirty();
            //       Log.Message("Colonists marked dirty.x03");
        }
    }
    [HarmonyPatch(typeof(Caravan), "PostAdd")]
    static class PostAdd_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            MarkDirty_Helper.Dirty();
            //         Log.Message("Colonists marked dirty.x04");
        }
    }
    [HarmonyPatch(typeof(Caravan), "PostRemove")]
    static class PostRemove_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            MarkDirty_Helper.Dirty();
            //        Log.Message("Colonists marked dirty.x05");
        }
    }

    //  [HarmonyPatch(typeof(PlaySettings), "DoPlaySettingsGlobalControls")]
    static class DoPlaySettingsGlobalControls_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            MarkDirty_Helper.Dirty();
            //        Log.Message("Colonists marked dirty.x06");
        }
    }
    [HarmonyPatch(typeof(Corpse), "NotifyColonistBar")]
    static class NotifyColonistBar_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty(Corpse __instance)
        {
            Pawn InnerPawn = __instance.InnerPawn;

            if (InnerPawn == null)
                return;

            if (InnerPawn.Faction == Faction.OfPlayer && Current.ProgramState == ProgramState.Playing)
            {
                MarkDirty_Helper.Dirty();
                //              Log.Message("Colonists marked dirty.x07");
            }
        }
    }
    [HarmonyPatch(typeof(Game), "AddMap")]
    static class AddMap_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            MarkDirty_Helper.Dirty();
            //        Log.Message("Colonists marked dirty.x08");
        }
    }
    [HarmonyPatch(typeof(Game), "DeinitAndRemoveMap")]
    static class DeinitAndRemoveMap_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            if (Current.ProgramState != ProgramState.Playing)
                return;

            MarkDirty_Helper.Dirty();
            //         Log.Message("Colonists marked dirty.x09");
        }
    }
    [HarmonyPatch(typeof(MapPawns), "DoListChangedNotifications")]
    static class DoListChangedNotifications_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            if (Find.ColonistBar != null)
            {
                MarkDirty_Helper.Dirty();
                //             Log.Message("Colonists marked dirty.x10");
            }
        }
    }
    [HarmonyPatch(typeof(Pawn), "Kill")]
    static class Kill_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty(Pawn __instance)
        {
            if (__instance.Faction != null && __instance.Faction.IsPlayer && Current.ProgramState == ProgramState.Playing)
            {
                MarkDirty_Helper.Dirty();
                //              Log.Message("Colonists marked dirty.x11");
            }
        }
    }
    [HarmonyPatch(typeof(Pawn), "SetFaction")]
    static class SetFaction_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            MarkDirty_Helper.Dirty();
            //         Log.Message("Colonists marked dirty.x12");
        }
    }
    [HarmonyPatch(typeof(Thing), "DeSpawn")]
    static class DeSpawn_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty(Thing __instance)
        {
            var pawn = __instance as Pawn;
            if (pawn == null)
                return;
            if (pawn.Faction != Faction.OfPlayer)
                return;
            if (!pawn.RaceProps.Humanlike)
                return;

            MarkDirty_Helper.Dirty();
            //          Log.Message("Colonists marked dirty.x13");
        }
    }
    [HarmonyPatch(typeof(Thing), "SpawnSetup")]
    static class SpawnSetup_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty(Thing __instance)
        {
            var pawn = __instance as Pawn;
            if (pawn == null)
                return;
            if (pawn.Faction != Faction.OfPlayer)
                return;
            if (!pawn.RaceProps.Humanlike)
                return;

            if (__instance is IThingHolder && Find.ColonistBar != null)
            {
                MarkDirty_Helper.Dirty();
                //               Log.Message("Colonists marked dirty.x14");
            }
        }
    }
    [HarmonyPatch(typeof(ThingOwner), "NotifyColonistBarIfColonistCorpse")]
    static class NotifyColonistBarIfColonistCorpse_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty(Thing __instance)
        {
            Corpse corpse = __instance as Corpse;
            if (corpse != null && !corpse.Bugged && corpse.InnerPawn.Faction != null &&
                corpse.InnerPawn.Faction.IsPlayer && Current.ProgramState == ProgramState.Playing)
            {
                MarkDirty_Helper.Dirty();
                //             Log.Message("Colonists marked dirty.x15");
            }
        }
    }
    [HarmonyPatch(typeof(Window), "Notify_ResolutionChanged")]
    static class Notify_ResolutionChanged_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            if (Current.ProgramState == ProgramState.Playing)
            {
                MarkDirty_Helper.Dirty();
                //           Log.Message("Colonists marked dirty.x16");
            }
        }
    }

}
