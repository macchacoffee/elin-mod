using DG.Tweening;
using UnityEngine;
using Macchacoffee.ElinMods.SimpleDamageTracker.Config;

namespace Macchacoffee.ElinMods.SimpleDamageTracker.Mod;

internal sealed class ModDamageDisplay : MonoBehaviour
{
    private const float _animationDuration = 0.2f;

    private readonly record struct TextAppearance(
        float X,
        float Y,
        float SizeScale,
        ModHorizontalTextAlignment HorizontalAlignment,
        Color Color,
        int BaseSize,
        int BaseFontSize
    );

    private readonly record struct ValueState(
        long Damage,
        long TotalDamage,
        bool DisplayDamage,
        bool DisplayDamageShare,
        bool DisplayNoDamage,
        bool UseAnimation,
        bool UseCompactDamageFormat
    )
    {
        public double DamageShare => TotalDamage > 0 ? (double)Damage / TotalDamage * 100.0 : 0.0;
    }

    private UIText? _baseText;
    private UIText? _damageText;
    private UIText? _damageShareText;

    private int _uid;

    private TextAppearance _lastDamageAppearance;
    private TextAppearance _lastDamageShareAppearance;
    private bool _hasAppearance;

    private ValueState _lastValueState;
    private bool _hasValueState;

    private long _displayDamage;
    private double _displayDamageShare;

    private Tween? _damageTween;
    private Tween? _damageShareTween;

    public void Bind(ButtonRoster roster, Chara chara)
    {
        KillValueTweens();

        _uid = chara.uid;
        _baseText = roster.textName;

        _hasValueState = false;

        CreateTexts(roster);
        RefreshIfNeeded();
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

        text.lang = null;
        text.text = string.Empty;
        text.raycastTarget = false;

        text.resizeTextForBestFit = false;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.alignment = TextAnchor.MiddleCenter;

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
            X: config.Damage.X,
            Y: config.Damage.Y,
            SizeScale: config.Damage.SizeScale,
            HorizontalAlignment: config.Damage.HorizontalAlignment,
            Color: config.Damage.Color,
            BaseSize: _baseText!.size,
            BaseFontSize: _baseText!.fontSize
        );
        var damageShareAppearance = new TextAppearance(
            X: config.DamageShare.X,
            Y: config.DamageShare.Y,
            SizeScale: config.DamageShare.SizeScale,
            HorizontalAlignment: config.DamageShare.HorizontalAlignment,
            Color: config.DamageShare.Color,
            BaseSize: _baseText!.size,
            BaseFontSize: _baseText!.fontSize
        );

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
        var config = ModContext.WorldConfig.Display;

        var state = new ValueState(
            Damage: ModContext.DamageTracker.GetDamage(_uid),
            TotalDamage: ModContext.DamageTracker.TotalDamage,
            DisplayDamage: config.Damage.Display,
            DisplayDamageShare: config.DamageShare.Display,
            DisplayNoDamage: config.DisplayNoDamage,
            UseAnimation: config.UseAnimation,
            UseCompactDamageFormat: config.UseCompactDamageFormat
        );

        if (_hasValueState && state == _lastValueState)
        {
            return;
        }

        var hasPreviousState = _hasValueState;
        var previousState = _lastValueState;

         var valueChanged = !hasPreviousState
            || state.Damage != previousState.Damage
            || state.TotalDamage != previousState.TotalDamage;
         var displaySettingChanged = !hasPreviousState
            || state.DisplayDamage != previousState.DisplayDamage
            || state.DisplayDamageShare != previousState.DisplayDamageShare
            || state.DisplayNoDamage != previousState.DisplayNoDamage
            || state.UseAnimation != previousState.UseAnimation
            || state.UseCompactDamageFormat != previousState.UseCompactDamageFormat;

        // 通常の計測中はDamage/TotalDamageとも増加しかしない
        // 減少した場合はリセットとみなしてアニメーションしない
         var reset = hasPreviousState
            && (state.Damage < previousState.Damage || state.TotalDamage < previousState.TotalDamage);

        _lastValueState = state;
        _hasValueState = true;

        var animates = state.UseAnimation
            && hasPreviousState
            && valueChanged
            && !displaySettingChanged
            && !reset
            && (state.Damage > 0 || state.DisplayNoDamage);

        if (animates)
        {
            AnimateTo(state);
        }
        else
        {
            SetValueImmediately(state);
        }
    }

    private void AnimateTo(ValueState state)
    {
        KillValueTweens();

        var damageShare = state.DamageShare;
        if (state.DisplayDamage && _displayDamage != state.Damage)
        {
            _damageTween = DOTween.To(
                () => _displayDamage,
                value =>
                {
                    _displayDamage = value;
                    ApplyDamageText();
                },
                state.Damage,
                _animationDuration)
            .SetEase(Ease.OutQuad)
            .SetLink(gameObject);
        }
        else
        {
            _displayDamage = state.Damage;
            ApplyDamageText();
        }

        if (state.DisplayDamageShare && _displayDamageShare != damageShare)
        {
            _damageShareTween = DOTween.To(
                () => _displayDamageShare,
                value =>
                {
                    _displayDamageShare = value;
                    ApplyDamageShareText();
                },
                damageShare,
                _animationDuration)
            .SetEase(Ease.OutQuad)
            .SetLink(gameObject);
        }
        else
        {
            _displayDamageShare = damageShare;
            ApplyDamageShareText();
        }
    }
    private void SetValueImmediately(ValueState state)
    {
        KillValueTweens();

        _displayDamage = state.Damage;
        _displayDamageShare = state.DamageShare;

        ApplyDamageText();
        ApplyDamageShareText();
    }

    private void ApplyDamageText()
    {
        if (!_lastValueState.DisplayDamage || !DisplaysCurrentValue())
        {
            _damageText!.text = string.Empty;
            return;
        }
        _damageText!.text = ModDamageFormatter.Format(_displayDamage, _lastValueState.UseCompactDamageFormat);
    }

    private void ApplyDamageShareText()
    {
        if (!_lastValueState.DisplayDamageShare || !DisplaysCurrentValue())
        {
            _damageShareText!.text = string.Empty;
            return;
        }
        _damageShareText!.text = $"{_displayDamageShare:0.0}%";
    }

    private bool DisplaysCurrentValue()
    {
        return _lastValueState.Damage > 0 || _lastValueState.DisplayNoDamage;
    }

    private void KillValueTweens()
    {
        _damageTween?.Kill();
        _damageTween = null;
        _damageShareTween?.Kill();
        _damageShareTween = null;
    }
}
