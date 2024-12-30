namespace Hotel_Management.Models
{
    public class Staff
    {
        public int StaffId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string SAddress { get; set; }
        public decimal Salary { get; set; }
        public string Designation { get; set; }
        public DateTime JoinDate { get; set; }
        public string NIC { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int RoleId { get; set; }
        public Role? Role { get; set; }
    }

}