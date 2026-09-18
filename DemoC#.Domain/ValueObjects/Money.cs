using System;

namespace DemoC_.Domain.ValueObjects
{
    /// <summary>
    /// Value Object đại diện cho Tiền tệ.
    /// Nó bất biến (Immutable) và cung cấp sẵn logic nghiệp vụ riêng của tiền bạc.
    /// </summary>
    public record Money
    {
        public decimal Amount { get; init; }
        public string Currency { get; init; }

        public Money(decimal amount, string currency = "VND")
        {
            if (amount < 0)
                throw new ArgumentException("Số tiền không được âm.", nameof(amount));
            
            Amount = amount;
            Currency = currency;
        }
        
        public override string ToString() => $"{Amount:N0} {Currency}";
    }
}
