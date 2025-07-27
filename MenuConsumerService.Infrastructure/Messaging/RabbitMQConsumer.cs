using MenuConsumerService.Application.DTO;
using MenuConsumerService.Application.DTO.MenuConsumerService.Application.DTO;
using MenuConsumerService.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace MenuConsumerService.Infrastructure.Messaging
{
    public class RabbitMQConsumer : IDisposable
    {
        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQSettings _settings;
        private IConnection? _connection;
        private IModel? _channel;

        public RabbitMQConsumer(
            ILogger<RabbitMQConsumer> logger,
            IServiceProvider serviceProvider,
            RabbitMQSettings settings)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _settings = settings;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                UserName = _settings.Username,
                Password = _settings.Password,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "menu.item.registered", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueDeclare(queue: "menu.item.updated", durable: true, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (_, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    _logger.LogInformation("Mensagem recebida da fila '{0}': {1}", ea.RoutingKey, json);

                    var menuDto = JsonSerializer.Deserialize<MenuDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (menuDto == null)
                    {
                        _logger.LogWarning("Falha ao desserializar o menu.");
                        _channel.BasicNack(ea.DeliveryTag, false, false);
                        return;
                    }

                    using var scope = _serviceProvider.CreateScope();
                    var menuService = scope.ServiceProvider.GetRequiredService<IMenuService>();

                    if (ea.RoutingKey.Equals("menu.item.registered", StringComparison.OrdinalIgnoreCase))
                    {
                        menuDto.CreatedAt = DateTime.UtcNow.AddHours(-3);
                        menuDto.UpdatedAt = DateTime.UtcNow.AddHours(-3);
                        _logger.LogInformation("Criando novo menu com ID {0}", menuDto.Id);
                        var entity = menuDto.ToEntity();
                        await menuService.SalvarMenuAsync(entity);
                    }
                    else if (ea.RoutingKey.Equals("menu.item.updated", StringComparison.OrdinalIgnoreCase))
                    {
                        menuDto.UpdatedAt = DateTime.UtcNow.AddHours(-3);
                        _logger.LogInformation("Atualizando menu com ID existente {0}", menuDto.Id);
                        var entity = menuDto.ToEntity();
                        await menuService.AtualizarMenuAsync(entity);
                    }
                    else
                    {
                        _logger.LogWarning("Fila desconhecida: {0}", ea.RoutingKey);
                        _channel.BasicNack(ea.DeliveryTag, false, false);
                        return;
                    }

                    _channel.BasicAck(ea.DeliveryTag, false);
                    _logger.LogInformation("Menu {0} salvo com sucesso!", menuDto.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar mensagem da fila '{0}'", ea.RoutingKey);
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                }
            };

            _channel.BasicConsume(queue: "menu.item.registered", autoAck: false, consumer: consumer);
            _channel.BasicConsume(queue: "menu.item.updated", autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Parando RabbitMQConsumer...");
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _logger.LogInformation("Finalizando RabbitMQConsumer...");
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }

    public class RabbitMQSettings
    {
        public string Host { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}