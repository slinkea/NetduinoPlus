namespace BookShop
{
    public class Book
    {
        public Book(string Name, decimal Price)
        {
            this.Name = Name;
            this.Price = Price;
        }

        public string Name;
        public decimal Price;
    }
}