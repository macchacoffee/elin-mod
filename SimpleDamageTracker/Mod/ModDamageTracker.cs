using System.Collections.Generic;

namespace SimpleDamageTracker.Mod;

internal class ModDamageTracker
{
    private readonly Dictionary<int, long> _damageByUid = [];
    public IReadOnlyDictionary<int, long> DamageByUid => _damageByUid;
    public long TotalDamage { get; private set; }

    public void RecordDamage(Card target, Card origin, long damage)
    {
        Plugin.LogInfo($"OnDamage {target.Name} <- {origin.Name} ({damage})");

        if (origin?.Chara is not Chara originChara || !originChara.IsPCParty)
        {
            return;
        }
        AddDamage(originChara, damage);
    }

    private void AddDamage(Chara chara, long damage)
    {
        var uid = chara.uid;
        if (_damageByUid.TryGetValue(uid, out var current))
        {
            _damageByUid[uid] = current + damage;
        }
        else
        {
            _damageByUid.Add(uid, damage);
        }
        TotalDamage += damage;
    }
}
