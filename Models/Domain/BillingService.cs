namespace Hotel_Management.Models
{
    public class BillingService
    {
        public int BillingId { get; set; } // Foreign Key to Billing
        public Billing? Billing { get; set; } // Navigation Property

        public int ServiceId { get; set; } // Foreign Key to Service
        public Service? Service { get; set; } // Navigation Property
    }
}
