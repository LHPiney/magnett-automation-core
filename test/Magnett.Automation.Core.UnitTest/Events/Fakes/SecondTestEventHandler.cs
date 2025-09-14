using System;
using System.Threading.Tasks;
using Magnett.Automation.Core.Events;
using Microsoft.Extensions.Logging;

namespace Magnett.Automation.Core.UnitTest.Events.Fakes;

/// <summary>
/// Handler de prueba para TestEvent
/// </summary>
public class SecondTestEventHandler : IEventHandler<SecondTestEvent>
{
    public bool WasHandled { get; private set; }
    public SecondTestEvent LastHandledEvent { get; private set; }
    
    public Task Handle(SecondTestEvent @event, ILogger logger)
    {
        WasHandled = true;
        LastHandledEvent = @event;
        logger?.LogInformation("SecondTestEvent handled event: {EventName}", @event.Name);
        return Task.CompletedTask;
    }
}