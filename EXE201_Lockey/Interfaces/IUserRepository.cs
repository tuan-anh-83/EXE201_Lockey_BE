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
		bool UserExists(int pokeId);
	}
}
