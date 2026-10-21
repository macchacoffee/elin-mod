using System.Collections.Generic;
using System.Reflection;

using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.AdditionalUniqueNPC.Patches;

[HarmonyPatch(typeof(DramaManager))]
internal static class DramaManagerPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(DramaManager.ParseLine), [typeof(Dictionary<string, string>)])]
    private static void ParseLine_Postfix(
        DramaManager __instance,
        Dictionary<string, string> item)
    {
        if (__instance.setup?.book == "fiama")
        {
            // ステップ start_pet における最後の選択肢
            if (item.TryGetValue("id", out var id) && id == "9" &&
                item.TryGetValue("action", out var action) && action == "choice" &&
                item.TryGetValue("jump", out var jump) && jump == "pet4")
            {
                FiamaPetExtra(__instance);
                return;
            }
        }

    }

    private static void FiamaPetExtra(DramaManager dm)
    {
        var talk = dm.lastTalk;
        if (talk == null)
        {
            return;
        }

        var layer = dm.layer;
        var sequence = dm.sequence;

        // 一度でもPCの仲間になったことがある場合、選択肢を追加しない。
        if (EClass.player.dialogFlags.TryGetValue(
            ModConsts.DramaFlag.MisalieEverJoined, out var everJoined) && everJoined >= 1)
        {
            return;
        }

        var target = EClass.game.cards.globalCharas.Find(ModConsts.SourceId.Misalie);
        if (target == null || target.IsPCFaction)
        {
            return;
        }

        talk.AddChoice(new DramaChoice(ModConsts.SourceId.DcFiamaPetExtra.lang(), null)
        {
            onJump = () =>
            {
                layer.SetOnKill(() =>
                {
                    LayerDrama.Activate(ModConsts.SourceId.Misalie, null, ModConsts.DramaStep.FiamaPetExtra1, target);
                });

                sequence.Exit();
            }
        });
    }
}
