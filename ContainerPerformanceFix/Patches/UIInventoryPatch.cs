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

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    private static UIInventory? _redrawSortOwner;
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
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator)
    {
        var patched = instructions.ToList();
        patched = PatchMergeLoop(patched, generator);
        patched = PatchFinalRedraw(patched, generator);
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

    private static List<CodeInstruction> PatchFinalRedraw(
        List<CodeInstruction> instructions,
        ILGenerator generator)
    {
        try
        {
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
                throw new MissingMemberException(
                    "A method or field required for guarded redraw could not be resolved.");
            }

            // // 変更前
            // if (redraw)
            // {
            //     list.Redraw();
            // }
            // // 変更後
            // if (redraw)
            // {
            //     GuardedRedraw(list, this);
            // }
            var matcher = new CodeMatcher(instructions, generator);
            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldarg_1),
                new CodeMatch(IsBranchFalse),
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld, listField),
                new CodeMatch(instruction =>
                    instruction.Calls(baseRedraw) || instruction.Calls(listRedraw)),
                new CodeMatch(OpCodes.Ret)
            ).ThrowIfInvalid("Could not find UIInventory.Sort's final redraw block.");

            if (matcher.InstructionAt(1).operand is not Label returnLabel
                || !matcher.InstructionAt(5).labels.Contains(returnLabel))
            {
                throw new InvalidOperationException(
                    "The redraw=false branch did not target the matched return.");
            }

            matcher.Advance(4);
            if (matcher.Blocks.Count != 0)
            {
                throw new InvalidOperationException(
                    "The final Redraw call was on an exception-block boundary.");
            }

            var callLabels = matcher.Labels.ToList();
            matcher.Labels.Clear();
            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(callLabels));
            matcher.Set(OpCodes.Call, guardedRedraw);
            return matcher.Instructions();
        }
        catch (Exception ex)
        {
            LogDuplicateSortPatchFailure(
                $"Unexpected error while matching the final redraw: {ex}");
            return instructions;
        }
    }

    private static List<CodeInstruction> PatchMergeLoop(
        List<CodeInstruction> instructions,
        ILGenerator generator)
    {
        try
        {
            var tryStackTo = AccessTools.DeclaredMethod(
                typeof(Card),
                nameof(Card.TryStackTo),
                [typeof(Thing)])
                ?? throw new MissingMethodException(
                    typeof(Card).FullName,
                    nameof(Card.TryStackTo));

            // // 変更前
            // var merged = true;
            // while (merged)
            // {
            //     merged = false;
            //     // owner.Container.thingsを二重に走査してTryStackToする。
            // }
            // // 変更後
            // StackMerger.FastMergeStacks(this);
            var matcher = new CodeMatcher(instructions, generator);
            var flagLocal = -1;
            var conditionLabel = default(Label);

            matcher.MatchStartForward(
                new CodeMatch(instruction => instruction.LoadsConstant(1)),
                new CodeMatch(instruction =>
                    TryGetLocalIndex(instruction, store: true, out flagLocal)),
                new CodeMatch(instruction =>
                    TryGetUnconditionalBranchTarget(instruction, out conditionLabel)),
                new CodeMatch(instruction => instruction.LoadsConstant(0)),
                new CodeMatch(instruction =>
                    TryGetLocalIndex(instruction, store: true, out var local)
                    && local == flagLocal)
            ).ThrowIfInvalid("Could not find UIInventory.Sort's merge-loop initializer.");

            var start = matcher.Pos;
            var loopBodyLabels = matcher.InstructionAt(3).labels;

            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Callvirt, tryStackTo),
                new CodeMatch(IsBranchFalse)
            ).ThrowIfInvalid(
                "Could not find Card.TryStackTo inside UIInventory.Sort's merge loop.");

            matcher.MatchStartForward(
                new CodeMatch(instruction =>
                    instruction.labels.Contains(conditionLabel)
                    && TryGetLocalIndex(instruction, store: false, out var local)
                    && local == flagLocal),
                new CodeMatch(instruction =>
                    IsBranchTrue(instruction)
                    && instruction.operand is Label loopBodyLabel
                    && loopBodyLabels.Contains(loopBodyLabel))
            ).ThrowIfInvalid(
                "Could not find UIInventory.Sort's merge-loop condition.");

            var end = matcher.Pos + 1;
            if (!ExceptionBlocksAreContained(instructions, start, end))
            {
                throw new InvalidOperationException(
                    "The merge loop crossed an exception-block boundary.");
            }

            if (HasExternalBranchIntoRemovedRegion(instructions, start, end))
            {
                throw new InvalidOperationException(
                    "Code outside the merge loop branched into the replacement range.");
            }

            // The original loop and the replacement both enter and leave with an empty stack.
            // Mutating the first instruction preserves any entry labels and block metadata.
            matcher.Start()
                .Advance(start)
                .Set(OpCodes.Ldarg_0, null)
                .Advance(1)
                .RemoveInstructions(end - start)
                .InsertAndAdvance(
                    CodeInstruction.Call(() => StackMerger.FastMergeStacks(default!)));
            return matcher.Instructions();
        }
        catch (Exception ex)
        {
            LogMergePatchFailure(
                $"Unexpected error while matching the stack merge loop: {ex}");
            return instructions;
        }
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

    private static bool TryGetLocalIndex(
        CodeInstruction instruction,
        bool store,
        out int index)
    {
        index = store
            ? instruction.opcode switch
            {
                var opcode when opcode == OpCodes.Stloc_0 => 0,
                var opcode when opcode == OpCodes.Stloc_1 => 1,
                var opcode when opcode == OpCodes.Stloc_2 => 2,
                var opcode when opcode == OpCodes.Stloc_3 => 3,
                _ => -1,
            }
            : instruction.opcode switch
            {
                var opcode when opcode == OpCodes.Ldloc_0 => 0,
                var opcode when opcode == OpCodes.Ldloc_1 => 1,
                var opcode when opcode == OpCodes.Ldloc_2 => 2,
                var opcode when opcode == OpCodes.Ldloc_3 => 3,
                _ => -1,
            };
        if (index >= 0)
        {
            return true;
        }

        var expectedLongOpcode = store ? OpCodes.Stloc : OpCodes.Ldloc;
        var expectedShortOpcode = store ? OpCodes.Stloc_S : OpCodes.Ldloc_S;
        if (instruction.opcode != expectedLongOpcode
            && instruction.opcode != expectedShortOpcode)
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

    private static bool TryGetUnconditionalBranchTarget(
        CodeInstruction instruction,
        out Label target)
    {
        if (IsUnconditionalBranch(instruction)
            && instruction.operand is Label label)
        {
            target = label;
            return true;
        }

        target = default;
        return false;
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
