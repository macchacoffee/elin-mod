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

        // Successful merges cannot change any StackKey member, so buckets are
        // independent and can be stabilized in their first-appearance order.
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

        // Keep this key limited to strict equality checks performed by Thing.CanStackTo
        // before Trait.CanStackTo. In particular, do not add the conditional dye check
        // or any checks that vanilla performs after Trait.CanStackTo.
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

    // Every member below maps to an unconditional equality check performed by
    // Thing.CanStackTo before Trait.CanStackTo. Recheck this boundary on updates.
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
            // A successful merge only averages decay values that are already on the
            // same side of MaxDecay, so this classification remains stable afterward.
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

    private sealed class Bucket
    {
        private const int MinCompactCount = 32;
        private const int CompactRatioDenominator = 4;

        private readonly Thing _first;
        private List<Thing>? _items;
        private int _destroyedCount;

        public Bucket(Thing first)
        {
            _first = first;
            _items = null;
            _destroyedCount = 0;
        }

        public void Add(Thing thing)
        {
            if (_items is null)
            {
                _items = new List<Thing>(4)
                {
                    _first,
                };
            }

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

                // The list preserves ThingContainer-relative order. After a merge,
                // restart this bucket to preserve vanilla source/target semantics.
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
                            // A successful TryStackTo always destroys its source.
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

                // Compact only after leaving both enumerators. RemoveAll preserves
                // the relative order of every surviving candidate.
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
