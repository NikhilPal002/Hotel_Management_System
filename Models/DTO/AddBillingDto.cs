using System;
using System.Collections.Generic;

namespace Hotel_Management.Models
{
    public class AddBillingDto
    {
         public decimal Taxes { get; set; }
        public int BookingId{get;set;}
    }
}
