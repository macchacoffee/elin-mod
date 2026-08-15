using ModUtility.Config;
using UnityEngine;

namespace AbilityRestriction.Config;

internal class ModConfig : BepInExModConfigBase<ModConfig>
{
    private const string _general = "General";
    private const string _cheats  = "Cheats";

    public BepInExModConfigEntry<bool> EnableViaResidentBoard { get; } = new(
        _general, "EnableViaResidentBoard", true,
        "住民掲示板からのアビリティ制限を有効にする。\nEnable ability restrictions via the resident board.");

    public BepInExModConfigEntry<bool> EnableViaConversation { get; } = new(
        _general, "EnableViaConversation", false,
        "会話からのアビリティ制限を有効にする。\nEnable ability restrictions via conversation.");

    public BepInExModConfigEntry<bool> EnableViaInteraction { get; } = new(
        _general, "EnableViaInteraction", false,
        "インタラクションからのアビリティ制限を有効にする。\nEnable ability restrictions via interaction.");

    public BepInExModConfigEntry<bool> EnableForAllNPC { get; } = new(
        _cheats, "EnableForAllNPC", false,
        // "すべてのNPCに対してアビリティ制限を有効にする。\n*注意* ゲームバランスが壊れたり予期しない事象を引き起こしたりする可能性があります。\nEnable ability restrictions for all NPC.\n*Warning* This may disrupt game balance or cause unexpected behavior.");
        $"すべてのNPCに対してアビリティ制限を有効にする。\n{"*注意*".TagColor(Color.red)} ゲームバランスが壊れたり予期しない事象を引き起こしたりする可能性があります。\nEnable ability restrictions for all NPC.\n{"*Warning*".TagColor(Color.red)} This may disrupt game balance or cause unexpected behavior.");
}
