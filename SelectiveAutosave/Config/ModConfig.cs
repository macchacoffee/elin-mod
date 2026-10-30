using Macchacoffee.ElinMods.ModUtility.Config;

namespace Macchacoffee.ElinMods.SelectiveAutosave.Config;

internal class ModConfig : BepInExModConfigBase<ModConfig>
{
    private const string _general = "General";

    public BepInExModConfigEntry<bool> SaveOnMapChange { get; } = new(
        _general, "SaveOnMapChange", false,
        "マップ移動時の自動セーブを有効にする。\nEnable autosaving when changing maps.");

    public BepInExModConfigEntry<bool> SaveOnSleep { get; } = new(
        _general, "SaveOnSleep", false,
        "就寝時の自動セーブを有効にする。\nEnable autosaving after sleeping.");

    public BepInExModConfigEntry<bool> SaveOnBossDefeat { get; } = new(
        _general, "SaveOnBossDefeat", false,
        "ボス撃破時の自動セーブを有効にする。\nEnable autosaving after defeating a boss.");

    public BepInExModConfigEntry<bool> SaveOnTreasureDiscovery { get; } = new(
        _general, "SaveOnTreasureDiscovery", false,
        "宝の地図で宝を発見した時の自動セーブを有効にする。\nEnable autosaving when discovering treasure using a treasure map.");

    public BepInExModConfigEntry<bool> SaveOnRelocation { get; } = new(
        _general, "SaveOnRelocation", false,
        "拠点の移設時の自動セーブを有効にする。\nEnable autosaving when relocating a base.");

    public BepInExModConfigEntry<bool> SaveOnEvent { get; } = new(
        _general, "SaveOnEvent", false,
        "イベント (会話など) による強制セーブを有効にする。\nEnable forced saves triggered by events, such as dialogue.");

    public BepInExModConfigEntry<bool> SaveOnReturnToTitle { get; } = new(
        _general, "SaveOnReturnToTitle", false,
        "タイトルへ戻る時の自動セーブを有効にする。\nEnable autosaving when returning to the title screen.");

    public BepInExModConfigEntry<bool> SaveOnQuit { get; } = new(
        _general, "SaveOnQuit", false,
        "ゲーム内メニューから終了する時の自動セーブを有効にする。\nEnable autosaving when exiting from the in-game menu.");
}
