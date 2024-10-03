namespace EXE201_Lockey.Dto
{
	public class ResetPasswordDto
	{
		public string Email { get; set; }
		public string Token { get; set; }  // Token reset mật khẩu được gửi qua email
		public string NewPassword { get; set; }
	}
}
