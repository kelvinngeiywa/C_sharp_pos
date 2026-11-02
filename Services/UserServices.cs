using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using my_pos.Data;
using my_pos.Models;
using my_pos.Models.DTOs;
using my_pos.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace my_pos.Services
{
    public class UserServices(AppDbContext dbContext,IConfiguration configuration) : IUser
    {
        public async Task<List<UserModel>> GetAllUsersAsync()
        {
            return await dbContext.users.ToListAsync();
        }

        public async Task<UserModel> RegisterUserAsync(UserModelDTO registerUser)
        {
            if (string.IsNullOrWhiteSpace(registerUser.Email))
                return null;

            var email = registerUser.Email.Trim().ToLowerInvariant();

            if (await dbContext.users.AnyAsync(user => user.Email == email))
            {
                return null;
            }

            var newuser = new UserModel();
            //var checkPassword =;
            var hashedPassword = new PasswordHasher<UserModel>().HashPassword(newuser, registerUser.Password);

            newuser.Name = registerUser.Name;
            newuser.Email = email;
            newuser.Password = hashedPassword;
            newuser.RegisteredDate = DateTime.UtcNow;
            newuser.DOB = registerUser.DOB;
            newuser.Role = registerUser.Role;

            dbContext.users.Add(newuser);
            await dbContext.SaveChangesAsync();
            return newuser;
        }

        public async Task<LoginResponseDTO> LoginUserAsync(LoginUserDTO loginUser)
        {
            var user = await dbContext.users.FirstOrDefaultAsync(user => user.Email.ToLower() == loginUser.Email.ToLower());
            if (user is null)
            {
                return null;
            }

            var confirmPassword = new PasswordHasher<UserModel>().VerifyHashedPassword(user, user.Password, loginUser.Password);
            if(confirmPassword == PasswordVerificationResult.Failed)
            {
                return null;
            }
            Console.WriteLine(user);
            var response = new LoginResponseDTO { 
                AccessToken=CreateToken(user),
                RefreshToken= await GenerateAndSaveRefreshToken(user)
            };

            return response;
        }

        private string CreateToken(UserModel user)
        {
            var claims = new List<Claim> { 
                new Claim(ClaimTypes.Name,user.Name),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Role,user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("Jwt:Issuer"),
                audience: configuration.GetValue<string>("Jwt:Audience"),
                claims:claims,
                expires:DateTime.UtcNow.AddMinutes(30),
                signingCredentials:creds
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateAndSaveRefreshToken(UserModel userModel)
        {
            var refreshToken = GenerateRefreshToken();
            userModel.RefreshToken = refreshToken;
            userModel.RefreshTokenLifeTime = DateTime.UtcNow.AddDays(1);

            await dbContext.SaveChangesAsync();
            return refreshToken;

        }

        public async Task<LoginResponseDTO> RefreshTokensAsync(RefreshTokenRequestDTO tokenRequestDTO)
        {
            var user = await ValidateRefreshToken(tokenRequestDTO.Id, tokenRequestDTO.RefreshToken);
            if (user is null)
            {
                return null;
            }

            return new LoginResponseDTO { 
                AccessToken=CreateToken(user),
                RefreshToken=await GenerateAndSaveRefreshToken(user)
            };
        }

        private async Task<UserModel?> ValidateRefreshToken(Guid Id, string refreshToken)
        {
            var user = await dbContext.users.FindAsync(Id);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenLifeTime <= DateTime.UtcNow)
            {
                return null;
            }

            return user;
        }

        public async Task<UserModel> GetUserByIdAsync(Guid id)
        {
            var user = await dbContext.users.FindAsync(id);
            if (user is null)
            {
                return null;
            }

            return user;
        }

        public async Task<UserModel> UpdateUserAsync(Guid userId,UserModelDTO updateUser)
        {
            var user= await dbContext.users.FindAsync(userId);
            if (user is null)
            {
                return null;
            }

            var email = updateUser.Email.Trim().ToLowerInvariant();

            if (await dbContext.users.AnyAsync(u => u.Email == email && u.Id != userId))
            {
                return null;
            }
            //var updatedPassword = new PasswordHasher<UserModel>().HashPassword(user, updateUser.Password);
            user.Name = updateUser.Name;
            user.Email = email;
            user.Role = updateUser.Role;

            if (!string.IsNullOrWhiteSpace(updateUser.Password))
            {
                user.Password= new PasswordHasher<UserModel>().HashPassword(user, updateUser.Password);
            }
            

            await dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<string> DeleteUserAsync(UserModel userModel)
        {
            dbContext.users.Remove(userModel);
            await dbContext.SaveChangesAsync();
            return "";
        }

        public async Task<string> ChangePasswordAsync(Guid userId, string newPassword)
        {
            var user = await dbContext.users.FindAsync(userId);
            if (user is null)
            {
                return null;
            }

            var updatedPassword= new PasswordHasher<UserModel>().HashPassword(user, newPassword);
            user.Password = updatedPassword;

            await dbContext.SaveChangesAsync();
            return "";

        }
    }
}
