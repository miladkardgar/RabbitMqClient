using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqReceiverApp;

internal abstract class RabbitMqReceiverApp
{
    public static void Main()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "172.16.40.47",
            UserName = "m.kardgar",
            Password = "14381438Aa",
            Port = 5672
        };
        using var connection = factory.CreateConnection();
        var queueOtp = "SmsQueue";

        using (var channel = connection.CreateModel())
        {
            IDictionary<String, Object> args = new Dictionary<String, Object>();
            channel.BasicQos(0, 10, false);
            var consumer = new QueueingBasicConsumer(channel);
            consumer.m_consumerCancelled = null;
            consumer.ConsumerTag = null;
            IDictionary<string, object> consumerArgs = new Dictionary<string, object>();
            channel.BasicConsume(queueOtp, false, "", args, consumer);
            Console.WriteLine(" [*] Waiting for messages. " +
                              "To exit press CTRL+C");
            while (true)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(100));
                var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
                channel.BasicAck(ea.DeliveryTag, false);
            }
        }
    }
}