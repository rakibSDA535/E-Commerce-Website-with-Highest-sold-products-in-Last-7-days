using CardCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CardCore.Models.DTOs
{
    public class UpdateOrderStatusModel
    {
        public int OrderId { get; set; }

        [Required]
        public EOrderStatus OrderStatus { get; set; }

        public IEnumerable<SelectListItem>? OrderStatusList { get; set; }

    }
}

