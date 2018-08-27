namespace ColonistBarKF
{
    using System.Diagnostics.CodeAnalysis;

    using JetBrains.Annotations;

    using UnityEngine;

    using Verse;

    public class Controlleralt : Mod
    {
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once StyleCop.SA1307
        [SuppressMessage(
            "StyleCop.CSharp.MaintainabilityRules",
            "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        public static SettingsPSI psiSettings;
        public static SettingsColonistBar barSettings;

        public Controller(ModContentPack content)
            : base(content)
        {
            psiSettings = this.GetSettings<SettingsPSI>();
            barSettings = this.GetSettings<SettingsColonistBar>();
        }

       // public override void DoSettingsWindowContents(Rect inRect)
       // {
       //     psiSettings.DoWindowContents(inRect);
       // }

        [NotNull]
        public override string SettingsCategory()
        {
            return "ColonistBar";
        }

        // ReSharper disable once MissingXmlDoc
        public override void WriteSettings()
        {
            psiSettings?.Write();
            barSettings?.Write();
        }
    }
}