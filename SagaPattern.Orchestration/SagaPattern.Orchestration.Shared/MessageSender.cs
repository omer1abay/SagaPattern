using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Data.Common;
using System.Text;

namespace SagaPattern.Orchestration.Shared
{
    public static class MessageSender
    {

        public static void SendMessage(string queueName, IMessage message, IConnection connection)
        {
            // Send message
            using (IModel channel = connection.CreateModel())
            {

                channel.QueueDeclare(queue: queueName,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

                byte[] body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                channel.BasicPublish(exchange: string.Empty,
                     routingKey: queueName,
                     mandatory: false,
                     basicProperties: null,
                     body: body);
            }
        }

        public static void SendMessage(string queueName, string messageType, IMessage message, IConnection connection)
        {
            // Send message
            using (IModel channel = connection.CreateModel())
            {

                var properties = channel.CreateBasicProperties();
                properties.Type = messageType;

                channel.QueueDeclare(queue: queueName,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

                byte[] body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                channel.BasicPublish(exchange: string.Empty,
                     routingKey: queueName,
                     mandatory: false,
                     basicProperties: properties,
                     body: body);
            }
        }

    }
}
