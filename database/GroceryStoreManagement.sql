IF DB_ID(N'GroceryStoreManagement') IS NULL
BEGIN
    CREATE DATABASE GroceryStoreManagement;
END
GO

USE GroceryStoreManagement;
GO

DROP TABLE IF EXISTS LichSuTruyCap;
DROP TABLE IF EXISTS LichSuTonKho;
DROP TABLE IF EXISTS ChiTietKiemKe;
DROP TABLE IF EXISTS PhieuKiemKe;
DROP TABLE IF EXISTS ChiTietHoaDon;
DROP TABLE IF EXISTS HoaDon;
DROP TABLE IF EXISTS KhachHang;
DROP TABLE IF EXISTS ChiTietPhieuNhap;
DROP TABLE IF EXISTS PhieuNhap;
DROP TABLE IF EXISTS SanPham;
DROP TABLE IF EXISTS NhaCungCap;
DROP TABLE IF EXISTS DonViTinh;
DROP TABLE IF EXISTS DanhMuc;
DROP TABLE IF EXISTS NhanVien;
DROP TABLE IF EXISTS TaiKhoan;
DROP TABLE IF EXISTS VaiTro;
GO

CREATE TABLE VaiTro
(
    Id INT IDENTITY(1,1) CONSTRAINT PK_VaiTro PRIMARY KEY,
    TenVaiTro NVARCHAR(50) NOT NULL CONSTRAINT UQ_VaiTro_TenVaiTro UNIQUE,
    MoTa NVARCHAR(255) NULL
);

CREATE TABLE TaiKhoan
(
    Id INT IDENTITY(1,1) CONSTRAINT PK_TaiKhoan PRIMARY KEY,
    TenDangNhap NVARCHAR(50) NOT NULL CONSTRAINT UQ_TaiKhoan_TenDangNhap UNIQUE,
    MatKhauHash VARCHAR(128) NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_TaiKhoan_IsActive DEFAULT (1),
    VaiTroId INT NOT NULL,
    CONSTRAINT FK_TaiKhoan_VaiTro FOREIGN KEY (VaiTroId) REFERENCES VaiTro(Id)
);

CREATE TABLE NhanVien
(
    Id INT IDENTITY(1,1) CONSTRAINT PK_NhanVien PRIMARY KEY,
    MaNhanVien NVARCHAR(30) NOT NULL CONSTRAINT UQ_NhanVien_MaNhanVien UNIQUE,
    HoTen NVARCHAR(120) NOT NULL,
    NgaySinh DATE NULL,
    GioiTinh NVARCHAR(10) NULL,
    ChucVu NVARCHAR(80) NULL,
    SoDienThoai NVARCHAR(20) NULL,
    DiaChi NVARCHAR(255) NULL,
    TaiKhoanId INT NULL CONSTRAINT UQ_NhanVien_TaiKhoan UNIQUE,
    CONSTRAINT FK_NhanVien_TaiKhoan FOREIGN KEY (TaiKhoanId) REFERENCES TaiKhoan(Id)
);

CREATE TABLE DanhMuc
(
    Id INT IDENTITY(1,1) CONSTRAINT PK_DanhMuc PRIMARY KEY,
    MaDanhMuc NVARCHAR(30) NOT NULL CONSTRAINT UQ_DanhMuc_MaDanhMuc UNIQUE,
    TenDanhMuc NVARCHAR(120) NOT NULL,
    MoTa NVARCHAR(255) NULL
);

CREATE TABLE DonViTinh
(
    Id INT IDENTITY(1,1) CONSTRAINT PK_DonViTinh PRIMARY KEY,
    TenDonVi NVARCHAR(50) NOT NULL CONSTRAINT UQ_DonViTinh_TenDonVi UNIQUE
);

CREATE TABLE NhaCungCap
(
    Id INT IDENTITY(1,1) CONSTRAINT PK_NhaCungCap PRIMARY KEY,
    MaNhaCungCap NVARCHAR(30) NOT NULL CONSTRAINT UQ_NhaCungCap_MaNhaCungCap UNIQUE,
    TenNhaCungCap NVARCHAR(150) NOT NULL,
    SoDienThoai NVARCHAR(20) NULL,
    DiaChi NVARCHAR(255) NULL,
    Email NVARCHAR(120) NULL
);

