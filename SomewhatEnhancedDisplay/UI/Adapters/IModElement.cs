namespace SomewhatEnhancedDisplay.UI.Adapters;

public interface IModElement
{
    public int Id { get; }
    public int Value { get; }
    public SourceElement.Row Source { get; }
}
