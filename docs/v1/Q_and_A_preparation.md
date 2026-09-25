# TÀI LIỆU CHUẨN BỊ Q&A (HỎI ĐÁP THUYẾT TRÌNH)

Dưới đây là danh sách những câu hỏi "kinh điển" và khó nhằn nhất mà các các anh chị senior thường xuyên dùng để "thử thách" lập trình viên khi thuyết trình về Clean Architecture và CQRS. Kèm theo đó là câu trả lời mẫu mang phong cách tự tin, hiểu sâu bản chất.

---

### Câu hỏi 1 (CQRS & MediatR): 
**Người hỏi:** *"Tại sao em phải dùng thư viện MediatR cho phức tạp? Controller gọi thẳng vào Interface của Service (như `IProductService`) không phải là nhanh và đơn giản hơn sao?"*

**Trả lời:** 
"Dạ đúng là gọi thẳng Service thì code nhanh hơn ở giai đoạn đầu. Nhưng khi dự án lớn lên, Controller của em sẽ bị 'phình to' vì phải Inject (tiêm) quá nhiều Service khác nhau (ProductService, CategoryService, LoggingService...).
Khi em dùng **MediatR**, Controller của em bị mù hoàn toàn (blind), nó không cần biết ai xử lý, nó chỉ việc ném ra một cái `Command`. Điều này giúp hệ thống đạt được sự **Loosely Coupled (Liên kết lỏng lẻo)** tuyệt đối. Hơn nữa, nhờ MediatR em mới có thể chèn được một cái 'Màng lọc' (Pipeline Behavior) vào giữa để tự động Validation mà không cần phải viết code kiểm tra lỗi ở mọi nơi ạ."

---

### Câu hỏi 2 (Repository Pattern): 
**Người hỏi:** *"anh/chị thấy em xài Clean Architecture, nhưng tại sao lại không có Repository Pattern (`IProductRepository`)? Em gọi thẳng `IApplicationDbContext` ở tầng Application như thế là sai kiến trúc rồi!"*

**Trả lời:**
"Dạ không sai ạ. Bản chất của Entity Framework Core (`DbContext` và `DbSet`) sinh ra đã chính là một Repository Pattern và Unit of Work rồi. Việc tạo thêm một lớp bọc `IProductRepository` bên ngoài EF Core (Generic Repository) bị cộng đồng thế giới đánh giá là một 'Anti-pattern' (thực hành xấu) vì nó làm code cồng kềnh, sinh ra quá nhiều file thừa mà không mang lại giá trị thực tế. 
Thay vào đó, em trừu tượng hóa toàn bộ Database bằng giao diện `IApplicationDbContext`. Tầng Application của em gọi vào Interface này, hoàn toàn không phụ thuộc vào EF Core nên vẫn đảm bảo tính đúng đắn 100% của Clean Architecture ạ."

---

### Câu hỏi 3 (Domain-Driven Design):
**Người hỏi:** *"Trong tầng Domain của em, sự khác nhau giữa **Entity** và **Value Object** là gì?"*

**Trả lời:**
"Dạ, **Entity (Thực thể)** là một đối tượng được định danh bằng **ID** (Ví dụ 2 Sản phẩm có cùng tên, cùng giá nhưng ID = 1 và ID = 2 thì nó là 2 sản phẩm hoàn toàn khác nhau).
Còn **Value Object (Đối tượng giá trị)** thì không có ID, nó được định danh bằng **chính giá trị của nó**. (Ví dụ tờ tiền 500k của em và tờ 500k của anh/chị là hoàn toàn tương đương nhau và có thể tráo đổi cho nhau).
Trong code của em, class `Product` là Entity, còn kiểu tiền tệ `Money` là Value Object. Value Object của em mang tính Bất biến (Immutable) - tạo ra xong là không được sửa, muốn sửa phải tạo cái mới ạ."

---

