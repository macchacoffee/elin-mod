using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;

namespace DisplayDNAEnchantValues.Patches;

[HarmonyPatch(typeof(Element))]
public static class ElementPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyPatch(nameof(Element.AddEncNote), [typeof(UINote), typeof(Card), typeof(ElementContainer.NoteMode), typeof(Func<Element, string, string>), typeof(Action<UINote, Element>)]), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> AddEncNote_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // ...
        // if (!flag && !flag2 && !source.tag.Contains("flag"))
        // {
        //     text = text + " [" + "*".Repeat(Mathf.Clamp(num * source.mtp / num4 + num3, 1, 5)) + ((num * source.mtp / num4 + num3 > 5) ? "+" : "") + "]";
        // }
        // ...

        // ...
        // if (!flag && !flag2 && !source.tag.Contains("flag"))
        // {
        //     text = text + " [" + "*".Repeat(Mathf.Clamp(num * source.mtp / num4 + num3, 1, 5)) + ((num * source.mtp / num4 + num3 > 5) ? "+" : "") + "]" + "(" + num + ")";
        // }
        // ...

        // ...
        //  1: brtrue Label59
        //  2: ldc.i4.5 NULL
        //  3: newarr System.String
        //  4: dup NULL
        //  5: ldc.i4.0 NULL
        //  6: ldloc.0 NULL
        //  7: stelem.ref NULL
        //  8: dup NULL
        //  9: ldc.i4.1 NULL
        // 10: ldstr " ["
        // ...
        // 11: ldstr "+" [Label60]
        // 12: stelem.ref NULL [Label61]
        // 13: dup NULL
        // 14: ldc.i4.4 NULL
        // 15: ldstr "]"
        // 16: stelem.ref NULL
        // 17: call static string string::Concat(string[] values)
        // 18: stloc.0 NULL
        // 19: ldarg.0 NULL [Label57, Label58, Label59]
        // 20: ldstr "hidden"
        // ...

        // ...
        //  1: brtrue Label59
        //  2: ldc.i4.5 NULL
        //  3: newarr System.String
        //  4: dup NULL
        //  5: ldc.i4.0 NULL
        //  6: ldloc.0 NULL
        //  7: stelem.ref NULL
        //  8: dup NULL
        //  9: ldc.i4.1 NULL
        // 10: ldstr " ["
        // ...
        // 11: ldstr "+" [Label60]
        // 12: stelem.ref NULL [Label61]
        // 13: dup NULL
        // 14: ldc.i4.4 NULL
        // 15: ldstr "]"
        // 16: stelem.ref NULL

        // 17: dup NULL
        // 18: ldc.i4.5 NULL
        // 19: ldstr "("
        // 20: stelem.ref NULL
        // 21: dup NULL
        // 22: ldc.i4.6 NULL
        // 23: ldloc.s 5 (System.Int32)
        // 24: call virtual string int::ToString()
        // 25: stelem.ref NULL
        // 26: dup NULL
        // 27: ldc.i4.7 NULL
        // 28: ldstr ")"
        // 29: stelem.ref NULL

        // 30: call static string string::Concat(string[] values)
        // 31: stloc.0 NULL
        // 32: ldarg.0 NULL [Label57, Label58, Label59]
        // 33: ldstr "hidden"
        // ...

        var matcher = new CodeMatcher(instructions, generator);
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Brtrue),
            new CodeMatch(OpCodes.Ldc_I4_5),
            new CodeMatch(OpCodes.Newarr),
            new CodeMatch(OpCodes.Dup),
            new CodeMatch(OpCodes.Ldc_I4_0),
            new CodeMatch(OpCodes.Ldloc_0),
            new CodeMatch(OpCodes.Stelem_Ref),
            new CodeMatch(OpCodes.Dup),
            new CodeMatch(OpCodes.Ldc_I4_1),
            new CodeMatch(OpCodes.Ldstr, " [")
        );

        matcher.Advance(1);          //  2: ldc.i4.5 NULL
        matcher.RemoveInstruction(); //  2: ldc.i4.5 NULL -> ldc.i4.8 NULL
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldc_I4_8)
        );

        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldstr, "+"),
            new CodeMatch(OpCodes.Stelem_Ref),
            new CodeMatch(OpCodes.Dup),
            new CodeMatch(OpCodes.Ldc_I4_4),
            new CodeMatch(OpCodes.Ldstr, "]"),
            new CodeMatch(OpCodes.Stelem_Ref),
            new CodeMatch(OpCodes.Call)
        );

        // 17: call static string string::Concat(string[] values)
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Dup),
            new CodeInstruction(OpCodes.Ldc_I4_5),
            new CodeInstruction(OpCodes.Ldstr, " ("),
            new CodeInstruction(OpCodes.Stelem_Ref),
            new CodeInstruction(OpCodes.Dup),
            new CodeInstruction(OpCodes.Ldc_I4_6),
            new CodeInstruction(OpCodes.Ldloca_S, 5),
            CodeInstruction.Call(() => default(int).ToString()),
            new CodeInstruction(OpCodes.Stelem_Ref),
            new CodeInstruction(OpCodes.Dup),
            new CodeInstruction(OpCodes.Ldc_I4_7),
            new CodeInstruction(OpCodes.Ldstr, ")"),
            new CodeInstruction(OpCodes.Stelem_Ref)
        ); // 30: call static string string::Concat(string[] values)

        return matcher.InstructionEnumeration();
    }
}
