# Báo Cáo Bài Tập Lớn

## Đề tài

**HỆ THỐNG QUẢN LÝ HÀNG TẠP HÓA**

## Công nghệ sử dụng

- Ngôn ngữ lập trình: C# .NET
- Giao diện: WPF
- Mô hình giao diện: MVVM
- Cơ sở dữ liệu: Microsoft SQL Server
- Kết nối dữ liệu: ADO.NET
- Kiến trúc phần mềm: 3-Layer Architecture
- Công cụ phát triển: Visual Studio 2022 trở lên, khuyến nghị cài workload .NET Desktop Development cho .NET 8 WPF
- Công cụ mô hình hóa: UML, Mermaid, ERD

---

# Chương 1. Tổng Quan Về Đề Tài

## 1.1. Đặt vấn đề

Tiệm tạp hóa thường có số lượng mặt hàng lớn, thay đổi liên tục theo nhập hàng, bán hàng, tồn kho và hạn sử dụng. Nếu quản lý bằng sổ sách hoặc bảng tính thủ công, chủ cửa hàng dễ gặp các vấn đề như sai lệch tồn kho, quên hạn sử dụng, khó thống kê doanh thu và mất thời gian khi tra cứu sản phẩm.

Hệ thống quản lý hàng tạp hóa được xây dựng nhằm hỗ trợ cửa hàng quản lý toàn bộ nghiệp vụ cơ bản: tài khoản người dùng, sản phẩm, danh mục, nhà cung cấp, phiếu nhập, hóa đơn bán hàng, khách hàng, kiểm kê kho và báo cáo thống kê.

## 1.2. Lý do chọn đề tài

Đề tài gần gũi với thực tế, có đầy đủ nghiệp vụ nhập, xuất, tồn và báo cáo. Đây cũng là bài toán phù hợp để áp dụng phân tích thiết kế hướng đối tượng, mô hình hóa UML, thiết kế cơ sở dữ liệu quan hệ và triển khai ứng dụng desktop theo WPF MVVM.

## 1.3. Mục tiêu đề tài

- Phân tích nghiệp vụ quản lý tiệm tạp hóa.
- Thiết kế actor, use case, activity diagram, sequence diagram, class diagram và ERD.
- Xây dựng database SQL Server có đầy đủ khóa chính, khóa ngoại, ràng buộc và dữ liệu mẫu.
- Triển khai mã nguồn minh họa theo kiến trúc 3 tầng và mô hình MVVM.
- Xây dựng giao diện WPF cho các nghiệp vụ chính.

## 1.4. Đối tượng và phạm vi nghiên cứu

Đối tượng nghiên cứu là quy trình quản lý hàng hóa tại tiệm tạp hóa quy mô nhỏ và vừa. Phạm vi hệ thống tập trung vào quản lý nội bộ tại cửa hàng, chưa triển khai bán hàng online, đồng bộ đa chi nhánh hoặc tích hợp thiết bị phần cứng chuyên dụng.

## 1.5. Yêu cầu chức năng

- Đăng nhập, đăng xuất, đổi mật khẩu.
- Quản lý tài khoản, vai trò và nhân viên.
- Quản lý danh mục, đơn vị tính, sản phẩm và nhà cung cấp.
- Lập phiếu nhập hàng, cập nhật tồn kho.
- Lập hóa đơn bán hàng, kiểm tra tồn kho, thanh toán.
- Quản lý khách hàng và điểm tích lũy.
- Kiểm kê kho và ghi nhận chênh lệch.
- Cảnh báo hàng tồn thấp và sản phẩm sắp hết hạn.
- Báo cáo doanh thu, nhập hàng, bán hàng và sản phẩm bán chạy.

## 1.6. Yêu cầu phi chức năng

- Giao diện dễ dùng, phù hợp thao tác bán hàng nhanh.
- Dữ liệu nhất quán nhờ transaction khi nhập hàng, bán hàng và kiểm kê.
- Mã nguồn tách lớp rõ ràng, dễ bảo trì.
- Cơ sở dữ liệu có ràng buộc để hạn chế dữ liệu sai.
- Có thể mở rộng thêm thiết bị quét mã vạch, in hóa đơn và quản lý đa chi nhánh.

## 1.7. Phương pháp nghiên cứu và công cụ sử dụng

