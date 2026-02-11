using System.Linq;
using HarmonyLib;

namespace AbilityRestriction.Patches;

[HarmonyPatch(typeof(CharaAbility))]
public static class CharaAbilityPatch
{
    [HarmonyPatch(nameof(CharaAbility.Refresh), []), HarmonyPostfix]
    public static void Refresh_Postfix(CharaAbility __instance)
    {
        var owner = __instance.owner;

        var deniedAbility = Mod.config.GetDeniedAbility(owner.uid);
        if (deniedAbility == null)
        {
            Mod.originalActStorage.RemoveActs(owner);
            return;
        }

        // Store orginal chara abilities.
        Mod.originalActStorage.SetActs(owner, __instance.list.items);

        // Remove forgotten chara abilities from denied abilities.
        deniedAbility.IntersectWith(__instance.list.items.Select(item => item.act.id));
        // Restrict chara abilities.
        __instance.list.items.RemoveAll(item => deniedAbility.Contains(item.act.id));
    }
}
