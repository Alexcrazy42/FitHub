using FitHub.Common.Extensions.Configuration;
using FitHub.RabbitMQ.Configuration;
using FitHub.RabbitMQ.Consumers;
using FitHub.RabbitMQ.Contracts;
using FitHub.RabbitMQ.Producers;
using Microsoft.Extensions.DependencyInjection;

namespace FitHub.RabbitMQ;

public static class ServiceRegistry
{
    /// <summary>
    /// Базовый метод регистрации нашей либы
    /// </summary>
    public static void AddRabbitMq<TOptions>(this IServiceCollection services)
        where TOptions : class, IRabbitMqOptions
    {
        services.AddBindedOptions<TOptions>();
        services.AddScoped<IRabbitMqConnection<TOptions>, RabbitMqConnection<TOptions>>();
    }

    public static void AddProducer<TMessage, TProducer, TOptions>(this IServiceCollection services)
        where TMessage : class, IRabbitMqContract
        where TProducer : class, IRabbitProducer<TMessage>
        where TOptions : class, IRabbitMqOptions
    {
        services.AddScoped<IRabbitProducer<TMessage>, RabbitMqProducer<TMessage, TProducer, TOptions>>();
    }

    public static void AddBasicProducer<TOptions>(this IServiceCollection services)
        where TOptions : class, IRabbitMqOptions
    {
        services.AddScoped<IBasicProducer<TOptions>, BasicRabbitProducer<TOptions>>();
    }


    /// <summary>
    /// Добавить консюмера, как BackgroundService
    /// </summary>
    public static void AddConsumerAsBackgroundService<TMessage, THandler, TOptions>(this IServiceCollection services)
        where TMessage : class, IRabbitMqContract
        where THandler : class, IRabbitMqConsumerHandler<TMessage>
        where TOptions : class, IRabbitMqOptions
    {
        services.AddScoped<IRabbitMqConsumerHandler<TMessage>, THandler>();
        services.AddHostedService<RabbitMqConsumer<TMessage, TOptions>>();
    }
}
