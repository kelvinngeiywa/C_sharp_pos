namespace my_pos.Models.DTOs
{
    public record RegisterResultDTO
    {
        public UserModel? userObject { get; set; }
        public string? Error { get; set; }
    }
}
