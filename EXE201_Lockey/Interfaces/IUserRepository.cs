using EXE201_Lockey.Models;

namespace EXE201_Lockey.Interfaces
{
	public interface IUserRepository
	{
		ICollection<User> GetUsers();
		User GetUser(int id);
		User GetUserByEmail(string email);

		bool CreateUser(User user);
		bool UpdateUser(User user);
		bool DeleteUser(User user);
		bool Save();
		bool UserExists(int userId);


		// Các phương thức mới cho Forgot Password
		bool SavePasswordResetToken(int userId, string token);  // Lưu token reset password
		string GetPasswordResetTokenByEmail(string email);      // Lấy token theo email người dùng
		bool ResetPassword(string email, string newPassword);   // Đặt lại mật khẩu

		public User GetUserByOtp(string otp);
	}

}
