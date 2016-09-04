using System.IO;
using RimWorld;
using UnityEngine;
using Verse;
using Object = UnityEngine.Object;

namespace RW_ColonistBarKF
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

    internal class CBKF : MonoBehaviour
    {

#if NoCCL

        public static ModSettings Settings = new ModSettings();

        public static ModSettings LoadSettings(string path = "ColonistBarKF.xml")
        {
            var configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
            ModSettings result = XmlLoader.ItemFromXmlFile<ModSettings>(configFolder + "/" + path, true);
            return result;
        }

        public static void SaveSettings(string path = "ColonistBarKF.xml")
        {
            var configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
            XmlSaver.SaveDataObject(Settings, configFolder + "/" + path);
        }

#endif
        private int _lastStatUpdate;

        public void FixedUpdate()
        {
            if (Current.ProgramState != ProgramState.MapPlaying)
                return;

            if (Find.TickManager.TicksGame - _lastStatUpdate > 1900)
            {
                ((UIRootMap)Find.UIRoot).colonistBar.MarkColonistsListDirty();
                _lastStatUpdate = Find.TickManager.TicksGame;
            }

        }

        public void Start()
        {
            Settings = LoadSettings();
            Settings.Firstload = true;
            _lastStatUpdate = -5000;
        }
    }
}
