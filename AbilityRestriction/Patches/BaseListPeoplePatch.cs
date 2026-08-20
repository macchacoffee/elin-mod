using System.Reflection;
using Macchacoffee.ElinMods.AbilityRestriction.Mod;
using HarmonyLib;
using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.AbilityRestriction.Patches;

[HarmonyPatch(typeof(BaseListPeople))]
internal static class BaseListPeoplePatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    public static Chara? TargetChara { get; set; }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(BaseListPeople.OnClick), [typeof(Chara), typeof(ItemGeneral)])]
    private static void OnClick_Prefix(BaseListPeople __instance, Chara c, ItemGeneral i)
    {
        if (__instance.GetType() != typeof(ListPeople))
        {
            return;
        }
        if (!ModContext.Config.EnableViaResidentBoard.Value)
        {
            return;
        }
        if (!ModAbilityRestriction.CanRestrictAbility(c))
        {
            return;
        }
        TargetChara = c;
    }
}
