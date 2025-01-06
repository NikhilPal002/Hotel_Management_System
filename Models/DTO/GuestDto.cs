using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models
{
    public class GuestDto
    {
        [Key]
        public int GuestId { get; set; }
        [Required]
        public string GuestName { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string State { get; set; }
        public string PinCode { get; set; }
    }
}