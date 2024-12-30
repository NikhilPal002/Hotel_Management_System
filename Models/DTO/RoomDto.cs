using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models
{
    public class RoomDto
    {
        [Key]
        public int RoomId { get; set; }
        public string RoomType { get; set; }
        public string Description { get; set; }
        public int NumberOfBeds { get; set; }
        public decimal PricePerNight { get; set; }
        public string Status { get; set; }
    }
}