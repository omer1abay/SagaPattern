using SagaPattern.Orchestration.OrchestratorService.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddRabbitMQClient("messaging");

builder.Services.AddHostedService<OrderConsumer>();
builder.Services.AddHostedService<PaymentConsumer>();
builder.Services.AddHostedService<ProductConsumer>();

// Add services to the container.

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.Run();
