using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;
using MoreEffectiveLuck.Game;

namespace MoreEffectiveLuck.Patches;

[HarmonyPatch(typeof(QuestRandom))]
public static class QuestRandomPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(QuestRandom.OnDropReward), [])]
    private static IEnumerable<CodeInstruction> OnDropReward_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // thing = ThingGen.Create("plat").SetNum(GetRewardPlat(num2));
        // // 変更後
        // thing = ThingGen.Create("plat").SetNum(QuestRandomPatch.GetRewardPlatForGetEnchant(num2));
        var matcher = new CodeMatcher(instructions, generator);

        // プラチナ硬貨の数をダイスロールして決定する処理に差し替える
        // callvirt virtual int Quest::GetRewardPlat(int money)
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Quest), nameof(Quest.GetRewardPlat), [typeof(int)]))
        );
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(QuestRandomPatch), nameof(GetRewardPlatForGetEnchant), [typeof(Quest), typeof(int)]))
        );

        return matcher.InstructionEnumeration();
    }

    private static int GetRewardPlatForGetEnchant(Quest quest, int money)
    {
        var dice = LuckDice<int?>.Create(
            resultFunc: () => quest.GetRewardPlat(money),
            resultCompareFunc: (result, prev) => result > prev,
            card: EClass.pc
        );
        return dice.Roll().Value;
    }
}