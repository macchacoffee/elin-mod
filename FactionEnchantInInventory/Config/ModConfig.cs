using ModUtility.Config;

namespace FactionEnchantInInventory.Config;

public class ModConfig : BepInExModConfigBase<ModConfig>
{
    private static readonly string _general = "General";

    public BepInExModConfigEntry<bool> EnablePermaCurse { get; } = new(
        _general, "EnablePermaCurse", false,
        "「それは装備するたびに呪われる」のエンチャントが付いている装備に対しても有効にする。\nEnable for equipment has the \"It curses itself every time it is equipped\" enchantment.");
}
