using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;
using Macchacoffee.ElinMods.ModUtility.Logging;

using Macchacoffee.ElinMods.ModUtility.Patch;
using Macchacoffee.ElinMods.StarweaversMoongateQuickFix.Mod;

namespace Macchacoffee.ElinMods.StarweaversMoongateQuickFix.Patches;

[HarmonyPatch(typeof(TraitMoongateEx))]
internal static class TraitMoongateExPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(TraitMoongateEx._OnUse), [])]
    private static IEnumerable<CodeInstruction> _OnUse_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator)
    {
        // // 変更前
        // layer = EClass.ui.AddLayer<LayerList>().SetList2(list, ...);
        // // 変更後
        // layer = MoongatePaging.SetList2(EClass.ui.AddLayer<LayerList>(), list, ...);
        var targetMethod = AccessTools.DeclaredMethod(
            typeof(LayerList),
            nameof(LayerList.SetList2),
            generics: [typeof(MapMetaData)])
            ?? throw new MissingMethodException(typeof(LayerList).FullName, nameof(LayerList.SetList2));
        var replacementMethod = AccessTools.Method(typeof(MoongatePaging), nameof(MoongatePaging.SetList2))
            ?? throw new MissingMethodException(typeof(MoongatePaging).FullName, nameof(MoongatePaging.SetList2));
        var matcher = new CodeMatcher(instructions, generator);

        // callvirt instance LayerList LayerList::SetList2<MapMetaData>(...)
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Callvirt, targetMethod)
        ).ThrowIfInvalid("Could not find TraitMoongateEx._OnUse's call to LayerList.SetList2<MapMetaData>.");
        // ムーンゲートのリスト生成だけをページング用の処理に置き換える。
        matcher.Opcode = OpCodes.Call;
        matcher.Operand = replacementMethod;

        return matcher.InstructionEnumeration();
    }
}
