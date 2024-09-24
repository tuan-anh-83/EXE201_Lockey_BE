namespace EXE201_Lockey.Models
{
	public class Product
	{
		public int ProductID { get; set; }
		public int UserID { get; set; }
		public int TemplateID { get; set; }
		public string File2DLink { get; set; }
		public string Preview3D { get; set; }
		public DateTime CreatedAt { get; set; }
		public decimal Price { get; set; }
		public DateTime UpdatedAt { get; set; }

		// Navigation properties
		public User User { get; set; }
		public Template Template { get; set; }
		public ICollection<Order> Orders { get; set; }
	}
}
