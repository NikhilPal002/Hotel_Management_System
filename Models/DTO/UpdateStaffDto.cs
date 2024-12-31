namespace Hotel_Management.Models
{
    public class UpdateStaffDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string SAddress { get; set; }
        public decimal Salary { get; set; }
        public string Designation { get; set; }
        public int UserId { get; set; }
    }

}