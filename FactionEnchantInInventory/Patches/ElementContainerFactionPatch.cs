using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using ModUtility.Patch;
using FactionEnchantInInventory.Extensions;

namespace FactionEnchantInInventory.Patches;

[HarmonyPatch(typeof(ElementContainerFaction))]
public static class ElementContainerFactionPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    private class ECFElementExtra
    {
        public bool IsApplied { get; set; }
    }

    private static readonly ConditionalWeakTable<Element, ECFElementExtra> ElementExtra = new();

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
        // if (value.IsGlobalElement && !ElementContainerFactionPatch.IsElementApplied(value))
        // {
        //     ElementContainerFactionPatch.SetElementApplied(value);
        //     ModBase(value.id, value.Value).vExp = value.vExp;
        //     isDirty = true;
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
        var label1 = matcher.Operand;
        matcher.Advance(1);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_1),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ElementContainerFactionPatch), nameof(IsElementApplied), [typeof(Element)])),
            new CodeInstruction(OpCodes.Brtrue, label1),
            new CodeInstruction(OpCodes.Ldloc_1),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ElementContainerFactionPatch), nameof(SetElementApplied), [typeof(Element)]))
        );

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
        // if (value.IsGlobalElement || ElementContainerFactionPatch.IsElementApplied(value))
        // {
        //     ElementContainerFactionPatch.UnsetEffectApplied(value);
        //     ModBase(value.id, -value.Value);
        //     isDirty = true;
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
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ElementContainerFactionPatch), nameof(IsElementApplied), [typeof(Element)])),
            new CodeInstruction(OpCodes.Brfalse, label1),
            new CodeInstruction(OpCodes.Ldloc_1),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ElementContainerFactionPatch), nameof(UnsetElementApplied), [typeof(Element)]))
        );

        return matcher.InstructionEnumeration();
    }

    private static bool IsElementApplied(Element element)
    {
        if (!ElementExtra.TryGetValue(element, out var extra))
        {
            return false;
        }
        return extra.IsApplied;
    }

    private static void SetElementApplied(Element element)
    {
        UpdateElementApplied(element, true);
    }

    private static void UnsetElementApplied(Element element)
    {
        UpdateElementApplied(element, false);
    }

    private static void UpdateElementApplied(Element element, bool value)
    {
        if (!ElementExtra.TryGetValue(element, out var extra))
        {
            extra = new();
            ElementExtra.Add(element, extra);
        }
        extra.IsApplied = value;
    }
}