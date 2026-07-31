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
        // var localDice = new();
        // var localRoll = 0;
        // while (localRoll < localDice.RollCount)
        // {
        //     localDice.UpdateResult((item.mtp + EClass.rnd(item.mtp + (int)num5)) / item.mtp * ((!(flag && neg)) ? 1 : (-1)));
        //     localRoll += 1;
        // }
        // int num6 = localDice.Result;
        var matcher = new CodeMatcher(instructions, generator);

        // エンチャント値計算のダイスと現在のロール回数を保持する変数を定義する
        var localDice = generator.DeclareLocal(typeof(LuckDice));
        var localRoll = generator.DeclareLocal(typeof(int));

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
            new CodeInstruction(OpCodes.Newobj, AccessTools.Constructor(typeof(LuckDice), [])),
            new CodeInstruction(OpCodes.Stloc_S, localDice),
            new CodeInstruction(OpCodes.Ldc_I4_0),
            new CodeInstruction(OpCodes.Stloc_S, localRoll),
            new CodeInstruction(OpCodes.Ldloc_S, localRoll),
            new CodeInstruction(OpCodes.Ldloc_S, localDice),
            new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(LuckDice), nameof(LuckDice.RollCount))),
            new CodeInstruction(OpCodes.Bge),
            new CodeInstruction(OpCodes.Ldloc_S, localDice)
        );
        matcher.CreateLabelWithOffsets(-5, out var labelMod1);
        var pos1 = matcher.Pos - 2;
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
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(LuckDice), nameof(LuckDice.UpdateResult), [typeof(int)])),
            new CodeInstruction(OpCodes.Ldloc_S, localRoll),
            new CodeInstruction(OpCodes.Ldc_I4_1),
            new CodeInstruction(OpCodes.Add),
            new CodeInstruction(OpCodes.Stloc_S, localRoll),
            new CodeInstruction(OpCodes.Br, labelMod1),
            new CodeInstruction(OpCodes.Ldloc_S, localDice),
            new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(LuckDice), nameof(LuckDice.Result)))
        );
        matcher.CreateLabelWithOffsets(-2, out var labelMod2);
        matcher.Advance(pos1 - matcher.Pos);
        matcher.Operand = labelMod2;
 
        return matcher.InstructionEnumeration();
    }
}