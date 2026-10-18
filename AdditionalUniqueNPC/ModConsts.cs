namespace Macchacoffee.ElinMods.AbilityRestriction;

internal static class ModConsts
{
    internal static class SourceId
    {
        private const string _prefix = "mc_aun_";
        private static string Prefixed(string id) => $"{_prefix}{id}";

        public static readonly string Misalie = Prefixed("misalie");
        public static readonly string DcFiamaPetExtra = Prefixed("dcFiamaPetExtra");
    }

    internal static class DramaStep
    {
        public static readonly string FiamaPetExtra1 = "fiama_pet_extra_1";
    }

    internal static class DramaFlag
    {
        private const string _prefix = "mc_aun_";
        private static string Prefixed(string id) => $"{_prefix}{id}";

        public static readonly string MisalieEverJoined = Prefixed("misalie_ever_joined");
    }
}
