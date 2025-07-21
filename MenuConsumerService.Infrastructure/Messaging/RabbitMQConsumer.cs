using MenuConsumerService.Application.DTO;
using MenuConsumerService.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using MenuConsumerService.Application.DTO;


namespace MenuConsumerService.Infrastructure.Messaging
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQSettings _rabbitMqSettings;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQConsumer(ILogger<RabbitMQConsumer> logger, IServiceProvider serviceProvider, RabbitMQSettings rabbitMqSettings, IConnection connection = null, IModel channel = null)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _rabbitMqSettings = rabbitMqSettings;

            if (connection == null || channel == null)
            {
                try
                {
                    var factory = new ConnectionFactory()
                    {
                        HostName = _rabbitMqSettings.Host,
                        UserName = _rabbitMqSettings.Username,
                        Password = _rabbitMqSettings.Password
                    };

                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();
                    _channel.QueueDeclare(queue: _rabbitMqSettings.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                    _logger.LogInformation("Conectado ao RabbitMQ em {0} e aguardando mensagens na fila '{1}'...",
                        _rabbitMqSettings.Host, _rabbitMqSettings.QueueName);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Erro ao conectar ao RabbitMQ: {0}", ex.Message);
                    throw;
                }
            }
            else
            {
                _connection = connection;
                _channel = channel;
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);

                    _logger.LogInformation("Mensagem recebida: {0}", messageJson);

                    var jsonObject = JsonNode.Parse(messageJson);
                    var messageNode = jsonObject?["message"];

                    if (messageNode != null)
                    {
                        var menuJson = messageNode.ToString();
                        var menu = JsonSerializer.Deserialize<Application.DTO.MenuConsumerService.Application.DTO.MenuDto>(menuJson, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (menu != null)
                        {
                            if (string.IsNullOrWhiteSpace(menu.Action))
                            {
                                _logger.LogWarning("A propriedade 'Action' está vazia ou nula.");
                                _channel.BasicNack(ea.DeliveryTag, false, false);
                                return;
                            }

                            using var scope = _serviceProvider.CreateScope();
                            var menuService = scope.ServiceProvider.GetRequiredService<IMenuService>();

                            if (menu.Action.ToUpper() == "CREATE")
                            {
                                menu.Id = Guid.NewGuid();
                                menu.CreatedAt = DateTime.UtcNow;
                                menu.UpdatedAt = DateTime.UtcNow;
                                _logger.LogInformation("Criando novo item de menu com ID: {0}", menu.Id);
                            }
                            else if (menu.Action.ToUpper() == "UPDATE")
                            {
                                menu.UpdatedAt = DateTime.UtcNow;
                                _logger.LogInformation("Atualizando item de menu com ID existente: {0}", menu.Id);
                            }
                            else
                            {
                                _logger.LogWarning("Ação desconhecida recebida: {0}", menu.Action);
                                _channel.BasicNack(ea.DeliveryTag, false, false);
                                return;
                            }

                            var menuEntity = menu.ToEntity();
                            await menuService.SalvarMenuAsync(menuEntity);
                            _channel.BasicAck(ea.DeliveryTag, false);
                            _logger.LogInformation("Menu {0} salvo no banco!", menu.Id);
                        }
                        else
                        {
                            _logger.LogWarning("Falha ao desserializar o menu.");
                            _channel.BasicNack(ea.DeliveryTag, false, false);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("JSON recebido não contém a propriedade 'message'.");
                        _channel.BasicNack(ea.DeliveryTag, false, false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Erro ao processar mensagem: {0}", ex.Message);
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                }
            };

            _channel.BasicConsume(queue: _rabbitMqSettings.QueueName, autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }


        public override void Dispose()
        {
            _logger.LogInformation("Finalizando RabbitMQConsumer...");
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }

    public class RabbitMQSettings
    {
        public string Host { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string QueueName { get; set; } = string.Empty;
    }
}
