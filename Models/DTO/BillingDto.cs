using System;
using System.Collections.Generic;

namespace Hotel_Management.Models
{
    public class BillingDto
    {
        public int Id { get; set; } // Primary Key
        public string BillingNo { get; set; }
        public decimal Price { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalCost { get; set; } // Calculated Total Cost
        public List<BillingService>? BillingServices { get; set; } // Navigation Property for Services
        public Booking? Booking{get;set;}
    }
}
