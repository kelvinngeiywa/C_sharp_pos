using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using my_pos.Models;
using my_pos.Models.DTOs;
using my_pos.Services.IServices;

namespace my_pos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IUser userServices) : ControllerBase
    {
        private readonly ResponseDTO response = new ResponseDTO();

        [HttpPost("register")]
        public async Task<ActionResult<ResponseDTO>> RegisterUserAsync(UserModelDTO newUser)
        {
            var registeredUser = await userServices.RegisterUserAsync(newUser);
            if(registeredUser is null)
            {
                response.Success = false;
                response.Message = $"Email {newUser.Email} exists";
                return BadRequest(response);
            }
            response.Success = true;
            response.Message = "User registered successfully!!";
            response.Data = registeredUser;
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ResponseDTO>> LoginuserAsync(LoginUserDTO loginUserDTO)
        {
            var loggeduser = await userServices.LoginUserAsync(loginUserDTO);
            Console.WriteLine(loggeduser);
            if (loggeduser is null)
            {
                response.Success = false;
                response.Message = "Username or password is incorrect";
                response.Data = loggeduser;
                return BadRequest(response);
            }

            response.Success = true;
            response.Data = loggeduser;
            response.Message = "Login successfully!!";
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ResponseDTO>> RefreshTokenAsync(RefreshTokenRequestDTO tokenRequestDTO)
        {
            var result = await userServices.RefreshTokensAsync(tokenRequestDTO);

            if(result is null || result.AccessToken is null || result.RefreshToken is null)
            {
                response.Success = false;
                response.Message = "Invalid refresh token";
                response.Data = result;
                return BadRequest(response);
            }

            response.Success = true;
            response.Message = "Token refresh successful!!";
            response.Data = result;

            return Ok(response);
        }
    }
}
