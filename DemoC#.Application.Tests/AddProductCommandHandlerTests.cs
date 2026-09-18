using DemoC_.Application.DTOs;
using DemoC_.Application.Features.Products.Commands.AddProduct;
using DemoC_.Application.Interfaces;
using DemoC_.Domain.Entities;
using DemoC_.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DemoC_.Application.Tests
{
    public class TestDbContext : DbContext, IApplicationDbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
    }

    public class AddProductCommandHandlerTests
    {
        private readonly AddProductCommandHandler _handler;
        private readonly TestDbContext _dbContext;

        public AddProductCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new TestDbContext(options);
            _handler = new AddProductCommandHandler(_dbContext);
        }

        [Fact]
        public async Task Handle_ShouldCreateProduct_WhenDataIsValid()
        {
            // Arrange
            var dto = new CreateProductDto(
                Name: "Bàn phím cơ Test",
                Description: "Mô tả test",
                Price: 1500000,
                Quantity: 10,
                Category: ProductCategory.Electronics
            );
            var command = new AddProductCommand(dto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Bàn phím cơ Test", result.Name);
            Assert.Equal(1500000, result.Price);
            
            var productInDb = await _dbContext.Products.FirstOrDefaultAsync();
            Assert.NotNull(productInDb);
            Assert.Equal(result.Id, productInDb.Id);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenPriceIsNegative()
        {
            // Arrange
            var dto = new CreateProductDto(
                Name: "Sản phẩm lỗi",
                Description: "Giá âm là không hợp lệ",
                Price: -50000,
                Quantity: 5,
                Category: ProductCategory.Others
            );
            var command = new AddProductCommand(dto);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _handler.Handle(command, CancellationToken.None));
                
            Assert.Equal("Giá sản phẩm không được âm (Parameter 'price')", exception.Message);
        }
    }
}
