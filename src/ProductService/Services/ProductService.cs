using ProductService.Models;

namespace ProductService.Services
{
    public class ProductService : IProductService
    {

        private readonly List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Laptop", Description = "Gaming Laptop", Price = 1500.00m, Stock = 10 },
        new Product { Id = 2, Name = "Mouse", Description = "Wireless Mouse", Price = 25.00m, Stock = 50 },
        new Product { Id = 3, Name = "Keyboard", Description = "Mechanical Keyboard", Price = 75.00m, Stock = 30 }
        };

        private int _nextId = 4;

        public Task<List<Product>> GetAllAsync()
        {
            return Task.FromResult(_products.ToList());
        }

        public Task<Product?> GetByIdAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }

        public Task<Product> CreateAsync(Product product)
        {
            product.Id = _nextId++;
            product.CreatedAt = DateTime.UtcNow;
            _products.Add(product);
            return Task.FromResult(product);
        }

        public Task<Product?> UpdateAsync(int id, Product updatedProduct)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product is null) return Task.FromResult<Product?>(null);

            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;
            product.Stock = updatedProduct.Stock;

            return Task.FromResult<Product?>(product);
        }


        public Task<bool> DeleteAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);

            if (product is null) return Task.FromResult(false);

            _products.Remove(product);
            return Task.FromResult(true);

        }


    }
}
