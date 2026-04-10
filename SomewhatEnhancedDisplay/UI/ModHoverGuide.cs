using System;
using SomewhatEnhancedDisplay.Config;
using SomewhatEnhancedDisplay.Extensions;

namespace SomewhatEnhancedDisplay.UI;

public class ModHoverGuide
{
    private ModHealthBar HealthBar1 { get; }
    private UIText TextName2 { get; }
    private UIText TextName3 { get; }
    private ModHealthBar HealthBar2 { get; }
    private UIText TextName4 { get; }

    private int BaseFontSize { get; }

    public ModHoverGuide(WidgetMouseover widget)
    {
        HealthBar1 = new(widget);

        TextName2 = UnityEngine.Object.Instantiate(widget.textName);
        TextName2.transform.SetParent(widget.layout.transform);
        TextName2.transform.localScale = widget.textName.transform.localScale;

        TextName3 = UnityEngine.Object.Instantiate(widget.textName);
        TextName3.transform.SetParent(widget.layout.transform);
        TextName3.transform.localScale = widget.textName.transform.localScale;

        HealthBar2 = new(widget);

        TextName4 = UnityEngine.Object.Instantiate(widget.textName);
        TextName4.transform.SetParent(widget.layout.transform);
        TextName4.transform.localScale = widget.textName.transform.localScale;

        // ゲーム設定のウィジェットのフォントサイズが最小の場合を基準とする
        BaseFontSize = widget.textName.fontSize - EClass.core.config.font.fontWidget.size;
    }

    public void Show(WidgetMouseover widget, string? text, string? text2, string? text3, string? text4, Card? target1, Card? target2)
    {
        var fontSize = ModUIUtil.ComputeFontSize(BaseFontSize);

        var isGroup1Enabled = false;
        if (!string.IsNullOrEmpty(text))
        {
            widget.textName.fontSize = fontSize;
            widget.textName.enabled = true;
            isGroup1Enabled = true;
        }
        else
        {
            widget.textName.enabled = false;
        }
        if (target1 is Chara chara1)
        {
            var enabled = DisplaysHealthBar(chara1);
            HealthBar1.Enabled = enabled;
            HealthBar1.Update(chara1);
            isGroup1Enabled = enabled;
        }
        else
        {
            HealthBar1.Enabled = false;
        }
        if (!string.IsNullOrEmpty(text2))
        {
            TextName2.fontSize = fontSize;
            TextName2.text = text2.TagSize(fontSize);
            TextName2.enabled = true;
            isGroup1Enabled = true;
        }
        else
        {
            TextName2.text = string.Empty;
            TextName2.enabled = false;
        }

        if (!string.IsNullOrEmpty(text3))
        {
            if (isGroup1Enabled)
            {
                text3 = $"{Environment.NewLine}{text3}";
            }
            TextName3.fontSize = fontSize;
            TextName3.text = text3.TagSize(fontSize);
            TextName3.enabled = true;
        }
        else
        {
            TextName3.text = string.Empty;
            TextName3.enabled = false;
        }
        if (target2 is Chara chara2)
        {
            HealthBar2.Enabled = DisplaysHealthBar(chara2);
            HealthBar2.Update(chara2);
        }
        else
        {
            HealthBar2.Enabled = false;
        }
        if (!string.IsNullOrEmpty(text4))
        {
            TextName4.fontSize = fontSize;
            TextName4.text = text4.TagSize(fontSize);
            TextName4.enabled = true;
        }
        else
        {
            TextName4.text = string.Empty;
            TextName4.enabled = false;
        }

        widget.Show(text?.TagSize(fontSize));
    }

    public void ShowForManager(WidgetMouseover widget)
    {
        widget.textName.enabled = true;
        HealthBar1.Enabled = false;
        TextName2.enabled = false;
        TextName3.enabled = false;
        HealthBar2.Enabled = false;
        TextName4.enabled = false;
    }

    private bool DisplaysHealthBar(Chara chara)
    {
        var configProfile = Mod.Config.HoverGuide.CurrentProfile;
        if (!configProfile.DisplayHealthBar)
        {
            return false;
        }

        var config = configProfile.HealthBar;
        switch (chara.hostility)
        {
            case Hostility.Enemy:
                return DisplaysHealthBar(chara, config.DisplayForEnemy);
            case Hostility.Neutral:
                return DisplaysHealthBar(chara, config.DisplayForNetural);
            case Hostility.Friend:
                return DisplaysHealthBar(chara, config.DisplayForFriend);
            case Hostility.Ally:
                return DisplaysHealthBar(chara, config.DisplayForAlly);
            default:
                return false;
        }
    }

    private bool DisplaysHealthBar(Chara chara, ModHealthBarDisplay config)
    {
        var displays = config.Target switch
        {
            ModHealthBarDisplayTarget.None => false,
            ModHealthBarDisplayTarget.Elite => chara.IsPowerful,
            ModHealthBarDisplayTarget.All => true,
            _ => false,
        };
        if (!displays)
        {
            return false;
        }

        if (!config.NotInCombat && !chara.IsInCombat)
        {
            return false;
        }

        return config.InFullHealth || !chara.IsInFullHealth;
    }
}
