using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Core.UnitTest.Events.Fakes;

public record EventMock(string Name, string Caller) : Event(Name, Caller);