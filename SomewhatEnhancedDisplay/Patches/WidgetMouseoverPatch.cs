using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;
using SomewhatEnhancedDisplay.UI;

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

    private static UIText? TextName2 { get; set; } 
    private static UIText? TextName3 { get; set; } 
    private static UIText? TextName4 { get; set; } 
    private static ModHealthBar? HealthBar1 { get; set; }
    private static ModHealthBar? HealthBar2 { get; set; }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(WidgetMouseover.OnActivate), [])]
    private static void OnActivate_Postfix(WidgetMouseover __instance)
    {
        HealthBar1 = new(__instance);

        TextName2 = UnityEngine.Object.Instantiate(__instance.textName);
        TextName2.transform.SetParent(__instance.layout.transform);
        TextName2.transform.localScale = __instance.textName.transform.localScale;

        TextName3 = UnityEngine.Object.Instantiate(__instance.textName);
        TextName3.transform.SetParent(__instance.layout.transform);
        TextName3.transform.localScale = __instance.textName.transform.localScale;

        HealthBar2 = new(__instance);

        TextName4 = UnityEngine.Object.Instantiate(__instance.textName);
        TextName4.transform.SetParent(__instance.layout.transform);
        TextName4.transform.localScale = __instance.textName.transform.localScale;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(WidgetMouseover.Refresh), [])]
    private static IEnumerable<CodeInstruction> Refresh_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // text += "otherCards".lang((count - 1).ToString() ?? "");
        // // 変更後
        // text = WidgetMouseoverPatch.BuildGHoverText(text, "otherCards".lang((count - 1).ToString() ?? ""));
        var matcher = new CodeMatcher(instructions, generator);

        // ホバーテキスト下部 (おおよそ2行目以降) に表示される文字列、
        // GetHoverTextとGetHoverText2関数を呼び出したインスタンスの参照を保存する変数を定義する
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
        // GetHoverText2関数の戻り値と呼び出したインスタンスの参照 (騎乗Chara) を保存する
        // GetHoverText2関数の戻り値がtextに追加されないようにする
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

        // ldfld Chara Chara::parasite
        // callvirt virtual string Card::GetHoverText()
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Chara), nameof(Chara.parasite))),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Card), nameof(Card.GetHoverText), []))
        );
        // GetHoverText関数の戻り値を保存する
        matcher.Advance(1);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Stloc_S, localText3),
            new CodeInstruction(OpCodes.Ldloc_S, localText3)
        );

        // ldfld Chara Chara::parasite
        // callvirt virtual string Card::GetHoverText2()
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Chara), nameof(Chara.parasite))),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Card), nameof(Card.GetHoverText2), []))
        );
        // GetHoverText2関数の戻り値と呼び出したインスタンスの参照 (寄生Chara) を保存する
        // GetHoverText2関数の戻り値がtextに追加されないようにする
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
        // "(他+n)"の文字列を調整し、GetHoverText関数の戻り値の末尾に追加されるようにする
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            CodeInstruction.Call(() => BuildHoverText(default!, default!))
        );

        // ldloc.3 NULL [Label21, Label22]
        // callvirt virtual string Card::GetHoverText2()
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldloc_3),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Card), nameof(Card.GetHoverText2), []))
        );
        // GetHoverText2関数の戻り値と呼び出したインスタンスの参照 (card) を保存する
        // GetHoverText2関数の戻り値がtextに追加されないようにする
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
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_S, localText2)
        );
        matcher.Advance(5); 
        matcher.RemoveInstruction();
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Stloc_S, localText2)
        );

        // call void WidgetMouseover::Show(string s)
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(WidgetMouseover), nameof(WidgetMouseover.Show), [typeof(string)]))
        );
        // Modで追加したUIを更新する
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_S, localText2),
            new CodeInstruction(OpCodes.Ldloc_S, localText3),
            new CodeInstruction(OpCodes.Ldloc_S, localText4),
            new CodeInstruction(OpCodes.Ldloc_S, localTarget1),
            new CodeInstruction(OpCodes.Ldloc_S, localTarget2),
            CodeInstruction.Call(() => ShowForMod(default!, default!, default!, default!, default!))
        );

        return matcher.InstructionEnumeration();
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(WidgetMouseover.Show), [typeof(string)])]
    private static void Show_Prefix(WidgetMouseover __instance, ref string s)
    {
        // 15行表示の場合にpivotのyが0.8fとなるようにする
        var lineCount = s.SplitByNewline().Length;
        __instance.layout.Rect().pivot = new(0.5f, 0.5f * (1 + 0.043f * (lineCount - 1)));
    }

    private static void ShowForMod(string? text2, string? text3, string? text4, Card? target1, Card? target2)
    {
        // Plugin.LogInfo($"text2: {text2}");
        // Plugin.LogInfo($"text3: {text3}");
        // Plugin.LogInfo($"text4: {text4}");
        // Plugin.LogInfo($"target1: {target1}");
        // Plugin.LogInfo($"target2: {target2}");

        if (!string.IsNullOrEmpty(text2))
        {
            TextName2!.text = text2;
            TextName2.SetActive(true);
        }
        else
        {
            TextName2!.text = string.Empty;
            TextName2.SetActive(false);
        }
        if (!string.IsNullOrEmpty(text3))
        {
            TextName3!.text = text3;
            TextName3.SetActive(true);
        }
        else
        {
            TextName3!.text = string.Empty;
            TextName3.SetActive(false);
        }
        if (!string.IsNullOrEmpty(text4))
        {
            TextName4!.text = text4;
            TextName4.SetActive(true);
        }
        else
        {
            TextName4!.text = string.Empty;
            TextName4.SetActive(false);
        }

        if (target1 is Chara chara1)
        {
            HealthBar1!.Update(chara1);
            HealthBar1!.SetActive(true);
        }
        else
        {
            HealthBar1!.SetActive(false);
        }
        if (target2 is Chara chara2)
        {
            HealthBar2!.Update(chara2);
            HealthBar2!.SetActive(true);
        }
        else
        {
            HealthBar2!.SetActive(false);
        }
    }

    private static string BuildHoverText(string hoverText, string otherCardsText)
    {
         return $"{hoverText}{otherCardsText.TagSize(14)}";
        // var lines = hoverText.SplitByNewline();
        // lines[0] = $"{lines[0]}{otherCardsText.TagSize(14)}";
        // return string.Join(Environment.NewLine, lines);
    }
}
