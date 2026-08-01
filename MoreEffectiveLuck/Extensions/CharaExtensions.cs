using System;

namespace MoreEffectiveLuck.Extensions;

public static class CharaExtensions
{
    public static bool CanUseBane(this Chara chara)
    {
        foreach (ActList.Item item in chara.ability.list.items)
        {
            if (item.act.id == SPELL.SpBane)
            {
                return true;
            }
        }
        return false;
    }
}
