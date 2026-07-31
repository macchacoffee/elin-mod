using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;
using MoreEffectiveLuck.Utils;

namespace MoreEffectiveLuck.Patches;

[HarmonyPatch(typeof(Thing))]
public static class ThingPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Thing.GetEnchant), [typeof(long), typeof(Func<SourceElement.Row, bool>), typeof(bool)])]
    private static IEnumerable<CodeInstruction> GetEnchant_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // int num6 = (item.mtp + EClass.rnd(item.mtp + (int)num5)) / item.mtp * ((!(flag && neg)) ? 1 : (-1));
        // // 変更後
        // var localValueMax = int.MinValue;
        // var localRollCount = LuckDice.RollCount();
        // var localRoll = 0;
        // while (localRoll < localRollCount)
        // {
        //     var localValue = (item.mtp + EClass.rnd(item.mtp + (int)num5)) / item.mtp * ((!(flag && neg)) ? 1 : (-1));
        //     localValueMax = Math.Max(localValueMax, localValue);
        //     localRoll += 1;
        // }
        // int num6 = localValueMax;
        var matcher = new CodeMatcher(instructions, generator);

        // エンチャント値計算における最大のロール回数、現在のロール回数を保持する変数、
        // 全ロールを通して最大のエンチャント値を保持するための変数を定義する
        var localRollCount = generator.DeclareLocal(typeof(int));
        var localRoll = generator.DeclareLocal(typeof(int));
        var localValue = generator.DeclareLocal(typeof(int));
        var localValueMax = generator.DeclareLocal(typeof(int));

        // ldloc.s 7 (SourceElement+Row)
        // ldfld int SourceElement+Row::mtp
        // ldloc.s 7 (SourceElement+Row)
        // ldfld int SourceElement+Row::mtp
        // ldloc.s 10 (System.Single)
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(SourceElement.Row), nameof(SourceElement.Row.mtp))),
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(SourceElement.Row), nameof(SourceElement.Row.mtp))),
            new CodeMatch(OpCodes.Ldloc_S)
        );
       // エンチャント値の計算を複数回ロールするループ制御を追加する
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldc_I4, int.MinValue),
            new CodeInstruction(OpCodes.Stloc_S, localValueMax),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(LuckDice), nameof(LuckDice.RollCount), [])),
            new CodeInstruction(OpCodes.Stloc_S, localRollCount),
            new CodeInstruction(OpCodes.Ldc_I4_0),
            new CodeInstruction(OpCodes.Stloc_S, localRoll),
            new CodeInstruction(OpCodes.Ldloc_S, localRoll),
            new CodeInstruction(OpCodes.Ldloc_S, localRollCount),
            new CodeInstruction(OpCodes.Bge)
        );
        matcher.CreateLabelWithOffsets(-3, out var labelMod1);
        var pos1 = matcher.Pos - 1;
        // mul NULL [Label15]
        // stloc.s 11 (System.Int32)
        // ldloc.s 7 (SourceElement+Row)
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Mul),
            new CodeMatch(OpCodes.Stloc_S),
            new CodeMatch(OpCodes.Ldloc_S)
        );
       matcher.Advance(-1);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Stloc_S, localValue),
            new CodeInstruction(OpCodes.Ldloc_S, localValueMax),
            new CodeInstruction(OpCodes.Ldloc_S, localValue),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Math), nameof(Math.Max), [typeof(int), typeof(int)])),
            new CodeInstruction(OpCodes.Stloc_S, localValueMax),
            new CodeInstruction(OpCodes.Ldloc_S, localRoll),
            new CodeInstruction(OpCodes.Ldc_I4_1),
            new CodeInstruction(OpCodes.Add),
            new CodeInstruction(OpCodes.Stloc_S, localRoll),
            new CodeInstruction(OpCodes.Br, labelMod1),
            new CodeInstruction(OpCodes.Ldloc_S, localValueMax)
        );
        matcher.CreateLabelWithOffsets(-1, out var labelMod2);
        matcher.Advance(pos1 - matcher.Pos);
        matcher.Operand = labelMod2;
 
        return matcher.InstructionEnumeration();
    }
}