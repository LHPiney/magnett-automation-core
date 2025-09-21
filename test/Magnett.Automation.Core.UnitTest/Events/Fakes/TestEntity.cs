using System.Threading;
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

    public Task DoSomethingAsync(string data, CancellationToken cancellationToken)
    {
        var testEvent = new TestEvent(data, nameof(TestEntity));
        
        return EmitEventAsync(testEvent, cancellationToken);
    }

    public Task EmitNullEventAsync()
    {
        return EmitEventAsync<IEvent>(null!);
    }

    public new void ClearLocalEvents()
    {
        base.ClearLocalEvents();
    }
}