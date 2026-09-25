# 🔐 Bóc Tách Chuyên Sâu Về JWT (JSON Web Token)

Tài liệu này cung cấp kiến thức nền tảng và giải thích chi tiết cách JWT được thiết kế, cấu trúc, các chuẩn Claims quốc tế, cũng như cách áp dụng vào hệ thống bảo mật Zero Trust của dự án `DemoC#`.

---

## 1. Bản Chất Của JWT Là Gì?

**JWT (JSON Web Token)** là một tiêu chuẩn mở (RFC 7519) định nghĩa một cách nhỏ gọn và độc lập để truyền thông tin an toàn giữa các bên dưới dạng một đối tượng JSON. Thông tin này có thể được xác minh và tin tưởng vì nó được ký (Signed) bằng thuật toán mã hóa (VD: HMAC, RSA).

*Ví dụ đời thực:* JWT giống như một chiếc **"Thẻ Căn Cước Công Dân có gắn chip"**. 
- Bạn không thể tự in ra (vì không có chữ ký của nhà nước).
- Khi bạn cầm thẻ này đi qua cổng an ninh, bảo vệ chỉ cần quét chữ ký điện tử là biết ngay thẻ thật hay giả, tên bạn là gì... mà **không cần gọi điện về trụ sở (Database) để hỏi**. 
- Tính năng này gọi là **Stateless** (Không lưu trạng thái), giúp Server xử lý hàng triệu request mà không bị nghẽn Database.

### Cấu Trúc Của Một JWT
Một chuỗi JWT trông giống như thế này: `xxxxx.yyyyy.zzzzz` (Gồm 3 phần ngăn cách bởi dấu chấm).

1. **Header (`xxxxx`):** Chứa thông tin về loại Token (JWT) và thuật toán ký (VD: `HS256`).
2. **Payload (`yyyyy`):** Chứa các thông tin thực sự cần truyền tải (được gọi là **Claims**).
3. **Signature (`zzzzz`):** Chữ ký bảo mật. Nếu ai đó cố tình sửa Payload, chữ ký sẽ bị sai lệch và Server lập tức từ chối.

---

## 2. Giải Phẫu JWT Payload & Phân Loại Claims (Chuẩn RFC 7519)

Dựa theo tiêu chuẩn quốc tế RFC 7519, Payload của JWT được cấu tạo từ các **Claims** (Lớp xác nhận thông tin) và được chia thành 3 nhóm chính. Dưới đây là cách dự án `DemoC#` áp dụng chúng vào thực tế:

### 2.1. Registered Claims (Claims tiêu chuẩn đã đăng ký)
Đây là tập hợp các claims có tên viết tắt (thường là 3 chữ cái) được quốc tế quy chuẩn. Chúng giúp các hệ thống khác nhau dễ dàng tương thích.
- **`sub` (Subject):** Chủ thể của JWT. Trong dự án, nó lưu **UserId**. Nó cho biết "Người cầm thẻ này là ai".
- **`exp` (Expiration Time):** Thời hạn sống của Token. Lớp Middleware Authentication của ASP.NET Core sẽ tự động quét trường này, nếu quá hạn, nó báo lỗi 401 ngay lập tức.
- **`iss` (Issuer) & `aud` (Audience):** Nơi phát hành (Server Auth của chúng ta) và Đối tượng nhận (Client App).
- **`iat` (Issued At) & `jti` (JWT ID):** Thời điểm tạo và Mã định danh duy nhất của Token (Dùng để chống tấn công Replay Attack).

### 2.2. Public Claims (Claims công khai)
Đây là các claims do lập trình viên định nghĩa nhưng để chia sẻ công khai giữa các tổ chức (Ví dụ: Facebook chia sẻ cho Tiki). Để tránh xung đột tên (Collision), người ta phải đăng ký qua IANA hoặc dùng định dạng URI (VD: `https://mycompany.com/claims/department`).
- *Trong dự án `DemoC#`:* Do hệ thống là nội bộ, chúng ta không dùng URI phức tạp. Tuy nhiên, claim **`scope`** (Phạm vi: `news.write`) kế thừa từ chuẩn OAuth2 có thể xem là một dạng Public Claim phổ biến. Nó giới hạn quyền hạn của **Ứng dụng Client**, độc lập hoàn toàn với quyền của User.

### 2.3. Private Claims (Claims nội bộ / riêng tư)
Đây là "trái tim" của kiến trúc Zero Trust trong dự án. Private Claims là sự ngầm hiểu nội bộ giữa Auth Service và API Controllers của chúng ta.
- **`role` (Vai trò):** VD: `"Writer"`. Thay vì nhét hàng trăm quyền (`articles:create`, `articles:edit`) vào JWT khiến Token phình to, ta chỉ nhét `role`. Server sẽ lấy `role` này chọc xuống Database để query ra quyền thực sự (Giải quyết triệt để lỗi JWT Bloat).
- **`department` (Phòng ban):** VD: `"News"`. Đây là ngữ cảnh (Context) để chạy Lớp bảo mật ABAC (Lớp 3). Việc lưu sẵn `department` vào Private Claim giúp Server đánh giá ngay trên RAM xem User có được quyền sửa bài báo của phòng "News" hay không mà không cần hỏi lại Database.

---

## 3. Quyết Định Thiết Kế: Vấn Đề "JWT Bloat"

Nhiều hệ thống mắc sai lầm khi nhét toàn bộ mảng `permissions` (quyền hạn) vào Private Claims của JWT. 

**Tại sao chúng ta KHÔNG làm thế?**
- Nếu một User có 500 quyền khác nhau, JWT sẽ phình to khổng lồ. 
- Các Web Server (Nginx, IIS) thường giới hạn kích thước HTTP Header (~8KB). Nếu JWT quá to, Request bị lỗi `431 Request Header Fields Too Large` và đánh sập hệ thống.
- **Giải pháp:** Áp dụng phương pháp Lai (Hybrid). Token chỉ chứa những thứ thật sự cốt lõi và nhỏ gọn. Riêng `permissions` khổng lồ sẽ được ánh xạ động (mapping) thông qua bảng `AspNetRoleClaims` ở Database bằng `RoleManager`.

---

## 4. Vòng Đời Của JWT Trong Luồng Zero Trust

1. **Đăng nhập (Login):** Server kiểm tra Mật khẩu, nếu đúng sẽ lấy `sub`, `department`, `role` nạp vào Payload. Dùng Secret Key băm ra Chữ ký.
2. **Gửi Token (Bearer):** Client nhận Token, nhét vào Header `Authorization: Bearer <token>` mỗi khi gọi API.
3. **Xác thực (Authentication):** Middleware kiểm tra Chữ ký, đối chiếu Registered Claims (như `exp`, `iss`). Nếu hợp lệ, chuyển Payload thành mảng in-memory.
4. **Phân quyền (Zero Trust):** 
   - Lớp 1 (Scope): Kiểm tra Public Claim `scope`.
   - Lớp 2 (PBAC): Đọc Private Claim `role` đi check DB.
   - Lớp 3 (ABAC): Đọc Private Claim `department` đối chiếu với data bài viết.
5. **Thành công:** Trả về HTTP 200/201.
