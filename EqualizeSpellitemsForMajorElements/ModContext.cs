using System.IO;
using BepInEx;
using BepInEx.Configuration;
using EqualizeSpellitemsForMajorElements.Config;

namespace EqualizeSpellitemsForMajorElements;

internal static class ModContext
{
    private const string _configFileName = $"{PluginInfo.Guid}.cfg";

    public static ModConfig Config { get; private set; } = new();

    public static ConfigFile BindConfig()
    {
        var configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, _configFileName), true);
        Config.Bind(configFile);
        return configFile;
    }
}
