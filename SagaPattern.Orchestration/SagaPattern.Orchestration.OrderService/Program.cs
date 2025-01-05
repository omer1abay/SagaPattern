using SagaPattern.Orchestration.OrderService.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapPost("/order", async (CreateOrder createOrder) =>
{
    var order = await orderService.CreateOrder(createOrder);
    return Results.Created($"/order/{order.Id}", order);
});

app.Run();

public record CreateOrder(List<Product> items, CardInformation cardInfo);