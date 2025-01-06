using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaPattern.Orchestration.Shared.Messages
{
    public class ProductStockReservedMessage : IMessage
    {
        public Guid OrderId { get; set; }
        public Guid PaymentId { get; set; }
        public bool IsCompleted { get; set; }
    }
}
