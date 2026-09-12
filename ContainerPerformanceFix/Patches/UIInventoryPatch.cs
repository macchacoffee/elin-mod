using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Macchacoffee.ElinMods.ContainerPerformanceFix.Mod;
using Macchacoffee.ElinMods.ModUtility.Logging;
using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.ContainerPerformanceFix.Patches;

[HarmonyPatch(typeof(UIInventory))]
internal static class UIInventoryPatch
{
    private static readonly PatchTarget _patchTarget = new();

    private static UIInventory? _redrawSortOwner;

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    private static bool _mergeFailureLogged;
    private static bool _duplicateSortFailureLogged;

    [HarmonyPrefix]
    [HarmonyPatch(nameof(UIInventory.Sort), [typeof(bool)])]
    private static bool Sort_Prefix(UIInventory __instance, bool redraw)
    {
        return redraw || !ReferenceEquals(_redrawSortOwner, __instance);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(UIInventory.Sort), [typeof(bool)])]
    private static IEnumerable<CodeInstruction> Sort_Transpiler(
        IEnumerable<CodeInstruction> instructions)
    {
        var patched = instructions.ToList();

        try
        {
            if (!TryFindMergeLoop(patched, out var start, out var end, out var reason))
            {
                LogMergePatchFailure(reason);
            }
            else
            {
                var replacementMethod = AccessTools.DeclaredMethod(
                    typeof(StackMerger),
                    nameof(StackMerger.FastMergeStacks),
                    [typeof(UIInventory)]);
                if (replacementMethod is null)
                {
                    LogMergePatchFailure("FastMergeStacks could not be resolved.");
                }
                else
                {
                    var replacement = new CodeInstruction(OpCodes.Ldarg_0);
                    replacement.labels.AddRange(patched[start].labels);

                    var mergePatched = patched.ToList();
                    mergePatched.RemoveRange(start, end - start + 1);
                    mergePatched.InsertRange(start,
                    [
                        replacement,
                        new CodeInstruction(OpCodes.Call, replacementMethod)
                    ]);
                    patched = mergePatched;
                }
            }
        }
        catch (Exception ex)
        {
            LogMergePatchFailure($"Unexpected error while matching the stack merge loop: {ex}");
        }

        try
        {
            if (TryReplaceFinalRedraw(patched, out var redrawPatched, out var reason))
            {
                patched = redrawPatched;
            }
            else
            {
                LogDuplicateSortPatchFailure(reason);
            }
        }
        catch (Exception ex)
        {
            LogDuplicateSortPatchFailure(
                $"Unexpected error while matching the final redraw: {ex}");
        }

        return patched;
    }

    private static void GuardedRedraw(UIList list, UIInventory inventory)
    {
        var previous = _redrawSortOwner;
        _redrawSortOwner = inventory;

        try
        {
            list.Redraw();
        }
        finally
        {
            _redrawSortOwner = previous;
        }
    }

    private static bool TryReplaceFinalRedraw(
        IReadOnlyList<CodeInstruction> instructions,
        out List<CodeInstruction> patched,
        out string reason)
    {
        patched = instructions.ToList();
        reason = "The final UIInventory list redraw pattern was not found.";

        var baseRedraw = AccessTools.DeclaredMethod(
            typeof(BaseList),
            nameof(BaseList.Redraw),
            Type.EmptyTypes);
        var listRedraw = AccessTools.DeclaredMethod(
            typeof(UIList),
            nameof(UIList.Redraw),
            Type.EmptyTypes);
        var listField = AccessTools.DeclaredField(
            typeof(UIInventory),
            nameof(UIInventory.list));
        var guardedRedraw = AccessTools.DeclaredMethod(
            typeof(UIInventoryPatch),
            nameof(GuardedRedraw),
            [typeof(UIList), typeof(UIInventory)]);
        if (baseRedraw is null || listRedraw is null
            || listField is null || guardedRedraw is null)
        {
            reason = "A method or field required for guarded redraw could not be resolved.";
            return false;
        }

        var redrawCalls = new List<int>();
        for (var i = 0; i < instructions.Count; i++)
        {
            if (instructions[i].Calls(baseRedraw) || instructions[i].Calls(listRedraw))
            {
                redrawCalls.Add(i);
            }
        }

        if (redrawCalls.Count != 1)
        {
            reason = $"Expected exactly one list Redraw call, found {redrawCalls.Count}.";
            return false;
        }

        var callIndex = redrawCalls[0];
        if (callIndex < 4 || callIndex + 2 != instructions.Count
            || instructions[callIndex - 4].opcode != OpCodes.Ldarg_1
            || !IsBranchFalse(instructions[callIndex - 3])
            || instructions[callIndex - 2].opcode != OpCodes.Ldarg_0
            || instructions[callIndex - 1].opcode != OpCodes.Ldfld
            || !Equals(instructions[callIndex - 1].operand, listField)
            || instructions[callIndex + 1].opcode != OpCodes.Ret)
        {
            reason = "The Redraw call was not inside the expected redraw-argument tail block.";
            return false;
        }

        if (instructions[callIndex - 3].operand is not Label returnLabel)
        {
            reason = "The redraw condition did not branch to a label.";
            return false;
        }

        var branchTargetIndex = FindLabelTarget(instructions, returnLabel);
        if (branchTargetIndex != callIndex + 1)
        {
            reason = "The redraw=false branch did not target the final return.";
            return false;
        }

        if (instructions[callIndex].blocks.Count != 0)
        {
            reason = "The final Redraw call was on an exception-block boundary.";
            return false;
        }

        var inventoryLoad = new CodeInstruction(OpCodes.Ldarg_0);
        inventoryLoad.labels.AddRange(instructions[callIndex].labels);
        var guardedCall = new CodeInstruction(OpCodes.Call, guardedRedraw);

        patched.RemoveAt(callIndex);
        patched.InsertRange(callIndex,
        [
            inventoryLoad,
            guardedCall
        ]);
        return true;
    }

    private static bool TryFindMergeLoop(
        IReadOnlyList<CodeInstruction> instructions,
        out int start,
        out int end,
        out string reason)
    {
        start = -1;
        end = -1;
        reason = "The vanilla stack merge loop pattern was not found.";

        var tryStackTo = AccessTools.DeclaredMethod(
            typeof(Card),
            nameof(Card.TryStackTo),
            [typeof(Thing)]);
        if (tryStackTo is null)
        {
            reason = "Card.TryStackTo(Thing) could not be resolved.";
            return false;
        }

        var tryStackCalls = new List<int>();
        for (var i = 0; i < instructions.Count; i++)
        {
            if (instructions[i].Calls(tryStackTo))
            {
                tryStackCalls.Add(i);
            }
        }

        if (tryStackCalls.Count != 1)
        {
            reason = $"Expected exactly one Card.TryStackTo call, found {tryStackCalls.Count}.";
            return false;
        }

        var tryStackCall = tryStackCalls[0];
        var startCandidates = new List<int>();
        for (var i = 0; i + 4 < tryStackCall; i++)
        {
            if (instructions[i].opcode == OpCodes.Ldc_I4_1
                && TryGetStoredLocalIndex(instructions[i + 1], out _)
                && IsUnconditionalBranch(instructions[i + 2])
                && instructions[i + 3].opcode == OpCodes.Ldc_I4_0
                && TryGetStoredLocalIndex(instructions[i + 4], out _))
            {
                startCandidates.Add(i);
            }
        }

        if (startCandidates.Count != 1)
        {
            reason = $"Expected one merge-loop initializer, found {startCandidates.Count}.";
            return false;
        }

        start = startCandidates[0];
        if (!TryGetStoredLocalIndex(instructions[start + 1], out var flagLocal)
            || !TryGetStoredLocalIndex(instructions[start + 4], out var loopFlagLocal)
            || flagLocal != loopFlagLocal)
        {
            reason = "The merge-loop flag local did not match its initializer.";
            return false;
        }

        if (instructions[start + 2].operand is not Label conditionLabel)
        {
            reason = "The initial merge-loop branch target was not a label.";
            return false;
        }

        var conditionIndex = FindLabelTarget(instructions, conditionLabel);
        if (conditionIndex <= tryStackCall || conditionIndex + 1 >= instructions.Count)
        {
            reason = "The merge-loop condition target was outside the expected range.";
            return false;
        }

        if (!TryGetLoadedLocalIndex(instructions[conditionIndex], out var conditionLocal)
            || conditionLocal != flagLocal
            || !IsBranchTrue(instructions[conditionIndex + 1]))
        {
            reason = "The trailing merge-loop condition did not use the expected flag.";
            return false;
        }

        var loopBodyLabels = instructions[start + 3].labels;
        if (instructions[conditionIndex + 1].operand is not Label loopBodyLabel
            || !loopBodyLabels.Contains(loopBodyLabel))
        {
            reason = "The trailing merge-loop branch did not return to the loop body.";
            return false;
        }

        end = conditionIndex + 1;
        if (tryStackCall <= start || tryStackCall >= end
            || tryStackCall + 1 >= instructions.Count
            || !IsBranchFalse(instructions[tryStackCall + 1]))
        {
            reason = "Card.TryStackTo was not inside the expected success-check structure.";
            return false;
        }

        if (!ExceptionBlocksAreContained(instructions, start, end))
        {
            reason = "The merge loop crossed an exception-block boundary.";
            return false;
        }

        if (HasExternalBranchIntoRemovedRegion(instructions, start, end))
        {
            reason = "Code outside the merge loop branched into the replacement range.";
            return false;
        }

        return true;
    }

    private static int FindLabelTarget(IReadOnlyList<CodeInstruction> instructions, Label label)
    {
        for (var i = 0; i < instructions.Count; i++)
        {
            if (instructions[i].labels.Contains(label))
            {
                return i;
            }
        }

        return -1;
    }

    private static bool ExceptionBlocksAreContained(
        IReadOnlyList<CodeInstruction> instructions,
        int start,
        int end)
    {
        var depth = 0;
        for (var i = 0; i < start; i++)
        {
            UpdateExceptionDepth(instructions[i], ref depth);
        }

        if (depth != 0)
        {
            return false;
        }

        for (var i = start; i <= end; i++)
        {
            UpdateExceptionDepth(instructions[i], ref depth);
            if (depth < 0)
            {
                return false;
            }
        }

        return depth == 0;
    }

    private static void UpdateExceptionDepth(CodeInstruction instruction, ref int depth)
    {
        foreach (var block in instruction.blocks)
        {
            if (block.blockType == ExceptionBlockType.BeginExceptionBlock)
            {
                depth++;
            }
            else if (block.blockType == ExceptionBlockType.EndExceptionBlock)
            {
                depth--;
            }
        }
    }

    private static bool HasExternalBranchIntoRemovedRegion(
        IReadOnlyList<CodeInstruction> instructions,
        int start,
        int end)
    {
        var removedLabels = new HashSet<Label>();
        for (var i = start + 1; i <= end; i++)
        {
            foreach (var label in instructions[i].labels)
            {
                removedLabels.Add(label);
            }
        }

        for (var i = 0; i < instructions.Count; i++)
        {
            if (i >= start && i <= end)
            {
                continue;
            }

            if (instructions[i].operand is Label label && removedLabels.Contains(label))
            {
                return true;
            }

            if (instructions[i].operand is Label[] labels)
            {
                foreach (var switchLabel in labels)
                {
                    if (removedLabels.Contains(switchLabel))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private static bool TryGetStoredLocalIndex(CodeInstruction instruction, out int index)
    {
        if (instruction.opcode == OpCodes.Stloc_0)
        {
            index = 0;
            return true;
        }
        if (instruction.opcode == OpCodes.Stloc_1)
        {
            index = 1;
            return true;
        }
        if (instruction.opcode == OpCodes.Stloc_2)
        {
            index = 2;
            return true;
        }
        if (instruction.opcode == OpCodes.Stloc_3)
        {
            index = 3;
            return true;
        }

        return TryGetOperandLocalIndex(instruction, OpCodes.Stloc, OpCodes.Stloc_S, out index);
    }

    private static bool TryGetLoadedLocalIndex(CodeInstruction instruction, out int index)
    {
        if (instruction.opcode == OpCodes.Ldloc_0)
        {
            index = 0;
            return true;
        }
        if (instruction.opcode == OpCodes.Ldloc_1)
        {
            index = 1;
            return true;
        }
        if (instruction.opcode == OpCodes.Ldloc_2)
        {
            index = 2;
            return true;
        }
        if (instruction.opcode == OpCodes.Ldloc_3)
        {
            index = 3;
            return true;
        }

        return TryGetOperandLocalIndex(instruction, OpCodes.Ldloc, OpCodes.Ldloc_S, out index);
    }

    private static bool TryGetOperandLocalIndex(
        CodeInstruction instruction,
        OpCode longOpcode,
        OpCode shortOpcode,
        out int index)
    {
        index = -1;
        if (instruction.opcode != longOpcode && instruction.opcode != shortOpcode)
        {
            return false;
        }

        switch (instruction.operand)
        {
            case LocalBuilder local:
                index = local.LocalIndex;
                return true;
            case int intIndex:
                index = intIndex;
                return true;
            case byte byteIndex:
                index = byteIndex;
                return true;
            case sbyte sbyteIndex:
                index = sbyteIndex;
                return true;
            default:
                return false;
        }
    }

    private static bool IsUnconditionalBranch(CodeInstruction instruction)
    {
        return instruction.opcode == OpCodes.Br || instruction.opcode == OpCodes.Br_S;
    }

    private static bool IsBranchTrue(CodeInstruction instruction)
    {
        return instruction.opcode == OpCodes.Brtrue || instruction.opcode == OpCodes.Brtrue_S;
    }

    private static bool IsBranchFalse(CodeInstruction instruction)
    {
        return instruction.opcode == OpCodes.Brfalse || instruction.opcode == OpCodes.Brfalse_S;
    }

    private static void LogMergePatchFailure(string reason)
    {
        if (_mergeFailureLogged)
        {
            return;
        }

        _mergeFailureLogged = true;
        ModLog.Error(
            $"Failed to patch the UIInventory.Sort stack merge optimization. "
            + $"Falling back to vanilla stack merging. Reason: {reason}");
    }

    private static void LogDuplicateSortPatchFailure(string reason)
    {
        if (_duplicateSortFailureLogged)
        {
            return;
        }

        _duplicateSortFailureLogged = true;
        ModLog.Error(
            $"Failed to patch duplicate Sort suppression. "
            + $"Falling back to vanilla redraw sorting. Reason: {reason}");
    }
}