### Câu hỏi 4 (Xử lý lỗi - Exception):
**Người hỏi:** *"Tại sao ở hàm `Product.Create()`, khi giá tiền âm em lại ném ra `ArgumentException` làm sập chương trình? Sao không trả về `false` hay `null` cho an toàn?"*

**Trả lời:**
"Dạ theo lý thuyết Domain-Driven Design, Entity luôn phải bảo vệ tính toàn vẹn của nó. Việc truyền giá âm vào sản phẩm là một trạng thái 'bất hợp pháp', do đó ném Exception là cách phản ứng đúng đắn nhất của code.
Tuy nhiên, chương trình của em **không hề bị sập**. Em đã lường trước điều này nên em thiết kế một cái `GlobalExceptionHandler` (Middleware) ở tầng vỏ ngoài cùng (API). Khi Entity ném Exception ra, cái phễu Middleware này sẽ tự động tóm lấy nó, triệt tiêu lỗi sập, và gói gém nó thành một mã lỗi HTTP 400 Bad Request định dạng JSON trả về cho Frontend một cách cực kỳ êm ái ạ."

---

### Câu hỏi 5 (Tính thực tế):
**Người hỏi:** *"Em khen Clean Architecture nhiều thế, vậy nhược điểm của nó là gì? Lúc nào thì KHÔNG NÊN dùng?"*

**Trả lời:**
"Dạ nhược điểm lớn nhất của nó là **Over-engineering (Thiết kế thái quá)** và đẻ ra quá nhiều file (Boilerplate code). Đường cong học tập (Learning curve) cực kỳ gắt. 
Nếu sếp yêu cầu em làm một cái web CRUD bán hàng nhỏ gọn chạy xong trong 1 tuần, em chắc chắn sẽ KHÔNG dùng kiến trúc này mà dùng MVC hoặc N-Layer 3 lớp thông thường cho nhanh. Em chỉ dùng Clean Architecture cho các dự án Enterprise, nghiệp vụ cực kỳ phức tạp và cần vòng đời bảo trì lên tới 5-10 năm ạ!"

---

### Câu hỏi 6 (CQRS cấp độ Database):
**Người hỏi:** *"CQRS tách biệt Command (Ghi) và Query (Đọc). Vậy tại sao em vẫn dùng chung một Database PostgreSQL cho cả Ghi và Đọc?"*

**Trả lời:**
"Dạ, CQRS chia làm 2 cấp độ: Cấp độ Code và Cấp độ Database. 
Hiện tại dự án của em đang áp dụng **CQRS ở cấp độ Code** (Tách class `Command` và class `Query` riêng biệt). Điều này giúp code cực kỳ dễ quản lý.
Còn ở **Cấp độ Database** (Tách hẳn 1 Database chuyên Ghi, 1 Database chuyên Đọc) thì chỉ áp dụng cho các hệ thống siêu lớn có lượng Traffic khổng lồ (như Shopee). dự án của em chưa có lưu lượng truy cập mức độ đó nên việc tách DB là lãng phí tài nguyên và làm khó việc đồng bộ dữ liệu. Tư duy của em là: Cấu trúc code trước, khi nào hệ thống quá tải thì mới tách DB sau ạ."

---

### Câu hỏi 7 (Khái niệm CQRS):
**Người hỏi:** *"Em nhắc nhiều đến CQRS, vậy giải thích ngắn gọn CQRS là gì?"*

**Trả lời:**
"Dạ CQRS viết tắt của *Command Query Responsibility Segregation* (Tách biệt Trách nhiệm Lệnh và Truy vấn). 
Thay vì dùng 1 Service xử lý cả đọc và ghi (như `IProductService` có cả hàm `Add` và `GetAll`), CQRS bắt buộc chẻ đôi luồng dữ liệu ra:
- **Command (Lệnh):** Chuyên làm thay đổi trạng thái hệ thống (Thêm, Sửa, Xóa).
- **Query (Truy vấn):** Chuyên đọc dữ liệu ra, tuyệt đối không làm thay đổi hệ thống.
Việc tách biệt này giúp em dễ dàng tối ưu hóa riêng rẽ: Ví dụ hàm lấy danh sách (Query) cần chạy nhanh em có thể bỏ tracking của EF Core (`AsNoTracking()`), còn hàm thêm mới (Command) em sẽ áp dụng validation nghiêm ngặt."

