using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;

namespace NPCGotoBehaviorTweaks.Patches;

[HarmonyPatch(typeof(AI_Goto))]
public static class AI_GotoPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(AI_Goto.TryGoTo), [])]
    internal static IEnumerable<CodeInstruction> TryGoTo_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // if (shared.HasChara && !owner.IsPC) {
        // ...
        // }
        // // 変更後
        // if (shared.HasChara && !owner.IsPC && false) {
        // ...
        // }
        var matcher = new CodeMatcher(instructions, generator);

        // callvirt bool Point::get_HasChara()
        // brfalse Label27
        // ldarg.0 NULL
        // ldfld Chara AIAct::owner
        // callvirt virtual bool Card::get_IsPC()
        // brtrue Label28
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Point), nameof(Point.HasChara))),
            new CodeMatch(OpCodes.Brfalse),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld),
            new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Card), nameof(Card.IsPC))),
            new CodeMatch(OpCodes.Brtrue)
        );
        // ロジック追加の場所の基準となる brtrue Label28 の位置を保存する
        var start = matcher.Pos;

        // call static int EClass::rnd(int a)
        // brfalse Label30
        // ldc.i4.0 NULL [Label29]
        // ret NULL
        // ldarg.0 NULL [Label27, Label28, Label30]
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(EClass), nameof(EClass.rnd), [typeof(int)])),
            new CodeMatch(OpCodes.Brfalse),
            new CodeMatch(OpCodes.Ldc_I4_0),
            new CodeMatch(OpCodes.Ret),
            new CodeMatch(OpCodes.Ldarg_0)
        );
        // 待機しない場合の遷移先となるLabelMod1を生成する
        matcher.CreateLabel(out var label1);
        // 常に待機しないような条件を追加する
        matcher.Advance(start - matcher.Pos + 1);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Br, label1)
        );

        return matcher.InstructionEnumeration();
    }
}
