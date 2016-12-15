using System.IO;
using RimWorld;
using UnityEngine;
using Verse;

namespace ColonistBarKF
{
    public class ModInitializer : ITab
    {
        private GameObject modInitializerControllerObject;
        private GameObject _psiObject;

        public ModInitializer()
        {
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                modInitializerControllerObject = new GameObject("Colonist Bar KF");
                modInitializerControllerObject.AddComponent<CBKF>();
                Object.DontDestroyOnLoad(modInitializerControllerObject);
                Log.Message("Colonist Bar KF Initialized");

                _psiObject = GameObject.Find("PSIMain") ?? new GameObject("PSIMain");
                _psiObject.AddComponent<PSI.PSI>();
                Object.DontDestroyOnLoad(_psiObject);
                Log.Message("PSI Injected!!");
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



        // ReSharper disable once UnusedMember.Global
        public void FixedUpdate()
        {
            if (Current.ProgramState != ProgramState.Playing)
            {
                return;
            }

            if (Find.TickManager.TicksGame - _lastStatUpdate > 1900)
            {
                Find.ColonistBar.MarkColonistsDirty();
                _lastStatUpdate = Find.TickManager.TicksGame;
            }
        }

        // ReSharper disable once UnusedMember.Global
        public void Start()
        {

            GameObject initializer = new GameObject("MapComponentInjectorCBKF");
            initializer.AddComponent<MapComponentInjector>();
            Object.DontDestroyOnLoad(initializer);




            ColBarSettings = LoadBarSettings();
            PsiSettings = LoadPsiSettings();
            _lastStatUpdate = -5000;

            //PSI


        }
    }
}
