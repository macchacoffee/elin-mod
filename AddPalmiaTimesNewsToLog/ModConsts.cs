namespace AddPalmiaTimesNewsToLog;

public static class ModConsts
{
    public static class GameObjectName
    {
        private const string PREFIX = "MCAPTNTL";
        private static string Prefixed(string id) => $"{PREFIX}{id}";
    }
    public static class SourceId
    {
        private const string PREFIX = "mc_aptntl_";
        private static string Prefixed(string id) => $"{PREFIX}{id}";
    }
}