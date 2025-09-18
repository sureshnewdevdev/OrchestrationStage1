using CatelogService.Model;

namespace CatelogService.Model.Data
{
    public enum OrcState
    {
        ProductCatalogShow,
        ProductAddedToCart,
        BillPaid,
        NotificationSent
    }

    public interface ICommand
    {
        OrcState State { get; }
        OrcState? PreviousCommand { get; set; }
        OrcState? NextCommand { get; }
        string CommandName { get; }
        CommandResult Execute();
    }

    public class ProductCatalogShowCommand : ICommand
    {
        public OrcState State => OrcState.ProductCatalogShow;
        public OrcState? PreviousCommand { get; set; }
        public OrcState? NextCommand { get; private set; }
        public string CommandName => "ProductCatalogShow";

        public CommandResult Execute()
        {
            try
            {
                var products = DoProductShow();
                NextCommand = OrcState.ProductAddedToCart;
                return new CommandResult(State, NextCommand, "Loaded product catalog.", products);
            }
            catch (Exception ex)
            {
                NextCommand = State;
                return new CommandResult(State, NextCommand, $"Unable to load products: {ex.Message}");
            }
        }

        private IReadOnlyList<Product> DoProductShow()
        {
            return SampleData.GetProducts();
        }
    }

    public class AddProductToCartCommand : ICommand
    {
        public OrcState State => OrcState.ProductAddedToCart;
        public OrcState? PreviousCommand { get; set; }
        public OrcState? NextCommand { get; private set; }
        public string CommandName => "AddProductToCart";

        public CommandResult Execute()
        {
            try
            {
                var cart = BuildCart();
                NextCommand = OrcState.BillPaid;
                return new CommandResult(State, NextCommand, "Product added to cart.", cart);
            }
            catch (Exception ex)
            {
                NextCommand = PreviousCommand ?? OrcState.ProductCatalogShow;
                return new CommandResult(State, NextCommand, $"Unable to update cart: {ex.Message}");
            }
        }

        private CartSummary BuildCart()
        {
            var cart = SampleData.CreateCart();
            return cart;
        }
    }

    public class PayBillCommand : ICommand
    {
        public OrcState State => OrcState.BillPaid;
        public OrcState? PreviousCommand { get; set; }
        public OrcState? NextCommand { get; private set; }
        public string CommandName => "PayBill";

        public CommandResult Execute()
        {
            try
            {
                var receipt = ProcessPayment();
                NextCommand = OrcState.NotificationSent;
                return new CommandResult(State, NextCommand, "Payment completed successfully.", receipt);
            }
            catch (Exception ex)
            {
                NextCommand = State;
                return new CommandResult(State, NextCommand, $"Payment failed: {ex.Message}");
            }
        }

        private PaymentReceipt ProcessPayment()
        {
            var cart = SampleData.CreateCart();
            return new PaymentReceipt
            {
                ReceiptNumber = $"RCT-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}",
                PaidOn = DateTime.UtcNow,
                Amount = cart.Total,
                PaymentMethod = "Credit Card"
            };
        }
    }

    public class SendNotificationCommand : ICommand
    {
        public OrcState State => OrcState.NotificationSent;
        public OrcState? PreviousCommand { get; set; }
        public OrcState? NextCommand { get; private set; }
        public string CommandName => "SendNotification";

        public CommandResult Execute()
        {
            try
            {
                var result = DispatchNotification();
                NextCommand = null;
                return new CommandResult(State, NextCommand, "Notification delivered.", result);
            }
            catch (Exception ex)
            {
                NextCommand = State;
                return new CommandResult(State, NextCommand, $"Notification failed: {ex.Message}");
            }
        }

        private NotificationResult DispatchNotification()
        {
            return new NotificationResult
            {
                Recipient = "customer@example.com",
                Channel = "Email",
                Message = "Thanks for shopping with us! Your payment was received.",
                SentOn = DateTime.UtcNow
            };
        }
    }

    public class CommandInvoker
    {
        private readonly IDictionary<OrcState, Func<ICommand>> _commandFactory;

        public CommandInvoker()
        {
            _commandFactory = new Dictionary<OrcState, Func<ICommand>>
            {
                { OrcState.ProductCatalogShow, () => new ProductCatalogShowCommand() },
                { OrcState.ProductAddedToCart, () => new AddProductToCartCommand() },
                { OrcState.BillPaid, () => new PayBillCommand() },
                { OrcState.NotificationSent, () => new SendNotificationCommand() }
            };
        }

        public CommandResult Execute(OrcState state, OrcState? previous = null)
        {
            if (!_commandFactory.TryGetValue(state, out var factory))
            {
                throw new ArgumentOutOfRangeException(nameof(state), state, "Unsupported orchestration state.");
            }

            var command = factory();
            command.PreviousCommand = previous;
            return command.Execute();
        }

        public IReadOnlyList<CommandResult> RunWorkflow(OrcState startState)
        {
            var results = new List<CommandResult>();
            OrcState? current = startState;
            OrcState? previous = null;

            while (current.HasValue)
            {
                var result = Execute(current.Value, previous);
                results.Add(result);

                if (!result.NextState.HasValue || result.IsTerminal)
                {
                    break;
                }

                previous = current;
                current = result.NextState;
            }

            return results;
        }
    }
}
