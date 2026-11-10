using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace UniversalRimWorldTranslator
{
    [StaticConstructorOnStartup]
    public static class Bootstrap
    {
        static Bootstrap()
        {
            try
            {
                SettingsStore.Load();
                var harmony = new Harmony("erickpaul.universalrimworldtranslator");
                var method = AccessTools.Method(typeof(Translator), "Translate", new[] { typeof(string) });
                if (method != null)
                    harmony.Patch(method, postfix: new HarmonyMethod(typeof(TranslatePatch), nameof(TranslatePatch.Postfix)));
                Log.Message("[Universal Translator] Inicializado. Caché: " + SettingsStore.Cache.Count);
            }
            catch (Exception ex)
            {
                Log.Error("[Universal Translator] Error al iniciar: " + ex);
            }
        }
    }

    [HarmonyPatch]
    public static class TranslatePatch
    {
        public static void Postfix(string key, ref TaggedString __result)
        {
            try
            {
                __result = TranslationService.Apply(key, __result.ToString());
            }
            catch (Exception ex)
            {
                Log.Warning("[Universal Translator] No se pudo procesar un texto: " + ex.Message);
            }
        }
    }
}
