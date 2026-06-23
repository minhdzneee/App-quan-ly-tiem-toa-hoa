using GroceryStoreManagement.DAL.Database;
using GroceryStoreManagement.Models;
using Microsoft.Data.SqlClient;

namespace GroceryStoreManagement.DAL.Repositories;

public class NhaCungCapRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public NhaCungCapRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<NhaCungCap>> GetAllAsync()
    {
        const string sql = """
            SELECT Id, MaNhaCungCap, TenNhaCungCap, SoDienThoai, DiaChi, Email
            FROM NhaCungCap
            ORDER BY TenNhaCungCap;
            """;
        var result = new List<NhaCungCap>();

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new NhaCungCap
            {
                Id = reader.GetInt32Value("Id"),
                MaNhaCungCap = reader.GetString(reader.GetOrdinal("MaNhaCungCap")),
                TenNhaCungCap = reader.GetString(reader.GetOrdinal("TenNhaCungCap")),
                SoDienThoai = reader.GetNullableString("SoDienThoai"),
                DiaChi = reader.GetNullableString("DiaChi"),
                Email = reader.GetNullableString("Email")
            });
        }

        return result;
    }
}
