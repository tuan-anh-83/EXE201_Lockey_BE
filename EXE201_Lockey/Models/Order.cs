namespace EXE201_Lockey.Models
{
	public class Order
	{
		public int OrderID { get; set; }
		public int ProductID { get; set; }
		public DateTime OrderDate { get; set; }
		public decimal Amount { get; set; }
		public string Status { get; set; }
		public decimal TotalPrice { get; set; }

		// Navigation properties
		public Product Product { get; set; }
		public Payment Payment { get; set; }
	}
}
