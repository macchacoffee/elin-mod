using System;
using BepInEx.Configuration;

namespace ModUtility.External.ModConfigGUI.UI;

[ModExternalType("ModConfigGUI", "ModConfigGUI.UI.LayerBuilder")]
public static class ExternalLayerBuilder
{
    private static Lazy<TDelegate?> Create<TDelegate>(string methodName) where TDelegate : Delegate => ModExternalMethod.Create<TDelegate>(typeof(ExternalLayerBuilder), methodName);

    private static readonly Lazy<Action<string, string, ConfigFile>?> _registerDefaultBuilder = Create<Action<string, string, ConfigFile>>(nameof(RegisterDefaultBuilder));

    public static bool IsAvailable => _registerDefaultBuilder.Value is not null;

    public static void RegisterDefaultBuilder(string guid, string name, ConfigFile configFile) =>
        _registerDefaultBuilder.Value?.Invoke(guid, name, configFile);
}
