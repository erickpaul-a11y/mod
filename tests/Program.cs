using System;
using System.IO;
using UniversalRimWorldTranslator;
using Verse;

internal static class Program
{
    private static int Main()
    {
        string translated;
        if (!BuiltInDictionary.TryTranslate("Save game", out translated) || translated != "Guardar partida")
            return Fail("Built-in dictionary lookup failed.");

        SettingsStore.Cache["stub-key"] = "texto traducido";
        if (TranslationService.Apply("stub-key", "stub-key").ToString() != "texto traducido")
            return Fail("Translation cache lookup failed.");

        string root = Path.Combine(Path.GetTempPath(), "urt-stub-" + Guid.NewGuid().ToString("N"));
        try
        {
            string keyed = Path.Combine(root, "Languages", "English", "Keyed");
            string injected = Path.Combine(root, "Languages", "English", "DefInjected");
            Directory.CreateDirectory(keyed);
            Directory.CreateDirectory(injected);
            File.WriteAllText(Path.Combine(keyed, "Words.xml"), "<LanguageData><Example_Key>Keyed sample</Example_Key></LanguageData>");
            File.WriteAllText(Path.Combine(injected, "ThingDef.xml"), "<LanguageData><Example><label>Injected sample</label></Example></LanguageData>");
            LoadedModManager.RunningModsListForReading.Clear();
            LoadedModManager.RunningModsListForReading.Add(new ModContentPack { RootDir = root, PackageId = "test.stub" });
            SettingsStore.Data = new TranslatorData { OnlineEnabled = false, SourceLanguageFolder = "English" };
            int count = TranslationService.ScanInstalledMods();
            if (count != 1) return Fail("Scanner should include Keyed XML and exclude DefInjected XML; got " + count + ".");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }

        Console.WriteLine("Stub checks passed: dictionary, cache, Keyed-only scan.");
        return 0;
    }
    private static int Fail(string message) { Console.Error.WriteLine(message); return 1; }
}
