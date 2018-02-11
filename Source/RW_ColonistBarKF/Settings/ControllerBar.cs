namespace ColonistBarKF
{
    using System.IO;

    using JetBrains.Annotations;

    using UnityEngine;

    using Verse;

    public class Settings : Mod
    {
        public Settings(ModContentPack content)
            : base(content)
        {
            barSettings = this.GetSettings<SettingsColonistBar>();
        }

        public override void WriteSettings()
        {
            barSettings?.Write();
        }

        [NotNull]
        public static SettingsColonistBar barSettings = new SettingsColonistBar();



        // public static void SaveBarSettings([NotNull] string path = "ColonistBar_KF.xml")
        // {
        //     string configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
        //     DirectXmlSaver.SaveDataObject(barSettings, configFolder + "/" + path);
        // }
        //
        // public static void SavePsiSettings([NotNull] string path = "ColonistBar_PSIKF.xml")
        // {
        //     string configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
        //     DirectXmlSaver.SaveDataObject(psiSettings, configFolder + "/" + path);
        // }
        //
        // [NotNull]
        // internal static SettingsColonistBar LoadBarSettings([NotNull] string path = "ColonistBar_KF.xml")
        // {
        //     string configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
        //     SettingsColonistBar __result =
        //         DirectXmlLoader.ItemFromXmlFile<SettingsColonistBar>(configFolder + "/" + path);
        //     return __result;
        // }
        //
        // [NotNull]
        // internal static SettingsPSI LoadPsiSettings([NotNull] string path = "ColonistBar_PSIKF.xml")
        // {
        //     string configFolder = Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath);
        //     SettingsPSI __result = DirectXmlLoader.ItemFromXmlFile<SettingsPSI>(configFolder + "/" + path);
        //     return __result;
        // }
    }
}