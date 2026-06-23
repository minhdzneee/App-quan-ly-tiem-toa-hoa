using GroceryStoreManagement.DAL.Database;
using GroceryStoreManagement.Models;
using Microsoft.Data.SqlClient;

namespace GroceryStoreManagement.DAL.Repositories;

public class PhieuNhapRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PhieuNhapRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateAsync(PhieuNhap phieuNhap)
    {
        const string insertPhieuNhapSql = """
            INSERT INTO PhieuNhap(MaPhieuNhap, NgayNhap, NhaCungCapId, NhanVienId, TongTien, GhiChu)
            OUTPUT INSERTED.Id
            VALUES (@MaPhieuNhap, @NgayNhap, @NhaCungCapId, @NhanVienId, @TongTien, @GhiChu);
            """;

        const string insertDetailSql = """
            INSERT INTO ChiTietPhieuNhap(PhieuNhapId, SanPhamId, SoLuong, DonGiaNhap, ThanhTien)
            VALUES (@PhieuNhapId, @SanPhamId, @SoLuong, @DonGiaNhap, @ThanhTien);
            """;

        const string updateStockSql = """
            UPDATE SanPham
            SET SoLuongTon = SoLuongTon + @SoLuong,
                GiaNhap = @DonGiaNhap
            WHERE Id = @SanPhamId;
            SELECT SoLuongTon FROM SanPham WHERE Id = @SanPhamId;
            """;

        const string insertHistorySql = """
            INSERT INTO LichSuTonKho(SanPhamId, ThoiGian, LoaiBienDong, SoLuongThayDoi, SoLuongSauBienDong, LyDo)
            VALUES (@SanPhamId, SYSDATETIME(), N'NhapHang', @SoLuongThayDoi, @SoLuongSauBienDong, @LyDo);
            """;

        phieuNhap.LapPhieuNhap();

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            await using var insertPhieuNhap = new SqlCommand(insertPhieuNhapSql, connection, transaction);
            insertPhieuNhap.Parameters.AddWithValue("@MaPhieuNhap", phieuNhap.MaPhieuNhap);
            insertPhieuNhap.Parameters.AddWithValue("@NgayNhap", phieuNhap.NgayNhap);
            insertPhieuNhap.Parameters.AddWithValue("@NhaCungCapId", phieuNhap.NhaCungCapId);
            insertPhieuNhap.Parameters.AddWithValue("@NhanVienId", phieuNhap.NhanVienId);
            insertPhieuNhap.Parameters.AddWithValue("@TongTien", phieuNhap.TongTien);
            insertPhieuNhap.Parameters.AddWithValue("@GhiChu", (object?)phieuNhap.GhiChu ?? DBNull.Value);

            var phieuNhapId = Convert.ToInt32(await insertPhieuNhap.ExecuteScalarAsync());

            foreach (var item in phieuNhap.ChiTietPhieuNhaps)
            {
                item.TinhThanhTien();

                await using var insertDetail = new SqlCommand(insertDetailSql, connection, transaction);
                insertDetail.Parameters.AddWithValue("@PhieuNhapId", phieuNhapId);
                insertDetail.Parameters.AddWithValue("@SanPhamId", item.SanPhamId);
                insertDetail.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                insertDetail.Parameters.AddWithValue("@DonGiaNhap", item.DonGiaNhap);
                insertDetail.Parameters.AddWithValue("@ThanhTien", item.ThanhTien);
                await insertDetail.ExecuteNonQueryAsync();

                await using var updateStock = new SqlCommand(updateStockSql, connection, transaction);
                updateStock.Parameters.AddWithValue("@SanPhamId", item.SanPhamId);
                updateStock.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                updateStock.Parameters.AddWithValue("@DonGiaNhap", item.DonGiaNhap);
                var soLuongSauBienDong = Convert.ToInt32(await updateStock.ExecuteScalarAsync());

                await using var insertHistory = new SqlCommand(insertHistorySql, connection, transaction);
                insertHistory.Parameters.AddWithValue("@SanPhamId", item.SanPhamId);
                insertHistory.Parameters.AddWithValue("@SoLuongThayDoi", item.SoLuong);
                insertHistory.Parameters.AddWithValue("@SoLuongSauBienDong", soLuongSauBienDong);
                insertHistory.Parameters.AddWithValue("@LyDo", $"Phieu nhap {phieuNhap.MaPhieuNhap}");
                await insertHistory.ExecuteNonQueryAsync();
            }

            transaction.Commit();
            return phieuNhapId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<List<PhieuNhap>> GetRecentAsync(int top = 50)
    {
        const string sql = """
            SELECT TOP (@Top) pn.Id, pn.MaPhieuNhap, pn.NgayNhap, pn.NhaCungCapId, ncc.TenNhaCungCap,
                   pn.NhanVienId, nv.HoTen AS TenNhanVien, pn.TongTien, pn.GhiChu
            FROM PhieuNhap pn
            INNER JOIN NhaCungCap ncc ON ncc.Id = pn.NhaCungCapId
            INNER JOIN NhanVien nv ON nv.Id = pn.NhanVienId
            ORDER BY pn.NgayNhap DESC;
            """;

        var result = new List<PhieuNhap>();
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Top", top);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new PhieuNhap
            {
                Id = reader.GetInt32Value("Id"),
                MaPhieuNhap = reader.GetString(reader.GetOrdinal("MaPhieuNhap")),
                NgayNhap = reader.GetDateTimeValue("NgayNhap"),
                NhaCungCapId = reader.GetInt32Value("NhaCungCapId"),
                TenNhaCungCap = reader.GetNullableString("TenNhaCungCap"),
                NhanVienId = reader.GetInt32Value("NhanVienId"),
                TenNhanVien = reader.GetNullableString("TenNhanVien"),
                TongTien = reader.GetDecimalValue("TongTien"),
                GhiChu = reader.GetNullableString("GhiChu")
            });
        }

        return result;
    }
}
