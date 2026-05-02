using System;
using System.Collections.Generic;
using System.Linq;
using SomewhatEnhancedDisplay.Config;

namespace SomewhatEnhancedDisplay.UI.HoverGuide.Config;

public class ModLayerConfigContext
{
    public int SelectedStyleIndex
    {
        get;
        set
        {
            var oldValue = field;
            field = value;
            foreach (var listener in StyleChangedListeners)
            {
                listener(value, oldValue);
            }
        }
    } = 0;

    private List<Action<int, int>> StyleChangedListeners { get; } = [];
    private List<Action<ModConfigHoverGuideStyle, int>> StyleAddedListeners { get; } = [];
    private List<Action<ModConfigHoverGuideStyle, int>> StyleDeletedListeners { get; } = [];
    private List<Action<int, int>> StyleMovedListeners { get; } = [];

    public Chara SampleChara { get; set; }
    public Thing SampleThing { get; set; }
    public ModHoverGuideTargetModifier SampleModifier { get; set; } = new(healthBarRatio: 1);

    private static ModConfigHoverGuide Config => Mod.Config.HoverGuide;
    public ModConfigHoverGuideStyle SelectedStyle => Config.Styles[SelectedStyleIndex];
    public string SelectedStyleName => GetStyleName(SelectedStyleIndex, SelectedStyle);

    public ModLayerConfigContext()
    {
        SampleChara = PickSampleCharaRandom();
        SampleThing = PickSampleThingRandom();
    }

    public ModLayerConfigContext(Chara sampleChara, Thing sampleThing)
    {
        SampleChara = sampleChara;
        SampleThing = sampleThing;
    }

    public static string GetStyleName(int index, ModConfigHoverGuideStyle style)
    {
        return  $"{index + 1}. {style.Name}";
    }

    public void AddSelectedStyleChangedListener(Action<int, int> listener)
    {
        StyleChangedListeners.Add(listener);
    }

    public void AddStyleAddedListener(Action<ModConfigHoverGuideStyle, int> listener)
    {
        StyleAddedListeners.Add(listener);
    }

    public void AddStyleDeletedListener(Action<ModConfigHoverGuideStyle, int> listener)
    {
        StyleDeletedListeners.Add(listener);
    }

     public void AddStyleMovedListener(Action<int, int> listener)
    {
        StyleMovedListeners.Add(listener);
    }

    private Chara PickSampleCharaRandom()
    {
        return PickSampleCharaRandom(false);
    }

    private Chara PickSampleCharaRandom(bool includesPC)
    {
        var chara = EClass.pc;
        var members = EClass.pc.party.members;
        members = includesPC ? EClass.pc.party.members : [.. members.Where(c => !c.IsPC)];
        if (members.Count > 0)
        {
            chara = members.RandomItem();
        }
        return chara;
    }

    private Thing PickSampleThingRandom()
    {
        return EClass._zone.map.things.RandomItem();
    }

    public void UpdateSampleCharaRandom()
    {
        SampleChara = PickSampleCharaRandom();
    }

    public void UpdateSampleThingRandom()
    {
        SampleThing = PickSampleThingRandom();
    }

    public void AddStyle(ModConfigHoverGuideStyle style)
    {
        Config.Styles.Add(style);
        var index = Config.Styles.Count - 1;
        SelectedStyleIndex = index;
        foreach (var listener in StyleAddedListeners)
        {
            listener(style, index);
        }
    }

    public void DeleteStyle(int index)
    {
        var style = Config.Styles[index];
        Config.Styles.RemoveAt(index);
        SelectedStyleIndex = Math.Max(0, SelectedStyleIndex - 1);
        foreach (var listener in StyleDeletedListeners)
        {
            listener(style, index);
        }
    }

    public void MoveStyleBackward(int index)
    {
        MoveStyleWithOffset(index, -1);
    }

    public void MoveStyleForward(int index)
    {
        MoveStyleWithOffset(index, 1);
    }

    private void MoveStyleWithOffset(int index, int offset)
    {
        var moveTo = (index + offset) % Config.Styles.Count;
        moveTo += moveTo < 0 ? Config.Styles.Count : 0;
        (Config.Styles[index], Config.Styles[moveTo]) = (Config.Styles[moveTo], Config.Styles[index]);
        SelectedStyleIndex = moveTo;
        foreach (var listener in StyleMovedListeners)
        {
            listener(moveTo, index);
        }
    }
}
