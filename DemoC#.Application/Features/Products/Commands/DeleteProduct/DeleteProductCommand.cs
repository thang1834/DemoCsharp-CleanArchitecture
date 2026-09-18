using DemoC_.Application.Interfaces;
using DemoC_.Domain.Exceptions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DemoC_.Application.Features.Products.Commands.DeleteProduct
{
    public record DeleteProductCommand(int Id) : IRequest<bool>;

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteProductCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(new object[] { request.Id }, cancellationToken);
            if (product == null)
            {
                throw new NotFoundException($"Product with ID {request.Id} not found.");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
