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
        var taiKhoan = await _taiKhoanRepository.GetByIdAsync(taiKhoanId);
        if (taiKhoan is null)
        {
            throw new InvalidOperationException("Tai khoan khong ton tai.");
        }

        if (!taiKhoan.DangNhap(HashPassword(matKhauCu)))
        {
            throw new InvalidOperationException("Mat khau cu khong dung.");
        }

        if (string.IsNullOrWhiteSpace(matKhauMoi) || matKhauMoi.Length < 6)
        {
            throw new InvalidOperationException("Mat khau moi phai co toi thieu 6 ky tu.");
        }

        await _taiKhoanRepository.DoiMatKhauAsync(taiKhoanId, HashPassword(matKhauMoi));
        await _taiKhoanRepository.GhiLichSuTruyCapAsync(taiKhoanId, "Doi mat khau");
    }

    public Task LogoutAsync(int taiKhoanId)
    {
        return _taiKhoanRepository.GhiLichSuTruyCapAsync(taiKhoanId, "Dang xuat he thong");
    }

    public Task<List<VaiTro>> GetVaiTrosAsync()
    {
        return _taiKhoanRepository.GetVaiTrosAsync();
    }

    public async Task RegisterAsync(string tenDangNhap, string matKhau, string xacNhanMatKhau, int vaiTroId, string hoTen)
    {
        if (string.IsNullOrWhiteSpace(tenDangNhap))
        {
            throw new InvalidOperationException("Ten dang nhap la bat buoc.");
        }

        if (string.IsNullOrWhiteSpace(hoTen))
        {
            throw new InvalidOperationException("Ho ten nhan vien la bat buoc.");
        }

        if (string.IsNullOrWhiteSpace(matKhau) || matKhau.Length < 6)
        {
            throw new InvalidOperationException("Mat khau phai co toi thieu 6 ky tu.");
        }

        if (matKhau != xacNhanMatKhau)
        {
            throw new InvalidOperationException("Xac nhan mat khau khong khop.");
        }

        if (vaiTroId <= 0)
        {
            throw new InvalidOperationException("Vui long chon vai tro.");
        }

        if (await _taiKhoanRepository.ExistsAsync(tenDangNhap.Trim()))
        {
            throw new InvalidOperationException("Ten dang nhap da ton tai.");
        }

        var taiKhoan = new TaiKhoan
        {
            TenDangNhap = tenDangNhap.Trim(),
            MatKhauHash = HashPassword(matKhau),
            VaiTroId = vaiTroId,
            IsActive = true
        };

        var nhanVien = new NhanVien
        {
            MaNhanVien = $"NV{DateTime.Now:yyyyMMddHHmmss}",
            HoTen = hoTen.Trim(),
            ChucVu = "Nhan vien"
        };

        await _taiKhoanRepository.CreateWithNhanVienAsync(taiKhoan, nhanVien);
    }

    public static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}
