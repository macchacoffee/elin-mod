using System;

namespace Macchacoffee.ElinMods.FactionEnchantInInventory.Extensions;

internal static class ElementContainerFactionExtensions
{
    public static void OnAdd(this ElementContainerFaction ecf, Thing thing)
    {
        UpdateRecursive(ecf, thing, (ecf, t) =>
        {
            // 「それは装備するたびに呪われる」エンチャントが付いているアイテムは対象外とする
            // 設定で有効な場合は対象外にしない
            if (ModContext.Config.EnableRecursiveCurse.Value || t.Evalue(ENC.permaCurse) <= 0)
            {
                ecf.OnEquip(t);
            }
        });
    }

    public static void OnAddThings(this ElementContainerFaction ecf, Chara chara)
    {
        foreach (var thing in chara.things)
        {
            OnAdd(ecf, thing);
        }
    }

    public static void OnRemove(this ElementContainerFaction ecf, Thing thing)
    {
        UpdateRecursive(ecf, thing, (ecf, t) => ecf.OnUnequip(t));
    }

    public static void OnRemoveThings(this ElementContainerFaction ecf, Chara chara)
    {
        foreach (var thing in chara.things)
        {
            OnRemove(ecf, thing);
        }
    }

    private static void UpdateRecursive(
        ElementContainerFaction ecf,
        Thing thing,
        Action<ElementContainerFaction, Thing> update)
    {
        update(ecf, thing);

        // アイテムが★収納箱以外のコンテナである場合はその内部のアイテムも再帰的に対象とする
        if (!thing.IsContainer || thing.things.IsMagicChest)
        {
            return;
        }
        foreach (var childThing in thing.things)
        {
            UpdateRecursive(ecf, childThing, update);
        }
    }
}
