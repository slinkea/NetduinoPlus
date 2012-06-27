namespace BookShop
{
    public class Order
    {
        public Order(string Client, Book Book)
        {
            this.Client = Client;
            this.Book = Book;
            LastID++;
            ID = LastID;
        }

        public int ID;
        public string Client;
        public Book Book;
        public bool Canceled;
        public static int LastID = 0;
    }
}