using System;
using System.Collections.Generic;

namespace Macchacoffee.ElinMods.ContainerPerformanceFix.Mod;

internal static class StackMerger
{
    private const int SmallContainerThreshold = 16;

    public static void FastMergeStacks(UIInventory inventory)
    {
        var things = inventory.owner.Container.things;
        if (things.Count <= SmallContainerThreshold)
        {
            MergeStacksVanillaOrder(things);
            return;
        }

        // マージに成功してもStackKeyの構成要素は変化しないため、各bucketは独立しており、
        // 初出順に1つずつ安定するまで処理できる。
        var orderedBuckets = BuildBuckets(things);
        foreach (var bucket in orderedBuckets)
        {
            bucket.MergeStacks();
        }
    }

    private static List<Bucket> BuildBuckets(ThingContainer things)
    {
        var lookup = new Dictionary<StackKey, Bucket>(things.Count);
        var orderedBuckets = new List<Bucket>(things.Count);

        // このキーには、Thing.CanStackToがTrait.CanStackToより前に行う厳密な一致判定だけを含める。
        // 特に、条件付きの染色判定や、バニラがTrait.CanStackToより後に行う判定は追加しない。
        foreach (var thing in things)
        {
            if (thing.invY == ThingContainer.InvYHotbar || thing.isDestroyed)
            {
                continue;
            }

            var key = new StackKey(thing);
            if (lookup.TryGetValue(key, out var bucket))
            {
                bucket.Add(thing);
            }
            else
            {
                bucket = new Bucket(thing);
                lookup.Add(key, bucket);
                orderedBuckets.Add(bucket);
            }
        }

        return orderedBuckets;
    }

    private static void MergeStacksVanillaOrder(ThingContainer things)
    {
        while (true)
        {
            var merged = false;

            foreach (var source in things)
            {
                if (source.invY == ThingContainer.InvYHotbar)
                {
                    continue;
                }

                foreach (var target in things)
                {
                    if (!ReferenceEquals(source, target)
                        && target.invY != ThingContainer.InvYHotbar
                        && source.TryStackTo(target))
                    {
                        merged = true;
                        break;
                    }
                }

                if (merged)
                {
                    break;
                }
            }

            if (!merged)
            {
                return;
            }
        }
    }

    // 以下の各メンバーは、Thing.CanStackToがTrait.CanStackToより前に行う無条件の一致判定に対応する。
    // ゲーム更新時には、この境界が変わっていないか再確認すること。
    private readonly struct StackKey : IEquatable<StackKey>
    {
        private readonly string? _id;
        private readonly int _idMaterial;
        private readonly int _refVal;
        private readonly BlessedState _blessedState;
        private readonly int _rarityLv;
        private readonly int _tier;
        private readonly int _idSkin;
        private readonly bool _isGifted;
        private readonly string? _idRefCard;
        private readonly string? _idRefCard2;
        private readonly bool _isDecayed;

        public StackKey(Thing thing)
        {
            _id = thing.id;
            _idMaterial = thing.idMaterial;
            _refVal = thing.refVal;
            _blessedState = thing.blessedState;
            _rarityLv = thing.rarityLv;
            _tier = thing.tier;
            _idSkin = thing.idSkin;
            _isGifted = thing.isGifted;
            _idRefCard = thing.c_idRefCard;
            _idRefCard2 = thing.c_idRefCard2;
            // マージに成功するのはMaxDecayの同じ側にあるdecay値同士だけであり、
            // その加重平均も閾値を跨がないため、マージ後もこの分類は変化しない。
            _isDecayed = thing.IsDecayed;
        }

        public bool Equals(StackKey other)
        {
            return _idMaterial == other._idMaterial
                && _refVal == other._refVal
                && _blessedState == other._blessedState
                && _rarityLv == other._rarityLv
                && _tier == other._tier
                && _idSkin == other._idSkin
                && _isGifted == other._isGifted
                && _isDecayed == other._isDecayed
                && string.Equals(_id, other._id, StringComparison.Ordinal)
                && string.Equals(_idRefCard, other._idRefCard, StringComparison.Ordinal)
                && string.Equals(_idRefCard2, other._idRefCard2, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj)
        {
            return obj is StackKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 31 + (_id?.GetHashCode() ?? 0);
                hash = hash * 31 + _idMaterial;
                hash = hash * 31 + _refVal;
                hash = hash * 31 + (int)_blessedState;
                hash = hash * 31 + _rarityLv;
                hash = hash * 31 + _tier;
                hash = hash * 31 + _idSkin;
                hash = hash * 31 + (_isGifted ? 1 : 0);
                hash = hash * 31 + (_idRefCard?.GetHashCode() ?? 0);
                hash = hash * 31 + (_idRefCard2?.GetHashCode() ?? 0);
                hash = hash * 31 + (_isDecayed ? 1 : 0);
                return hash;
            }
        }
    }

    private sealed class Bucket(Thing first)
    {
        private const int MinCompactCount = 32;
        private const int CompactRatioDenominator = 4;

        private readonly Thing _first = first;
        private List<Thing>? _items = null;
        private int _destroyedCount = 0;

        public void Add(Thing thing)
        {
            _items ??= new(4)
            {
                _first,
            };

            _items.Add(thing);
        }

        public void MergeStacks()
        {
            if (_items is null)
            {
                return;
            }

            while (_items.Count - _destroyedCount > 1)
            {
                var merged = false;

                // ListはThingContainer内での相対順を維持する。マージ後はこのbucketの先頭へ戻り、
                // バニラと同じsource / targetの探索規則を維持する。
                foreach (var source in _items)
                {
                    if (source.invY == ThingContainer.InvYHotbar || source.isDestroyed)
                    {
                        continue;
                    }

                    foreach (var target in _items)
                    {
                        if (!ReferenceEquals(source, target)
                            && !target.isDestroyed
                            && target.invY != ThingContainer.InvYHotbar
                            && source.TryStackTo(target))
                        {
                            // TryStackToが成功すると、sourceは必ずDestroyされる。
                            _destroyedCount++;
                            merged = true;
                            break;
                        }
                    }

                    if (merged)
                    {
                        break;
                    }
                }

                if (!merged)
                {
                    return;
                }

                // 両方のEnumeratorを抜けた後だけcompactする。
                // RemoveAllは生存している候補の相対順を維持する。
                if (_destroyedCount >= MinCompactCount
                    && _destroyedCount * CompactRatioDenominator >= _items.Count)
                {
                    _items.RemoveAll(static thing => thing.isDestroyed);
                    _destroyedCount = 0;
                }
            }
        }
    }
}
