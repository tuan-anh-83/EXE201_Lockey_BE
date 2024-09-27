using EXE201_Lockey.Models;

namespace EXE201_Lockey.Interfaces
{
	public interface IThemeRepository
	{
		ICollection<Theme> GetThemes();
		Theme GetTheme(int themeId);
		bool ThemeExists(int themeId);
		bool CreateTheme(Theme theme);
		bool UpdateTheme(Theme theme);
		bool DeleteTheme(Theme theme);
		bool Save();
	}
}
