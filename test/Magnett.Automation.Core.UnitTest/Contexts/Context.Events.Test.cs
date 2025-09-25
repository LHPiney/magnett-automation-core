using System.Linq;
using System.Threading.Tasks;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Contexts.Events;
using Magnett.Automation.Core.Events.Implementations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.Contexts;

public class ContextEventsTest
{
    private readonly ILogger<EventBus> _busLogger;
    private readonly ILogger<EventStream> _streamLogger;

    public ContextEventsTest()
    {
        var loggerFactory = new NullLoggerFactory();
        _busLogger = loggerFactory.CreateLogger<EventBus>();
        _streamLogger = loggerFactory.CreateLogger<EventStream>();
    }

    [Fact]
    public async Task WhenFieldChangeValue_ShouldEmitEvent()
    {
        var field = ContextField<string>.WithName("TestField");
        var contextVault = new Mock<IContextVault>();
        var context = Context.Create(contextVault.Object);

        await context.StoreAsync(field, "TestValue");

        var changeFieldValueEvent = context.LocalEvents.Select(e => e)
            .OfType<OnChangeFieldValueEvent>()
            .FirstOrDefault();

        Assert.NotNull(changeFieldValueEvent);
        Assert.Single(context.LocalEvents);
    }
}