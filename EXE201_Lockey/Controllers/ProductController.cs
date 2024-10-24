using EXE201_Lockey.Dto;
using EXE201_Lockey.Interfaces;
using EXE201_Lockey.Models;
using EXE201_Lockey.Repository;
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
        private readonly IProductRepository _productRepository;
        private readonly ITemplateRepository _templateRepository;

        public ProductController(FirebaseService firebaseService, IProductService productService, IProductRepository productRepository, ITemplateRepository templateRepository)
        {
            _firebaseService = firebaseService;
            _productService = productService;
            _productRepository = productRepository;
            _templateRepository = templateRepository;
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

            product.Price = 0;
            product.CreatedAt = DateTime.Now;
            product.File2DLink = downloadUrl;
            product.UpdatedAt = DateTime.UtcNow;

            _productService.AddProduct(product);

            return Ok(new { url = downloadUrl });
        }



        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductDto productDto)
        {
            if (productDto == null || productDto.ImageFile == null)
            {
                return BadRequest("Invalid product data or missing image file.");
            }

            // Lưu file tạm thời trên hệ thống
            var tempFilePath = Path.GetTempFileName();
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await productDto.ImageFile.CopyToAsync(stream);
            }

            // Tạo đường dẫn lưu trữ trên Firebase
            var storageFilePath = $"products/{Guid.NewGuid()}_{productDto.ImageFile.FileName}";

            // Upload file lên Firebase
            var imageUrl = await _firebaseService.UploadFileAsync(tempFilePath, storageFilePath);

            // Xóa file tạm sau khi upload lên Firebase
            System.IO.File.Delete(tempFilePath);

            if (string.IsNullOrEmpty(imageUrl))
            {
                return StatusCode(500, "Error uploading image to Firebase.");
            }


            var newProduct = new Product
            {
                UserID = productDto.UserID,
                TemplateID = productDto.TemplateID,
                File2DLink = imageUrl,  // Lưu đường dẫn file trên Firebase
                Preview3D = "string",
                Price = productDto.Price,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Lưu sản phẩm vào cơ sở dữ liệu
            var isProductCreated = _productRepository.CreateProduct(newProduct);
            if (!isProductCreated)
            {
                return StatusCode(500, "Error saving product.");
            }

            return Ok(new { Message = "Product created successfully", Product = newProduct });
        }


        // Tương tự như phần bạn đã làm cho template, tôi để phần GetProducts và GetProduct dưới đây để tham khảo.

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDto>))]
        public IActionResult GetProducts()
        {
            var products = _productRepository.GetProducts().Select(product => new ProductDto
            {
                ProductID = product.ProductID,
                UserID = product.UserID,
                TemplateID = product.TemplateID,
                Price = product.Price
            });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(products);
        }

        [HttpGet("{productId}")]
        [ProducesResponseType(200, Type = typeof(ProductDto))]
        [ProducesResponseType(400)]
        public IActionResult GetProduct(int productId)
        {
            if (!_productRepository.ProductExists(productId))
                return NotFound();

            var product = _productRepository.GetProduct(productId);
            var productDto = new ProductDto
            {
                ProductID = product.ProductID,
                UserID = product.UserID,
                TemplateID = product.TemplateID,
                Price = product.Price
            };

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(productDto);
        }






    }

}
