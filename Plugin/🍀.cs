using BepInEx;
using BepInEx.Logging;
using BepInEx.NET.Common;
using BepInEx.Configuration;
using BepInExResoniteShim;
using HarmonyLib;

namespace FourLeafClover;

[ResonitePlugin(PluginMetadata.GUID, PluginMetadata.NAME, PluginMetadata.VERSION, PluginMetadata.AUTHORS, PluginMetadata.REPOSITORY_URL)]
[BepInDependency(BepInExResoniteShim.PluginMetadata.GUID, BepInDependency.DependencyFlags.HardDependency)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;

    public static ConfigEntry<bool> Enabled;
    public static ConfigEntry<bool> Equilibrium;

    public override void Load()
    {
        Enabled = Config.Bind("General", "Enabled", true, "Makes you luckier, by using the power of 🍀");
        Equilibrium = Config.Bind("General", "Equilibrium", false, "Makes it so all clips are equally likely to be played. \"Perfectly balanced, as all things should be.\"");

        HarmonyInstance.PatchAll();

        // Plugin startup logic
        Log = base.Log;
        Log.LogInfo($"Plugin {PluginMetadata.GUID} is loaded!");
    }

    // Patch function which was added by the Patcher
    [HarmonyPatch(typeof(Elements.Core.CollectionsExtensions), "🍀_ProcessNumber")]
    public class ProcessNumberPatch
    {
        public static bool Prefix(ref float __result, float num)
        {
            if (!Enabled.Value) return true;

            if (Equilibrium.Value) __result = 1;
            else __result = num == 0 ? 0 : 1 / num;

            return false;
        }
    }
}
