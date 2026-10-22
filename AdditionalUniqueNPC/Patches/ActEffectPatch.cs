using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Macchacoffee.ElinMods.AdditionalUniqueNPC.Mod;
using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.AdditionalUniqueNPC.Patches;

[HarmonyPatch(typeof(ActEffect))]
internal static class ActEffectPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(
        nameof(ActEffect.DamageEle),
        [
            typeof(Card),
            typeof(EffectId),
            typeof(int),
            typeof(Element),
            typeof(List<Point>),
            typeof(ActRef),
            typeof(string),
            typeof(Point)
        ])]
    private static IEnumerable<CodeInstruction> DamageEle_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator)
    {
        // // 変更前
        // c.DamageHP(dmg, ele, eleP, attackSource, origin, showEffect, weapon, originalTarget, penetration);
        // // 変更後
        // ManaAbsorption.DamageHP(c, dmg, ele, eleP, attackSource, origin, showEffect, weapon, originalTarget, penetration, ManaAbsorption.IsMagicSource(actref));
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
        ).ThrowIfInvalid("Could not find ActEffect.DamageEle's DamageHP call.");

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
            m.Insert(
                new CodeInstruction(OpCodes.Ldarg_S, (byte)5),
                CodeInstruction.Call(() => ManaAbsorption.IsMagicSource(default))
            );
            m.AddLabels(labels);
            m.Advance(3);
            replacements++;
        });

        if (replacements != 1)
        {
            throw new InvalidOperationException(
                $"Expected 1 DamageHP call in ActEffect.DamageEle, found {replacements}.");
        }

        return matcher.InstructionEnumeration();
    }

    [HarmonyTranspiler]
    [HarmonyPatch(
        nameof(ActEffect.Proc),
        [
            typeof(EffectId),
            typeof(int),
            typeof(BlessedState),
            typeof(Card),
            typeof(Card),
            typeof(ActRef)
        ])]
    private static IEnumerable<CodeInstruction> Proc_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator)
    {
        // // 変更前
        // target.DamageHP(dmg, ele, eleP, attackSource, origin, showEffect, weapon, originalTarget, penetration);
        // target.DamageHP(dmg, attackSource, origin);
        // // 変更後
        // ManaAbsorption.DamageHP(target, dmg, ele, eleP, attackSource, origin,
        //     showEffect, weapon, originalTarget, penetration, ManaAbsorption.IsMagicSource(actRef));
        // ManaAbsorption.DamageHP(target, dmg, attackSource, origin, ManaAbsorption.IsMagicSource(actRef));
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
        ).ThrowIfInvalid("Could not find ActEffect.Proc's elemental DamageHP calls.");

        var elementalReplacements = 0;
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
            m.Insert(
                new CodeInstruction(OpCodes.Ldarg_S, (byte)5),
                CodeInstruction.Call(() => ManaAbsorption.IsMagicSource(default))
            );
            m.AddLabels(labels);
            m.Advance(3);
            elementalReplacements++;
        });

        if (elementalReplacements != 4)
        {
            throw new InvalidOperationException(
                $"Expected 4 elemental DamageHP calls in ActEffect.Proc, found {elementalReplacements}.");
        }

        // callvirt void Card::DamageHP(long, AttackSource, Card)
        matcher.Start().MatchStartForward(
            new CodeMatch(
                OpCodes.Callvirt,
                AccessTools.Method(
                    typeof(Card),
                    nameof(Card.DamageHP),
                    [typeof(long), typeof(AttackSource), typeof(Card)]))
        ).ThrowIfInvalid("Could not find ActEffect.Proc's non-elemental DamageHP call.");

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
                    default!, default, default, default!, default)).operand
            );
            m.Insert(
                new CodeInstruction(OpCodes.Ldarg_S, (byte)5),
                CodeInstruction.Call(() => ManaAbsorption.IsMagicSource(default))
            );
            m.AddLabels(labels);
            m.Advance(3);
            replacements++;
        });

        if (replacements != 1)
        {
            throw new InvalidOperationException(
                $"Expected 1 non-elemental DamageHP call in ActEffect.Proc, found {replacements}.");
        }

        return matcher.InstructionEnumeration();
    }
}
