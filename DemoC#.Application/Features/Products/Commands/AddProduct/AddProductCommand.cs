using DemoC_.Application.DTOs;
using DemoC_.Application.Interfaces;
using DemoC_.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DemoC_.Application.Features.Products.Commands.AddProduct
{
    /// <summary>
    /// Command mang theo dữ liệu (Tương đương DTO đầu vào).
    /// Theo chuẩn CQRS, Command chỉ làm nhiệm vụ "ra lệnh" (chứa data), không chứa logic.
    /// Kế thừa IRequest(T) báo hiệu cho MediatR biết Command này sẽ trả về ProductResponseDto.
    /// </summary>
    public record AddProductCommand(CreateProductDto Dto) : IRequest<ProductResponseDto>;

    /// <summary>
    /// Handler xử lý logic của AddProductCommand (Đã thay thế cho ProductService cũ).
    /// Bất cứ khi nào Controller Send một AddProductCommand, MediatR sẽ tự động tìm và chạy hàm Handle() ở đây.
    /// </summary>
    public class AddProductCommandHandler : IRequestHandler<AddProductCommand, ProductResponseDto>
    {
        private readonly IApplicationDbContext _context;

        public AddProductCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductResponseDto> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            // Bước 1: Khởi tạo Entity bằng Factory Method (Domain-Driven Design)
            var price = new DemoC_.Domain.ValueObjects.Money(request.Dto.Price);
            var product = Product.Create(
                request.Dto.Name,
                request.Dto.Description,
                price,
                request.Dto.Quantity,
                request.Dto.Category);

            // Bước 2: Tương tác trực tiếp với Database qua IApplicationDbContext
            // Loại bỏ hoàn toàn Repository Pattern giúp code gọn gàng, chạy nhanh hơn.
            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            // Bước 3: Ánh xạ kết quả sang ResponseDto để trả về an toàn
            return new ProductResponseDto(
                product.Id,
                product.Name,
                product.Description,
                product.Price.Amount, // Rút tiền từ Value Object ra
                product.Quantity,
                product.Category
            );
        }
    }
}
