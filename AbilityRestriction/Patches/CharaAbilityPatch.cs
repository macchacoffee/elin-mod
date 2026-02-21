using System.Linq;

namespace AbilityRestriction.Patches;

public static class CharaAbilityPatch
{
    public static void Refresh_Postfix(CharaAbility __instance)
    {
        var owner = __instance.owner;

        var deniedAbility = Mod.Config.GetDeniedAbility(owner.uid);
        if (deniedAbility == null)
        {
            Mod.originalActStorage.RemoveActs(owner);
            return;
        }

        // Store orginal chara abilities.
        Mod.originalActStorage.SetActs(owner, __instance.list.items);

        // Remove forgotten chara abilities from denied abilities.
        deniedAbility.IntersectWith(__instance.list.items.Select(item => new ModDeniedAct(item)));
        // Restrict chara abilities.
        __instance.list.items.RemoveAll(item => deniedAbility.Contains(new ModDeniedAct(item)));
    }
}
