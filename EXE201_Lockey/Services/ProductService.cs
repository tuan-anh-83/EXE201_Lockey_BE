using EXE201_Lockey.Interfaces;
using EXE201_Lockey.Models;

namespace EXE201_Lockey.Services
{
    
        public class ProductService : IProductService
        {
            private readonly IProductRepository _productRepository;

            public ProductService(IProductRepository productRepository)
            {
                _productRepository = productRepository;
            }

            public async Task<Product> GetProductByIdAsync(int id)
            {
                return await _productRepository.GetProductByIdAsync(id);
            }

            public  bool AddProduct(Product product)
            {
                return _productRepository.UpdateProduct(product);
               
            }
        }

        public interface IProductService
        {
            Task<Product> GetProductByIdAsync(int id);
           // Task AddProductAsync(Product product);
        bool AddProduct(Product product);
        }
    
}
