using common.models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace customer.microservice
{
    public interface IPublisher
    {
        void Publish(CreditStatusEvent creditStatus);
    }

    public class Publisher : IPublisher
    {
        private readonly ILogger<Publisher> _logger;
        private readonly IModel _channel;
        public Publisher(ILogger<Publisher> logger)
        {
            _logger = logger;
            var factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: "customerEventsQueue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }
        public void Publish(CreditStatusEvent creditStatus)
        {
            string message = JsonConvert.SerializeObject(creditStatus);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                                 routingKey: "customerEventsQueue",
                                 basicProperties: null,
                                 body: body);
            _logger.LogInformation("Event pushed to customerEventsQueue........");
        }
    }
}
