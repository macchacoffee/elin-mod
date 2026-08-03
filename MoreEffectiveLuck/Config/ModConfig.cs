using System;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;

namespace MoreEffectiveLuck.Config;

public class ModConfig
{
    private class ConfigBinding(ConfigFile configFile)
    {
        private static readonly string EnchantPower = "EnchantPower";
        private static readonly string EquipmentRarity = "EquipmentRarity";
        private static readonly string SpecialMerchantRarity = "SpecialMerchantRarity";
        private static readonly string ReuqestReward = "ReuqestReward";
        private static readonly string LuckChanger = "LuckChanger";

        public readonly ConfigFile ConfigFile = configFile;

        public ConfigEntry<bool> EnableEnchantPower = configFile.Bind(EnchantPower, "Enable", true, new ConfigDescription(
            "ランダムエンチャントの強度に対して有効にする。\nEnable for power of random enchant."
        ));
        public ConfigEntry<int> EnchantPowerLuckPerRoll = configFile.Bind(EnchantPower, "LuckPerRoll", 100, new ConfigDescription(
            "ランダムエンチャントの強度 / 1ロール追加に必要な運の値\nPower of random enchant / Required luck value for an extra roll"
        ));
        public ConfigEntry<int> EnchantPowerMaxRoll = configFile.Bind(EnchantPower, "MaxRoll", 20, new ConfigDescription(
            "ランダムエンチャントの強度 / ロール数の上限\nPower of random enchant / Maximum roll count"
        ));
        public ConfigEntry<bool> EnableEquipmentRarity = configFile.Bind(EquipmentRarity, "Enable", true, new ConfigDescription(
            "ランダム生成装備のレアリティに対して有効にする。\nEnable for rarity of Randomly generated equipment."
        ));
        public ConfigEntry<int> EquipmentRarityLuckPerRoll = configFile.Bind(EquipmentRarity, "LuckPerRoll", 150, new ConfigDescription(
            "ランダム生成装備のレアリティ / 1ロール追加に必要な運の値\nRarity of Randomly generated equipment / Required luck value for an extra roll"
        ));
        public ConfigEntry<int> EquipmentRarityMaxRoll = configFile.Bind(EquipmentRarity, "MaxRoll", 15, new ConfigDescription(
            "ランダム生成装備のレアリティ / ロール数の上限\nRarity of Randomly generated equipment / Maximum roll count"
        ));
        public ConfigEntry<bool> EnableSpecialMerchantRarity = configFile.Bind(SpecialMerchantRarity, "Enable", true, new ConfigDescription(
            "特殊な商人 (ブラックマーケットなど) が販売する装備のレアリティに対して有効にする。\nEnable for rarity of equipment sold by special merchant (e.g., blackmarket)."
        ));
        public ConfigEntry<int> SpecialMerchantRarityLuckPerRoll = configFile.Bind(SpecialMerchantRarity, "LuckPerRoll", 200, new ConfigDescription(
            "特殊な商人が販売する装備のレアリティ / 1ロール追加に必要な運の値\nRarity of equipment sold by special merchant / Required luck value for an extra roll"
        ));
        public ConfigEntry<int> SpecialMerchantRarityMaxRoll = configFile.Bind(SpecialMerchantRarity, "MaxRoll", 10, new ConfigDescription(
            "特殊な商人が販売する装備のレアリティ / ロール数の上限\nRarity of equipment sold by special merchant / Maximum roll count"
        ));
        public ConfigEntry<bool> EnableReuqestReward = configFile.Bind(ReuqestReward, "Enable", true, new ConfigDescription(
            "依頼の報酬に対して有効にする。\nEnable for request reward."
        ));
        public ConfigEntry<int> ReuqestRewardLuckPerRoll = configFile.Bind(ReuqestReward, "LuckPerRoll", 100, new ConfigDescription(
            "依頼の報酬 / 1ロール追加に必要な運の値\nRequest reward / Required luck value for an extra roll"
        ));
        public ConfigEntry<int> ReuqestRewardMaxRoll = configFile.Bind(ReuqestReward, "MaxRoll", 20, new ConfigDescription(
            "依頼の報酬 / ロール数の上限\nRequest reward / Maximum roll count"
        ));
        public ConfigEntry<bool> EnableLuckyFood = configFile.Bind(LuckChanger, "EnableLuckyFood", true, new ConfigDescription(
            "運気が向上する食べ物を有効にする。\nEnable lucky food."
        ));
        public ConfigEntry<bool> EnableLuckyDay = configFile.Bind(LuckChanger, "EnableLuckyDay", true, new ConfigDescription(
            "幸運の日を有効にする。\nEnable lucky day."
        ));
        public ConfigEntry<bool> EnableLuckyMonth = configFile.Bind(LuckChanger, "EnableLuckyMonth", true, new ConfigDescription(
            "幸運の月を有効にする。\nEnable lucky month."
        ));
    }

    private ConfigBinding? Binding { get; set; }

    public bool EnableEnchantPower => Binding!.EnableEnchantPower.Value;
    public int EnchantPowerLuckPerRoll => Binding!.EnchantPowerLuckPerRoll.Value;
    public int EnchantPowerMaxRoll => Binding!.EnchantPowerMaxRoll.Value;
    public bool EnableEquipmentRarity => Binding!.EnableEquipmentRarity.Value;
    public int EquipmentRarityLuckPerRoll => Binding!.EquipmentRarityLuckPerRoll.Value;
    public int EquipmentRarityMaxRoll => Binding!.EquipmentRarityMaxRoll.Value;
    public bool EnableSpecialMerchantRarity => Binding!.EnableSpecialMerchantRarity.Value;
    public int SpecialMerchantRarityLuckPerRoll => Binding!.SpecialMerchantRarityLuckPerRoll.Value;
    public int SpecialMerchantRarityMaxRoll => Binding!.SpecialMerchantRarityMaxRoll.Value;
    public bool EnableReuqestReward => Binding!.EnableReuqestReward.Value;
    public int ReuqestRewardLuckPerRoll => Binding!.ReuqestRewardLuckPerRoll.Value;
    public int ReuqestRewardMaxRoll => Binding!.ReuqestRewardMaxRoll.Value;
    public bool EnableLuckyFood => Binding!.EnableLuckyFood.Value;
    public bool EnableLuckyDay => Binding!.EnableLuckyDay.Value;
    public bool EnableLuckyMonth => Binding!.EnableLuckyMonth.Value;

    public void Bind(ConfigFile configFile)
    {
        Binding = new(configFile);
    }
}
