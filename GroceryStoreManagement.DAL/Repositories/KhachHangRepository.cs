using GroceryStoreManagement.DAL.Database;
using GroceryStoreManagement.Models;
using Microsoft.Data.SqlClient;

namespace GroceryStoreManagement.DAL.Repositories;

public class KhachHangRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public KhachHangRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<KhachHang>> SearchAsync(string? keyword)
    {
        const string sql = """
            SELECT Id, MaKhachHang, HoTen, SoDienThoai, DiaChi, DiemTichLuy
            FROM KhachHang
            WHERE @Keyword IS NULL
               OR MaKhachHang LIKE '%' + @Keyword + '%'
               OR HoTen LIKE '%' + @Keyword + '%'
               OR SoDienThoai LIKE '%' + @Keyword + '%'
            ORDER BY HoTen;
            """;

        var result = new List<KhachHang>();
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Keyword", string.IsNullOrWhiteSpace(keyword) ? DBNull.Value : keyword);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new KhachHang
            {
                Id = reader.GetInt32Value("Id"),
                MaKhachHang = reader.GetString(reader.GetOrdinal("MaKhachHang")),
                HoTen = reader.GetString(reader.GetOrdinal("HoTen")),
                SoDienThoai = reader.GetNullableString("SoDienThoai"),
                DiaChi = reader.GetNullableString("DiaChi"),
                DiemTichLuy = reader.GetInt32Value("DiemTichLuy")
            });
        }

        return result;
    }

    public async Task<int> AddAsync(KhachHang khachHang)
    {
        const string sql = """
            INSERT INTO KhachHang(MaKhachHang, HoTen, SoDienThoai, DiaChi, DiemTichLuy)
            OUTPUT INSERTED.Id
            VALUES (@MaKhachHang, @HoTen, @SoDienThoai, @DiaChi, @DiemTichLuy);
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        AddParameters(command, khachHang);
        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    public async Task UpdateAsync(KhachHang khachHang)
    {
        const string sql = """
            UPDATE KhachHang
            SET MaKhachHang = @MaKhachHang,
                HoTen = @HoTen,
                SoDienThoai = @SoDienThoai,
                DiaChi = @DiaChi,
                DiemTichLuy = @DiemTichLuy
            WHERE Id = @Id;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", khachHang.Id);
        AddParameters(command, khachHang);
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        const string sql = """
            IF EXISTS (SELECT 1 FROM HoaDon WHERE KhachHangId = @Id)
                THROW 51000, 'Khong the xoa khach hang da co hoa don.', 1;

            DELETE FROM KhachHang WHERE Id = @Id;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<bool> IsDuplicateAsync(string maKhachHang, int? excludeId = null)
    {
        const string sql = """
            SELECT COUNT(1)
            FROM KhachHang
            WHERE MaKhachHang = @MaKhachHang
              AND (@ExcludeId IS NULL OR Id <> @ExcludeId);
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@MaKhachHang", maKhachHang);
        command.Parameters.AddWithValue("@ExcludeId", excludeId.HasValue ? excludeId.Value : DBNull.Value);
        return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
    }

    private static void AddParameters(SqlCommand command, KhachHang khachHang)
    {
        command.Parameters.AddWithValue("@MaKhachHang", khachHang.MaKhachHang);
        command.Parameters.AddWithValue("@HoTen", khachHang.HoTen);
        command.Parameters.AddWithValue("@SoDienThoai", string.IsNullOrWhiteSpace(khachHang.SoDienThoai) ? DBNull.Value : khachHang.SoDienThoai);
        command.Parameters.AddWithValue("@DiaChi", string.IsNullOrWhiteSpace(khachHang.DiaChi) ? DBNull.Value : khachHang.DiaChi);
        command.Parameters.AddWithValue("@DiemTichLuy", khachHang.DiemTichLuy);
    }
}
