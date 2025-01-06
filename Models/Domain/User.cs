using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Role? Role { get; set; }
    }
}