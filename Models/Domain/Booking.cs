using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        [Required]
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        [Required]
        public DateTime CheckIn { get; set; }
        [Required]
        public DateTime CheckOut { get; set; }
        public int NumberOfNights { get; set; }
        public decimal TotalCost { get; set; }
        public string BookingStatus { get; set; }
        public string PaymentStatus {get;set;}
        [Required]
        public int GuestId { get; set; }
        [Required]
        public int RoomId { get; set; }
        public Guest? Guest { get; set; }
        public Room? Room { get; set; }
    }
}