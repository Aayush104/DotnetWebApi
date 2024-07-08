/*using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailSender _emailSender;

        public EmailController(AppDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        private static string GenerateOtp()
        {
            Random rnd = new Random();
            int otp = rnd.Next(1000, 10000); // Generates a number between 1000 and 9999
            return otp.ToString("D4"); // Converts the number to a string with 4 digits
        }

        [HttpPost("sendEmail")]
        public async Task<IActionResult> SendEmail( RegistrationDTO registrationDto)
        {
            try
            {
                MailRequest mailRequest = new MailRequest
                {
                    ToEmail = registrationDto.Email,
                    Subject = "User Verification Code",
                    Body = $"Your OTP Code is {GenerateOtp()}"
                };

                await _emailSender.SendEmailAsync(mailRequest);
                return Ok(); // 200 OK response if email sent successfully
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // 400 Bad Request with error message if an exception occurs
            }
        }
    }
}
*/