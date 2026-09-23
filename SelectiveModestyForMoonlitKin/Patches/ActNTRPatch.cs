using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

namespace Macchacoffee.ElinMods.SelectiveModestyForMoonlitKin.Patches;

[HarmonyPatch(typeof(ActNTR))]
internal static class ActNTRPatch
{
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(ActNTR.CanPerform), [])]
    private static IEnumerable<CodeInstruction> CanPerform_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator)
    {
        // // 変更前
        // if (Act.CC.GetInt(119) != 0)
        // {
        //     return false;
        // }
        // // 変更後
        // Act.CC.GetInt(119);
        var matcher = new CodeMatcher(instructions, generator);

        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Brfalse)
        );
        matcher.Opcode = OpCodes.Br;
        matcher.Insert(new CodeInstruction(OpCodes.Pop));

        return matcher.InstructionEnumeration();
    }
}
