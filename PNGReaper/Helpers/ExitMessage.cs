using Prism.Events;

namespace PNGReaper.Helpers;

internal class ExitMessage
{
    public bool DeferShutdown { get; set; }
}

internal class ExitMessageEvent : PubSubEvent<ExitMessage>
{
}