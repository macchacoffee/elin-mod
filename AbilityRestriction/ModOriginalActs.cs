using System.Collections.Generic;

namespace AbilityRestriction;

public class ModOriginalActStorage
{
    private readonly Dictionary<int, List<ActList.Item>> UidToActs = [];

    public ICollection<ActList.Item>? GetActs(Chara chara)
    {
        if (UidToActs.TryGetValue(chara.uid, out var acts))
        {
            return [.. acts];
        }
        return [.. chara.ability.list.items];
    }

    public void SetActs(Chara chara, IEnumerable<ActList.Item> acts)
    {
        UidToActs[chara.uid] = [.. acts];
    }

    public bool RemoveActs(Chara chara)
    {
        return UidToActs.Remove(chara.uid);
    }
}
