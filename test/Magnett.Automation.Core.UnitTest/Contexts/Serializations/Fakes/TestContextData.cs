#nullable enable

namespace Magnett.Automation.Core.UnitTest.Contexts.Serializations.Fakes;

internal class TestContextData
{
    public string Name { get; set; } = string.Empty;
    public bool HasEventBus { get; set; }
    public bool HasLogger { get; set; }
}

