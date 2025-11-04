namespace FitHub.RabbitMQ.Producers;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ProducerAttribute : Attribute
{
    public string ExchangeType { get; }


    public ProducerAttribute(string exchangeType)
    {
        ExchangeType = exchangeType;
    }
}
