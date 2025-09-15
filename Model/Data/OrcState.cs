using System.Threading.RateLimiting;

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
        public OrcState PreviousCommand { get; set; }
        public OrcState NextCommand { get; set; }
        string CommandName { get; }
        void Execute();
    }

    public class ProductCatalogShowCommand : ICommand
    {
        public OrcState PreviousCommand { get; set; }
        public OrcState NextCommand { get; set; }
        public string CommandName => "ProductCatalogShow";
        public void Execute()
        {
            try
            {
                DoProductShow();
                NextCommand = OrcState.ProductAddedToCart;
            }
            catch (Exception)
            {
                NextCommand = OrcState.ProductCatalogShow;
            }
            // Implementation for showing product catalog
        }


        private void DoProductShow()
        {
            List<Product> productList = new List<Product>()
            {
                new Product(){ ProductId=1,ProductName="Product 1"},
                new Product(){ ProductId=2,ProductName="Product 2"},
                new Product(){ ProductId=3,ProductName="Product 3"},
                new Product(){ ProductId=4,ProductName="Product 4"},
                new Product(){ ProductId=5,ProductName="Product 5"},
            };


            //return (IActionResult)productList;
        }
    }

    /// <summary>
    /// Command to start an order process ( ProductCatalog -> BillPaid -> NotificationSent )
    /// </summary>
    public class StartOrderCommand : ICommand
    {
        public string CommandName => "StartOrder";

        public OrcState PreviousCommand { get ; set ; }
        public OrcState NextCommand { get ; set ; }

        public void Execute()
        {
            // Implementation for starting an order
            try
            {
                //Do action based on OrcState => Catalog, Bill, Notification
                DoSomething();
                NextCommand = OrcState.BillPaid;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void DoSomething()
        {
            
        }
    }

    /// <summary>
    /// Command to pay a bill ( BillPaid -> NotificationSent )
    /// </summary>
    public class PayBillCommand : ICommand
    {
        public OrcState PreviousCommand { get; set; }
        public OrcState NextCommand { get; set; }

        public string CommandName => "PayBill";
        public void Execute()
        {
          
        }
    }

    /// <summary>
    /// Command to send a notification ( NotificationSent )
    /// </summary>
    public class SendNotificationCommand : ICommand
    {
        public OrcState PreviousCommand { get; set; }
        public OrcState NextCommand { get; set; }


        public string CommandName => "SendNotification";
        public void Execute()
        {
            // Implementation for sending a notification
        }
    }

    public class CommandInvoker
    {
        public ICommand ActionInvoke(OrcState orcState)
        {
            ICommand command = new StartOrderCommand(); // Default command
            OrcState orcStateAfterExecution = OrcState.ProductCatalogShow;
            switch (orcState)
            {
                case OrcState.ProductCatalogShow:
                    command = new ProductCatalogShowCommand();
                    command.Execute();
                    orcStateAfterExecution = command.NextCommand;

                    break;
                case OrcState.ProductAddedToCart:
                    command.Execute();
                    orcStateAfterExecution = command.NextCommand;
                    break;
                case OrcState.BillPaid:
                    command = new PayBillCommand();
                    command.Execute();
                    orcStateAfterExecution = command.NextCommand;
                    break;
                case OrcState.NotificationSent:
                    command = new SendNotificationCommand();
                    command.Execute();
                    orcStateAfterExecution = command.NextCommand;

                    break;
                default:
                    break;
            }

            return command;
        }
    }
}
