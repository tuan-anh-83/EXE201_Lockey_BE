namespace EXE201_Lockey.Dto.PayOsDtos
{
    public class PaymentData
    {
        public int OrderCode { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public List<ItemData> Items { get; set; }
        public string CancelUrl { get; set; }
        public string ReturnUrl { get; set; }

        public PaymentData(int orderCode, decimal amount, string description, List<ItemData> items, string cancelUrl, string returnUrl)
        {
            OrderCode = orderCode;
            Amount = amount;
            Description = description;
            Items = items;
            CancelUrl = cancelUrl;
            ReturnUrl = returnUrl;
        }
    }

}
