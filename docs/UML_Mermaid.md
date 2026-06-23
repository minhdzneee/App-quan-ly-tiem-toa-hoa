# UML và ERD Mermaid

Tài liệu này cung cấp Mermaid code để chèn vào báo cáo hoặc render lại sơ đồ. Trong thư mục gốc cũng đã có các file ảnh PNG của Use Case, Activity, Sequence, Class Diagram và ERD.

## Use Case Tổng Quát

```mermaid
flowchart LR
    Admin[Admin / Quản trị viên]
    QuanLy[Chủ cửa hàng / Quản lý]
    Kho[Nhân viên kho]
    BanHang[Nhân viên bán hàng]
    NCC[Nhà cung cấp]
    KH[Khách hàng]

    subgraph System[Hệ thống quản lý hàng tạp hóa]
        UCLogin((Đăng nhập))
        UCLogout((Đăng xuất))
        UCTK((Quản lý tài khoản))
        UCPQ((Phân quyền người dùng))
        UCNV((Quản lý nhân viên))
        UCSP((Quản lý sản phẩm))
        UCDM((Quản lý danh mục))
        UCDVT((Quản lý đơn vị tính))
        UCNCC((Quản lý nhà cung cấp))
        UCPN((Lập phiếu nhập hàng))
        UCTon((Quản lý tồn kho))
        UCKK((Kiểm kê kho))
        UCHD((Lập hóa đơn bán hàng))
        UCKH((Quản lý khách hàng))
        UCBaoCao((Xem báo cáo thống kê))
    end

    Admin --> UCLogin
    Admin --> UCTK
    Admin --> UCPQ
    Admin --> UCNV
    Admin --> UCLogout
    QuanLy --> UCSP
    QuanLy --> UCDM
    QuanLy --> UCDVT
    QuanLy --> UCNCC
    QuanLy --> UCTon
    QuanLy --> UCKK
    QuanLy --> UCBaoCao
    Kho --> UCPN
    Kho --> UCTon
    Kho --> UCKK
    BanHang --> UCHD
    BanHang --> UCKH
    NCC --> UCPN
    KH --> UCHD
```

## Use Case Chi Tiết: Đăng Nhập

```mermaid
flowchart LR
    User[Admin / Quản lý / Nhân viên]
    Login((Đăng nhập hệ thống))
    Auth((Xác thực tài khoản))
    Role((Xác định quyền hạn))
    Error((Hiển thị lỗi đăng nhập))
    Locked((Thông báo tài khoản bị khóa))

    User --> Login
    Login -. include .-> Auth
    Login -. include .-> Role
    Login -. extend .-> Error
    Login -. extend .-> Locked
```

## Use Case Chi Tiết: Quản Lý Sản Phẩm

```mermaid
flowchart LR
    QuanLy[Quản lý]
    Kho[Nhân viên kho]
    UC((Quản lý sản phẩm))
    Add((Thêm sản phẩm))
    Edit((Sửa sản phẩm))
    Delete((Xóa sản phẩm))
    Search((Tìm kiếm sản phẩm))
    Duplicate((Kiểm tra trùng mã))
    Save((Lưu thông tin))
    LowStock((Cảnh báo sắp hết hàng))
    Expired((Cảnh báo hết hạn))

    QuanLy --> UC
    Kho --> UC
    UC -. include .-> Add
    UC -. include .-> Edit
    UC -. include .-> Delete
    UC -. include .-> Search
    UC -. include .-> Duplicate
    UC -. include .-> Save
    UC -. extend .-> LowStock
    UC -. extend .-> Expired
```

## Activity Diagram: Quy Trình Bán Hàng

```mermaid
flowchart TD
    A([Bắt đầu]) --> B[Nhân viên đăng nhập]
    B --> C[Chọn chức năng bán hàng]
    C --> D[Tìm sản phẩm hoặc quét mã vạch]
    D --> E{Đủ số lượng tồn?}
    E -- Không --> F[Hiển thị cảnh báo]
    F --> D
    E -- Có --> G[Thêm sản phẩm vào hóa đơn]
    G --> H{Còn sản phẩm khác?}
    H -- Có --> D
    H -- Không --> I[Tính tổng tiền]
    I --> J{Có giảm giá?}
    J -- Có --> K[Áp dụng giảm giá]
    J -- Không --> L[Khách hàng thanh toán]
    K --> L
    L --> M[Lưu hóa đơn]
    M --> N[Trừ số lượng tồn kho]
    N --> O[In hóa đơn]
    O --> P([Kết thúc])
```