Đề tài sử dụng phương pháp phân tích hướng đối tượng, mô hình hóa UML, thiết kế database quan hệ và lập trình ứng dụng desktop. Công cụ gồm Visual Studio, SQL Server Management Studio và Mermaid để mô tả sơ đồ.

## 1.8. Kế hoạch thực hiện dự án

| Giai đoạn | Nội dung |
| --- | --- |
| 1 | Khảo sát nghiệp vụ, xác định yêu cầu |
| 2 | Phân tích actor, use case và luồng xử lý |
| 3 | Thiết kế class diagram, ERD và SQL script |
| 4 | Thiết kế kiến trúc 3 tầng kết hợp MVVM |
| 5 | Lập trình Models, DAL, BLL, ViewModels và Views |
| 6 | Kiểm thử chức năng chính và hoàn thiện báo cáo |

## 1.9. Kết luận chương

Chương 1 đã trình bày bối cảnh, mục tiêu và phạm vi đề tài. Hệ thống được định hướng là một ứng dụng desktop quản lý hàng tạp hóa có tính thực tế và phù hợp với yêu cầu môn học.

---

# Chương 2. Khảo Sát Và Phân Tích Hệ Thống

## 2.1. Khảo sát nghiệp vụ hiện tại

Trong mô hình thủ công, chủ cửa hàng thường ghi nhận nhập hàng, bán hàng và tồn kho bằng sổ hoặc file Excel. Khi khách mua hàng, nhân viên kiểm tra hàng trên kệ, tính tiền và ghi hóa đơn. Khi nhập hàng, nhân viên kho kiểm số lượng và cập nhật lại tồn. Cuối ngày hoặc cuối tháng, chủ cửa hàng tổng hợp doanh thu và kiểm tra hàng tồn.

## 2.2. Các vấn đề của hệ thống thủ công

- Dễ sai sót khi tính tiền hoặc cập nhật tồn kho.
- Khó kiểm soát sản phẩm sắp hết hàng.
- Khó phát hiện sản phẩm hết hạn hoặc sắp hết hạn.
- Mất thời gian tổng hợp báo cáo doanh thu.
- Không phân quyền rõ ràng giữa quản lý, kho và bán hàng.
- Khó truy vết lịch sử truy cập và lịch sử biến động tồn kho.

## 2.3. Xác định yêu cầu hệ thống

Hệ thống mới cần số hóa các nghiệp vụ chính, lưu trữ dữ liệu tập trung trong SQL Server, tự động tính toán tổng tiền, cập nhật tồn kho, ghi lịch sử biến động và cung cấp báo cáo để hỗ trợ quyết định.

## 2.4. Actor của hệ thống

| Actor | Vai trò |
| --- | --- |
| Admin | Quản lý tài khoản, phân quyền, nhân viên và lịch sử truy cập |
| Chủ cửa hàng / Quản lý | Quản lý sản phẩm, danh mục, nhà cung cấp, tồn kho và báo cáo |
| Nhân viên kho | Lập phiếu nhập, kiểm kê, cập nhật tồn kho |
| Nhân viên bán hàng | Tìm sản phẩm, lập hóa đơn, thanh toán, quản lý khách hàng |
| Nhà cung cấp | Cung cấp hàng hóa, liên quan đến phiếu nhập |
| Khách hàng | Mua hàng, được lưu thông tin và lịch sử mua |

## 2.5. Các chức năng chính

- Quản trị hệ thống: tài khoản, vai trò, nhân viên.
- Quản lý hàng hóa: sản phẩm, danh mục, đơn vị tính, nhà cung cấp.
- Nhập hàng: lập phiếu nhập, chi tiết nhập, cập nhật tồn.
- Bán hàng: hóa đơn, chi tiết hóa đơn, thanh toán, in hóa đơn.
- Kho: tồn kho, kiểm kê, lịch sử biến động.
- Báo cáo: doanh thu, hóa đơn, phiếu nhập, sản phẩm bán chạy, tồn thấp, sắp hết hạn.

---

# Chương 3. Phân Tích Và Thiết Kế Hệ Thống

## 3.1. Tổng quan hệ thống đề xuất

Hệ thống được tổ chức theo kiến trúc 3 tầng kết hợp MVVM:

