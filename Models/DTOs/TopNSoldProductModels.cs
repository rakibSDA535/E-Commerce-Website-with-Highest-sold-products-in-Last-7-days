namespace CardCore.Models.DTOs;

public record TopNSoldProductModel(string ProductName, string CompanyName,string Image, int TotalUnitSold);
//OR
//public class TopNSoldProductModel
//{
//    public string? ProductName { get; set; }
//    public string? CompanyName { get; set; }
//    public string? Image { get; set; }
//    public int TotalUnitSold { get; set; }
//}
public record TopNSoldProductsVm(DateTime StartDate, DateTime EndDate, IEnumerable<TopNSoldProductModel> TopNSoldProducts);
