using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;

namespace EnableDiningSpotSignInTent.Patches;

[HarmonyPatch(typeof(Chara))]
public static class CharaPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Chara.CanReplace), [typeof(Chara)])]
    internal static IEnumerable<CodeInstruction> CanReplace_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // if (IsPCFaction && !c.IsPCParty)
        // {
        //     return true;
        // }
        // // 変更後
        // if (IsPCFaction && (!c.IsPCParty || (EClass._zone is Zone_Tent && HasAIGotoForEat(this) && !HasAIGotoForEat(c))))
        // {
        //     return true;
        // }
        var matcher = new CodeMatcher(instructions, generator);

        // callvirt virtual bool Card::get_IsPCFaction()
        // brfalse Label21
        // ldarg.1 NULL
        // callvirt virtual bool Card::get_IsPCParty()
        // brtrue Label22
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Card), nameof(Card.IsPCFaction))),
            new CodeMatch(OpCodes.Brfalse),
            new CodeMatch(OpCodes.Ldarg_1),
            new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Card), nameof(Card.IsPCParty))),
            new CodeMatch(OpCodes.Brtrue)
        );
        // テント内かつ食事目的で移動するAIである場合、押しのけ可能とするロジックを追加する
        matcher.Opcode = OpCodes.Brfalse;
        matcher.CreateLabelWithOffsets(1, out var label1);
        matcher.CreateLabelWithOffsets(3, out var label2);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Brfalse, label1),
            new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(EClass), nameof(EClass._zone))),
            new CodeInstruction(OpCodes.Isinst, typeof(Zone_Tent)),
            new CodeInstruction(OpCodes.Brfalse, label2),
            new CodeInstruction(OpCodes.Ldarg_0),
            CodeInstruction.Call(() => HasAIGotoForEat(default!))
        );

        return matcher.InstructionEnumeration();
    }

    private static bool HasAIGotoForEat(Chara chara)
    {
        // 特定のAI階層にAI_EatとAI_Gotoが存在する場合、食事目的で移動しているとみなす
        AI_Eat? aiEat;
        var ai = chara.ai;
        while (true)
        {
            aiEat = ai as AI_Eat;
            if (aiEat is not null || !ai.IsChildRunning)
            {
                break;
            }
            ai = ai.child;
        }
        return aiEat is not null && aiEat.child is AI_Goto;
    }
}
