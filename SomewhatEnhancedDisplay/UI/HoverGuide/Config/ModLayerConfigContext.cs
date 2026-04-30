using System;
using System.Collections.Generic;
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

    public Chara SampleChara { get; set; }
    public Thing SampleThing { get; set; }

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

    private Chara PickSampleCharaRandom()
    {
        return PickSampleCharaRandom(false);
    }

    private Chara PickSampleCharaRandom(bool includesPC)
    {
        var chara = EClass.pc;
        var members = EClass.pc.party.members;
        members = includesPC ? [EClass.pc, ..members] : members;
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
}
