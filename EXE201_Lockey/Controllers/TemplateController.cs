using EXE201_Lockey.Dto;
using EXE201_Lockey.Interfaces;
using EXE201_Lockey.Models;
using EXE201_Lockey.Services;
using Microsoft.AspNetCore.Mvc;



namespace EXE201_Lockey.Controllers
{
	/*[Route("api/[controller]")]
	[ApiController]
	public class TemplateController : ControllerBase
	{
		private readonly ITemplateRepository _templateRepository;

		public TemplateController(ITemplateRepository templateRepository)
		{
			_templateRepository = templateRepository;
		}

		// Get all templates
		[HttpGet]
		[ProducesResponseType(200, Type = typeof(IEnumerable<TemplateDto>))]
		public IActionResult GetTemplates()
		{
			var templates = _templateRepository.GetTemplates().Select(template => new TemplateDto
			{
				TemplateID = template.TemplateID,
				ThemeID = template.ThemeID,
				TemplateName = template.TemplateName,
				TemplateImage = template.TemplateImage,
				FileTemplate = template.FileTemplate
			});

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			return Ok(templates);
		}

		// Get template by ID
		[HttpGet("{templateId}")]
		[ProducesResponseType(200, Type = typeof(TemplateDto))]
		[ProducesResponseType(400)]
		public IActionResult GetTemplate(int templateId)
		{
			if (!_templateRepository.TemplateExists(templateId))
				return NotFound();

			var template = _templateRepository.GetTemplate(templateId);
			var templateDto = new TemplateDto
			{
				TemplateID = template.TemplateID,
				ThemeID = template.ThemeID,
				TemplateName = template.TemplateName,
				TemplateImage = template.TemplateImage,
				FileTemplate = template.FileTemplate
			};

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			return Ok(templateDto);
		}

		// Create new template
		[HttpPost]
		[ProducesResponseType(201, Type = typeof(TemplateDto))]
		[ProducesResponseType(400)]
		public IActionResult CreateTemplate([FromBody] TemplateDto templateDto)
		{
			if (templateDto == null)
				return BadRequest(ModelState);

			var template = new Template
			{
				ThemeID = templateDto.ThemeID,
				TemplateName = templateDto.TemplateName,
				TemplateImage = templateDto.TemplateImage,
				FileTemplate = templateDto.FileTemplate
			};

			if (!_templateRepository.CreateTemplate(template))
			{
				ModelState.AddModelError("", "Something went wrong while saving the template");
				return StatusCode(500, ModelState);
			}

			return Ok("Template created successfully");
		}

		// Update template
		[HttpPut("{templateId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		public IActionResult UpdateTemplate(int templateId, [FromBody] TemplateDto templateDto)
		{
			if (templateDto == null || templateId != templateDto.TemplateID)
				return BadRequest(ModelState);

			if (!_templateRepository.TemplateExists(templateId))
				return NotFound();

			var template = _templateRepository.GetTemplate(templateId);
			template.ThemeID = templateDto.ThemeID;
			template.TemplateName = templateDto.TemplateName;
			template.TemplateImage = templateDto.TemplateImage;
			template.FileTemplate = templateDto.FileTemplate;

			if (!_templateRepository.UpdateTemplate(template))
			{
				ModelState.AddModelError("", "Something went wrong updating the template");
				return StatusCode(500, ModelState);
			}

			return Ok("Template updated successfully");
		}

		// Delete template
		[HttpDelete("{templateId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		public IActionResult DeleteTemplate(int templateId)
		{
			if (!_templateRepository.TemplateExists(templateId))
				return NotFound();

			var template = _templateRepository.GetTemplate(templateId);

			if (!_templateRepository.DeleteTemplate(template))
			{
				ModelState.AddModelError("", "Something went wrong deleting the template");
				return StatusCode(500, ModelState);
			}

			return Ok("Template deleted successfully");
		}
	}*/

	[ApiController]
	[Route("api/[controller]")]
	public class TemplateController : ControllerBase
	{
		private readonly ITemplateRepository _templateRepository;
		private readonly FirebaseService _firebaseService;

		public TemplateController(ITemplateRepository templateRepository, FirebaseService firebaseService)
		{
			_templateRepository = templateRepository;
			_firebaseService = firebaseService;
		}

		[HttpPost("CreateTemplate")]
		public async Task<IActionResult> CreateTemplate([FromForm] TemplateDto templateDto)
		{
			if (templateDto == null || templateDto.ImageFile == null)
			{
				return BadRequest("Invalid template data or missing image file.");
			}

			// Kiểm tra nếu template đã tồn tại
			if (_templateRepository.TemplateExists(templateDto.TemplateID ?? 0))
			{
				return BadRequest("Template already exists.");
			}

			// Tạo file tạm thời
			var tempFilePath = FileHelper.CreateTempFile();

			// Ghi dữ liệu từ form thành file tạm
			await FileHelper.WriteToFileAsync(templateDto.ImageFile, tempFilePath);

			// Tạo đường dẫn lưu trữ trên Firebase
			var storageFilePath = $"templates/{Guid.NewGuid()}_{templateDto.ImageFile.FileName}";

			// Upload file lên Firebase
			var imageUrl = await _firebaseService.UploadFileAsync(tempFilePath, storageFilePath);

			// Xóa file tạm sau khi upload lên Firebase
			FileHelper.DeleteTempFile(tempFilePath);

			if (string.IsNullOrEmpty(imageUrl))
			{
				return StatusCode(500, "Error uploading image to Firebase.");
			}

			// Tạo template mới
			var newTemplate = new Template
			{
				ThemeID = templateDto.ThemeID,
				TemplateName = templateDto.TemplateName,
				TemplateImage = imageUrl,
				FileTemplate = templateDto.FileTemplate
			};

			// Lưu template vào cơ sở dữ liệu
			if (!_templateRepository.CreateTemplate(newTemplate))
			{
				return StatusCode(500, "Error saving template.");
			}

			return Ok(new { Message = "Template created successfully", Template = newTemplate });
		}



        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<TemplateDto>))]
        public IActionResult GetTemplates()
        {
            var templates = _templateRepository.GetTemplates().Select(template => new TemplateDto
            {
                TemplateID = template.TemplateID,
                ThemeID = template.ThemeID,
                TemplateName = template.TemplateName,
                TemplateImage = template.TemplateImage,
                FileTemplate = template.FileTemplate
            });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(templates);
        }


        [HttpGet("{templateId}")]
        [ProducesResponseType(200, Type = typeof(TemplateDto))]
        [ProducesResponseType(400)]
        public IActionResult GetTemplate(int templateId)
        {
            if (!_templateRepository.TemplateExists(templateId))
                return NotFound();

            var template = _templateRepository.GetTemplate(templateId);
            var templateDto = new TemplateDto
            {
                TemplateID = template.TemplateID,
                ThemeID = template.ThemeID,
                TemplateName = template.TemplateName,
                TemplateImage = template.TemplateImage,
                FileTemplate = template.FileTemplate
            };

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(templateDto);
        }

    }


}
