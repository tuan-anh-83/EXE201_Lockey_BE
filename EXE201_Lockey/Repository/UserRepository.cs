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
			return _context.Users.Where(p => p.Id == id).FirstOrDefault();
		}

		public User GetUserByEmail(string email)
		{
			return _context.Users.Where(p => p.Email == email).FirstOrDefault();
		}



		public ICollection<User> GetUsers()
		{
			return _context.Users.OrderBy(p => p.Id).ToList();
		}

		public bool Save()
		{
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}

		public bool UpdateUser(User user)
		{
			try
			{
				// Tìm người dùng hiện có trong cơ sở dữ liệu
				var existingUser = _context.Users.FirstOrDefault(u => u.Id == user.Id);
				if (existingUser == null)
				{
					return false; // Trả về false nếu không tìm thấy người dùng
				}

				// Cập nhật các trường nếu có giá trị mới
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

				// Nếu người dùng nhập mật khẩu mới, thì hash lại trước khi lưu
				if (!string.IsNullOrEmpty(user.Password))
				{
					existingUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
				}

				// Cập nhật người dùng trong DbContext
				_context.Users.Update(existingUser);
				return Save(); // Lưu thay đổi vào cơ sở dữ liệu
			}
			catch (Exception ex)
			{
				// Ghi lại log lỗi nếu xảy ra lỗi
				Console.WriteLine($"Error updating user: {ex.Message}");
				return false;
			}
		}


		public bool UserExists(int pokeId)
		{
			return _context.Users.Any(p => p.Id == pokeId);
		}
	}
}
