namespace EXE201_Lockey.Models
{
	public class Payment
	{
		public int PaymentID { get; set; }
		public int OrderID { get; set; }
		public Order Order { get; set; }
		public DateTime PaymentDate { get; set; }
		public string PaymentMethod { get; set; }
		public decimal AmountPaid { get; set; }
	}
}
