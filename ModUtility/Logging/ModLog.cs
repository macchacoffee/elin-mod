using System;

using BepInEx.Logging;

namespace Macchacoffee.ElinMods.ModUtility.Logging;

internal static class ModLog
{
    private static ManualLogSource? _source;

    private static ManualLogSource Source => _source
        ?? throw new InvalidOperationException("ModLog has not been initialized.");

    public static void Initialize(ManualLogSource logger)
    {
        _source = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public static void Debug(object message)
    {
        Source.LogDebug(message);
    }

    public static void Info(object message)
    {
        Source.LogInfo(message);
    }

    public static void Error(object message)
    {
        Source.LogError(message);
    }
}
