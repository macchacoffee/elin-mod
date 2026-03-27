using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;

namespace EnableDiningSpotSignInTent.Patches;

[HarmonyPatch(typeof(AI_Eat))]
public static class AI_EatPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(AI_Eat.Run), [])]
    internal static IEnumerable<CodeInstruction> Run_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // Runはyield returnのメソッドであるため、パッチを当てたい実装は
        // コンパイラ生成のAI_Eatの内部クラスのメソッドMoveNextに存在する
        // そのためILから内部クラス名を特定し、MoveNextメソッドを取得する
        var matcher = new CodeMatcher(instructions, generator);

        // newobj void AI_Eat+<Run>d__9::.ctor(int <>1__state) で内部クラスのコンストラクタが呼び出される
        // そのコンストラクタから内部クラスとMoveNextメソッドを取得し、パッチを当てる
        matcher.MatchStartForward(new CodeMatch(OpCodes.Newobj));
        var ctor = (matcher.Instruction.operand as ConstructorInfo)!;
        var interClass = ctor.DeclaringType;
        var moveNextMethod = AccessTools.Method(interClass, "MoveNext");
        var transpilerMethod = new HarmonyMethod(typeof(AI_EatRunPatch), nameof(AI_EatRunPatch.MoveNext_Transpiler));
        Plugin.Harmony?.Patch(moveNextMethod, transpiler: transpilerMethod);

        // パッチを当てたいのはこのメソッドではないため、何も変更しない
        return instructions;
    }
}

public static class AI_EatRunPatch
{
    internal static IEnumerable<CodeInstruction> MoveNext_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
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
        // PCの拠点内またはテント内である場合にtrueとなるようにする
        matcher.Operand = AccessTools.PropertyGetter(typeof(Zone), nameof(Zone.IsPCFactionOrTent));

        // callvirt virtual bool Card::get_IsPCParty()
        // brtrue Label24
        // ldloc.1 NULL
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Card), nameof(Card.IsPCParty))),
            new CodeMatch(OpCodes.Brtrue)
        );
        // DoGotoSpotの呼び出しをスキップする遷移に変えるため、brfalseに置き換える
        matcher.Opcode = OpCodes.Brfalse;
        // ownerがPCのパーティでない場合の遷移先となるLabelMod1を生成する
        matcher.CreateLabelWithOffsets(1, out var label1);
        // || EClass._zone is Zone_Tent の処理を追加する
        // ownerがPCのパーティであってもテント内であればtrueとなるようにする
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Brfalse, label1),
            new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(EClass), nameof(EClass._zone))),
            new CodeInstruction(OpCodes.Isinst, typeof(Zone_Tent))
        );

        return matcher.InstructionEnumeration();
    }
}
