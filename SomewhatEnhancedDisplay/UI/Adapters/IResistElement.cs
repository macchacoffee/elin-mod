namespace Macchacoffee.ElinMods.SomewhatEnhancedDisplay.UI.Adapters;

internal interface IResistElement
{
    public int Id { get; }
    public int Value { get; }
    public SourceElement.Row Source { get; }
}
