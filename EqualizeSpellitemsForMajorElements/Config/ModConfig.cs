using Macchacoffee.ElinMods.ModUtility.Config;

namespace Macchacoffee.ElinMods.EqualizeSpellitemsForMajorElements.Config;

internal class ModConfig : BepInExModConfigBase<ModConfig>
{
    private const string _general = "General";

    public BepInExModConfigEntry<bool> EnableImpact { get; } = new(
        _general, "EnableImpact", false,
        "衝撃属性に対しても有効にする。\nEnable for impact element.");
}
