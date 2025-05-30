using System.Collections;

namespace TradeSimulator.Shared.Models
{
    public interface ITradeHub
    {
        IEnumerable<Ticker> GetTickers();
        Ticker GetTickerById(string id);



        Broker GetOrCreateBroker(string id);



        OrderBook GetOrderBook(string orderBookId);
        IEnumerable<OrderBook> GetOrderBooks(string brokerId = null);
        Task<OrderBook> CreateOrderBook(string brokerId, string tickerId);
        Task DeleteOrderBook(string orderBookId);
        Task OpenOrderBook(string orderBookId);
        Task CloseOrderBook(string orderBookId);



        IEnumerable<Transaction> GetTransactions(string brokerId = null);
        Task OpenTransactionHistory();
        Task CloseTransactionHistory();
    }
}
