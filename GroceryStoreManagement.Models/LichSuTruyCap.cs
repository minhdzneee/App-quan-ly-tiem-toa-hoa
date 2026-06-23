namespace GroceryStoreManagement.Models;

public class LichSuTruyCap
{
    public int Id { get; set; }
    public int TaiKhoanId { get; set; }
    public DateTime ThoiGian { get; set; } = DateTime.Now;
    public string HanhDong { get; set; } = string.Empty;
    public string? DiaChiIP { get; set; }
}