- Presentation Layer: WPF Views, ViewModels, Commands, Binding.
- Business Logic Layer: service xử lý nghiệp vụ và kiểm tra dữ liệu.
- Data Access Layer: repository truy cập SQL Server bằng ADO.NET.
- Models: lớp miền dùng chung cho cả ba tầng.

Luồng xử lý chuẩn là: Người dùng → View → ViewModel → BLL Service → DAL Repository → SQL Server.

## 3.2. Use Case Diagram tổng quát

Use Case tổng quát gồm đăng nhập, đăng xuất, quản lý tài khoản, phân quyền, quản lý sản phẩm, danh mục, đơn vị tính, nhà cung cấp, nhập hàng, tồn kho, kiểm kê, bán hàng, khách hàng và báo cáo. Mermaid code được đặt trong `docs/UML_Mermaid.md`; hình ảnh có sẵn trong thư mục gốc.

## 3.3. Use Case chi tiết đăng nhập

Actor gồm Admin, Quản lý, Nhân viên bán hàng và Nhân viên kho. Use case chính là đăng nhập hệ thống. Hệ thống include xác thực tài khoản và xác định quyền hạn. Trường hợp mở rộng gồm hiển thị lỗi đăng nhập và thông báo tài khoản bị khóa.

## 3.4. Use Case chi tiết quản lý sản phẩm

Actor gồm Quản lý và Nhân viên kho. Các chức năng include thêm, sửa, xóa, tìm kiếm, kiểm tra trùng mã và lưu sản phẩm. Các tình huống extend gồm cảnh báo tồn kho thấp và cảnh báo hết hạn sử dụng.

## 3.5. Use Case chi tiết lập phiếu nhập hàng

Actor gồm Quản lý, Nhân viên kho và Nhà cung cấp. Hệ thống cho phép chọn nhà cung cấp, tìm sản phẩm, nhập chi tiết, tính tổng tiền, lưu phiếu nhập và cập nhật tồn kho. Trường hợp mở rộng gồm thêm nhanh nhà cung cấp hoặc sản phẩm.

## 3.6. Use Case chi tiết lập hóa đơn bán hàng

Actor gồm Nhân viên bán hàng và Khách hàng. Hệ thống hỗ trợ tìm sản phẩm, kiểm tra tồn, thêm vào hóa đơn, tính tổng tiền, giảm giá, thanh toán, cập nhật tồn kho và in hóa đơn. Trường hợp mở rộng gồm không đủ hàng, áp dụng khuyến mãi và hủy hóa đơn.

## 3.7. Activity Diagram

Hai quy trình chính là bán hàng và nhập hàng. Quy trình bán hàng bắt đầu từ đăng nhập, tìm sản phẩm, kiểm tra tồn kho, thêm hóa đơn, thanh toán, lưu hóa đơn và trừ tồn. Quy trình nhập hàng bắt đầu từ chọn nhà cung cấp, chọn sản phẩm, nhập số lượng, lưu phiếu nhập và cộng tồn kho.

## 3.8. Sequence Diagram

Sequence Diagram được thiết kế theo đúng tuyến MVVM và 3 tầng. Với bán hàng, `BanHangView` gửi command đến `BanHangViewModel`, sau đó gọi `BanHangService`, repository và SQL Server. Với nhập hàng, `NhapHangViewModel` gọi `NhapHangService`, service gọi `PhieuNhapRepository` để lưu phiếu và cập nhật tồn kho trong transaction.

## 3.9. Class Diagram

Các lớp chính gồm `VaiTro`, `TaiKhoan`, `NhanVien`, `DanhMuc`, `DonViTinh`, `SanPham`, `NhaCungCap`, `PhieuNhap`, `ChiTietPhieuNhap`, `HoaDon`, `ChiTietHoaDon`, `KhachHang`, `PhieuKiemKe`, `ChiTietKiemKe`, `LichSuTonKho`, `LichSuTruyCap`.

Quan hệ chính:

- `VaiTro` 1-N `TaiKhoan`.
- `TaiKhoan` 1-1 `NhanVien`.
- `DanhMuc` 1-N `SanPham`.
- `DonViTinh` 1-N `SanPham`.
- `NhaCungCap` 1-N `PhieuNhap`.
- `PhieuNhap` 1-N `ChiTietPhieuNhap`.
- `HoaDon` 1-N `ChiTietHoaDon`.
- `PhieuKiemKe` 1-N `ChiTietKiemKe`.
- `SanPham` 1-N các chi tiết nhập, bán, kiểm kê và lịch sử tồn kho.

