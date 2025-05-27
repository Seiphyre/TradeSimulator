namespace TradeSimulator.Backend.Models
{
    public class Ticker : EntityBase
    {
        public string DisplayName { get; set; }

        public List<Order> Orders { get; set; }
    }
}