---

### Câu hỏi 8 (Khái niệm DDD):
**Người hỏi:** *"Domain-Driven Design (DDD) mà em áp dụng trong tầng Domain là gì?"*

**Trả lời:**
"Dạ DDD là *Thiết kế hướng Dòng nghiệp vụ*. Thay vì code xoay quanh các bảng trong Database (Data-driven) như cách làm truyền thống, DDD bắt buộc code phải xoay quanh **Nghiệp vụ thực tế**.
Trong DDD, các class không chỉ là 'những cái túi rỗng' (Anemic Model) chứa getter/setter để Entity Framework lưu xuống DB, mà nó phải chứa cả quy tắc kinh doanh (Business Logic). Ví dụ, class `Product` của em tự quyết định việc nó có được phép tạo ra hay không dựa vào hàm `Create()`, chứ không phó mặc việc đó cho tầng Service ở ngoài ạ."

---

### Câu hỏi 9 (Khái niệm MediatR):
**Người hỏi:** *"MediatR là thư viện gì? Bản chất nó hoạt động như thế nào?"*

**Trả lời:**
"Dạ MediatR là một thư viện của .NET triển khai mẫu thiết kế **Mediator Pattern** (Người trung gian).
Bản chất của nó giống như một tổng đài bưu điện. Controller (Người gửi) tạo ra một lá thư (`Command`/`Query`) và gửi cho MediatR. MediatR sẽ tự động dò tìm xem ai là người có khả năng xử lý lá thư đó (`Handler`) và chuyển phát đến đúng nơi. Nhờ vậy, Người gửi và Người nhận không cần biết mặt nhau (Decoupled), giúp code không bị chằng chịt các Dependency Injection."

---

### Câu hỏi 10 (Tính đóng gói - Encapsulation):
**Người hỏi:** *"Tại sao các thuộc tính trong Entity `Product` (như Name, Price, Quantity) em lại để là `public get` nhưng lại `private set`?"*

**Trả lời:**
"Dạ đây chính là nguyên tắc Đóng gói (Encapsulation) cốt lõi của OOP và DDD ạ. 
Nếu em để `public set`, bất kỳ ai ở bên ngoài (ví dụ 1 anh Coder mới vào team) cũng có thể viết `product.Price = -500;`. Dữ liệu sẽ bị hỏng ngay lập tức.
Khi em để `private set`, không ai ở ngoài có quyền gán giá trị trực tiếp. Muốn thay đổi dữ liệu, họ bắt buộc phải gọi thông qua các hàm có ý nghĩa nghiệp vụ rõ ràng (như `product.UpdateDetails()`). Bên trong hàm đó, em đã giăng sẵn các bẫy lỗi (`if price < 0 throw Exception`). Nhờ vậy, Entity của em luôn ở trong trạng thái hợp lệ 100%!"

---

### Câu hỏi 11 (Khái niệm Loosely Coupled):
**Người hỏi:** *"Em hay dùng từ Loosely Coupled (Liên kết lỏng lẻo). Vậy cụ thể nó mang lại lợi ích gì cho code của em?"*

