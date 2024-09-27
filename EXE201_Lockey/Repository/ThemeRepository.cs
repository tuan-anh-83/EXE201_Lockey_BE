using EXE201_Lockey.Data;
using EXE201_Lockey.Interfaces;
using EXE201_Lockey.Models;
using Microsoft.EntityFrameworkCore;


namespace EXE201_Lockey.Repository
{
	public class ThemeRepository : IThemeRepository
	{
		private readonly DataContext _context;

		public ThemeRepository(DataContext context)
		{
			_context = context;
		}

		public ICollection<Theme> GetThemes()
		{
			return _context.Themes.Include(t => t.Templates).OrderBy(t => t.ThemeID).ToList();
		}

		public Theme GetTheme(int themeId)
		{
			return _context.Themes.Include(t => t.Templates).FirstOrDefault(t => t.ThemeID == themeId);
		}

		public bool ThemeExists(int themeId)
		{
			return _context.Themes.Any(t => t.ThemeID == themeId);
		}

		public bool CreateTheme(Theme theme)
		{
			_context.Themes.Add(theme);
			return Save();
		}

		public bool UpdateTheme(Theme theme)
		{
			_context.Themes.Update(theme);
			return Save();
		}

		public bool DeleteTheme(Theme theme)
		{
			_context.Themes.Remove(theme);
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

        public ICollection<Theme> Search(string searchInfo)
        {
			return _context.Themes.Where(t => t.ThemeName.Contains(searchInfo)).ToList();
        }
    }

}
