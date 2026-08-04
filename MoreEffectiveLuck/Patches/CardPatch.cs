using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;
using MoreEffectiveLuck.Game;

namespace MoreEffectiveLuck.Patches;

[HarmonyPatch(typeof(Card))]
public static class CardPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyReversePatch(HarmonyReversePatchType.Original)]
    [HarmonyPatch(nameof(Card.Create), [typeof(string), typeof(int), typeof(int)])]
    private static Rarity RollRarityForCreate(Rarity rarity)
    {
        // Card.Create()から品質を決定するコードを抽出する。
        static IEnumerable<CodeInstruction> transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            // if (EClass.rnd(10) == 0)
            // {
            //     rarity = Rarity.Crude;
            // }
            // else if (EClass.rnd(10) == 0)
            // {
            //     rarity = Rarity.Superior;
            // }
            // else if (EClass.rnd(80) == 0)
            // {
            //     rarity = Rarity.Legendary;
            // }
            // else if (EClass.rnd(250) == 0)
            // {
            //     rarity = Rarity.Mythical;
            // }
            // return ratiry;
            var matcher = new CodeMatcher(instructions, generator);

            // ldc.i4.s 10 [Label9]
            // call static int EClass::rnd(int a)
            // brtrue Label11
            // ldarg.0 NULL
            // ldc.i4.m1 NULL
            // call void Card::set_rarity(Rarity value)
            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldc_I4_S),
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(EClass), nameof(EClass.rnd), [typeof(int)])),
                new CodeMatch(OpCodes.Brtrue),
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldc_I4_M1),
                new CodeMatch(OpCodes.Call, AccessTools.PropertySetter(typeof(Card), nameof(Card.rarity)))
            );
            var start = matcher.Pos;
            // ldc.i4 250 [Label15]
            // call static int EClass::rnd(int a)
            // brtrue Label17
            // ldarg.0 NULL
            // ldc.i4.3 NULL
            // call void Card::set_rarity(Rarity value)
            matcher.MatchEndForward(
                new CodeMatch(OpCodes.Ldc_I4),
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(EClass), nameof(EClass.rnd), [typeof(int)])),
                new CodeMatch(OpCodes.Brtrue),
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldc_I4_3),
                new CodeMatch(OpCodes.Call, AccessTools.PropertySetter(typeof(Card), nameof(Card.rarity)))
            );
            var end = matcher.Pos;
            // 末尾にreturnを追加する
            matcher = new CodeMatcher([
                ..matcher.InstructionsInRange(start, end),
                new CodeInstruction(OpCodes.Ret)
            ], generator);

            // 決定した品質が関数の戻り値になるように調整する
            // ldc.i4.s 10
            matcher.MatchStartForward(new CodeMatch(OpCodes.Ldc_I4_S));
            matcher.Labels.Clear();
            // ldarg.0 NULL
            matcher.Start();
            matcher.MatchStartForward(new CodeMatch(OpCodes.Ldarg_0));
            matcher.Repeat(matchAction: m =>
            {
                m.RemoveInstruction();
            });
            // call void Card::set_rarity(Rarity value)
            matcher.Start();
            matcher.MatchStartForward(new CodeMatch(OpCodes.Call, AccessTools.PropertySetter(typeof(Card), nameof(Card.rarity))));
            matcher.Repeat(matchAction: m =>
            {
                m.RemoveInstruction();
            });
            // br
            var labelList1 = new List<Label>();
            matcher.Start();
            matcher.MatchStartForward(new CodeMatch(OpCodes.Br));
            matcher.Repeat(matchAction: m =>
            {
                labelList1.Add((Label)m.Operand);
                matcher.Advance(1);
            });
            // brtrue Label17
            // ldc.i4.3 NULL
            matcher.Start();
            matcher.MatchEndForward(
                new CodeMatch(OpCodes.Brtrue),
                new CodeMatch(OpCodes.Ldc_I4_3)
            );
            var brtruePos1 = matcher.Pos - 1;
            matcher.Advance(1);
            matcher.InsertBranchAndAdvance(OpCodes.Br, matcher.Pos);
            matcher.Insert(
                new CodeInstruction(OpCodes.Ldarg_0)
            );
            matcher.CreateLabel(out var labelMod1);
            matcher.Advance(brtruePos1 - matcher.Pos);
            matcher.Operand = labelMod1;
            matcher.End();
            matcher.AddLabels(labelList1);

            return matcher.InstructionEnumeration();
        }

        _ = transpiler(null!, null!);
        return default!;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Card.Create), [typeof(string), typeof(int), typeof(int)])]
    private static IEnumerable<CodeInstruction> Create_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // if (EClass.rnd(10) == 0)
        // {
        //     rarity = Rarity.Crude;
        // }
        // else if (EClass.rnd(10) == 0)
        // {
        //     rarity = Rarity.Superior;
        // }
        // else if (EClass.rnd(80) == 0)
        // {
        //     rarity = Rarity.Legendary;
        // }
        // else if (EClass.rnd(250) == 0)
        // {
        //     rarity = Rarity.Mythical;
        // }
        // // 変更後
        // rarity = CardPatch.CalculateNum6ForGetEnchant(item, num5, flag, neg);
        var matcher = new CodeMatcher(instructions, generator);

        // 品質をダイスロールして決定する処理に差し替える
        // ldc.i4.s 10 [Label9]
        // call static int EClass::rnd(int a)
        // brtrue Label11
        // ldarg.0 NULL
        // ldc.i4.m1 NULL
        // call void Card::set_rarity(Rarity value)
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldc_I4_S),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(EClass), nameof(EClass.rnd), [typeof(int)])),
            new CodeMatch(OpCodes.Brtrue),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldc_I4_M1),
            new CodeMatch(OpCodes.Call, AccessTools.PropertySetter(typeof(Card), nameof(Card.rarity)))
        );
        var start = matcher.Pos;
        var labelList1 = matcher.Labels.Copy();
        matcher.Labels.Clear();
        // ldc.i4 250 [Label15]
        // call static int EClass::rnd(int a)
        // brtrue Label17
        // ldarg.0 NULL
        // ldc.i4.3 NULL
        // call void Card::set_rarity(Rarity value)
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldc_I4),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(EClass), nameof(EClass.rnd), [typeof(int)])),
            new CodeMatch(OpCodes.Brtrue),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldc_I4_3),
            new CodeMatch(OpCodes.Call, AccessTools.PropertySetter(typeof(Card), nameof(Card.rarity)))
        );
        var end = matcher.Pos;
        matcher.Advance(start - matcher.Pos);
        matcher.RemoveInstructionsInRange(start, end);
        matcher.Insert(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(Card), nameof(Card.rarity))),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CardPatch), nameof(CalculateRarityForCreate), [typeof(Rarity)])),
            new CodeInstruction(OpCodes.Call, AccessTools.PropertySetter(typeof(Card), nameof(Card.rarity)))
        );
        matcher.AddLabels(labelList1);

        return matcher.InstructionEnumeration();
    }

    private static Rarity CalculateRarityForCreate(Rarity rarity)
    {
        Rarity resultFunc() => RollRarityForCreate(rarity);
        if (!Mod.Config.EnableEquipmentRarity.Value)
        {
            return resultFunc();
        }
        var dice = LuckDice<Rarity>.Create(
            resultFunc: resultFunc,
            resultCompareFunc: (result, prev) => result > prev,
            card: EClass.pc,
            luckPerRoll: Mod.Config.EquipmentRarityLuckPerRoll.Value,
            maxRoll: Mod.Config.EquipmentRarityMaxRoll.Value
        );
        return dice.Roll().Value;
    }
}
