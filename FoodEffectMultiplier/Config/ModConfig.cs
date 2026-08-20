using BepInEx.Configuration;
using Macchacoffee.ElinMods.ModUtility.Config;

namespace Macchacoffee.ElinMods.FoodEffectMultiplier.Config;

internal class ModConfig : BepInExModConfigBase<ModConfig>
{
    private const string _general = "General";

    public BepInExModConfigEntry<float> PCMultiplier { get; } = new(
        _general, "PCMultiplier", -1f,
        "PCの食事効果倍率 (負の値の場合はゲームのデフォルト値を使用 (1倍))\nPC Food Effect Mutiplier (negative value means game default multiplier (1x))");

    public BepInExModConfigEntry<float> NPCMultiplier { get; } = new(
        _general, "NPCMultiplier", -1f,
        "NPCの食事効果倍率 (負の値の場合はゲームのデフォルト値を使用 (3倍))\nNPC Food Effect Mutiplier (negative value means game default multiplier (3x))");
}
