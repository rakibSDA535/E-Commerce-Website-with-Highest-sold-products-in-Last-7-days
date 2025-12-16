using System.ComponentModel.DataAnnotations;

namespace CardCore.Models.DTOs
{
    public class ProductInformationDTO
    {
        public int Id { get; set; }
        [MaxLength(2000)]
        public string? Description { get; set; }
        [MaxLength(50)]
        public string? MadeIn { get; set; }
    }
}
