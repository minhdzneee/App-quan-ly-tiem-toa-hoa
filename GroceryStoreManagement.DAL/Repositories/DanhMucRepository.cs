using GroceryStoreManagement.DAL.Database;
using GroceryStoreManagement.Models;
using Microsoft.Data.SqlClient;

namespace GroceryStoreManagement.DAL.Repositories;

public class DanhMucRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DanhMucRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<DanhMuc>> GetAllAsync()
    {
        const string sql = "SELECT Id, MaDanhMuc, TenDanhMuc, MoTa FROM DanhMuc ORDER BY TenDanhMuc;";
        var result = new List<DanhMuc>();

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new DanhMuc
            {
                Id = reader.GetInt32Value("Id"),
                MaDanhMuc = reader.GetString(reader.GetOrdinal("MaDanhMuc")),
                TenDanhMuc = reader.GetString(reader.GetOrdinal("TenDanhMuc")),
                MoTa = reader.GetNullableString("MoTa")
            });
        }

        return result;
    }
}
