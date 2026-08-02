using FitHub.BankManager.Clients;
using FitHub.BankManager.Jobs.Consumers.Payments;
using FitHub.BankManager.Rabbit.Contracts.Payments;
using FitHub.Jobs.Workers;
using FitHub.RabbitMQ;
using FitHub.RabbitMQ.Configuration;

// TODO: добавить логгирование
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddBankManagerClients();
builder.Services.AddRabbitMq<RabbitMqClusterOptions>();
builder.Services.AddConsumerAsBackgroundService<PaymentIntentRequestedMessage, PaymentIntentRequestedConsumer, RabbitMqClusterOptions>();
builder.Services.AddHostedService<PaymentIntentAutoCompleteWorker>();
builder.Services.AddHostedService<RabbitOutboxPublisherWorker>();

var host = builder.Build();
host.Run();
