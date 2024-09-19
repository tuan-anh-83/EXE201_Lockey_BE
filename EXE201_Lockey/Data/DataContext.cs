using EXE201_Lockey.Models;
using Microsoft.EntityFrameworkCore;

namespace EXE201_Lockey.Data
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{

		}

		public DbSet<User> User { get; set; }
	}
}
