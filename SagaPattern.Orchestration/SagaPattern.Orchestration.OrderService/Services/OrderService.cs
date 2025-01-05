using SagaPattern.Orchestration.OrderService.Models;

namespace SagaPattern.Orchestration.OrderService.Services
{
    public class OrderService : IOrderService
    {
        public Task CreateOrderAsync(Order order)
        {
            throw new NotImplementedException();
        }

        public Task RevertOrderAsync(Guid orderId)
        {
            throw new NotImplementedException();
        }
    }
}
