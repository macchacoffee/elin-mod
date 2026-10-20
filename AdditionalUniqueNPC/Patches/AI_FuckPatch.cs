using System;
using System.Collections.Generic;
using System.Reflection;

using HarmonyLib;

using Macchacoffee.ElinMods.AbilityRestriction;
using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.AdditionalUniqueNPC.Patches;

[HarmonyPatch(typeof(AI_Fuck))]
internal static class AI_FuckPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    private const int _drainManaMultiplier = 20;
    private static readonly HashSet<string> _drainManaCharaIds = [
        ModConsts.SourceId.Misalie,
    ];

    [HarmonyPrefix]
    [HarmonyPatch(nameof(AI_Fuck.Finish), [])]
    private static void Finish_Prefix(AI_Fuck __instance, out int __state)
    {
        // 特定のキャラはイタズラ後に対象からマナ吸収を行うように変更する。
        // イタズラ後に対象が死亡してもマナ吸収は行われる。
        __state = 0;

        if (__instance.Type != AI_Fuck.FuckType.fuck ||
            __instance.variation != AI_Fuck.Variation.NTR ||
            __instance.sell)
        {
            return;
        }

        var attacker = __instance.owner;
        var target = __instance.target;

        if (attacker == null || target == null ||
            !_drainManaCharaIds.Contains(attacker.id)  ||
            attacker == target ||
            !attacker.IsAliveInCurrentZone ||
            !target.IsAliveInCurrentZone)
        {
            return;
        }

         // イタズラの実効レベルと使用者のステータスから威力を計算
        var level = attacker.Evalue(ABILITY.ActNTR);
        var basePower = Math.Max(
            (long)level * 8 + 50,
            Math.Max(
                (long)attacker.LV * 6 + 30,
                (long)attacker.CHA * 4 + 30
            )
        );

        var p = EClass.curve(basePower, 400, 100);
        var e = attacker.MAG;

        // ベースはマナの眼差しのダイス式と同じ。
        var num = Math.Max(1, 2 + p / 200 + e / 100);
        var sides = Math.Max(1, 3 + p / 100);

        // ダイス結果の20倍をMPダメージとする。
        // ただし攻撃側のレベルが低い場合、レベルによる最大MPと同様の補正で威力を下げる。
        var roll = Dice.Roll(num, sides * _drainManaMultiplier, 0, attacker);
        var damage = Math.Max(0L, roll) * Math.Min(attacker.LV, 25) / 25;

        __state = (int)Math.Min(damage, int.MaxValue);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(AI_Fuck.Finish), [])]
    private static void Finish_Postfix(AI_Fuck __instance, int __state)
    {
        if (__state <= 0)
        {
            return;
        }

        var attacker = __instance.owner;
        var target = __instance.target;

        if (attacker == null || target == null ||
            !attacker.IsAliveInCurrentZone)
        {
            return;
        }

        var before = target.mana.value;

        // MPの絶対下限を超えないようにする
        var capacity = Math.Max(
            0L,
            (long)before - target.mana.min
        );
        var damage = (int)Math.Min(__state, capacity);

        target.mana.Mod(-damage);

        var drained = Math.Max(
            0,
            before - target.mana.value
        );
        if (drained > 0)
        {
            attacker.mana.Mod(drained);
            attacker.PlaySound("heal");
            attacker.PlayEffect("heal");
        }
    }
}
