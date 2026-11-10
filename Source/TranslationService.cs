using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Xml;
using Verse;

namespace UniversalRimWorldTranslator
{
    public static class TranslationService
    {
        private static readonly ConcurrentDictionary<string, byte> Pending = new ConcurrentDictionary<string, byte>();
        private static readonly object SaveLock = new object();
        private static readonly SemaphoreSlim NetworkGate = new SemaphoreSlim(1, 1);
        private const int MaxPending = 100;
        private const int MaxScanTexts = 150;

        public static TaggedString Apply(string key, string original)
        {
            if (string.IsNullOrWhiteSpace(original) || original.Length > 4000)
                return original;

            if (SettingsStore.Cache.TryGetValue(original, out var cached))
                return cached;

            string local;
            if (BuiltInDictionary.TryTranslate(original, out local))
            {
                lock (SaveLock)
                {
                    SettingsStore.Cache[original] = local;
                    SettingsStore.Save();
                }
                return local;
            }

            if (SettingsStore.Data.OnlineEnabled && LooksTranslatable(original))
                QueueTranslation(original);
            return original;
        }

        // Escanea solamente archivos de idioma de los mods cargados. No altera ningún archivo.
        public static int ScanInstalledMods()
        {
            int found = 0;
            foreach (ModContentPack mod in LoadedModManager.RunningModsListForReading)
            {
                try
                {
                    string languageRoot = Path.Combine(mod.RootDir, "Languages");
                    if (!Directory.Exists(languageRoot)) continue;
                    string selectedLanguageRoot = Path.Combine(languageRoot, SettingsStore.Data.SourceLanguageFolder ?? "English");
                    if (!Directory.Exists(selectedLanguageRoot)) continue;

                    foreach (string file in Directory.GetFiles(selectedLanguageRoot, "*.xml", SearchOption.AllDirectories))
                    {
                        string relativePath = file.Substring(selectedLanguageRoot.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                        if (!relativePath.StartsWith("Keyed" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) &&
                            !relativePath.StartsWith("Keyed" + Path.AltDirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                            continue;
                        if (found >= MaxScanTexts) return found;
                        try
                        {
                            var document = new XmlDocument();
                            document.XmlResolver = null;
                            document.Load(file);
                            XmlNodeList nodes = document.SelectNodes("//text()");
                            if (nodes == null) continue;
                            foreach (XmlNode node in nodes)
                            {
                                string value = node.Value == null ? null : node.Value.Trim();
                                if (!LooksTranslatable(value) || value.Length < 3 || value.Length > 1000)
                                    continue;
                                if (SettingsStore.Cache.ContainsKey(value)) continue;
                                string local;
                                if (BuiltInDictionary.TryTranslate(value, out local))
                                {
                                    lock (SaveLock) SettingsStore.Cache[value] = local;
                                }
                                else if (SettingsStore.Data.OnlineEnabled)
                                    QueueTranslation(value);
                                found++;
                                if (found >= MaxScanTexts) break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Warning("[Universal Translator] No se pudo leer " + file + ": " + ex.Message);
                        }
                        if (found >= MaxScanTexts) break;
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning("[Universal Translator] No se pudo explorar el mod " + mod.PackageId + ": " + ex.Message);
                }
                if (found >= MaxScanTexts) break;
            }
            lock (SaveLock) SettingsStore.Save();
            return found;
        }

        private static bool LooksTranslatable(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            if (text.IndexOf('{') >= 0 || text.IndexOf('}') >= 0 || text.IndexOf("http", StringComparison.OrdinalIgnoreCase) >= 0)
                return false;
            foreach (char c in text)
                if (char.IsLetter(c)) return true;
            return false;
        }

        private static void QueueTranslation(string text)
        {
            if (Pending.Count >= MaxPending || !Pending.TryAdd(text, 0)) return;
            Task.Run(async () =>
            {
                try
                {
                    await NetworkGate.WaitAsync().ConfigureAwait(false);
                    try
                    {
                        string translated = await RequestTranslation(text).ConfigureAwait(false);
                        if (!string.IsNullOrWhiteSpace(translated) && !string.Equals(translated, text, StringComparison.Ordinal))
                        {
                            lock (SaveLock)
                            {
                                SettingsStore.Cache[text] = translated;
                                SettingsStore.Save();
                            }
                        }
                        await Task.Delay(1200).ConfigureAwait(false);
                    }
                    finally { NetworkGate.Release(); }
                }
                catch (Exception ex)
                {
                    Log.Warning("[Universal Translator] Traducción online fallida: " + ex.Message);
                }
                finally { byte ignored; Pending.TryRemove(text, out ignored); }
            });
        }

        private static async Task<string> RequestTranslation(string text)
        {
            string langPair = Uri.EscapeDataString(SettingsStore.Data.SourceLanguage + "|" + SettingsStore.Data.TargetLanguage);
            string endpoint = string.IsNullOrWhiteSpace(SettingsStore.Data.Endpoint) ? "https://api.mymemory.translated.net/get" : SettingsStore.Data.Endpoint.Trim();
            string url = endpoint + "?q=" + Uri.EscapeDataString(text) + "&langpair=" + langPair;
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                client.Headers[HttpRequestHeader.UserAgent] = "UniversalRimWorldTranslator/0.2";
                string json = await client.DownloadStringTaskAsync(url).ConfigureAwait(false);
                return ParseTranslationResponse(json);
            }
        }

        internal static string ParseTranslationResponse(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            const string marker = "\"translatedText\"";
            int key = json.IndexOf(marker, StringComparison.Ordinal);
            if (key < 0) return null;
            int colon = json.IndexOf(':', key + marker.Length);
            if (colon < 0) return null;
            int start = colon + 1;
            while (start < json.Length && char.IsWhiteSpace(json[start])) start++;
            if (start >= json.Length || json[start] != '"') return null;

            var result = new StringBuilder();
            for (int i = start + 1; i < json.Length; i++)
            {
                char value = json[i];
                if (value == '"') return WebUtility.HtmlDecode(result.ToString());
                if (value != '\\')
                {
                    result.Append(value);
                    continue;
                }

                if (++i >= json.Length) return null;
                switch (json[i])
                {
                    case '"': result.Append('"'); break;
                    case '\\': result.Append('\\'); break;
                    case '/': result.Append('/'); break;
                    case 'b': result.Append('\b'); break;
                    case 'f': result.Append('\f'); break;
                    case 'n': result.Append('\n'); break;
                    case 'r': result.Append('\r'); break;
                    case 't': result.Append('\t'); break;
                    case 'u':
                        if (i + 4 >= json.Length) return null;
                        int codeUnit;
                        if (!int.TryParse(json.Substring(i + 1, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out codeUnit))
                            return null;
                        result.Append((char)codeUnit);
                        i += 4;
                        break;
                    default: return null;
                }
            }
            return null;
        }
    }
}


