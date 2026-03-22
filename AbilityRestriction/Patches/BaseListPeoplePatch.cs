using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;

namespace AbilityRestriction.Patches;


[HarmonyPatch(typeof(BaseListPeople))]
public static class BaseListPeoplePatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
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
        if (c.IsPC || !c.IsHomeMember())
        {
            return;
        }

        TargetChara = c;
    }
}