CREATE TABLE SanPham
(
    Id INT IDENTITY(1,1) CONSTRAINT PK_SanPham PRIMARY KEY,
    MaSanPham NVARCHAR(30) NOT NULL CONSTRAINT UQ_SanPham_MaSanPham UNIQUE,
    TenSanPham NVARCHAR(150) NOT NULL,
    DanhMucId INT NOT NULL,
    DonViTinhId INT NOT NULL,
    GiaNhap DECIMAL(18,2) NOT NULL CONSTRAINT DF_SanPham_GiaNhap DEFAULT (0),
    GiaBan DECIMAL(18,2) NOT NULL CONSTRAINT DF_SanPham_GiaBan DEFAULT (0),
    SoLuongTon INT NOT NULL CONSTRAINT DF_SanPham_SoLuongTon DEFAULT (0),
    TonToiThieu INT NOT NULL CONSTRAINT DF_SanPham_TonToiThieu DEFAULT (0),
    HanSuDung DATE NULL,
    MaVach NVARCHAR(60) NULL,
    TrangThai NVARCHAR(30) NOT NULL CONSTRAINT DF_SanPham_TrangThai DEFAULT (N'DangBan'),
    CONSTRAINT FK_SanPham_DanhMuc FOREIGN KEY (DanhMucId) REFERENCES DanhMuc(Id),
    CONSTRAINT FK_SanPham_DonViTinh FOREIGN KEY (DonViTinhId) REFERENCES DonViTinh(Id),
    CONSTRAINT CK_SanPham_GiaNhap CHECK (GiaNhap >= 0),
    CONSTRAINT CK_SanPham_GiaBan CHECK (GiaBan >= 0),
    CONSTRAINT CK_SanPham_SoLuongTon CHECK (SoLuongTon >= 0),
    CONSTRAINT CK_SanPham_TonToiThieu CHECK (TonToiThieu >= 0)
);
GO

CREATE UNIQUE INDEX UX_SanPham_MaVach_NotNull ON SanPham(MaVach) WHERE MaVach IS NOT NULL;
GO

CREATE TABLE PhieuNhap
(
    Id INT IDENTITY(1,1) CONSTRAINT PK_PhieuNhap PRIMARY KEY,
    MaPhieuNhap NVARCHAR(30) NOT NULL CONSTRAINT UQ_PhieuNhap_MaPhieuNhap UNIQUE,
    NgayNhap DATETIME2 NOT NULL CONSTRAINT DF_PhieuNhap_NgayNhap DEFAULT (SYSDATETIME()),
    NhaCungCapId INT NOT NULL,
    NhanVienId INT NOT NULL,
    TongTien DECIMAL(18,2) NOT NULL CONSTRAINT DF_PhieuNhap_TongTien DEFAULT (0),
    GhiChu NVARCHAR(255) NULL,
    CONSTRAINT FK_PhieuNhap_NhaCungCap FOREIGN KEY (NhaCungCapId) REFERENCES NhaCungCap(Id),
    CONSTRAINT FK_PhieuNhap_NhanVien FOREIGN KEY (NhanVienId) REFERENCES NhanVien(Id),
    CONSTRAINT CK_PhieuNhap_TongTien CHECK (TongTien >= 0)
);

CREATE TABLE ChiTietPhieuNhap
(
    Id INT IDENTITY(1,1) CONSTRAINT PK_ChiTietPhieuNhap PRIMARY KEY,
    PhieuNhapId INT NOT NULL,
    SanPhamId INT NOT NULL,
    SoLuong INT NOT NULL,
    DonGiaNhap DECIMAL(18,2) NOT NULL,
    ThanhTien DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_ChiTietPhieuNhap_PhieuNhap FOREIGN KEY (PhieuNhapId) REFERENCES PhieuNhap(Id),
    CONSTRAINT FK_ChiTietPhieuNhap_SanPham FOREIGN KEY (SanPhamId) REFERENCES SanPham(Id),
    CONSTRAINT CK_ChiTietPhieuNhap_SoLuong CHECK (SoLuong > 0),
    CONSTRAINT CK_ChiTietPhieuNhap_DonGiaNhap CHECK (DonGiaNhap >= 0),
    CONSTRAINT CK_ChiTietPhieuNhap_ThanhTien CHECK (ThanhTien >= 0)
);

CREATE TABLE KhachHang
(
    Id INT IDENTITY(1,1) CONSTRAINT PK_KhachHang PRIMARY KEY,
    MaKhachHang NVARCHAR(30) NOT NULL CONSTRAINT UQ_KhachHang_MaKhachHang UNIQUE,
    HoTen NVARCHAR(120) NOT NULL,
    SoDienThoai NVARCHAR(20) NULL,
    DiaChi NVARCHAR(255) NULL,
    DiemTichLuy INT NOT NULL CONSTRAINT DF_KhachHang_DiemTichLuy DEFAULT (0),
    CONSTRAINT CK_KhachHang_DiemTichLuy CHECK (DiemTichLuy >= 0)
);

