using DemoC_.Application.Features.Products.Commands.AddProduct;
using FluentValidation;

namespace DemoC_.Application.Features.Products.Commands.AddProduct
{
    public class AddProductCommandValidator : AbstractValidator<AddProductCommand>
    {
        public AddProductCommandValidator()
        {
            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Tên sản phẩm không được để trống.")
                .MaximumLength(255).WithMessage("Tên sản phẩm không được vượt quá 255 ký tự.");

            RuleFor(x => x.Dto.Price)
                .GreaterThan(0).WithMessage("Giá sản phẩm phải lớn hơn 0.");

            RuleFor(x => x.Dto.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Số lượng không được âm.");
                
            RuleFor(x => x.Dto.Category)
                .IsInEnum().WithMessage("Loại danh mục không hợp lệ.");
        }
    }
}
