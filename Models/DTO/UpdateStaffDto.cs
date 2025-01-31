using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models
{
    public class UpdateStaffDto
    {
        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; }

        [Range(18, int.MaxValue, ErrorMessage = "Age must be at least 18 years.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string SAddress { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Salary must be greater than 0.")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Designation is required.")]
        public string Designation { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
    }

}