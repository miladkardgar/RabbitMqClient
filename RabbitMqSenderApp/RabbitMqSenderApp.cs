using System.Text;
using RabbitMQ.Client;

namespace RabbitMqSenderApp;

internal abstract class RabbitMqSenderApp
{
    public static void Main()
    {
        ConnectionFactory factory = new()
        {
            Uri = new Uri("amqp://m.kardgar:14381438Aa@172.16.40.47:5672"),
            ClientProvidedName = "Rabbit Sender App"
        };
        IConnection cnn = factory.CreateConnection();
        IModel channel = cnn.CreateModel();
        var exchangeName = "SMS";
        var routingKey = "sms-routing-key";
        var queueOtp = "SmsQueue";
        IDictionary <String , Object> args = new Dictionary<String,Object>() ;
        args.Add("x-max-priority", 24);
        channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);
        channel.QueueDeclare(queueOtp, true, false, false, args);
        channel.QueueBind(queueOtp, exchangeName, routingKey, null);

        var random = new Random();
        for (var i = 0; i < 300; i++)
        {
            var pr = random.Next(1, 6);
            var message = "";
            switch (pr)
            {
                case 5:
                    message = "Otp: " + random.Next(1020, 9090);
                    break;
                case 4:
                    message = "Transaction Amount: " + random.Next(102044, 909099);
                    break;
                case 3:
                    message = "Notice";
                    break;
                case 2:
                    message = "Reverse";
                    break;
                case 1:
                    message = "Public message";
                    break;
            }
            var data = new
            {
                To="09124211477,09356070351",
                Message=message,
                Priority=pr,
                DateTime = DateTime.Now
            };
            var messageBodyBytes = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(data));
            var properties = channel.CreateBasicProperties();
            properties.Priority = Convert.ToByte(pr);
            channel.BasicPublish(exchangeName, routingKey, properties, messageBodyBytes);
            Thread.Sleep(1000);
        }
    }
}