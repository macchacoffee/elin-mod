namespace Macchacoffee.ElinMods.SomewhatEnhancedDisplay.UI.Adapters;

internal class ResistElementMock(int id, int value, SourceElement.Row source) : IResistElement
{
    public int Id { get; } = id;
    public int Value { get; } = value;
    public SourceElement.Row Source { get; } = source;
}
