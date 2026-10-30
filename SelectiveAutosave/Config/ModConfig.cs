using Macchacoffee.ElinMods.ModUtility.Config;

namespace Macchacoffee.ElinMods.SelectiveAutosave.Config;

internal class ModConfig : BepInExModConfigBase<ModConfig>
{
    private const string _general = "General";
    private const string _experimental = "Experimental";

    public BepInExModConfigEntry<bool> SaveOnMapChange { get; } = new(
        _general, "SaveOnMapChange", false,
        "マップの移動に伴う自動セーブを有効にする。\nEnable autosaving when changing maps.");

    public BepInExModConfigEntry<bool> SaveOnSleep { get; } = new(
        _general, "SaveOnSleep", false,
        "睡眠が完了した後の自動セーブを有効にする。\nEnable autosaving after sleeping.");

    public BepInExModConfigEntry<bool> SaveOnBossDefeat { get; } = new(
        _general, "SaveOnBossDefeat", false,
        "ボス撃破による自動セーブを有効にする。\nEnable autosaving after defeating a boss.");

    public BepInExModConfigEntry<bool> SaveOnTreasureDiscovery { get; } = new(
        _general, "SaveOnTreasureDiscovery", false,
        "宝の地図の宝を掘り当てた後の自動セーブを有効にする。\nEnable autosaving after digging up treasure from a treasure map.");

    public BepInExModConfigEntry<bool> SaveOnRelocation { get; } = new(
        _general, "SaveOnRelocation", false,
        "拠点の移設に伴う自動セーブを有効にする。\nEnable autosaving when relocating a base.");

    public BepInExModConfigEntry<bool> SaveOnReturnToTitle { get; } = new(
        _general, "SaveOnReturnToTitle", false,
        "タイトルへ戻る時の自動セーブを有効にする。\nEnable autosaving when returning to the title screen.");

    public BepInExModConfigEntry<bool> SaveOnQuit { get; } = new(
        _general, "SaveOnQuit", false,
        "ゲーム内メニューから終了する時の自動セーブを有効にする。\nEnable autosaving when exiting from the in-game menu.");

    public BepInExModConfigEntry<bool> SaveOnEvent { get; } = new(
        _experimental, "SaveOnEvent", true,
        "イベント (会話など) による強制セーブを有効にする。\nEnable forced saves triggered by events, such as dialogue.");
}
