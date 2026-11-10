using UnityEngine;
using Verse;
using RimWorld;

namespace UniversalRimWorldTranslator
{
    public class UniversalTranslatorMod : Mod
    {
        public UniversalTranslatorMod(ModContentPack content) : base(content)
        {
            SettingsStore.Load();
            LongEventHandler.ExecuteWhenFinished(() =>
            {
                int detected = TranslationService.ScanInstalledMods();
                Log.Message("[Universal Translator] Escaneo inicial: " + detected + " textos detectados.");
            });
        }

        public override string SettingsCategory() => "Universal RimWorld Translator";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.Label("Universal RimWorld Translator - prototipo");
            listing.Gap();
            listing.CheckboxLabeled("Activar traducción online", ref SettingsStore.Data.OnlineEnabled);
            listing.Label("Idioma de origen (código, por ejemplo en):");
            SettingsStore.Data.SourceLanguage = listing.TextEntry(SettingsStore.Data.SourceLanguage);
            listing.Label("Carpeta del idioma de origen (normalmente English):");
            SettingsStore.Data.SourceLanguageFolder = listing.TextEntry(SettingsStore.Data.SourceLanguageFolder);
            listing.Label("Idioma de destino (código, es para español):");
            SettingsStore.Data.TargetLanguage = listing.TextEntry(SettingsStore.Data.TargetLanguage);
            listing.Label("Proveedor HTTP:");
            SettingsStore.Data.Endpoint = listing.TextEntry(SettingsStore.Data.Endpoint);
            listing.Gap();
            listing.Label("Traducciones guardadas: " + SettingsStore.Cache.Count);
            if (listing.ButtonText("Guardar configuración"))
            {
                SettingsStore.Save();
                Messages.Message("Configuración del traductor guardada.", MessageTypeDefOf.PositiveEvent, false);
            }
            if (listing.ButtonText("Traducir textos detectados en archivos de mods"))
            {
                int count = TranslationService.ScanInstalledMods();
                Messages.Message("Se encontraron " + count + " textos. Las traducciones online se procesarán en segundo plano.", MessageTypeDefOf.PositiveEvent, false);
            }
            if (listing.ButtonText("Borrar caché de traducciones"))
            {
                SettingsStore.Cache.Clear();
                SettingsStore.Save();
                Messages.Message("Caché borrada.", MessageTypeDefOf.PositiveEvent, false);
            }
            listing.Gap();
            listing.Label("Nota: los textos enviados al proveedor online salen de tu PC. Las traducciones pueden tardar y requieren revisión. La exploración no modifica los archivos originales.");
            listing.End();
        }
    }
}

