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

    public async Task<int> AddAsync(NhaCungCap nhaCungCap)
    {
        const string sql = """
            INSERT INTO NhaCungCap(MaNhaCungCap, TenNhaCungCap, SoDienThoai, DiaChi, Email)
            OUTPUT INSERTED.Id
            VALUES (@MaNhaCungCap, @TenNhaCungCap, @SoDienThoai, @DiaChi, @Email);
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        AddParameters(command, nhaCungCap);
        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    public async Task UpdateAsync(NhaCungCap nhaCungCap)
    {
        const string sql = """
            UPDATE NhaCungCap
            SET MaNhaCungCap = @MaNhaCungCap,
                TenNhaCungCap = @TenNhaCungCap,
                SoDienThoai = @SoDienThoai,
                DiaChi = @DiaChi,
                Email = @Email
            WHERE Id = @Id;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", nhaCungCap.Id);
        AddParameters(command, nhaCungCap);
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        const string sql = """
            IF EXISTS (SELECT 1 FROM PhieuNhap WHERE NhaCungCapId = @Id)
                THROW 51000, 'Khong the xoa nha cung cap da co phieu nhap.', 1;

            DELETE FROM NhaCungCap WHERE Id = @Id;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<bool> IsDuplicateAsync(string maNhaCungCap, int? excludeId = null)
    {
        const string sql = """
            SELECT COUNT(1)
            FROM NhaCungCap
            WHERE MaNhaCungCap = @MaNhaCungCap
              AND (@ExcludeId IS NULL OR Id <> @ExcludeId);
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@MaNhaCungCap", maNhaCungCap);
        command.Parameters.AddWithValue("@ExcludeId", excludeId.HasValue ? excludeId.Value : DBNull.Value);
        return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
    }

    private static void AddParameters(SqlCommand command, NhaCungCap nhaCungCap)
    {
        command.Parameters.AddWithValue("@MaNhaCungCap", nhaCungCap.MaNhaCungCap);
        command.Parameters.AddWithValue("@TenNhaCungCap", nhaCungCap.TenNhaCungCap);
        command.Parameters.AddWithValue("@SoDienThoai", string.IsNullOrWhiteSpace(nhaCungCap.SoDienThoai) ? DBNull.Value : nhaCungCap.SoDienThoai);
        command.Parameters.AddWithValue("@DiaChi", string.IsNullOrWhiteSpace(nhaCungCap.DiaChi) ? DBNull.Value : nhaCungCap.DiaChi);
        command.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(nhaCungCap.Email) ? DBNull.Value : nhaCungCap.Email);
    }
}
