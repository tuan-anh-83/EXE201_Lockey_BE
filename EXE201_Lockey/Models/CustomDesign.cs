using System.ComponentModel.DataAnnotations;

namespace EXE201_Lockey.Models
{
	public class CustomDesign
	{
		[Key]
		public int DesignID { get; set; }  // Đây là khóa chính
		public int UserID { get; set; }
		public User User { get; set; }
		public int TemplateID { get; set; }
		public Template Template { get; set; }
		public string UploadedImage { get; set; }
		public string ChosenColor { get; set; }
		public string Preview3D { get; set; }
		public string DesignStatus { get; set; }  // Pending, Completed

		// Navigation properties
		public Product Product { get; set; }
	}
}
