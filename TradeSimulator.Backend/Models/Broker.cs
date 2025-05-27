namespace TradeSimulator.Backend.Models
{
    public class Broker : EntityBase
    {
        public string DisplayName { get; set; }

        public List<OrderBook> OrderBooks { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
