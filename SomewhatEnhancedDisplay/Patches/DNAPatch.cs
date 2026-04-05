using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;

namespace SomewhatEnhancedDisplay.Patches;

[HarmonyPatch(typeof(DNA))]
public static class DNAPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(DNA.WriteNote), [typeof(UINote), typeof(Chara)])]
    private static IEnumerable<CodeInstruction> WriteNote_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // if (flag)
        // {
        //     text2 = text2 + " (" + element.Value + ")";
        // }
        // // 変更後
        // text2 = text2 + " (" + element.Value + ")";
        var matcher = new CodeMatcher(instructions, generator);

        // brfalse Label30
        // ldloc.s 10 (System.String)
        // ldstr " ("
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Brfalse),
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(OpCodes.Ldstr, " (")
        );
        // brfalseをpop (スタックの要素を1つ取り出すだけの命令) に置き換える
        // 条件分岐がなくなり、常に遺伝子Elementの値が追加されるようになる
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Pop)
        );

        return matcher.InstructionEnumeration();
    }
}
