using GroceryStoreManagement.DAL.Database;
using GroceryStoreManagement.Models;
using Microsoft.Data.SqlClient;

namespace GroceryStoreManagement.DAL.Repositories;

public class HoaDonRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public HoaDonRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateAsync(HoaDon hoaDon)
    {
        const string insertHoaDonSql = """
            INSERT INTO HoaDon(MaHoaDon, NgayLap, NhanVienId, KhachHangId, TongTien, GiamGia, ThanhTien, TrangThai)
            OUTPUT INSERTED.Id
            VALUES (@MaHoaDon, @NgayLap, @NhanVienId, @KhachHangId, @TongTien, @GiamGia, @ThanhTien, @TrangThai);
            """;

        const string checkStockSql = "SELECT SoLuongTon FROM SanPham WITH (UPDLOCK, ROWLOCK) WHERE Id = @SanPhamId;";

        const string insertDetailSql = """
            INSERT INTO ChiTietHoaDon(HoaDonId, SanPhamId, SoLuong, DonGia, ThanhTien)
            VALUES (@HoaDonId, @SanPhamId, @SoLuong, @DonGia, @ThanhTien);
            """;

        const string updateStockSql = """
            UPDATE SanPham
            SET SoLuongTon = SoLuongTon - @SoLuong
            WHERE Id = @SanPhamId;
            SELECT SoLuongTon FROM SanPham WHERE Id = @SanPhamId;
            """;

        const string insertHistorySql = """
            INSERT INTO LichSuTonKho(SanPhamId, ThoiGian, LoaiBienDong, SoLuongThayDoi, SoLuongSauBienDong, LyDo)
            VALUES (@SanPhamId, SYSDATETIME(), N'BanHang', @SoLuongThayDoi, @SoLuongSauBienDong, @LyDo);
            """;

        hoaDon.TinhTongTien();

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            foreach (var item in hoaDon.ChiTietHoaDons)
            {
                await using var checkStock = new SqlCommand(checkStockSql, connection, transaction);
                checkStock.Parameters.AddWithValue("@SanPhamId", item.SanPhamId);
                var currentStock = Convert.ToInt32(await checkStock.ExecuteScalarAsync());
                if (currentStock < item.SoLuong)
                {
                    throw new InvalidOperationException($"Product {item.SanPhamId} does not have enough stock.");
                }
            }

            await using var insertHoaDon = new SqlCommand(insertHoaDonSql, connection, transaction);
            insertHoaDon.Parameters.AddWithValue("@MaHoaDon", hoaDon.MaHoaDon);
            insertHoaDon.Parameters.AddWithValue("@NgayLap", hoaDon.NgayLap);
            insertHoaDon.Parameters.AddWithValue("@NhanVienId", hoaDon.NhanVienId);
            insertHoaDon.Parameters.AddWithValue("@KhachHangId", hoaDon.KhachHangId.HasValue ? hoaDon.KhachHangId.Value : DBNull.Value);
            insertHoaDon.Parameters.AddWithValue("@TongTien", hoaDon.TongTien);
            insertHoaDon.Parameters.AddWithValue("@GiamGia", hoaDon.GiamGia);
            insertHoaDon.Parameters.AddWithValue("@ThanhTien", hoaDon.ThanhTien);
            insertHoaDon.Parameters.AddWithValue("@TrangThai", hoaDon.TrangThai);

            var hoaDonId = Convert.ToInt32(await insertHoaDon.ExecuteScalarAsync());

            foreach (var item in hoaDon.ChiTietHoaDons)
            {
                item.TinhThanhTien();

                await using var insertDetail = new SqlCommand(insertDetailSql, connection, transaction);
                insertDetail.Parameters.AddWithValue("@HoaDonId", hoaDonId);
                insertDetail.Parameters.AddWithValue("@SanPhamId", item.SanPhamId);
                insertDetail.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                insertDetail.Parameters.AddWithValue("@DonGia", item.DonGia);
                insertDetail.Parameters.AddWithValue("@ThanhTien", item.ThanhTien);
                await insertDetail.ExecuteNonQueryAsync();

                await using var updateStock = new SqlCommand(updateStockSql, connection, transaction);
                updateStock.Parameters.AddWithValue("@SanPhamId", item.SanPhamId);
                updateStock.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                var soLuongSauBienDong = Convert.ToInt32(await updateStock.ExecuteScalarAsync());

                await using var insertHistory = new SqlCommand(insertHistorySql, connection, transaction);
                insertHistory.Parameters.AddWithValue("@SanPhamId", item.SanPhamId);
                insertHistory.Parameters.AddWithValue("@SoLuongThayDoi", -item.SoLuong);
                insertHistory.Parameters.AddWithValue("@SoLuongSauBienDong", soLuongSauBienDong);
                insertHistory.Parameters.AddWithValue("@LyDo", $"Hoa don {hoaDon.MaHoaDon}");
                await insertHistory.ExecuteNonQueryAsync();
            }

            transaction.Commit();
            return hoaDonId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<List<HoaDon>> GetRecentAsync(int top = 50)
    {
        const string sql = """
            SELECT TOP (@Top) hd.Id, hd.MaHoaDon, hd.NgayLap, hd.NhanVienId, nv.HoTen AS TenNhanVien,
                   hd.KhachHangId, kh.HoTen AS TenKhachHang, hd.TongTien, hd.GiamGia, hd.ThanhTien, hd.TrangThai
            FROM HoaDon hd
            INNER JOIN NhanVien nv ON nv.Id = hd.NhanVienId
            LEFT JOIN KhachHang kh ON kh.Id = hd.KhachHangId
            ORDER BY hd.NgayLap DESC;
            """;

        var result = new List<HoaDon>();
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Top", top);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new HoaDon
            {
                Id = reader.GetInt32Value("Id"),
                MaHoaDon = reader.GetString(reader.GetOrdinal("MaHoaDon")),
                NgayLap = reader.GetDateTimeValue("NgayLap"),
                NhanVienId = reader.GetInt32Value("NhanVienId"),
                TenNhanVien = reader.GetNullableString("TenNhanVien"),
                KhachHangId = reader.GetNullableInt32("KhachHangId"),
                TenKhachHang = reader.GetNullableString("TenKhachHang"),
                TongTien = reader.GetDecimalValue("TongTien"),
                GiamGia = reader.GetDecimalValue("GiamGia"),
                ThanhTien = reader.GetDecimalValue("ThanhTien"),
                TrangThai = reader.GetString(reader.GetOrdinal("TrangThai"))
            });
        }

        return result;
    }
}
