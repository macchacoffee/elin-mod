namespace SimpleDamageTracker;

internal static class ModConsts
{
    internal static class GameObjectName
    {
        private const string Prefix = "MCSDT";
        private static string Prefixed(string id) => $"{Prefix}{id}";

        public static readonly string ConfigGenaral = Prefixed("ConfigGenaral");
        public static readonly string DamageDisplayDamage = Prefixed("DamageDisplayDamage");
        public static readonly string DamageDisplayPercentage = Prefixed("DamageDisplayPercentage");
    }

    internal static class SourceId
    {
        private const string Prefix = "mc_sdt_";
        private static string Prefixed(string id) => $"{Prefix}{id}";

        public static readonly string ModName = Prefixed("modName");
        public static readonly string ConfigGeneral = Prefixed("configGeneral");
        public static readonly string ConfigDisplayItems = Prefixed("configDisplayItems");
        public static readonly string ConfigDamage = Prefixed("configDamage");
        public static readonly string ConfigDamageShare = Prefixed("configDamageShare");
        public static readonly string DisplayNoDamage = Prefixed("displayNoDamage");
        public static readonly string UseAnimation = Prefixed("useAnimation");
        public static readonly string UseCompactDamageFormat = Prefixed("useCompactDamageFormat");
        public static readonly string Display = Prefixed("display");
        public static readonly string PositionX = Prefixed("positionX");
        public static readonly string PositionY = Prefixed("positionY");
        public static readonly string SizeScale = Prefixed("sizeScale");
        public static readonly string HorizontalAlignment = Prefixed("horizontalAlignment");
        public static readonly string AlignmentLeft = Prefixed("alignmentLeft");
        public static readonly string AlignmentCenter = Prefixed("alignmentCenter");
        public static readonly string AlignmentRight = Prefixed("alignmentRight");
        public static readonly string Color = Prefixed("color");
        public static readonly string ResetConfig = Prefixed("resetConfig");
        public static readonly string DialogResetConfig = Prefixed("dialogResetConfig");
        public static readonly string TooltipUseCompactDamageFormat = Prefixed("tooltipUseCompactDamageFormat");
    }
}
