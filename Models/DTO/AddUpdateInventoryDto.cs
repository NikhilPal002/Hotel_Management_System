using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models
{
    public class AddUpdateInventoryDto
    {
        [Required]
        public string InventoryName { get; set; }
        [Required]
        public int Quantity { get; set; }
        public string Category { get; set; }
    }
}