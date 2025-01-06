using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models
{
    public class AddBillingDto
    {
        public decimal Taxes { get; set; }
        [Required]
        public int BookingId { get; set; }
    }
}
