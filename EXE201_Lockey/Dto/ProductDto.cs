namespace EXE201_Lockey.Dto
{
    public class ProductDto
    {
        public int? ProductID { get; set; } // Có thể để null khi tạo mới
        public int UserID { get; set; }
        public int TemplateID { get; set; }
        public IFormFile ImageFile { get; set; } // Để lưu file hình ảnh
        public decimal Price { get; set; }
    }
}
