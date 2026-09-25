# 🎓 Bài Giảng Chuyên Sâu: Các Mô Hình Phân Quyền Trong ASP.NET Core
*Biên soạn: Giáo sư Antigravity*

Chào các bạn sinh viên và các kỹ sư tương lai. Đứng trước bài toán bảo mật của một hệ thống doanh nghiệp (Enterprise), việc chọn đúng mô hình phân quyền (Authorization) mang tính quyết định sự sống còn của dự án. Hôm nay, tôi sẽ giúp các bạn bóc tách sâu vào 4 phương pháp phân quyền phổ biến nhất. Đi kèm là Demo code, phân tích ưu/nhược điểm và bảng so sánh trực diện.

---

## 1. RBAC (Role-Based Access Control) - Phân quyền theo Vai trò
RBAC là mô hình kinh điển nhất. Chúng ta gom người dùng vào các nhóm (Role) như `Admin`, `User`, `Manager`. Quyền truy cập được cấp cho tập thể Role thay vì từng cá nhân.

### 💻 Demo Code
```csharp
// Đơn giản chỉ cần gắn Attribute lên Controller hoặc Action
[Authorize(Roles = "Admin, Manager")]
[HttpPost("create-article")]
public IActionResult CreateArticle() 
{ 
    return Ok("Thêm bài viết thành công!");
}
```

### ⚖️ Đánh giá
* **Ưu điểm:** Cực kỳ dễ code, dễ hiểu. Hoàn hảo cho các dự án nhỏ, nội bộ, ít biến động.
* **Nhược điểm (Bệnh "Role Explosion"):** Khi hệ thống lớn, bạn sẽ sinh ra những Role quái thai như `Admin_Nhưng_Không_Được_Xóa_Bài`. Lúc này số lượng Role bùng nổ không kiểm soát, code bị hardcode cứng ngắc.

---

## 2. PBAC (Permission-Based Access Control) - Phân quyền theo Hành động
Khắc phục RBAC, ta bẻ nhỏ hệ thống thành các Quyền (Permission) như `articles:create`, `articles:delete`. Bạn có Permission nào thì làm được việc đó, bất kể bạn là sếp hay nhân viên.

### 💻 Demo Code
```csharp
// 1. Khai báo Policy ở Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanCreateArticle", policy => 
        policy.RequireClaim("permission", "articles:create"));
});

// 2. Sử dụng ở Controller (Không cần quan tâm tới Role nữa)
[Authorize(Policy = "CanCreateArticle")]
[HttpPost("create-article")]
public IActionResult CreateArticle() { ... }
```

### ⚖️ Đánh giá
* **Ưu điểm:** Rất chi tiết (Granular). Dễ dàng tái cấu trúc API. Controller tách biệt hoàn toàn khỏi khái niệm "Vai trò" rườm rà.
* **Nhược điểm:** Vẫn thiếu **Ngữ cảnh (Context)**. Bạn có quyền sửa bài, nhưng có được sửa bài của *người khác* không? PBAC bó tay ở điểm này.

---

## 3. Dynamic Filter theo Controller/Action (Truyền thống/CMS)
Đây là cách các hệ thống CMS cũ thường làm: Viết một Filter chặn mọi Request, lấy tên `ControllerName` và `ActionName`, rồi chọc xuống Database xem User này có được Admin tick chọn quyền hay không.

### 💻 Demo Code
```csharp
public class PermissionAuthorizationFilter : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // 1. Lấy thông tin Controller & Action từ Route
        var controller = context.ActionDescriptor.RouteValues["controller"];
        var action = context.ActionDescriptor.RouteValues["action"];
        
        // 2. Query xuống Database kiểm tra quyền
        bool hasPermission = await _dbContext.CheckUserPermission(userId, controller, action);
        
        // 3. Chặn nếu không có quyền
        if (!hasPermission) context.Result = new ForbidResult();
    }
}
```

### ⚖️ Đánh giá
* **Ưu điểm:** Cực kỳ linh hoạt trên giao diện (Dynamic). Admin có một màn hình quản lý chằng chịt các checkbox để cấp quyền cho từng Controller/Action mà không cần build lại code.
* **Nhược điểm Chí Mạng:**
  * **Code cực kỳ mỏng manh (Fragile):** Bạn vui tay đổi tên Controller/Action phát là vỡ toàn bộ cấu hình dưới Database (lỗi 403 hàng loạt).
  * **Hiệu năng thảm họa (Performance Bottleneck):** Request nào gọi tới cũng phải chọc xuống Database query quyền.
  * **Thiếu ngữ cảnh (ABAC blindness):** Tương tự PBAC, không thể kiểm tra được *"Sửa bài của phòng ban nào?"* ngay tại Filter chung này.

