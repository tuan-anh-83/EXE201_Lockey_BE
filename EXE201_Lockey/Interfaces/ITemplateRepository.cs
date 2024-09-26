using EXE201_Lockey.Models;

namespace EXE201_Lockey.Interfaces
{
	public interface ITemplateRepository
	{
		ICollection<Template> GetTemplates();
		Template GetTemplate(int templateId);
		bool TemplateExists(int templateId);
		bool CreateTemplate(Template template);
		bool UpdateTemplate(Template template);
		bool DeleteTemplate(Template template);
		bool Save();
	}
}
