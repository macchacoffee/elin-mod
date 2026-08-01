using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;
using MoreEffectiveLuck.Game;
using NPOI.SS.Formula.Functions;

namespace MoreEffectiveLuck.Patches;


[HarmonyPatch(typeof(FoodEffect))]
public static class FoodEffectPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(FoodEffect.Proc), [typeof(Chara), typeof(Thing), typeof(bool)])]
    private static IEnumerable<CodeInstruction> Proc_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        //
        // // 変更後
        //
        var matcher = new CodeMatcher(instructions, generator);

        matcher.End();
        // callvirt void Chara::Vomit()
        matcher.MatchStartBackwards(
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Chara), nameof(Chara.Vomit), []))
        );
        // lpop NULL
        // ldloc.0 NULL [Label82, Label83]
        // ldfld Chara FoodEffect+<>c__DisplayClass1_0::c
        matcher.MatchStartBackwards(
            new CodeMatch(OpCodes.Pop),
            new CodeMatch(OpCodes.Ldloc_0),
            new CodeMatch(OpCodes.Ldfld)
        );
        matcher.Advance(1);
        var labelList1 = matcher.Labels.Copy();
        matcher.Labels.Clear();
        matcher.Advance(1);
        var charaOperand = matcher.Operand;
        matcher.Advance(-1);
        matcher.Insert(
            new CodeInstruction(OpCodes.Ldloc_0),
            new CodeInstruction(OpCodes.Ldfld, charaOperand),
            new CodeInstruction(OpCodes.Ldarg_1),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FoodEffectPatch), nameof(ProcLuckyFood), [typeof(Chara), typeof(Thing)]))
        );
        matcher.AddLabels(labelList1);

        return matcher.InstructionEnumeration();
    }

    private readonly static Dictionary<string, Func<Chara, Thing, int>> LuckyFoodPower = new() {
        ["kagamimochi"] = (_, _) => 30,
        ["churyu"]  = (_, _) => 22,
        ["wedding_cake1"] = (_, _) => 20,
        ["bushdenoel"] = (_, _) => 10,
        ["crimale2"] = (_, _) => 11,
        ["65_gold"] = (_, _) => 177,    // 金のコイ
        ["86"] = (_, _) => 11,          // マダイ
        ["71"] = (_, _) => 11,          // シロアマダイ
        ["_poop"] = (_, f) =>{
            return f.material.alias switch
            {
                "gold" => 77,
                "silver" => 7,
                _ => 0
            };
        }
    };

    private static void ProcLuckyFood(Chara chara, Thing food)
    {
        if (!LuckyFoodPower.TryGetValue(food.source.id, out var getPower))
        {
            return;
        }
        var power = getPower(chara, food);
        if (power > 0)
        {
            chara.AddCondition<ConMCMELFortunate>(power);
        }
    }
}
