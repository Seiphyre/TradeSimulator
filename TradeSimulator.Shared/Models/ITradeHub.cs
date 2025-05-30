using System.Collections;

namespace TradeSimulator.Shared.Models
{
    public interface ITradeHub
    {
        IEnumerable<Ticker> GetTickers();
        Ticker GetTickerById(string id);



        Broker GetOrCreateBroker(string id);
        Broker GetBrokerById(string id);
        Broker CreateBroker(string id);



        IEnumerable<OrderBook> GetOrderBooks(string brokerId = null);
        Task<OrderBook> CreateOrderBook(string brokerId, string tickerId);
        Task DeleteOrderBook(string orderBookId);
        Task OpenOrderBook(string orderBookId);



        IEnumerable<Transaction> GetTransactions(string brokerId = null);
        Task OpenTransactionHistory();
    }
}
