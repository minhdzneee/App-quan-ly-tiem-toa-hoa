using GroceryStoreManagement.BLL.Services;
using GroceryStoreManagement.DAL.Database;
using GroceryStoreManagement.DAL.Repositories;

namespace GroceryStoreManagement.Presentation;

public sealed class AppServices
{
    public AppServices(string connectionString)
    {
        var connectionFactory = new DbConnectionFactory(connectionString);

        var taiKhoanRepository = new TaiKhoanRepository(connectionFactory);
        var sanPhamRepository = new SanPhamRepository(connectionFactory);
        var danhMucRepository = new DanhMucRepository(connectionFactory);
        var donViTinhRepository = new DonViTinhRepository(connectionFactory);
        var nhaCungCapRepository = new NhaCungCapRepository(connectionFactory);
        var nhanVienRepository = new NhanVienRepository(connectionFactory);
        var khachHangRepository = new KhachHangRepository(connectionFactory);
        var phieuNhapRepository = new PhieuNhapRepository(connectionFactory);
        var hoaDonRepository = new HoaDonRepository(connectionFactory);
        var kiemKeRepository = new KiemKeRepository(connectionFactory);
        var baoCaoRepository = new BaoCaoRepository(connectionFactory);

        AuthService = new AuthService(taiKhoanRepository);
        SanPhamService = new SanPhamService(sanPhamRepository);
        DanhMucService = new DanhMucService(danhMucRepository);
        DonViTinhService = new DonViTinhService(donViTinhRepository);
        NhaCungCapService = new NhaCungCapService(nhaCungCapRepository);
        NhanVienService = new NhanVienService(nhanVienRepository);
        KhachHangService = new KhachHangService(khachHangRepository);
        NhapHangService = new NhapHangService(phieuNhapRepository);
        BanHangService = new BanHangService(hoaDonRepository, sanPhamRepository);
        KiemKeService = new KiemKeService(kiemKeRepository);
        BaoCaoService = new BaoCaoService(baoCaoRepository);
    }

    public AuthService AuthService { get; }
    public SanPhamService SanPhamService { get; }
    public DanhMucService DanhMucService { get; }
    public DonViTinhService DonViTinhService { get; }
    public NhaCungCapService NhaCungCapService { get; }
    public NhanVienService NhanVienService { get; }
    public KhachHangService KhachHangService { get; }
    public NhapHangService NhapHangService { get; }
    public BanHangService BanHangService { get; }
    public KiemKeService KiemKeService { get; }
    public BaoCaoService BaoCaoService { get; }
}
