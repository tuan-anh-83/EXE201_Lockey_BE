using System.Net.Mail;
using System.Net;

namespace EXE201_Lockey.Services
{
	public class EmailService : IEmailService
	{
		private readonly IConfiguration _configuration;

		public EmailService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public void SendPasswordResetEmail(string email, string resetUrl)
		{
			var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:SmtpPort"]))
			{
				Credentials = new NetworkCredential(_configuration["EmailSettings:SenderEmail"], _configuration["EmailSettings:SenderPassword"]),
				EnableSsl = true
			};

			var mailMessage = new MailMessage
			{
				From = new MailAddress(_configuration["EmailSettings:SenderEmail"]),
				Subject = "Reset your password",
				Body = $"Please reset your password by clicking the following link: {resetUrl}",
				IsBodyHtml = true
			};
			mailMessage.To.Add(email);

			smtpClient.Send(mailMessage);
		}
	}
}
