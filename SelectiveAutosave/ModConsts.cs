namespace Macchacoffee.ElinMods.SelectiveAutosave;

internal static class ModConsts
{
    public static class SourceId
    {
        private const string _prefix = "mc_sa_";
        private static string Prefixed(string id) => $"{_prefix}{id}";

        public static readonly string ConfirmReturnToTitle = Prefixed("confirmReturnToTitle");
        public static readonly string ConfirmQuit = Prefixed("confirmQuit");
    }
}
