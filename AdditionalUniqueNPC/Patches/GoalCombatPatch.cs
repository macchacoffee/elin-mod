using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;
using UnityEngine;

using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.AdditionalUniqueNPC.Patches;

[HarmonyPatch(typeof(GoalCombat))]
internal static class GoalCombatPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(GoalCombat.TryUseAbility), [typeof(int), typeof(bool)])]
    private static IEnumerable<CodeInstruction> TryUseAbility_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator)
    {
        // // 変更前
        // if (num > 0)
        // // 変更後
        // num = AdjustAbilityPriority(num, this, ability);
        // if (num > 0)
        var matcher = new CodeMatcher(instructions, generator);

        // ldloc.s num
        // ldc.i4.0
        // ble.s ...
        // ldloc.s num
        // ldloc.s ability
        // ldfld int GoalCombat.ItemAbility::priorityMod
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(OpCodes.Ldc_I4_0),
            new CodeMatch(OpCodes.Ble_S),
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(
                OpCodes.Ldfld,
                AccessTools.Field(typeof(GoalCombat.ItemAbility), nameof(GoalCombat.ItemAbility.priorityMod)))
        ).ThrowIfInvalid("Could not find GoalCombat.TryUseAbility's priority adjustment block.");

        var numOperand = matcher.Operand;
        matcher.Advance(4);
        var abilityOperand = matcher.Operand;
        matcher.Advance(-4);

        // 既存の分岐からも追加処理を通るようにする。
        var labels = matcher.Labels.Copy();
        matcher.Labels.Clear();
        matcher.Insert(
            new CodeInstruction(OpCodes.Ldloc_S, numOperand),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldloc_S, abilityOperand),
            CodeInstruction.Call(() => AdjustAbilityPriority(default, default!, default!)),
            new CodeInstruction(OpCodes.Stloc_S, numOperand)
        );
        matcher.AddLabels(labels);

        return matcher.InstructionEnumeration();
    }

    private static int AdjustAbilityPriority(
        int num,
        GoalCombat combat,
        GoalCombat.ItemAbility ability)
    {
        var owner = combat.owner;

        if (owner == null ||
            ability.act?.id != ABILITY.ActNTR ||
            !owner.HasElement(ModConsts.SourceNumericId.FeatManaAbsorber))
        {
            return num;
        }

        var missing = Mathf.Clamp(
            100 - (int)((long)owner.mana.value * 100
                / Mathf.Max(1, owner.mana.max)),
            0,
            100
        );

        // MPが少ないほどイタズラの優先度を上げる。
        return missing / 2;
    }
}
