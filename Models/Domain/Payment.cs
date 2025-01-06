namespace Hotel_Management.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public int BillingId { get; set; }
        public Billing? Billing { get; set; }

    }
}