CREATE TABLE HoaDon
(
    Id INT IDENTITY(1,1) CONSTRAINT PK_HoaDon PRIMARY KEY,
    MaHoaDon NVARCHAR(30) NOT NULL CONSTRAINT UQ_HoaDon_MaHoaDon UNIQUE,
    NgayLap DATETIME2 NOT NULL CONSTRAINT DF_HoaDon_NgayLap DEFAULT (SYSDATETIME()),
    NhanVienId INT NOT NULL,
    KhachHangId INT NULL,
    TongTien DECIMAL(18,2) NOT NULL CONSTRAINT DF_HoaDon_TongTien DEFAULT (0),
    GiamGia DECIMAL(18,2) NOT NULL CONSTRAINT DF_HoaDon_GiamGia DEFAULT (0),
    ThanhTien DECIMAL(18,2) NOT NULL CONSTRAINT DF_HoaDon_ThanhTien DEFAULT (0),
    TrangThai NVARCHAR(30) NOT NULL CONSTRAINT DF_HoaDon_TrangThai DEFAULT (N'DaThanhToan'),
    CONSTRAINT FK_HoaDon_NhanVien FOREIGN KEY (NhanVienId) REFERENCES NhanVien(Id),
    CONSTRAINT FK_HoaDon_KhachHang FOREIGN KEY (KhachHangId) REFERENCES KhachHang(Id),
    CONSTRAINT CK_HoaDon_TongTien CHECK (TongTien >= 0),
    CONSTRAINT CK_HoaDon_GiamGia CHECK (GiamGia >= 0),
    CONSTRAINT CK_HoaDon_ThanhTien CHECK (ThanhTien >= 0)
);

CREATE TABLE ChiTietHoaDon
(
    Id INT IDENTITY(1,1) CONSTRAINT PK_ChiTietHoaDon PRIMARY KEY,
    HoaDonId INT NOT NULL,
    SanPhamId INT NOT NULL,
    SoLuong INT NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL,
    ThanhTien DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_ChiTietHoaDon_HoaDon FOREIGN KEY (HoaDonId) REFERENCES HoaDon(Id),
    CONSTRAINT FK_ChiTietHoaDon_SanPham FOREIGN KEY (SanPhamId) REFERENCES SanPham(Id),
    CONSTRAINT CK_ChiTietHoaDon_SoLuong CHECK (SoLuong > 0),
    CONSTRAINT CK_ChiTietHoaDon_DonGia CHECK (DonGia >= 0),
    CONSTRAINT CK_ChiTietHoaDon_ThanhTien CHECK (ThanhTien >= 0)
);

CREATE TABLE PhieuKiemKe
(
    Id INT IDENTITY(1,1) CONSTRAINT PK_PhieuKiemKe PRIMARY KEY,
    MaPhieuKiemKe NVARCHAR(30) NOT NULL CONSTRAINT UQ_PhieuKiemKe_MaPhieuKiemKe UNIQUE,
    NgayKiemKe DATETIME2 NOT NULL CONSTRAINT DF_PhieuKiemKe_NgayKiemKe DEFAULT (SYSDATETIME()),
    NhanVienId INT NOT NULL,
    GhiChu NVARCHAR(255) NULL,
    CONSTRAINT FK_PhieuKiemKe_NhanVien FOREIGN KEY (NhanVienId) REFERENCES NhanVien(Id)
);

CREATE TABLE ChiTietKiemKe
(
    Id INT IDENTITY(1,1) CONSTRAINT PK_ChiTietKiemKe PRIMARY KEY,
    PhieuKiemKeId INT NOT NULL,
    SanPhamId INT NOT NULL,
    SoLuongHeThong INT NOT NULL,
    SoLuongThucTe INT NOT NULL,
    ChenhLech INT NOT NULL,
    CONSTRAINT FK_ChiTietKiemKe_PhieuKiemKe FOREIGN KEY (PhieuKiemKeId) REFERENCES PhieuKiemKe(Id),
    CONSTRAINT FK_ChiTietKiemKe_SanPham FOREIGN KEY (SanPhamId) REFERENCES SanPham(Id),
    CONSTRAINT CK_ChiTietKiemKe_SoLuongHeThong CHECK (SoLuongHeThong >= 0),
    CONSTRAINT CK_ChiTietKiemKe_SoLuongThucTe CHECK (SoLuongThucTe >= 0)
);

