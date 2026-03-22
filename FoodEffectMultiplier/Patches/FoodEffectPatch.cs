using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;

namespace FoodEffectMultiplier.Patches;


[HarmonyPatch(typeof(FoodEffect))]
public static class FoodEffectPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyPatch(nameof(FoodEffect.Proc), [typeof(Chara), typeof(Thing), typeof(bool)]), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Proc_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // if (!c2.IsPC)
        // {
        //    num2 *= 3f;
        // }
        // // 変更後
        // if (!c2.IsPC)
        // {
        //    num2 *= FoodEffectPatch.GetNPCFoodEffectMultiplier(3f);
        // }
        // else
        // {
        //    num2 *= FoodEffectPatch.GetPCFoodEffectMultiplier(1f);
        // }
        var matcher = new CodeMatcher(instructions, generator);

        //  brtrue Label34
        //  ldloc.3 NULL
        //  ldc.r4 3
        //  mul NULL
        //  stloc.3 NULL
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Brtrue),
            new CodeMatch(OpCodes.Ldloc_3),
            new CodeMatch(OpCodes.Ldc_R4),
            new CodeMatch(OpCodes.Mul),
            new CodeMatch(OpCodes.Stloc_3)
        );
        // brtrue Label34 からc2がNPCの場合の遷移先であるLabel34を取得する
        var start = matcher.Pos;
        var originalLabel = matcher.Instruction.operand;
        // mul NULLの直前に ldc.r4 3 で生成した定数3fを引数とするGetNPCFoodEffectMultiplierの呼び出しを追加し、
        // その戻り値がNPCの食事効果倍率に適用されるようにする
        matcher.Advance(3);
        matcher.InsertAndAdvance(
            CodeInstruction.Call(() => GetNPCFoodEffectMultiplier(default))
        );
        // c2がNPCの場合にPCの食事効果倍率が適用されないようにするため、
        // 無条件でLabel34に遷移する br Label34 を追加する
        // またPCの食事効果倍率の計算に必要となる、倍率適用前の食事効果値をローカル変数から読み込む
        matcher.Advance(2);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Br, originalLabel),
            new CodeInstruction(OpCodes.Ldloc_3)
        );
        // c2がPCの場合の遷移先となるLabelMod1を生成する
        matcher.CreateLabelWithOffsets(-1, out var label1);
        // c2がPCの場合における食事効果倍率の適用ロジックを追加する
        // バニラではPCの食事効果に倍率は適用されないため、GetPCFoodEffectMultiplier呼び出しの引数は1fとする
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldc_R4, 1f),
            CodeInstruction.Call(() => GetPCFoodEffectMultiplier(default)),
            new CodeInstruction(OpCodes.Mul),
            new CodeInstruction(OpCodes.Stloc_3)
        );
        // c2がPCの場合にtrueとなる brtrue Label34 を brtrue LabelMod1 に置き換え、
        // PCの食事効果に倍率が適用されるようにする
        matcher.Advance(start - matcher.Pos);
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Brtrue, label1)
        );

        return matcher.InstructionEnumeration();
    }

    private static float GetPCFoodEffectMultiplier(float defaultValue)
    {
        return Mod.Config.PCMultiplier ?? defaultValue;
    }

    private static float GetNPCFoodEffectMultiplier(float defaultValue)
    {
        return Mod.Config.NPCMultiplier ?? defaultValue;
    }
}
