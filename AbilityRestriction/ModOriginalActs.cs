using System.Collections.Generic;

namespace AbilityRestriction;

public class ModOriginalActStorage
{
    private readonly Dictionary<int, List<ActList.Item>> uidToActs = new Dictionary<int, List<ActList.Item>>();

    public ICollection<ActList.Item>? GetActs(Chara chara)
    {
        if (uidToActs.TryGetValue(chara.uid, out var acts))
        {
            return [.. acts];
        }
        return [.. chara.ability.list.items];
    }

    public void SetActs(Chara chara, IEnumerable<ActList.Item> acts)
    {
        uidToActs[chara.uid] = [.. acts];
    }

    public bool RemoveActs(Chara chara)
    {
        return uidToActs.Remove(chara.uid);
    }
}
