using TradeSimulator.Shared.Extensions;
using TradeSimulator.Shared.Models;

namespace TradeSimulator.Backend.Repositories
{
    public class TickerRepository : RepositoryBase<Ticker>
    {

        public TickerRepository()
        {
            Entities = GenerateTickers();
        }

        private List<Ticker> GenerateTickers(int count = 10)
        {
            List<string> tickerDisplayNames = new() 
            { 
                "TSLA", 
                "AAPL", 
                "AMZN", 
                "WMT", 
                "NVDA", 
                "PDD", 
                "AMC", 
                "MSFT", 
                "AMD", 
                "DUOL", 
                "SPOT", 
                "GOOG", 
                "META", 
                "NFLX", 
                "IBM",
            };

            count = Math.Clamp(count, 1, tickerDisplayNames.Count);

            List<Ticker> tickers = tickerDisplayNames
                .Shuffle()
                .Take(count)
                .Select(displayName => new Ticker() 
                { 
                    DisplayName = displayName,
                    Orders = new List<Order>()
                        .Concat(GenerateOrders(TransactionType.Buy, 10))
                        .Concat(GenerateOrders(TransactionType.Sell, 10))
                        .ToList()
                })
                .ToList();

            return tickers;
        }

        private List<Order> GenerateOrders(TransactionType transactionType, int count = 10)
        {
            int minPrice = transactionType == TransactionType.Buy ? 100 : 80;
            int maxPrice = transactionType == TransactionType.Buy ? 120 : 100;
            int minQuantity = 2;
            int maxQuantity = 100;

            Random random = new Random();

            List<Order> orders = new ();

            for (int i = 0; i < count; i++)
            {
                var order = new Order()
                {
                    Price = random.Next(minPrice, maxPrice),
                    Quantity = random.Next(minQuantity, maxQuantity),
                    TransactionType = transactionType
                };

                orders.Add(order);
            }

            return orders;
        }
    }
}