CREATE TABLE LichSuTonKho
(
    Id INT IDENTITY(1,1) CONSTRAINT PK_LichSuTonKho PRIMARY KEY,
    SanPhamId INT NOT NULL,
    ThoiGian DATETIME2 NOT NULL CONSTRAINT DF_LichSuTonKho_ThoiGian DEFAULT (SYSDATETIME()),
    LoaiBienDong NVARCHAR(30) NOT NULL,
    SoLuongThayDoi INT NOT NULL,
    SoLuongSauBienDong INT NOT NULL,
    LyDo NVARCHAR(255) NULL,
    CONSTRAINT FK_LichSuTonKho_SanPham FOREIGN KEY (SanPhamId) REFERENCES SanPham(Id),
    CONSTRAINT CK_LichSuTonKho_SoLuongSau CHECK (SoLuongSauBienDong >= 0)
);

CREATE TABLE LichSuTruyCap
(
    Id INT IDENTITY(1,1) CONSTRAINT PK_LichSuTruyCap PRIMARY KEY,
    TaiKhoanId INT NOT NULL,
    ThoiGian DATETIME2 NOT NULL CONSTRAINT DF_LichSuTruyCap_ThoiGian DEFAULT (SYSDATETIME()),
    HanhDong NVARCHAR(120) NOT NULL,
    DiaChiIP NVARCHAR(50) NULL,
    CONSTRAINT FK_LichSuTruyCap_TaiKhoan FOREIGN KEY (TaiKhoanId) REFERENCES TaiKhoan(Id)
);
GO

INSERT INTO VaiTro(TenVaiTro, MoTa)
VALUES
(N'Admin', N'Quản trị toàn hệ thống'),
(N'QuanLy', N'Chủ cửa hàng hoặc quản lý'),
(N'NhanVienKho', N'Nhân viên nhập hàng và kiểm kê kho'),
(N'NhanVienBanHang', N'Nhân viên lập hóa đơn bán hàng');

DECLARE @DefaultPasswordHash VARCHAR(128) = '8D969EEF6ECAD3C29A3A629280E686CF0C3F5D5A86AFF3CA12020C923ADC6C92';

INSERT INTO TaiKhoan(TenDangNhap, MatKhauHash, VaiTroId)
VALUES
(N'admin', @DefaultPasswordHash, (SELECT Id FROM VaiTro WHERE TenVaiTro = N'Admin')),
(N'quanly', @DefaultPasswordHash, (SELECT Id FROM VaiTro WHERE TenVaiTro = N'QuanLy')),
(N'kho', @DefaultPasswordHash, (SELECT Id FROM VaiTro WHERE TenVaiTro = N'NhanVienKho')),
(N'banhang', @DefaultPasswordHash, (SELECT Id FROM VaiTro WHERE TenVaiTro = N'NhanVienBanHang'));

INSERT INTO NhanVien(MaNhanVien, HoTen, NgaySinh, GioiTinh, ChucVu, SoDienThoai, DiaChi, TaiKhoanId)
VALUES
(N'NV001', N'Nguyễn Văn An', '1990-05-12', N'Nam', N'Quản trị viên', N'0901000001', N'Quận 1, TP.HCM', (SELECT Id FROM TaiKhoan WHERE TenDangNhap = N'admin')),
(N'NV002', N'Trần Thị Bình', '1992-08-20', N'Nữ', N'Quản lý cửa hàng', N'0901000002', N'Quận 3, TP.HCM', (SELECT Id FROM TaiKhoan WHERE TenDangNhap = N'quanly')),
(N'NV003', N'Lê Văn Cường', '1998-03-14', N'Nam', N'Nhân viên kho', N'0901000003', N'Quận 5, TP.HCM', (SELECT Id FROM TaiKhoan WHERE TenDangNhap = N'kho')),
(N'NV004', N'Phạm Thu Dung', '2000-11-02', N'Nữ', N'Nhân viên bán hàng', N'0901000004', N'Quận 10, TP.HCM', (SELECT Id FROM TaiKhoan WHERE TenDangNhap = N'banhang'));

