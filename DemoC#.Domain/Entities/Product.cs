using DemoC_.Domain.Enums;
using DemoC_.Domain.ValueObjects;
using DemoC_.Domain.Events;
using System;
using System.Collections.Generic;
namespace DemoC_.Domain.Entities
{
    /// <summary>
    /// Lớp Product được thiết kế theo chuẩn "Rich Domain Model" (Domain Giàu có) của Domain-Driven Design (DDD).
    public class Product
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }
        
        // Sử dụng Value Object thay vì kiểu nguyên thủy (primitive type)
        public Money Price { get; private set; } 
        
        public int Quantity { get; private set; }
        public ProductCategory Category { get; private set; }

        // Cơ chế quản lý Domain Events
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        private Product() { }

        public static Product Create(string name, string? description, Money price, int quantity, ProductCategory category)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên sản phẩm không được để trống", nameof(name));
            
            var product = new Product
            {
                Name = name,
                Description = description,
                Price = price,
                Quantity = quantity,
                Category = category
            };
            // Bắn ra sự kiện miền (Domain Event) ngay khi tạo thành công
            product._domainEvents.Add(new ProductCreatedEvent(product, DateTime.UtcNow));

            return product;
        }

        public void UpdateDetails(string name, string? description, Money price, int quantity, ProductCategory category)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên sản phẩm không được để trống", nameof(name));
            if (quantity < 0)
                throw new ArgumentException("Số lượng không được âm", nameof(quantity));

            Name = name;
            Description = description;
            Price = price;
            Quantity = quantity;
            Category = category;
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
