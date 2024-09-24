namespace EXE201_Lockey.Models
{
	public class Theme
	{
		public int ThemeID { get; set; }
		public string ThemeName { get; set; }

		// Navigation properties
		public ICollection<Template> Templates { get; set; }
	}
}
