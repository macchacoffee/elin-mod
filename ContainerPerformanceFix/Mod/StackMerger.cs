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

                if (!buckets.TryGetValue(new StackKey(source.id, source.idMaterial), out var bucket))
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

        // CanStackTo rejects a different id or idMaterial before compress can mutate
        // encLV/elements. The key only removes comparisons that vanilla must reject.
        foreach (var thing in things)
        {
            if (thing.invY == ThingContainer.InvYHotbar || thing.isDestroyed)
            {
                continue;
            }

            var key = new StackKey(thing.id, thing.idMaterial);
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

    private readonly struct StackKey : IEquatable<StackKey>
    {
        private readonly string _id;
        private readonly int _idMaterial;

        public StackKey(string id, int idMaterial)
        {
            _id = id;
            _idMaterial = idMaterial;
        }

        public bool Equals(StackKey other)
        {
            return _idMaterial == other._idMaterial
                && string.Equals(_id, other._id, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj)
        {
            return obj is StackKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_id?.GetHashCode() ?? 0) * 397) ^ _idMaterial;
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
