using System;
using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Events;
using Microsoft.Extensions.Logging;

namespace Magnett.Automation.Core.UnitTest.Events.Fakes;

/// <summary>
/// Handler de prueba para TestEvent
/// </summary>
public class TestEventHandler : IEventHandler<TestEvent>
{
    public bool WasHandled { get; private set; }
    public TestEvent LastHandledEvent { get; private set; }
    
    public Task Handle(TestEvent @event, ILogger logger, CancellationToken cancellationToken)
    {
        WasHandled = true;
        LastHandledEvent = @event;
        logger?.LogInformation("TestEventHandler handled event: {EventName}", @event.Name);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Otro handler para TestEvent
/// </summary>
public class AnotherTestEventHandler : IEventHandler<TestEvent>
{
    public bool WasHandled { get; private set; }
    public TestEvent LastHandledEvent { get; private set; }
    
    public Task Handle(TestEvent @event, ILogger logger, CancellationToken cancellationToken)
    {
        WasHandled = true;
        LastHandledEvent = @event;
        logger?.LogInformation("AnotherTestEventHandler handled event: {EventName}", @event.Name);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Handler para AnotherTestEvent
/// </summary>
public class AnotherTestEventEventHandler : IEventHandler<AnotherTestEvent>
{
    public bool WasHandled { get; private set; }
    public AnotherTestEvent LastHandledEvent { get; private set; }
    
    public Task Handle(AnotherTestEvent @event, ILogger logger, CancellationToken cancellationToken)
    {
        WasHandled = true;
        LastHandledEvent = @event;
        logger?.LogInformation("AnotherTestEventEventHandler handled event: {EventName}", @event.Name);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Handler que falla para probar manejo de errores
/// </summary>
public class FailingTestEventHandler : IEventHandler<TestEvent>
{
    public bool WasHandled { get; private set; }
    
    public Task Handle(TestEvent @event, ILogger logger, CancellationToken cancellationToken)
    {
        WasHandled = true;
        throw new InvalidOperationException("Simulated handler failure");
    }
}

/// <summary>
/// Clase que no es un handler válido (no implementa IEventHandler)
/// </summary>
public class InvalidHandler
{
    public void DoSomething() { }
}

/// <summary>
/// Clase abstracta que no debería ser registrada
/// </summary>
public abstract class AbstractTestEventHandler : IEventHandler<TestEvent>
{
    public abstract Task Handle(TestEvent @event, ILogger logger, CancellationToken cancellationToken);
} 