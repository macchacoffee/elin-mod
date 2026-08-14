using ModUtility.Config;

namespace FactionEnchantInInventory.Config;

internal class ModConfig : BepInExModConfigBase<ModConfig>
{
    private static readonly string _general = "General";

    public BepInExModConfigEntry<bool> EnableRecursiveCurse { get; } = new(
        _general, "EnableRecursiveCurse", false,
        "「それは装備するたびに呪われる」のエンチャントが付いている装備に対しても有効にする。\nEnable for equipment has the \"It curses itself every time it is equipped\" enchantment.");
}
