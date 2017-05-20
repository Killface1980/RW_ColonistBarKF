using System.Reflection;
using Harmony;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ColonistBarKF
{
    // patching not working - JIT?!?
    [HarmonyPatch(typeof(ColonistBar), "MarkColonistsDirty")]
    static class MarkColonistsDirty_Prefix
    {
        [HarmonyPrefix]
        private static void MarkColonistsDirty()
        {
            ColonistBar_KF.helper.entriesDirty = true;
            Log.Message("Colonists marked dirty.x02");
        }
    }

    static class MarkDirty_Helper
    {
        public static void Dirty()
        {
            ColonistBar_KF.helper.entriesDirty = true;
            Log.Message("Colonists marked dirty.x02");
        }
    }

    [HarmonyPatch(typeof(Caravan), "Notify_PawnAdded")]
    static class Notify_PawnAdded_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            MarkDirty_Helper.Dirty();
        }
    }

    [HarmonyPatch(typeof(Caravan), "Notify_PawnRemoved")]
    static class Notify_PawnRemoved_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            MarkDirty_Helper.Dirty();
        }
    }
    [HarmonyPatch(typeof(Caravan), "PostAdd")]
    static class PostAdd_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            MarkDirty_Helper.Dirty();
        }
    }
    [HarmonyPatch(typeof(Caravan), "PostRemove")]
    static class PostRemove_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            MarkDirty_Helper.Dirty();
        }
    }

    //  [HarmonyPatch(typeof(PlaySettings), "DoPlaySettingsGlobalControls")]
    static class DoPlaySettingsGlobalControls_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            MarkDirty_Helper.Dirty();
        }
    }
    [HarmonyPatch(typeof(Corpse), "NotifyColonistBar")]
    static class NotifyColonistBar_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            MarkDirty_Helper.Dirty();
        }
    }
    //   [HarmonyPatch(typeof(Game), "AddMap")]
    static class AddMap_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            MarkDirty_Helper.Dirty();
        }
    }
    //   [HarmonyPatch(typeof(Game), "DeinitAndRemoveMap")]
    static class DeinitAndRemoveMap_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            MarkDirty_Helper.Dirty();
        }
    }
    [HarmonyPatch(typeof(MapPawns), "DoListChangedNotifications")]
    static class DoListChangedNotifications_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            MarkDirty_Helper.Dirty();
        }
    }
    [HarmonyPatch(typeof(Pawn), "Kill")]
    static class Kill_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            MarkDirty_Helper.Dirty();
        }
    }
    [HarmonyPatch(typeof(Pawn), "SetFaction")]
    static class SetFaction_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            MarkDirty_Helper.Dirty();
        }
    }
    [HarmonyPatch(typeof(Thing), "DeSpawn")]
    static class DeSpawn_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            MarkDirty_Helper.Dirty();
        }
    }
    [HarmonyPatch(typeof(Thing), "SpawnSetup")]
    static class SpawnSetup_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty(Thing __instance)
        {
            if (__instance is IThingHolder && Find.ColonistBar != null)
                MarkDirty_Helper.Dirty();
        }
    }
    [HarmonyPatch(typeof(ThingOwner), "NotifyColonistBarIfColonistCorpse")]
    static class NotifyColonistBarIfColonistCorpse_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty(Thing __instance)
        {
            Corpse corpse = __instance as Corpse;
            if (corpse != null && !corpse.Bugged && corpse.InnerPawn.Faction != null && corpse.InnerPawn.Faction.IsPlayer && Current.ProgramState == ProgramState.Playing)
                MarkDirty_Helper.Dirty();
        }
    }
    [HarmonyPatch(typeof(Window), "Notify_ResolutionChanged")]
    static class Notify_ResolutionChanged_Postfix
    {
        [HarmonyPostfix]
        private static void MarkColonistsDirty()
        {
            if (Current.ProgramState == ProgramState.Playing)
                MarkDirty_Helper.Dirty();
        }
    }

}
