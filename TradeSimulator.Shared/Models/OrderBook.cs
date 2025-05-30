namespace TradeSimulator.Shared.Models
{
    public class OrderBook : EntityBase
    {
        public string BrokerId { get; set; }

        public string TickerId { get; set; }

        public bool IsOpen { get; set; }
    }
}
