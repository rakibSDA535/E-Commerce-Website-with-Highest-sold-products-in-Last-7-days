using CardCore.Models;

namespace CardCore.Models.DTOs
{
    public class OrderDetailModalDTO
    {
        public string DivId { get; set; }
        public IEnumerable<OrderDetail> OrderDetail { get; set; }
    }
}

