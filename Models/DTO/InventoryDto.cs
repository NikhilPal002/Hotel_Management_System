namespace Hotel_Management.Models
{
    public class InventoryDto
    {
        public int InventoryId { get; set; }
        public string InventoryName { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}