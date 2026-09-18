using DemoC_.Application.DTOs;
using DemoC_.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DemoC_.Application.Features.Products.Queries.GetProducts
{
    public record PaginatedResponse<T>(List<T> Items, int TotalCount, int PageNumber, int PageSize);

    public record GetProductsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedResponse<ProductResponseDto>>;

    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PaginatedResponse<ProductResponseDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetProductsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResponse<ProductResponseDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Products.AsNoTracking();
            
            var totalCount = await query.CountAsync(cancellationToken);
            
            var items = await query
                .OrderBy(x => x.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new ProductResponseDto(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Price.Amount,
                    p.Quantity,
                    p.Category)) // Enum Category
                .ToListAsync(cancellationToken);
                
            return new PaginatedResponse<ProductResponseDto>(items, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