---

# Chương 4. Thiết Kế Cơ Sở Dữ Liệu

## 4.1. Mô hình dữ liệu

Cơ sở dữ liệu được thiết kế theo mô hình quan hệ. Mỗi bảng có khóa chính `Id INT IDENTITY`. Các bảng nghiệp vụ sử dụng khóa ngoại để liên kết sản phẩm, nhân viên, nhà cung cấp, khách hàng và chứng từ.

## 4.2. ERD tổng quát và chi tiết

ERD tổng quát thể hiện các nhóm bảng chính: quản trị người dùng, danh mục hàng hóa, nhập hàng, bán hàng, kiểm kê và báo cáo tồn kho. ERD chi tiết mô tả từng bảng, khóa chính, khóa ngoại và ràng buộc dữ liệu. Mermaid code đặt trong `docs/UML_Mermaid.md`; ảnh ERD có sẵn trong thư mục gốc.

## 4.3. Danh sách bảng dữ liệu

| Bảng | Mục đích |
| --- | --- |
| `VaiTro` | Lưu vai trò người dùng |
| `TaiKhoan` | Lưu tài khoản đăng nhập |
| `NhanVien` | Lưu thông tin nhân viên |
| `DanhMuc` | Nhóm sản phẩm |
| `DonViTinh` | Đơn vị tính |
| `SanPham` | Thông tin hàng hóa và tồn kho |
| `NhaCungCap` | Thông tin nhà cung cấp |
| `PhieuNhap` | Chứng từ nhập hàng |
| `ChiTietPhieuNhap` | Chi tiết sản phẩm trong phiếu nhập |
| `KhachHang` | Thông tin khách hàng |
| `HoaDon` | Hóa đơn bán hàng |
| `ChiTietHoaDon` | Chi tiết sản phẩm trong hóa đơn |
| `PhieuKiemKe` | Phiếu kiểm kê |
| `ChiTietKiemKe` | Chi tiết kiểm kê |
| `LichSuTonKho` | Lịch sử biến động tồn kho |
| `LichSuTruyCap` | Lịch sử truy cập tài khoản |

## 4.4. Ràng buộc dữ liệu

- Tiền tệ dùng `DECIMAL(18,2)`.
- Ngày giờ dùng `DATETIME2` hoặc `DATE`.
- Tên và mô tả dùng `NVARCHAR`.
- Mã sản phẩm, hóa đơn, phiếu nhập, nhân viên, khách hàng đặt unique.
- Số lượng và giá tiền có `CHECK` không âm.
- Ngày lập, trạng thái hóa đơn, trạng thái tài khoản và tồn kho có `DEFAULT`.
- Các bảng chi tiết có khóa ngoại rõ ràng đến bảng cha.

## 4.5. SQL Script

Script hoàn chỉnh nằm tại `database/GroceryStoreManagement.sql`, bao gồm `CREATE DATABASE`, `CREATE TABLE`, khóa chính, khóa ngoại, unique, check, default, dữ liệu mẫu và SELECT kiểm thử.

---

# Chương 5. Thiết Kế Và Triển Khai Hệ Thống

## 5.1. Kiến trúc hệ thống

Ứng dụng chia thành bốn project:

- `GroceryStoreManagement.Models`: định nghĩa dữ liệu và phương thức nghiệp vụ đơn giản trong model.
- `GroceryStoreManagement.DAL`: truy vấn SQL Server và transaction.
- `GroceryStoreManagement.BLL`: xử lý nghiệp vụ, kiểm tra điều kiện trước khi lưu.
- `GroceryStoreManagement.Presentation`: WPF, MVVM, Binding và Command.

## 5.2. Cấu trúc project

```text
GroceryStoreManagement/
  GroceryStoreManagement.Models/
  GroceryStoreManagement.DAL/
    Database/
    Repositories/
  GroceryStoreManagement.BLL/
    Services/
  GroceryStoreManagement.Presentation/
    Commands/
    ViewModels/
    Views/
  database/
  docs/
```

## 5.3. Thiết kế giao diện WPF

Các màn hình chính:

- `LoginView.xaml`: đăng nhập.
- `MainWindow.xaml`: menu trái, thông tin người dùng, vùng hiển thị chức năng.
- `DashboardView.xaml`: doanh thu và cảnh báo nhanh.
- `SanPhamView.xaml`: quản lý sản phẩm bằng DataGrid và form nhập liệu.
- `NhapHangView.xaml`: lập phiếu nhập.
- `BanHangView.xaml`: lập hóa đơn bán hàng.
- `KiemKeView.xaml`: kiểm kê kho.
- `BaoCaoView.xaml`: báo cáo thống kê.

## 5.4. Mã nguồn minh họa

Một số lớp tiêu biểu:

- `RelayCommand.cs`, `AsyncRelayCommand.cs`: triển khai command cho MVVM.
- `BaseViewModel.cs`: hỗ trợ `INotifyPropertyChanged`.
- `AuthService.cs`: xác thực đăng nhập bằng mật khẩu băm SHA-256.
- `SanPhamService.cs`: kiểm tra dữ liệu sản phẩm, chống trùng mã và giá/số lượng âm.
- `BanHangService.cs`: kiểm tra tồn kho trước khi bán.
- `NhapHangService.cs`: lập phiếu nhập và điều phối cập nhật tồn.
- `HoaDonRepository.cs`: lưu hóa đơn và chi tiết trong transaction.
- `PhieuNhapRepository.cs`: lưu phiếu nhập và cộng tồn kho trong transaction.

## 5.5. Cách kết nối SQL Server

Chuỗi kết nối mặc định đặt trong `App.xaml.cs`:

```csharp
@"Server=(localdb)\MSSQLLocalDB;Database=GroceryStoreManagement;Trusted_Connection=True;TrustServerCertificate=True;"
```

Khi triển khai trên máy khác, chỉ cần thay `Server` theo SQL Server instance đang sử dụng.

## 5.6. Kiểm thử chức năng

Các ca kiểm thử chính:

| Ca kiểm thử | Kết quả mong đợi |
| --- | --- |
| Đăng nhập đúng tài khoản | Vào màn hình chính |
| Đăng nhập sai mật khẩu | Hiển thị thông báo lỗi |
| Thêm sản phẩm trùng mã | Không cho lưu |
| Nhập giá âm hoặc số lượng âm | Không cho lưu |
| Bán quá số lượng tồn | Cảnh báo không đủ hàng |
| Lưu hóa đơn hợp lệ | Tạo hóa đơn, trừ tồn, ghi lịch sử tồn kho |
| Lưu phiếu nhập hợp lệ | Tạo phiếu nhập, cộng tồn, ghi lịch sử tồn kho |
| Kiểm kê hợp lệ | Lưu phiếu kiểm kê và cập nhật tồn thực tế |

---

# Chương 6. Kết Luận Và Hướng Phát Triển

## 6.1. Kết quả đạt được

Đề tài đã xây dựng được bộ phân tích thiết kế và mã nguồn minh họa cho hệ thống quản lý hàng tạp hóa. Sản phẩm có database SQL Server, kiến trúc 3 tầng, mô hình MVVM, các màn hình WPF chính và tài liệu UML/ERD đi kèm.

## 6.2. Hạn chế

- Chưa tích hợp máy in hóa đơn thật.
- Chưa tích hợp thiết bị quét mã vạch vật lý.
- Chưa có phân quyền chi tiết đến từng nút chức năng.
- Chưa triển khai báo cáo xuất Excel/PDF.
- Chưa có đồng bộ dữ liệu đa chi nhánh.

## 6.3. Hướng phát triển

- Bổ sung in hóa đơn, in phiếu nhập và xuất báo cáo.
- Tích hợp máy quét mã vạch.
- Thêm module khuyến mãi và quản lý công nợ nhà cung cấp.
- Hoàn thiện phân quyền chi tiết theo chức năng.
- Xây dựng API hoặc đồng bộ cloud để mở rộng đa chi nhánh.

## 6.4. Kết luận

Hệ thống quản lý hàng tạp hóa là một đề tài có tính ứng dụng cao và phù hợp để rèn luyện kỹ năng phân tích thiết kế, thiết kế cơ sở dữ liệu và lập trình WPF theo MVVM. Sản phẩm hiện tại đáp ứng các nghiệp vụ cốt lõi và có nền tảng rõ ràng để tiếp tục mở rộng.
