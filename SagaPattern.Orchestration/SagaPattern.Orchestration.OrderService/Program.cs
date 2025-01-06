using SagaPattern.Orchestration.OrderService.Consumer;
using SagaPattern.Orchestration.OrderService.Models;
using SagaPattern.Orchestration.OrderService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddRabbitMQClient("messaging");

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddHostedService<MessageConsumer>();

// Add services to the container.

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapPost("/order", async (CreateOrder createOrder, IOrderService orderService) =>
{
    var addOrder = new Order(createOrder.items, createOrder.cardInfo);

    await orderService.CreateOrderAsync(addOrder);
    return Results.Created($"/order/{addOrder.Id}", addOrder);
});

app.Run();

public record CreateOrder(List<Product> items, CardInformation cardInfo);