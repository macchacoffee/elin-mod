using NPOI.HSSF.Record;

namespace SomewhatEnhancedDisplay.UI.Adapters;

public class ModElementMock(int id, int value, SourceElement.Row source) : IModElement
{
    public int Id { get; } = id;
    public int Value { get; } = value;
    public SourceElement.Row Source { get; } = source;
}
