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

    public async Task<int> AddAsync(DanhMuc danhMuc)
    {
        const string sql = """
            INSERT INTO DanhMuc(MaDanhMuc, TenDanhMuc, MoTa)
            OUTPUT INSERTED.Id
            VALUES (@MaDanhMuc, @TenDanhMuc, @MoTa);
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        AddParameters(command, danhMuc);
        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    public async Task UpdateAsync(DanhMuc danhMuc)
    {
        const string sql = """
            UPDATE DanhMuc
            SET MaDanhMuc = @MaDanhMuc,
                TenDanhMuc = @TenDanhMuc,
                MoTa = @MoTa
            WHERE Id = @Id;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", danhMuc.Id);
        AddParameters(command, danhMuc);
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        const string sql = """
            IF EXISTS (SELECT 1 FROM SanPham WHERE DanhMucId = @Id)
                THROW 51000, 'Khong the xoa danh muc dang co san pham.', 1;

            DELETE FROM DanhMuc WHERE Id = @Id;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<bool> IsDuplicateAsync(string maDanhMuc, int? excludeId = null)
    {
        const string sql = """
            SELECT COUNT(1)
            FROM DanhMuc
            WHERE MaDanhMuc = @MaDanhMuc
              AND (@ExcludeId IS NULL OR Id <> @ExcludeId);
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@MaDanhMuc", maDanhMuc);
        command.Parameters.AddWithValue("@ExcludeId", excludeId.HasValue ? excludeId.Value : DBNull.Value);
        return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
    }

    private static void AddParameters(SqlCommand command, DanhMuc danhMuc)
    {
        command.Parameters.AddWithValue("@MaDanhMuc", danhMuc.MaDanhMuc);
        command.Parameters.AddWithValue("@TenDanhMuc", danhMuc.TenDanhMuc);
        command.Parameters.AddWithValue("@MoTa", string.IsNullOrWhiteSpace(danhMuc.MoTa) ? DBNull.Value : danhMuc.MoTa);
    }
}
