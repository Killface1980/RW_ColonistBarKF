using System.IO;

using ColonistBarKF.Settings;

using RimWorld;

using UnityEngine;

using Verse;

namespace ColonistBarKF
{
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
    public class CBKF : Mod
    {
        public CBKF(ModContentPack content)
            : base(content)
        {
        }

        private int _lastStatUpdate;

        private GameObject _psiObject;

        private GameObject _followObject;

        private GameObject _zoomObject;

        private bool _reinjectNeeded;

        private float _reinjectTime;

        private void OnLevelWasLoaded(int level)
        {
            _reinjectNeeded = true;
            _reinjectTime = level >= 0 ? 1f : 0.0f;
        }

        // ReSharper disable once UnusedMember.Global
        public void FixedUpdate()
        {
            if (Current.ProgramState != ProgramState.Playing) return;

            // if (Find.TickManager.TicksGame - _lastStatUpdate > 1900)
            // {
            // ColonistBar_KF.MarkColonistsDirty();
            // _lastStatUpdate = Find.TickManager.TicksGame;
            // }

            // PSI 
            if (_reinjectNeeded)
            {
                _reinjectTime -= Time.fixedDeltaTime;
                if (_reinjectTime > 0.0) return;
                _reinjectNeeded = false;
                _reinjectTime = 0.0f;

                // _psiObject = GameObject.Find("PSIMain") ?? new GameObject("PSIMain");
                // _psiObject.AddComponent<PSI.PSI>();
                Log.Message("PSI Injected!!");
            }
        }

        public void Start()
        {
            ColBarSettings = LoadBarSettings();
            PsiSettings = LoadPsiSettings();
            _lastStatUpdate = -5000;
            ColonistBar_KF.MarkColonistsDirty();

            // PSI
            OnLevelWasLoaded(0);
        }
    }
}