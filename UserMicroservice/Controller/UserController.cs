using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyBusiness.UserMicroservice.DTO;
using MyBusiness.UserMicroservice.Models;
using MyBusiness.UserMicroservice.Services;

namespace MyBusiness.UserMicroservice.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(User user)
    {
        try
        {
            var registration = await _userService.RegistrationAsync(user);
            return Ok(registration);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(UserDTO userDTO)
    {
        try
        {
            var token = await _userService.LoginAsync(userDTO.Email, userDTO.Password);
            return Ok(new { Token = token });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

  }
}
