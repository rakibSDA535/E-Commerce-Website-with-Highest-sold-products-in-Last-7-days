using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CardCore.Repositories
{
    public interface IProductRepository
    {
         Task AddProduct(Product product);
         Task UpdateProduct(Product product);
         Task DeleteProduct(Product product);
         Task<Product?> GetProductById(int id);
         Task<IEnumerable<Product>> GetProducts();
         IQueryable<Product> GetAllProductsQueryable();

    }

    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task AddProduct(Product product)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
            }

            public async Task UpdateProduct(Product product)
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }

            public async Task DeleteProduct(Product product)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        //======================xx
        public async Task<Product?> GetProductById(int id)
        {
            return await _context.Products
                .Include(p => p.Genre)
                .Include(p => p.ProductInformations)//
                .Include(p => p.Stock)//
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        //==============================xx
        public async Task<IEnumerable<Product>> GetProducts() => await _context.Products.Include(a => a.Genre).ToListAsync();
        public IQueryable<Product> GetAllProductsQueryable()
        {
            return _context.Products.Include(p => p.Genre)
                                    .Include(p => p.ProductInformations)//for masterdetail view data altogether
                                    .Include(p => p.Stock)//for masterdetail view data altogether
                                    .AsQueryable();
        }
    }
    
}
