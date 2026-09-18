using DemoC_.Application.Interfaces;
using DemoC_.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace DemoC_.Infrastructure.Data
{
    /// <summary>
    /// AppDBContext đại diện cho CSDL chính của hệ thống.
    /// Kế thừa DbContext (của EF Core) và cài đặt giao diện IApplicationDbContext (của tầng Application).
    /// Việc này giúp Tầng Application gọi được Database mà không cần tham chiếu trực tiếp đến file của tầng Infrastructure (Đảo ngược phụ thuộc - Dependency Inversion).
    /// </summary>
    public class AppDBContext : DbContext, IApplicationDbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Tự động quét và áp dụng tất cả các cấu hình (Configurations) trong project Infrastructure
            // Thay vì viết hàng trăm dòng cấu hình bảng ở đây, ta gom chúng vào từng file riêng (Fluent API).
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDBContext).Assembly);
        }
        
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