**Trả lời:**
"Dạ, Loosely Coupled nghĩa là các thành phần (class/module) trong hệ thống làm việc với nhau nhưng **ít phụ thuộc vào nhau nhất có thể**. 
Lợi ích lớn nhất là khi em thay đổi hoặc đập bỏ một module, các module khác không bị vỡ (break). Ví dụ trong dự án của em, Controller gọi nghiệp vụ thông qua MediatR chứ không gọi thẳng Class. Database được gọi qua Interface thay vì Entity Framework. Nhờ 'lỏng lẻo' như vậy, ngày mai sếp yêu cầu đổi Database từ PostgreSQL sang SQL Server, em chỉ cần đổi đúng 1 dòng config mà không phải sửa bất kỳ dòng code logic nào ạ."

---

### Câu hỏi 12 (Dependency Inversion - Chữ D trong SOLID):
**Người hỏi:** *"Nguyên lý Đảo ngược phụ thuộc (Dependency Inversion) được thể hiện ở chỗ nào trong kiến trúc này?"*

**Trả lời:**
"Dạ, trong kiến trúc 3 lớp cũ, tầng Logic luôn phải phụ thuộc vào tầng Database (gọi trực tiếp DbContext).
Nhưng trong Clean Architecture, em đã **đảo ngược** điều đó. Tầng Application (Lõi) của em tự định nghĩa ra một cái hợp đồng là `IApplicationDbContext`. Còn tầng Infrastructure (Vỏ ngoài) buộc phải 'chui' vào kế thừa và tuân thủ cái hợp đồng đó. 
Như vậy, Database giờ đây đang phải phụ thuộc ngược lại vào Lõi Nghiệp vụ. Lõi của em hoàn toàn 'bất khả xâm phạm' và không quan tâm thế giới bên ngoài dùng công nghệ gì ạ."

---

### Câu hỏi 13 (Pipeline Behavior vs Middleware):
**Người hỏi:** *"Pipeline Behavior của MediatR mà em dùng để Validation thực chất là gì? Tại sao em không dùng luôn Middleware của ASP.NET Core cho tiện?"*

**Trả lời:**
"Dạ Pipeline Behavior hoạt động giống y hệt Middleware, nhưng nó nằm ở tầng **Application**, còn Middleware của ASP.NET Core nằm ở tầng **API (Presentation)**.
Nếu em dùng Middleware của API, em chỉ chặn được luồng HTTP Web Request. Nhưng giả sử ngày mai team em viết thêm chức năng nhập dữ liệu từ File Excel (Background Worker) hoặc dùng gRPC, thì Middleware HTTP sẽ bị vô hiệu hóa.
Việc em đẩy Validation vào **Pipeline Behavior** giúp tạo ra một 'trạm kiểm duyệt' bắt trực tiếp vào các Command. Dù Request đến từ Web, Mobile, hay Background Job, cứ đụng vào Command là sẽ bị validate. Điều này đảm bảo an toàn tuyệt đối cho hệ thống ạ!"

---

### Câu hỏi 14 (Khái niệm SOLID):
**Người hỏi:** *"Em giải thích ngắn gọn nguyên lý SOLID là gì, và nó được áp dụng cụ thể như thế nào trong dự án của em?"*

**Trả lời:**
"Dạ SOLID là bộ 5 nguyên lý thiết kế hướng đối tượng giúp code linh hoạt và dễ bảo trì. Dự án Clean Architecture của em chính là một biểu hiện hoàn hảo của cả 5 nguyên lý này:
1. **S - Single Responsibility (Đơn trách nhiệm):** Thay vì viết một `ProductService` khổng lồ ôm đồm mọi thứ, CQRS giúp em chẻ nhỏ thành các file độc lập. `AddProductCommand` chỉ lo việc thêm, `GetProductsQuery` chỉ lo việc đọc.
2. **O - Open/Closed (Mở để mở rộng, Đóng để sửa đổi):** Nhờ cơ chế Pipeline Behavior của MediatR, nếu sau này cần thêm tính năng Ghi log (Logging) hoặc Caching, em chỉ cần viết thêm một Behavior mới cắm vào Pipeline, hoàn toàn không phải sửa lại code cũ của Controller hay Handler.
3. **L - Liskov Substitution (Thay thế Liskov):** Mọi Handler trong hệ thống của em đều tuân thủ đúng hợp đồng của giao diện `IRequestHandler`. MediatR có thể triệu gọi bất kỳ Handler nào thay thế cho nhau mà không làm hỏng tính đúng đắn của chương trình.
4. **I - Interface Segregation (Phân tách Giao diện):** Em không dùng một Interface phình to như `IRepository` chứa cả chục hàm không dùng tới. CQRS giúp mỗi tính năng có một Request Interface cực kỳ nhỏ gọn và tập trung.
5. **D - Dependency Inversion (Đảo ngược phụ thuộc):** Tầng Application cốt lõi không gọi thẳng vào Entity Framework. Nó định nghĩa ra `IApplicationDbContext`. Thằng EF Core ở ngoài cùng buộc phải chui vào implement Interface này, giúp lõi nghiệp vụ miễn nhiễm với các thay đổi về công nghệ DB ạ."

