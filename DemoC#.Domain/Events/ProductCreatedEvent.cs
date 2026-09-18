using DemoC_.Domain.Entities;
using System;

namespace DemoC_.Domain.Events
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }

    /// <summary>
    /// Domain Event: Sự kiện phát ra khi một Sản phẩm mới được tạo thành công.
    /// </summary>
    public record ProductCreatedEvent(Product Product, DateTime OccurredOn) : IDomainEvent;
}
