using TradeSimulator.Shared.Extensions;
using TradeSimulator.Shared.Models;

namespace TradeSimulator.Backend.Repositories
{
    public class TransactionRepository : RepositoryBase<Transaction>
    {
        private readonly TickerRepository _tickerRepository;

        public TransactionRepository(TickerRepository tickerRepository) 
        {
            _tickerRepository = tickerRepository;
        }

        public virtual List<Transaction> GetAll(string brokerId = null)
        {
            if (Entities == null || Entities.Count == 0)
                return new List<Transaction>();

            IEnumerable<Transaction> entities = new List<Transaction>(Entities);

            if (brokerId != null)
                entities = entities.Where(entity => entity.BrokerId == brokerId);

            return entities.ToList();
        }

        public List<Transaction> CreateRandomTransactions(string brokerId, int tickerCount = 2, int transactionPerTicker = 3)
        {
            List<Ticker> tickers = _tickerRepository.GetAll();

            tickerCount = Math.Clamp(tickerCount, 1, tickers.Count);
            tickers = tickers.Shuffle().Take(tickerCount).ToList();

            var newTransactions = new List<Transaction>();
            var random = new Random();

            foreach (var ticker in tickers)
            {
                int orderCount = Math.Clamp(transactionPerTicker, 1, ticker.Orders.Count);

                IEnumerable<Transaction> tickerTransactions = ticker.Orders.Shuffle()
                    .Take(orderCount)
                    .Select(order => new Transaction()
                    {
                        TickerDisplayName = ticker.DisplayName,

                        Price = order.Price,
                        Quantity = order.Quantity,
                        TransactionType = order.TransactionType,

                        CreationDate = DateTime.Now
                            .AddDays(-1 * (random.Next(1, 7)))
                            .AddHours(-1 * (random.Next(0, 23)))
                            .AddMinutes(-1 * (random.Next(0, 59)))
                            .AddSeconds(-1 * (random.Next(0, 59)))
                    });

                newTransactions.AddRange(tickerTransactions);
            }

            Entities.AddRange(newTransactions);

            return newTransactions;
        }

    }
}
