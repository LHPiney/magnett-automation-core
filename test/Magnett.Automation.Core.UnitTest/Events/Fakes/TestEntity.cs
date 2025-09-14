using System.Threading.Tasks;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Events.Implementations;

namespace Magnett.Automation.Core.UnitTest.Events.Fakes;

public class TestEntity(IEventBus? eventBus)
    : EventEmitterEntity(eventBus)
{

    public Task DoSomethingAsync(string data)
    {
        var testEvent = new TestEvent(data, nameof(TestEntity));
        
        return EmitEventAsync(testEvent);
    }
}