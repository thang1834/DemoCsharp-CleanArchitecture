# 🎤 Cẩm Nang Q&A: Kiến Trúc Phân Quyền Zero Trust & Clean Architecture

Tài liệu này tổng hợp các câu hỏi (từ cơ bản đến chuyên sâu) mà Hội đồng đánh giá, Giám đốc kỹ thuật (CTO), hoặc các Architect có thể đặt ra trong buổi thuyết trình của bạn. Các câu trả lời được biên soạn theo văn phong chuyên nghiệp, tập trung vào bản chất kỹ thuật và tư duy thiết kế.

---

## PHẦN 1: TƯ DUY KIẾN TRÚC (CLEAN ARCHITECTURE)

### Q1. Tại sao bạn lại đặt các Handler xác thực (Authorization) ở tầng Infrastructure và API, thay vì tầng Application?
**Trả lời:** 
Theo triết lý của Clean Architecture, tầng Application (Logic nghiệp vụ) và Domain (Lõi) phải hoàn toàn "mù" về các công nghệ giao diện như HTTP hay JWT Token. Phân quyền (Authorization) dựa trên Token là một dạng *Cross-cutting Concern* (vấn đề xuyên suốt) của hệ thống web. 
Nếu nhét logic giải mã Token vào Application layer, chúng ta sẽ vi phạm quy tắc Hướng tâm (Dependency Rule) vì bắt Application phụ thuộc vào thư viện bảo mật web. Đặt ở Infrastructure và API giúp hệ thống giữ được sự thuần khiết; tầng Application chỉ nhận vào các DTO đã được lọc sạch.

### Q2. Nếu tầng Application không biết về HTTP Token, làm sao Application biết được "Ai" (UserId) đang thực hiện hành động tạo Article để lưu vào DB?
**Trả lời:** 
Chúng ta giải quyết bằng kỹ thuật Đảo ngược Phụ thuộc (Dependency Inversion). 
1. Ở tầng `Application`, tôi định nghĩa một Interface: `ICurrentUserService` với property `string UserId`.
2. Ở tầng `API`, tôi tạo class `CurrentUserService` implement interface đó, class này sẽ tiêm `IHttpContextAccessor` để lấy `UserId` từ Token.
3. Bằng cách này, Application layer lấy được thông tin User hiện tại mà vẫn không hề dính dáng đến HTTP hay JWT.

---

## PHẦN 2: PHƯƠNG PHÁP PHÂN QUYỀN (ZERO TRUST LAYERS)

### Q3. Việc kiểm tra Scope (Lớp 1) và kiểm tra Permission / PBAC (Lớp 2) có bị trùng lặp không? Tại sao phải cần cả hai?
**Trả lời:** 
Hoàn toàn không trùng lặp, chúng phục vụ hai mục đích khác biệt nhưng bổ trợ cho nhau:
* **Scope (OAuth2)**: Xác thực mức độ ứng dụng (Client Application). Nó trả lời câu hỏi: *"Ứng dụng bên thứ 3 này có được phép đại diện người dùng để ghi dữ liệu không?"* (Ví dụ: Một App đọc tin tức sẽ chỉ được cấp scope `news.read`, dù user có là Admin).
* **PBAC (Permissions)**: Xác thực mức độ người dùng (User). Nó trả lời: *"Bản thân người dùng này có quyền tạo bài báo không?"*.
=> Zero Trust yêu cầu cả 2: Thiết bị/App phải an toàn (Scope) VÀ Người dùng phải hợp lệ (PBAC).

### Q4. Mô hình ABAC (Thuộc tính) rất mạnh, nhưng nếu mỗi Request đều phải query Database để lấy thuộc tính so sánh thì có gây nghẽn cổ chai (Bottleneck) hiệu suất không?
**Trả lời:** 
Đó là một rủi ro thực tế nếu triển khai sai. Để giải quyết, chúng ta có 2 chiến lược:
1. **Đóng gói thuộc tính tĩnh vào Token:** Các thuộc tính ít thay đổi (như `Department="News"`) được đẩy thẳng vào Claims của JWT lúc đăng nhập. ABAC Handler chỉ cần bóc Token ra kiểm tra, tốn `O(1)` thời gian, không cần chạm Database.
2. **Sử dụng Distributed Cache (Redis):** Đối với các thuộc tính động (như trạng thái tài khoản đang bị khóa hay không), thay vì Query DB liên tục, ABAC Handler sẽ lookup từ Redis Cache với tốc độ phản hồi tính bằng mili-giây.

