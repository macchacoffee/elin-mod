using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using Emmersive.API.Services;
using HarmonyLib;

namespace Macchacoffee.ElinMods.EmmersiveIndividualBackgrounds;

internal static class IndividualBackgroundState
{
    internal const string IndividualsDirectory = "Emmersive/Characters/Individuals";

    internal static readonly List<string> ModeLabels = ["Common", "Individual"];

    private static readonly Func<Chara, string> _getUnifiedId = CreateUnifiedIdGetter();
    private static readonly ConcurrentDictionary<string, bool> _individualModes = new();
    private static readonly ConcurrentDictionary<string, WeakReference<Chara>> _charactersByIndividualPath = new();
    private static int _refreshRequested;

    [ThreadStatic]
    private static Chara? _currentChara;

    internal static Chara? CurrentChara
    {
        get => _currentChara;
        set => _currentChara = value;
    }

    internal static string GetCommonPath(Chara chara)
    {
        return $"Emmersive/Characters/{_getUnifiedId(chara)}.txt";
    }

    internal static string GetIndividualPath(Chara chara)
    {
        return $"{IndividualsDirectory}/{Game.id}/{chara.uid}.txt";
    }

    internal static void Register(Chara chara)
    {
        var key = new ResourceKey(GetIndividualPath(chara));
        _charactersByIndividualPath[key.ResourcePath] = new(chara);
    }

    internal static bool TryGetRegisteredChara(ResourceKey key, out Chara chara)
    {
        if (_charactersByIndividualPath.TryGetValue(key.ResourcePath, out var reference)
            && reference.TryGetTarget(out chara))
        {
            return true;
        }

        chara = null!;
        return false;
    }

    internal static bool IsIndividualMode(Chara chara)
    {
        var path = GetIndividualPath(chara);
        return _individualModes.GetOrAdd(path, _ => HasCustomResource(new ResourceKey(path)));
    }

    internal static void SetIndividualMode(Chara chara, bool enabled)
    {
        _individualModes[GetIndividualPath(chara)] = enabled;
    }

    internal static bool HasIndividualBackground(Chara chara)
    {
        return HasCustomResource(new ResourceKey(GetIndividualPath(chara)));
    }

    internal static bool HasCustomResource(ResourceKey key)
    {
        var fullPath = ResourceFetch.CustomFolder + key;
        return File.Exists(fullPath.ResourcePath);
    }

    internal static void RequestRefresh()
    {
        Interlocked.Exchange(ref _refreshRequested, 1);
    }

    internal static bool ConsumeRefreshRequest()
    {
        return Interlocked.Exchange(ref _refreshRequested, 0) != 0;
    }

    private static Func<Chara, string> CreateUnifiedIdGetter()
    {
        var method = AccessTools.Method("Emmersive.Helper.ProfileHelper:get_UnifiedId")
            ?? throw new MissingMethodException("Emmersive.Helper.ProfileHelper", "get_UnifiedId");

        return (Func<Chara, string>)Delegate.CreateDelegate(typeof(Func<Chara, string>), method);
    }
}
