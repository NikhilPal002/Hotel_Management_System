using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models
{
    public class Staff
    {
        [Key]
        public int StaffId { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public int Age { get; set; }
        public string SAddress { get; set; }
        public decimal Salary { get; set; }
        public string Designation { get; set; }
        public DateTime JoinDate { get; set; }
        public string NIC { get; set; }
    }

}