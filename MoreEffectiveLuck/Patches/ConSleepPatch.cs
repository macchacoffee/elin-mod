using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;
using MoreEffectiveLuck.Mod;

namespace MoreEffectiveLuck.Patches;


[HarmonyPatch(typeof(ConSleep))]
internal static class ConSleepPatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(ConSleep.OnRemoved), [])]
    private static IEnumerable<CodeInstruction> OnSimulateDay_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // EClass.player.DreamSpell();
        // // 変更後
        // EClass.player.DreamSpell();
        // ConSleepPatch.ApplyLuckyDayCondition();
        var matcher = new CodeMatcher(instructions, generator);

        // callvirt void Player::DreamSpell()
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Player), nameof(Player.DreamSpell), []))
        );
        matcher.Advance(1);
        matcher.InsertAndAdvance(
            CodeInstruction.Call(() => ApplyLuckyDayCondition())
        );

        return matcher.InstructionEnumeration();
    }

    private static void ApplyLuckyDayCondition()
    {
        if (!ModContext.Config.EnableLuckyDay.Value)
        {
            return;
        }
        var chara = EClass.pc;
        if (EClass.rnd(chara.faith == EClass.game.religions.Luck ? 200 : 400) == 0)
        {
            Msg.Say("umi");
            SE.Play("aura_heaven");
            chara.PlayEffect("aura_heaven");
            chara.AddCondition<ConMCMELLuckyDay>(777);
        }
    }
}
