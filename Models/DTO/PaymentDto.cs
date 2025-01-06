namespace Hotel_Management.Models
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public Billing? Billing { get; set; }
    }
}