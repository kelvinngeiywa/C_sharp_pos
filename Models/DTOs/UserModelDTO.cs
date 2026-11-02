using System.ComponentModel.DataAnnotations;

namespace my_pos.Models.DTOs
{
    public class UserModelDTO
    {
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required!!!")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
    ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; }
        public DateOnly DOB { get; set; }
        public DateTime RegisteredDate { get; set; }
        public string Role { get; set; } = "User";
    }
}
