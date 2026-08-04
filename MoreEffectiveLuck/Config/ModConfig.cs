using ModUtility.Config;

namespace MoreEffectiveLuck.Config;

public class ModConfig : BepInExModConfigBase<ModConfig>
{
    private const string EnchantPower = "EnchantPower";
    private const string EquipmentRarity = "EquipmentRarity";
    private const string SpecialMerchantRarity = "SpecialMerchantRarity";
    private const string ReuqestReward = "ReuqestReward";
    private const string LuckChanger = "LuckChanger";

    public BepInExModConfigEntry<bool> EnableEnchantPower { get; } = new(
        EnchantPower, "Enable", true,
        "ランダムエンチャントの強度に対して有効にする。\nEnable for power of random enchant.");

    public BepInExModConfigEntry<int> EnchantPowerLuckPerRoll { get; } = new(
        EnchantPower, "LuckPerRoll", 100,
        "ランダムエンチャントの強度 / 1ロール追加に必要な運の値\nPower of random enchant / Required luck value for an extra roll");

    public BepInExModConfigEntry<int> EnchantPowerMaxRoll { get; } = new(
        EnchantPower, "MaxRoll", 20,
        "ランダムエンチャントの強度 / ロール数の上限\nPower of random enchant / Maximum roll count");

    public BepInExModConfigEntry<bool> EnableEquipmentRarity { get; } = new(
        EquipmentRarity, "Enable", true,
        "ランダム生成装備のレアリティに対して有効にする。\nEnable for rarity of Randomly generated equipment.");

    public BepInExModConfigEntry<int> EquipmentRarityLuckPerRoll { get; } = new(
        EquipmentRarity, "LuckPerRoll", 150,
        "ランダム生成装備のレアリティ / 1ロール追加に必要な運の値\nRarity of Randomly generated equipment / Required luck value for an extra roll");

    public BepInExModConfigEntry<int> EquipmentRarityMaxRoll { get; } = new(
        EquipmentRarity, "MaxRoll", 15,
        "ランダム生成装備のレアリティ / ロール数の上限\nRarity of Randomly generated equipment / Maximum roll count");

    public BepInExModConfigEntry<bool> EnableSpecialMerchantRarity { get; } = new(
        SpecialMerchantRarity, "Enable", true,
        "特殊な商人 (ブラックマーケットなど) が販売する装備のレアリティに対して有効にする。\nEnable for rarity of equipment sold by special merchant (e.g., blackmarket).");

    public BepInExModConfigEntry<int> SpecialMerchantRarityLuckPerRoll { get; } = new(
        SpecialMerchantRarity, "LuckPerRoll", 200,
        "特殊な商人が販売する装備のレアリティ / 1ロール追加に必要な運の値\nRarity of equipment sold by special merchant / Required luck value for an extra roll");

    public BepInExModConfigEntry<int> SpecialMerchantRarityMaxRoll { get; } = new(
        SpecialMerchantRarity, "MaxRoll", 10,
        "特殊な商人が販売する装備のレアリティ / ロール数の上限\nRarity of equipment sold by special merchant / Maximum roll count");

    public BepInExModConfigEntry<bool> EnableReuqestReward { get; } = new(
        ReuqestReward, "Enable", true,
        "依頼の報酬に対して有効にする。\nEnable for request reward.");

    public BepInExModConfigEntry<int> ReuqestRewardLuckPerRoll { get; } = new(
        ReuqestReward, "LuckPerRoll", 100,
         "依頼の報酬 / 1ロール追加に必要な運の値\nRequest reward / Required luck value for an extra roll");

    public BepInExModConfigEntry<int> ReuqestRewardMaxRoll { get; } = new(
        ReuqestReward, "MaxRoll", 20,
        "依頼の報酬 / ロール数の上限\nRequest reward / Maximum roll count");

    public BepInExModConfigEntry<bool> EnableLuckyFood { get; } = new(
        LuckChanger, "EnableLuckyFood", true,
        "運気が向上する食べ物を有効にする。\nEnable lucky food.");

    public BepInExModConfigEntry<bool> EnableLuckyDay { get; } = new(
        LuckChanger, "EnableLuckyDay", true,
        "幸運の日を有効にする。\nEnable lucky day.");

    public BepInExModConfigEntry<bool> EnableLuckyMonth { get; } = new(
        LuckChanger, "EnableLuckyMonth", true,
        "幸運の月を有効にする。\nEnable lucky month.");
}
