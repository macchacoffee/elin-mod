using Macchacoffee.ElinMods.ModUtility.Config;

namespace Macchacoffee.ElinMods.MoreEffectiveLuck.Config;

internal class ModConfig : BepInExModConfigBase<ModConfig>
{
    private const string _enchantPower = "EnchantPower";
    private const string _equipmentRarity = "EquipmentRarity";
    private const string _specialMerchantRarity = "SpecialMerchantRarity";
    private const string _reuqestReward = "ReuqestReward";
    private const string _luckChanger = "LuckChanger";

    public BepInExModConfigEntry<bool> EnableEnchantPower { get; } = new(
        _enchantPower, "Enable", true,
        "ランダムエンチャントの強度に対して有効にする。\nEnable for power of random enchant.");

    public BepInExModConfigEntry<int> EnchantPowerLuckPerRoll { get; } = new(
        _enchantPower, "LuckPerRoll", 100,
        "ランダムエンチャントの強度 / 1ロール追加に必要な運の値\nPower of random enchant / Required luck value for an extra roll");

    public BepInExModConfigEntry<int> EnchantPowerMaxRoll { get; } = new(
        _enchantPower, "MaxRoll", 20,
        "ランダムエンチャントの強度 / ロール数の上限\nPower of random enchant / Maximum roll count");

    public BepInExModConfigEntry<bool> EnableEquipmentRarity { get; } = new(
        _equipmentRarity, "Enable", true,
        "ランダム生成装備のレアリティに対して有効にする。\nEnable for rarity of Randomly generated equipment.");

    public BepInExModConfigEntry<int> EquipmentRarityLuckPerRoll { get; } = new(
        _equipmentRarity, "LuckPerRoll", 150,
        "ランダム生成装備のレアリティ / 1ロール追加に必要な運の値\nRarity of Randomly generated equipment / Required luck value for an extra roll");

    public BepInExModConfigEntry<int> EquipmentRarityMaxRoll { get; } = new(
        _equipmentRarity, "MaxRoll", 15,
        "ランダム生成装備のレアリティ / ロール数の上限\nRarity of Randomly generated equipment / Maximum roll count");

    public BepInExModConfigEntry<bool> EnableSpecialMerchantRarity { get; } = new(
        _specialMerchantRarity, "Enable", true,
        "特殊な商人 (ブラックマーケットなど) が販売する装備のレアリティに対して有効にする。\nEnable for rarity of equipment sold by special merchant (e.g., blackmarket).");

    public BepInExModConfigEntry<int> SpecialMerchantRarityLuckPerRoll { get; } = new(
        _specialMerchantRarity, "LuckPerRoll", 200,
        "特殊な商人が販売する装備のレアリティ / 1ロール追加に必要な運の値\nRarity of equipment sold by special merchant / Required luck value for an extra roll");

    public BepInExModConfigEntry<int> SpecialMerchantRarityMaxRoll { get; } = new(
        _specialMerchantRarity, "MaxRoll", 10,
        "特殊な商人が販売する装備のレアリティ / ロール数の上限\nRarity of equipment sold by special merchant / Maximum roll count");

    public BepInExModConfigEntry<bool> EnableReuqestReward { get; } = new(
        _reuqestReward, "Enable", true,
        "依頼の報酬に対して有効にする。\nEnable for request reward.");

    public BepInExModConfigEntry<int> ReuqestRewardLuckPerRoll { get; } = new(
        _reuqestReward, "LuckPerRoll", 100,
         "依頼の報酬 / 1ロール追加に必要な運の値\nRequest reward / Required luck value for an extra roll");

    public BepInExModConfigEntry<int> ReuqestRewardMaxRoll { get; } = new(
        _reuqestReward, "MaxRoll", 20,
        "依頼の報酬 / ロール数の上限\nRequest reward / Maximum roll count");

    public BepInExModConfigEntry<bool> EnableLuckyFood { get; } = new(
        _luckChanger, "EnableLuckyFood", true,
        "運気が向上する食べ物を有効にする。\nEnable lucky food.");

    public BepInExModConfigEntry<bool> EnableLuckyDay { get; } = new(
        _luckChanger, "EnableLuckyDay", true,
        "幸運の日を有効にする。\nEnable lucky day.");

    public BepInExModConfigEntry<bool> EnableLuckyMonth { get; } = new(
        _luckChanger, "EnableLuckyMonth", true,
        "幸運の月を有効にする。\nEnable lucky month.");
}
