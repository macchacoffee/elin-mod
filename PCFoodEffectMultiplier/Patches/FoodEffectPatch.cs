using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;
using PCFoodEffectMultiplier;

namespace AbilityRestriction.Patches;


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
        //    num2 *= 3f;
        // }
        // -----
        // else
        // {
        //    num2 *= Xf;
        // }
        // -----
        /// ...

        // ...
        //  1: ldloc.0 NULL [Label33]
        //  2: ldfld Chara FoodEffect+<>c__DisplayClass1_0::c
        //  3: callvirt virtual bool Card::get_IsPC()
        //  4: brtrue Label34 -> brtrue LabelMod1
        //  5: ldloc.3 NULL
        //  6: ldc.r4 3
        //  7: mul NULL
        //  8: stloc.3 NULL
        // -----
        //  9: br Label34
        // 10: ldloc.3 NULL [LabelMod1] 
        // 11: ldc.r4 X
        // 12: mul NULL
        // 13: stloc.3 NULL
        // -----
        // 14: ldloc.s 10 (System.Boolean) [Label34]
        // ...

        var matcher = new CodeMatcher(instructions, generator);
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Brtrue),
            new CodeMatch(OpCodes.Ldloc_3),
            new CodeMatch(OpCodes.Ldc_R4, 3f),
            new CodeMatch(OpCodes.Mul),
            new CodeMatch(OpCodes.Stloc_3)
        );

        var start = matcher.Pos; //  4: brtrue Label34
        var originalLabel = matcher.Instruction.operand;

        matcher.Advance(5); //  9: ldloc.s 10 (System.Boolean) [Label34]
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Br, originalLabel), //  9: br Label34
            new CodeInstruction(OpCodes.Ldloc_3)            // 10: ldloc.3 NULL
        );
        matcher.CreateLabelWithOffsets(-1, out var label1); // 10: ldloc.3 NULL [LabelMod1]
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldc_R4, 3f), // 11: ldc.r4 X
            new CodeInstruction(OpCodes.Mul),        // 12: mul NULL
            new CodeInstruction(OpCodes.Stloc_3)     // 13: stloc.3 NULL
        );

        matcher.Advance(start - matcher.Pos); //  4: brtrue Label34
        matcher.RemoveInstruction();          //  4: brtrue Label34 -> brtrue LabelMod1
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Brtrue, label1)
        );

        return matcher.InstructionEnumeration();
    }
}
