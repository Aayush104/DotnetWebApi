using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Diagnostics.Eventing.Reader;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using static System.Net.WebRequestMethods;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly UserManager<Registration> _userManager;
        private readonly SignInManager<Registration> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;
        private readonly ILogger<BlogController> _logger;

        public UsersController(AppDbContext context, UserManager<Registration> userManager, SignInManager<Registration> signInManager, IEmailSender emailSender, IConfiguration config, ILogger<BlogController> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _config = config;
            _logger = logger;
        }


        private static string GenerateOtp()
        {
            Random rnd = new Random();
            int otp = rnd.Next(1000, 10000); // Generates a number between 1000 and 9999
            return otp.ToString("D4"); // Converts the number to a string with 4 digits
        }


        [HttpPost("register")]
        public async Task<IActionResult> Registration(RegistrationDTO registrationDTO)
        {
            // Check if a user with the same email or username already exists
            var existingUserEmail = await _userManager.FindByEmailAsync(registrationDTO.Email);
            var existingUserUsername = await _userManager.FindByNameAsync(registrationDTO.userName);

            // Generate OTP
            string otp = GenerateOtp();


            if (existingUserUsername != null && existingUserUsername.validity == "True")
            {
                return Conflict("User with that name exists");
            }
            else if (existingUserEmail != null && existingUserEmail.validity == "False")
            {
                // Update existing user's username and email and set the new OTP
                existingUserEmail.UserName = registrationDTO.userName;
                existingUserEmail.Email = registrationDTO.Email;
                existingUserEmail.Otp = otp;

                var updateResult = await _userManager.UpdateAsync(existingUserEmail);


                if (!updateResult.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update existing user.");
                }



            }
            else if (existingUserEmail != null && existingUserEmail.validity == "True")
            {
                return Conflict("User with that email exists");
            }

            // Send OTP via email
            MailRequest mailRequest = new MailRequest
            {
                ToEmail = registrationDTO.Email,
                Subject = "User Verification Code",
                Body = $"Your OTP Code is {otp}"
            };

            await _emailSender.SendEmailAsync(mailRequest);




            if (existingUserEmail == null)
            {
                var newRegistration = new Registration()
                {
                    UserName = registrationDTO.userName,
                    Email = registrationDTO.Email,
                    Otp = otp,
                    validity = "False"
                };

                var createResult = await _userManager.CreateAsync(newRegistration, registrationDTO.Password);
                if (!createResult.Succeeded)
                {
                    return BadRequest(createResult.Errors);
                }



            }


            return Ok("User registered successfully.");
        }


        //verifying otp
        [HttpPost("Verify/{email}")]
        public async Task<IActionResult> Verify(string email, VerifyDto verify)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userData = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (userData == null)
            {
                return Unauthorized("User not found");
            }

            _logger.LogInformation("Database OTP: {Otp}", userData.Otp);
            _logger.LogInformation("Form OTP: {Otp}", verify.Otp);

            if (userData.Otp == verify.Otp)
            {
                userData.validity = "True";  // Assuming Validity is a string
                _context.Users.Update(userData);
                await _context.SaveChangesAsync();
                return Ok(userData);
            }

            return BadRequest("OTP is incorrect");
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


                ///user bata role leko
                var roles = await _userManager.GetRolesAsync(user);


                var claims = new[]
                {

            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("Id", user.Id.ToString()),
            new Claim("Email", user.Email.ToString()),
            new Claim("validity", user.validity.ToString()),
            new Claim("Role", string.Join(",", roles)), //claim ma role ni ad garya

        };
                //for to generate a token

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

        [HttpPost("signin-google")]
        public async Task<IActionResult> GoogleLogin(GoogleLoginDto googleLoginDto)
        {

            var existingEmail = await _userManager.FindByEmailAsync(googleLoginDto.Email);
            var existingName = await _userManager.FindByNameAsync(googleLoginDto.userName);


            if (existingEmail == null && existingName == null)
            {

                var Registration = new Registration
                {
                    UserName = googleLoginDto.userName,
                    Email = googleLoginDto.Email,
                    EmailConfirmed = googleLoginDto.EmailConfirmed,
                    validity = "True",
                    Otp = GenerateOtp()

                };

               var gg = await _userManager.CreateAsync(Registration);

               
            }
         

            var user = await _userManager.FindByEmailAsync(googleLoginDto.Email);
            var roles = await _userManager.GetRolesAsync(user);

            // Define claims
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("Id", user.Id.ToString()),
        new Claim("Email", user.Email),
        new Claim("Validity", user.validity.ToString()),
        new Claim("Role", string.Join(",", roles))
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

            // Return the token and user information
            return Ok(new { token = tokenValue, user = new { user.UserName, user.Email } });




        }



    }



}
