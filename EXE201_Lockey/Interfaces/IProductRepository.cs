﻿using EXE201_Lockey.Models;

namespace EXE201_Lockey.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetProductByIdAsync(int id);
        Task AddProductAsync(Product product);
        Task SaveAsync();
         bool UpdateProduct(Product product);
         bool Save();
    }
}