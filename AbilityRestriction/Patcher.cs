using System;
using System.Collections.Generic;
using System.Reflection;
using AbilityRestriction.Patches;
using HarmonyLib;

namespace AbilityRestriction;

public static class Patcher
{
    private class PatcherItem
    {
        public MethodInfo Original { get; set; }
        public MethodInfo? Prefix { get; set; }
        public MethodInfo? Postfix { get; set; }
        public MethodInfo? Transpiler { get; set; }
        public MethodInfo? Finalizer { get; set; }
        public MethodInfo? Ilmanipulator { get; set; }

        public PatcherItem(MethodInfo original, MethodInfo? prefix = null, MethodInfo? postfix = null, MethodInfo? transpiler = null, MethodInfo? finalizer = null, MethodInfo? ilmanipulator = null)
        {
            Original = original;
            Prefix = prefix;
            Postfix = postfix;
            Transpiler = transpiler;
            Finalizer = finalizer;
            Ilmanipulator = ilmanipulator;
        }

        public void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: Original,
                prefix: Prefix != null ? new HarmonyMethod(Prefix) : null,
                postfix: Postfix != null ? new HarmonyMethod(Postfix) : null,
                transpiler: Transpiler != null ? new HarmonyMethod(Transpiler) : null,
                finalizer: Finalizer != null ? new HarmonyMethod(Finalizer) : null,
                ilmanipulator: Ilmanipulator != null ? new HarmonyMethod(Ilmanipulator) : null
            );
        }
    }

    public static void Patch()
    {
        var harmony = new Harmony(PluginInfo.Guid);
        foreach (var item in CreatePatcherItems())
        {
            item.Patch(harmony);
        }
    }

    private static List<PatcherItem> CreatePatcherItems()
    {
        return [
            new PatcherItem(
                original: typeof(BaseListPeople).GetMethod(nameof(BaseListPeople.OnClick), [typeof(Chara), typeof(ItemGeneral)]),
                prefix: typeof(BaseListPeoplePatch).GetMethod(nameof(BaseListPeoplePatch.OnClick_Prefix))
            ),
            new PatcherItem(
                original: typeof(CharaAbility).GetMethod(nameof(CharaAbility.Refresh), []),
                postfix: typeof(CharaAbilityPatch).GetMethod(nameof(CharaAbilityPatch.Refresh_Postfix))
            ),
            new PatcherItem(
                original: typeof(GameIO).GetMethod(nameof(GameIO.SaveGame), []),
                prefix: typeof(GameIOPatch).GetMethod(nameof(GameIOPatch.SaveGame_Prefix))
            ),
            new PatcherItem(
                original: typeof(Game).GetMethod(nameof(Game.Load), [typeof(string), typeof(bool)]),
                postfix: typeof(GamePatch).GetMethod(nameof(GamePatch.Load_Postfix))
            ),
            new PatcherItem(
                original: typeof(UIContextMenu).GetMethod(nameof(UIContextMenu.AddButton), [typeof(string), typeof(Action), typeof(bool)]),
                postfix: typeof(UIContextMenuPatch).GetMethod(nameof(UIContextMenuPatch.AddButton_Postfix))
            ),
        ];
    }
}