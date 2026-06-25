# Hướng Dẫn Sử Dụng Giao Diện Quản Lý Cửa Hàng

Tài liệu này dùng khi demo/test ứng dụng WPF. Trước khi chạy app, cần chạy script `database/GroceryStoreManagement.sql` trong SQL Server Management Studio và kiểm tra connection string trong `App.xaml.cs`.

## 1. Đăng Nhập

Tài khoản mẫu:

| Tài khoản | Mật khẩu | Vai trò |
| --- | --- | --- |
| `admin` | `123456` | Admin |
| `quanly` | `123456` | Quản lý |
| `kho` | `123456` | Nhân viên kho |
| `banhang` | `123456` | Nhân viên bán hàng |

Cách dùng:

1. Nhập tên đăng nhập.
2. Nhập mật khẩu.
3. Bấm **Đăng nhập**.

Nếu sai tài khoản, sai mật khẩu hoặc tài khoản bị khóa, hệ thống hiển thị lỗi ngay trên màn hình đăng nhập.

## 2. Tạo Tài Khoản

Cách dùng:

1. Ở màn đăng nhập, bấm **Tạo tài khoản**.
2. Nhập tên đăng nhập.
3. Nhập họ tên nhân viên.
4. Chọn vai trò.
5. Nhập mật khẩu và xác nhận mật khẩu.
6. Bấm **Tạo tài khoản**.

Khi tạo thành công, hệ thống tạo đồng thời `TaiKhoan` và `NhanVien` trong database.

## 3. Đổi Mật Khẩu

Cách dùng:

1. Đăng nhập vào app.
2. Bấm **Đổi mật khẩu** ở menu bên trái.
3. Nhập mật khẩu cũ.
4. Nhập mật khẩu mới.
5. Nhập xác nhận mật khẩu mới.
6. Bấm **Đổi mật khẩu**.

Hệ thống kiểm tra mật khẩu cũ trước khi lưu mật khẩu mới.

## 4. Dashboard

Chức năng:

- Xem doanh thu hôm nay.
- Xem số hóa đơn hôm nay.
- Xem số phiếu nhập trong tháng.
- Xem số sản phẩm tồn kho thấp.
- Xem số sản phẩm sắp hết hạn.

Cách test:

1. Đăng nhập bằng `admin` hoặc `quanly`.
2. Vào **Dashboard**.
3. Bấm **Tải lại** để cập nhật dữ liệu thống kê.

## 5. Sản Phẩm

Chức năng:

- Tìm kiếm sản phẩm theo mã, tên hoặc mã vạch.
- Lọc theo danh mục.
- Thêm sản phẩm mới.
- Cập nhật sản phẩm.
- Xóa mềm sản phẩm.

Cách thêm:

1. Bấm **Thêm mới**.
2. Nhập mã sản phẩm, tên sản phẩm, danh mục, đơn vị tính, giá nhập, giá bán, số lượng tồn.
3. Chọn trạng thái `DangBan`.
4. Bấm **Lưu**.

Cách sửa:

1. Chọn một dòng sản phẩm trong bảng.
2. Sửa thông tin ở form bên phải.
3. Bấm **Lưu**.

Cách xóa:

1. Chọn một dòng sản phẩm.
2. Bấm **Xóa**.
3. Hệ thống cập nhật trạng thái `DaXoa` và ẩn khỏi danh sách để không vi phạm khóa ngoại ở hóa đơn/phiếu nhập.

## 6. Danh Mục

Chức năng:

- Thêm danh mục sản phẩm.
- Sửa danh mục.
- Xóa danh mục chưa có sản phẩm.

Cách dùng:

1. Vào **Danh mục**.
2. Bấm **Thêm mới**.
3. Nhập mã danh mục, tên danh mục, mô tả.
4. Bấm **Lưu**.

Nếu danh mục đang được sản phẩm sử dụng, hệ thống không cho xóa.

## 7. Đơn Vị Tính

Chức năng:

- Thêm đơn vị tính.
- Sửa đơn vị tính.
- Xóa đơn vị tính chưa có sản phẩm.

Cách dùng:

1. Vào **Đơn vị tính**.
2. Bấm **Thêm mới**.
3. Nhập tên đơn vị, ví dụ `Thùng`, `Lốc`, `Túi`.
4. Bấm **Lưu**.

