using MenuConsumerService.Infrastructure.Messaging;

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

            try
            {
                await _consumer.StartAsync(stoppingToken);
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao iniciar o consumidor RabbitMQ.");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RabbitWorker solicitando parada do consumer...");
            await _consumer.StopAsync(cancellationToken);
            await base.StopAsync(cancellationToken);
        }
    }
}
