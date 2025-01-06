using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models
{
    public class AddBookingDto
    {
        [Required]
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        [Required]
        public DateTime CheckIn { get; set; }
        [Required]
        public DateTime CheckOut { get; set; }
        [Required]
        public int GuestId { get; set; }
        public int RoomId { get; set; }
    }
}