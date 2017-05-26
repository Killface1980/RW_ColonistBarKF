using System.Collections.Generic;
using System.IO;
using ColonistBarKF.PSI;
using Verse;

namespace ColonistBarKF
{
    public static class Settings
    {
        public static Dictionary<Pawn, PawnStats> StatsDict = new Dictionary<Pawn, PawnStats>();

        public static SettingsColonistBar ColBarSettings = new SettingsColonistBar();
        public static SettingsPSI PsiSettings = new SettingsPSI();

        internal static SettingsColonistBar LoadBarSettings(string path = "ColonistBar_KF.xml")
        {
            string configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
            SettingsColonistBar result = DirectXmlLoader.ItemFromXmlFile<SettingsColonistBar>(configFolder + "/" + path);
            return result;
        }
        public static void SaveBarSettings(string path = "ColonistBar_KF.xml")
        {
            string configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
            DirectXmlSaver.SaveDataObject(ColBarSettings, configFolder + "/" + path);
        }

        internal static SettingsPSI LoadPsiSettings(string path = "ColonistBar_PSIKF.xml")
        {
            string configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
            SettingsPSI result = DirectXmlLoader.ItemFromXmlFile<SettingsPSI>(configFolder + "/" + path);
            return result;
        }

        public static void SavePsiSettings(string path = "ColonistBar_PSIKF.xml")
        {
            string configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
            DirectXmlSaver.SaveDataObject(PsiSettings, configFolder + "/" + path);
        }
    }
}
