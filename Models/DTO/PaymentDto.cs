using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        [Required]
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        [Required]
        public Billing? Billing { get; set; }
    }
}