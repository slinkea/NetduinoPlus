using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MM.Monitor.Cloud;

namespace BookShop
{
    public class BookShop
    {
        private readonly List<Order> OrderList = new List<Order>();
        private List<Book> BookList = new List<Book>();
        private List<string> ClientList = new List<string>();
        private const int PAGE_LIST_ORDERS = 91;
        private const int PAGE_LIST_CANCELED_ORDERS = 92;
        private const int PAGE_LIST_BOOKS = 93;
        private const int COMMAND_CANCEL_ORDER = 1;
        private const int COMMAND_CLEAR_DATA = 2;
        private const int COMMAND_RESET_ORDER = 3;

        public BookShop()
        {
            BookList = DataBank.GenerateBooks();
            ClientList = DataBank.GenerateClientNames();
            Service.Instance.OnDetailsRequest += OnDetailsRequest;
            Service.Instance.OnPageRequest += OnPageRequest;
            Service.Instance.OnCommandReceived += OnCommandReceived;
            Service.Instance.OnPageCommandReceived += OnPageCommandReceived;
            Service.Instance.OnExceptionOccurred += OnExceptionOccured;
            Service.Instance.Configure("Book Shop", "Cloud", "Running on " + Environment.MachineName, false);
            Service.Instance.Start("rocca1976", "0123456789", "D7-C6-B1-F2-3A-7D-69-6C-36-B4-EE-1D-CE-D3-42-23-68-C0-66-FF-AA-E0-C6-51-05-5D-96-92-19-03-7D-2A-A4-B8-0D-D0-2F-CB-31-1E-C0-8F-9C-31-E2-0C-31-E8-76-FB-EB-46-5D-62-87-3B-81-7A-09-8D-41-8D-94-96");
            new Thread(AddOrders).Start();
            Console.ReadLine();
        }

        private void AddOrders()
        {
            while (OrderList.Count != 50)
            {
                OrderList.Add(new Order(ClientList[OrderList.Count], BookList[OrderList.Count]));
                Thread.Sleep(10000);
            }
        }

        private int GetCanceledOrdersCount()
        {
            return OrderList.Count(n => n.Canceled == true);
        }

        private void OnDetailsRequest()
        {
            Groups container = new Groups();
            Group stats = new Group("Statistics");
            stats.Items.Add(new SimpleItem("Book Count: ", BookList.Count.ToString()));
            stats.Items.Add(new SimpleItem("Orders Count: ", OrderList.Count.ToString()));
            stats.Items.Add(new SimpleItem("Canceled Orders Count: ", GetCanceledOrdersCount().ToString()));
            Group pages = new Group("Lists");
            pages.Items.Add(new PageItem(PAGE_LIST_BOOKS, "List Books"));
            pages.Items.Add(new PageItem(PAGE_LIST_ORDERS, "List Orders"));
            pages.Items.Add(new PageItem(PAGE_LIST_CANCELED_ORDERS, "List Canceled Orders"));
            Group actions = new Group("Actions");
            actions.Items.Add(new CommandItem(COMMAND_CLEAR_DATA, "Reset DataBank"));
            container.Add(stats);
            container.Add(pages);
            container.Add(actions);
            Service.Instance.SetDetails(container);
        }

        private void OnPageRequest(int pageId, string mobileDeviceIdentifier)
        {
            Groups container = new Groups();
            Group contents = null;
            switch (pageId)
            {
                case PAGE_LIST_BOOKS:
                    contents = new Group("Book List");
                    foreach (var b in BookList)
                        contents.Items.Add(new SimpleItem(b.Name, b.Price.ToString()));
                    break;
                case PAGE_LIST_ORDERS:
                    contents = new Group("Order List");
                    foreach (Order o in OrderList)
                        contents.Items.Add(new PageItem(o.ID, String.Format("Client: {0}\nBook: {1}\nBook Price: {2}\nCanceled: {3}", o.Client, o.Book.Name, o.Book.Price, o.Canceled)));
                    break;
                case PAGE_LIST_CANCELED_ORDERS:
                    contents = new Group("Canceled Order List");
                    foreach (var co in OrderList.Where(q => q.Canceled == true))
                        contents.Items.Add(new PageItem(co.ID, String.Format("Client: {0}\nBook: {1}\nBook Price: {2}\nCanceled: {3}", co.Client, co.Book.Name, co.Book.Price, co.Canceled)));
                    break;
                default:
                    contents = new Group("Order Details");
                    Order current = OrderList.Single(q => q.ID == pageId);
                    contents.Items.Add(new SimpleItem("Client Name: ", current.Client));
                    contents.Items.Add(new SimpleItem("Book Name: ", current.Book.Name));
                    contents.Items.Add(new SimpleItem("Book Price: ", current.Book.Price.ToString()));
                    contents.Items.Add(new SimpleItem("Canceled: ", current.Canceled.ToString()));
                    if (!current.Canceled)
                        contents.Items.Add(new CommandItem(COMMAND_CANCEL_ORDER, "Cancel Order"));
                    else
                        contents.Items.Add(new CommandItem(COMMAND_RESET_ORDER, "Reset Order"));
                    break;
            }
            container.Add(contents);
            Service.Instance.SetPageDetails(pageId, container);
        }

        private void OnCommandReceived(int commandId, string mobileDeviceIdentifier)
        {
            if (commandId == COMMAND_CLEAR_DATA)
            {
                BookList = DataBank.GenerateBooks();
                ClientList = DataBank.GenerateClientNames();
                OrderList.Clear();
                Order.LastID = 0;
            }
        }

        private void OnPageCommandReceived(int pageId, int commandId, string mobileDeviceIdentifier)
        {
            if (commandId == COMMAND_CANCEL_ORDER)
                OrderList.Single(q => q.ID == pageId).Canceled = true;
            else if (commandId == COMMAND_RESET_ORDER)
                OrderList.Single(q => q.ID == pageId).Canceled = false;
        }

        private void OnExceptionOccured(Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.ReadLine();
        }
    }
}