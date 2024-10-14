using EXE201_Lockey.Data;
using EXE201_Lockey.Interfaces;
using EXE201_Lockey.Models;
using Google;
using Microsoft.EntityFrameworkCore;

namespace EXE201_Lockey.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;

        public ProductRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.Where(p => p.ProductID == id).FirstOrDefaultAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task AddProductAsync(Product product)
        {
            
            await _context.Products.AddAsync(product);
        }
        public bool UpdateProduct(Product product)
        {
             _context.Products.Update(product);
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
        public bool ProductExists(int productId)
        {
            return _context.Products.Any(t => t.ProductID == productId);
        }

        public Product GetProduct(int id)
        {
            return _context.Products.FirstOrDefault(t => t.ProductID == id);
        }
        public bool CreateProduct(Product product)
        {
            _context.Products.Add(product);
            return Save();
        }
        public ICollection<Product> GetProducts()
        {
            return _context.Products.OrderBy(t => t.ProductID).ToList();
        }
    }
}

