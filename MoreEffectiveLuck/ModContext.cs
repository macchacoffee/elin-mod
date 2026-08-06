using System.IO;
using BepInEx;
using BepInEx.Configuration;
using MoreEffectiveLuck.Config;

namespace MoreEffectiveLuck;

public static class ModContext
{
    private static readonly string _configFileName = $"{PluginInfo.Guid}.cfg";

    public static ModConfig Config { get; private set; } = new();

    public static ConfigFile BindConfig()
    {
        var configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, _configFileName), true);
        Config.Bind(configFile);
        return configFile;
    }
}
