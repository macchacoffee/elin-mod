using BepInEx.Configuration;

using Macchacoffee.ElinMods.ModUtility.Config;

namespace Macchacoffee.ElinMods.StarweaversMoongateQuickFix.Config;

internal class ModConfig : BepInExModConfigBase<ModConfig>
{
    private const string _general = "General";

    public BepInExModConfigEntry<int> ItemsPerPage { get; } = new(
        _general, "ItemsPerPage", 50,
        "1ページに表示するユーザーマップの最大件数。\nMaximum number of user maps shown per page.",
        new AcceptableValueRange<int>(10, 200));
}
