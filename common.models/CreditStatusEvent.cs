namespace common.models
{
    public class CreditStatusEvent
    {
        public Guid OrderId { get; set; }
        public string Status { get; set; }
    }
}
