namespace Macchacoffee.ElinMods.StarweaversMoongatePaging;

internal static class ModConsts
{
    internal static class SourceId
    {
        private const string _prefix = "mc_smp_";
        private static string Prefixed(string id) => $"{_prefix}{id}";

        public static readonly string PreviousPage = Prefixed("previousPage");
        public static readonly string NextPage = Prefixed("nextPage");
    }
}
