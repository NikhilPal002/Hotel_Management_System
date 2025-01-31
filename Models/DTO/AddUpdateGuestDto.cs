using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models
{
    public class AddUpdateGuestDto
    {
        [Required(ErrorMessage = "Guest name is required.")]
        public string GuestName { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public string State { get; set; }

        [Required(ErrorMessage = "Pin code is required.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Pin code must be 6 digits.")]
        public string PinCode { get; set; }

    }
}