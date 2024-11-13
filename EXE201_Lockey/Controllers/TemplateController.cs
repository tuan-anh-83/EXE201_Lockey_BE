using EXE201_Lockey.Dto;
using EXE201_Lockey.Interfaces;
using EXE201_Lockey.Models;
using EXE201_Lockey.Services;
using Microsoft.AspNetCore.Mvc;



namespace EXE201_Lockey.Controllers
{
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
                FileTemplate = template.FileTemplate,
                Price = template.Price // Bao gồm Price trong dữ liệu trả về
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
                FileTemplate = template.FileTemplate,
                Price = template.Price // Bao gồm Price trong dữ liệu trả về
            };

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(templateDto);
        }

        [HttpPost("CreateTemplate")]
        public async Task<IActionResult> CreateTemplate([FromForm] TemplateDto templateDto)
        {
            if (templateDto == null || templateDto.ImageFile == null)
            {
                return BadRequest("Invalid template data or missing image file.");
            }

            if (_templateRepository.TemplateExists(templateDto.TemplateID ?? 0))
            {
                return BadRequest("Template already exists.");
            }

            var tempFilePath = FileHelper.CreateTempFile();
            await FileHelper.WriteToFileAsync(templateDto.ImageFile, tempFilePath);
            var storageFilePath = $"templates/{Guid.NewGuid()}_{templateDto.ImageFile.FileName}";
            var imageUrl = await _firebaseService.UploadFileAsync(tempFilePath, storageFilePath);
            FileHelper.DeleteTempFile(tempFilePath);

            if (string.IsNullOrEmpty(imageUrl))
            {
                return StatusCode(500, "Error uploading image to Firebase.");
            }

            var newTemplate = new Template
            {
                ThemeID = templateDto.ThemeID,
                TemplateName = templateDto.TemplateName,
                TemplateImage = imageUrl,
                FileTemplate = templateDto.FileTemplate,
                Price = templateDto.Price // Thêm Price vào đối tượng Template
            };

            if (!_templateRepository.CreateTemplate(newTemplate))
            {
                return StatusCode(500, "Error saving template.");
            }

            return Ok(new { Message = "Template created successfully", Template = newTemplate });
        }

        [HttpPut("UpdateTemplate/{templateId}")]
        public async Task<IActionResult> UpdateTemplate(int templateId, [FromForm] TemplateDto templateDto)
        {
            if (templateDto == null || templateId != templateDto.TemplateID)
            {
                return BadRequest("Invalid template data.");
            }

            if (!_templateRepository.TemplateExists(templateId))
            {
                return NotFound("Template not found.");
            }

            var templateToUpdate = _templateRepository.GetTemplate(templateId);

            templateToUpdate.ThemeID = templateDto.ThemeID;
            templateToUpdate.TemplateName = templateDto.TemplateName;
            templateToUpdate.FileTemplate = templateDto.FileTemplate;
            templateToUpdate.Price = templateDto.Price; // Cập nhật Price

            if (templateDto.ImageFile != null)
            {
                var tempFilePath = FileHelper.CreateTempFile();
                await FileHelper.WriteToFileAsync(templateDto.ImageFile, tempFilePath);

                var storageFilePath = $"templates/{Guid.NewGuid()}_{templateDto.ImageFile.FileName}";
                var imageUrl = await _firebaseService.UploadFileAsync(tempFilePath, storageFilePath);

                FileHelper.DeleteTempFile(tempFilePath);

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    templateToUpdate.TemplateImage = imageUrl;
                }
            }

            if (!_templateRepository.UpdateTemplate(templateToUpdate))
            {
                return StatusCode(500, "Error updating template.");
            }

            return Ok(new { Message = "Template updated successfully", Template = templateToUpdate });
        }

        [HttpDelete("DeleteTemplate/{templateId}")]
        public IActionResult DeleteTemplate(int templateId)
        {
            if (!_templateRepository.TemplateExists(templateId))
            {
                return NotFound("Template not found.");
            }

            var templateToDelete = _templateRepository.GetTemplate(templateId);

            if (!_templateRepository.DeleteTemplate(templateToDelete))
            {
                return StatusCode(500, "Error deleting template.");
            }

            return Ok(new { Message = "Template deleted successfully" });
        }
    }

}
