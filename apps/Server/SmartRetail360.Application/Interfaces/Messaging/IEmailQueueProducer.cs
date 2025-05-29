namespace SmartRetail360.Application.Interfaces.Messaging;

public interface IEmailQueueProducer
{
    Task SendAsync(object message);
}