using common.models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace order.microservice
{
    public class EventsListener : BackgroundService
    {
        private readonly IModel _channel;
        private readonly ILogger<EventsListener> _logger;
        private readonly OrderDb _orderDb;

        public EventsListener(ILogger<EventsListener> logger, OrderDb orderDb)
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: "customerEventsQueue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            _logger = logger;
            _orderDb = orderDb;
        }

        protected override async Task<Task> ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Received message:" + message);

                var creditStatus = JsonConvert.DeserializeObject<CreditStatusEvent>(message);
                if (creditStatus != null)
                {
                    var order = await _orderDb.Orders.FindAsync(creditStatus.OrderId);

                    if (order != null)
                    {
                        if (creditStatus.Status == "Reserved")
                        {
                            order.OrderStatus = OrderStatus.Approved;
                        }
                        else
                        {
                            order.OrderStatus = OrderStatus.Rejected;
                        }
                    }
                    await _orderDb.SaveChangesAsync(); 
                }
            };

            _channel.BasicConsume(queue: "customerEventsQueue", autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }
    }
}
