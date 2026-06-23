using GroceryStoreManagement.DAL.Database;
using GroceryStoreManagement.Models;
using Microsoft.Data.SqlClient;

namespace GroceryStoreManagement.DAL.Repositories;

public class SanPhamRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SanPhamRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<SanPham>> SearchAsync(string? keyword = null, int? danhMucId = null)
    {
        const string sql = """
            SELECT sp.Id, sp.MaSanPham, sp.TenSanPham, sp.DanhMucId, dm.TenDanhMuc,
                   sp.DonViTinhId, dvt.TenDonVi, sp.GiaNhap, sp.GiaBan, sp.SoLuongTon,
                   sp.TonToiThieu, sp.HanSuDung, sp.MaVach, sp.TrangThai
            FROM SanPham sp
            INNER JOIN DanhMuc dm ON dm.Id = sp.DanhMucId
            INNER JOIN DonViTinh dvt ON dvt.Id = sp.DonViTinhId
            WHERE (@Keyword IS NULL
                   OR sp.MaSanPham LIKE '%' + @Keyword + '%'
                   OR sp.TenSanPham LIKE '%' + @Keyword + '%'
                   OR sp.MaVach LIKE '%' + @Keyword + '%')
              AND (@DanhMucId IS NULL OR sp.DanhMucId = @DanhMucId)
            ORDER BY sp.TenSanPham;
            """;

        var result = new List<SanPham>();
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Keyword", string.IsNullOrWhiteSpace(keyword) ? DBNull.Value : keyword);
        command.Parameters.AddWithValue("@DanhMucId", danhMucId.HasValue ? danhMucId.Value : DBNull.Value);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(MapSanPham(reader));
        }

        return result;
    }

    public async Task<SanPham?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT sp.Id, sp.MaSanPham, sp.TenSanPham, sp.DanhMucId, dm.TenDanhMuc,
                   sp.DonViTinhId, dvt.TenDonVi, sp.GiaNhap, sp.GiaBan, sp.SoLuongTon,
                   sp.TonToiThieu, sp.HanSuDung, sp.MaVach, sp.TrangThai
            FROM SanPham sp
            INNER JOIN DanhMuc dm ON dm.Id = sp.DanhMucId
            INNER JOIN DonViTinh dvt ON dvt.Id = sp.DonViTinhId
            WHERE sp.Id = @Id;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapSanPham(reader) : null;
    }

    public async Task<SanPham?> GetByCodeOrBarcodeAsync(string keyword)
    {
        const string sql = """
            SELECT TOP 1 sp.Id, sp.MaSanPham, sp.TenSanPham, sp.DanhMucId, dm.TenDanhMuc,
                   sp.DonViTinhId, dvt.TenDonVi, sp.GiaNhap, sp.GiaBan, sp.SoLuongTon,
                   sp.TonToiThieu, sp.HanSuDung, sp.MaVach, sp.TrangThai
            FROM SanPham sp
            INNER JOIN DanhMuc dm ON dm.Id = sp.DanhMucId
            INNER JOIN DonViTinh dvt ON dvt.Id = sp.DonViTinhId
            WHERE sp.MaSanPham = @Keyword OR sp.MaVach = @Keyword OR sp.TenSanPham LIKE '%' + @Keyword + '%'
            ORDER BY sp.TenSanPham;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Keyword", keyword);

        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapSanPham(reader) : null;
    }

    public async Task<int> AddAsync(SanPham sanPham)
    {
        const string sql = """
            INSERT INTO SanPham(MaSanPham, TenSanPham, DanhMucId, DonViTinhId, GiaNhap, GiaBan,
                                SoLuongTon, TonToiThieu, HanSuDung, MaVach, TrangThai)
            OUTPUT INSERTED.Id
            VALUES (@MaSanPham, @TenSanPham, @DanhMucId, @DonViTinhId, @GiaNhap, @GiaBan,
                    @SoLuongTon, @TonToiThieu, @HanSuDung, @MaVach, @TrangThai);
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        AddSanPhamParameters(command, sanPham);
        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    public async Task UpdateAsync(SanPham sanPham)
    {
        const string sql = """
            UPDATE SanPham
            SET MaSanPham = @MaSanPham,
                TenSanPham = @TenSanPham,
                DanhMucId = @DanhMucId,
                DonViTinhId = @DonViTinhId,
                GiaNhap = @GiaNhap,
                GiaBan = @GiaBan,
                SoLuongTon = @SoLuongTon,
                TonToiThieu = @TonToiThieu,
                HanSuDung = @HanSuDung,
                MaVach = @MaVach,
                TrangThai = @TrangThai
            WHERE Id = @Id;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", sanPham.Id);
        AddSanPhamParameters(command, sanPham);
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        const string sql = "DELETE FROM SanPham WHERE Id = @Id;";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<bool> IsDuplicateAsync(string maSanPham, string? maVach, int? excludeId = null)
    {
        const string sql = """
            SELECT COUNT(1)
            FROM SanPham
            WHERE (@ExcludeId IS NULL OR Id <> @ExcludeId)
              AND (MaSanPham = @MaSanPham OR (@MaVach IS NOT NULL AND MaVach = @MaVach));
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@MaSanPham", maSanPham);
        command.Parameters.AddWithValue("@MaVach", string.IsNullOrWhiteSpace(maVach) ? DBNull.Value : maVach);
        command.Parameters.AddWithValue("@ExcludeId", excludeId.HasValue ? excludeId.Value : DBNull.Value);

        return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
    }

    private static void AddSanPhamParameters(SqlCommand command, SanPham sanPham)
    {
        command.Parameters.AddWithValue("@MaSanPham", sanPham.MaSanPham);
        command.Parameters.AddWithValue("@TenSanPham", sanPham.TenSanPham);
        command.Parameters.AddWithValue("@DanhMucId", sanPham.DanhMucId);
        command.Parameters.AddWithValue("@DonViTinhId", sanPham.DonViTinhId);
        command.Parameters.AddWithValue("@GiaNhap", sanPham.GiaNhap);
        command.Parameters.AddWithValue("@GiaBan", sanPham.GiaBan);
        command.Parameters.AddWithValue("@SoLuongTon", sanPham.SoLuongTon);
        command.Parameters.AddWithValue("@TonToiThieu", sanPham.TonToiThieu);
        command.Parameters.AddWithValue("@HanSuDung", (object?)sanPham.HanSuDung ?? DBNull.Value);
        command.Parameters.AddWithValue("@MaVach", string.IsNullOrWhiteSpace(sanPham.MaVach) ? DBNull.Value : sanPham.MaVach);
        command.Parameters.AddWithValue("@TrangThai", sanPham.TrangThai);
    }

    private static SanPham MapSanPham(SqlDataReader reader)
    {
        return new SanPham
        {
            Id = reader.GetInt32Value("Id"),
            MaSanPham = reader.GetString(reader.GetOrdinal("MaSanPham")),
            TenSanPham = reader.GetString(reader.GetOrdinal("TenSanPham")),
            DanhMucId = reader.GetInt32Value("DanhMucId"),
            TenDanhMuc = reader.GetNullableString("TenDanhMuc"),
            DonViTinhId = reader.GetInt32Value("DonViTinhId"),
            TenDonVi = reader.GetNullableString("TenDonVi"),
            GiaNhap = reader.GetDecimalValue("GiaNhap"),
            GiaBan = reader.GetDecimalValue("GiaBan"),
            SoLuongTon = reader.GetInt32Value("SoLuongTon"),
            TonToiThieu = reader.GetInt32Value("TonToiThieu"),
            HanSuDung = reader.GetNullableDateTime("HanSuDung"),
            MaVach = reader.GetNullableString("MaVach"),
            TrangThai = reader.GetString(reader.GetOrdinal("TrangThai"))
        };
    }
}
