using ModUtility.Config;
using UnityEngine;

namespace SomewhatEnhancedDisplay.Config;

public class ModConfig : BepInExModConfigBase<ModConfig>
{
    private static readonly string _hoverGuide = "HoverGuide";

    public BepInExModConfigEntry<KeyCode> NextStyleKey { get; } = new(
        _hoverGuide, "NextStyleKey", KeyCode.H,
        "次のスタイルに切り替えるキー\nKey to switch to the next style");

    public BepInExModConfigEntry<KeyCode> LockKey { get; } = new(
        _hoverGuide, "LockKey", KeyCode.L,
        "ホバーガイドの対象ロックを切り替えるキー\nKey to toggle the hover guide target lock");
}
