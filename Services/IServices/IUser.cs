using my_pos.Models;
using my_pos.Models.DTOs;

namespace my_pos.Services.IServices
{
    public interface IUser
    {
        Task<UserModel> RegisterUserAsync(UserModelDTO registerUser);
        Task<LoginResponseDTO> LoginUserAsync(LoginUserDTO loginUser);
        Task<List<UserModel>> GetAllUsersAsync();
        Task<UserModel> GetUserByIdAsync(Guid id);

        Task<UserModel> UpdateUserAsync(Guid userId,UserModelDTO updateUser);
        Task<LoginResponseDTO> RefreshTokensAsync(RefreshTokenRequestDTO tokenRequestDTO);
        Task<string> DeleteUserAsync(UserModel userModel);

        Task<string> ChangePasswordAsync(Guid userId, ChangePasswordDTO newPassword);
        Task<string> AssignRoleAsync(Guid id, ChangeRoleDTO newRole);
    }
}
