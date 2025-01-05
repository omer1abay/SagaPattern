var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.SagaPattern_Orchestration_OrchestratorService>("sagapattern-orchestration-orchestratorservice");

builder.AddProject<Projects.SagaPattern_Orchestration_OrderService>("sagapattern-orchestration-orderservice");

builder.AddProject<Projects.SagaPattern_Orchestration_PaymentService>("sagapattern-orchestration-paymentservice");

builder.AddProject<Projects.SagaPattern_Orchestration_ProductService>("sagapattern-orchestration-productservice");

builder.Build().Run();
