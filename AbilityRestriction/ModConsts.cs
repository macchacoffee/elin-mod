namespace Macchacoffee.ElinMods.AbilityRestriction;

internal static class ModConsts
{
    internal static class SourceId
    {
        private const string _prefix = "mc_ar_";
        private static string Prefixed(string id) => $"{_prefix}{id}";

        public static readonly string RestrictAbilities = Prefixed("restrictAbilities");
        public static readonly string Party = Prefixed("party");
        public static readonly string DaRestrictAbilities = Prefixed("daRestrictAbilities");
     }
}
