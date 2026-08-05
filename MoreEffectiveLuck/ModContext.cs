using System.IO;
using BepInEx;
using BepInEx.Configuration;
using MoreEffectiveLuck.Config;

namespace MoreEffectiveLuck;

public static class ModContext
{
    private static readonly string ConfigFileName = $"{PluginInfo.Guid}.cfg";

    public static ModConfig Config { get; private set; } = new();

    public static ConfigFile BindConfig()
    {
        var configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, ConfigFileName), true);
        Config.Bind(configFile);
        return configFile;
    }
}
