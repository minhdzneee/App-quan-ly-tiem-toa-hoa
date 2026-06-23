using GroceryStoreManagement.DAL.Database;
using GroceryStoreManagement.Models;
using Microsoft.Data.SqlClient;

namespace GroceryStoreManagement.DAL.Repositories;

public class BaoCaoRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public BaoCaoRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        const string sql = """
            SELECT
                ISNULL((SELECT SUM(ThanhTien) FROM HoaDon WHERE CAST(NgayLap AS date) = CAST(GETDATE() AS date)), 0) AS DoanhThuHomNay,
                (SELECT COUNT(*) FROM HoaDon WHERE CAST(NgayLap AS date) = CAST(GETDATE() AS date)) AS TongHoaDonHomNay,
                (SELECT COUNT(*) FROM PhieuNhap WHERE YEAR(NgayNhap) = YEAR(GETDATE()) AND MONTH(NgayNhap) = MONTH(GETDATE())) AS TongPhieuNhapThang,
                (SELECT COUNT(*) FROM SanPham WHERE SoLuongTon <= TonToiThieu) AS SanPhamTonKhoThap,
                (SELECT COUNT(*) FROM SanPham WHERE HanSuDung IS NOT NULL AND HanSuDung BETWEEN CAST(GETDATE() AS date) AND DATEADD(day, 30, CAST(GETDATE() AS date))) AS SanPhamSapHetHan;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return new DashboardStats();
        }

        return new DashboardStats
        {
            DoanhThuHomNay = reader.GetDecimalValue("DoanhThuHomNay"),
            TongHoaDonHomNay = reader.GetInt32Value("TongHoaDonHomNay"),
            TongPhieuNhapThang = reader.GetInt32Value("TongPhieuNhapThang"),
            SanPhamTonKhoThap = reader.GetInt32Value("SanPhamTonKhoThap"),
            SanPhamSapHetHan = reader.GetInt32Value("SanPhamSapHetHan")
        };
    }

    public async Task<List<SanPhamBanChay>> GetTopSanPhamBanChayAsync(DateTime fromDate, DateTime toDate, int top = 10)
    {
        const string sql = """
            SELECT TOP (@Top) sp.Id AS SanPhamId, sp.MaSanPham, sp.TenSanPham,
                   SUM(ct.SoLuong) AS SoLuongBan,
                   SUM(ct.ThanhTien) AS DoanhThu
            FROM ChiTietHoaDon ct
            INNER JOIN HoaDon hd ON hd.Id = ct.HoaDonId
            INNER JOIN SanPham sp ON sp.Id = ct.SanPhamId
            WHERE hd.NgayLap >= @FromDate AND hd.NgayLap < DATEADD(day, 1, @ToDate)
            GROUP BY sp.Id, sp.MaSanPham, sp.TenSanPham
            ORDER BY SUM(ct.SoLuong) DESC;
            """;

        var result = new List<SanPhamBanChay>();
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Top", top);
        command.Parameters.AddWithValue("@FromDate", fromDate.Date);
        command.Parameters.AddWithValue("@ToDate", toDate.Date);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new SanPhamBanChay
            {
                SanPhamId = reader.GetInt32Value("SanPhamId"),
                MaSanPham = reader.GetString(reader.GetOrdinal("MaSanPham")),
                TenSanPham = reader.GetString(reader.GetOrdinal("TenSanPham")),
                SoLuongBan = reader.GetInt32Value("SoLuongBan"),
                DoanhThu = reader.GetDecimalValue("DoanhThu")
            });
        }

        return result;
    }

    public async Task<List<SanPham>> GetSanPhamTonKhoThapAsync()
    {
        const string sql = """
            SELECT sp.Id, sp.MaSanPham, sp.TenSanPham, sp.DanhMucId, dm.TenDanhMuc,
                   sp.DonViTinhId, dvt.TenDonVi, sp.GiaNhap, sp.GiaBan, sp.SoLuongTon,
                   sp.TonToiThieu, sp.HanSuDung, sp.MaVach, sp.TrangThai
            FROM SanPham sp
            INNER JOIN DanhMuc dm ON dm.Id = sp.DanhMucId
            INNER JOIN DonViTinh dvt ON dvt.Id = sp.DonViTinhId
            WHERE sp.SoLuongTon <= sp.TonToiThieu
            ORDER BY sp.SoLuongTon ASC;
            """;

        return await ReadSanPhamListAsync(sql);
    }

    public async Task<List<SanPham>> GetSanPhamSapHetHanAsync(int soNgay = 30)
    {
        const string sql = """
            SELECT sp.Id, sp.MaSanPham, sp.TenSanPham, sp.DanhMucId, dm.TenDanhMuc,
                   sp.DonViTinhId, dvt.TenDonVi, sp.GiaNhap, sp.GiaBan, sp.SoLuongTon,
                   sp.TonToiThieu, sp.HanSuDung, sp.MaVach, sp.TrangThai
            FROM SanPham sp
            INNER JOIN DanhMuc dm ON dm.Id = sp.DanhMucId
            INNER JOIN DonViTinh dvt ON dvt.Id = sp.DonViTinhId
            WHERE sp.HanSuDung IS NOT NULL
              AND sp.HanSuDung BETWEEN CAST(GETDATE() AS date) AND DATEADD(day, @SoNgay, CAST(GETDATE() AS date))
            ORDER BY sp.HanSuDung ASC;
            """;

        var result = new List<SanPham>();
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@SoNgay", soNgay);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(MapSanPham(reader));
        }

        return result;
    }

    private async Task<List<SanPham>> ReadSanPhamListAsync(string sql)
    {
        var result = new List<SanPham>();
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(MapSanPham(reader));
        }

        return result;
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