---

### Câu hỏi 15 (Vấn đề Hiệu năng - Performance):
**Người hỏi:** *"Kiến trúc của em chẻ ra quá nhiều tầng, lại dùng MediatR gọi qua trung gian (Reflection). Điều này có làm cho ứng dụng chạy chậm hơn so với code gộp 1 cục không?"*

**Trả lời:**
"Dạ chắc chắn là có độ trễ (overhead) so với việc gọi hàm trực tiếp, nhưng độ trễ này chỉ rơi vào khoảng vài mili-giây, cực kỳ nhỏ và không thể cảm nhận được ở cấp độ người dùng. 
Đổi lại, kiến trúc này giúp code rất dễ bảo trì. Hơn nữa, việc áp dụng CQRS thực chất lại giúp tối ưu hiệu năng tổng thể: Ở các luồng Query (truy vấn), em hoàn toàn có thể bỏ qua Tracking của EF Core (`AsNoTracking`), hoặc thậm chí viết truy vấn thẳng bằng thư viện Dapper (Raw SQL) để đạt tốc độ phản hồi tính bằng micro-giây mà không làm ảnh hưởng đến luồng Command (Ghi) ạ."

---

### Câu hỏi 16 (Bảo mật & DTO):
**Người hỏi:** *"Tại sao em phải mất công tạo các class DTO (`CreateProductDto`, `ProductResponseDto`)? Tại sao không dùng luôn class Entity `Product` để nhận Request và trả Response cho nhanh?"*

**Trả lời:**
"Dạ nếu trả thẳng Entity ra ngoài thì sẽ vi phạm nguyên tắc bảo mật, dễ bị tấn công kiểu **Over-posting (Mass Assignment)**. 
Ví dụ Entity User có thuộc tính `IsAdmin = false`. Nếu dùng luôn Entity để nhận dữ liệu, một hacker có thể chủ động truyền thêm trường `"isAdmin": true` vào JSON, và EF Core sẽ lưu thẳng nó xuống Database.
DTO (Data Transfer Object) đóng vai trò như một cái 'cái màng lọc' hoặc 'áo giáp', nó chỉ cho phép hứng và trả về đúng những trường dữ liệu mà mình muốn phơi bày cho Frontend thôi ạ."

---

### Câu hỏi 17 (FluentValidation vs DataAnnotations):
**Người hỏi:** *"Trong .NET có sẵn DataAnnotations (`[Required]`, `[MaxLength]`) viết thẳng lên Model rất tiện. Tại sao em lại đi cài thêm thư viện bên thứ 3 là FluentValidation làm gì cho phức tạp?"*

