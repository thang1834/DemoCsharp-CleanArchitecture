# Changelog

Tất cả những thay đổi nổi bật của dự án sẽ được ghi chú tại đây.

## [2026-09-25] - Nâng cấp Bảo mật: Refresh Token & HttpOnly Cookie
### 🛡️ Những Thay Đổi Về Mặt Kỹ Thuật (Chốt hạ Zero Trust)
- **Thu gọn tuổi thọ Token:** Access Token (`JwtTokenGenerator`) giờ đây chỉ sống trong **15 phút**. Tăng cường bảo mật với cấu hình `ValidateLifetime = true` và độ trễ `ClockSkew = TimeSpan.Zero` trong Program.
- **Cơ chế Refresh Token an toàn:** Khởi tạo bảng `UserRefreshTokens`. API Login tạo Refresh Token 64-byte CSPRNG siêu ngẫu nhiên lưu xuống Database.
- **Phòng chống XSS & CSRF Tuyệt đối:** API Login trả Refresh Token thẳng vào **HttpOnly, Secure, SameSite=Strict Cookie** (Client không thể đọc bằng JS). Access Token (15 phút) vẫn được trả về qua JSON Body.
- **Quản lý đa thiết bị:** 
  - `POST /api/auth/refresh-token`: Đọc ngầm Cookie, luân chuyển Token tự động.
  - `POST /api/auth/revoke-token`: Hỗ trợ "đá" thiết bị khác bằng cách đánh dấu Revoked qua `DeviceId`, tích hợp lệnh xóa Cookie triệt để từ Server.
- **Kiểm định:** Bộ Automation Test (xUnit) được nâng cấp lên 19 bài, cover toàn bộ kịch bản vòng đời Token.

## [2026-09-25] - Nâng cấp Authentication & Role Management
### 🚀 Những Thay Đổi Về Mặt Kỹ Thuật
#### 1. Khắc Phục JWT Bloat & Nâng cấp Lớp 2 (PBAC)
- Token giờ đây chỉ chứa role, sub, scope, department (rất mỏng nhẹ).
- **PBAC Handler:** Đọc role từ JWT, sau đó dùng RoleManager truy vấn xuống DB để xác thực permission.
#### 2. Thêm Module Role Management & User Management
- Hỗ trợ tạo Role, gán Permission vào Role, và gán User vào Role.

## [2026-09-24] - Tích hợp Zero Trust & CQRS
- Xây dựng 4 lớp bảo mật (Scope, PBAC, ABAC, Audit Log).
- Chuyển đổi DTOs sang record.
- Đổi từ Product sang Categories & Articles.
