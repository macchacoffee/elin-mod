namespace SomewhatEnhancedDisplay.UI.Adapters;

internal class ModElementReal(Element RealElement) : IModElement
{
    public int Id => RealElement.id;
    public int Value => RealElement.Value;
    public SourceElement.Row Source => RealElement.source;
}
