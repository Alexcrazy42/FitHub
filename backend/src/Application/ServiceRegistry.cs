using Amazon.Runtime;
using Amazon.S3;
using FitHub.Application.Files;
using FitHub.Application.Users;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.AspNetCore.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitHub.Application;

public static class ServiceRegistry
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationAssembly = typeof(ServiceRegistry).Assembly;

        var interfaces = applicationAssembly.GetTypes()
            .Where(t => t.IsInterface && t.Name.ToLower().EndsWith("service"))
            .ToList();

        foreach (var interfaceType in interfaces)
        {
            var implementation = applicationAssembly.GetTypes()
                .FirstOrDefault(t => t.IsClass && !t.IsAbstract && interfaceType.IsAssignableFrom(t));

            if (implementation != null)
            {
                services.AddTransient(interfaceType, implementation);
            }
        }

        services.AddTransient<IIdentityUserService, IdentityUserService>();
        services.AddTransient<IAuthenticationService, IdentityUserService>();
        services.AddFiles(configuration);

        return services;
    }

    private static void AddFiles(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IS3FileService, S3FileService>();
        services.AddSingleton<IAmazonS3>(sp => new AmazonS3Client(
            new BasicAWSCredentials(configuration["AWS:AccessKey"], configuration["AWS:SecretKey"]),
            new AmazonS3Config
            {
                ServiceURL = configuration["AWS:ServiceURL"],
                ForcePathStyle = true,
                UseHttp = true,
                Timeout = TimeSpan.FromSeconds(20),
                MaxErrorRetry = 3,
                AuthenticationRegion = "us-east-1",
            }
        ));
    }
}
