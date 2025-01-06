using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SagaPattern.Orchestration.Shared;
using SagaPattern.Orchestration.Shared.Messages;
using System.Text;

namespace SagaPattern.Orchestration.OrchestratorService.Consumers;

public class OrderConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private EventingBasicConsumer _consumer;
    private IConnection? _messageConnection;
    private IModel? _messageChannel;

    public OrderConsumer(IServiceProvider serviceProvider, IConnection? messageConnection)
    {
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string queueName = "order-received";

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
        OrderReceivedMessage orderReceivedMessage = JsonConvert.DeserializeObject<OrderReceivedMessage>(message)!;

        PaymentPendingMessage paymentPendingMessage = new()
        {
            CardHolderName = orderReceivedMessage.CardHolderName,
            CardNumber = orderReceivedMessage.CardNumber,
            CVV = orderReceivedMessage.CVV,
            OrderId = orderReceivedMessage.OrderId,
            ProductIds = orderReceivedMessage.Products,
            TotalAmount = orderReceivedMessage.TotalAmount
        };

        SendMessageToQueue(paymentPendingMessage);
    }

    private void SendMessageToQueue(IMessage message)
    {
        // Send message
        MessageSender.SendMessage("payment-pending", message, _messageConnection);


    }
}