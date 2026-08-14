using SimpleDamageTracker.Config;
using UnityEngine;

namespace SimpleDamageTracker.Mod;

internal sealed class ModDamageDisplay : MonoBehaviour
{
    private readonly record struct TextAppearance(
        float X,
        float Y,
        float SizeScale,
        ModHorizontalTextAlignment HorizontalAlignment,
        Color Color,
        int BaseSize,
        int BaseFontSize
    );

    private UIText? _baseText;
    private UIText? _damageText;
    private UIText? _damageShareText;

    private int _uid;

    private long _lastDamage = -1;
    private long _lastTotalDamage = -1;

    private TextAppearance _lastDamageAppearance;
    private TextAppearance _lastDamageShareAppearance;
    private bool _hasAppearance;
    private bool _lastDisplayDamage;
    private bool _lastDisplayDamageShare;
    private bool _lastDisplayNoDamage;
    private bool _hasValue;

    public void Bind(ButtonRoster roster, Chara chara)
    {
        _uid = chara.uid;
        _baseText = roster.textName;

        _lastDamage = -1;
        _lastTotalDamage = -1;

        CreateTexts(roster);
    }

    private void CreateTexts(ButtonRoster roster)
    {
        if (_damageText == null)
        {
            _damageText = CreateText(roster, ModConsts.GameObjectName.DamageDisplayDamage);
        }

        if (_damageShareText == null)
        {
            _damageShareText = CreateText(roster, ModConsts.GameObjectName.DamageDisplayPercentage);
        }
    }

    private static UIText CreateText(ButtonRoster roster, string name)
    {
        var text = Instantiate(roster.textName, roster.rect);

        text.gameObject.name = name;

        // textName由来のローカライズIDを引き継がない
        text.lang = null;
        text.text = string.Empty;
        text.raycastTarget = false;

        // 数値表示なので自動折り返しなどを無効にする
        text.resizeTextForBestFit = false;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.alignment = TextAnchor.MiddleCenter;

        // 元UIより手前に描画する.
        text.transform.SetAsLastSibling();

        return text;
    }

    public void RefreshIfNeeded()
    {
        if (_damageText == null || _damageShareText == null)
        {
            return;
        }
        RefreshAppearanceIfNeeded();
        RefreshValueIfNeeded();
    }

    private void RefreshAppearanceIfNeeded()
    {
        var config = ModContext.WorldConfig.Display;

        var damageAppearance = new TextAppearance(
            config.Damage.X, config.Damage.Y,
            config.Damage.SizeScale, config.Damage.HorizontalAlignment, config.Damage.Color,
            _baseText!.size, _baseText!.fontSize);
        var damageShareAppearance = new TextAppearance(
            config.DamageShare.X, config.DamageShare.Y,
            config.DamageShare.SizeScale, config.DamageShare.HorizontalAlignment, config.DamageShare.Color,
            _baseText!.size, _baseText!.fontSize);
        if (!_hasAppearance || damageAppearance != _lastDamageAppearance)
        {
            ApplyAppearance(_damageText!, damageAppearance);
            _lastDamageAppearance = damageAppearance;
        }
        if (!_hasAppearance || damageShareAppearance != _lastDamageShareAppearance)
        {
            ApplyAppearance(_damageShareText!, damageShareAppearance);
            _lastDamageShareAppearance = damageShareAppearance;
        }
        _hasAppearance = true;
    }

    private static void ApplyAppearance(UIText text, TextAppearance appearance)
    {
        var rect = text.rectTransform;

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);

        switch (appearance!.HorizontalAlignment)
        {
            case ModHorizontalTextAlignment.Left:
                rect.pivot = new Vector2(0f, 0.5f);
                text.alignment = TextAnchor.MiddleLeft;
                break;

            case ModHorizontalTextAlignment.Center:
                rect.pivot = new Vector2(0.5f, 0.5f);
                text.alignment = TextAnchor.MiddleCenter;
                break;

            case ModHorizontalTextAlignment.Right:
                rect.pivot = new Vector2(1f, 0.5f);
                text.alignment = TextAnchor.MiddleRight;
                break;
        }

        rect.anchoredPosition = new Vector2(appearance.X, appearance.Y);

        var fontSize = Mathf.Max(1, Mathf.RoundToInt(appearance.BaseFontSize * appearance.SizeScale));
        text.SetSize(appearance.BaseSize + fontSize - appearance.BaseFontSize);
        text.color = appearance.Color;
    }

    private void RefreshValueIfNeeded()
    {
        var damage = ModContext.DamageTracker.GetDamage(_uid);
        var totalDamage = ModContext.DamageTracker.TotalDamage;
        var config = ModContext.WorldConfig.Display;
        var displayDamage = config.Damage.Display;
        var displayDamageShare = config.DamageShare.Display;
        var displayNoDamage = config.DisplayNoDamage;
        if (_hasValue && _lastDamage == damage && _lastTotalDamage == totalDamage && _lastDisplayDamage == displayDamage && _lastDisplayDamageShare == displayDamageShare && _lastDisplayNoDamage == displayNoDamage)
        {
            return;
        }
        _lastDamage = damage;
        _lastTotalDamage = totalDamage;
        _lastDisplayDamage = displayDamage;
        _lastDisplayDamageShare = displayDamageShare;
        _lastDisplayNoDamage = displayNoDamage;
        _hasValue = true;

        if (damage <= 0 && !displayNoDamage)
        {
            _damageText!.text = string.Empty;
            _damageShareText!.text = string.Empty;
            return;
        }

        if (displayDamage)
        {
            _damageText!.text = $"{damage:N0}";
        }
        else
        {
             _damageText!.text = string.Empty;
        }

        if (displayDamageShare)
        {
            var percentage = totalDamage > 0 ? (double)damage / totalDamage * 100.0 : 0.0;
            _damageShareText!.text = $"{percentage:0.0}%";
        }
        else
        {
             _damageShareText!.text = string.Empty;
        }
    }
}
