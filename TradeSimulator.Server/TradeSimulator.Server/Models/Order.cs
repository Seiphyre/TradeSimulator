namespace TradeSimulator.Server.Models
{
    public class Order
    {
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}
