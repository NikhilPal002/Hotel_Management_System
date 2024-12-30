using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int NumberOfNights { get; set; }
        public decimal TotalCost { get; set; }
        public string BookingStatus { get; set; }
        public int GuestId { get; set; }
        public int RoomId { get; set; }
        public Guest? Guest { get; set; }
        public Room? Room { get; set; }
    }
}