INSERT INTO DanhMuc(MaDanhMuc, TenDanhMuc, MoTa)
VALUES
(N'DM001', N'Sữa và sản phẩm từ sữa', N'Sữa tươi, sữa chua, sữa đặc'),
(N'DM002', N'Đồ uống', N'Nước ngọt, nước suối, trà đóng chai'),
(N'DM003', N'Thực phẩm khô', N'Mì gói, bún, nui, đồ hộp'),
(N'DM004', N'Bánh kẹo', N'Bánh quy, kẹo, snack'),
(N'DM005', N'Gia vị', N'Dầu ăn, nước mắm, đường, muối');

INSERT INTO DonViTinh(TenDonVi)
VALUES (N'Hộp'), (N'Gói'), (N'Lon'), (N'Chai'), (N'Kg');

INSERT INTO NhaCungCap(MaNhaCungCap, TenNhaCungCap, SoDienThoai, DiaChi, Email)
VALUES
(N'NCC001', N'Công ty Vinamilk', N'02811112222', N'TP.HCM', N'sales@vinamilk.vn'),
(N'NCC002', N'Nhà phân phối Acecook', N'02822223333', N'Bình Dương', N'contact@acecook.vn'),
(N'NCC003', N'Đại lý nước giải khát Minh Phát', N'02833334444', N'TP.HCM', N'minhphat@example.com');

INSERT INTO SanPham(MaSanPham, TenSanPham, DanhMucId, DonViTinhId, GiaNhap, GiaBan, SoLuongTon, TonToiThieu, HanSuDung, MaVach, TrangThai)
VALUES
(N'SP001', N'Sữa tươi Vinamilk 180ml', (SELECT Id FROM DanhMuc WHERE MaDanhMuc = N'DM001'), (SELECT Id FROM DonViTinh WHERE TenDonVi = N'Hộp'), 6200, 8000, 120, 20, '2026-12-31', N'8934673000011', N'DangBan'),
(N'SP002', N'Mì Hảo Hảo tôm chua cay', (SELECT Id FROM DanhMuc WHERE MaDanhMuc = N'DM003'), (SELECT Id FROM DonViTinh WHERE TenDonVi = N'Gói'), 3200, 4500, 300, 50, '2027-01-31', N'8934563000022', N'DangBan'),
(N'SP003', N'Coca Cola lon 330ml', (SELECT Id FROM DanhMuc WHERE MaDanhMuc = N'DM002'), (SELECT Id FROM DonViTinh WHERE TenDonVi = N'Lon'), 7200, 10000, 45, 60, '2026-09-30', N'8935001000033', N'DangBan'),
(N'SP004', N'Dầu ăn Tường An 1L', (SELECT Id FROM DanhMuc WHERE MaDanhMuc = N'DM005'), (SELECT Id FROM DonViTinh WHERE TenDonVi = N'Chai'), 38000, 46000, 35, 10, '2027-05-31', N'8936001000044', N'DangBan'),
(N'SP005', N'Bánh Oreo socola', (SELECT Id FROM DanhMuc WHERE MaDanhMuc = N'DM004'), (SELECT Id FROM DonViTinh WHERE TenDonVi = N'Hộp'), 14000, 19000, 25, 15, '2026-08-15', N'8937001000055', N'DangBan');

INSERT INTO KhachHang(MaKhachHang, HoTen, SoDienThoai, DiaChi, DiemTichLuy)
VALUES
(N'KH001', N'Nguyễn Minh Khang', N'0912000001', N'Quận 1, TP.HCM', 120),
(N'KH002', N'Lê Ngọc Mai', N'0912000002', N'Quận Bình Thạnh, TP.HCM', 80);

INSERT INTO PhieuNhap(MaPhieuNhap, NgayNhap, NhaCungCapId, NhanVienId, TongTien, GhiChu)
VALUES
(N'PN20260622001', '2026-06-22T09:15:00', (SELECT Id FROM NhaCungCap WHERE MaNhaCungCap = N'NCC001'), (SELECT Id FROM NhanVien WHERE MaNhanVien = N'NV003'), 620000, N'Nhập sữa đầu tuần');

INSERT INTO ChiTietPhieuNhap(PhieuNhapId, SanPhamId, SoLuong, DonGiaNhap, ThanhTien)
VALUES
((SELECT Id FROM PhieuNhap WHERE MaPhieuNhap = N'PN20260622001'), (SELECT Id FROM SanPham WHERE MaSanPham = N'SP001'), 100, 6200, 620000);

