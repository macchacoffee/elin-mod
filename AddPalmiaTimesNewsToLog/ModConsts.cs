namespace AddPalmiaTimesNewsToLog;

internal static class ModConsts
{
    internal static class GameObjectName
    {
        private const string _prefix = "MCAPTNTL";
        private static string Prefixed(string id) => $"{_prefix}{id}";
    }

    internal static class SourceId
    {
        private const string _prefix = "mc_aptntl_";
        private static string Prefixed(string id) => $"{_prefix}{id}";
    }
}