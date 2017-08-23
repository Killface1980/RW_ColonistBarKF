namespace ColonistBarKF
{
    using System.IO;


    using UnityEngine;

    using Verse;

    public static class Settings
    {
        #region Public Fields

        [NotNull]
        public static SettingsColonistBar ColBarSettings = new SettingsColonistBar();

        [NotNull]
        public static SettingsPSI PsiSettings = new SettingsPSI();

        #endregion Public Fields

        #region Public Properties

        public static float ViewOpacityCrit => Mathf.Max(PsiSettings.IconOpacityCritical, PsiSettings.IconOpacity);

        #endregion Public Properties

        #region Public Methods

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

        #endregion Public Methods

        #region Internal Methods

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

        #endregion Internal Methods
    }
}