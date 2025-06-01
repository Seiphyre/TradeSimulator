namespace TradeSimulator.Shared.Models
{
    public class OrderBook : EntityBase
    {
        public string BrokerId { get; set; }

        public string TickerId { get; set; }
        public string TickerDisplayName { get; set; }

        public bool IsOpen { get; set; }
    }
}
