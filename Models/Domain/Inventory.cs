using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }
        [Required]
        public string InventoryName { get; set; }
        [Required]
        public int Quantity { get; set; }
        public string Category { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}