Nếu đơn vị tính đang được sản phẩm sử dụng, hệ thống không cho xóa.

## 8. Nhà Cung Cấp

Chức năng:

- Thêm nhà cung cấp.
- Sửa nhà cung cấp.
- Xóa nhà cung cấp chưa phát sinh phiếu nhập.

Cách dùng:

1. Vào **Nhà cung cấp**.
2. Bấm **Thêm mới**.
3. Nhập mã nhà cung cấp, tên, số điện thoại, email, địa chỉ.
4. Bấm **Lưu**.

Nếu nhà cung cấp đã có phiếu nhập, hệ thống không cho xóa để bảo toàn dữ liệu lịch sử.

## 9. Nhập Hàng

Chức năng:

- Chọn nhà cung cấp.
- Tìm sản phẩm.
- Nhập số lượng và đơn giá nhập.
- Thêm nhiều dòng vào phiếu nhập.
- Lưu phiếu nhập.
- Cộng tồn kho.
- Ghi lịch sử tồn kho.

Cách dùng:

1. Vào **Nhập hàng**.
2. Chọn nhà cung cấp.
3. Nhập mã sản phẩm hoặc mã vạch rồi bấm **Tìm sản phẩm**.
4. Nhập số lượng và đơn giá nhập.
5. Bấm **Thêm vào phiếu**.
6. Lặp lại nếu có nhiều sản phẩm.
7. Bấm **Lưu phiếu nhập**.

Sau khi lưu, bảng `PhieuNhap`, `ChiTietPhieuNhap`, `SanPham`, `LichSuTonKho` sẽ thay đổi trong database.

## 10. Bán Hàng

Chức năng:

- Tìm sản phẩm theo mã, tên hoặc mã vạch.
- Thêm sản phẩm vào hóa đơn.
- Kiểm tra tồn kho trước khi bán.
- Chọn khách hàng.
- Nhập giảm giá.
- Thanh toán.
- Trừ tồn kho.
- Ghi lịch sử tồn kho.

Cách dùng:

1. Vào **Bán hàng**.
2. Nhập mã sản phẩm hoặc mã vạch.
3. Bấm **Tìm**.
4. Nhập số lượng mua.
5. Bấm **Thêm sản phẩm**.
6. Chọn khách hàng nếu có.
7. Nhập giảm giá nếu có.
8. Bấm **Thanh toán**.

Sau khi thanh toán, bảng `HoaDon`, `ChiTietHoaDon`, `SanPham`, `LichSuTonKho` sẽ thay đổi trong database.

## 11. Khách Hàng

Chức năng:

- Tìm khách hàng theo mã, tên, số điện thoại.
- Thêm khách hàng.
- Sửa khách hàng.
- Xóa khách hàng chưa phát sinh hóa đơn.

Cách thêm:

1. Vào **Khách hàng**.
2. Bấm **Thêm mới**.
3. Nhập mã khách hàng, họ tên, số điện thoại, địa chỉ, điểm tích lũy.
4. Bấm **Lưu**.

Nếu khách hàng đã có hóa đơn, hệ thống không cho xóa để giữ lịch sử bán hàng.

## 12. Kiểm Kê

Chức năng:

- Tìm sản phẩm.
- Lấy số lượng tồn theo hệ thống.
- Nhập số lượng thực tế.
- Tính chênh lệch.
- Lưu phiếu kiểm kê.
- Cập nhật tồn kho.

Cách dùng:

1. Vào **Kiểm kê**.
2. Nhập mã sản phẩm hoặc mã vạch.
3. Bấm **Tìm sản phẩm**.
4. Nhập số lượng thực tế.
5. Bấm **Thêm dòng**.
6. Bấm **Lưu kiểm kê**.

## 13. Báo Cáo

Chức năng:

- Xem tổng quan doanh thu.
- Xem top sản phẩm bán chạy.
- Xem sản phẩm tồn kho thấp.
- Xem sản phẩm sắp hết hạn.

Cách dùng:

1. Vào **Báo cáo**.
2. Chọn khoảng ngày.
3. Bấm **Tải báo cáo**.
4. Chuyển tab để xem từng loại báo cáo.

## 14. Đăng Xuất

Cách dùng:

1. Bấm **Đăng xuất** ở menu bên trái.
2. Hệ thống ghi lịch sử đăng xuất.
3. Ứng dụng quay lại màn hình đăng nhập.
