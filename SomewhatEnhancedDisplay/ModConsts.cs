namespace SomewhatEnhancedDisplay;

public static class ModConsts
{
    public static class GameObjectName
    {
        private const string PREFIX = "MCSED";
        private static string Prefixed(string id) => $"{PREFIX}{id}";

        public static readonly string HoverGuidePadding = Prefixed("HoverGuidePadding");
        public static readonly string HealthBar = Prefixed("HealthBar");
        public static readonly string HealthBarBG = Prefixed("HealthBarBG");
        public static readonly string HealthBarFG = Prefixed("HealthBarFG1");
        public static readonly string HealthBarFGDamage = Prefixed("HealthBarFGDamage");
        public static readonly string HealthBarFGRestore = Prefixed("HealthBarFGRestore");
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
        public static readonly string Chara = Prefixed("chara");
        public static readonly string Thing = Prefixed("thing");
        public static readonly string Attributes = Prefixed("attributes");
        public static readonly string HP = Prefixed("hp");
        public static readonly string Mana = Prefixed("mana");
        public static readonly string Stamina = Prefixed("stamina");
        public static readonly string Resist = Prefixed("resist");
        public static readonly string HealthBar = Prefixed("healthBar");
        public static readonly string Rarity = Prefixed("rarity");
        public static readonly string Material = Prefixed("material");
        public static readonly string Fressness = Prefixed("fressness");
        public static readonly string LockLv = Prefixed("lockLv");
        public static readonly string Line = Prefixed("line");
        public static readonly string Others = Prefixed("others");
        public static readonly string Type = Prefixed("type");
        public static readonly string Lv = Prefixed("lv");
        public static readonly string Gender = Prefixed("gender");
        public static readonly string Age = Prefixed("age");
        public static readonly string Race = Prefixed("race");
        public static readonly string JobTactics = Prefixed("jobTactics");
        public static readonly string Hobby = Prefixed("hobby");
        public static readonly string Affinity = Prefixed("affinity");
        public static readonly string Favorite = Prefixed("favorite");
        public static readonly string DVPV = Prefixed("dvPV");
        public static readonly string Speed = Prefixed("speed");
        public static readonly string Exp = Prefixed("exp");
        public static readonly string MainElement = Prefixed("mainElement");
        public static readonly string PrimaryAttributes = Prefixed("primaryAttributes");
        public static readonly string Feat = Prefixed("feat");
        public static readonly string FeatValue = Prefixed("featValue");
        public static readonly string Act = Prefixed("act");
        public static readonly string ActParty = Prefixed("actParty");
        public static readonly string ResistValue = Prefixed("resistValue");
        public static readonly string Stats = Prefixed("stats");
        public static readonly string StatsValue = Prefixed("statsValue");
        public static readonly string EnableMimicry = Prefixed("enableMimicry");
        public static readonly string UseRarityColor = Prefixed("useRarityColor");
        public static readonly string Width = Prefixed("width");
        public static readonly string DisplayValue = Prefixed("displayValue");
        public static readonly string UseAnimation = Prefixed("useAnimation");
        public static readonly string HealthBarConditions = Prefixed("healthBarConditions");
        public static readonly string Enemy = Prefixed("enemy");
        public static readonly string Netural = Prefixed("netural");
        public static readonly string Friend = Prefixed("friend");
        public static readonly string Ally = Prefixed("ally");
        public static readonly string Target = Prefixed("target");
        public static readonly string TargetNone = Prefixed("targetNone");
        public static readonly string TargetAll = Prefixed("targetAll");
        public static readonly string TargetElite = Prefixed("targetElite");
        public static readonly string TargetBoss = Prefixed("targetBoss");
        public static readonly string NotInCombat = Prefixed("notInCombat");
        public static readonly string InFullHealth = Prefixed("inFullHealth");
        public static readonly string ConfigOf = Prefixed("configOf");
        public static readonly string ConfigHoverGuide = Prefixed("configHoverGuide");
        public static readonly string ConfigGeneral = Prefixed("configGeneral");
        public static readonly string ConfigStyle = Prefixed("configStyle");
        public static readonly string ConfigDisplay = Prefixed("configDisplay");
        public static readonly string ConfigColors = Prefixed("configColors");
        public static readonly string ConfigSelectStyleToEdit = Prefixed("configSelectStyleToEdit");
        public static readonly string ConfigAddStyle = Prefixed("configAddStyle");
        public static readonly string ConfigDeleteStyle = Prefixed("configDeleteStyle");
        public static readonly string ConfigDisplayItems = Prefixed("configDisplayItems");
        public static readonly string ResetConfig = Prefixed("resetConfig");
        public static readonly string ResetConfigTab = Prefixed("resetConfigTab");
        public static readonly string DialogResetConfig = Prefixed("dialogResetConfig");
        public static readonly string DialogResetConfigTab = Prefixed("dialogResetConfigTab"); 
        public static readonly string DialogDeleteStyle = Prefixed("dialogDeleteStyle"); 
        public static readonly string ZoomScale = Prefixed("zoomScale");
        public static readonly string HorizontalPivot = Prefixed("horizontalPivot");
        public static readonly string VerticalPivot = Prefixed("verticalPivot");
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
        public static readonly string HealthBarFGRestoreColor = Prefixed("healthBarFGRestoreColor");
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