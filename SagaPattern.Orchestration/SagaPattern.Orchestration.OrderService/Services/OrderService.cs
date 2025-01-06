using Newtonsoft.Json;
using RabbitMQ.Client;
using SagaPattern.Orchestration.OrderService.Models;
using SagaPattern.Orchestration.Shared;
using SagaPattern.Orchestration.Shared.Messages;
using System.Text;

namespace SagaPattern.Orchestration.OrderService.Services
{
    public class OrderService(IConnection connection) : IOrderService
    {
        public Task CreateOrderAsync(Order order)
        {
            // Send order to the queue
            SendQueue(order);

            return Task.CompletedTask;
        }

        private void SendQueue(Order order)
        {
            OrderReceivedMessage message = new();
            message.OrderId = order.Id;
            message.CardNumber = order.CardInformation.CardNumber;
            message.CardHolderName = order.CardInformation.CardHolderName;
            message.CVV = order.CardInformation.CVV;
            message.Products = order.Products.Select(x => x.Id).ToList();
            message.TotalAmount = order.TotalAmount;

            MessageSender.SendMessage("order-received", message, connection);
        }

        public Task RevertOrderAsync(Guid orderId)
        {
            throw new NotImplementedException();
        }
    }
}
