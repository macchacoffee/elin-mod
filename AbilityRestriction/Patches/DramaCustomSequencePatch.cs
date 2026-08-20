using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Macchacoffee.ElinMods.AbilityRestriction.Mod;
using HarmonyLib;
using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.AbilityRestriction.Patches;

[HarmonyPatch(typeof(DramaCustomSequence))]
internal static class DramaCustomSequencePatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(DramaCustomSequence.Build), [typeof(Chara)])]
    private static IEnumerable<CodeInstruction> Build_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        // // 変更前
        // if (c.IsPCParty)
        // {
        //     if (!c.isSummon)
        //     {
        //         Choice((c.GetInt(106) == 0) ? "daShutup" : "daShutup2", "_shutup");
        //         if (c.CanInsult() || c.GetInt(108) == 1)
        //         {
        //             Choice((c.GetInt(108) == 0) ? "daInsult" : "daInsult2", "_insult");
        //         }
        //     }
        // }
        // ...
        // Step("_insult");
        // Method(delegate
        // {
        //     c.SetInt(108, (c.GetInt(108) == 0) ? 1 : 0);
        // });
        // _Talk("tg", GetTopic(c, (c.GetInt(108) == 0) ? "insult" : "insult2"));
        // Method(delegate
        // {
        //     if (c.GetInt(108) == 1)
        //     {
        //         c.Talk("insult");
        //     }
        // });
        // End();
        // // 変更後
        // if (DramaCustomSequencePatch.IsAbilityRestrictionEnabled(c))
        // {
        //     Choice(ModConsts.SourceId.DaRestrictAbilities, "_restrictAbilities");
        // }
        // if (c.IsPCParty)
        // {
        //     if (!c.isSummon)
        //     {
        //         Choice((c.GetInt(106) == 0) ? "daShutup" : "daShutup2", "_shutup");
        //         if (c.CanInsult() || c.GetInt(108) == 1)
        //         {
        //             Choice((c.GetInt(108) == 0) ? "daInsult" : "daInsult2", "_insult");
        //         }
        //     }
        // }
        // ...
        // Step("_insult");
        // Method(delegate
        // {
        //     c.SetInt(108, (c.GetInt(108) == 0) ? 1 : 0);
        // });
        // _Talk("tg", GetTopic(c, (c.GetInt(108) == 0) ? "insult" : "insult2"));
        // Method(delegate
        // {
        //     if (c.GetInt(108) == 1)
        //     {
        //         c.Talk("insult");
        //     }
        // });
        // End();
        // Step("_restrictAbilities");
        // DramaCustomSequencePatch.HandleAbilityRestriction(this, c);
        // End();
        var matcher = new CodeMatcher(instructions, generator);

        // アビリティを制限できる相手であれば「アビリティを制限する」選択肢を追加する
        // ldstr "daShutup2"
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, "daShutup2")
        );
        // ldloc.0 NULL [Label163, Label164, Label165]
        // ldfld Chara DramaCustomSequence+<>c__DisplayClass14_0::c
        // callvirt virtual bool Card::get_IsPCParty()
        matcher.MatchStartBackwards(
            new CodeMatch(OpCodes.Ldloc_0),
            new CodeMatch(OpCodes.Ldfld),
            new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Card), nameof(Card.IsPCParty)))
        );
       matcher.Advance(1);
        var charaOperand = matcher.Operand;
        matcher.Advance(-1);
        var pos1 = matcher.Pos;
        var labelList1 = matcher.Labels.Copy();
        matcher.Labels.Clear();
        matcher.CreateLabel(out var labelMod1);
        matcher.Insert(
            new CodeInstruction(OpCodes.Ldloc_0),
            new CodeInstruction(OpCodes.Ldfld, charaOperand),
            CodeInstruction.Call(() => IsAbilityRestrictionEnabled(default!)),
            new CodeInstruction(OpCodes.Brfalse, labelMod1),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldstr, ModConsts.SourceId.DaRestrictAbilities),
            new CodeInstruction(OpCodes.Ldstr, "_restrictAbilities"),
            new CodeInstruction(OpCodes.Ldc_I4_0),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DramaCustomSequence), nameof(DramaCustomSequence.Choice), [typeof(string), typeof(string), typeof(bool)])),
            new CodeInstruction(OpCodes.Pop)
        );
        matcher.AddLabels(labelList1);

        // ldstr "_insult"
        // call void DramaCustomSequence::Step(string step)
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldstr, "_insult"),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(DramaCustomSequence), nameof(DramaCustomSequence.Step), [typeof(string)]))
        );
        // call void DramaCustomSequence::End()
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(DramaCustomSequence), nameof(DramaCustomSequence.End), []))
        );
        matcher.Advance(1);
        matcher.Insert(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldstr, "_restrictAbilities"),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DramaCustomSequence), nameof(DramaCustomSequence.Step), [typeof(string)])),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldloc_0),
            new CodeInstruction(OpCodes.Ldfld, charaOperand),
            CodeInstruction.Call(() => HandleAbilityRestriction(default!, default!)),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DramaCustomSequence), nameof(DramaCustomSequence.End), []))
        );

        return matcher.InstructionEnumeration();
    }

    private static bool IsAbilityRestrictionEnabled(Chara chara)
    {
        return ModContext.Config.EnableViaConversation.Value && ModAbilityRestriction.CanRestrictAbility(chara);
    }

    private static void HandleAbilityRestriction(DramaCustomSequence dcs, Chara chara)
    {
        dcs.Method(() =>
        {
            ModAbilityRestriction.BuildSettingLayer(chara)();
        });
    }
}
