namespace TradeSimulator.Shared.Models
{
    public class Transaction
    {
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public TransactionType TransactionType { get; set; }
        public string TickerDisplayName { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
