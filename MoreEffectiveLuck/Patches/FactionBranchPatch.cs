using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;
using MoreEffectiveLuck.Game;

namespace MoreEffectiveLuck.Patches;


[HarmonyPatch(typeof(FactionBranch))]
public static class FactionBranchPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(FactionBranch.OnSimulateDay), [typeof(VirtualDate)])]
    private static IEnumerable<CodeInstruction> OnSimulateDay_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // 
        // // 変更後
        // 
        var matcher = new CodeMatcher(instructions, generator);

        // ---
        // brfalse Label23
        // ldarg.0 NULL
        // ldstr "lucky_month"
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Brfalse),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldstr, "lucky_month")
        );
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Pop)
        );
        // ---

        // ldarg.0 NULL
        // ldc.i4.1 NULL
        // stfld bool FactionBranch::luckyMonthDone
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldc_I4_1),
            new CodeMatch(OpCodes.Stfld, AccessTools.Field(typeof(FactionBranch), nameof(FactionBranch.luckyMonthDone)))
        );
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FactionBranchPatch), nameof(ApplyLuckyMonthCondition)))
        );

        return matcher.InstructionEnumeration();
    }

    private static void ApplyLuckyMonthCondition()
    {
        EClass.pc.AddCondition<ConMCMELLuckyMonth>(222);
    }
}
