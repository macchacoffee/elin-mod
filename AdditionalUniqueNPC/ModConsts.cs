namespace Macchacoffee.ElinMods.AdditionalUniqueNPC;

internal static class ModConsts
{
    public static class SourceId
    {
        private const string _prefix = "mc_aun_";
        private static string Prefixed(string id) => $"{_prefix}{id}";

        public static readonly string Misalie = Prefixed("misalie");
        public static readonly string DcFiamaPetExtra = Prefixed("dcFiamaPetExtra");
    }

    public static class SourceNumericId
    {
        private const int _base = 395820000;
        private static int Id(int id) => _base + id;

        public static readonly int FeatManaAbsorber = Id(1);
        public static readonly int FeatDemonBlood = Id(2);
    }

    public static class DramaStep
    {
        public static readonly string FiamaPetExtra1 = "fiama_pet_extra_1";
    }

    public static class DramaFlag
    {
        private const string _prefix = "mc_aun_";
        private static string Prefixed(string id) => $"{_prefix}{id}";

        public static readonly string MisalieEverJoined = Prefixed("misalie_ever_joined");
    }
}
