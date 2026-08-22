using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.SomewhatEnhancedDisplay.Patches;

[HarmonyPatch(typeof(DNA))]
internal static class DNAPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original) && ModContext.Config.EnableDNA.Value;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(DNA.WriteNote), [typeof(UINote), typeof(Chara)])]
    private static IEnumerable<CodeInstruction> WriteNote_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator)
    {
        // // 変更前
        // if (flag)
        // {
        //     text2 = text2 + " (" + element.Value + ")";
        // }
        // // 変更後
        // text2 = text2 + " (" + element.Value + ")";
        var matcher = new CodeMatcher(instructions, generator);

        // brfalse <skipValue>
        // ldloc.s (System.String)
        // ldstr " ("
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Brfalse),
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(OpCodes.Ldstr, " (")
        );
        // 条件分岐をpopに置き換え、分岐判定の値だけをスタックから破棄する。
        // 条件分岐がなくなり、常に遺伝子Elementの値が追加されるようになる。
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Pop)
        );

        return matcher.InstructionEnumeration();
    }
}
