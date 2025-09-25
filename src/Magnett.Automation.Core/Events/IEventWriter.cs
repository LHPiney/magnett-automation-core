using System.Threading.Channels;

namespace Magnett.Automation.Core.Events;

public interface IEventWriter : IAsyncDisposable
{
    ChannelWriter<IEvent> Writer { get; }
} 