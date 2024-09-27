using EXE201_Lockey.Dto;
using EXE201_Lockey.Interfaces;
using EXE201_Lockey.Models;
using Microsoft.AspNetCore.Mvc;

namespace EXE201_Lockey.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ThemeController : ControllerBase
	{
		private readonly IThemeRepository _themeRepository;

		public ThemeController(IThemeRepository themeRepository)
		{
			_themeRepository = themeRepository;
		}

		// Get all themes
		[HttpGet]
		[ProducesResponseType(200, Type = typeof(IEnumerable<ThemeDto>))]
		public IActionResult GetThemes()
		{
			var themes = _themeRepository.GetThemes().Select(theme => new ThemeDto
			{
				ThemeID = theme.ThemeID,
				ThemeName = theme.ThemeName
			});

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			return Ok(themes);
		}

		// Get theme by ID
		[HttpGet("{themeId}")]
		[ProducesResponseType(200, Type = typeof(ThemeDto))]
		[ProducesResponseType(400)]
		public IActionResult GetTheme(int themeId)
		{
			if (!_themeRepository.ThemeExists(themeId))
				return NotFound();

			var theme = _themeRepository.GetTheme(themeId);
			var themeDto = new ThemeDto
			{
				ThemeID = theme.ThemeID,
				ThemeName = theme.ThemeName
			};

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			return Ok(themeDto);
		}

		// Create new theme
		[HttpPost]
		[ProducesResponseType(201, Type = typeof(ThemeDto))]
		[ProducesResponseType(400)]
		public IActionResult CreateTheme([FromBody] ThemeDto themeDto)
		{
			if (themeDto == null)
				return BadRequest(ModelState);

			var theme = new Theme
			{
				ThemeName = themeDto.ThemeName
			};

			if (!_themeRepository.CreateTheme(theme))
			{
				ModelState.AddModelError("", "Something went wrong while saving the theme");
				return StatusCode(500, ModelState);
			}

			return Ok("Theme created successfully");
		}

		// Update theme
		[HttpPut("{themeId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		public IActionResult UpdateTheme(int themeId, [FromBody] ThemeDto themeDto)
		{
			if (themeDto == null || themeId != themeDto.ThemeID)
				return BadRequest(ModelState);

			if (!_themeRepository.ThemeExists(themeId))
				return NotFound();

			var theme = _themeRepository.GetTheme(themeId);
			theme.ThemeName = themeDto.ThemeName;

			if (!_themeRepository.UpdateTheme(theme))
			{
				ModelState.AddModelError("", "Something went wrong updating the theme");
				return StatusCode(500, ModelState);
			}

			return Ok("Theme updated successfully");
		}

		// Delete theme
		[HttpDelete("{themeId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		public IActionResult DeleteTheme(int themeId)
		{
			if (!_themeRepository.ThemeExists(themeId))
				return NotFound();

			var theme = _themeRepository.GetTheme(themeId);

			if (!_themeRepository.DeleteTheme(theme))
			{
				ModelState.AddModelError("", "Something went wrong deleting the theme");
				return StatusCode(500, ModelState);
			}

			return Ok("Theme deleted successfully");
		}



        [HttpGet("search")]
       
        [ProducesResponseType(400)]
        public IActionResult Seach(string search)
        {
           /* if (!_themeRepository.ThemeExists(themeId))
                return NotFound();
*/
            var theme = _themeRepository.Search(search);
            

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(theme);
        }

    }

}
