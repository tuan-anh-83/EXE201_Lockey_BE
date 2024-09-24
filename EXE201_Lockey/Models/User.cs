namespace EXE201_Lockey.Models
{
	public class User
	{

		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string Role { get; set; }  // Admin, Customer
		public string Phone { get; set; }
		public string Address { get; set; }

		public ICollection<Order> Orders { get; set; }
		public ICollection<CustomDesign> CustomDesigns { get; set; }
		
	}
}
