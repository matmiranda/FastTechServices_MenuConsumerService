using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using MenuConsumerService.Infrastructure.Messaging;

namespace MenuConsumerService.Test.UnitTests
{
    public class WorkerTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldProcessMessage()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<RabbitMQConsumer>>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            var rabbitMqSettings = new RabbitMQSettings { QueueName = "testQueue" };
            var connectionMock = new Mock<IConnection>();
            var channelMock = new Mock<IModel>();

            // Configurar o mock do canal para simular o comportamento do RabbitMQ
            channelMock.Setup(c => c.QueueDeclare(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>()));
            channelMock.Setup(c => c.BasicConsume(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>(), It.IsAny<IBasicConsumer>()))
                       .Returns("consumerTag");

            // Configurar o mock da conexão para retornar o canal mockado
            connectionMock.Setup(c => c.CreateModel()).Returns(channelMock.Object);

            // Criar uma instância do RabbitMQConsumer com mocks
            var consumer = new RabbitMQConsumer(loggerMock.Object, serviceProviderMock.Object, rabbitMqSettings, connectionMock.Object, channelMock.Object);

            // Capturar mensagens de log
            var logMessages = new List<string>();
            loggerMock.Setup(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
                .Callback<LogLevel, EventId, object, Exception, Delegate>((level, eventId, state, exception, formatter) =>
                {
                    if (state != null)
                    {
                        logMessages.Add(state.ToString() ?? string.Empty);
                    }
                });

            // Act
            await consumer.StartAsync(CancellationToken.None);

            // Simular o recebimento de uma mensagem
            var basicDeliverEventArgs = new BasicDeliverEventArgs
            {
                Body = Encoding.UTF8.GetBytes("{\"message\": {\"Nome\": \"Teste\"}}")
            };

            var eventingBasicConsumer = new EventingBasicConsumer(channelMock.Object);
            eventingBasicConsumer.Received += (model, ea) =>
            {
                // Processar a mensagem recebida
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                loggerMock.Object.LogInformation("Mensagem recebida: {0}", message);
            };

            // Simular a entrega da mensagem
            eventingBasicConsumer.HandleBasicDeliver("consumerTag", basicDeliverEventArgs.DeliveryTag, basicDeliverEventArgs.Redelivered, basicDeliverEventArgs.Exchange, basicDeliverEventArgs.RoutingKey, basicDeliverEventArgs.BasicProperties, basicDeliverEventArgs.Body);

            // Esperar um tempo para garantir que a mensagem seja processada
            await Task.Delay(500);

            // Assert
            Assert.Contains(logMessages, msg => msg.Contains("Mensagem recebida"));
        }
    }
}
