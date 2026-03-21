using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;

namespace DisplayDNAEnchantValues.Patches;

[HarmonyPatch(typeof(DNA))]
public static class DNAPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyPatch(nameof(DNA.WriteNote), [typeof(UINote), typeof(Chara)]), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> WriteNote_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // ...
        // if (flag)
        // {
        //     text2 = text2 + " (" + element.Value + ")";
        // }
        // n.AddText("NoteText_enc", "gene_info".lang(element.Name.ToTitleCase(wholeText: true), text2), color2);
        // ...

        // ...
        // text2 = text2 + " (" + element.Value + ")";
        // n.AddText("NoteText_enc", "gene_info".lang(element.Name.ToTitleCase(wholeText: true), text2), color2);
        // ...

        // ...
        //  1: brfalse Label30
        //  2.:ldloc.s 10 (System.String)
        //  3: ldstr " ("
        //  4: ldloc.s 9 (Element)
        //  5: callvirt int Element::get_Value()
        //  6: stloc.0 NULL
        //  7: ldloca.s 0 (System.Int32)
        //  8: call virtual string int::ToString()
        //  9: ldstr ")"
        // 10: call static string string::Concat(string str0, string str1, string str2, string str3)
        // 11: stloc.s 10 (System.String)
        // 12: ldarg.1 NULL [Label30]
        // 13: ldstr "NoteText_enc"
        // 14: ldstr "gene_info"
        // 15: ldloc.s 9 (Element)
        // ...

        // ...
        //  1: pop
        //  2.:ldloc.s 10 (System.String)
        //  3: ldstr " ("
        //  4: ldloc.s 9 (Element)
        //  5: callvirt int Element::get_Value()
        //  6: stloc.0 NULL
        //  7: ldloca.s 0 (System.Int32)
        //  8: call virtual string int::ToString()
        //  9: ldstr ")"
        // 10: call static string string::Concat(string str0, string str1, string str2, string str3)
        // 11: stloc.s 10 (System.String)
        // 12: ldarg.1 NULL [Label30]
        // 13: ldstr "NoteText_enc"
        // 14: ldstr "gene_info"
        // 15: ldloc.s 9 (Element)
        // ...

        var matcher = new CodeMatcher(instructions, generator);
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Brfalse),
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(OpCodes.Ldstr, " ("),
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Element), nameof(Element.Value))),
            new CodeMatch(OpCodes.Stloc_0),
            new CodeMatch(OpCodes.Ldloca_S),
            new CodeMatch(OpCodes.Call),
            new CodeMatch(OpCodes.Ldstr, ")")
        );

        matcher.RemoveInstruction(); // 1: brfalse Label30
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Pop)
        ); // 1: pop

        return matcher.InstructionEnumeration();
    }
}
