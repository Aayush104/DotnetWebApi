using Backend.Models;

namespace Backend.Services
{
    public interface IEmailSender
    {

        Task SendEmailAsync(MailRequest mailRequest);
    }
}
