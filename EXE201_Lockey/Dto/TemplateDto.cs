namespace EXE201_Lockey.Dto
{
	public class TemplateDto
	{
		public int? TemplateID { get; set; }
		public int ThemeID { get; set; }
		public string TemplateName { get; set; }
		public string TemplateImage { get; set; } // URL of the uploaded image
		public double FileTemplate { get; set; }
		public IFormFile ImageFile { get; set; }  // File uploaded from the client

        public decimal Price { get; set; }
    }
}
