using System;
using System.Collections.Concurrent;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Verse;

namespace UniversalRimWorldTranslator
{
    public static class TranslationService
    {
        private static readonly ConcurrentDictionary<string, byte> Pending = new ConcurrentDictionary<string, byte>();
        private static readonly object SaveLock = new object();

        public static TaggedString Apply(string key, string original)
        {
            if (string.IsNullOrWhiteSpace(original) || original.Length > 4000)
                return original;
            if (SettingsStore.Cache.TryGetValue(original, out var cached))
                return cached;
            if (SettingsStore.Data.OnlineEnabled && LooksTranslatable(original))
                QueueTranslation(original);
            return original;
        }

        private static bool LooksTranslatable(string text)
        {
            // Evita enviar códigos, rutas y cadenas sin letras.
            bool hasLetter = false;
            foreach (char c in text)
                if (char.IsLetter(c)) { hasLetter = true; break; }
            return hasLetter && !text.StartsWith("http", StringComparison.OrdinalIgnoreCase);
        }

        private static void QueueTranslation(string text)
        {
            if (!Pending.TryAdd(text, 0)) return;
            Task.Run(async () =>
            {
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
            string url = SettingsStore.Data.Endpoint + "?q=" + Uri.EscapeDataString(text) + "&langpair=" + langPair;
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                client.Headers[HttpRequestHeader.UserAgent] = "UniversalRimWorldTranslator/0.1";
                string json = await client.DownloadStringTaskAsync(url).ConfigureAwait(false);
                // MyMemory JSON response; parse only the translatedText value.
                const string marker = "\"translatedText\":\"";
                int start = json.IndexOf(marker, StringComparison.Ordinal);
                if (start < 0) return null;
                start += marker.Length;
                var result = new StringBuilder();
                bool escaped = false;
                for (int i = start; i < json.Length; i++)
                {
                    char c = json[i];
                    if (escaped) { result.Append(c == 'n' ? '\n' : c); escaped = false; continue; }
                    if (c == '\\') { escaped = true; continue; }
                    if (c == '"') break;
                    result.Append(c);
                }
                return HttpUtility.HtmlDecode(result.ToString());
            }
        }
    }
}
