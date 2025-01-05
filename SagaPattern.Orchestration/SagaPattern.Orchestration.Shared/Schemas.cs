using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaPattern.Orchestration.Shared
{
    //In this class, we define the schemas that we will use in our database in order to logical separation of our microservices.
    public class Schemas
    {
        public string Default = "public";
        public string Order = "order";
        public string Payment = "payment";
        public string Product = "product";
    }
}
