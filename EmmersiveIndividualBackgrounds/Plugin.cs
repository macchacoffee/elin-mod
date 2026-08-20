using System;
using System.Collections;
using System.Reflection;

using BepInEx;
using HarmonyLib;
using UnityEngine;

using Macchacoffee.ElinMods.EmmersiveIndividualBackgrounds.Patches;

namespace Macchacoffee.ElinMods.EmmersiveIndividualBackgrounds;

internal static class PluginInfo
{
    public const string Guid = "maccha-coffee.emmersive-individual-backgrounds";
    public const string Name = "Emmersive Individual Backgrounds";
    public const string Version = "1.0.0";
    public const string EmmersiveGuid = "dk.elinplugins.emmersive";
}

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
internal class Plugin : BaseUnityPlugin
{
    internal static Plugin? Instance { get; private set; }

    private bool _panelReopenScheduled;

    private void Awake()
    {
        Instance = this;
        if (AccessTools.TypeByName("Emmersive.EmMod") == null)
        {
            Logger.LogError(
                $"Elin with AI ({PluginInfo.EmmersiveGuid}) is not loaded. " +
                "Emmersive Individual Backgrounds will remain disabled.");
            enabled = false;
            return;
        }

        try
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginInfo.Guid);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to apply harmony patch: {ex}");
        }
    }

    internal static void LogError(object message)
    {
        Instance?.Logger.LogError(message);
    }

    internal void SchedulePanelReopen(float? scrollPosition)
    {
        if (_panelReopenScheduled)
        {
            return;
        }

        _panelReopenScheduled = true;
        StartCoroutine(ReopenPanelOnNextFrame(scrollPosition));
    }

    private IEnumerator ReopenPanelOnNextFrame(float? scrollPosition)
    {
        yield return null;

        LayerPanelAccess.Reopen();

        yield return null;

        if (scrollPosition is float position)
        {
            Canvas.ForceUpdateCanvases();
            LayerPanelAccess.RestoreScrollPosition(position);
        }

        _panelReopenScheduled = false;
    }
}
