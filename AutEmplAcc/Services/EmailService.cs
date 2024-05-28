using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace AutEmplAcc.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string body);

    }
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_configuration["Email:From"]));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_configuration["Email:SmtpServer"],
                                     int.Parse(_configuration["Email:Port"]),
                                     SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(_configuration["Email:Username"],
                                        _configuration["Email:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
