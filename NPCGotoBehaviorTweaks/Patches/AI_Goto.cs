using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;

namespace NPCGotoBehaviorTweaks.Patches;

[HarmonyPatch(typeof(AI_Goto))]
public static class AI_GotoPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(AI_Goto.TryGoTo), [])]
    internal static IEnumerable<CodeInstruction> TryGoTo_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // if (waitCount < 3 || EClass.rnd(5) != 0)
        // {
        //     return Status.Running;
        // }
        // // 変更後
        // if (waitCount < 1 || EClass.rnd(2) != 0)
        // {
        //     return Status.Running;
        // }
        var matcher = new CodeMatcher(instructions, generator);

        // ldfld int AI_Goto::waitCount
        // ldc.i4.3 NULL
        // blt Label29
        // ldc.i4.5 NULL
        // call static int EClass::rnd(int a)
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(AI_Goto), nameof(AI_Goto.waitCount))),
            new CodeMatch(OpCodes.Ldc_I4_3),
            new CodeMatch(OpCodes.Blt),
            new CodeMatch(OpCodes.Ldc_I4_5),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(EClass), nameof(EClass.rnd), [typeof(int)]))
        );
        // NPCの移動先に別のキャラクターが存在する場合に発生する
        // 2回の最小待機回数を無効にし、その待機後にさらに待機を続ける確率を4/5から1/2に変更する
        matcher.Advance(1);
        matcher.Opcode = OpCodes.Ldc_I4_1;
        matcher.Advance(2);
        matcher.Opcode = OpCodes.Ldc_I4_2;

        return matcher.InstructionEnumeration();
    }
}
