namespace ColonistBarKF
{
    using System.IO;

    using JetBrains.Annotations;

    using UnityEngine;

    using Verse;

    public static class Settings
    {
        [NotNull]
        public static SettingsColonistBar ColBarSettings = new SettingsColonistBar();

        [NotNull]
        public static SettingsPSI PsiSettings = new SettingsPSI();

        public static float ViewOpacityCrit => Mathf.Max(PsiSettings.IconOpacityCritical, PsiSettings.IconOpacity);

        public static void SaveBarSettings([NotNull] string path = "ColonistBar_KF.xml")
        {
            string configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
            DirectXmlSaver.SaveDataObject(ColBarSettings, configFolder + "/" + path);
        }

        public static void SavePsiSettings([NotNull] string path = "ColonistBar_PSIKF.xml")
        {
            string configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
            DirectXmlSaver.SaveDataObject(PsiSettings, configFolder + "/" + path);
        }

        [NotNull]
        internal static SettingsColonistBar LoadBarSettings([NotNull] string path = "ColonistBar_KF.xml")
        {
            string configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
            SettingsColonistBar result =
                DirectXmlLoader.ItemFromXmlFile<SettingsColonistBar>(configFolder + "/" + path);
            return result;
        }

        [NotNull]
        internal static SettingsPSI LoadPsiSettings([NotNull] string path = "ColonistBar_PSIKF.xml")
        {
            string configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
            SettingsPSI result = DirectXmlLoader.ItemFromXmlFile<SettingsPSI>(configFolder + "/" + path);
            return result;
        }
    }
}