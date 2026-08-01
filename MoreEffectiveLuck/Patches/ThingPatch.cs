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

    [HarmonyReversePatch(HarmonyReversePatchType.Original)]
    [HarmonyPatch(nameof(Thing.GetEnchant), [typeof(long), typeof(Func<SourceElement.Row, bool>), typeof(bool)])]
    private static int RollNum6ForGetEnchant(SourceElement.Row row, float num5, bool flag, bool neg)
    {
        // Thing.GetEnchant()からエンチャントの強度を計算するコードを抽出する。
        static IEnumerable<CodeInstruction> transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            // (item.mtp + EClass.rnd(item.mtp + (int)num5)) / item.mtp * ((!(flag && neg)) ? 1 : (-1));
            var matcher = new CodeMatcher(instructions, generator);

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
            var start = matcher.Pos;

            // mul NULL [Label15]
            // stloc.s 11 (System.Int32)
            // ldloc.s 7 (SourceElement+Row)
            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Mul),
                new CodeMatch(OpCodes.Stloc_S),
                new CodeMatch(OpCodes.Ldloc_S)
            );
            var end = matcher.Pos;

            matcher = new CodeMatcher(matcher.InstructionsInRange(start, end), generator);

            // エンチャント強度の計算でこの関数の引数を参照するようにする
            // ldarg.2 NULL
            matcher.MatchStartForward(new CodeMatch(OpCodes.Ldarg_2));
            matcher.Repeat(matchAction: m =>
            {
                m.RemoveInstruction();
                m.InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_S, 3));
            });
            // ldloc.s
            matcher.Start();
            matcher.MatchStartForward(new CodeMatch(OpCodes.Ldloc_S));
            matcher.Repeat(matchAction: m =>
            {
                var localIndex = (m.Operand as LocalBuilder)!.LocalIndex;
                m.RemoveInstruction();
                switch (localIndex)
                {
                    case 7:
                        m.InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_S, 0));
                        break;
                    case 9:
                        m.InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_S, 2));
                        break;
                    case 10:
                        m.InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_S, 1));
                        break;
                }
            });

            // returnを追加する
            return [
                ..matcher.InstructionEnumeration(),
                new CodeInstruction(OpCodes.Ret)
            ];
        }

        _ = transpiler(null!, null!);
        return default!;
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
        var start = matcher.Pos;
        // mul NULL [Label15]
        // stloc.s 11 (System.Int32)
        // ldloc.s 7 (SourceElement+Row)
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Mul),
            new CodeMatch(OpCodes.Stloc_S),
            new CodeMatch(OpCodes.Ldloc_S)
        );
        var end = matcher.Pos;
        // エンチャント強度を複数回ロールする処理に差し替える
        matcher.Advance(start - matcher.Pos);
        matcher.RemoveInstructionsInRange(start, end);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_S, 7),
            new CodeInstruction(OpCodes.Ldloc_S, 10),
            new CodeInstruction(OpCodes.Ldloc_S, 9),
            new CodeInstruction(OpCodes.Ldarg_2),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ThingPatch), nameof(CalculateNum6ForGetEnchant), [typeof(SourceElement.Row), typeof(float), typeof(bool), typeof(bool)]))
        );

        return matcher.InstructionEnumeration();
    }

    private static int CalculateNum6ForGetEnchant(SourceElement.Row row, float num5, bool flag, bool neg)
    {
        var dice = LuckDice<int?>.Create(
            resultFunc: () => RollNum6ForGetEnchant(row, num5, flag, neg),
            resultCompareFunc: (result, prev) => result > prev,
            card: EClass.pc
        );
        return dice.Roll().Value;
    }
}