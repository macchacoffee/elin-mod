using UnityEngine;

using Macchacoffee.ElinMods.ModUtility.Config;

namespace Macchacoffee.ElinMods.SomewhatEnhancedDisplay.Config;

internal class ModConfig : BepInExModConfigBase<ModConfig>
{
    private const string _general = "General";
    private const string _hoverGuide = "HoverGuide";

    public BepInExModConfigEntry<bool> EnableHoverGuide { get; } = new(
        _general, "EnableHoverGuide", true,
        "ホバーガイド関連の機能を有効にする (再起動が必要)。\nEnable hover guide features (restart required).");
    public BepInExModConfigEntry<bool> EnableDNA { get; } = new(
        _general, "EnableDNA", true,
        "遺伝子関連の機能を有効にする (再起動が必要)。\nEnable DNA features (restart required).");
    public BepInExModConfigEntry<bool> EnableEnchant { get; } = new(
        _general, "EnableEnchant", true,
        "エンチャント関連の機能を有効にする (再起動が必要)。\nEnable enchant features (restart required).");
    public BepInExModConfigEntry<bool> EnableStatusNotification { get; } = new(
        _general, "EnableStatusNotification", true,
        "ステータス通知関連の機能を有効にする (再起動が必要)。\nEnable status notification features (restart required).");

    public BepInExModConfigEntry<KeyCode> NextStyleKey { get; } = new(
        _hoverGuide, "NextStyleKey", KeyCode.H,
        "次のスタイルに切り替えるキー\nKey to switch to the next style");

    public BepInExModConfigEntry<KeyCode> LockKey { get; } = new(
        _hoverGuide, "LockKey", KeyCode.L,
        "ホバーガイドの対象ロックを切り替えるキー\nKey to toggle the hover guide target lock");
}
