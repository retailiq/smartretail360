namespace SmartRetail360.Messaging.Interfaces;

public interface IEmailQueueProducer
{
    Task SendAsync(object message);
}