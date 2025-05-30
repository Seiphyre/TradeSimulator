using System.Xml.Linq;

namespace TradeSimulator.Shared.Models
{
    public class Ticker : EntityBase
    {
        public string DisplayName { get; set; }

        public List<Order> Orders { get; set; }
    }
}
