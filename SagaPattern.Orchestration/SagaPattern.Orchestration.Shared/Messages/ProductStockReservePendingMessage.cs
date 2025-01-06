using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaPattern.Orchestration.Shared.Messages
{
    public class ProductStockReservePendingMessage : IMessage
    {
        public Guid OrderId { get; set; }
        public Guid PaymentId { get; set; }
        public List<Guid> Guids { get; set; }
    }
}
