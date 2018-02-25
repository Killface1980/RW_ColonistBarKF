using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColonistBarKF.Bar;
using Harmony;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace ColonistBarKF
{
    [StaticConstructorOnStartup]
    internal static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            bool   injected = false;
            string patchLog = "Start injecting PSI to pawns ...";
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs.Where(
                                                                         x => x.race != null && x.race.Humanlike &&
                                                                              x.race.IsFlesh))
            {
                patchLog += "\nPSI check: " + def;
                if (def?.comps != null)
                {
                    def.comps.Add(new CompProperties(typeof(CompPSI)));
                    patchLog += " - PSI injected.";
                    injected =  true;
                }
            }

            patchLog += injected ? string.Empty : "\nNo pawns found for PSI :(";
            Log.Message(patchLog);

            HarmonyInstance harmony = HarmonyInstance.Create("com.colonistbarkf.rimworld.mod");

            harmony.Patch(
                          AccessTools.Method(typeof(ColonistBar), nameof(ColonistBar.ColonistBarOnGUI)),
                          new HarmonyMethod(typeof(ColonistBar_KF), nameof(ColonistBar_KF.ColonistBarOnGUI_Prefix)),
                          null);

            harmony.Patch(
                          AccessTools.Method(typeof(ColonistBar),
                                             nameof(ColonistBar.MapColonistsOrCorpsesInScreenRect)),
                          new HarmonyMethod(typeof(HarmonyPatches), nameof(MapColonistsOrCorpsesInScreenRect_Prefix)),
                          null);

            harmony.Patch(
                          AccessTools.Method(typeof(ColonistBar),
                                             nameof(ColonistBar.CaravanMembersCaravansInScreenRect)),
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
                          new HarmonyMethod(typeof(HarmonyPatches), nameof(EntriesDirty_Postfix)));

            harmony.Patch(
                          AccessTools.Method(typeof(Caravan), nameof(Caravan.Notify_PawnRemoved)),
                          null,
                          new HarmonyMethod(typeof(HarmonyPatches), nameof(EntriesDirty_Postfix)));

            harmony.Patch(
                          AccessTools.Method(typeof(Caravan), nameof(Caravan.PostAdd)),
                          null,
                          new HarmonyMethod(typeof(HarmonyPatches), nameof(EntriesDirty_Postfix)));

            harmony.Patch(
                          AccessTools.Method(typeof(Caravan), nameof(Caravan.PostRemove)),
                          null,
                          new HarmonyMethod(typeof(HarmonyPatches), nameof(EntriesDirty_Postfix)));

            harmony.Patch(
                          AccessTools.Method(typeof(Game), nameof(Game.AddMap)),
                          null,
                          new HarmonyMethod(typeof(HarmonyPatches), nameof(EntriesDirty_Postfix)));

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
                          new HarmonyMethod(typeof(HarmonyPatches), nameof(EntriesDirty_Postfix)));

            harmony.Patch(
                          AccessTools.Method(typeof(Pawn), nameof(Pawn.SpawnSetup)),
                          null,
                          new HarmonyMethod(typeof(HarmonyPatches), nameof(Pawn_SpawnSetup_Postfix)));

            harmony.Patch(
                          AccessTools.Method(typeof(Pawn), nameof(Pawn.Kill)),
                          null,
                          new HarmonyMethod(typeof(HarmonyPatches), nameof(Pawn_Kill_Postfix)));

            harmony.Patch(
                          AccessTools.Method(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.Notify_Resurrected)),
                          null,
                          new HarmonyMethod(typeof(HarmonyPatches), nameof(Pawn_Resurrected_Postfix)));

            // NOT WORKING, FollowMe immediatly cancels if this is active
            // harmony.Patch(
            // AccessTools.Method(typeof(CameraDriver), nameof(CameraDriver.JumpToVisibleMapLoc), new[] { typeof(Vector3) }),
            // new HarmonyMethod(typeof(HarmonyPatches), nameof(StopFollow_Prefix)),
            // null);
            harmony.Patch(
                          AccessTools.Method(
                                             typeof(WorldCameraDriver),
                                             nameof(WorldCameraDriver.JumpTo),
                                             new[] {typeof(Vector3)}),
                          new HarmonyMethod(typeof(HarmonyPatches), nameof(StopFollow_Prefix)),
                          null);

            harmony.Patch(
                          AccessTools.Method(
                                             typeof(ThingSelectionUtility),
                                             nameof(ThingSelectionUtility.SelectNextColonist)),
                          new HarmonyMethod(typeof(HarmonyPatches), nameof(StartFollowSelectedColonist1)),
                          new HarmonyMethod(typeof(HarmonyPatches), nameof(StartFollowSelectedColonist2)),
                          null);

            harmony.Patch(
                          AccessTools.Method(
                                             typeof(ThingSelectionUtility),
                                             nameof(ThingSelectionUtility.SelectPreviousColonist)),
                          new HarmonyMethod(typeof(HarmonyPatches), nameof(StartFollowSelectedColonist1)),
                          new HarmonyMethod(typeof(HarmonyPatches), nameof(StartFollowSelectedColonist2)),
                          null);

            harmony.Patch(
                          AccessTools.Method(
                                             typeof(CameraDriver),
                                             nameof(CameraDriver.JumpToVisibleMapLoc),
                                             new[] {typeof(Vector3)}),
                          new HarmonyMethod(typeof(HarmonyPatches), nameof(StopFollow_Prefix_Vector3)),
                          null);

            harmony.Patch(
                          AccessTools.Method(typeof(Pawn), nameof(Pawn.PostApplyDamage)),
                          null,
                          new HarmonyMethod(typeof(HarmonyPatches), nameof(Pawn_PostApplyDamage_Postfix)));

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

            Log.Message(
                        "Colonistbar KF successfully completed " + harmony.GetPatchedMethods().Count()
                                                                 + " patches with harmony.");
        }

        public static void MarkColonistsDirty_Postfix()
        {
            ColonistBar_KF.RecalcSizes();
            ColonistBar_KF.BarHelperKf.EntriesDirty = true;

            // Log.Message("Colonists marked dirty.01");
        }

        private static bool CaravanMemberCaravanAt_Prefix([CanBeNull] ref Caravan __result, Vector2 at)
        {
            if (!ColonistBar_KF.Visible)
            {
                __result = null;
                return false;
            }

            Thing thing = null;
            ColonistOrCorpseAt_Prefix(ref thing, at);

            if (thing is Pawn pawn && pawn.IsCaravanMember())
            {
                __result = pawn.GetCaravan();
                return false;
            }

            __result = null;
            return false;
        }

        private static bool CaravanMembersCaravansInScreenRect_Prefix([NotNull] ref List<Caravan> __result, Rect rect)
        {
            ColonistBar_KF.BarHelperKf.tmpCaravans.Clear();
            if (!ColonistBar_KF.Visible)
            {
                __result = ColonistBar_KF.BarHelperKf.tmpCaravans;
                return false;
            }

            List<Pawn> list = ColonistBar_KF.CaravanMembersInScreenRect(rect);
            for (int i = 0; i < list.Count; i++)
            {
                ColonistBar_KF.BarHelperKf.tmpCaravans.Add(list[i].GetCaravan());
            }

            __result = ColonistBar_KF.BarHelperKf.tmpCaravans;
            return false;
        }

        private static bool ColonistOrCorpseAt_Prefix([CanBeNull] ref Thing __result, Vector2 pos)
        {
            if (!ColonistBar_KF.Visible)
            {
                __result = null;
                return false;
            }

            if (!ColonistBar_KF.BarHelperKf.TryGetEntryAt(pos, out EntryKF entry))
            {
                __result = null;
                return false;
            }

            Pawn  pawn = entry.Pawn;
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

        private static void DeSpawn_Postfix([NotNull] Thing __instance)
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

            EntriesDirty_Postfix();
        }

        private static void EntriesDirty_Postfix()
        {
            ColonistBar_KF.BarHelperKf.EntriesDirty = true;
        }

        private static bool GetColonistsInOrder_Prefix([NotNull] ref List<Pawn> __result)
        {
            List<EntryKF> entries = ColonistBar_KF.BarHelperKf.Entries;
            ColonistBar_KF.BarHelperKf.tmpColonistsInOrder.Clear();
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].Pawn != null)
                {
                    ColonistBar_KF.BarHelperKf.tmpColonistsInOrder.Add(entries[i].Pawn);
                }
            }

            __result = ColonistBar_KF.BarHelperKf.tmpColonistsInOrder;
            return false;
        }

        private static void IsColonistBarNull_Postfix()
        {
            if (Find.ColonistBar != null)
            {
                EntriesDirty_Postfix();
            }
        }

        private static void IsPlayingDirty_Postfix()
        {
            if (Current.ProgramState == ProgramState.Playing)
            {
                EntriesDirty_Postfix();
            }
        }

        private static bool MapColonistsOrCorpsesInScreenRect_Prefix(ref List<Thing> __result, Rect rect)
        {
            ColonistBar_KF.BarHelperKf.tmpMapColonistsOrCorpsesInScreenRect.Clear();
            if (!ColonistBar_KF.Visible)
            {
                __result = ColonistBar_KF.BarHelperKf.tmpMapColonistsOrCorpsesInScreenRect;
                return false;
            }

            List<Thing> list = ColonistBar_KF.ColonistsOrCorpsesInScreenRect(rect);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Spawned)
                {
                    ColonistBar_KF.BarHelperKf.tmpMapColonistsOrCorpsesInScreenRect.Add(list[i]);
                }
            }

            __result = ColonistBar_KF.BarHelperKf.tmpMapColonistsOrCorpsesInScreenRect;
            return false;
        }

        private static void NotifyColonistBar_Postfix([NotNull] Corpse __instance)
        {
            Pawn innerPawn = __instance.InnerPawn;

            if (innerPawn == null)
            {
                return;
            }

            if (innerPawn.Faction == Faction.OfPlayer && Current.ProgramState == ProgramState.Playing)
            {
                EntriesDirty_Postfix();

                // Log.Message("Colonists marked dirty.x07");
            }
        }

        private static void NotifyColonistBarIfColonistCorpse_Postfix([NotNull] Thing __instance)
        {
            if (Current.ProgramState != ProgramState.Playing)
            {
                return;
            }


            if (__instance is Corpse corpse)
            {
                if (!corpse.Bugged)
                {
                    if (corpse.InnerPawn != null && corpse.InnerPawn.Faction?.IsPlayer == true)
                    {
                        EntriesDirty_Postfix();
                    }
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        private static void Pawn_Kill_Postfix([NotNull] Pawn __instance)
        {
            if (__instance.Faction?.IsPlayer == true && Current.ProgramState == ProgramState.Playing)
            {
                EntriesDirty_Postfix();
                CompPSI compPSI = __instance.GetComp<CompPSI>();
                if (compPSI != null)
                {
                    compPSI.BgColor      = Color.gray;
                    compPSI.ThisColCount = 0;
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        private static void Pawn_Resurrected_Postfix([NotNull] Pawn_HealthTracker __instance)
        {
            FieldInfo pawnFieldInfo =
            typeof(Pawn_HealthTracker).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);
            Pawn pawn = (Pawn) pawnFieldInfo?.GetValue(__instance);

            if (pawn?.Faction != null && pawn.Faction.IsPlayer && Current.ProgramState == ProgramState.Playing)
            {
                EntriesDirty_Postfix();
                CompPSI compPSI = pawn.GetComp<CompPSI>();
                compPSI?.CheckTraits();
            }
        }

        private static void Pawn_PostApplyDamage_Postfix([NotNull] Pawn __instance)
        {
            CompPSI compPSI = __instance.GetComp<CompPSI>();

            compPSI?.SetEntriesDirty();
        }

        private static void Pawn_SpawnSetup_Postfix(Pawn __instance)
        {
            if (__instance == null)
            {
                return;
            }

            if (!__instance.RaceProps.Humanlike || !__instance.RaceProps.IsFlesh)
            {
                return;
            }

            if (__instance is IThingHolder && Find.ColonistBar != null)
            {
                EntriesDirty_Postfix();
            }
        }

        private static void PlaySettingsDirty_Postfix(bool __state)
        {
            if (__state != Find.PlaySettings.showColonistBar)
            {
                EntriesDirty_Postfix();
            }
        }

        // ReSharper disable once RedundantAssignment
        private static void PlaySettingsDirty_Prefix(ref bool __state)
        {
            __state = Find.PlaySettings.showColonistBar;
        }

        private static void StopFollow_Prefix()
        {
            if (FollowMe.CurrentlyFollowing)
            {
                FollowMe.StopFollow("Harmony");
            }
        }

        private static void StartFollowSelectedColonist1(ref bool __state)
        {
            __state = FollowMe.CurrentlyFollowing;
        }

        private static void StartFollowSelectedColonist2(bool __state)
        {
            if (__state)
            {
                FollowMe.TryStartFollow(Find.Selector.SingleSelectedThing);
            }
        }

        private static void StopFollow_Prefix_Vector3([NotNull] Vector3 loc)
        {
            if (FollowMe.CurrentlyFollowing)
            {
                if (FollowMe.FollowedThing.TrueCenter() != loc)
                {
                    FollowMe.StopFollow("Harmony");
                }
            }
        }
    }
}