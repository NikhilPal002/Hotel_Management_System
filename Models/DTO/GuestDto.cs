using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models
{
    public class GuestDto
    {
        [Key]
        public int GuestId { get; set; }
        public string GuestName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string State { get; set; }
        public string PinCode { get; set; }
    }
}