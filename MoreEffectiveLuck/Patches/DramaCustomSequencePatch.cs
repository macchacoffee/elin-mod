using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModUtility.Patch;

namespace MoreEffectiveLuck.Patches;

[HarmonyPatch(typeof(DramaCustomSequence))]
public static class DramaCustomSequencePatch
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
        // if (c.IsPCParty)
        // {
        //     if (!c.isSummon)
        //     {
        //         Choice((c.GetInt(106) == 0) ? "daShutup" : "daShutup2", "_shutup");
        //         if (c.CanInsult() || c.GetInt(108) == 1)
        //         {
        //             Choice((c.GetInt(108) == 0) ? "daInsult" : "daInsult2", "_insult");
        //         }
        //         if (c.CanUseBane())
        //         {
        //             Choice(ModConsts.SourceId.DaBane, "_bane");
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
        // Step("_bane");
        // _Talk("tg", GetTopic(c, "insult"));
        // DramaCustomSequencePatch.AddBaneMethodForBuild(this, c);
        // End();
        var matcher = new CodeMatcher(instructions, generator);

        // 災いを覚えているパーティメンバーに話しかけた際に「災いをかけてもらう」選択肢を追加する
        // ldstr "daInsult2"
        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, "daInsult2")
        );
        // ldfld Chara DramaCustomSequence+<>c__DisplayClass14_0::c
        matcher.MatchStartBackwards(
            new CodeMatch(OpCodes.Ldfld)
        );
        var charaOperand = matcher.Operand;
        // bne.un Label170
        matcher.MatchStartBackwards(
            new CodeMatch(OpCodes.Bne_Un)
        );
        var label1 = matcher.Operand;
        var pos1 = matcher.Pos;
        // ldstr "daInsult" [Label171]
        // ldstr "_insult" [Label172]
        // ldc.i4.0 NULL
        // call DramaChoice DramaCustomSequence::Choice(string lang, string idJump, bool cancel)
        // pop NULL
        matcher.MatchEndForward(
            new CodeMatch(OpCodes.Ldstr, "daInsult"),
            new CodeMatch(OpCodes.Ldstr, "_insult"),
            new CodeMatch(OpCodes.Ldc_I4_0),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(DramaCustomSequence), nameof(DramaCustomSequence.Choice), [typeof(string), typeof(string), typeof(bool)])),
            new CodeMatch(OpCodes.Pop)
        );
        matcher.Advance(1);
        matcher.Insert(
            new CodeInstruction(OpCodes.Ldloc_0),
            new CodeInstruction(OpCodes.Ldfld, charaOperand),
            CodeInstruction.Call(() => CanUseBane(default!)),
            new CodeInstruction(OpCodes.Brfalse, label1),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldstr, ModConsts.SourceId.DaBane),
            new CodeInstruction(OpCodes.Ldstr, "_bane"),
            new CodeInstruction(OpCodes.Ldc_I4_0),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DramaCustomSequence), nameof(DramaCustomSequence.Choice), [typeof(string), typeof(string), typeof(bool)])),
            new CodeInstruction(OpCodes.Pop)
        );
        matcher.CreateLabel(out var labelMod1);
        matcher.Advance(pos1 - matcher.Pos);
        matcher.Operand = labelMod1;

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
            new CodeInstruction(OpCodes.Ldstr, "_bane"),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DramaCustomSequence), nameof(DramaCustomSequence.Step), [typeof(string)])),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldstr, "tg"),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldloc_0),
            new CodeInstruction(OpCodes.Ldfld, charaOperand),
            new CodeInstruction(OpCodes.Ldstr, "insult"),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DramaCustomSequence), nameof(DramaCustomSequence.GetTopic), [typeof(Chara), typeof(string)])),
            new CodeInstruction(OpCodes.Ldnull),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DramaCustomSequence), nameof(DramaCustomSequence._Talk), [typeof(string), typeof(string), typeof(string)])),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldloc_0),
            new CodeInstruction(OpCodes.Ldfld, charaOperand),
            CodeInstruction.Call(() => AddBaneMethodForBuild(default!, default!)),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DramaCustomSequence), nameof(DramaCustomSequence.End), []))
        );

        return matcher.InstructionEnumeration();
    }

    private static bool CanUseBane(this Chara chara)
    {
        foreach (ActList.Item item in chara.ability.list.items)
        {
            if (item.act.id == SPELL.SpBane)
            {
                return true;
            }
        }
        return false;
    }


    private static void AddBaneMethodForBuild(DramaCustomSequence dcs, Chara chara)
    {
        dcs.Method(() =>
        {
            chara.UseAbility(SPELL.SpBane, EClass.pc);
        });
    }
}
