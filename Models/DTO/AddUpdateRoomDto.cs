
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models
{
    public class AddUpdateRoomDto
    {
        public string RoomType { get; set; }
        public string Description { get; set; }
        [Required]
        public int NumberOfBeds { get; set; }
        [Required]
        public decimal PricePerNight { get; set; }
        public string Status { get; set; }
    }
}