using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardCore.Models
{
    [Table("ProductDetail")]
    public class ProductInformation
    {
        public int Id { get; set; }
        [MaxLength(2000)]
        public string? Description { get; set; }
        [MaxLength(50)]
        public string? MadeIn { get; set; }
        [ForeignKey("Device")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
