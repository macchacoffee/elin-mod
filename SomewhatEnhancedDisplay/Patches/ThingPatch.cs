using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;
using SomewhatEnhancedDisplay.Config;
using SomewhatEnhancedDisplay.Extensions;
using SomewhatEnhancedDisplay.UI;
using SomewhatEnhancedDisplay.UI.HoverGuide;

namespace SomewhatEnhancedDisplay.Patches;

[HarmonyPatch(typeof(Thing))]
internal static class ThingPatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original) && ModContext.Config.EnableHoverGuide.Value;
    }

    private static ModConfigHoverGuide Config => ModContext.WorldConfig.HoverGuide;
    private static ModConfigHoverGuideStyleThing StyleConfig => Config.CurrentStyle.Thing;

    private sealed class HoverTextContext(Thing? previousTarget)
    {
        public Thing? PreviousTarget { get; } = previousTarget;
        public bool Restored { get; set; } = false;
    }

    private static Thing? _hoverTextTarget;

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Thing.GetName), [typeof(NameStyle), typeof(int)])]
    private static IEnumerable<CodeInstruction> GetName_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // base.IsIdentified
        // // 変更後
        // ThingPatch.ShouldDisplayAsIdentified(this)
        var matcher = new CodeMatcher(instructions, generator);

        // call Card::get_IsIdentified()
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(Card), nameof(Card.IsIdentified)))
        );
        // 鑑定済みを判定する条件でModの設定を参照するように変更する。
        var shouldDisplayAsIdentified = AccessTools.Method(typeof(ThingPatch), nameof(ShouldDisplayAsIdentified));
        matcher.Repeat(matchAction: m =>
        {
            m.Opcode = OpCodes.Call;
            m.Operand = shouldDisplayAsIdentified;
            m.Advance(1);
        });

        return matcher.InstructionEnumeration();
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Thing.GetHoverText), [])]
    private static void GetHoverText_Prefix(Thing __instance, out HoverTextContext? __state)
    {
        __state = new(_hoverTextTarget);
        _hoverTextTarget = __instance;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Thing.GetHoverText), [])]
    private static void GetHoverText_Postfix(Thing __instance, HoverTextContext? __state)
    {
        RestoreHoverTextContext(__state);
    }

    [HarmonyFinalizer]
    [HarmonyPatch(nameof(Thing.GetHoverText), [])]
    private static Exception? GetHoverText_Finalizer(Exception? __exception, HoverTextContext? __state)
    {
        RestoreHoverTextContext(__state);
        return __exception;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Thing.GetHoverText), [])]
    private static IEnumerable<CodeInstruction> GetHoverText_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // text = text + " <size=14>(" + Lang._weight(base.ChildrenAndSelfWeight) + ")</size> ";
        // ...
        // if (!hoverText.IsEmpty())
        // {
        //     text = text + Environment.NewLine + hoverText;
        // }
        // ...
        // return base.GetHoverText() + text;
        // // 変更後
        // text = text + " <size=14>(" + Lang._weight(base.ChildrenAndSelfWeight) + ")</size>";
        // ...
        // if (!hoverText.IsEmpty())
        // {
        // }
        // ...
        // return ThingPatch.BuildHoverText(base.GetHoverText(), text, this);
        var matcher = new CodeMatcher(instructions, generator);

        // ldstr ")</size> "
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, ")</size> ")
        );
        // 末尾のスペースを削除する。
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldstr, ")</size>")
        );

        // brtrue <skipTraitText>
        // ldloc.0
        // call static string Environment::get_NewLine()
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Brtrue),
            new CodeMatch(OpCodes.Ldloc_0),
            new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(Environment), nameof(Environment.NewLine)))
        );
        // trait.GetHoverText()の戻り値がtextに追加されないようにする。
        matcher.Advance(1);
        matcher.RemoveInstructions(5);

        // call static string string::Concat(string str0, string str1)
        // ret
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), [typeof(string), typeof(string)])),
            new CodeMatch(OpCodes.Ret)
        );
        // 表示内容の文字列を組み立てる処理を差し替える。
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
             CodeInstruction.Call(() => BuildHoverText(default!, default!, default!))
        );

        return matcher.InstructionEnumeration();
    }

    private static void RestoreHoverTextContext(HoverTextContext? context)
    {
        if (context is null || context.Restored)
        {
            return;
        }
        _hoverTextTarget = context.PreviousTarget;
        context.Restored = true;
    }

    private static bool ShouldDisplayAsIdentified (Thing thing)
    {
        return thing.IsIdentified || (ReferenceEquals(_hoverTextTarget, thing) && StyleConfig.DisplayUnidentifiedItemsAsIdentified);
    }

    private static string IntToString(int value)
    {
        return value.ToString();
    }

    private static int ComputeFontSize(int size)
    {
        // フォントサイズを微調整する。
        return ModUIUtil.ComputeFontSize(size - 1);
    }

    private static string BuildHoverText(string cardText, string text, Thing thing)
    {
        cardText = cardText.TagResize(ComputeFontSize);
        text = text.TagResize(ComputeFontSize);
        return ModThingHoverTextBuilder.BuildHoverText(thing, cardText, text);
    }
}