**Trả lời:**
"Dạ DataAnnotations vi phạm nguyên lý **Single Responsibility** vì nó trộn lẫn logic kiểm tra (Validation) vào chung với class chứa dữ liệu (DTO/Model).
Khi các luật kiểm tra trở nên phức tạp (Ví dụ: Ngày kết thúc phải lớn hơn ngày bắt đầu, hoặc Giá sản phẩm phải lớn hơn 0 nếu thuộc danh mục A), thì DataAnnotations gần như bất lực. 
FluentValidation giúp em tách bạch toàn bộ luật lệ ra một class riêng, code đọc trơn tru như tiếng Anh, và đặc biệt là cực kỳ dễ viết Unit Test độc lập ạ."

---

### Câu hỏi 18 (Xử lý lỗi tập trung - Global Exception):
**Người hỏi:** *"Tại sao lại dùng GlobalExceptionHandler? Nếu trong code em muốn bắt lỗi (try-catch) riêng rẽ để xử lý logic khác thì sao?"*

**Trả lời:**
"Dạ GlobalExceptionHandler không cấm chúng ta dùng `try...catch` ở bên trong lõi nghiệp vụ. Nếu một lỗi cần được xử lý để đi tiếp (ví dụ: lỗi gọi API bên thứ 3, nếu lỗi thì chuyển sang API dự phòng), em vẫn viết `try...catch` trong Handler bình thường.
Nhưng Global Exception giúp em 'hốt' tất cả những lỗi không thể lường trước (Unhandled Exceptions), hoặc các lỗi nghiệp vụ cố ý ném ra (như `NotFoundException`). Nó giúp Controller của em sạch bóng các khối `try...catch` rác rưởi, và đảm bảo mọi lỗi trả về cho Frontend đều theo một định dạng chuẩn chung nhất (ProblemDetails)."

---

### Câu hỏi 19 (Dependency Injection - Lifetime):
**Người hỏi:** *"Trong Dependency Injection của .NET có 3 vòng đời: Transient, Scoped, Singleton. Entity Framework `DbContext` của em đang được đăng ký bằng loại nào và tại sao?"*

**Trả lời:**
"Dạ `DbContext` mặc định luôn được đăng ký dưới dạng **Scoped**. 
Nghĩa là mỗi khi có một Client gửi HTTP Request tới, hệ thống sẽ sinh ra 1 bản sao DbContext, dùng chung xuyên suốt cho tất cả các class tham gia xử lý Request đó (giúp gom chung các thao tác vào một Transaction / Unit of Work). Khi Request kết thúc và trả về Response, DbContext đó sẽ tự động bị hủy để giải phóng kết nối tới Database.
Nếu dùng Singleton sẽ bị kẹt kết nối (dùng chung cho mọi user), còn Transient thì lại sinh ra quá nhiều kết nối rác ạ."

---

### Câu hỏi 20 (Unit of Work):
**Người hỏi:** *"Ở trên em có nhắc đến Unit of Work. Vậy Unit of Work là gì và nó nằm ở đâu trong dự án của em?"*

**Trả lời:**
"Dạ Unit of Work (Đơn vị công việc) là một Design Pattern. Nó có nhiệm vụ gom nhóm tất cả các thao tác (Thêm, Sửa, Xóa) trong một luồng nghiệp vụ lại với nhau thành một 'cục' duy nhất. Hệ thống sẽ đảm bảo: **Hoặc là tất cả cùng thành công, hoặc là tất cả cùng thất bại (Atomic/Transaction)**.
Trong dự án của em, em không cần phải tự code class `UnitOfWork` vì bản thân `DbContext` của Entity Framework Core đã được Microsoft thiết kế chìm theo chuẩn Unit of Work rồi.
Cụ thể, khi em dùng `_context.Products.Add(product)`, nó chỉ lưu tạm vào RAM (Memory) của DbContext. Chỉ khi nào gọi dòng lệnh `await _context.SaveChangesAsync()`, thì nó mới đóng gói tất cả thay đổi và gửi 1 câu lệnh SQL duy nhất xuống Database để thực thi. Việc này vừa an toàn dữ liệu, vừa tối ưu hiệu năng mạng ạ!"

---

