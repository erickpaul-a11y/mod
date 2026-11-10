using System;
using System.Collections.Generic;
using System.Reflection;

namespace Verse
{
    [AttributeUsage(AttributeTargets.Class)] public sealed class StaticConstructorOnStartup : Attribute { }
    public struct TaggedString
    {
        private readonly string value;
        public TaggedString(string value) { this.value = value; }
        public override string ToString() => value ?? "";
        public static implicit operator TaggedString(string value) => new TaggedString(value);
        public static implicit operator string(TaggedString value) => value.ToString();
    }
    public class ModContentPack { public string RootDir; public string PackageId; }
    public static class LoadedModManager { public static List<ModContentPack> RunningModsListForReading = new List<ModContentPack>(); }
    public class Translator { public TaggedString Translate(string key) => key; }
    public static class GenFilePaths { public static string ConfigFolderPath = System.IO.Path.GetTempPath(); }
    public static class Log { public static void Message(string value) { } public static void Error(string value) { } public static void Warning(string value) { } }
    public class Mod
    {
        public Mod(ModContentPack content) { }
        public virtual string SettingsCategory() => "";
        public virtual void DoSettingsWindowContents(UnityEngine.Rect rect) { }
    }
    public static class LongEventHandler { public static void ExecuteWhenFinished(Action action) => action(); }
    public class Listing_Standard
    {
        public void Begin(UnityEngine.Rect rect) { }
        public void End() { }
        public void Gap() { }
        public void Label(string value) { }
        public bool CheckboxLabeled(string label, bool value) => value;
        public string TextEntry(string value) => value;
        public bool ButtonText(string label) => false;
    }
    public static class Messages { public static void Message(string text, object type, bool historical) { } }
    public static class MessageTypeDefOf { public static object PositiveEvent = new object(); }
}
namespace UnityEngine { public struct Rect { } }
namespace HarmonyLib
{
    public class Harmony
    {
        public Harmony(string id) { }
        public void Patch(MethodBase original, HarmonyMethod prefix = null, HarmonyMethod postfix = null) { }
    }
    public class HarmonyMethod { public HarmonyMethod(Type type, string methodName) { } }
    public static class AccessTools
    {
        public static MethodInfo Method(Type type, string name, Type[] parameters) => type.GetMethod(name, parameters);
    }
}
