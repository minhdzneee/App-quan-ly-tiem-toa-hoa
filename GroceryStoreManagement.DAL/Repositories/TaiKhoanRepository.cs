using GroceryStoreManagement.DAL.Database;
using GroceryStoreManagement.Models;
using Microsoft.Data.SqlClient;

namespace GroceryStoreManagement.DAL.Repositories;

public class TaiKhoanRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TaiKhoanRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<TaiKhoan?> GetByTenDangNhapAsync(string tenDangNhap)
    {
        const string sql = """
            SELECT tk.Id, tk.TenDangNhap, tk.MatKhauHash, tk.IsActive, tk.VaiTroId,
                   vt.TenVaiTro, nv.Id AS NhanVienId, nv.HoTen AS HoTenNhanVien
            FROM TaiKhoan tk
            INNER JOIN VaiTro vt ON vt.Id = tk.VaiTroId
            LEFT JOIN NhanVien nv ON nv.TaiKhoanId = tk.Id
            WHERE tk.TenDangNhap = @TenDangNhap;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new TaiKhoan
        {
            Id = reader.GetInt32Value("Id"),
            TenDangNhap = reader.GetString(reader.GetOrdinal("TenDangNhap")),
            MatKhauHash = reader.GetString(reader.GetOrdinal("MatKhauHash")),
            IsActive = reader.GetBooleanValue("IsActive"),
            VaiTroId = reader.GetInt32Value("VaiTroId"),
            TenVaiTro = reader.GetString(reader.GetOrdinal("TenVaiTro")),
            NhanVienId = reader.GetNullableInt32("NhanVienId"),
            HoTenNhanVien = reader.GetNullableString("HoTenNhanVien")
        };
    }

    public async Task DoiMatKhauAsync(int taiKhoanId, string matKhauHash)
    {
        const string sql = "UPDATE TaiKhoan SET MatKhauHash = @MatKhauHash WHERE Id = @Id;";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", taiKhoanId);
        command.Parameters.AddWithValue("@MatKhauHash", matKhauHash);
        await command.ExecuteNonQueryAsync();
    }

    public async Task GhiLichSuTruyCapAsync(int taiKhoanId, string hanhDong, string? diaChiIp = null)
    {
        const string sql = """
            INSERT INTO LichSuTruyCap(TaiKhoanId, ThoiGian, HanhDong, DiaChiIP)
            VALUES (@TaiKhoanId, SYSDATETIME(), @HanhDong, @DiaChiIP);
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@TaiKhoanId", taiKhoanId);
        command.Parameters.AddWithValue("@HanhDong", hanhDong);
        command.Parameters.AddWithValue("@DiaChiIP", (object?)diaChiIp ?? DBNull.Value);
        await command.ExecuteNonQueryAsync();
    }
}
