using DemoC_.Application.DTOs;
using DemoC_.Application.Interfaces;
using DemoC_.Domain.Exceptions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DemoC_.Application.Features.Products.Commands.UpdateProduct
{
    public record UpdateProductCommand(UpdateProductDto Dto) : IRequest<bool>;

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public UpdateProductCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(new object[] { request.Dto.Id }, cancellationToken);
            if (product == null)
            {
                throw new NotFoundException($"Product with ID {request.Dto.Id} not found.");
            }

            var price = new DemoC_.Domain.ValueObjects.Money(request.Dto.Price);
            product.UpdateDetails(
                request.Dto.Name,
                request.Dto.Description,
                price,
                request.Dto.Quantity,
                request.Dto.Category);

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
