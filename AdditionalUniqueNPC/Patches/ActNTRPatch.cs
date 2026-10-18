using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Macchacoffee.ElinMods.AbilityRestriction;
using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.AdditionalUniqueNPC.Patches;

[HarmonyPatch(typeof(ActNTR))]
internal static class ActNTRPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    private static readonly HashSet<string> _awakeNTRCharaIds = [
        ModConsts.SourceId.Misalie,
    ];

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(ActNTR.CanPerform), [])]
    private static IEnumerable<CodeInstruction> CanPerform_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator)
    {
        // // 変更前
        // Act.CC.HasElement(1239)
        // // 変更後
        // HasAwakePermission(Act.CC, 1239, false)
        var matcher = new CodeMatcher(instructions, generator);

        // 特定のキャラは起きている相手もイラズラの対象にできるようにする。
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldsfld, AccessTools.Field(typeof(Act), nameof(Act.CC))),
            new CodeMatch(OpCodes.Ldc_I4, 1239),
            new CodeMatch(OpCodes.Ldc_I4_0),
            new CodeMatch(
                OpCodes.Callvirt,
                AccessTools.Method(typeof(Card), nameof(Card.HasElement), [typeof(int), typeof(bool)]))
        );
        matcher.Advance(3);
        matcher.Set(
            OpCodes.Call,
            AccessTools.Method(typeof(ActNTRPatch), nameof(HasAwakePermission))
        );

        return matcher.InstructionEnumeration();
    }

    private static bool HasAwakePermission(
        Card chara, int elementId, bool includeNegative)
    {
        return chara.HasElement(elementId, includeNegative) ||
            _awakeNTRCharaIds.Contains(chara.id);
    }
}
