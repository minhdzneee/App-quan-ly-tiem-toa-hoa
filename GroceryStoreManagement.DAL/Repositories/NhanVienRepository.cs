using GroceryStoreManagement.DAL.Database;
using GroceryStoreManagement.Models;
using Microsoft.Data.SqlClient;

namespace GroceryStoreManagement.DAL.Repositories;

public class NhanVienRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public NhanVienRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<NhanVien>> GetAllAsync()
    {
        const string sql = """
            SELECT Id, MaNhanVien, HoTen, NgaySinh, GioiTinh, ChucVu, SoDienThoai, DiaChi, TaiKhoanId
            FROM NhanVien
            ORDER BY HoTen;
            """;

        var result = new List<NhanVien>();
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new NhanVien
            {
                Id = reader.GetInt32Value("Id"),
                MaNhanVien = reader.GetString(reader.GetOrdinal("MaNhanVien")),
                HoTen = reader.GetString(reader.GetOrdinal("HoTen")),
                NgaySinh = reader.GetNullableDateTime("NgaySinh"),
                GioiTinh = reader.GetNullableString("GioiTinh"),
                ChucVu = reader.GetNullableString("ChucVu"),
                SoDienThoai = reader.GetNullableString("SoDienThoai"),
                DiaChi = reader.GetNullableString("DiaChi"),
                TaiKhoanId = reader.GetNullableInt32("TaiKhoanId")
            });
        }

        return result;
    }
}
