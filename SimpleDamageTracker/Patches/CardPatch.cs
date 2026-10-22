using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.SimpleDamageTracker.Patches;

[HarmonyPatch(typeof(Card))]
internal static class CardPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(
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
        ])]
    private static IEnumerable<CodeInstruction> DamageHP_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator)
    {
        // // 変更前
        // if (isSynced && dmg != 0L)
        // // 変更後
        // CardPatch.RecordDamage(this, origin, dmg);
        // if (isSynced && dmg != 0L)
        var matcher = new CodeMatcher(instructions, generator);

        // ldarg.0
        // callvirt bool Card::get_isSynced()
        // brfalse <skipDisplay>
        // ldloc <damageClosure>
        // ldfld long <damageClosure>::dmg
        // brfalse <skipDisplay>
        var pattern = new[]
        {
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Card), nameof(Card.isSynced))),
            new CodeMatch(code => code.opcode == OpCodes.Brfalse || code.opcode == OpCodes.Brfalse_S),
            new CodeMatch(code => code.IsLdloc()),
            new CodeMatch(code =>
                code.opcode == OpCodes.Ldfld &&
                code.operand is FieldInfo field &&
                field.Name == "dmg" &&
                field.FieldType == typeof(long)),
            new CodeMatch(code => code.opcode == OpCodes.Brfalse || code.opcode == OpCodes.Brfalse_S)
        };

        matcher.MatchStartForward(pattern)
            .ThrowIfInvalid("Could not find Card.DamageHP's damage display block.");

        if (matcher.Clone().Advance(1).MatchStartForward(pattern).IsValid)
        {
            throw new InvalidOperationException("Card.DamageHP has multiple damage display blocks.");
        }

        if (matcher.InstructionAt(2).operand is not Label syncSkip ||
            matcher.InstructionAt(5).operand is not Label damageSkip ||
            matcher.Clone().Advance(6).MatchStartForward(
                new CodeMatch(code =>
                    code.labels.Contains(syncSkip) &&
                    code.labels.Contains(damageSkip))
            ).IsInvalid)
        {
            throw new InvalidOperationException("Card.DamageHP's display branch targets changed.");
        }

        if (matcher.Instruction.blocks.Count != 0)
        {
            throw new InvalidOperationException("Card.DamageHP's display block is on an unexpected exception boundary.");
        }

        var loadClosure = matcher.InstructionAt(3);
        var dmgField = (FieldInfo)matcher.InstructionAt(4).operand;

        // ldloc <damageClosure>
        // ldarg.1
        // stfld long <damageClosure>::dmg
        var capture = matcher.Clone().Start().MatchStartForward(
            new CodeMatch(code => code.opcode == loadClosure.opcode && Equals(code.operand, loadClosure.operand)),
            new CodeMatch(OpCodes.Ldarg_1),
            new CodeMatch(OpCodes.Stfld, dmgField)
        ).ThrowIfInvalid("Could not find Card.DamageHP's captured dmg argument.");

        if (capture.Pos >= matcher.Pos)
        {
            throw new InvalidOperationException(
                "Card.DamageHP's dmg argument must be captured before damage is applied.");
        }

        var labels = matcher.Labels.Copy();
        matcher.Labels.Clear();
        matcher.Insert(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldarg_S, (byte)5),
            new CodeInstruction(loadClosure.opcode, loadClosure.operand),
            new CodeInstruction(OpCodes.Ldfld, dmgField),
            CodeInstruction.Call(() => RecordDamage(default!, default!, default))
        );
        matcher.AddLabels(labels);

        return matcher.InstructionEnumeration();
    }

    private static void RecordDamage(Card card, Card? origin, long dmg)
    {
        if (origin?.Chara is Chara originChara && originChara.IsPCParty)
        {
            ModContext.DamageTracker.AddDamage(originChara.uid, dmg);
        }
        if (card.Chara is Chara targetChara && targetChara.IsPCParty)
        {
            ModContext.DamageTakenTracker.AddDamage(targetChara.uid, dmg);
        }
    }
}
