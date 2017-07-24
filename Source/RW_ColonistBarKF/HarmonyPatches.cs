namespace FacialStuff.Detouring
{
    using ColonistBarKF.Bar;
    using Harmony;
    using RimWorld;
    using RimWorld.Planet;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Verse;

    using static ColonistBarKF.Bar.ColonistBar_KF;

    [StaticConstructorOnStartup]
    internal class HarmonyPatches
    {
        #region Fields

        private static bool flag;

        #endregion Fields

        #region Constructors

        static HarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("com.colonistbarkf.rimworld.mod");

            harmony.Patch(
                AccessTools.Method(typeof(ColonistBar), nameof(ColonistBar.ColonistBarOnGUI)),
                new HarmonyMethod(typeof(ColonistBar_KF), nameof(ColonistBarOnGUI_Prefix)),
                null);

            harmony.Patch(
                AccessTools.Method(typeof(ColonistBar), nameof(ColonistBar.MapColonistsOrCorpsesInScreenRect)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(MapColonistsOrCorpsesInScreenRect_Prefix)),
                null);

            harmony.Patch(
                AccessTools.Method(typeof(ColonistBar), nameof(ColonistBar.CaravanMembersCaravansInScreenRect)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(CaravanMembersCaravansInScreenRect_Prefix)),
                null);

            harmony.Patch(
                AccessTools.Method(typeof(ColonistBar), nameof(ColonistBar.ColonistOrCorpseAt)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(ColonistOrCorpseAt_Prefix)),
                null);

            harmony.Patch(
                AccessTools.Method(typeof(ColonistBar), nameof(ColonistBar.CaravanMemberCaravanAt)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(CaravanMemberCaravanAt_Prefix)),
                null);

            harmony.Patch(
                AccessTools.Method(typeof(ColonistBar), nameof(ColonistBar.GetColonistsInOrder)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(GetColonistsInOrder_Prefix)),
                null);

            harmony.Patch(
                AccessTools.Method(typeof(ColonistBar), nameof(ColonistBar.MarkColonistsDirty)),
                null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(MarkColonistsDirty_Postfix)));

            harmony.Patch(
                AccessTools.Method(typeof(Caravan), nameof(Caravan.Notify_PawnAdded)),
                null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(Dirty_Postfix)));

            harmony.Patch(
                AccessTools.Method(typeof(Caravan), nameof(Caravan.Notify_PawnRemoved)),
                null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(Dirty_Postfix)));

            harmony.Patch(
                AccessTools.Method(typeof(Caravan), nameof(Caravan.PostAdd)),
                null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(Dirty_Postfix)));

            harmony.Patch(
                AccessTools.Method(typeof(Caravan), nameof(Caravan.PostRemove)),
                null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(Dirty_Postfix)));

            harmony.Patch(
                AccessTools.Method(typeof(Game), nameof(Game.AddMap)),
                null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(Dirty_Postfix)));

            harmony.Patch(
                AccessTools.Method(typeof(Window), nameof(Window.Notify_ResolutionChanged)),
                null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(IsPlayingDirty_Postfix)));

            harmony.Patch(
                AccessTools.Method(typeof(Game), nameof(Game.DeinitAndRemoveMap)),
                null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(IsPlayingDirty_Postfix)));

            harmony.Patch(
                AccessTools.Method(typeof(Pawn), nameof(Pawn.SetFaction)),
                null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(Dirty_Postfix)));

            harmony.Patch(
                AccessTools.Method(typeof(Thing), nameof(Thing.SpawnSetup)),
                null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(SpawnSetup_Postfix)));

            harmony.Patch(
                AccessTools.Method(typeof(Pawn), nameof(Pawn.Kill)),
                null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(Kill_Postfix)));

            harmony.Patch(
                AccessTools.Method(typeof(Corpse), "NotifyColonistBar"),
                null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(NotifyColonistBar_Postfix)));

            harmony.Patch(
                AccessTools.Method(typeof(MapPawns), "DoListChangedNotifications"),
                null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(IsColonistBarNull_Postfix)));

            harmony.Patch(
                AccessTools.Method(typeof(ThingOwner), "NotifyColonistBarIfColonistCorpse"),
                null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(NotifyColonistBarIfColonistCorpse_Postfix)));

            harmony.Patch(
                AccessTools.Method(typeof(Thing), nameof(Thing.DeSpawn)),
                null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(DeSpawn_Postfix)));

            harmony.Patch(
                AccessTools.Method(typeof(PlaySettings), nameof(PlaySettings.DoPlaySettingsGlobalControls)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(PlaySettingsDirty_Prefix)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(PlaySettingsDirty_Postfix)));

            // harmony.PatchAll(Assembly.GetExecutingAssembly());
            Log.Message(
                "Colonistbar KF successfully completed " + harmony.GetPatchedMethods().Count()
                + " patches with harmony.");
        }

        #endregion Constructors

        #region Methods

        public static bool CaravanMemberCaravanAt_Prefix(ref Caravan __result, Vector2 at)
        {
            if (!Visible)
            {
                __result = null;
                return false;
            }

            Thing thing = null;
            ColonistOrCorpseAt_Prefix(ref thing, at);
            Pawn pawn = thing as Pawn;

            if (pawn != null && pawn.IsCaravanMember())
            {
                __result = pawn.GetCaravan();
                return false;
            }

            __result = null;
            return false;
        }

        public static bool CaravanMembersCaravansInScreenRect_Prefix(ref List<Caravan> __result, Rect rect)
        {
            BarHelperKf.tmpCaravans.Clear();
            if (!Visible)
            {
                __result = BarHelperKf.tmpCaravans;
                return false;
            }

            List<Pawn> list = CaravanMembersInScreenRect(rect);
            for (int i = 0; i < list.Count; i++)
            {
                BarHelperKf.tmpCaravans.Add(list[i].GetCaravan());
            }

            __result = BarHelperKf.tmpCaravans;
            return false;
        }

        public static bool ColonistOrCorpseAt_Prefix(ref Thing __result, Vector2 pos)
        {
            if (!Visible)
            {
                __result = null;
                return false;
            }

            if (!BarHelperKf.TryGetEntryAt(pos, out ColonistBar.Entry entry))
            {
                __result = null;
                return false;
            }

            Pawn pawn = entry.pawn;
            Thing result;
            if (pawn != null && pawn.Dead && pawn.Corpse != null && pawn.Corpse.SpawnedOrAnyParentSpawned)
            {
                result = pawn.Corpse;
            }
            else
            {
                result = pawn;
            }

            __result = result;
            return false;
        }

        public static void Dirty_Postfix()
        {
            BarHelperKf.EntriesDirty = true;
        }

        public static bool GetColonistsInOrder_Prefix(ref List<Pawn> __result)
        {
            List<ColonistBar.Entry> entries = BarHelperKf.Entries;
            BarHelperKf.tmpColonistsInOrder.Clear();
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].pawn != null)
                {
                    BarHelperKf.tmpColonistsInOrder.Add(entries[i].pawn);
                }
            }

            __result = BarHelperKf.tmpColonistsInOrder;
            return false;
        }

        public static bool MapColonistsOrCorpsesInScreenRect_Prefix(ref List<Thing> __result, Rect rect)
        {
            BarHelperKf.tmpMapColonistsOrCorpsesInScreenRect.Clear();
            if (!Visible)
            {
                __result = BarHelperKf.tmpMapColonistsOrCorpsesInScreenRect;
                return false;
            }

            List<Thing> list = ColonistsOrCorpsesInScreenRect(rect);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Spawned)
                {
                    BarHelperKf.tmpMapColonistsOrCorpsesInScreenRect.Add(list[i]);
                }
            }

            __result = BarHelperKf.tmpMapColonistsOrCorpsesInScreenRect;
            return false;
        }

        public static void MarkColonistsDirty_Postfix()
        {
            RecalcSizes();
            BarHelperKf.EntriesDirty = true;

            // Log.Message("Colonists marked dirty.01");
        }
        private static void DeSpawn_Postfix(Thing __instance)
        {
            Pawn pawn = __instance as Pawn;
            if (pawn == null)
            {
                return;
            }

            if (pawn.Faction != Faction.OfPlayer)
            {
                return;
            }

            if (!pawn.RaceProps.Humanlike)
            {
                return;
            }

            Dirty_Postfix();

            // Log.Message("Colonists marked dirty.x13");
        }

        private static void IsColonistBarNull_Postfix()
        {
            if (Find.ColonistBar != null)
            {
                Dirty_Postfix();

                // Log.Message("Colonists marked dirty.x10");
            }
        }

        private static void IsPlayingDirty_Postfix()
        {
            if (Current.ProgramState == ProgramState.Playing)
            {
                Dirty_Postfix();
            }
        }

        private static void Kill_Postfix(Pawn __instance)
        {
            if (__instance.Faction != null && __instance.Faction.IsPlayer
                && Current.ProgramState == ProgramState.Playing)
            {
                Dirty_Postfix();

                // Log.Message("Colonists marked dirty.x11");
            }
        }

        private static void NotifyColonistBar_Postfix(Corpse __instance)
        {
            Pawn InnerPawn = __instance.InnerPawn;

            if (InnerPawn == null)
            {
                return;
            }

            if (InnerPawn.Faction == Faction.OfPlayer && Current.ProgramState == ProgramState.Playing)
            {
                Dirty_Postfix();

                // Log.Message("Colonists marked dirty.x07");
            }
        }

        private static void NotifyColonistBarIfColonistCorpse_Postfix(Thing __instance)
        {
            if (__instance is Corpse corpse && !corpse.Bugged && corpse.InnerPawn.Faction != null
                && corpse.InnerPawn.Faction.IsPlayer && Current.ProgramState == ProgramState.Playing)
            {
                Dirty_Postfix();
            }
        }

        private static void PlaySettingsDirty_Postfix()
        {
            if (flag != Find.PlaySettings.showColonistBar)
            {
                Dirty_Postfix();
            }

            // Log.Message("Colonists marked dirty.x06");
        }

        private static void PlaySettingsDirty_Prefix()
        {
            flag = Find.PlaySettings.showColonistBar;
        }
        private static void SpawnSetup_Postfix(Thing __instance)
        {
            Pawn pawn = __instance as Pawn;
            if (pawn == null)
            {
                return;
            }

            if (pawn.Faction != Faction.OfPlayer)
            {
                return;
            }

            if (!pawn.RaceProps.Humanlike)
            {
                return;
            }

            if (__instance is IThingHolder && Find.ColonistBar != null)
            {
                Dirty_Postfix();
            }
        }

        #endregion Methods
    }
}