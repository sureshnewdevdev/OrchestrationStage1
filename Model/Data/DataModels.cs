using CatelogService.Model;

namespace CatelogService.Model.Data
{
    public record CommandResult(
        OrcState CurrentState,
        OrcState? NextState,
        string Message,
        object? Payload = null)
    {
        public bool IsTerminal => !NextState.HasValue || NextState == CurrentState;
    }

    public class CartItem
    {
        public required Product Product { get; init; }
        public int Quantity { get; init; }
        public decimal LineTotal => Product.Price * Quantity;
    }

    public class CartSummary
    {
        public IList<CartItem> Items { get; init; } = new List<CartItem>();
        public decimal Total => Items.Sum(item => item.LineTotal);
    }

    public class PaymentReceipt
    {
        public string ReceiptNumber { get; init; } = string.Empty;
        public DateTime PaidOn { get; init; }
        public decimal Amount { get; init; }
        public string PaymentMethod { get; init; } = string.Empty;
    }

    public class NotificationResult
    {
        public string Recipient { get; init; } = string.Empty;
        public string Channel { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public DateTime SentOn { get; init; }
    }

    public static class SampleData
    {
        public static IReadOnlyList<Product> GetProducts() => new List<Product>
        {
            new() { ProductId = 1, ProductName = "Wireless Mouse", Price = 25.99m },
            new() { ProductId = 2, ProductName = "Mechanical Keyboard", Price = 78.50m },
            new() { ProductId = 3, ProductName = "27\" Monitor", Price = 225.00m },
            new() { ProductId = 4, ProductName = "USB-C Dock", Price = 139.99m },
            new() { ProductId = 5, ProductName = "Noise Cancelling Headphones", Price = 189.00m }
        };

        public static CartSummary CreateCart()
        {
            var products = GetProducts();
            return new CartSummary
            {
                Items = new List<CartItem>
                {
                    new() { Product = products[0], Quantity = 1 },
                    new() { Product = products[1], Quantity = 1 },
                    new() { Product = products[3], Quantity = 2 }
                }
            };
        }
    }
}
