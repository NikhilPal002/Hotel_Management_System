namespace Hotel_Management.Models
{
    public class AddPaymentDto
    {
        public decimal PaymentAmount { get; set; }
        public string PaymentMethod { get; set; }
        public int BookingId { get; set; }
    }
}