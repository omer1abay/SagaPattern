using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaPattern.Orchestration.Shared.Messages
{
    public class PaymentCompletedMessage : IMessage
    {
        public List<Guid> ProductIds { get; set; }
        public Guid PaymentId { get; set; }
        public Guid OrderId { get; set; }
        public bool IsCompleted { get; set; }
    }
}
