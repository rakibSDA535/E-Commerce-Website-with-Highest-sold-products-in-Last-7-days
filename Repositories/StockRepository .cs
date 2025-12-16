using Microsoft.EntityFrameworkCore;

namespace CardCore.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _context;

        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Stock?> GetStockByProductId(int productId) => await _context.Stocks.FirstOrDefaultAsync(s => s.ProductId == productId);

        public async Task ManageStock(StockDTO stockToManage)
        {
            var existingStock = await GetStockByProductId(stockToManage.ProductId);
            if (existingStock is null)
            {
                var stock = new Stock { ProductId = stockToManage.ProductId, Quantity = stockToManage.Quantity };
                _context.Stocks.Add(stock);
            }
            else
            {
                existingStock.Quantity = stockToManage.Quantity;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<StockDisplayModel>> GetStocks(string sTerm = "")
        {
            var stocksQuery = _context.Products
                                      .AsNoTracking()
                                      .Include(b => b.Stock)
                                      .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(sTerm))
            {
                stocksQuery = stocksQuery.Where(b => b.ProductName.StartsWith(sTerm.ToLower()));
            }

            var stocks = stocksQuery
                .AsNoTracking()
                .Select(product => new StockDisplayModel
                {
                    ProductId = product.Id,
                    ProductName = product.ProductName,
                    Quantity = product.Stock == null ? 0 : product.Stock.Quantity
                });
            return stocks;
        }

    }

    public interface IStockRepository
    {
        Task<IEnumerable<StockDisplayModel>> GetStocks(string sTerm = "");
        Task<Stock?> GetStockByProductId(int productId);
        Task ManageStock(StockDTO stockToManage);
    }
}