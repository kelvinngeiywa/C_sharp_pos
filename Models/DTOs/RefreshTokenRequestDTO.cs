namespace my_pos.Models.DTOs
{
    public class RefreshTokenRequestDTO
    {
        public Guid Id { get; set; }
        public required string RefreshToken { get; set; }
    }
}
