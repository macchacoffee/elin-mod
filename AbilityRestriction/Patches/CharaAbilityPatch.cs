using System.Linq;
using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;

namespace AbilityRestriction.Patches;

[HarmonyPatch(typeof(CharaAbility))]
public static class CharaAbilityPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyPatch(nameof(CharaAbility.Refresh), []), HarmonyPostfix]
    private static void Refresh_Postfix(CharaAbility __instance)
    {
        var owner = __instance.owner;

        var deniedAbility = Mod.Config.GetDeniedAbility(owner.uid);
        if (deniedAbility is null)
        {
            Mod.OriginalActStorage.RemoveActs(owner);
            return;
        }

        // Store orginal chara abilities.
        Mod.OriginalActStorage.SetActs(owner, __instance.list.items);

        // Remove forgotten chara abilities from denied abilities.
        deniedAbility.IntersectWith(__instance.list.items.Select(item => new ModDeniedAct(item)));
        // Restrict chara abilities.
        __instance.list.items.RemoveAll(item => deniedAbility.Contains(new ModDeniedAct(item)));
    }
}
