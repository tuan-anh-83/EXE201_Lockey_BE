namespace EXE201_Lockey.Models
{
	public class Payment
	{
		public int PaymentID { get; set; }
		public int OrderID { get; set; }
		public string PaymentMethod { get; set; }
		public string Status { get; set; }
		public DateTime PaymentDate { get; set; }

		// Navigation property
		public Order Order { get; set; }
	}
}
