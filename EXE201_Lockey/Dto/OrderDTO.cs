namespace EXE201_Lockey.Dto
{
    public class OrderDTO
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
    }

}
