namespace EXE201_Lockey.Models
{
	public class Template
	{
		public int TemplateID { get; set; }
		public string TemplateName { get; set; }
		public string Description { get; set; }
		public string FrameType { get; set; }
		public string BaseColor { get; set; }

		public ICollection<CustomDesign> CustomDesigns { get; set; }
	}
}
