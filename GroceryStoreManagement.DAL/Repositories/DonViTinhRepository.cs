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

    public async Task<int> AddAsync(DonViTinh donViTinh)
    {
        const string sql = """
            INSERT INTO DonViTinh(TenDonVi)
            OUTPUT INSERTED.Id
            VALUES (@TenDonVi);
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@TenDonVi", donViTinh.TenDonVi);
        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    public async Task UpdateAsync(DonViTinh donViTinh)
    {
        const string sql = "UPDATE DonViTinh SET TenDonVi = @TenDonVi WHERE Id = @Id;";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", donViTinh.Id);
        command.Parameters.AddWithValue("@TenDonVi", donViTinh.TenDonVi);
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        const string sql = """
            IF EXISTS (SELECT 1 FROM SanPham WHERE DonViTinhId = @Id)
                THROW 51000, 'Khong the xoa don vi tinh dang co san pham.', 1;

            DELETE FROM DonViTinh WHERE Id = @Id;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<bool> IsDuplicateAsync(string tenDonVi, int? excludeId = null)
    {
        const string sql = """
            SELECT COUNT(1)
            FROM DonViTinh
            WHERE TenDonVi = @TenDonVi
              AND (@ExcludeId IS NULL OR Id <> @ExcludeId);
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@TenDonVi", tenDonVi);
        command.Parameters.AddWithValue("@ExcludeId", excludeId.HasValue ? excludeId.Value : DBNull.Value);
        return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
    }
}
