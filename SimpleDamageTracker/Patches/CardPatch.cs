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
        // else
        // {
        //     hp -= (int)dmg;
        // }

        // if (isSynced && dmg != 0L)
        // {
        // // 変更後
        // else
        // {
        //     hp -= (int)dmg;
        // }
        //
        // CardPatch.OnDamage(this, origin, dmg);
        //
        // if (isSynced && dmg != 0L)
        // {
        var matcher = new CodeMatcher(instructions, generator);

        // ldfld long Card+<>c__DisplayClass835_0::dmg
        // conv.i4 NULL
        // sub NULL
        // call void Card::set_hp(int value)
        // ldarg.0 NULL [Label155]
        // callvirt virtual bool Card::get_isSynced()
        // brfalse Label156
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldfld),
            new CodeMatch(OpCodes.Conv_I4),
            new CodeMatch(OpCodes.Sub),
            new CodeMatch(OpCodes.Call, AccessTools.PropertySetter(typeof(Card), nameof(Card.hp))),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Card), nameof(Card.isSynced))),
            new CodeMatch(OpCodes.Brfalse)
        );
        // 
        var dmgOperand = matcher.Operand;
        matcher.Advance(4);
        var labelList1 = matcher.Labels.Copy();
        matcher.Labels.Clear();
        matcher.Insert(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldarg_S, 5),
            new CodeInstruction(OpCodes.Ldloc_0),
            new CodeInstruction(OpCodes.Ldfld, dmgOperand),
            CodeInstruction.Call(() => RecordDamage(default!, default!, default))
        );
        matcher.AddLabels(labelList1);

        return matcher.InstructionEnumeration();
    }

    private static void RecordDamage(Card card, Card origin, long dmg)
    {
        if (origin?.Chara is not Chara originChara || !originChara.IsPCParty)
        {
            return;
        }
        ModContext.DamageTracker.AddDamage(originChara.uid, dmg);
    }
}
