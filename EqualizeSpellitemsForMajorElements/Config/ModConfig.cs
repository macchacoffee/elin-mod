using ModUtility.Config;

namespace EqualizeSpellitemsForMajorElements.Config;

public class ModConfig : BepInExModConfigBase<ModConfig>
{
    private static readonly string _general = "General";

    public BepInExModConfigEntry<bool> EnableImpact { get; } = new(
        _general, "EnableImpact", false,
        "衝撃属性を有効にする。\nEnable for impact element.");
}
