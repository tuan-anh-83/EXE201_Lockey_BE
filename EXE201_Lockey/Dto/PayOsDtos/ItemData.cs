namespace EXE201_Lockey.Dto.PayOsDtos
{
    public class ItemData
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public ItemData(string name, int quantity, decimal price)
        {
            Name = name;
            Quantity = quantity;
            Price = price;
        }
    }

}
