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

    public async Task<TaiKhoan?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT tk.Id, tk.TenDangNhap, tk.MatKhauHash, tk.IsActive, tk.VaiTroId,
                   vt.TenVaiTro, nv.Id AS NhanVienId, nv.HoTen AS HoTenNhanVien
            FROM TaiKhoan tk
            INNER JOIN VaiTro vt ON vt.Id = tk.VaiTroId
            LEFT JOIN NhanVien nv ON nv.TaiKhoanId = tk.Id
            WHERE tk.Id = @Id;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

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

    public async Task<List<VaiTro>> GetVaiTrosAsync()
    {
        const string sql = "SELECT Id, TenVaiTro, MoTa FROM VaiTro ORDER BY TenVaiTro;";
        var result = new List<VaiTro>();

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new VaiTro
            {
                Id = reader.GetInt32Value("Id"),
                TenVaiTro = reader.GetString(reader.GetOrdinal("TenVaiTro")),
                MoTa = reader.GetNullableString("MoTa")
            });
        }

        return result;
    }

    public async Task<bool> ExistsAsync(string tenDangNhap)
    {
        const string sql = "SELECT COUNT(1) FROM TaiKhoan WHERE TenDangNhap = @TenDangNhap;";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
        return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
    }

    public async Task<int> CreateWithNhanVienAsync(TaiKhoan taiKhoan, NhanVien nhanVien)
    {
        const string insertAccountSql = """
            INSERT INTO TaiKhoan(TenDangNhap, MatKhauHash, IsActive, VaiTroId)
            OUTPUT INSERTED.Id
            VALUES (@TenDangNhap, @MatKhauHash, @IsActive, @VaiTroId);
            """;

        const string insertEmployeeSql = """
            INSERT INTO NhanVien(MaNhanVien, HoTen, ChucVu, SoDienThoai, DiaChi, TaiKhoanId)
            OUTPUT INSERTED.Id
            VALUES (@MaNhanVien, @HoTen, @ChucVu, @SoDienThoai, @DiaChi, @TaiKhoanId);
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            await using var insertAccount = new SqlCommand(insertAccountSql, connection, transaction);
            insertAccount.Parameters.AddWithValue("@TenDangNhap", taiKhoan.TenDangNhap);
            insertAccount.Parameters.AddWithValue("@MatKhauHash", taiKhoan.MatKhauHash);
            insertAccount.Parameters.AddWithValue("@IsActive", taiKhoan.IsActive);
            insertAccount.Parameters.AddWithValue("@VaiTroId", taiKhoan.VaiTroId);
            var taiKhoanId = Convert.ToInt32(await insertAccount.ExecuteScalarAsync());

            await using var insertEmployee = new SqlCommand(insertEmployeeSql, connection, transaction);
            insertEmployee.Parameters.AddWithValue("@MaNhanVien", nhanVien.MaNhanVien);
            insertEmployee.Parameters.AddWithValue("@HoTen", nhanVien.HoTen);
            insertEmployee.Parameters.AddWithValue("@ChucVu", (object?)nhanVien.ChucVu ?? DBNull.Value);
            insertEmployee.Parameters.AddWithValue("@SoDienThoai", (object?)nhanVien.SoDienThoai ?? DBNull.Value);
            insertEmployee.Parameters.AddWithValue("@DiaChi", (object?)nhanVien.DiaChi ?? DBNull.Value);
            insertEmployee.Parameters.AddWithValue("@TaiKhoanId", taiKhoanId);
            await insertEmployee.ExecuteScalarAsync();

            transaction.Commit();
            return taiKhoanId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
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
