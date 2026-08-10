using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;
using MoreEffectiveLuck.Mod;

namespace MoreEffectiveLuck.Patches;


[HarmonyPatch(typeof(FactionBranch))]
public static class FactionBranchPatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(FactionBranch.OnSimulateDay), [typeof(VirtualDate)])]
    private static IEnumerable<CodeInstruction> OnSimulateDay_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // if (luckyMonth)
        // {
        //     Log("lucky_month", EClass._zone.Name);
        //     Msg.Say("lucky_month", EClass._zone.Name);
        //     Msg.Say("umi");
        //     SE.Play("godbless");
        //     EClass.world.SendPackage(ThingGen.Create("book_kumiromi"));
        //     luckyMonthDone = true;
        // }
        // // 変更後
        // if (luckyMonth)
        // {
        //     Log("lucky_month", EClass._zone.Name);
        //     Msg.Say("lucky_month", EClass._zone.Name);
        //     Msg.Say("umi");
        //     SE.Play("godbless");
        //     EClass.world.SendPackage(ThingGen.Create("book_kumiromi"));
        //     FactionBranchPatch.ApplyLuckyMonthCondition();
        //     luckyMonthDone = true;
        // }
        var matcher = new CodeMatcher(instructions, generator);

        // ldarg.0 NULL
        // ldc.i4.1 NULL
        // stfld bool FactionBranch::luckyMonthDone
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldc_I4_1),
            new CodeMatch(OpCodes.Stfld, AccessTools.Field(typeof(FactionBranch), nameof(FactionBranch.luckyMonthDone)))
        );
        matcher.InsertAndAdvance(
            CodeInstruction.Call(() => ApplyLuckyMonthCondition())
        );

        return matcher.InstructionEnumeration();
    }

    private static void ApplyLuckyMonthCondition()
    {
        if (!ModContext.Config.EnableLuckyMonth.Value)
        {
            return;
        }
        var chara = EClass.pc;
        chara.PlayEffect("aura_heaven");
        EClass.pc.AddCondition<ConMCMELLuckyMonth>(222);
    }
}
