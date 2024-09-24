namespace EXE201_Lockey.Models
{
	public class Order
	{
		public int OrderID { get; set; }
		public int UserID { get; set; }
		public User User { get; set; }
		public DateTime OrderDate { get; set; }
		public decimal TotalAmount { get; set; }
		public string PaymentStatus { get; set; }
		public string ShippingStatus { get; set; }  // Processing, Shipped

		// Navigation properties
		public ICollection<OrderDetail> OrderDetails { get; set; }
		public Payment Payment { get; set; }  // One-to-One relationship with Payment
	}
}
