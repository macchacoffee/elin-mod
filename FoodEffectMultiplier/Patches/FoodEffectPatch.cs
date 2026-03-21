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
        // ...
        // if (!c2.IsPC)
        // {
        //    num2 *= FoodEffectPatch::GetNPCFoodEffectMultiplier(3f);
        // }
        // -----
        // else
        // {
        //    num2 *= FoodEffectPatch::GetPCFoodEffectMultiplier(1f);
        // }
        // -----
        // ...

        // ...
        //  1: ldloc.0 NULL [Label33]
        //  2: ldfld Chara FoodEffect+<>c__DisplayClass1_0::c
        //  3: callvirt virtual bool Card::get_IsPC()
        //  4: brtrue Label34
        //  5: ldloc.3 NULL
        //  6: ldc.r4 3
        //  7: mul NULL
        //  8: stloc.3 NULL
        //  9: ldloc.s 10 (System.Boolean) [Label34]
        // ...

        // ...
        //  1: ldloc.0 NULL [Label33]
        //  2: ldfld Chara FoodEffect+<>c__DisplayClass1_0::c
        //  3: callvirt virtual bool Card::get_IsPC()
        //  4: brtrue LabelMod1
        //  5: ldloc.3 NULL
        //  6: ldc.r4 3
        //  7: call static float AbilityRestriction.Patches.FoodEffectPatch::GetNPCFoodEffectMultiplier(float defaultValue)
        //  8: mul NULL
        //  9: stloc.3 NULL
        // 10: br Label34
        // 11: ldloc.3 NULL [LabelMod1] 
        // 12: ldc.r4 1
        // 13: call static float AbilityRestriction.Patches.FoodEffectPatch::GetPCFoodEffectMultiplier(float defaultValue)
        // 14: mul NULL
        // 15: stloc.3 NULL
        // 16: ldloc.s 10 (System.Boolean) [Label34]
        // ...
        var matcher = new CodeMatcher(instructions, generator);
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Brtrue),
            new CodeMatch(OpCodes.Ldloc_3),
            new CodeMatch(OpCodes.Ldc_R4),
            new CodeMatch(OpCodes.Mul),
            new CodeMatch(OpCodes.Stloc_3)
        );

        var start = matcher.Pos; //  4: brtrue Label34
        var originalLabel = matcher.Instruction.operand;

        matcher.Advance(3); //  7: mul NULL
        matcher.InsertAndAdvance(
            CodeInstruction.Call(() => GetNPCFoodEffectMultiplier(default)) //  7: call static float AbilityRestriction.Patches.FoodEffectPatch::GetNPCFoodEffectMultiplier(float defaultValue)
        ); //  8: mul NULL

        matcher.Advance(2); // 10: ldloc.s 10 (System.Boolean) [Label34]
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Br, originalLabel), // 10: br Label34
            new CodeInstruction(OpCodes.Ldloc_3)            // 11: ldloc.3 NULL
        ); // 12: ldloc.s 10 (System.Boolean) [Label34]
        matcher.CreateLabelWithOffsets(-1, out var label1); // 11: ldloc.3 NULL [LabelMod1]
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldc_R4, 1f),                        // 12: ldc.r4 1f
            CodeInstruction.Call(() => GetPCFoodEffectMultiplier(default)), // 13: call static float AbilityRestriction.Patches.FoodEffectPatch::GetFoodEffectMultiplier(float defaultValue)
            new CodeInstruction(OpCodes.Mul),                               // 14: mul NULL
            new CodeInstruction(OpCodes.Stloc_3)                            // 15: stloc.3 NULL
        ); // 16: ldloc.s 10 (System.Boolean) [Label34]

        matcher.Advance(start - matcher.Pos); //  4: brtrue Label34
        matcher.RemoveInstruction();          //  4: brtrue Label34 -> brtrue LabelMod1
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
