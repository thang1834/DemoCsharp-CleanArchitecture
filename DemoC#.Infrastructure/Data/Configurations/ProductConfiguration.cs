using DemoC_.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoC_.Infrastructure.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.Price)
                .HasConversion(
                    money => money.Amount, // Khi lưu vào DB: Rút trích số tiền
                    value => new DemoC_.Domain.ValueObjects.Money(value, "VND") // Khi đọc từ DB: Gói lại thành Value Object
                )
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Quantity)
                .IsRequired();
                
            builder.Property(x => x.Category)
                .IsRequired();
        }
    }
}
