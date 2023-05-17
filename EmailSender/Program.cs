using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Text;
using RekrutacjaLib;
using System.Net.Mail;
using System.Net;

namespace EmailSender
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var connection = RabbitMQConnect();

            using var channel = connection.CreateModel();

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (ch, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Mail mail = JsonConvert.DeserializeObject<Mail>(message);
#if DEBUG
                Console.WriteLine($"Topic: {mail.Topic} Message: {mail.Message} To: {mail.To}");
#endif
                SendMail(mail.To, mail.Topic, TopSecret.FromMail, mail.Message);
                channel.BasicAck(ea.DeliveryTag, false);
            };

            channel.BasicConsume("mail", false, consumer);

            Console.ReadKey();
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

        private static void SendMail(string To, string Topic, string From, string msg)
        {
            string server = TopSecret.ServerSMTP;

            MailMessage message = new MailMessage();
            message.From = new MailAddress(From);

            SmtpClient client = new SmtpClient(server, 25);
            client.Credentials = new NetworkCredential(From, TopSecret.FromMailPassword);

            try
            {
                message.To.Add(To);
                message.Subject = Topic;
                message.Body = msg;

                client.Send(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}