---

## 4. Kiến Trúc Zero Trust 4 Lớp (Triết lý của chúng ta)
Để giải quyết tất cả nhược điểm trên, kiến trúc của dự án DemoC# áp dụng tư duy Zero Trust (Không tin tưởng bất kỳ ai), thiết lập một "Đường ống" (Pipeline) gồm 4 màng lọc thép ngay tại tầng Infrastructure (Kết hợp cả Scope, PBAC và ABAC).

### 💻 Demo Cấu Trúc Lớp (Pipeline)
1. **Lớp 1 (Scope Check):** Client App (Postman/Mobile) phải có chuỗi `news.write`.
2. **Lớp 2 (PBAC Check):** Bóc `Role` từ Token, đối chiếu xuống Database để xác nhận Role đó có chứa permission `articles:create` hay không.
3. **Lớp 3 (ABAC Check):** So khớp động: Department của User phải giống với phòng ban được phép cấu hình.
4. **Lớp 4 (Audit Log):** Dù đi tiếp hay bị chặn, một Middleware ngầm luôn lưu vết (Log) xuống Database để làm bằng chứng.

### 💻 Demo Code
```csharp
// 1. Controller Cực Mỏng, chỉ gắn 1 Policy duy nhất
[Authorize(Policy = "ZeroTrust_ArticleCreatePolicy")]
[HttpPost]
public IActionResult CreateArticle() { ... }

// 2. Lớp 3 (ABAC Handler) - Xử lý nghiệp vụ khó (Ví dụ: Kiểm tra phòng ban)
protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ArticleAbacRequirement req)
{
    // Đọc department từ JWT Token (Nhanh như điện, không cần DB)
    var userDept = context.User.FindFirst("department")?.Value;
    if (userDept == req.RequiredDepartment) 
    {
        context.Succeed(req); // Vượt ải!
    }
    return Task.CompletedTask;
}
```

### ⚖️ Đánh giá
* **Ưu điểm Tuyệt đối:**
  * **Bảo mật tuyệt đỉnh:** Chặn đứng cả nội gián lấy cắp Token nhưng dùng sai App (Lớp 1) hoặc trái phòng ban (Lớp 3).
  * **Tối ưu Token & Tránh JWT Bloat:** Quyền hạn (Permission) được ánh xạ từ DB thông qua Role ở Lớp 2. Token chỉ chứa những Claims thật sự gọn nhẹ (`sub`, `department`, `role`), giữ HTTP Header siêu nhỏ gọn.
  * **Chống gãy code:** Phân quyền theo chuỗi string Permission cố định (`articles:create`), đổi tên Controller thoải mái.
  * **Dễ bảo trì (Single Responsibility):** Logic khóa cửa gom hết vào các `Handler`. Controller sạch bóng không có 1 lệnh `if(user.role == ...)` nào.
* **Nhược điểm:** Đòi hỏi kiến trúc (Boilerplate) dài và người lập trình phải hiểu sâu về ASP.NET Core Authorization.

---
## 📊 Lời Khuyên Của Giáo Sư (Bảng So Sánh)

| Tiêu Chí | RBAC | PBAC | Controller/Action DB Filter | Zero Trust 4 Lớp (Chúng ta) |
| :--- | :--- | :--- | :--- | :--- |
| **Độ chi tiết** | Thấp | Cao | Trung Bình - Cao | Rất Cao |
| **Hiệu năng** | Cao | Cao | Thấp (Query DB liên tục) | Cao (In-memory Claims) |
| **Bền vững khi Refactor**| Cao | Cao | **Rất Kém** (Vỡ khi đổi tên) | Cao |
| **Bối cảnh động (ABAC)** | Không | Không | Không | CÓ (Kiểm tra Department) |
| **Kiểm soát App (Scope)** | Không | Không | Không | CÓ |
| **Độ phức tạp code** | Thấp | Trung Bình | Trung Bình | Cao (Đáng giá từng đồng) |

**Tóm lại:** Nếu bạn làm bài tập lớn, RBAC là đủ. Nếu làm CMS admin cổ điển, Controller Filter là một sự lựa chọn chấp nhận được. Nhưng để hệ thống đạt chuẩn Enterprise, bảo mật cho ngân hàng hoặc viễn thông, **Zero Trust 4 Lớp** là con đường chân lý duy nhất!
