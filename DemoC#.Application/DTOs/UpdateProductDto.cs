using DemoC_.Domain.Enums;

namespace DemoC_.Application.DTOs
{
    /// <summary>
    /// Đối tượng truyền tải dữ liệu khi cập nhật Sản phẩm.
    /// </summary>
    /// <param name="Id">Mã định danh sản phẩm cần cập nhật. <example>1</example></param>
    /// <param name="Name">Tên của sản phẩm. <example>Chuột không dây Razer (Bản nâng cấp)</example></param>
    /// <param name="Description">Mô tả chi tiết. <example>Chuột gaming siêu nhẹ</example></param>
    /// <param name="Price">Giá bán của sản phẩm. <example>1200000</example></param>
    /// <param name="Quantity">Số lượng sản phẩm trong kho. <example>30</example></param>
    /// <param name="Category">Phân loại danh mục. <example>Electronics</example></param>
    public record UpdateProductDto(
        int Id,
        string Name,
        string? Description,
        decimal Price,
        int Quantity,
        ProductCategory Category
    );
}
