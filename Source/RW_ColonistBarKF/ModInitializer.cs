using System.IO;
using RimWorld;
using UnityEngine;
using Verse;
using Object = UnityEngine.Object;

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

    internal class CBKF : MonoBehaviour
    {
        public static BarSettings BarSettings = new BarSettings();
        public static PSISettings PsiSettings= new PSISettings();

        public static BarSettings LoadBarSettings(string path = "ColonistBarKF.xml")
        {
            var configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
            BarSettings result = XmlLoader.ItemFromXmlFile<BarSettings>(configFolder + "/" + path, true);
            return result;
        }
        public static void SaveSettings(string path = "ColonistBarKF.xml")
        {
            var configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
            XmlSaver.SaveDataObject(BarSettings, configFolder + "/" + path);
        }
        public static PSISettings LoadPsiSettings(string path = "ColonistBarPSIKF.xml")
        {
            var configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
            PSISettings result = XmlLoader.ItemFromXmlFile<PSISettings>(configFolder + "/" + path, true);
            return result;
        }
        public static void SavePsiSettings(string path = "ColonistBarPSIKF.xml")
        {
            var configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
            XmlSaver.SaveDataObject(PsiSettings, configFolder + "/" + path);
        }
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
            BarSettings = LoadBarSettings();
            PsiSettings = LoadPsiSettings();
            BarSettings.Firstload = true;
            _lastStatUpdate = -5000;
        }
    }
}
