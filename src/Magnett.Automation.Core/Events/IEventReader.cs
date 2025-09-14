using System.Threading.Channels;

namespace Magnett.Automation.Core.Events;

public interface IEventReader : IAsyncDisposable
{
    ChannelReader<IEvent> Reader { get; }
    Task Completion { get; }
} 