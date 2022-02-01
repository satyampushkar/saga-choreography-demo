using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using common.models;

namespace customer.microservice
{
    public class EventsListener : BackgroundService
    {
        private readonly IModel _channel;
        private readonly ILogger<EventsListener> _logger;
        private readonly IService _service;

        public EventsListener(ILogger<EventsListener> logger, IService service)
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: "orderEventsQueue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            _logger = logger;
            _service = service;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Received message from Order service:" + message);

                var order = JsonConvert.DeserializeObject<OrderEvent>(message);
                if (order != null)
                {
                    _service.ReserveCredit(order);
                }
            };

            _channel.BasicConsume(queue: "orderEventsQueue", autoAck: true, consumer: consumer);

            
            return Task.CompletedTask;
        }
    }
}
