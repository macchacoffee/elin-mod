using System;
using System.Reflection;

using BepInEx;
using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Logging;

namespace Macchacoffee.ElinMods.NoPCC;

internal static class PluginInfo
{
    public const string Guid = "maccha-coffee.no-pcc";
    public const string Name = "No PCC";
    public const string Version = "1.0.1";
}

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
internal class Plugin : BaseUnityPlugin
{
    private void Awake()
    {
        ModLog.Initialize(Logger);
        try
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginInfo.Guid);
        }
        catch (Exception ex)
        {
            ModLog.Error($"Failed to apply harmony patch: {ex}");
        }
    }
}