### Câu hỏi 21 (Điểm lừa: Project Reference & Composition Root):
**Người hỏi:** *"Em nói tầng API (Presentation) không phụ thuộc vào tầng Infrastructure. Nhưng anh/chị mở file `DemoC#.csproj` lên lại thấy khai báo thẻ `<ProjectReference Include="..\DemoC#.Infrastructure..." />` rành rành ra đó. Thế này là em nói một đằng code một nẻo, phá vỡ kiến trúc rồi đúng không?"*

**Trả lời:**
"Dạ không hề phá vỡ kiến trúc ạ. Quy tắc phụ thuộc (Dependency Rule) của Clean Architecture là nói về sự phụ thuộc của **Logic nghiệp vụ bên trong mã nguồn**. Nếu anh/chị mở bất kỳ file Controller nào lên, sẽ thấy tuyệt đối không có sự xuất hiện của `AppDbContext` hay thư viện Database. Logic của Controller hoàn toàn 'mù' về Infrastructure.
Việc khai báo Reference trong file `.csproj` ở tầng API chỉ phục vụ duy nhất 1 mục đích: **Lắp ráp Dependency Injection lúc khởi động**.
Project API đóng vai trò là **Composition Root (Nơi lắp ráp trung tâm)**. Lúc ứng dụng vừa bật lên (tại file `Program.cs`), nó bắt buộc phải 'thấy' được project Infrastructure để gọi hàm `AddInfrastructureServices`, qua đó nạp chuỗi kết nối Database vào bộ nhớ. Ngoại trừ lúc khởi động ở Program.cs, các thành phần khác của tầng API tuyệt đối không được phép giao tiếp trực tiếp với Infrastructure ạ."

---

### Câu hỏi 22 (Cấu trúc Solution):
**Người hỏi:** *"Tại sao em phải mất công chia Solution ra thành 4 Project vật lý riêng biệt (Domain, Application, Infrastructure, API)? Tại sao không tạo 1 Project duy nhất rồi chia thành 4 cái thư mục (Folder) cho nhẹ nhàng?"*

**Trả lời:**
"Dạ chia thành các Folder trong 1 Project thì vẫn chạy được, nhưng nó thiếu đi **Sự bảo vệ của Trình biên dịch (Compile-time Protection)**.
Nếu gom chung vào 1 Project, một thành viên mới trong team rất dễ 'tiện tay' gọi (`using`) thẳng một thư viện Database của Infrastructure vào bên trong tầng Domain. Trình biên dịch (Visual Studio) sẽ không hề báo lỗi, và kiến trúc dần dần bị phá vỡ (Spaghetti code).
Bằng cách chia thành 4 Project vật lý (DLL) và thiết lập nguyên tắc tham chiếu (Project Reference) một chiều chặt chẽ trong file `.csproj`, hệ thống sẽ chặn đứng mọi ý đồ 'vượt rào'. Nếu em cố tình gọi Infrastructure từ Domain, Visual Studio sẽ báo lỗi đỏ lòm và không cho Build.
Ngoài ra, việc chẻ ra thành các file DLL riêng rẽ giúp tính tái sử dụng cực cao. Ngày mai sếp muốn làm thêm một giao diện Console App hay gRPC, em chỉ cần mang 2 cục DLL của Domain và Application cắm sang là chạy được ngay mà không cần mang theo cục API cũ ạ!"

---

### Câu hỏi 23 (Cập nhật công nghệ - .NET 10):
**Người hỏi:** *"Dự án này em đang sử dụng .NET 10. Em có thể chia sẻ nhanh .NET 10 có điểm gì nổi bật hoặc khác biệt so với các phiên bản cũ (như .NET 8) trong quá trình em phát triển dự án này không?"*

