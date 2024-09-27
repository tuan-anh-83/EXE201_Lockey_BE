using EXE201_Lockey.Data;
using EXE201_Lockey.Interfaces;
using EXE201_Lockey.Models;
using System.Data;

namespace EXE201_Lockey.Repository
{
	public class UserRepository : IUserRepository
	{
		private readonly DataContext _context;

		public UserRepository(DataContext context)
		{
			_context = context;
		}

		public bool CreateUser(User user)
		{
			user.Id = 0;
			var account = new User
			{
				Name = user.Name,
				Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
				Phone = user.Phone,
				Email = user.Email,
				Role = "Customer",
				Address = user.Address,
			};
			_context.Users.Add(account);
			return Save();
		}

		public bool DeleteUser(User user)
		{
			_context.Users.Remove(user);
			return Save();
		}

		public User GetUser(int id)
		{
			return _context.Users.FirstOrDefault(p => p.Id == id);
		}

		public User GetUserByEmail(string email)
		{
			return _context.Users.FirstOrDefault(p => p.Email == email);
		}

		public ICollection<User> GetUsers()
		{
			return _context.Users.OrderBy(p => p.Id).ToList();
		}

		public bool Save()
		{
			var saved = _context.SaveChanges();
			return saved > 0;
		}

		public bool UpdateUser(User user)
		{
			try
			{
				var existingUser = _context.Users.FirstOrDefault(u => u.Id == user.Id);
				if (existingUser == null)
				{
					return false;
				}

				if (!string.IsNullOrEmpty(user.Name))
				{
					existingUser.Name = user.Name;
				}

				if (!string.IsNullOrEmpty(user.Email))
				{
					existingUser.Email = user.Email;
				}

				if (!string.IsNullOrEmpty(user.Phone))
				{
					existingUser.Phone = user.Phone;
				}

				if (!string.IsNullOrEmpty(user.Address))
				{
					existingUser.Address = user.Address;
				}

				if (!string.IsNullOrEmpty(user.Password))
				{
					existingUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
				}

				_context.Users.Update(existingUser);
				return Save();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error updating user: {ex.Message}");
				return false;
			}
		}

		public bool UserExists(int userId)
		{
			return _context.Users.Any(p => p.Id == userId);
		}

		// Các phương thức mới cho Forgot Password
		public bool SavePasswordResetToken(int userId, string token)
		{
			var user = GetUser(userId);
			if (user == null)
			{
				return false;
			}

			user.PasswordResetToken = token;
			return Save();
		}

		public string GetPasswordResetTokenByEmail(string email)
		{
			var user = GetUserByEmail(email);
			return user?.PasswordResetToken;
		}

		public bool ResetPassword(string email, string newPassword)
		{
			var user = GetUserByEmail(email);
			if (user == null)
			{
				return false;
			}

			user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
			user.PasswordResetToken = null;  // Xóa token sau khi đặt lại mật khẩu thành công
			return Save();
		}
	}

}
