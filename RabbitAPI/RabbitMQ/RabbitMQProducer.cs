using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RekrutacjaLib;

namespace RabbitAPI.RabbitMQ
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        public void SendProductMessage<T>(T message)
        {
            var connection = RabbitMQConnect();

            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare("mail", exclusive: false, autoDelete: false);

                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);

                channel.BasicPublish(exchange: "", routingKey: "mail", body: body);
            }
        }

        private static IConnection RabbitMQConnect()
        {
            var factory = new ConnectionFactory
            {
                HostName = TopSecret.RabbitMQHostname,
                UserName = TopSecret.RabbitMQUsername,
                Password = TopSecret.RabbitMQPassword
            };

            var connection = factory.CreateConnection();

            return connection;
        }
    }
}
