using common.models;

namespace customer.microservice
{
    public interface IService
    {
        void ReserveCredit(OrderEvent order);
    }

    public class Service : IService
    {
        private readonly IPublisher _publisher;
        private readonly ILogger<Service> _logger;
        public Service(IPublisher publisher, ILogger<Service> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        public void ReserveCredit(OrderEvent order)
        {
            Random random = new();
            if (random.Next(1, 10) % 2 == 0)
            {
                //payment reserved
                _publisher.Publish(new CreditStatusEvent { OrderId = order.OrderId, Status = "Reserved" });
                _logger.LogInformation($"Payment reserved for customer {order.CustomerId} with orderid: {order.OrderId} & amount {order.OrderAmount}");
            }
            else
            {
                //credit limit exceeded 
                _publisher.Publish(new CreditStatusEvent { OrderId = order.OrderId, Status = "LimitExceeded" });
                _logger.LogInformation($"Credit limit exceeded for customer {order.CustomerId} with orderid: {order.OrderId}");
            }

        }
    }
}
