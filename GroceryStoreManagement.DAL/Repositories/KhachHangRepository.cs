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
}
