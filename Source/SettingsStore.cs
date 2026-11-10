using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Verse;

namespace UniversalRimWorldTranslator
{
    [Serializable]
    public class TranslatorData
    {
        public bool OnlineEnabled = true;
        public string SourceLanguage = "en";
        public string TargetLanguage = "es";
        public string Endpoint = "https://api.mymemory.translated.net/get";
        public Dictionary<string, string> Cache = new Dictionary<string, string>(StringComparer.Ordinal);
    }

    public static class SettingsStore
    {
        private static string FilePath => Path.Combine(GenFilePaths.ConfigFolderPath, "UniversalRimWorldTranslator.xml");
        public static TranslatorData Data = new TranslatorData();
        public static Dictionary<string, string> Cache => Data.Cache;

        public static void Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var serializer = new XmlSerializer(typeof(TranslatorData));
                    using (var stream = File.OpenRead(FilePath))
                        Data = (TranslatorData)serializer.Deserialize(stream) ?? new TranslatorData();
                    if (Data.Cache == null) Data.Cache = new Dictionary<string, string>(StringComparer.Ordinal);
                }
            }
            catch (Exception ex)
            {
                Log.Warning("[Universal Translator] No se pudo leer la configuración: " + ex.Message);
                Data = new TranslatorData();
            }
        }

        public static void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                var serializer = new XmlSerializer(typeof(TranslatorData));
                using (var stream = File.Create(FilePath))
                    serializer.Serialize(stream, Data);
            }
            catch (Exception ex)
            {
                Log.Warning("[Universal Translator] No se pudo guardar la caché: " + ex.Message);
            }
        }
    }
}