## Activity Diagram: Quy Trình Nhập Hàng

```mermaid
flowchart TD
    A([Bắt đầu]) --> B[Nhân viên kho đăng nhập]
    B --> C[Chọn chức năng nhập hàng]
    C --> D[Chọn nhà cung cấp]
    D --> E{Nhà cung cấp đã tồn tại?}
    E -- Không --> F[Thêm nhà cung cấp mới]
    E -- Có --> G[Tìm sản phẩm]
    F --> G
    G --> H{Sản phẩm đã tồn tại?}
    H -- Không --> I[Thêm sản phẩm mới]
    H -- Có --> J[Nhập số lượng và đơn giá nhập]
    I --> J
    J --> K{Còn sản phẩm khác?}
    K -- Có --> G
    K -- Không --> L[Tính tổng tiền phiếu nhập]
    L --> M[Lưu phiếu nhập]
    M --> N[Cập nhật số lượng tồn kho]
    N --> O[In phiếu nhập]
    O --> P([Kết thúc])
```

## Sequence Diagram: Quy Trình Bán Hàng

```mermaid
sequenceDiagram
    actor Staff as Nhân viên bán hàng
    participant View as BanHangView
    participant VM as BanHangViewModel
    participant BLL as BanHangService
    participant DAL as HoaDonRepository / SanPhamRepository
    participant DB as SQL Server

    Staff->>View: Nhập mã vạch hoặc tìm sản phẩm
    View->>VM: SearchProductCommand
    VM->>BLL: TimSanPhamAsync(keyword)
    BLL->>DAL: GetByCodeOrBarcodeAsync(keyword)
    DAL->>DB: SELECT SanPham
    DB-->>DAL: Thông tin sản phẩm
    DAL-->>BLL: SanPham
    BLL-->>VM: SanPham
    VM-->>View: Hiển thị sản phẩm
    Staff->>View: Nhập số lượng, thêm hóa đơn
    View->>VM: AddToInvoiceCommand
    VM->>VM: Kiểm tra tồn kho tạm thời
    Staff->>View: Thanh toán
    View->>VM: CheckoutCommand
    VM->>BLL: ThanhToanAsync(hoaDon)
    BLL->>DAL: Kiểm tra tồn kho và tạo hóa đơn
    DAL->>DB: INSERT HoaDon
    DAL->>DB: INSERT ChiTietHoaDon
    DAL->>DB: UPDATE SanPham.SoLuongTon
    DAL->>DB: INSERT LichSuTonKho
    DB-->>DAL: Thành công
    DAL-->>BLL: HoaDonId
    BLL-->>VM: Kết quả
    VM-->>View: Thông báo và in hóa đơn
```

## Sequence Diagram: Quy Trình Nhập Hàng

```mermaid
sequenceDiagram
    actor Staff as Nhân viên kho
    participant View as NhapHangView
    participant VM as NhapHangViewModel
    participant BLL as NhapHangService
    participant DAL as PhieuNhapRepository
    participant DB as SQL Server

    Staff->>View: Chọn chức năng nhập hàng
    View->>VM: LoadCommand
    VM->>BLL: Load danh sách nhà cung cấp
    BLL->>DAL: GetAllAsync()
    DAL->>DB: SELECT NhaCungCap
    DB-->>DAL: Danh sách nhà cung cấp
    DAL-->>VM: Dữ liệu
    Staff->>View: Chọn sản phẩm, nhập số lượng và đơn giá
    View->>VM: AddItemCommand
    VM->>VM: Tính thành tiền
    Staff->>View: Lưu phiếu nhập
    View->>VM: SaveCommand
    VM->>BLL: LapPhieuNhapAsync(phieuNhap)
    BLL->>DAL: CreateAsync(phieuNhap)
    DAL->>DB: INSERT PhieuNhap
    DAL->>DB: INSERT ChiTietPhieuNhap
    DAL->>DB: UPDATE SanPham.SoLuongTon
    DAL->>DB: INSERT LichSuTonKho
    DB-->>DAL: Thành công
    DAL-->>BLL: PhieuNhapId
    BLL-->>VM: Kết quả
    VM-->>View: Hiển thị phiếu nhập
```

## Class Diagram

