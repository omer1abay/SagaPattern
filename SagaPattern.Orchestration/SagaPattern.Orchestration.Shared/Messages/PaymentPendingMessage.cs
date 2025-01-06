using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaPattern.Orchestration.Shared.Messages
{
    public class PaymentPendingMessage : IMessage
    {
        public List<Guid> ProductIds { get; set; }
        public Guid OrderId { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public string CVV { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
