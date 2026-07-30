using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using ModUtility.Patch;

namespace FactionEffectInPCInventory.Patches;

[HarmonyPatch(typeof(ElementContainerFaction))]
public static class ElementContainerFactionPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    private class ElementContainerFactionExtraData
    {
        public bool IsEffectApplied { get; set; }
    }

    private static readonly ConditionalWeakTable<ElementContainerFaction, ElementContainerFactionExtraData> ECFExtraData = new();

    [HarmonyPrefix]
    [HarmonyPatch(nameof(ElementContainerFaction.OnEquip), [typeof(Chara), typeof(Thing)])]
    private static void OnEquip_Prefix(ElementContainerFaction __instance, Chara c, Thing t)
    {
        Plugin.LogInfo($"OnEquip_Prefix {__instance} {c.Name} {t.Name}");
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(ElementContainerFaction.OnUnequip), [typeof(Chara), typeof(Thing)])]
    private static void OnUnequip_Prefix(ElementContainerFaction __instance, Chara c, Thing t)
    {
        Plugin.LogInfo($"OnUnequip_Prefix {__instance} {c.Name} {t.Name}");
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(ElementContainerFaction.OnEquip), [typeof(Thing)])]
    private static IEnumerable<CodeInstruction> OnEquip_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // if (!IsEffective(t))
        // {
        //     return;
        // }
        // ...
        // ModBase(value.id, value.Value).vExp = value.vExp;
        // isDirty = true;
        // // 変更後
        // if (!IsEffective(t) || ElementContainerFactionPatch.IsEffectApplied(this))
        // {
        //     return;
        // }
        // ...
        // ModBase(value.id, value.Value).vExp = value.vExp;
        // ElementContainerFactionPatch.SetEffectApplied(this);
        // isDirty = true;
        var matcher = new CodeMatcher(instructions, generator);

        // call bool ElementContainerFaction::IsEffective(Thing t)
        // brtrue Label1
        // ret NULL
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ElementContainerFaction), nameof(ElementContainerFaction.IsEffective), [typeof(Thing)])),
            new CodeMatch(OpCodes.Brtrue),
            new CodeMatch(OpCodes.Ret)
        );
        // エンチャントが未適用であることをチェックする処理を挿入する
        matcher.CreateLabel(out var labelMod1);
        matcher.Advance(-1);
        var label1 = matcher.Operand;
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Brfalse, labelMod1),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ElementContainerFactionPatch), nameof(IsEffectApplied), [typeof(ElementContainerFaction)])),
            new CodeInstruction(OpCodes.Brfalse, label1)
        );

        // ldarg.0 NULL
        // ldc.i4.1 NULL
        // stfld bool ElementContainerFaction::isDirty
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldc_I4_1),
            new CodeMatch(OpCodes.Stfld, AccessTools.Field(typeof(ElementContainerFaction), nameof(ElementContainerFaction.isDirty)))
        );
        // エンチャントの適用フラグを有効にする処理を追加する
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ElementContainerFactionPatch), nameof(SetEffectApplied), [typeof(ElementContainerFaction)]))
        );

        return matcher.InstructionEnumeration();
    }


    [HarmonyTranspiler]
    [HarmonyPatch(nameof(ElementContainerFaction.OnUnequip), [typeof(Thing)])]
    private static IEnumerable<CodeInstruction> OnUnequip_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // if (!IsEffective(t))
        // {
        //     return;
        // }
        // ...
        // ModBase(value.id, -value.Value);
        // isDirty = true;
        // // 変更後
        // if (!IsEffective(t) || !ElementContainerFactionPatch.IsEffectApplied(this))
        // {
        //     return;
        // }
        // ...
        // ModBase(value.id, -value.Value);
        // ElementContainerFactionPatch.UnsetEffectApplied(this);
        // isDirty = true;
        var matcher = new CodeMatcher(instructions, generator);

        // call bool ElementContainerFaction::IsEffective(Thing t)
        // brtrue Label1
        // ret NULL
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ElementContainerFaction), nameof(ElementContainerFaction.IsEffective), [typeof(Thing)])),
            new CodeMatch(OpCodes.Brtrue),
            new CodeMatch(OpCodes.Ret)
        );
        // エンチャントが適用済みであることをチェックする処理を挿入する
        matcher.CreateLabel(out var labelMod1);
        matcher.Advance(-1);
        var label1 = matcher.Operand;
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Brfalse, labelMod1),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ElementContainerFactionPatch), nameof(IsEffectApplied), [typeof(ElementContainerFaction)])),
            new CodeInstruction(OpCodes.Brtrue, label1)
        );

        // ldarg.0 NULL
        // ldc.i4.1 NULL
        // stfld bool ElementContainerFaction::isDirty
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldc_I4_1),
            new CodeMatch(OpCodes.Stfld, AccessTools.Field(typeof(ElementContainerFaction), nameof(ElementContainerFaction.isDirty)))
        );
        // エンチャントの適用フラグを無効にする処理を追加する
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ElementContainerFactionPatch), nameof(UnsetEffectApplied), [typeof(ElementContainerFaction)]))
        );

        return matcher.InstructionEnumeration();
    }

    private static bool IsEffectApplied(ElementContainerFaction instance)
    {
        if (!ECFExtraData.TryGetValue(instance, out var extraData))
        {
            Plugin.LogInfo($"IsEffectApplied false");
            return false;
        }
        Plugin.LogInfo($"IsEffectApplied {extraData.IsEffectApplied}");
        return extraData.IsEffectApplied;
    }

    private static void SetEffectApplied(ElementContainerFaction instance)
    {
        UpdateEffectApplied(instance, true);
    }

    private static void UnsetEffectApplied(ElementContainerFaction instance)
    {
        UpdateEffectApplied(instance, false);
    }

    private static void UpdateEffectApplied(ElementContainerFaction instance, bool value)
    {
        Plugin.LogInfo($"UpdateEffectApplied {value}");
        if (!ECFExtraData.TryGetValue(instance, out var extraData))
        {
            extraData = new();
            ECFExtraData.Add(instance, extraData);
        }
        extraData.IsEffectApplied = value;
    }
}