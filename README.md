# 🚀 C# .NET 10 Clean Architecture & Zero Trust Authorization

Một dự án mẫu (Template/Demo) được xây dựng trên nền tảng **.NET 10**, áp dụng triệt để các tiêu chuẩn kỹ thuật phần mềm hiện đại như **Clean Architecture**, **CQRS** (Command Query Responsibility Segregation), **Domain-Driven Design (DDD)** và hệ thống phân quyền an ninh tối đa **Zero Trust Multi-Layer Authorization**. 

Dự án này là minh chứng cho việc tổ chức mã nguồn có tính mở rộng cao, dễ bảo trì, dễ dàng thực hiện Unit Test và phân tách hoàn toàn Logic nghiệp vụ khỏi các công nghệ hạ tầng (Database, UI, Security).

---

## 🏛️ Kiến Trúc Hệ Thống (Architecture)

Dự án được chia thành 4 Project vật lý (DLL) biệt lập, tuân thủ nguyên tắc Dependency Rule: **Dependencies only point inwards** (Sự phụ thuộc chỉ hướng vào lõi).

1. **Domain (DemoC#.Domain)**: Chứa các thực thể (Entities). Hoàn toàn KHÔNG phụ thuộc vào bên ngoài.
2. **Application (DemoC#.Application)**: Chứa Logic nghiệp vụ (CQRS qua MediatR), DTOs (ecord), và FluentValidation.
3. **Infrastructure (DemoC#.Infrastructure)**: EF Core Database, Zero Trust Handlers, Identity, và JWT Services.
4. **Presentation / API (DemoC#)**: Controllers, API Docs (Scalar/Swagger).

---

## 🛡️ Đường Ống Phân Quyền Zero Trust (4 Lớp)

Hệ thống API (ví dụ: tạo Bài Báo) không tin tưởng bất kỳ ai và yêu cầu vượt qua 4 lớp kiểm duyệt:
1. **Scope Check:** Đảm bảo Client App (ứng dụng bên thứ 3) được cấp quyền gọi API (
ews.write).
2. **PBAC Check:** Đọc Role từ JWT, tra cứu Database để đảm bảo User có quyền truy cập chức năng (rticles:create). Việc bóc tách này giúp tránh JWT Bloat.
3. **ABAC Check:** Đảm bảo User thỏa mãn các điều kiện động (VD: Thuộc tính department == "News").
4. **Audit Logging:** Dù Thành công hay Thất bại, mọi kết quả kiểm duyệt đều bị lưu vết xuống Database bằng AuditLogs.

---

## 🔐 Xác Thực & Quản Lý Người Dùng (Identity & User Management)

Hệ thống đã được tích hợp bộ quản lý danh tính **ASP.NET Core Identity** và **JWT (JSON Web Token)** chuẩn Enterprise:
- **Tạo User & Role:** Xây dựng đầy đủ Role Management để tránh quản lý quyền lắt nhắt. Quyền (Permissions) được cấp cho các Vai trò (Roles). Mở các API /api/roles và /api/users.
- **Lấy Token (Login):** API /api/auth/login sinh ra chuỗi JWT Token. Token này được thiết kế **siêu gọn nhẹ**, chỉ Inject sẵn UserId, Role, Scope và Department (cho ABAC). Nhờ việc không nhồi nhét hàng trăm Permissions vào Token, hệ thống tránh được lỗi JWT Bloat, trong khi đường ống Zero Trust vẫn tra cứu quyền chính xác qua RoleManager.


- **[Tài Liệu Đọc Thêm: Bóc Tách Chuyên Sâu JWT & Zero Trust](./docs/JWT_Deep_Dive.md)**

- **[Tài Liệu Đọc Thêm: Q&A Chuyên Sâu Về Token Security & Quản Lý Phiên (Session)](./docs/Q_and_A_Token_Security.md)**
