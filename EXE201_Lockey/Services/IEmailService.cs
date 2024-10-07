namespace EXE201_Lockey.Services
{
	public interface IEmailService
	{
		void SendPasswordResetEmail(string email, string resetUrl);
		void SendOTP(string email, string resetUrl);
	}
}
