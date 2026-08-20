using System;
using System.Reflection;

using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace Macchacoffee.ElinMods.EmmersiveIndividualBackgrounds.Patches;

internal static class LayerPanelAccess
{
    private static readonly Type _layerType = AccessTools.TypeByName("Emmersive.Components.LayerEmmersivePanel")
        ?? throw new TypeLoadException("Emmersive.Components.LayerEmmersivePanel");
    private static readonly MethodInfo _instanceGetter = AccessTools.PropertyGetter(_layerType, "Instance")
        ?? throw new MissingMethodException(_layerType.FullName, "get_Instance");
    private static readonly MethodInfo _windowGetter = AccessTools.PropertyGetter(_layerType, "Window")
        ?? throw new MissingMethodException(_layerType.FullName, "get_Window");
    private static readonly MethodInfo _currentContentGetter =
        AccessTools.PropertyGetter(_windowGetter.ReturnType, "CurrentContent")
        ?? throw new MissingMethodException(_windowGetter.ReturnType.FullName, "get_CurrentContent");
    private static readonly MethodInfo _reopenMethod = AccessTools.Method(_layerType, "Reopen")
        ?? throw new MissingMethodException(_layerType.FullName, "Reopen");

    internal static void Reopen()
    {
        try
        {
            var instance = _instanceGetter.Invoke(null, null);
            if (instance != null)
            {
                _reopenMethod.Invoke(instance, null);
            }
        }
        catch (Exception ex)
        {
            Plugin.LogError($"Failed to reopen Elin with AI panel: {ex}");
        }
    }

    internal static void ScheduleReopenPreservingScrollPosition()
    {
        var plugin = Plugin.Instance;
        if (plugin == null)
        {
            Plugin.LogError("Cannot schedule Elin with AI panel reopen because the plugin is unavailable.");
            return;
        }

        plugin.SchedulePanelReopen(GetCurrentScrollPosition());
    }

    internal static void RestoreScrollPosition(float position)
    {
        var scrollRect = GetCurrentScrollRect();
        scrollRect?.verticalNormalizedPosition = Mathf.Clamp01(position);
    }

    private static float? GetCurrentScrollPosition()
    {
        return GetCurrentScrollRect()?.verticalNormalizedPosition;
    }

    private static ScrollRect? GetCurrentScrollRect()
    {
        var instance = _instanceGetter.Invoke(null, null);
        if (instance == null)
        {
            return null;
        }

        var window = _windowGetter.Invoke(instance, null) as Component;
        if (window == null)
        {
            return null;
        }

        var currentContent = _currentContentGetter.Invoke(window, null) as Component;
        return currentContent?.GetComponent<ScrollRect>();
    }
}
