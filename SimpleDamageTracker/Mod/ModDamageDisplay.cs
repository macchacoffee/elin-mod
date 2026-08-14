
using SimpleDamageTracker;
using UnityEngine;

internal sealed class ModDamageDisplay : MonoBehaviour
{
    private readonly record struct TextLayout(float X, float Y, int Size);

    private UIText? _damageText;
    private UIText? _percentageText;

    private int _uid;

    private long _lastDamage = -1;
    private long _lastTotalDamage = -1;

    private TextLayout _lastDamageLayout;
    private TextLayout _lastPercentageLayout;
    private bool _hasLayout;

    public void Bind(ButtonRoster roster, Chara chara)
    {
        _uid = chara.uid;

        _lastDamage = -1;
        _lastTotalDamage = -1;

        EnsureTexts(roster);
    }

    private void EnsureTexts(ButtonRoster roster)
    {
        if (_damageText == null)
        {
            _damageText = CreateText(roster, ModConsts.GameObjectName.DamageDisplayDamage);
        }

        if (_percentageText == null)
        {
            _percentageText = CreateText(roster, ModConsts.GameObjectName.DamageDisplayPercentage);
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
        if (_damageText == null || _percentageText == null)
        {
            return;
        }
        RefreshLayoutIfNeeded();
        RefreshValueIfNeeded();
    }

    private void RefreshLayoutIfNeeded()
    {
        var config = ModContext.WorldConfig.Display;
        var damageLayout = new TextLayout(config.Damage.X, config.Damage.Y, config.Damage.Size);
        var percentageLayout = new TextLayout(config.Percentage.X, config.Percentage.Y, config.Percentage.Size);
        if (!_hasLayout || damageLayout != _lastDamageLayout)
        {
            ApplyLayout(_damageText!, damageLayout);
            _lastDamageLayout = damageLayout;
        }
        if (!_hasLayout || percentageLayout != _lastPercentageLayout)
        {
            ApplyLayout(_percentageText!, percentageLayout);
            _lastPercentageLayout = percentageLayout;
        }

        _hasLayout = true;
    }

    private static void ApplyLayout(UIText text, TextLayout layout)
    {
        var rect = text.rectTransform;

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        rect.anchoredPosition = new Vector2(layout.X, layout.Y);

        text.SetSize(layout.Size);
    }

    private void RefreshValueIfNeeded()
    {
        var damage = ModContext.DamageTracker.GetDamage(_uid);
        var totalDamage = ModContext.DamageTracker.TotalDamage;
        if (_lastDamage == damage && _lastTotalDamage == totalDamage)
        {
            return;
        }
        _lastDamage = damage;
        _lastTotalDamage = totalDamage;

        if (damage <= 0 && !ModContext.WorldConfig.Display.DisplayNoDamage)
        {
            _damageText!.text = string.Empty;
            _percentageText!.text = string.Empty;
            return;
        }

        var percentage = totalDamage > 0 ? (double)damage / totalDamage * 100.0 : 0.0;

        _damageText!.text = $"{damage:N0}";
        _percentageText!.text = $"{percentage:0.0}%";
    }
}