### Q5. RBAC (Role-based) truyền thống có lỗi thời khi đã có PBAC và ABAC không?
**Trả lời:** 
RBAC không lỗi thời, nhưng nó không đủ để đứng một mình trong kiến trúc hiện đại. Trong mô hình đa lớp này, RBAC lùi về làm nhiệm vụ **Quản trị Nhóm sơ cấp**. Người quản trị hệ thống (Admin) sẽ không đi gán hàng trăm Permission cho từng User, mà họ sẽ tạo một Role "Biên tập viên", nhét các Permission (PBAC) vào Role đó, rồi gán Role cho User. Như vậy, RBAC giúp thao tác vận hành dễ dàng, còn PBAC/ABAC đảm nhiệm việc kiểm duyệt chi tiết tại mã nguồn.

---

## PHẦN 3: BẢO MẬT & ĐỐI PHÓ RỦI RO (EDGE CASES)

### Q6. Nhược điểm chí mạng của JWT Token là không thể thu hồi trước hạn (Revoke). Lỡ một nhân viên bị đuổi việc nhưng Token của anh ta vẫn còn hạn 1 tiếng thì sao? Lớp Zero Trust của bạn giải quyết thế nào?
**Trả lời:** 
Vì JWT là phi trạng thái (Stateless), việc này là một nhược điểm cố hữu. Trong mô hình này, tôi xử lý bằng sự kết hợp giữa **ABAC và Token Revocation List (Blacklist)**:
1. Đặt thời gian sống của Access Token cực ngắn (VD: 5-10 phút), dùng Refresh Token để lấy lại token mới. Khi bị đuổi việc, Refresh Token bị hủy ngay lập tức trên Server.
2. Tại lớp ABAC Handler, chúng ta thêm một logic kiểm tra Id của Token (jti claim) vào danh sách Blacklist lưu trên Redis. Nếu Admin cấm khẩn cấp, Id đó bị ném vào Blacklist. ABAC Handler đọc thấy sẽ lập tức ném lỗi 403, vượt qua được nhược điểm của JWT.

### Q7. Audit Logging (Ghi vết) là yêu cầu của Lớp 4. Nhưng việc Ghi Log bằng Entity Framework xuống DB ở mọi Request thất bại có mở ra rủi ro bị tấn công DDoS làm tràn Database không?
**Trả lời:** 
Một câu hỏi rất hay. Nếu ghi Log đồng bộ (Synchronous) bằng EF Core ở mọi Request, hệ thống rất dễ bị nghẽn (DB Connection Pool exhaustion) khi bị DDoS. 
Trong thực tế áp dụng, class `CustomAuthorizationMiddlewareResultHandler` sẽ không ghi trực tiếp xuống DB. Thay vào đó, nó sẽ bắn dữ liệu AuditLog vào một **Message Queue (như RabbitMQ, Kafka) hoặc System.Threading.Channels** dưới dạng Fire-and-Forget. Một Background Worker Service (nằm ở tầng Infrastructure) sẽ âm thầm đọc Queue đó và chèn vào Database theo từng lô (Batch Insert). Cách này giữ cho API Layer phản hồi với độ trễ gần như bằng 0.

---

## PHẦN 4: ĐẶC THÙ TRIỂN KHAI TRÊN .NET C#

### Q8. Tại sao bạn lại chọn `IAuthorizationMiddlewareResultHandler` để làm Audit mà không dùng các Action Filter (như `IAsyncActionFilter`) của ASP.NET Core?
**Trả lời:** 
* `Action Filter` chỉ được kích hoạt *sau khi* quá trình Authorization đã thành công và Request bắt đầu đi vào Controller. Nếu một Request bị từ chối bởi Scope hoặc PBAC, Action Filter sẽ không bao giờ được chạy, dẫn đến việc chúng ta bị "mù" log (không ghi vết được lý do bị chặn).
* `IAuthorizationMiddlewareResultHandler` là điểm nghẽn (Choke point) nằm ở cấp độ Middleware của ASP.NET. Nó hứng được toàn bộ kết quả của Policy `AuthorizeResult` (Dù Thành công, Bị cấm - Forbidden, hay Lỗi xác thực - Challenged). Đây là vị trí "Độc tôn" và tối ưu nhất để đặt lớp Audit cuối cùng trong Zero Trust Pipeline.
