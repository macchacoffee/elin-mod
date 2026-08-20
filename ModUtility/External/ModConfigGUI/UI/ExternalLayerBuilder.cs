using System;

using BepInEx.Configuration;

namespace Macchacoffee.ElinMods.ModUtility.External.ModConfigGUI.UI;

[ModExternalType("ModConfigGUI", "ModConfigGUI.UI.LayerBuilder")]
internal static class ExternalLayerBuilder
{
    private static readonly ModExternalMethodSet _ext = ModExternalMethod.For(typeof(ExternalLayerBuilder));
    public static bool IsAvailable => _ext.IsAvailable;

    private static readonly Lazy<Action<string, string, ConfigFile>?> _registerDefaultBuilder =
        _ext.Create<Action<string, string, ConfigFile>>(nameof(RegisterDefaultBuilder));

    public static void RegisterDefaultBuilder(string guid, string name, ConfigFile configFile) =>
        _registerDefaultBuilder.Value?.Invoke(guid, name, configFile);
}
