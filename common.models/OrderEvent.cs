namespace common.models
{
    public class OrderEvent
    {
        public Guid OrderId { get; set; }
        public int CustomerId { get; set; }
        public int OrderAmount { get; set; }
    }
}
