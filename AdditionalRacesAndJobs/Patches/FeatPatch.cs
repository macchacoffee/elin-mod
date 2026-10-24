using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.AdditionalRacesAndJobs.Patches;

[HarmonyPatch(typeof(Feat))]
internal static class FeatPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    private static readonly int[] MonkNonMartialPotentialSkillIds = [
        SKILL.weaponSword,
        SKILL.weaponAxe,
        SKILL.weaponStaff,
        SKILL.weaponBow,
        SKILL.weaponGun,
        SKILL.weaponPolearm,
        SKILL.weaponDagger,
        SKILL.weaponCrossbow,
        SKILL.weaponScythe,
        SKILL.weaponBlunt,
        SKILL.shield,
    ];

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Feat.Apply), [typeof(int), typeof(ElementContainer), typeof(bool)])]
    private static IEnumerable<CodeInstruction> Apply_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator)
    {
        // // 変更前
        // var text = source.GetText("textExtra");
        // // 変更後
        // SetMonkHintReferences(this);
        // var text = source.GetText("textExtra");
        var matcher = new CodeMatcher(instructions, generator);

        // ldarg.0
        // call SourceElement.Row Element::get_source()
        // ldstr "textExtra"
        // ldc.i4.0
        // callvirt string SourceData.BaseRow::GetText(string, bool)
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(Element), nameof(Element.source))),
            new CodeMatch(OpCodes.Ldstr, "textExtra"),
            new CodeMatch(OpCodes.Ldc_I4_0),
            new CodeMatch(
                OpCodes.Callvirt,
                AccessTools.Method(typeof(SourceData.BaseRow), nameof(SourceData.BaseRow.GetText), [typeof(string), typeof(bool)]))
        ).ThrowIfInvalid("Could not find Feat.Apply's extra description block.");

        var labels = matcher.Labels.Copy();
        matcher.Labels.Clear();
        matcher.Insert(
            new CodeInstruction(OpCodes.Ldarg_0),
            CodeInstruction.Call(() => SetMonkHintReferences(default!))
        );
        matcher.AddLabels(labels);

        return matcher.InstructionEnumeration();
    }

    private static void SetMonkHintReferences(Feat feat)
    {
        if (feat.id != ModConsts.SourceNumericId.FeatMonk)
        {
            return;
        }

        var level = feat.Value;
        var format = "+#;-#;+0";
        Feat.featRef[0] = (level * ModConsts.Value.MonkMartialPotential).ToString(format);
        Feat.featRef[1] = (level * ModConsts.Value.MonkNonMartialPotential).ToString(format);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Feat.Apply), [typeof(int), typeof(ElementContainer), typeof(bool)])]
    private static void Apply_Postfix(Feat __instance, int a, ElementContainer owner, bool hint)
    {
        if (__instance.id != ModConsts.SourceNumericId.FeatMonk || hint || owner.Chara == null)
        {
            return;
        }

        owner.ModPotential(SKILL.martial, a * ModConsts.Value.MonkMartialPotential);

        foreach (var skillId in MonkNonMartialPotentialSkillIds)
        {
            owner.ModPotential(skillId, a * ModConsts.Value.MonkNonMartialPotential);
        }
    }
}
