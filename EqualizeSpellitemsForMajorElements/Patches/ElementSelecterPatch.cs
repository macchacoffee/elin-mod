using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;

namespace EqualizeSpellitemsForMajorElements.Patches;

[HarmonyPatch(typeof(ElementSelecter))]
public static class ElementSelecterPatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    private static readonly List<string> _elementTags;

    static ElementSelecterPatch()
    {
        if (EClass.core.version.IsBelow(new Version { minor = 23, batch = 330 }))
        {
            _elementTags = ["hand", "arrow", "bolt", "ball", "miasma", "funnel", "weapon", "sword"];
        }
        else
        {
            _elementTags = ["hand", "arrow", "bolt", "ball", "miasma", "funnel", "weapon", "sword", "flare", "comet"];
        }
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(ElementSelecter.Select), [typeof(int)])]
    private static IEnumerable<CodeInstruction> Select_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // if ((useDomain && !EClass.player.domains.Contains(row.id) && EClass.rnd(10) > 1) || !row.tag.Contains(item.alias.Split('_', StringSplitOptions.None)[0]))
        // {
        // // 変更後
        // if ((useDomain && !EClass.player.domains.Contains(row.id) && EClass.rnd(10) > 1) || !ElementSelecterPatch.ContainsTag(row, item.alias.Split('_', StringSplitOptions.None)[0]))
        // {
        var matcher = new CodeMatcher(instructions, generator);

        // ldfld string[] SourceElement+Row::tag
        // ldloc.2 NULL
        // ldfld string SourceElement+Row::alias
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(SourceElement.Row), nameof(SourceElement.Row.tag))),
            new CodeMatch(OpCodes.Ldloc_2),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(SourceElement.Row), nameof(SourceElement.Row.alias)))
        );
        // SourceElement.Rowのタグを取得しないようにする
        matcher.RemoveInstruction();

        // callvirt string[] string::Split(char separator, StringSplitOptions options)
        // ldc.i4.0 NULL
        // ldelem.ref NULL
        // call static bool ClassExtension::Contains(string[] strs, string id)
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(string), nameof(string.Split), [typeof(char), typeof(StringSplitOptions)])),
            new CodeMatch(OpCodes.Ldc_I4_0),
            new CodeMatch(OpCodes.Ldelem_Ref),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ClassExtension), nameof(ClassExtension.Contains), [typeof(string[]), typeof(string)]))
        );
        // 属性のSourceElement.Rowのタグに指定された文字列が含まれていないかを判定する処理を差し替える
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ElementSelecterPatch), nameof(ContainsTag), [typeof(SourceElement.Row), typeof(string)]))
        );

        return matcher.InstructionEnumeration();
    }

    private static bool ContainsTag(SourceElement.Row row, string alias)
    {
        var tag = row.tag;
        if (row.categorySub == "eleAttack" && row.id != SKILL.eleImpact && row.id != SKILL.eleVoid)
        {
            // 衝撃、無以外の属性の場合は各種魔法をタグに追加する
            tag = [.. tag.Union(_elementTags)];
        }

        return tag.Contains(alias);
    }
}
