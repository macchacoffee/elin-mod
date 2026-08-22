namespace Macchacoffee.ElinMods.SomewhatEnhancedDisplay.UI.Adapters;

internal class ResistElementReal(Element RealElement) : IResistElement
{
    public int Id => RealElement.id;
    public int Value => RealElement.Value;
    public SourceElement.Row Source => RealElement.source;
}
