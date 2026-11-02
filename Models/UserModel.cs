using System.ComponentModel.DataAnnotations;

namespace my_pos.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; }
        public DateOnly DOB { get; set; }
        public DateTime RegisteredDate { get; set; }
        public string Role { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenLifeTime { get; set; }

    }
}
