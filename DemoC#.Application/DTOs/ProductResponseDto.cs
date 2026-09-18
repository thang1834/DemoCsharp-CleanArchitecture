using DemoC_.Domain.Enums;

namespace DemoC_.Application.DTOs
{
    public record ProductResponseDto(
        int Id,
        string Name,
        string? Description,
        decimal Price,
        int Quantity,
        ProductCategory Category
    );
}
