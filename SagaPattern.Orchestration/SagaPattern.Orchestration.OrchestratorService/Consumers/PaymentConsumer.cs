
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using SagaPattern.Orchestration.Shared.Messages;
using SagaPattern.Orchestration.Shared;
using System.Text;

namespace SagaPattern.Orchestration.OrchestratorService.Consumers
{
    public class PaymentConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private EventingBasicConsumer _consumer;
        private IConnection? _messageConnection;
        private IModel? _messageChannel;

        public PaymentConsumer(IServiceProvider serviceProvider, IConnection? messageConnection)
        {
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string queueName = "payment-completed";

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
            IMessage sendingMessage;
            string message = Encoding.UTF8.GetString(e.Body.ToArray());
            PaymentCompletedMessage paymentCompletedMessage = JsonConvert.DeserializeObject<PaymentCompletedMessage>(message)!;

            ProductStockReservePendingMessage productStockReservePendingMessage = new()
            {
                PaymentId = paymentCompletedMessage.PaymentId,
                OrderId = paymentCompletedMessage.OrderId,
                Guids = paymentCompletedMessage.ProductIds
            };

            var queueName = paymentCompletedMessage.IsCompleted ? "product-reserve-pending" : "payment-canceled";
            sendingMessage = paymentCompletedMessage.IsCompleted ? productStockReservePendingMessage : paymentCompletedMessage;

            SendMessageToQueue(sendingMessage, queueName);
        }

        private void SendMessageToQueue(IMessage message, string queueName)
        {
            // Send message
            MessageSender.SendMessage(queueName, message, _messageConnection);
        }
    }
}
