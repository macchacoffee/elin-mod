using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;
using SomewhatEnhancedDisplay.UI.HoverGuide;
using UnityEngine;

namespace SomewhatEnhancedDisplay.Patches;

[HarmonyPatch(typeof(WidgetMouseover))]
public static class WidgetMouseoverPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    private static ModHoverGuide? HoverGuide { get; set; }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(WidgetMouseover.OnActivate), [])]
    private static void OnActivate_Postfix(WidgetMouseover __instance)
    {
        HoverGuide = new(__instance);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(WidgetMouseover.Refresh), [])]
    private static IEnumerable<CodeInstruction> Refresh_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // if (flag)
        // {
        // ...
        // text += EMono.pc.ride.GetHoverText2();
        // ...
        // if (EMono.pc.ride != null)
        // {
        //     text += Environment.NewLine;
        // }
        // ...
        // text += EMono.pc.parasite.GetHoverText();
        // text += EMono.pc.parasite.GetHoverText2();
        // ...
        // text += "otherCards".lang((count - 1).ToString() ?? "");
        // ...
        // text += card.GetHoverText2();
        // ...
        // text = text + Environment.NewLine + mouseTarget.target.InspectName;
        // ...
        // Show(text);
        // // 変更後
        // string? localText2;
        // string? localText3;
        // string? localText4;
        // Card? localTarget1;
        // Card? localTarget2;
        // if (flag)
        // {
        // ...
        // localText2 += EMono.pc.ride.GetHoverText2();
        // localTarget1 = EMono.pc.ride;
        // ...
        // if (EMono.pc.ride != null)
        // {
        // }
        // ...
        // localText3 = EMono.pc.parasite.GetHoverText();
        // localText4 = EMono.pc.parasite.GetHoverText2();
        // localTarget2 = EMono.pc.parasite;
        // ...
        // text = ModCharaHoverTextBuilder.BuildOtherCardsText(text, "otherCards".lang((count - 1).ToString() ?? ""));
        // ...
        // localText2 = card.GetHoverText2();
        // localTarget1 = card;
        // ...
        // localText2 = localText2 + Environment.NewLine + mouseTarget.target.InspectName;
        // localText2 = localText2 + Environment.NewLine + mouseTarget.target.InspectName;
        // ...
        // WidgetMouseoverPatch.ShowHoverGuide(this, text, localText2, localText3, localText4, localTarget1, localTarget2);
        var matcher = new CodeMatcher(instructions, generator);

        // ホバーテキスト下部 (おおよそ2行目以降) に表示される文字列、
        // GetHoverTextとGetHoverText2()を呼び出したインスタンスの参照を保存する変数を定義する
        var localText2 = generator.DeclareLocal(typeof(string));
        var localText3 = generator.DeclareLocal(typeof(string));
        var localText4 = generator.DeclareLocal(typeof(string));
        var localTarget1 = generator.DeclareLocal(typeof(Card));
        var localTarget2 = generator.DeclareLocal(typeof(Card));

        // ldfld Chara Chara::ride
        // callvirt virtual string Card::GetHoverText2()
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Chara), nameof(Chara.ride))),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Card), nameof(Card.GetHoverText2), []))
        );
        // GetHoverText2()の戻り値と呼び出したインスタンスの参照 (騎乗Chara) を保存する
        // GetHoverText2()の戻り値がtextに追加されないようにする
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Dup)
        );
        matcher.Advance(1);
        matcher.RemoveInstructions(2);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Stloc_S, localText2),
            new CodeInstruction(OpCodes.Stloc_S, localTarget1),
            new CodeInstruction(OpCodes.Pop)
        );

        // ldloc.2 NULL
        // Environment::get_NewLine()
        // call static string string::Concat(string str0, string str1)
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldloc_2),
            new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(Environment), nameof(Environment.NewLine))),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), [typeof(string), typeof(string)]))
        );
        // textに改行が追加されないようにする
        matcher.RemoveInstructions(4);

        // ldfld Chara Chara::parasite
        // callvirt virtual string Card::GetHoverText()
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Chara), nameof(Chara.parasite))),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Card), nameof(Card.GetHoverText), []))
        );
        // GetHoverText()の戻り値を保存し、textに追加されないようにする
        matcher.Advance(1);
        matcher.RemoveInstructions(2);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Stloc_S, localText3),
            new CodeInstruction(OpCodes.Pop)
        );

        // ldfld Chara Chara::parasite
        // callvirt virtual string Card::GetHoverText2()
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Chara), nameof(Chara.parasite))),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Card), nameof(Card.GetHoverText2), []))
        );
        // GetHoverText2()の戻り値と呼び出したインスタンスの参照 (寄生Chara) を保存する
        // GetHoverText2()の戻り値がtextに追加されないようにする
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Dup)
        );
        matcher.Advance(1);
        matcher.RemoveInstructions(2);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Stloc_S, localText4),
            new CodeInstruction(OpCodes.Stloc_S, localTarget2),
            new CodeInstruction(OpCodes.Pop)
        );

        // call static string ClassExtension::lang(string s, string ref1, string ref2, string ref3, string ref4, string ref5)
        // call static string string::Concat(string str0, string str1)
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ClassExtension), nameof(ClassExtension.lang), [typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string)])),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), [typeof(string), typeof(string)]))
        );
        // "(他+n)"の文字列を調整し、GetHoverText()の戻り値の末尾に追加されるようにする
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            CodeInstruction.Call(() => ModCharaHoverTextBuilder.BuildOtherCardsText(default!, default!))
        );

        // ldloc.3 NULL [Label21, Label22]
        // callvirt virtual string Card::GetHoverText2()
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldloc_3),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Card), nameof(Card.GetHoverText2), []))
        );
        // GetHoverText2()の戻り値と呼び出したインスタンスの参照 (card) を保存する
        // GetHoverText2()の戻り値がtextに追加されないようにする
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Dup)
        );
        matcher.Advance(1);
        matcher.RemoveInstructions(2);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Stloc_S, localText2),
            new CodeInstruction(OpCodes.Stloc_S, localTarget1),
            new CodeInstruction(OpCodes.Pop)
        );

        // ldloc.2 NULL
        // call static string Environment::get_NewLine()
        // ldloc.0 NULL
        // ldfld IInspect PointTarget::target
        // callvirt abstract virtual string IInspect::get_InspectName()
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldloc_2),
            new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(Environment), nameof(Environment.NewLine))),
            new CodeMatch(OpCodes.Ldloc_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(PointTarget), nameof(PointTarget.target))),
            new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(IInspect), nameof(IInspect.InspectName)))
        );
        // InspectNameを保存し、textには追加されないようにする
        matcher.RemoveInstructions(2);
        matcher.Advance(3);
        matcher.RemoveInstructions(2);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Stloc_S, localText3)
        );

        // call void WidgetMouseover::Show(string s)
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(WidgetMouseover), nameof(WidgetMouseover.Show), [typeof(string)]))
        );
        // Modで追加したUIを更新する
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_S, localText2),
            new CodeInstruction(OpCodes.Ldloc_S, localText3),
            new CodeInstruction(OpCodes.Ldloc_S, localText4),
            new CodeInstruction(OpCodes.Ldloc_S, localTarget1),
            new CodeInstruction(OpCodes.Ldloc_S, localTarget2),
            CodeInstruction.Call(() => ShowHoverGuide(default!, default!, default!, default!, default!, default!, default!))
        );

        return matcher.InstructionEnumeration();
    }


    [HarmonyTranspiler]
    [HarmonyPatch(nameof(WidgetMouseover.Show), [typeof(string)])]
    private static IEnumerable<CodeInstruction> Show_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // switch (this.Rect().GetAnchor())
        // {
        // // 変更後
        // switch (RectPosition.CENTER)
        // {
        var matcher = new CodeMatcher(instructions, generator);

        // call static RectPosition ClassExtension::GetAnchor(UnityEngine.RectTransform _rect)
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ClassExtension), nameof(ClassExtension.GetAnchor), [typeof(RectTransform)]))
        );
        // GetAnchor()の戻り値を固定値のRectPosition.CENTERに置き換え、
        // layout.childAlignmentがTextAnchor.MiddleCenterに設定されるようにする
        matcher.Advance(1);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Pop),
            new CodeInstruction(OpCodes.Ldc_I4_5)
        );

        return matcher.InstructionEnumeration();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(WidgetMouseover.Show), [typeof(string)])]
    private static void Show_Postfix(WidgetMouseover __instance, ref string s)
    {
        // ホバーテキストの初期位置において、
        // 1キャラクター分の全ての情報が画面内に収まるぐらいにpivotを調整する
        __instance.layout.Rect().pivot = new(0.5f, 0.8f);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(WidgetMouseover.OnManagerActivate), [])]
    private static void OnManagerActivate_Postfix(WidgetMouseover __instance)
    {
        HoverGuide!.ShowForManager(__instance);
    }

    private static void ShowHoverGuide(WidgetMouseover widget, string? text, string? text2, string? text3, string? text4, Card? target1, Card? target2)
    {
        HoverGuide!.Show(widget, text, text2, text3, text4, target1, target2);
    }
}
