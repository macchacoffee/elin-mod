namespace Macchacoffee.ElinMods.AddPalmiaTimesNewsToLog;

internal static class ModConsts
{
    public static class GameObjectName
    {
        private const string _prefix = "MCAPTNTL";
        private static string Prefixed(string id) => $"{_prefix}{id}";
    }

    public static class SourceId
    {
        private const string _prefix = "mc_aptntl_";
        private static string Prefixed(string id) => $"{_prefix}{id}";
    }
}
