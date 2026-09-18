using DemoC_.Domain.Enums;

namespace DemoC_.Application.DTOs
{
    /// <summary>
    /// Đối tượng truyền tải dữ liệu khi thêm mới Sản phẩm.
    /// </summary>
    /// <param name="Name">Tên của sản phẩm. <example>Bàn phím cơ Logitech G Pro</example></param>
    /// <param name="Description">Mô tả chi tiết. <example>Bàn phím xịn dành cho Game thủ</example></param>
    /// <param name="Price">Giá bán của sản phẩm. <example>2500000</example></param>
    /// <param name="Quantity">Số lượng sản phẩm trong kho. <example>50</example></param>
    /// <param name="Category">Phân loại danh mục. <example>Electronics</example></param>
    public record CreateProductDto(
        string Name,
        string? Description,
        decimal Price,
        int Quantity,
        ProductCategory Category
    );
}
