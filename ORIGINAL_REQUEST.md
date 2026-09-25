# ORIGINAL REQUEST & FEATURE TRACKING

Tài liệu này lưu trữ các yêu cầu gốc của User và tiến độ tích hợp vào DemoC# API.

## Lần 1: Refactor Clean Architecture & Zero Trust
- Xóa bỏ hệ thống Product cũ.
- Viết lại Categories, Articles theo chuẩn Clean Architecture và CQRS (MediatR).
- Xây dựng hệ thống bảo mật Zero Trust đa lớp:
  1. Scope (OAuth)
  2. PBAC (Permissions)
  3. ABAC (Attributes/Department)
  4. Audit Log (Ghi vết thành công/thất bại).

## Lần 2: Tích hợp User Management (Authentication)
- Khởi tạo Identity & User Management để cấp Token test thực tế.
- Tạo API /api/Auth/register và /api/Auth/login.
- Nhúng (Inject) 4 claims (permission, department, sub, scope) vào chuỗi JWT.
- Cung cấp bài giảng chuyên sâu Q&A so sánh mô hình Zero Trust với IAsyncAuthorizationFilter truyền thống.
