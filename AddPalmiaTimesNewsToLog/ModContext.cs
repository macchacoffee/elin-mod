using System.IO;

using Macchacoffee.ElinMods.AddPalmiaTimesNewsToLog.Config;
using Macchacoffee.ElinMods.AddPalmiaTimesNewsToLog.News;

namespace Macchacoffee.ElinMods.AddPalmiaTimesNewsToLog;

internal static class ModContext
{
    private const string _configFileName = $"{PluginInfo.Guid}.txt";

    public static ModWorldConfig WorldConfig { get; private set; } = new();

    public static NewsFeeder NewsFeeder { get; } = new();

    private static string BuildConfigFilePath(string root)
    {
        return Path.Combine(root, _configFileName);
    }

    public static void LoadWorldConfig(string root)
    {
        var filePath = BuildConfigFilePath(root);
        if (File.Exists(filePath))
        {
            var text = IO.IsCompressed(filePath) ? IO.Decompress(filePath) : File.ReadAllText(filePath);
            WorldConfig = ModWorldConfig.Deserialize(text);
        }
        else
        {
            WorldConfig = new();
        }
    }

    public static void SaveWorldConfig(string root)
    {
        var filePath = BuildConfigFilePath(root);
        var text = WorldConfig.Serialize();
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
