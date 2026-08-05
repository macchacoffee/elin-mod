using System;
using System.Collections.Concurrent;

namespace ModUtility.Util;

public static class ModSharedStore
{
    private static readonly string SlotKey = "maccha-coffee.ModUtility.Util.ModSharedStore";

    public static ConcurrentDictionary<TKey, TValue> GetData<TKey, TValue>(string key)
    {
        var name = $"{SlotKey}.{key}";
        if (AppDomain.CurrentDomain.GetData(name) is not ConcurrentDictionary<TKey, TValue> cache)
        {
            cache = [];
            AppDomain.CurrentDomain.SetData(name, cache);
        }
        
        return cache;
    }
}
