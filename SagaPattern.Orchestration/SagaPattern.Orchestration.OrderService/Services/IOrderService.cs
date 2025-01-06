using RabbitMQ.Client;
using SagaPattern.Orchestration.OrderService.Models;

namespace SagaPattern.Orchestration.OrderService.Services
{
    public interface IOrderService
    {
        public Task CreateOrderAsync(Order order); 
        public Task RevertOrderAsync(Guid orderId); 
    }
}
