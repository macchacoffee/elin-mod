namespace AbilityRestriction;

internal static class ModConsts
{
    internal static class SourceId
    {
        private const string Prefix = "mc_ar_";
        private static string Prefixed(string id) => $"{Prefix}{id}";

        public static readonly string RestrictAbilities = Prefixed("restrictAbilities");
        public static readonly string Party = Prefixed("party");
        public static readonly string DaRestrictAbilities = Prefixed("daRestrictAbilities");
     }
}