INSERT INTO HoaDon(MaHoaDon, NgayLap, NhanVienId, KhachHangId, TongTien, GiamGia, ThanhTien, TrangThai)
VALUES
(N'HD20260622001', '2026-06-22T18:30:00', (SELECT Id FROM NhanVien WHERE MaNhanVien = N'NV004'), (SELECT Id FROM KhachHang WHERE MaKhachHang = N'KH001'), 35000, 0, 35000, N'DaThanhToan');

INSERT INTO ChiTietHoaDon(HoaDonId, SanPhamId, SoLuong, DonGia, ThanhTien)
VALUES
((SELECT Id FROM HoaDon WHERE MaHoaDon = N'HD20260622001'), (SELECT Id FROM SanPham WHERE MaSanPham = N'SP001'), 2, 8000, 16000),
((SELECT Id FROM HoaDon WHERE MaHoaDon = N'HD20260622001'), (SELECT Id FROM SanPham WHERE MaSanPham = N'SP005'), 1, 19000, 19000);

INSERT INTO PhieuKiemKe(MaPhieuKiemKe, NgayKiemKe, NhanVienId, GhiChu)
VALUES
(N'KK20260622001', '2026-06-22T20:00:00', (SELECT Id FROM NhanVien WHERE MaNhanVien = N'NV003'), N'Kiểm kê cuối ngày');

INSERT INTO ChiTietKiemKe(PhieuKiemKeId, SanPhamId, SoLuongHeThong, SoLuongThucTe, ChenhLech)
VALUES
((SELECT Id FROM PhieuKiemKe WHERE MaPhieuKiemKe = N'KK20260622001'), (SELECT Id FROM SanPham WHERE MaSanPham = N'SP003'), 45, 45, 0);

INSERT INTO LichSuTonKho(SanPhamId, LoaiBienDong, SoLuongThayDoi, SoLuongSauBienDong, LyDo)
VALUES
((SELECT Id FROM SanPham WHERE MaSanPham = N'SP001'), N'NhapHang', 100, 120, N'Phiếu nhập PN20260622001'),
((SELECT Id FROM SanPham WHERE MaSanPham = N'SP001'), N'BanHang', -2, 118, N'Hóa đơn HD20260622001'),
((SELECT Id FROM SanPham WHERE MaSanPham = N'SP003'), N'KiemKe', 0, 45, N'Kiểm kê KK20260622001');

INSERT INTO LichSuTruyCap(TaiKhoanId, HanhDong, DiaChiIP)
VALUES
((SELECT Id FROM TaiKhoan WHERE TenDangNhap = N'admin'), N'Đăng nhập hệ thống', N'127.0.0.1');
GO

-- SELECT kiểm thử: tài khoản đăng nhập và vai trò
SELECT tk.TenDangNhap, tk.IsActive, vt.TenVaiTro, nv.HoTen
FROM TaiKhoan tk
INNER JOIN VaiTro vt ON vt.Id = tk.VaiTroId
LEFT JOIN NhanVien nv ON nv.TaiKhoanId = tk.Id;

-- SELECT kiểm thử: sản phẩm tồn kho thấp
SELECT MaSanPham, TenSanPham, SoLuongTon, TonToiThieu
FROM SanPham
WHERE SoLuongTon <= TonToiThieu;

-- SELECT kiểm thử: doanh thu theo ngày
SELECT CAST(NgayLap AS DATE) AS Ngay, SUM(ThanhTien) AS DoanhThu, COUNT(*) AS SoHoaDon
FROM HoaDon
GROUP BY CAST(NgayLap AS DATE)
ORDER BY Ngay DESC;

-- SELECT kiểm thử: top sản phẩm bán chạy
SELECT TOP 10 sp.MaSanPham, sp.TenSanPham, SUM(ct.SoLuong) AS SoLuongBan, SUM(ct.ThanhTien) AS DoanhThu
FROM ChiTietHoaDon ct
INNER JOIN HoaDon hd ON hd.Id = ct.HoaDonId
INNER JOIN SanPham sp ON sp.Id = ct.SanPhamId
GROUP BY sp.MaSanPham, sp.TenSanPham
ORDER BY SoLuongBan DESC;

-- SELECT kiểm thử: sản phẩm sắp hết hạn trong 30 ngày
SELECT MaSanPham, TenSanPham, HanSuDung, SoLuongTon
FROM SanPham
WHERE HanSuDung IS NOT NULL
  AND HanSuDung BETWEEN CAST(GETDATE() AS DATE) AND DATEADD(DAY, 30, CAST(GETDATE() AS DATE))
ORDER BY HanSuDung;
