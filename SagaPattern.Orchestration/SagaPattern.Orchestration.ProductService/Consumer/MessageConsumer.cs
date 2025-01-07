
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using SagaPattern.Orchestration.Shared;
using SagaPattern.Orchestration.Shared.Messages;
using Newtonsoft.Json;

namespace SagaPattern.Orchestration.ProductService.Consumer
{
    public class MessageConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private EventingBasicConsumer _consumer;
        private IConnection? _messageConnection;
        private IModel? _messageChannel;

        public MessageConsumer(IServiceProvider serviceProvider, IConnection? messageConnection)
        {
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string queueName = "product-reserve-pending";

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
            ProductStockReservePendingMessage productStockReservePendingMessage = JsonConvert.DeserializeObject<ProductStockReservePendingMessage>(message)!;

            ProductStockReservedMessage productStockReservedMessage = new()
            {
                PaymentId = productStockReservePendingMessage.PaymentId,
                IsCompleted = false,
                OrderId = productStockReservePendingMessage.OrderId,
            };

            SendMessageToQueue(productStockReservedMessage);
        }

        private void SendMessageToQueue(IMessage message)
        {
            // Send message
            MessageSender.SendMessage("product-reserve-completed", message, _messageConnection);


        }
    }
}
