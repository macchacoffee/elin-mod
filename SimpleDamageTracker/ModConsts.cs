namespace SimpleDamageTracker;

internal static class ModConsts
{
    internal static class GameObjectName
    {
        private const string Prefix = "MCSDT";
        private static string Prefixed(string id) => $"{Prefix}{id}";

        public static readonly string DamageDisplayDamage = Prefixed("DamageDisplayDamage");
        public static readonly string DamageDisplayPercentage = Prefixed("DamageDisplayPercentage");
    }

    internal static class SourceId
    {
        private const string Prefix = "mc_sdt_";
        private static string Prefixed(string id) => $"{Prefix}{id}";

        public static readonly string ModName = Prefixed("modName");
     }
}
