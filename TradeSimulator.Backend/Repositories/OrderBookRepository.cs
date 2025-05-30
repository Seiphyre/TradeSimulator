using TradeSimulator.Shared.Models;

namespace TradeSimulator.Backend.Repositories
{
    public class OrderBookRepository: RepositoryBase<OrderBook>
    {
        public virtual List<OrderBook> GetAll(string brokerId = null)
        {
            if (Entities == null || Entities.Count == 0)
                return new List<OrderBook>();

            IEnumerable<OrderBook> entities = new List<OrderBook>(Entities);

            if (brokerId != null)
                entities = entities.Where(entity => entity.BrokerId == brokerId);

            return entities.ToList();
        }
    }
}
