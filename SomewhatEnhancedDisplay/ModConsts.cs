public static class ModConsts
{
    public static class GameObjectName
    {
        private const string PREFIX = "MCSED";
        private static string Prefixed(string id) => $"{PREFIX}{id}";

        public static readonly string HoverGuidePadding = Prefixed("HoverGuidePadding");
        public static readonly string HealthBar = Prefixed("HealthBar");
        public static readonly string HealthBarBG = Prefixed("HealthBarBG");
        public static readonly string HealthBarFGDamege = Prefixed("HealthBarBG");
        public static readonly string HealthBarFG = Prefixed("HealthBarBG");
        public static readonly string HealthBarValue = Prefixed("HealthBarValue");
        public static readonly string Config = Prefixed("Config");
        public static readonly string ConfigGenaral = Prefixed("ConfigGenaral");
        public static readonly string ConfigStyle = Prefixed("ConfigStyle");
        public static readonly string ColorPicker = Prefixed("ColorPicker");
    }
    public static class SourceId
    {
        private const string PREFIX = "mc_sed_";
        private static string Prefixed(string id) => $"{PREFIX}{id}";

        public static readonly string ModName = Prefixed("modName");
        public static readonly string Attributes = Prefixed("attributes");
        public static readonly string HP = Prefixed("hp");
        public static readonly string Mana = Prefixed("mana");
        public static readonly string Stamina = Prefixed("stamina");
        public static readonly string Resist = Prefixed("resist");
        public static readonly string HealthBar = Prefixed("healthBar");
        public static readonly string Rarity = Prefixed("rarity");
        public static readonly string Fressness = Prefixed("fressness");
        public static readonly string LockLv = Prefixed("lockLv");
        public static readonly string ConfigHoverGuide = Prefixed("configHoverGuide");
        public static readonly string ConfigGeneral = Prefixed("configGeneral");
        public static readonly string ConfigStyle = Prefixed("configStyle");
        public static readonly string ConfigChara = Prefixed("configChara");
        public static readonly string ConfigThing = Prefixed("configThing");
        public static readonly string ConfigDisplay = Prefixed("configDisplay");
        public static readonly string ConfigColors = Prefixed("configColors");
        public static readonly string ConfigSelectStyleToEdit = Prefixed("configSelectStyleToEdit");
        public static readonly string ConfigAddStyle = Prefixed("configAddStyle");
        public static readonly string ConfigDeleteStyle = Prefixed("configDeleteStyle");
        public static readonly string ResetConfig = Prefixed("resetConfig");
        public static readonly string ResetConfigTab = Prefixed("resetConfigTab");
        public static readonly string DialogResetConfig = Prefixed("dialogResetConfig");
        public static readonly string DialogResetConfigTab = Prefixed("dialogResetConfigTab"); 
        public static readonly string DialogDeleteStyle = Prefixed("dialogDeleteStyle"); 
        public static readonly string ZoomScale = Prefixed("zoomScale");
        public static readonly string DefaultColor = Prefixed("defaultColor");
        public static readonly string MainTextColor = Prefixed("mainTextColor");
        public static readonly string SubTextColor = Prefixed("subTextColor");
        public static readonly string AttributeLabelColor = Prefixed("attributeLabelColor");
        public static readonly string AttributeValueColor = Prefixed("attributeValueColor");
        public static readonly string ResistLabelColor = Prefixed("resistLabelColor");
        public static readonly string NegativeResistLabelColor = Prefixed("negativeResistLabelColor");
        public static readonly string NoneResistLabelColor = Prefixed("noneResistLabelColor");
        public static readonly string HealthBarBGColor = Prefixed("healthBarBGColor");
        public static readonly string HealthBarFGColor = Prefixed("healthBarFGColor");
        public static readonly string HealthBarFGDamageColor = Prefixed("healthBarFGDamageColor");
        public static readonly string HealthBarLowValueFGColor = Prefixed("healthBarLowValueFGColor");
        public static readonly string HealthBarTextColor = Prefixed("healthBarTextColor");
        public static readonly string HealthBarLowValueTextColor = Prefixed("healthBarLowValueTextColor");
        public static readonly string FressnessValueColor = Prefixed("fressnessValueColor");
        public static readonly string FressnessLowValueColor = Prefixed("fressnessLowValueColor");
        public static readonly string RarityCrudeColor = Prefixed("rarityCrudeColor");
        public static readonly string RarityNormalColor = Prefixed("rarityNormalColor");
        public static readonly string RaritySuperiorColor = Prefixed("raritySuperiorColor");
        public static readonly string RarityLegendaryColor = Prefixed("rarityLegendaryColor");
        public static readonly string RarityMythicalColor = Prefixed("rarityMythicalColor");
        public static readonly string RarityArtifactColor = Prefixed("rarityArtifactColor");
    }
}