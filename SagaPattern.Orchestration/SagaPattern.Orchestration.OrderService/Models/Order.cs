using SagaPattern.Orchestration.Shared;

namespace SagaPattern.Orchestration.OrderService.Models
{
    public class Order : Entity
    {

        public Order(List<Product> products, CardInformation cardInformation)
        {
            Products = products;
            CardInformation = cardInformation;
            TotalAmount = products.Sum(p => p.Price * p.Quantity);
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public List<Product> Products { get; set; }
        public CardInformation CardInformation { get; set; }
        public decimal TotalAmount { get; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
    }

    public enum OrderStatus
    {
        Pending,
        PaymentCompleted,
        PaymentFailed,
        ProductsReserved,
        ProductsReservationFailed,
        Completed
    }

}
