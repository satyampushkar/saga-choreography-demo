
using Microsoft.EntityFrameworkCore;

namespace customer.microservice
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public double PaymentAmount { get; set; }
        public string PaymentStatus { get; set; }
    }

    class PaymentDTO
    {
        public Guid OrderId { get; set; }
        public double PaymentAmount { get; set; }
    }


    public class PaymentDb : DbContext
    {
        public PaymentDb(DbContextOptions<PaymentDb> options) : base(options)
        { }

        public DbSet<Payment> Payments => Set<Payment>();
    }
}