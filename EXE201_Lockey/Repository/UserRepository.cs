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
            _context.User.Add(account);
			return Save();
		}

		public bool DeleteUser(User user)
		{
			throw new NotImplementedException();
		}

		public User GetUser(int id)
		{
			return _context.User.Where(p => p.Id == id).FirstOrDefault();
		}

		public User GetUserByEmail(string email)
		{
			return _context.User.Where(p => p.Email == email).FirstOrDefault();
		}

       

        public ICollection<User> GetUsers()
		{
			return _context.User.OrderBy(p => p.Id).ToList();
		}

		public bool Save()
		{
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}

		public bool UpdateUser(User user)
		{
			throw new NotImplementedException();
		}

		public bool UserExists(int pokeId)
		{
			return _context.User.Any(p => p.Id == pokeId);
		}
	}
}
