using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

namespace Macchacoffee.ElinMods.SelectiveAutosave.Patches;

internal static class AutosaveTranspiler
{
    public static readonly MethodInfo SaveMethod = AccessTools.Method(
        typeof(Game),
        nameof(Game.Save),
        [typeof(bool), typeof(bool)])
        ?? throw new MissingMethodException(typeof(Game).FullName, nameof(Game.Save));

    public static readonly FieldInfo RequestField = AccessTools.Field(
        typeof(Player),
        nameof(Player.willAutoSave))
        ?? throw new MissingFieldException(typeof(Player).FullName, nameof(Player.willAutoSave));

    public static IEnumerable<CodeInstruction> ReplaceSaveCalls(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator,
        MethodBase original,
        string replacement)
    {
        var replacementMethod = GetReplacement(replacement);
        var matcher = new CodeMatcher(instructions, generator);
        var matched = false;

        while (matcher.MatchStartForward(new CodeMatch(OpCodes.Callvirt, SaveMethod)).IsValid)
        {
            // Game.Save()の呼び出しを置き換える。
            matcher.Opcode = OpCodes.Call;
            matcher.Operand = replacementMethod;
            matched = true;
            matcher.Advance(1);
        }
        if (!matched)
        {
            throw new InvalidOperationException($"Could not find a Game.Save call in {original.DeclaringType?.FullName}.{original.Name}.");
        }
        return matcher.InstructionEnumeration();
    }

    public static IEnumerable<CodeInstruction> ReplaceRequest(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator,
        MethodBase original,
        string replacement)
    {
        var replacementMethod = GetReplacement(replacement);
        var matcher = new CodeMatcher(instructions, generator);
        var matched = false;

        while (matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldc_I4_1),
            new CodeMatch(OpCodes.Stfld, RequestField)).IsValid)
        {
            // willAutoSaveを更新する処理を置き換える。
            matcher.Advance(1);
            matcher.Opcode = OpCodes.Call;
            matcher.Operand = replacementMethod;
            matched = true;
            matcher.Advance(1);
        }
        if (!matched)
        {
            throw new InvalidOperationException($"Could not find an autosave request in {original.DeclaringType?.FullName}.{original.Name}.");
        }
        return matcher.InstructionEnumeration();
    }

    private static MethodInfo GetReplacement(string name)
    {
        return AccessTools.Method(typeof(Mod.AutosaveControl), name)
            ?? throw new MissingMethodException(typeof(Mod.AutosaveControl).FullName, name);
    }
}
