using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Macchacoffee.ElinMods.AdditionalUniqueNPC.Mod;
using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.AdditionalUniqueNPC.Patches;

[HarmonyPatch(typeof(ActTelekinesis))]
internal static class ActTelekinesisPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(ActTelekinesis.DoDamage), [typeof(Card), typeof(int), typeof(int)])]
    private static IEnumerable<CodeInstruction> DoDamage_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator)
    {
        // // 変更前
        // c.DamageHP(dmg, ele, eleP, attackSource, origin, showEffect, weapon, originalTarget, penetration);
        // // 変更後
        // ManaAbsorption.DamageHP(c, dmg, ele, eleP, attackSource, origin, showEffect, weapon, originalTarget, penetration, true);
        var matcher = new CodeMatcher(instructions, generator);

        // callvirt void Card::DamageHP(long, int, int, AttackSource, Card, bool, Thing, Chara, int)
        matcher.MatchStartForward(
            new CodeMatch(
                OpCodes.Callvirt,
                AccessTools.Method(
                    typeof(Card),
                    nameof(Card.DamageHP),
                    [
                        typeof(long),
                        typeof(int),
                        typeof(int),
                        typeof(AttackSource),
                        typeof(Card),
                        typeof(bool),
                        typeof(Thing),
                        typeof(Chara),
                        typeof(int)
                    ]))
        ).ThrowIfInvalid("Could not find ActTelekinesis.DoDamage's DamageHP call.");

        var replacements = 0;
        matcher.Repeat(matchAction: m =>
        {
            var labels = m.Labels.Copy();
            m.Labels.Clear();

            if (m.Instruction.blocks.Count != 0)
            {
                throw new InvalidOperationException("DamageHP call is on an unexpected exception boundary.");
            }

            m.Set(
                OpCodes.Call,
                CodeInstruction.Call(() => ManaAbsorption.DamageHP(
                    default!,
                    default,
                    default,
                    default,
                    default,
                    default!,
                    default,
                    default!,
                    default!,
                    default,
                    default
                )).operand
            );
            m.Insert(new CodeInstruction(OpCodes.Ldc_I4_1));
            m.AddLabels(labels);
            m.Advance(2);
            replacements++;
        });

        if (replacements != 1)
        {
            throw new InvalidOperationException(
                $"Expected 1 DamageHP call in ActTelekinesis.DoDamage, found {replacements}.");
        }

        return matcher.InstructionEnumeration();
    }
}
