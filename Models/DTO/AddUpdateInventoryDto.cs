using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models
{
    public class AddUpdateInventoryDto
    {
        [Required(ErrorMessage = "Inventory name is required.")]
        public string InventoryName { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; }
    }
}