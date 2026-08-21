namespace Macchacoffee.ElinMods.StarweaversMoongateQuickFix;

internal static class ModConsts
{
    internal static class SourceId
    {
        private const string _prefix = "mc_smqf_";
        private static string Prefixed(string id) => $"{_prefix}{id}";

        public static readonly string PreviousPage = Prefixed("previousPage");
        public static readonly string NextPage = Prefixed("nextPage");
    }
}
