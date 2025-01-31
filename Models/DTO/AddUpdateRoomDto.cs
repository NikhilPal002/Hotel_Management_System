
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models
{
    public class AddUpdateRoomDto
    {
        [Required(ErrorMessage = "Room type is required.")]
        public string RoomType { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Number of beds is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "The number of beds should be at least 1.")]
        public int NumberOfBeds { get; set; }

        [Required(ErrorMessage = "Price per night is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "The price should be greater than 0.")]
        public decimal PricePerNight { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; }
    }
}