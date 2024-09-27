using EXE201_Lockey.Data;
using EXE201_Lockey.Interfaces;
using EXE201_Lockey.Models;

namespace EXE201_Lockey.Repository
{
	public class TemplateRepository : ITemplateRepository
	{
		private readonly DataContext _context;

		public TemplateRepository(DataContext context)
		{
			_context = context;
		}

		public ICollection<Template> GetTemplates()
		{
			return _context.Templates.OrderBy(t => t.TemplateID).ToList();
		}

		public Template GetTemplate(int templateId)
		{
			return _context.Templates.FirstOrDefault(t => t.TemplateID == templateId);
		}

		public bool TemplateExists(int templateId)
		{
			return _context.Templates.Any(t => t.TemplateID == templateId);
		}

		public bool CreateTemplate(Template template)
		{
			_context.Templates.Add(template);
			return Save();
		}

		public bool UpdateTemplate(Template template)
		{
			_context.Templates.Update(template);
			return Save();
		}

		public bool DeleteTemplate(Template template)
		{
			_context.Templates.Remove(template);
			return Save();
		}

		public bool Save()
		{
			try
			{
				return _context.SaveChanges() > 0;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error saving changes: {ex.Message}");
				return false;
			}
		}
	}

}
