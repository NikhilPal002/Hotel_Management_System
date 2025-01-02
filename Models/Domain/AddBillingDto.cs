using System;
using System.Collections.Generic;

namespace Hotel_Management.Models
{
    public class AddBillingDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public decimal Taxes { get; set; }
        public int PaymentId { get; set; }

    }
}
