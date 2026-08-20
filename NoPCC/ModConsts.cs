namespace Macchacoffee.ElinMods.NoPCC;

internal static class ModConsts
{
    internal static class SourceId
    {
        private const string _prefix = "mc_np_";
        private static string Prefixed(string id) => $"{_prefix}{id}";

        public static readonly string ModName = Prefixed("modName");
        public static readonly string Enable = Prefixed("enable");
        public static readonly string GeneralSettings = Prefixed("generalSettings");
        public static readonly string ReplacePCCtoSprite = Prefixed("replacePCCtoSprite");
        public static readonly string SpriteSettings = Prefixed("spriteSettings");
        public static readonly string DefaultSprite = Prefixed("defaultSprite");
        public static readonly string SnowSprite = Prefixed("snowSprite");
        public static readonly string UndressSprite = Prefixed("undressSprite");
        public static readonly string RideSprite = Prefixed("rideSprite");
        public static readonly string RideSnowSprite = Prefixed("rideSnowSprite");
        public static readonly string CombatSprite = Prefixed("combatSprite");
        public static readonly string CombatSnowSprite = Prefixed("combatSnowSprite");
        public static readonly string RideCombatSprite = Prefixed("rideCombatSprite");
        public static readonly string RideCombatSnowSprite = Prefixed("rideCombatSnowSprite");
     }
}
