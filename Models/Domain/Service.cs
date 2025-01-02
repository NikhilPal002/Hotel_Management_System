namespace Hotel_Management.Models
{
    public class Service
    {
        public int Id { get; set; } // Primary Key
        public string ServiceName { get; set; } // Service name (e.g., "Room Service")
        public decimal ServiceCost { get; set; } // Cost of the service
    }
}
