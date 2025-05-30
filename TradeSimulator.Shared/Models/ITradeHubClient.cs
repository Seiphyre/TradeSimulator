namespace TradeSimulator.Shared.Models
{
    public interface ITradeHubClient
    {
        Task Connected(string username);
        Task Disconnected(string username);

        Task CreatedOrderBook(string username, OrderBook orderBook);
        Task DeletedOrderBook(string username, OrderBook orderBook);
    }
}
