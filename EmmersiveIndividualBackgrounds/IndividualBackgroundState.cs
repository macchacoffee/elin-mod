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
    public const string IndividualsDirectory = "Emmersive/Characters/Individuals";

    public static readonly List<string> ModeLabels = ["Common", "Individual"];

    private static readonly Func<Chara, string> _getUnifiedId = CreateUnifiedIdGetter();
    private static readonly ConcurrentDictionary<string, bool> _individualModes = new();
    private static readonly ConcurrentDictionary<string, WeakReference<Chara>> _charactersByIndividualPath = new();
    private static int _refreshRequested;

    [ThreadStatic]
    private static Chara? _currentChara;

    public static Chara? CurrentChara
    {
        get => _currentChara;
        set => _currentChara = value;
    }

    public static string GetCommonPath(Chara chara)
    {
        return $"Emmersive/Characters/{_getUnifiedId(chara)}.txt";
    }

    public static string GetIndividualPath(Chara chara)
    {
        return $"{IndividualsDirectory}/{Game.id}/{chara.uid}.txt";
    }

    public static void Register(Chara chara)
    {
        var key = new ResourceKey(GetIndividualPath(chara));
        _charactersByIndividualPath[key.ResourcePath] = new(chara);
    }

    public static bool TryGetRegisteredChara(ResourceKey key, out Chara chara)
    {
        if (_charactersByIndividualPath.TryGetValue(key.ResourcePath, out var reference)
            && reference.TryGetTarget(out chara))
        {
            return true;
        }

        chara = null!;
        return false;
    }

    public static bool IsIndividualMode(Chara chara)
    {
        var path = GetIndividualPath(chara);
        return _individualModes.GetOrAdd(path, _ => HasCustomResource(new ResourceKey(path)));
    }

    public static void SetIndividualMode(Chara chara, bool enabled)
    {
        _individualModes[GetIndividualPath(chara)] = enabled;
    }

    public static bool HasIndividualBackground(Chara chara)
    {
        return HasCustomResource(new ResourceKey(GetIndividualPath(chara)));
    }

    public static bool HasCustomResource(ResourceKey key)
    {
        var fullPath = ResourceFetch.CustomFolder + key;
        return File.Exists(fullPath.ResourcePath);
    }

    public static void RequestRefresh()
    {
        Interlocked.Exchange(ref _refreshRequested, 1);
    }

    public static bool ConsumeRefreshRequest()
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
