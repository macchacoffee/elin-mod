using BepInEx.Configuration;
using ModUtility.Config;

namespace FoodEffectMultiplier.Config;

public class ModConfig : BepInExModConfigBase<ModConfig>
{
    private static readonly string General = "General";

    public BepInExModConfigEntry<float> PCMultiplier { get; } = new(
        General, "PCMultiplier", -1f,
        "PCの食事効果倍率 (負の値の場合はゲームのデフォルト値を使用 (1倍))\nPC Food Effect Mutiplier (negative value means game default multiplier (1x))");

    public BepInExModConfigEntry<float> NPCMultiplier { get; } = new(
        General, "NPCMultiplier", -1f,
        "NPCの食事効果倍率 (負の値の場合はゲームのデフォルト値を使用 (3倍))\nNPC Food Effect Mutiplier (negative value means game default multiplier (3x))");
}
