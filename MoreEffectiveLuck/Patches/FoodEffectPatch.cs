using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;
using MoreEffectiveLuck.Mod;

namespace MoreEffectiveLuck.Patches;


[HarmonyPatch(typeof(FoodEffect))]
internal static class FoodEffectPatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(FoodEffect.Proc), [typeof(Chara), typeof(Thing), typeof(bool)])]
    private static IEnumerable<CodeInstruction> Proc_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // if (!c.isDead)
        // {
        //     food.trait.OnDrink(c);
        // }
        // // 変更後
        // if (!c.isDead)
        // {
        //     FoodEffectPatch.ProcLuckyFood(c, food);
        //     food.trait.OnDrink(c);
        // }
        var matcher = new CodeMatcher(instructions, generator);

        // めでたい食べ物を食べた時に幸運バフが付ける処理を追加する
        matcher.End();
        // callvirt void Chara::Vomit()
        matcher.MatchStartBackwards(
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Chara), nameof(Chara.Vomit), []))
        );
        // pop NULL
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
            CodeInstruction.Call(() => ProcLuckyFood(default!, default!))
        );
        matcher.AddLabels(labelList1);

        return matcher.InstructionEnumeration();
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(FoodEffect.ProcDrink), [typeof(Chara), typeof(Thing)])]
    private static IEnumerable<CodeInstruction> ProcDrink_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // if (c.HasCondition<ConAnorexia>())
        // {
        //     c.Vomit();
        // }
        // // 変更後
        // FoodEffectPatch.ProcLuckyFood(c, food);
        // if (c.HasCondition<ConAnorexia>())
        // {
        //     c.Vomit();
        // }
        var matcher = new CodeMatcher(instructions, generator);

        // めでたい飲み物を飲んだ時に幸運バフが付ける処理を追加する
        // ldarg.1 NULL
        // ldfld Trait Card::trait
        // ldarg.0 NULL
        // callvirt virtual void Trait::OnDrink(Chara c)
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_1),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Card), nameof(Card.trait))),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Trait), nameof(Trait.OnDrink), [typeof(Chara)]))
        );
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldarg_1),
            CodeInstruction.Call(() => ProcLuckyFood(default!, default!))
        );

        return matcher.InstructionEnumeration();
    }

    private static void ProcLuckyFood(Chara chara, Thing food)
    {
        if (!ModContext.Config.EnableLuckyFood.Value)
        {
            return;
        }
        ModLuckyFood.ProcFoodEffect(chara, food);
    }
}
