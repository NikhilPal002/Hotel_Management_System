using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models
{
    public class AddBookingDto
    {
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int GuestId { get; set; }
        public int RoomId { get; set; }
    }
}