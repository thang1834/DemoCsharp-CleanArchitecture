using DemoC_.Application.DTOs;
using DemoC_.Application.Interfaces;
using DemoC_.Domain.Exceptions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DemoC_.Application.Features.Products.Queries.GetProductById
{
    public record GetProductByIdQuery(int Id) : IRequest<ProductResponseDto>;

    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductResponseDto>
    {
        private readonly IApplicationDbContext _context;

        public GetProductByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductResponseDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(new object[] { request.Id }, cancellationToken);
            if (product == null)
            {
                throw new NotFoundException($"Product with ID {request.Id} not found.");
            }

            return new ProductResponseDto(
                product.Id,
                product.Name,
                product.Description,
                product.Price.Amount,
                product.Quantity,
                product.Category
            );
        }
    }
}
