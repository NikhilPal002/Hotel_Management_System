using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models
{
    public class StaffDto
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
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