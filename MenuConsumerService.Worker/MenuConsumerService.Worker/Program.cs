using Serilog;
using MenuConsumerService.Infrastructure.Persistence;
using MenuConsumerService.Application.Interfaces;
using MenuConsumerService.Infrastructure.Services;
using MenuConsumerService.Infrastructure.Messaging;
using MenuConsumerService.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// grava logs em um arquivo no kubernete k8s azure
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("/app/logs/criar-item-menu/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureAppConfiguration((context, config) =>
    {
        // Carrega o arquivo appsettings.json
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        // Carrega as configurações do RabbitMQ do appsettings.json
        var rabbitMqSettings = context.Configuration.GetSection("RabbitMQSettings").Get<RabbitMQSettings>();

        if (rabbitMqSettings == null)
            throw new InvalidOperationException("RabbitMQSettings não pode ser nulo. Verifique o arquivo appsettings.json.");

        services.AddSingleton(rabbitMqSettings);

        // Registrando serviços e repositórios
        services.AddScoped<IMenuRepository, MenuRepository>();
        services.AddScoped<IMenuService, MenuService>();

        // Registrando o RabbitMQConsumer como Singleton e o Worker como HostedService
        services.AddSingleton<RabbitMQConsumer>();
        services.AddHostedService<RabbitWorker>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

await host.RunAsync();
