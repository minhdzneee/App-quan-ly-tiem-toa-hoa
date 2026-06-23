using GroceryStoreManagement.DAL.Database;
using GroceryStoreManagement.Models;
using Microsoft.Data.SqlClient;

namespace GroceryStoreManagement.DAL.Repositories;

public class KiemKeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public KiemKeRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateAsync(PhieuKiemKe phieuKiemKe)
    {
        const string insertPhieuSql = """
            INSERT INTO PhieuKiemKe(MaPhieuKiemKe, NgayKiemKe, NhanVienId, GhiChu)
            OUTPUT INSERTED.Id
            VALUES (@MaPhieuKiemKe, @NgayKiemKe, @NhanVienId, @GhiChu);
            """;

        const string insertDetailSql = """
            INSERT INTO ChiTietKiemKe(PhieuKiemKeId, SanPhamId, SoLuongHeThong, SoLuongThucTe, ChenhLech)
            VALUES (@PhieuKiemKeId, @SanPhamId, @SoLuongHeThong, @SoLuongThucTe, @ChenhLech);
            """;

        const string updateStockSql = """
            UPDATE SanPham
            SET SoLuongTon = @SoLuongThucTe
            WHERE Id = @SanPhamId;
            """;

        const string insertHistorySql = """
            INSERT INTO LichSuTonKho(SanPhamId, ThoiGian, LoaiBienDong, SoLuongThayDoi, SoLuongSauBienDong, LyDo)
            VALUES (@SanPhamId, SYSDATETIME(), N'KiemKe', @SoLuongThayDoi, @SoLuongSauBienDong, @LyDo);
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            await using var insertPhieu = new SqlCommand(insertPhieuSql, connection, transaction);
            insertPhieu.Parameters.AddWithValue("@MaPhieuKiemKe", phieuKiemKe.MaPhieuKiemKe);
            insertPhieu.Parameters.AddWithValue("@NgayKiemKe", phieuKiemKe.NgayKiemKe);
            insertPhieu.Parameters.AddWithValue("@NhanVienId", phieuKiemKe.NhanVienId);
            insertPhieu.Parameters.AddWithValue("@GhiChu", (object?)phieuKiemKe.GhiChu ?? DBNull.Value);
            var phieuKiemKeId = Convert.ToInt32(await insertPhieu.ExecuteScalarAsync());

            foreach (var item in phieuKiemKe.ChiTietKiemKes)
            {
                item.TinhChenhLech();

                await using var insertDetail = new SqlCommand(insertDetailSql, connection, transaction);
                insertDetail.Parameters.AddWithValue("@PhieuKiemKeId", phieuKiemKeId);
                insertDetail.Parameters.AddWithValue("@SanPhamId", item.SanPhamId);
                insertDetail.Parameters.AddWithValue("@SoLuongHeThong", item.SoLuongHeThong);
                insertDetail.Parameters.AddWithValue("@SoLuongThucTe", item.SoLuongThucTe);
                insertDetail.Parameters.AddWithValue("@ChenhLech", item.ChenhLech);
                await insertDetail.ExecuteNonQueryAsync();

                await using var updateStock = new SqlCommand(updateStockSql, connection, transaction);
                updateStock.Parameters.AddWithValue("@SanPhamId", item.SanPhamId);
                updateStock.Parameters.AddWithValue("@SoLuongThucTe", item.SoLuongThucTe);
                await updateStock.ExecuteNonQueryAsync();

                await using var insertHistory = new SqlCommand(insertHistorySql, connection, transaction);
                insertHistory.Parameters.AddWithValue("@SanPhamId", item.SanPhamId);
                insertHistory.Parameters.AddWithValue("@SoLuongThayDoi", item.ChenhLech);
                insertHistory.Parameters.AddWithValue("@SoLuongSauBienDong", item.SoLuongThucTe);
                insertHistory.Parameters.AddWithValue("@LyDo", $"Kiem ke {phieuKiemKe.MaPhieuKiemKe}");
                await insertHistory.ExecuteNonQueryAsync();
            }

            transaction.Commit();
            return phieuKiemKeId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
