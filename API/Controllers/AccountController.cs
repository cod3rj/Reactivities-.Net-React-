using System.Security.Claims;
using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly TokenService _tokenService;
        public AccountController(UserManager<AppUser> userManager, TokenService tokenService)
        {
            _tokenService = tokenService;
            _userManager = userManager;
        }

        [AllowAnonymous] // We allow anonymous access to this controller
        // Login endpoint
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // We get the user from the database
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            // We check if the user exists
            if (user == null) return Unauthorized();

            // We check if the password is correct
            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            // If the password is correct
            if (result)
            {
                return CreateUserObject(user);
            }

            // If the password is incorrect
            return Unauthorized();
        }

        [AllowAnonymous] // We allow anonymous access to this controller
        // Register endpoint
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            // We check if the UserName is already taken.
            if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.UserName))
            {
                return BadRequest("Username is already taken");
            }

            // Create a new user
            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.UserName
            };

            // We create the user
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            // We check if the user was created
            if (result.Succeeded)
            {
                return CreateUserObject(user);
            }

            return BadRequest(result.Errors);
        }



        // Get User endpoint
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email)); // We get the user from the database

            return CreateUserObject(user); // We return a UserDto with the user information
        }

        // Create a User Object Method
        private UserDto CreateUserObject(AppUser user)
        {
            // We return a UserDto with the user information
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Image = null,
                Token = _tokenService.CreateToken(user),
                UserName = user.UserName
            };
        }
    }
}