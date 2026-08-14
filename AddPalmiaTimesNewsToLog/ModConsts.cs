namespace AddPalmiaTimesNewsToLog;

internal static class ModConsts
{
    internal static class GameObjectName
    {
        private const string Prefix = "MCAPTNTL";
        private static string Prefixed(string id) => $"{Prefix}{id}";
    }

    internal static class SourceId
    {
        private const string Prefix = "mc_aptntl_";
        private static string Prefixed(string id) => $"{Prefix}{id}";
    }
}