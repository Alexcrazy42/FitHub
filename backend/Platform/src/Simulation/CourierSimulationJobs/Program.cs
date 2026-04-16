using FitHub.Clients;
using FitHub.CourierSimulationJobs;
using FitHub.Queue.Contracts.Marketplace;
using FitHub.RabbitMQ;
using FitHub.RabbitMQ.Configuration;
using FitHub.Simulation.CourierSimulationJobs;
using FitHub.Simulation.CourierSimulationJobs.Consumers;
using FitHub.Simulation.CourierSimulationJobs.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<CourierSimulationOptions>(
    builder.Configuration.GetSection(CourierSimulationOptions.SectionName));
builder.Services.AddRabbitMq<RabbitMqClusterOptions>();
builder.Services.AddFitHubClients();


// TODO: тут нужно как-то сделать симуляцию движения курьеров в нужную точку
builder.Services.AddConsumerAsBackgroundService<CourierDeliveryAssignedMessage, CourierDeliveryAssignedConsumer, RabbitMqClusterOptions>();
builder.Services.AddHostedService<CourierSimulationBootstrapWorker>();

var host = builder.Build();
host.Run();
