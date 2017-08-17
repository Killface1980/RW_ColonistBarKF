using System.IO;

using RimWorld;

using UnityEngine;

using Verse;

namespace ColonistBarKF
{
    using System.Linq;

    using HugsLib;

    // public class ModInitializer : ITab
    // {
    // protected GameObject modInitializerControllerObject;
    // public ModInitializer()
    // {
    // LongEventHandler.ExecuteWhenFinished(delegate
    // {
    // modInitializerControllerObject = new GameObject("Colonist Bar KF");
    // modInitializerControllerObject.AddComponent<CBKF>();
    // Object.DontDestroyOnLoad(modInitializerControllerObject);
    // Log.Message("Colonist Bar KF Initialized");
    // });
    // }
    // protected override void FillTab() { }
    // }
    public class CBKF : ModBase
    {
        private int _lastStatUpdate;

        private GameObject _psiObject;

        private GameObject _followObject;

        private GameObject _zoomObject;

        private bool _reinjectNeeded;

        private float _reinjectTime;

        public override void DefsLoaded()
        {
            base.DefsLoaded();
            if (!ModIsActive)
            {
                return;
            }

            Log.Message("Start injecting PSI to pawns ...");
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs.Where(
                x => x.race != null && x.race.Humanlike && x.race.IsFlesh))
            {
                Log.Message("PSI check: " + def);
                if (def?.comps != null)
                {
                    def.comps.Add(new CompProperties(typeof(CompPSI)));
                    Log.Message("PSI injected " + def);
                }
            }
        }

        public override string ModIdentifier
        {
            get
            {
                return "ColonistBarKF";
            }
        }
    }
}