namespace TradeSimulator.Shared.Models
{
    public interface ITradeHubClient
    {
        Task Connected(string username);
        Task Disconnected(string username);

        Task CreatedOrderBook(string username, OrderBook orderBook);
        Task DeletedOrderBook(string username, OrderBook orderBook);
        Task OpenedOrderBook(string username, OrderBook orderBook);
        Task ClosedOrderBook(string username, OrderBook orderBook);

        Task CreatedTransaction(string username, Transaction transaction);
        Task OpenedTransactionHistory(string username);
        Task ClosedTransactionHistory(string username);
    }
}
