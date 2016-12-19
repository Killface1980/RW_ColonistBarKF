using System.IO;
using HugsLib.Source.Detour;
using RimWorld;
using UnityEngine;
using Verse;

namespace ColonistBarKF
{
    public class ModInitializer : ITab
    {
        protected GameObject modInitializerControllerObject;

        public ModInitializer()
        {
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                modInitializerControllerObject = new GameObject("Colonist Bar KF");
                modInitializerControllerObject.AddComponent<CBKF>();
                Object.DontDestroyOnLoad(modInitializerControllerObject);
                Log.Message("Colonist Bar KF Initialized");
            });
        }

        protected override void FillTab() { }
    }


    public class CBKF : MonoBehaviour
    {
        public static SettingsColonistBar ColBarSettings = new SettingsColonistBar();
        public static SettingsPSI PsiSettings = new SettingsPSI();

        private static SettingsColonistBar LoadBarSettings(string path = "ColonistBar_KF.xml")
        {
            string configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
            SettingsColonistBar result = XmlLoader.ItemFromXmlFile<SettingsColonistBar>(configFolder + "/" + path);
            return result;
        }
        public static void SaveBarSettings(string path = "ColonistBar_KF.xml")
        {
            string configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
            XmlSaver.SaveDataObject(ColBarSettings, configFolder + "/" + path);
        }

        private static SettingsPSI LoadPsiSettings(string path = "ColonistBar_PSIKF.xml")
        {
            string configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
            SettingsPSI result = XmlLoader.ItemFromXmlFile<SettingsPSI>(configFolder + "/" + path);
            return result;
        }

        public static void SavePsiSettings(string path = "ColonistBar_PSIKF.xml")
        {
            string configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
            XmlSaver.SaveDataObject(PsiSettings, configFolder + "/" + path);
        }
        private int _lastStatUpdate;

        private GameObject _psiObject;
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
            if (Current.ProgramState != ProgramState.Playing)
                return;

            if (Find.TickManager.TicksGame - _lastStatUpdate > 1900)
            {
                Find.ColonistBar.MarkColonistsDirty();
                _lastStatUpdate = Find.TickManager.TicksGame;
            }

            // PSI 
            if (_reinjectNeeded)
            {
                _reinjectTime -= Time.fixedDeltaTime;
                if (_reinjectTime > 0.0)
                    return;
                _reinjectNeeded = false;
                _reinjectTime = 0.0f;
                _psiObject = GameObject.Find("PSIMain") ?? new GameObject("PSIMain");
                _psiObject.AddComponent<PSI.PSI>();
                Log.Message("PSI Injected!!");
            }
        }

        // ReSharper disable once UnusedMember.Global
        public void Start()
        {
            ColBarSettings = LoadBarSettings();
            PsiSettings = LoadPsiSettings();
            _lastStatUpdate = -5000;

            //PSI
            OnLevelWasLoaded(0);
            enabled = true;


        }
    }
}