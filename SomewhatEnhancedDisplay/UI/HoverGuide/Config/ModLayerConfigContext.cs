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
            field = value;
            foreach (var listener in StyleChangedListeners)
            {
                listener(value);
            }
        }
    } = 0;

    private List<Action<int>> StyleChangedListeners { get; } = [];
    private List<Action> StyleAddedListeners { get; } = [];
    private List<Action> StyleDeletedListeners { get; } = [];

    public Chara SampleChara { get; set; }
    public Thing SampleThing { get; set; }
    public ModHoverGuideTargetModifier SampleModifier { get; set; } = new(healthBarRatio: 1);

    private static ModConfigHoverGuide Config => Mod.Config.HoverGuide;
    public ModConfigHoverGuideStyle SelectedStyle => Config.Styles[SelectedStyleIndex];

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

    public void AddSelectedStyleChangedListener(Action<int> listener)
    {
        StyleChangedListeners.Add(listener);
    }

    public void AddStyleAddedListener(Action listener)
    {
        StyleAddedListeners.Add(listener);
    }

    public void AddStyleDeletedListener(Action listener)
    {
        StyleDeletedListeners.Add(listener);
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
        SelectedStyleIndex = Config.Styles.Count - 1;
        foreach (var listener in StyleAddedListeners)
        {
            listener();
        }
    }

    public void DeleteStyle(int index)
    {
        Config.Styles.RemoveAt(index);
        SelectedStyleIndex = Math.Max(0, SelectedStyleIndex - 1);
        foreach (var listener in StyleDeletedListeners)
        {
            listener();
        }
    }
}
