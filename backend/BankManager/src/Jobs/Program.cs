using FitHub.BankManager.Clients;
using FitHub.BankManager.Jobs.Consumers.Payments;
using FitHub.BankManager.Rabbit.Contracts.Payments;
using FitHub.Common.Logging;
using FitHub.Common.Telemetry.Extensions;
using FitHub.Jobs.Workers;
using FitHub.RabbitMQ;
using FitHub.RabbitMQ.Configuration;
using HostOptions = FitHub.BankManager.Jobs.HostOptions;


var host = Host.CreateDefaultBuilder(args)
    .UseCommonLogger<HostOptions>()
    .ConfigureServices((ctx, services) =>
    {

        services.AddBankManagerClients();
        services.AddCommonTelemetry(ctx.Configuration);
        services.AddRabbitMq<RabbitMqClusterOptions>();

        services.AddConsumerAsBackgroundService<PaymentIntentRequestedMessage, PaymentIntentRequestedConsumer, RabbitMqClusterOptions>();
        services.AddHostedService<PaymentIntentAutoCompleteWorker>();
        services.AddHostedService<RabbitOutboxPublisherWorker>();
    })
    .Build();

host.Run();
