using System.Security.Cryptography;
using System.Text;
using GroceryStoreManagement.DAL.Repositories;
using GroceryStoreManagement.Models;

namespace GroceryStoreManagement.BLL.Services;

public class AuthService
{
    private readonly TaiKhoanRepository _taiKhoanRepository;

    public AuthService(TaiKhoanRepository taiKhoanRepository)
    {
        _taiKhoanRepository = taiKhoanRepository;
    }

    public async Task<TaiKhoan> LoginAsync(string tenDangNhap, string matKhau)
    {
        if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
        {
            throw new InvalidOperationException("Vui long nhap ten dang nhap va mat khau.");
        }

        var taiKhoan = await _taiKhoanRepository.GetByTenDangNhapAsync(tenDangNhap.Trim());
        if (taiKhoan is null)
        {
            throw new InvalidOperationException("Tai khoan khong ton tai.");
        }

        if (!taiKhoan.IsActive)
        {
            throw new InvalidOperationException("Tai khoan da bi khoa.");
        }

        if (!taiKhoan.DangNhap(HashPassword(matKhau)))
        {
            await _taiKhoanRepository.GhiLichSuTruyCapAsync(taiKhoan.Id, "Dang nhap that bai");
            throw new InvalidOperationException("Mat khau khong dung.");
        }

        await _taiKhoanRepository.GhiLichSuTruyCapAsync(taiKhoan.Id, "Dang nhap thanh cong");
        return taiKhoan;
    }

    public async Task DoiMatKhauAsync(int taiKhoanId, string matKhauCu, string matKhauMoi)
    {
        if (string.IsNullOrWhiteSpace(matKhauMoi) || matKhauMoi.Length < 6)
        {
            throw new InvalidOperationException("Mat khau moi phai co toi thieu 6 ky tu.");
        }

        await _taiKhoanRepository.DoiMatKhauAsync(taiKhoanId, HashPassword(matKhauMoi));
        await _taiKhoanRepository.GhiLichSuTruyCapAsync(taiKhoanId, "Doi mat khau");
    }

    public static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}
