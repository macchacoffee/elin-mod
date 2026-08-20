using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

using HarmonyLib;

using Macchacoffee.ElinMods.FactionEnchantInInventory.Extensions;
using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.FactionEnchantInInventory.Patches;

[HarmonyPatch(typeof(ElementContainerFaction))]
internal static class ElementContainerFactionPatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    private class ECFElementExtra
    {
        public int AppliedCount { get; private set; } = 0;
        public bool IsApplied => AppliedCount > 0;

        public bool AddApplied()
        {
            AppliedCount += 1;
            return true;
        }

        public bool RemoveApplied()
        {
            if (AppliedCount <= 0)
            {
                return false;
            }
            AppliedCount -= 1;
            return true;
        }
    }

    private static readonly ConditionalWeakTable<Element, ECFElementExtra> _elementExtra = [];

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ElementContainerFaction.OnAddMemeber), [typeof(Chara)])]
    private static void OnAddMemeber_Postfix(ElementContainerFaction __instance, Chara c)
    {
        __instance.OnAddThings(c);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ElementContainerFaction.OnRemoveMember), [typeof(Chara)])]
    private static void OnRemoveMember_Postfix(ElementContainerFaction __instance, Chara c)
    {
        __instance.OnRemoveThings(c);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(ElementContainerFaction.OnEquip), [typeof(Thing)])]
    private static IEnumerable<CodeInstruction> OnEquip_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        //  if (value.IsGlobalElement)
        // {
        //     ModBase(value.id, value.Value).vExp = value.vExp;
        //     isDirty = true;
        // }
        // // 変更後
        // if (value.IsGlobalElement)
        // {
        //     if (!ElementContainerFactionPatch.IsElementApplied(value))
        //     {
        //         ModBase(value.id, value.Value).vExp = value.vExp;
        //         isDirty = true;
        //     }
        //     ElementContainerFactionPatch.AddElementApplied(value);
        // }
        // ...
        var matcher = new CodeMatcher(instructions, generator);

        // callvirt bool Element::get_IsGlobalElement()
        // brfalse Label3
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Element), nameof(Element.IsGlobalElement))),
            new CodeMatch(OpCodes.Brfalse)
        );
        // エンチャントの適用フラグを有効にする処理を追加する
        matcher.Advance(1);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_1),
            CodeInstruction.Call(() => IsElementApplied(default!)),
            new CodeInstruction(OpCodes.Brtrue, null)
        );
        var pos1 = matcher.Pos - 1;
        // stfld bool ElementContainerFaction::isDirty
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Stfld, AccessTools.Field(typeof(ElementContainerFaction), nameof(ElementContainerFaction.isDirty)))
        );
        matcher.Advance(1);
        matcher.Insert(
            new CodeInstruction(OpCodes.Ldloc_1),
            CodeInstruction.Call(() => AddElementApplied(default!)),
            new CodeInstruction(OpCodes.Pop)
        );
        matcher.CreateLabel(out var labelMod1);
        matcher.Advance(pos1 - matcher.Pos);
        matcher.Operand = labelMod1;

        return matcher.InstructionEnumeration();
    }


    [HarmonyTranspiler]
    [HarmonyPatch(nameof(ElementContainerFaction.OnUnequip), [typeof(Thing)])]
    private static IEnumerable<CodeInstruction> OnUnequip_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        //  if (value.IsGlobalElement)
        // {
        //     ModBase(value.id, -value.Value);
        //     isDirty = true;
        // }
        // // 変更後
        // if (value.IsGlobalElement)
        // {
        //     if (ElementContainerFactionPatch.RemoveElementApplied(value) && !ElementContainerFactionPatch.IsElementApplied(value)) 
        //     {
        //         ModBase(value.id, -value.Value);
        //         isDirty = true;
        //     }
        // }
        var matcher = new CodeMatcher(instructions, generator);

        // callvirt bool Element::get_IsGlobalElement()
        // brfalse Label3
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Element), nameof(Element.IsGlobalElement))),
            new CodeMatch(OpCodes.Brfalse)
        );
        // エンチャントの適用フラグを有効にする処理を追加する
        var label1 = matcher.Operand;
        matcher.Advance(1);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_1),
            CodeInstruction.Call(() => RemoveElementApplied(default!)),
            new CodeInstruction(OpCodes.Brfalse, label1),
            new CodeInstruction(OpCodes.Ldloc_1),
            CodeInstruction.Call(() => IsElementApplied(default!)),
            new CodeInstruction(OpCodes.Brtrue, label1)
        );

        return matcher.InstructionEnumeration();
    }

    private static bool IsElementApplied(Element element)
    {
        if (!_elementExtra.TryGetValue(element, out var extra))
        {
            return false;
        }
        return extra.IsApplied;
    }

    private static bool AddElementApplied(Element element)
    {
        return GetOrAddElementExtra(element).AddApplied();
    }

    private static bool RemoveElementApplied(Element element)
    {
        return GetOrAddElementExtra(element).RemoveApplied();
    }

    private static ECFElementExtra GetOrAddElementExtra(Element element)
    {
        if (!_elementExtra.TryGetValue(element, out var extra))
        {
            extra = new();
            _elementExtra.Add(element, extra);
        }
        return extra;
    }
}
