using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly UserManager<Registration> _userManager;
        private readonly SignInManager<Registration> _signInManager;
        private readonly IConfiguration _config;


        public UsersController(AppDbContext context, UserManager<Registration> userManager, SignInManager<Registration> signInManager, IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Registration(RegistrationDTO registrationDTO)
        {
            /*if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
*/
            // Check if a user with the same email or username already exists
            var existingUser = await _userManager.FindByEmailAsync(registrationDTO.Email)
                              ?? await _userManager.FindByNameAsync(registrationDTO.userName);

            if (existingUser != null)
            {
                return Conflict("User with the same email or username already exists.");
            }


            if (existingUser != null)
            {
                return Conflict("User with the same email or username already exists.");
            }

            try
            {
                // Create a new user registration object
                var registration = new Registration()
                {
                    UserName = registrationDTO.userName,
                    Email = registrationDTO.Email,
                };

                var result = await _userManager.CreateAsync(registration, registrationDTO.Password);
                if (result.Succeeded)
                {
                    return Ok("User registered successfully.");
                }

                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(loginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _signInManager.PasswordSignInAsync(loginDto.userName, loginDto.password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(loginDto.userName);

                if (user == null)
                {
                    return Unauthorized("Invalid login attempt.");
                }

                var claims = new[]
                {

            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("Id", user.Id.ToString()),
            new Claim("Email", user.Email.ToString())
        };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                   expires: DateTime.UtcNow.AddMinutes(60),
                    signingCredentials: creds);

                string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new { token = tokenValue, user = new { user.UserName, user.Email } });
            }

            return Unauthorized("Invalid login attempt.");
        }
    }
}