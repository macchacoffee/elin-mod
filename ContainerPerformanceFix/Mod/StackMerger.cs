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

        var buckets = BuildBuckets(things);

        while (true)
        {
            var merged = false;

            // Source order remains the current ThingContainer order, just like vanilla.
            for (var sourceIndex = 0; sourceIndex < things.Count; sourceIndex++)
            {
                var source = things[sourceIndex];
                if (source.invY == ThingContainer.InvYHotbar || source.isDestroyed)
                {
                    continue;
                }

                if (!buckets.TryGetValue(new StackKey(source), out var bucket))
                {
                    continue;
                }

                if (bucket.TryMergeFrom(source))
                {
                    merged = true;
                    break;
                }
            }

            if (!merged)
            {
                return;
            }

            // TryStackTo destroys and removes the source after a successful merge.
            // Restart from the first source to preserve vanilla's mutation semantics.
        }
    }

    private static Dictionary<StackKey, Bucket> BuildBuckets(ThingContainer things)
    {
        var buckets = new Dictionary<StackKey, Bucket>(things.Count);

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
            if (buckets.TryGetValue(key, out var bucket))
            {
                bucket.Add(thing);
                buckets[key] = bucket;
            }
            else
            {
                buckets.Add(key, new Bucket(thing));
            }
        }

        return buckets;
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

    private struct Bucket
    {
        private readonly Thing _first;
        private List<Thing>? _remaining;

        public Bucket(Thing first)
        {
            _first = first;
            _remaining = null;
        }

        public void Add(Thing thing)
        {
            (_remaining ??= new List<Thing>(1)).Add(thing);
        }

        public bool TryMergeFrom(Thing source)
        {
            if (TryMerge(source, _first))
            {
                return true;
            }

            if (_remaining is null)
            {
                return false;
            }

            foreach (var target in _remaining)
            {
                if (TryMerge(source, target))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TryMerge(Thing source, Thing target)
        {
            // A shared key is only a candidate filter. TryStackTo remains the sole
            // authority for every stateful vanilla check, including compress behavior.
            return !ReferenceEquals(source, target)
                && !target.isDestroyed
                && target.invY != ThingContainer.InvYHotbar
                && source.TryStackTo(target);
        }
    }
}
