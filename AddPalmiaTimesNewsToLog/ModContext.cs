using System.IO;

using Macchacoffee.ElinMods.AddPalmiaTimesNewsToLog.Config;
using Macchacoffee.ElinMods.AddPalmiaTimesNewsToLog.News;

namespace Macchacoffee.ElinMods.AddPalmiaTimesNewsToLog;

internal static class ModContext
{
    private const string _configFileName = $"{PluginInfo.Guid}.txt";

    public static ModConfig Config { get; private set; } = new();

    public static ModNewsFeeder NewsFeeder { get; } = new();

    private static string BuildConfigFilePath(string root)
    {
        return Path.Combine(root, _configFileName);
    }

    public static void LoadConfig(string root)
    {
        var filePath = BuildConfigFilePath(root);
        if (File.Exists(filePath))
        {
            var text = IO.IsCompressed(filePath) ? IO.Decompress(filePath) : File.ReadAllText(filePath);
            Config = ModConfig.Deserialize(text);
        }
        else
        {
            Config = new();
        }
    }

    public static void SaveConfig(string root)
    {
        var filePath = BuildConfigFilePath(root);
        var text = Config.Serialize();
        if (GameIO.compressSave)
        {
            IO.Compress(filePath, text);
        }
        else
        {
            File.WriteAllText(filePath, text);
        }
    }
}
