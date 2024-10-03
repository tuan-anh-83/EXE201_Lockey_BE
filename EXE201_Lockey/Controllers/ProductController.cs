using EXE201_Lockey.Services;
using Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EXE201_Lockey.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly FirebaseService _firebaseService;
        private readonly IProductService _productService;

        public ProductController(FirebaseService firebaseService, IProductService productService)
        {
            _firebaseService = firebaseService;
            _productService = productService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, int productId, int userId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File not selected");

            // Lưu file tạm thời
            var tempFilePath = Path.GetTempFileName();
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Tạo đường dẫn trong Firebase Storage
            var fileNameInStorage = $"products/{file.FileName}";
            var downloadUrl = await _firebaseService.UploadFileAsync(tempFilePath, fileNameInStorage);

            if (downloadUrl == null)
                return StatusCode(500, "Error uploading file");

            // Lưu thông tin vào Product
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
                return NotFound("Product not found");

            product.File2DLink = downloadUrl;
            product.UpdatedAt = DateTime.UtcNow;

            await _productService.AddProductAsync(product);

            return Ok(new { url = downloadUrl });
        }

    }

}
