using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;

namespace EnableDiningBoardInTent.Patches;

[HarmonyPatch(typeof(FoodEffect))]
public static class FoodEffectPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(FoodEffect.Proc), [typeof(Chara), typeof(Thing), typeof(bool)])]
    internal static IEnumerable<CodeInstruction> Proc_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // bool flag = EClass._zone.IsPCFaction && c2.IsInSpot<TraitSpotDining>();
        // // 変更後
        // bool flag = EClass._zone.IsPCFactionOrTent && c2.IsInSpot<TraitSpotDining>();
        var matcher = new CodeMatcher(instructions, generator);

        // callvirt void Card::CheckJustCooked()
        // call static Zone EClass::get__zone()
        // callvirt bool Zone::get_IsPCFaction()
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Card), nameof(Card.CheckJustCooked))),
            new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(EClass), nameof(EClass._zone))),
            new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Zone), nameof(Zone.IsPCFaction)))
        );
        // callvirt bool Zone::get_IsPCFaction() の get_IsPCFaction を get_IsPCFactionOrTent に置き換え、
        // PCの拠点内またはテント内である場合にtrueとなるようにする
        // これにより、テント内でも食堂の立札による食事効果上昇効果が適用されるようにする
        matcher.Operand = AccessTools.PropertyGetter(typeof(Zone), nameof(Zone.IsPCFactionOrTent));

        return matcher.InstructionEnumeration();
    }
}
