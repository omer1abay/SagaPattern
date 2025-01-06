var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("messaging");

builder
    .AddProject<Projects.SagaPattern_Orchestration_OrchestratorService>("sagapattern-orchestration-orchestratorservice")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder
    .AddProject<Projects.SagaPattern_Orchestration_OrderService>("sagapattern-orchestration-orderservice")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder
    .AddProject<Projects.SagaPattern_Orchestration_PaymentService>("sagapattern-orchestration-paymentservice")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder
    .AddProject<Projects.SagaPattern_Orchestration_ProductService>("sagapattern-orchestration-productservice")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.Build().Run();
