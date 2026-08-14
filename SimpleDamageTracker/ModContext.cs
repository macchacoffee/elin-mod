using SimpleDamageTracker.Mod;

namespace SimpleDamageTracker;

internal static class ModContext
{
    public static ModDamageTracker DamageTracker { get; } = new();
}
