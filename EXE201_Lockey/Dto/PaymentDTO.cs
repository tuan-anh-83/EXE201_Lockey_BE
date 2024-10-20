namespace EXE201_Lockey.Dto
{
    public class PaymentDTO
    {
        public int PaymentID { get; set; }
        public int OrderID { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public DateTime PaymentDate { get; set; }
    }


}
