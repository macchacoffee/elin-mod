using System.Collections.Generic;

namespace Macchacoffee.ElinMods.SimpleDamageTracker.Mod;

internal class DamageTracker
{
    private readonly Dictionary<int, long> _damageByUid = [];
    public long TotalDamage { get; private set; }

    public long GetDamage(int uid)
    {
        return _damageByUid.TryGetValue(uid, out long damage) ? damage : 0;
    }

    public void AddDamage(int uid, long damage)
    {
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

    public void Reset()
    {
        _damageByUid.Clear();
        TotalDamage = 0;
    }
}