```mermaid
classDiagram
    class VaiTro {
      int Id
      string TenVaiTro
      string MoTa
    }
    class TaiKhoan {
      int Id
      string TenDangNhap
      string MatKhauHash
      bool IsActive
      int VaiTroId
      DangNhap()
      DoiMatKhau()
    }
    class NhanVien {
      int Id
      string MaNhanVien
      string HoTen
      DateTime NgaySinh
      string GioiTinh
      string ChucVu
      string SoDienThoai
      string DiaChi
      int TaiKhoanId
    }
    class SanPham {
      int Id
      string MaSanPham
      string TenSanPham
      decimal GiaNhap
      decimal GiaBan
      int SoLuongTon
      int TonToiThieu
      DateTime HanSuDung
      string MaVach
      KiemTraTonKho()
      CapNhatSoLuong()
      KiemTraHetHan()
    }
    class PhieuNhap {
      int Id
      string MaPhieuNhap
      DateTime NgayNhap
      decimal TongTien
      LapPhieuNhap()
      TinhTongTien()
    }
    class ChiTietPhieuNhap {
      int SoLuong
      decimal DonGiaNhap
      decimal ThanhTien
      TinhThanhTien()
    }
    class HoaDon {
      int Id
      string MaHoaDon
      DateTime NgayLap
      decimal TongTien
      decimal GiamGia
      decimal ThanhTien
      TinhTongTien()
      ThanhToan()
      InHoaDon()
    }
    class ChiTietHoaDon {
      int SoLuong
      decimal DonGia
      decimal ThanhTien
      TinhThanhTien()
    }
    class DanhMuc
    class DonViTinh
    class NhaCungCap
    class KhachHang
    class PhieuKiemKe
    class ChiTietKiemKe
    class LichSuTonKho
    class LichSuTruyCap

    VaiTro "1" --> "N" TaiKhoan
    TaiKhoan "1" --> "1" NhanVien
    TaiKhoan "1" --> "N" LichSuTruyCap
    DanhMuc "1" --> "N" SanPham
    DonViTinh "1" --> "N" SanPham
    NhaCungCap "1" --> "N" PhieuNhap
    NhanVien "1" --> "N" PhieuNhap
    PhieuNhap "1" --> "N" ChiTietPhieuNhap
    SanPham "1" --> "N" ChiTietPhieuNhap
    NhanVien "1" --> "N" HoaDon
    KhachHang "1" --> "N" HoaDon
    HoaDon "1" --> "N" ChiTietHoaDon
    SanPham "1" --> "N" ChiTietHoaDon
    NhanVien "1" --> "N" PhieuKiemKe
    PhieuKiemKe "1" --> "N" ChiTietKiemKe
    SanPham "1" --> "N" ChiTietKiemKe
    SanPham "1" --> "N" LichSuTonKho
```

## ERD Chi Tiết

```mermaid
erDiagram
    VaiTro ||--o{ TaiKhoan : co
    TaiKhoan ||--o| NhanVien : gan
    TaiKhoan ||--o{ LichSuTruyCap : ghi
    DanhMuc ||--o{ SanPham : phan_loai
    DonViTinh ||--o{ SanPham : tinh_theo
    NhaCungCap ||--o{ PhieuNhap : cung_cap
    NhanVien ||--o{ PhieuNhap : lap
    PhieuNhap ||--o{ ChiTietPhieuNhap : gom
    SanPham ||--o{ ChiTietPhieuNhap : duoc_nhap
    NhanVien ||--o{ HoaDon : lap
    KhachHang ||--o{ HoaDon : mua
    HoaDon ||--o{ ChiTietHoaDon : gom
    SanPham ||--o{ ChiTietHoaDon : duoc_ban
    NhanVien ||--o{ PhieuKiemKe : lap
    PhieuKiemKe ||--o{ ChiTietKiemKe : gom
    SanPham ||--o{ ChiTietKiemKe : kiem_ke
    SanPham ||--o{ LichSuTonKho : co

    SanPham {
      int Id PK
      nvarchar MaSanPham UK
      nvarchar TenSanPham
      int DanhMucId FK
      int DonViTinhId FK
      decimal GiaNhap
      decimal GiaBan
      int SoLuongTon
      int TonToiThieu
      date HanSuDung
      nvarchar MaVach UK
      nvarchar TrangThai
    }
    HoaDon {
      int Id PK
      nvarchar MaHoaDon UK
      datetime2 NgayLap
      int NhanVienId FK
      int KhachHangId FK
      decimal TongTien
      decimal GiamGia
      decimal ThanhTien
      nvarchar TrangThai
    }
    PhieuNhap {
      int Id PK
      nvarchar MaPhieuNhap UK
      datetime2 NgayNhap
      int NhaCungCapId FK
      int NhanVienId FK
      decimal TongTien
    }
```
