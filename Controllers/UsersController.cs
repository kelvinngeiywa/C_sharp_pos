using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using my_pos.Models;
using my_pos.Models.DTOs;
using my_pos.Services.IServices;

namespace my_pos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUser userServices) : ControllerBase
    {
        private readonly ResponseDTO response = new ResponseDTO();
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<ActionResult<ResponseDTO>> GetAllUsers()
        {
            var users = await userServices.GetAllUsersAsync();
            if (users is null || !users.Any())
            {
                response.Success = false;
                response.Message = "No user found!!!";
                response.Data = users;
                
                return NotFound(response);
            }
            response.Success = true;
            response.Data = users;
            return Ok(response);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDTO>> GetUserByIdAsync(Guid id)
        {
            var result = await userServices.GetUserByIdAsync(id);

            if (result is null)
            {
                response.Success = false;
                response.Message = "User not found";
                response.Data = result;
                return NotFound(response);
            }

            response.Success = true;
            response.Message = "User found";
            response.Data=result;

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDTO>> UpdateUserAsync(Guid id,UserModelDTO updateUser)
        {
            var result = await userServices.UpdateUserAsync(id, updateUser);

            if(result is null)
            {
                response.Success = false;
                response.Message = "Update failed!!";
                response.Data = result;

                return BadRequest(response);
            }

            response.Success=true;
            response.Message = "User updated successfully";
            response.Data = updateUser;

            return Ok(response);
        }
        [Authorize]
        [HttpPut("change-password/{id}")]
        public async Task<ActionResult<ResponseDTO>> UpdatePasswordAsync(Guid id,string password)
        {
            var results = await userServices.ChangePasswordAsync(id,password);
            if (results is null)
            {
                response.Success = false;
                response.Message = $"User with id {id} not found!!!";
                return BadRequest(response);
            }

            response.Success = true;
            response.Message = "Password changed successful!!";
            response.Data = results;

            return Ok(response);
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDTO>> DeleteUser(Guid id)
        {
            var user= await userServices.GetUserByIdAsync(id);
            
            if (user is null)
            {
                response.Success = false;
                response.Message = $"User with id {id} not found!!!";
                response.Data = user;
                return NotFound(response);
            }

            var result = await userServices.DeleteUserAsync(user);
            if (result is null)
            {
                response.Success = false;
                response.Message = $"Deleted failed!!";
                response.Data = user;
                return BadRequest(response);
            }

            response.Success = true;
            response.Message = "User deleted successfully!!";
            response.Data = user;

            return Ok(response);
        }

    }
}
