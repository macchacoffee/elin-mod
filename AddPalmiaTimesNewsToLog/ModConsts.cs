namespace AddPalmiaTimesNewsToLog;

public static class ModConsts
{
    public static class GameObjectName
    {
        private const string Prefix = "MCAPTNTL";
        private static string Prefixed(string id) => $"{Prefix}{id}";
    }

    public static class SourceId
    {
        private const string Prefix = "mc_aptntl_";
        private static string Prefixed(string id) => $"{Prefix}{id}";
    }
}