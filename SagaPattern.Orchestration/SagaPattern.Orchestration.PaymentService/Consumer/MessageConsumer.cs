
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SagaPattern.Orchestration.Shared;
using SagaPattern.Orchestration.Shared.Messages;
using System.Text;

namespace SagaPattern.Orchestration.PaymentService.Consumer;

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
        // Mevcut payment-pending queue tanımı
        string queueName = "payment-pending";

        _messageConnection = _serviceProvider.GetRequiredService<IConnection>();
        _messageChannel = _messageConnection.CreateModel();
        _messageChannel.QueueDeclare(queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        // Tüketici için event handler
        _consumer = new EventingBasicConsumer(_messageChannel);
        _consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var messageType = ea.BasicProperties.Type; // Mesaj tipini kontrol etmek için

            switch (messageType)
            {
                case "PaymentPending":
                    // Mevcut payment-pending işlemleri
                    await ProcessMessageAsync(message);
                    break;

                case "ProductReserveFailed":
                    // Yeni eklenen product-reserve-failed işlemleri
                    await HandleProductReserveFailedAsync(message);
                    break;

                default:
                    // Bilinmeyen mesaj tipi için log veya hata işleme
                    //_logger.LogWarning($"Bilinmeyen mesaj tipi: {messageType}");
                    break;
            }

            _messageChannel.BasicAck(ea.DeliveryTag, false);
        };

        _messageChannel.BasicConsume(queue: queueName,
            autoAck: false,
            consumer: _consumer);

        return Task.CompletedTask;
    }

    private Task ProcessMessageAsync(string message)
    {
        PaymentPendingMessage paymentPendingMessage = JsonConvert.DeserializeObject<PaymentPendingMessage>(message)!;

        //do payment
        //end of payment process

        PaymentCompletedMessage paymentCompletedMessage = new()
        {
            IsCompleted = true,
            OrderId = paymentPendingMessage.OrderId,
            PaymentId = Guid.NewGuid(),
            ProductIds = paymentPendingMessage.ProductIds
        };

        SendMessageToQueue(paymentCompletedMessage);

        return Task.CompletedTask;
    }

    private async Task HandleProductReserveFailedAsync(string message)
    {
        // Ürün rezervasyonu başarısız olduğunda yapılacak işlemler
        // Örneğin:
        // 1. Ödeme işlemini iptal et
        // 2. Log kaydet
        // 3. Gerekirse telafi işlemleri yap
        try
        {
            var productReserveFailedMessage = JsonConvert.DeserializeObject<ProductStockReservedMessage>(message);

            PaymentCompletedMessage paymentCompletedMessage = new()
            {
                IsCompleted = false,
                OrderId = productReserveFailedMessage.OrderId,
                PaymentId = productReserveFailedMessage.PaymentId
            };

            SendMessageToQueue(paymentCompletedMessage);

            // Ödeme sürecini sonlandır
            //await _paymentService.CancelPaymentAsync(productReserveFailedMessage.OrderId);

            //_logger.LogWarning($"Ürün rezervasyonu başarısız: OrderId {productReserveFailedMessage.OrderId}");
        }
        catch (Exception ex)
        {
            //_logger.LogError($"Ürün rezervasyonu hatası işlenirken hata: {ex.Message}");
        }
    }

    private void SendMessageToQueue(IMessage message)
    {
        // Send message
        MessageSender.SendMessage("payment-completed", message, _messageConnection);
    }
}
