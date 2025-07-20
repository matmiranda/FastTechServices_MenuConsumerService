using MenuConsumerService.Infrastructure.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MenuConsumerService.Worker
{
    public class RabbitWorker : BackgroundService
    {
        private readonly ILogger<RabbitWorker> _logger;
        private readonly RabbitMQConsumer _consumer;

        public RabbitWorker(
            ILogger<RabbitWorker> logger,
            RabbitMQConsumer consumer)
        {
            _logger = logger;
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RabbitWorker iniciando o consumidor...");

            // Registra e inicia listeners de fila
            await _consumer.StartAsync(stoppingToken);

            // Mantém o worker vivo até cancelamento
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RabbitWorker solicitando parada do consumer...");
            await _consumer.StopAsync(cancellationToken);
            await base.StopAsync(cancellationToken);
        }
    }
}
