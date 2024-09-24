namespace EXE201_Lockey.Models
{
	public class Product
	{
		public int ProductID { get; set; }
		public int DesignID { get; set; }
		public CustomDesign Design { get; set; }
		public decimal Price { get; set; }
		public int Stock { get; set; }

		public ICollection<OrderDetail> OrderDetails { get; set; }
	}
}
