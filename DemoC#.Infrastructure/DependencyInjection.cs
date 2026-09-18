using DemoC_.Application.Interfaces;
using DemoC_.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DemoC_.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Đăng ký DbContext với PostgreSQL (Đang sử dụng mặc định)
            services.AddDbContext<AppDBContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // --- CÁC VÍ DỤ THAY ĐỔI DATABASE SIÊU NHANH (Chỉ cần cài đúng Package và bật code) ---
            
            // 1. Nếu muốn dùng SQL Server:
            // services.AddDbContext<AppDBContext>(options =>
            //     options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // 2. Nếu muốn dùng MySQL:
            // services.AddDbContext<AppDBContext>(options =>
            //     options.UseMySql(configuration.GetConnectionString("DefaultConnection"), 
            //     ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection"))));

            // 3. Nếu muốn dùng SQLite (Database nhẹ, chạy local):
            // services.AddDbContext<AppDBContext>(options =>
            //     options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            // Đăng ký IApplicationDbContext
            services.AddScoped<IApplicationDbContext, AppDBContext>();

            return services;
        }
    }
}
