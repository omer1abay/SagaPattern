
namespace SagaPattern.Orchestration.OrderService.Consumer
{
    public class MessageConsumer : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
