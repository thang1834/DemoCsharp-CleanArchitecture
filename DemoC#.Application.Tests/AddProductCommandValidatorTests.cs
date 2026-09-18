using DemoC_.Application.DTOs;
using DemoC_.Application.Features.Products.Commands.AddProduct;
using DemoC_.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace DemoC_.Application.Tests
{
    public class AddProductCommandValidatorTests
    {
        private readonly AddProductCommandValidator _validator;

        public AddProductCommandValidatorTests()
        {
            _validator = new AddProductCommandValidator();
        }

        [Fact]
        public void Should_NotHaveError_When_CommandIsValid()
        {
            var dto = new CreateProductDto("Tai nghe", "Mô tả", 500000, 10, ProductCategory.Electronics);
            var command = new AddProductCommand(dto);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_HaveError_When_NameIsEmpty()
        {
            var dto = new CreateProductDto("", "Mô tả", 500000, 10, ProductCategory.Electronics);
            var command = new AddProductCommand(dto);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Dto.Name)
                  .WithErrorMessage("Tên sản phẩm không được để trống.");
        }

        [Fact]
        public void Should_HaveError_When_PriceIsZeroOrNegative()
        {
            var dto = new CreateProductDto("Tai nghe", "Mô tả", 0, 10, ProductCategory.Electronics);
            var command = new AddProductCommand(dto);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Dto.Price)
                  .WithErrorMessage("Giá sản phẩm phải lớn hơn 0.");
        }
    }
}
