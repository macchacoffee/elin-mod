using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;
using MoreEffectiveLuck.Mod;

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
    [HarmonyPatch(nameof(Thing.WriteNote), [typeof(UINote), typeof(Action<UINote>), typeof(IInspect.NoteMode), typeof(Recipe)])]
    private static IEnumerable<CodeInstruction> WriteNote_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // if (FoodEffect.IsLeftoverable(this))
        // {
        //     AddText("isLeftoverable", FontColor.Default);
        // }
        // // 変更後
        // if (FoodEffect.IsLeftoverable(this))
        // {
        //     AddText("isLeftoverable", FontColor.Default);
        // }
        // if (ThingPatch.IsLuckyFood(this))
        // {
        //     AddText(ModConsts.SourceId.IsLuckyFood, FontColor.Default);
        // }
        var matcher = new CodeMatcher(instructions, generator);

        // brfalse Label174
        // ldloc.0 NULL
        // ldstr "isLeftoverable"
        // ldc.i4.1 NULL
        // callvirt void Thing+<>c__DisplayClass36_0::<WriteNote>g__AddText|1(string text, FontColor col)
        // ldarg.0 NULL [Label174]
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Brfalse),
            new CodeMatch(OpCodes.Ldloc_0),
            new CodeMatch(OpCodes.Ldstr, "isLeftoverable"),
            new CodeMatch(OpCodes.Ldc_I4_1),
            new CodeMatch(OpCodes.Callvirt),
            new CodeMatch(OpCodes.Ldarg_0)
        );
        matcher.Advance(-1);
        var addTextOperand = matcher.Operand;
        matcher.Advance(1);
        var labelList1 = matcher.Labels.Copy();
        matcher.Labels.Clear();
        matcher.Insert(
            new CodeInstruction(OpCodes.Ldarg_0)
        );
        matcher.AddLabels(labelList1);
        matcher.Advance(1);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ThingPatch), nameof(IsLuckyFood), [typeof(Thing)]))
        );
        matcher.InsertBranchAndAdvance(OpCodes.Brfalse, matcher.Pos);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_0),
            new CodeInstruction(OpCodes.Ldstr, ModConsts.SourceId.IsLuckyFood),
            new CodeInstruction(OpCodes.Ldc_I4_1),
            new CodeInstruction(OpCodes.Callvirt, addTextOperand)
        );

        return matcher.InstructionEnumeration();
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
            // 末尾にreturnを追加する
            matcher = new CodeMatcher([
                ..matcher.InstructionsInRange(start, end),
                new CodeInstruction(OpCodes.Ret)
            ], generator);

            // エンチャント強度の計算でこの関数の引数を参照するように調整する
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

            return matcher.InstructionEnumeration();
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
        // int num6 = ThingPatch.CalculateNum6ForGetEnchant(item, num5, flag, neg);
        var matcher = new CodeMatcher(instructions, generator);

        // エンチャント強度をダイスロールして決定する処理に差し替える
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

    private static bool IsLuckyFood(Thing thing)
    {
        return LuckyFood.IsLuckyFood(thing);
    }

    private static int CalculateNum6ForGetEnchant(SourceElement.Row row, float num5, bool flag, bool neg)
    {
        int resultFunc() => RollNum6ForGetEnchant(row, num5, flag, neg);
        if (!ModContext.Config.EnableEnchantPower.Value)
        {
            return resultFunc();
        }
        var dice = LuckDice<int>.Create(
            resultFunc: resultFunc,
            resultCompareFunc: (result, prev) => result > prev,
            card: EClass.pc,
            luckPerRoll: ModContext.Config.EnchantPowerLuckPerRoll.Value,
            maxRoll: ModContext.Config.EnchantPowerMaxRoll.Value
        );
        return dice.Roll().Value;
    }
}
