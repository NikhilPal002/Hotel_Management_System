namespace Hotel_Management.Models
{
    public class Billing
    {
        public int Id { get; set; } // Primary Key
        public string BillingNo { get; set; }
        // public DateTime StartDate { get; set; }
        // public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalCost { get; set; } // Calculated Total Cost
        public List<BillingService>? BillingServices { get; set; } // Navigation Property for Services
        public int BookingId{get;set;}
        public Booking? Booking{get;set;}

    }
}
