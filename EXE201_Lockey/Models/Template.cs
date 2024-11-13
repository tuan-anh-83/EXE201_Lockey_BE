namespace EXE201_Lockey.Models
{
	public class Template
	{
		public int TemplateID { get; set; }
		public int ThemeID { get; set; }
		public string TemplateName { get; set; }
		public string TemplateImage { get; set; }
		public double FileTemplate { get; set; }
        public decimal Price { get; set; }

        // Navigation properties
        public Theme Theme { get; set; }
		public ICollection<Product> Products { get; set; }
	}
}
