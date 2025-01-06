using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models
{
    public class AddPaymentDto
    {
        [Required]
        public string PaymentMethod { get; set; }
        [Required]
        public int BillingId{get;set;}
    }
}