**Trả lời:**
"Dạ, .NET 10 được tối ưu hóa cực kỳ mạnh về hiệu năng (Performance) và cấp phát bộ nhớ. Tuy nhiên, thay đổi tác động trực tiếp nhất đến dự án này chính là hệ thống tài liệu API.
Kể từ .NET 9 và .NET 10, Microsoft đã loại bỏ sự phụ thuộc vào thư viện bên thứ ba (như Swashbuckle/Swagger cũ), mà tự phát triển gói `Microsoft.AspNetCore.OpenApi` tích hợp sâu (built-in) vào framework. Nó tự động sinh ra cấu trúc dữ liệu theo chuẩn OAS 3.1 mới nhất. Nhờ sự 'chính chủ' này, em có thể dễ dàng cắm các giao diện UI hiện đại, xịn xò và nhanh hơn như **Scalar UI** vào dự án mà không cần phải viết code cấu hình dài dòng như ngày xưa ạ."

---

### Câu hỏi 24 (Kiểu dữ liệu Record):
**Người hỏi:** *"Trong các thư mục DTO hoặc Command/Query, anh/chị thấy em dùng từ khóa khai báo là `record` thay vì `class` truyền thống. Khác biệt cốt lõi là gì và tại sao em lại dùng nó?"*

**Trả lời:**
"Dạ `record` bản chất vẫn là một kiểu tham chiếu (Reference Type) giống hệt `class`, nhưng nó sinh ra để tối ưu riêng cho **dữ liệu bất biến (Immutable)**. Lợi ích khi em dùng `record` cho các DTO/Command là:
1. **Ngắn gọn:** Cú pháp khai báo trên 1 dòng (Positional syntax), không cần viết getter/setter dài dòng. Code tự nhiên ngắn đi 50%.
2. **An toàn:** Khi một gói tin Command (ví dụ `AddProductCommand`) được gửi đi từ Controller sang MediatR, nó mang tính chất là 'Chỉ Đọc'. Việc dùng `record` đảm bảo không có ai ở giữa đường có thể vô tình hay cố ý thay đổi nội dung của gói tin đó.
3. **So sánh theo giá trị:** `record` tự động override các hàm so sánh. Hai biến `record` khác nhau nhưng mang dữ liệu bên trong giống hệt nhau thì sẽ được coi là bằng nhau (`==`), rất tiện lợi nếu cần viết Unit Test để so sánh kết quả ạ!"

---

### Câu hỏi 25 (Phân biệt Record, Const và Enum):
**Người hỏi:** *"Anh/chị thấy em giải thích `record` là bất biến (không thay đổi). Vậy bản chất nó có giống với việc khai báo `const` hay `enum` không?"*

**Trả lời:**
"Dạ hoàn toàn khác nhau ạ, dù chúng đều mang ý nghĩa 'cố định' nhưng ở các cấp độ khác biệt:
- **`const` (Hằng số):** Chốt cứng một giá trị đơn giản (như số, chuỗi) **ngay từ lúc viết code** (Compile-time). Ví dụ: `const double PI = 3.14`.
- **`enum` (Kiểu liệt kê):** Là một **danh sách lựa chọn có giới hạn** được định nghĩa trước. Ví dụ Trạng thái đơn hàng chỉ có (Pending, Shipping, Completed). Mục đích là để code dễ đọc và chống gõ sai chính tả.
- **`record` (Đối tượng bất biến):** Là một cấu trúc đối tượng chứa thông tin phức tạp (như Command chứa Name, Price). Dữ liệu này là vô hạn do người dùng nhập vào **lúc chương trình đang chạy** (Run-time). Chữ 'bất biến' của record mang ý nghĩa: Một khi đối tượng đã được khởi tạo (`new`) và nhét dữ liệu vào, thì nó bị 'khóa sổ', không ai có thể sửa đổi các giá trị bên trong nó nữa (giúp truyền tải dữ liệu an toàn).
Thực tế trong dự án, một cái `record` (giỏ chở hàng) hoàn toàn có thể chứa một cái `enum` (lựa chọn danh mục cố định) bên trong nó ạ!"
