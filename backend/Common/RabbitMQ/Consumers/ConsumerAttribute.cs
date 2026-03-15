namespace FitHub.RabbitMQ.Consumers;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ConsumerAttribute : Attribute
{
    public string QueueName { get; }

    public string BindingRoutingKey { get; }

    public ConsumerAttribute(string queueName, string bindingRoutingKey)
    {
        QueueName = queueName;
        BindingRoutingKey = bindingRoutingKey;
    }
}
