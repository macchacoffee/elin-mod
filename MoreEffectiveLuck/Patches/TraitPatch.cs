using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;
using MoreEffectiveLuck.Game;

namespace MoreEffectiveLuck.Patches;

[HarmonyPatch(typeof(Trait))]
public static class TraitPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyReversePatch(HarmonyReversePatchType.Original)]
    [HarmonyPatch(nameof(Trait.CreateStock), [])]
    private static Rarity RollRarityForCreateStock(int num2)
    {
        // Trait.CreateStock()から品質を決定するコードを抽出する。
        static IEnumerable<CodeInstruction> transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            // return (EClass.rnd(num2 * 5) == 0) ? Rarity.Mythical : ((EClass.rnd(num2) == 0) ? Rarity.Legendary : ((EClass.rnd(5) == 0) ? Rarity.Superior : Rarity.Normal))
            var matcher = new CodeMatcher(instructions, generator);

            // ldloc.0 NULL [Label85]
            // ldc.i4.5 NULL
            // mul NULL
            // call static int EClass::rnd(int a)
            // brfalse Label86
            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldloc_0),
                new CodeMatch(OpCodes.Ldc_I4_5),
                new CodeMatch(OpCodes.Mul),
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(EClass), nameof(EClass.rnd), [typeof(int)])),
                new CodeMatch(OpCodes.Brfalse)
            );
            var start = matcher.Pos;
            // br Label91
            // ldc.i4.3 NULL [Label86]
            matcher.MatchEndForward(
                new CodeMatch(OpCodes.Br),
                new CodeMatch(OpCodes.Ldc_I4_3)
            );
            var end = matcher.Pos;
            matcher.Advance(1);
            var labelList1 = matcher.Labels.Copy();
            matcher.Labels.Clear();
            // 末尾にreturnを追加する
            matcher = new CodeMatcher([
                ..matcher.InstructionsInRange(start, end),
                new CodeInstruction(OpCodes.Ret)
            ], generator);

            // 決定した品質が関数の戻り値になるように調整する
            // ldloc.0 NULL [Label85]
            matcher.MatchStartForward(new CodeMatch(OpCodes.Ldloc_0));
            matcher.Labels.Clear();
            // ldloc.0 NULL
            matcher.Start();
            matcher.MatchStartForward(new CodeMatch(OpCodes.Ldloc_0));
            matcher.Repeat(matchAction: m =>
            {
                m.RemoveInstruction();
                m.InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0));
            });
            // ret Mull
            matcher.Start();
            matcher.MatchEndForward(
               new CodeMatch(OpCodes.Ret)
            );
            matcher.AddLabels(labelList1);

            return matcher.InstructionEnumeration();
        }

        _ = transpiler(null!, null!);
        return default!;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Trait.CreateStock), [])]
    private static IEnumerable<CodeInstruction> CreateStock_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // CardBlueprint.SetRarity((EClass.rnd(num2 * 5) == 0) ? Rarity.Mythical : ((EClass.rnd(num2) == 0) ? Rarity.Legendary : ((EClass.rnd(5) == 0) ? Rarity.Superior : Rarity.Normal)));
        // // 変更後
        // CardBlueprint.SetRarity(TraitPatch.CalculateRarityForCreateStock(num2));
        var matcher = new CodeMatcher(instructions, generator);

        // 品質をダイスロールして決定する処理に差し替える
        // ldloc.0 NULL [Label85]
        // ldc.i4.5 NULL
        // mul NULL
        // call static int EClass::rnd(int a)
        // brfalse Label86
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldloc_0),
            new CodeMatch(OpCodes.Ldc_I4_5),
            new CodeMatch(OpCodes.Mul),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(EClass), nameof(EClass.rnd), [typeof(int)])),
            new CodeMatch(OpCodes.Brfalse)
        );
        var start = matcher.Pos;
        var labelList1 = matcher.Labels.Copy();
        matcher.Labels.Clear();
        // br Label91
        // ldc.i4.3 NULL [Label86]
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Br),
            new CodeMatch(OpCodes.Ldc_I4_3)
        );
        var end = matcher.Pos;
        matcher.Advance(1);
        matcher.Labels.Clear();
        matcher.Advance(start - matcher.Pos);
        matcher.RemoveInstructionsInRange(start, end);
        matcher.Insert(
            new CodeInstruction(OpCodes.Ldloc_0),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TraitPatch), nameof(CalculateRarityForCreateStock), [typeof(int)]))
        );
        matcher.AddLabels(labelList1);

        return matcher.InstructionEnumeration();
    }

    private static Rarity CalculateRarityForCreateStock(int num2)
    {
        Rarity resultFunc() => RollRarityForCreateStock(num2);
        if (!Mod.Config.EnableSpecialMerchantRarity.Value)
        {
            return resultFunc();
        }
        var dice = LuckDice<Rarity>.Create(
            resultFunc: resultFunc,
            resultCompareFunc: (result, prev) => result > prev,
            card: EClass.pc,
            luckPerRoll: Mod.Config.SpecialMerchantRarityLuckPerRoll.Value,
            maxRoll: Mod.Config.SpecialMerchantRarityMaxRoll.Value
        );
        return dice.Roll().Value;
    }
}
