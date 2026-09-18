using DemoC_.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace DemoC_.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Product> Products { get; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
