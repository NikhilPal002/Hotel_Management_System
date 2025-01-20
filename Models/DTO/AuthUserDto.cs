using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models
{
    public class RegisterRequestDto
    {
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }

        public int RoleId { get; set; }
    }

    public class LoginDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }
    }

    public class LoginResponseDto{
       public string Email { get; set; }
        public string Token { get; set; }

        public List<string> Roles {get;set;}
    }
}