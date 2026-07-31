using System;
using FactionEffectInInventory;

namespace SomewhatEnhancedDisplay.Extensions;

public static class ElementContainerFactionExtensions
{
    public static void OnAdd(this ElementContainerFaction ecf, Thing thing)
    {
        UpdateRecursive(ecf, thing, (ecf, t) => ecf.OnEquip(t));
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

    private static void UpdateRecursive(ElementContainerFaction ecf, Thing thing, Action<ElementContainerFaction, Thing> update)
    {
        // 「それは装備するたびに呪われる」エンチャントが付いているアイテムは対象外とする
        if (thing.Evalue(ENC.permaCurse) <= 0)
        {
            update(ecf, thing);
        }

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