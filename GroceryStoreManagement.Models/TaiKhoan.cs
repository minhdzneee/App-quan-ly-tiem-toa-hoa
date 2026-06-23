namespace GroceryStoreManagement.Models;

public class TaiKhoan
{
    public int Id { get; set; }
    public string TenDangNhap { get; set; } = string.Empty;
    public string MatKhauHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int VaiTroId { get; set; }
    public string TenVaiTro { get; set; } = string.Empty;
    public int? NhanVienId { get; set; }
    public string? HoTenNhanVien { get; set; }

    public bool DangNhap(string passwordHash)
    {
        return IsActive && MatKhauHash == passwordHash;
    }

    public void DoiMatKhau(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        }

        MatKhauHash = passwordHash;
    }
}
