
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using SagaPattern.Orchestration.Shared.Messages;
using SagaPattern.Orchestration.Shared;
using System.Text;

namespace SagaPattern.Orchestration.OrchestratorService.Consumers
{
    public class ProductConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private EventingBasicConsumer _consumer;
        private IConnection? _messageConnection;
        private IModel? _messageChannel;

        public ProductConsumer(IServiceProvider serviceProvider, IConnection? messageConnection)
        {
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string queueName = "product-reserve-completed";

            _messageConnection = _serviceProvider.GetRequiredService<IConnection>();

            _messageChannel = _messageConnection.CreateModel();
            _messageChannel.QueueDeclare(queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _consumer = new EventingBasicConsumer(_messageChannel);
            _consumer.Received += ProcessMessageAsync;

            _messageChannel.BasicConsume(queue: queueName,
                autoAck: true,
                consumer: _consumer);

            return Task.CompletedTask;
        }

        private void ProcessMessageAsync(object sender, BasicDeliverEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Body.ToArray());
            ProductStockReservedMessage orderReceivedMessage = JsonConvert.DeserializeObject<ProductStockReservedMessage>(message)!;
        }

        private void SendMessageToQueue(IMessage message)
        {
            // Send message
            MessageSender.SendMessage("payment-pending", message, _messageConnection);
        }
    }
}
