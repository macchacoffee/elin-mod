using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.EnableDiningSpotSignInTent.Patches;

[HarmonyPatch(typeof(AI_Eat))]
internal static class AI_EatRunPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(MethodType.Enumerator)]
    [HarmonyPatch(nameof(AI_Eat.Run), [])]
    internal static IEnumerable<CodeInstruction> MoveNext_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator)
    {
        // // 変更前
        // if ((EClass._zone.IsPCFaction || EClass.rnd(4) != 0) && !owner.IsPCParty && owner.memberType != FactionMemberType.Livestock && !owner.noMove)
        // {
        //     yield return DoGotoSpot<TraitSpotDining>(base.KeepRunning);
        // }
        // // 変更後
        // if ((EClass._zone.IsPCFactionOrTent || EClass.rnd(4) != 0) && (!owner.IsPCParty || EClass._zone is Zone_Tent) && owner.memberType != FactionMemberType.Livestock && !owner.noMove)
        // {
        //     yield return DoGotoSpot<TraitSpotDining>(base.KeepRunning);
        // }
        var matcher = new CodeMatcher(instructions, generator);

        // callvirt bool Zone::get_IsPCFaction()
        // brtrue Label22
        // ldc.i4.4 NULL
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Zone), nameof(Zone.IsPCFaction))),
            new CodeMatch(OpCodes.Brtrue),
            new CodeMatch(OpCodes.Ldc_I4_4)
        );
        // callvirt bool Zone::get_IsPCFaction() の get_IsPCFaction を get_IsPCFactionOrTent に置き換え、
        // PCの拠点内またはテント内である場合にtrueとなるようにする。
        matcher.Operand = AccessTools.PropertyGetter(typeof(Zone), nameof(Zone.IsPCFactionOrTent));

        // callvirt virtual bool Card::get_IsPCParty()
        // brtrue Label24
        // ldloc.1 NULL
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Card), nameof(Card.IsPCParty))),
            new CodeMatch(OpCodes.Brtrue)
        );
        matcher.Opcode = OpCodes.Call;
        matcher.Operand = AccessTools.Method(typeof(AI_EatRunPatch), nameof(ShouldSkipDiningSpot));

        return matcher.InstructionEnumeration();
    }

    private static bool ShouldSkipDiningSpot(Card owner)
    {
        // ownerがPC、またはPCのパーティかつテント外であれば対象外とする。
        return owner.IsPC || (owner.IsPCParty && EClass._zone is not Zone_Tent);
    }
}
