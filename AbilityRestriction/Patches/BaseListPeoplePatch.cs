using System.Reflection;
using HarmonyLib;

namespace AbilityRestriction.Patches;


[HarmonyPatch(typeof(BaseListPeople))]
public static class BaseListPeoplePatch
{
    private static readonly PatchTarget Target = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return Target.IsPatchable(original);
    }

    public static Chara? TargetChara { get; set; }

    [HarmonyPatch(nameof(BaseListPeople.OnClick), [typeof(Chara), typeof(ItemGeneral)]), HarmonyPrefix]
    private static void OnClick_Prefix(BaseListPeople __instance, Chara c, ItemGeneral i)
    {
        if (__instance.GetType() != typeof(ListPeople))
        {
            return;
        }
        if (c.IsPC || !c.IsHomeMember())
        {
            return;
        }

        TargetChara = c;
    }
}
