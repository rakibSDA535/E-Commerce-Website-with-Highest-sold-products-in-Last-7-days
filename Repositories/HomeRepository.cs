namespace CardCore.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Genre>> Genres()
        {
            return await _db.Genres.ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetProducts(string sTerm = "", int genreId = 0)
        {
            var productQuery = _db.Products
                           .AsNoTracking()
                           .Include(x => x.Genre)
                           .Include(x => x.Stock)
                           .AsQueryable();

            if (!string.IsNullOrWhiteSpace(sTerm))
            {
                productQuery = productQuery.Where(b => b.ProductName.StartsWith(sTerm.ToLower()));
            }

            if (genreId > 0)
            {
                productQuery = productQuery.Where(b => b.GenreId == genreId);
            }

            var products = await productQuery
                .AsNoTracking()
                .Select(product => new Product
                {
                    Id = product.Id,
                    Image = product.Image,
                    CompanyName = product.CompanyName,
                    ProductName = product.ProductName,
                    GenreId = product.GenreId,
                    Price = product.Price,
                    GenreName = product.Genre.GenreName,
                    Quantity = product.Stock == null ? 0 : product.Stock.Quantity
                }).ToListAsync();

            return products;

        }
    }

}


