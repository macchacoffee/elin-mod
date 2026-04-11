public static class ModConsts
{
    public static class SourceId
    {
        private const string PREFIX = "mc_sed_";
        private static string Prefixed(string id) => $"{PREFIX}{id}";

        public static readonly string ModName = Prefixed("modName");
        public static readonly string ConfigSectionGeneral = Prefixed("configSectionGeneral");
        public static readonly string ConfigSectionHoverGuide = Prefixed("configSectionHoverGuide");
        public static readonly string ConfigEnable = Prefixed("configEnable");
        public static readonly string ConfigBaseFontSize = Prefixed("configBaseFontSize");
        public static readonly string Fressness = Prefixed("fressness");
        public static readonly string LockLv = Prefixed("lock_lv");
    }
}