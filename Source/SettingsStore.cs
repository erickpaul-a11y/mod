using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Verse;

namespace UniversalRimWorldTranslator
{
    [Serializable]
    public class TranslationEntry
    {
        [XmlAttribute] public string Original;
        [XmlAttribute] public string Translated;
    }

    [Serializable]
    public class TranslatorData
    {
        public bool OnlineEnabled = true;
        public string SourceLanguage = "en";
        public string SourceLanguageFolder = "English";
        public string TargetLanguage = "es";
        public string Endpoint = "https://api.mymemory.translated.net/get";
        [XmlArray("Translations"), XmlArrayItem("Entry")]
        public List<TranslationEntry> CacheEntries = new List<TranslationEntry>();

        [XmlIgnore]
        public Dictionary<string, string> Cache = new Dictionary<string, string>(StringComparer.Ordinal);

        public void RebuildCache()
        {
            Cache = CacheEntries.Where(x => x != null && x.Original != null && x.Translated != null)
                .GroupBy(x => x.Original, StringComparer.Ordinal)
                .ToDictionary(g => g.Key, g => g.Last().Translated, StringComparer.Ordinal);
        }

        public void SyncEntries()
        {
            CacheEntries = Cache.Select(x => new TranslationEntry { Original = x.Key, Translated = x.Value }).ToList();
        }
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
                    Data.RebuildCache();
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
                Data.SyncEntries();
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
