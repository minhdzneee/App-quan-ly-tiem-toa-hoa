using GroceryStoreManagement.DAL.Database;
using GroceryStoreManagement.Models;
using Microsoft.Data.SqlClient;

namespace GroceryStoreManagement.DAL.Repositories;

public class DonViTinhRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DonViTinhRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<DonViTinh>> GetAllAsync()
    {
        const string sql = "SELECT Id, TenDonVi FROM DonViTinh ORDER BY TenDonVi;";
        var result = new List<DonViTinh>();

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new DonViTinh
            {
                Id = reader.GetInt32Value("Id"),
                TenDonVi = reader.GetString(reader.GetOrdinal("TenDonVi"))
            });
        }

        return result;
    }
}
