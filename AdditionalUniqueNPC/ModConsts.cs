namespace Macchacoffee.ElinMods.AdditionalUniqueNPC;

internal static class ModConsts
{
    internal static class SourceId
    {
        private const string _prefix = "mc_aun_";
        private static string Prefixed(string id) => $"{_prefix}{id}";

        public static readonly string Misalie = Prefixed("misalie");
        public static readonly string DcFiamaPetExtra = Prefixed("dcFiamaPetExtra");
    }

    internal static class SourceNumericId
    {
        private const int _base = 395820000;
        private static int Id(int id) => _base + id;

        public static readonly int FeatManaAbsorber = Id(1);
        public static readonly int FeatDemonBlood = Id(2);
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
