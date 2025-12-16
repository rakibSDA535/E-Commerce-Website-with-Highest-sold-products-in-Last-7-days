using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using CardCore.Models.DTOs;

namespace CardCore.Repositories
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;
        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TopNSoldProductModel>> GetTopNSellingProductsByDate(DateTime startDate, DateTime endDate)
        {
            var startDateParam = new SqlParameter("@startDate", startDate);
            var endDateParam = new SqlParameter("@endDate", endDate);
            var topFiveSoldProducts = await _context.Database.SqlQueryRaw<TopNSoldProductModel>("exec Usp_GetTopNSellingProductsByDate @startDate,@endDate", startDateParam, endDateParam).ToListAsync();
            return topFiveSoldProducts;
        }

    }
    public interface IReportRepository
    {
        Task<IEnumerable<TopNSoldProductModel>> GetTopNSellingProductsByDate(DateTime startDate, DateTime endDate);
    }
}
