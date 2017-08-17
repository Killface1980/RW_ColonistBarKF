namespace ColonistBarKF
{
    using System.IO;

    using UnityEngine;

    using Verse;

    public static class Settings
    {
        public static SettingsColonistBar ColBarSettings = new SettingsColonistBar();

        public static SettingsPSI PsiSettings = new SettingsPSI();

        public static float ViewOpacityCrit => Mathf.Max(PsiSettings.IconOpacityCritical, PsiSettings.IconOpacity);

        internal static SettingsColonistBar LoadBarSettings(string path = "ColonistBar_KF.xml")
        {
            string configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
            SettingsColonistBar result =
                DirectXmlLoader.ItemFromXmlFile<SettingsColonistBar>(configFolder + "/" + path);
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