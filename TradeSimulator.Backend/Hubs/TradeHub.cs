using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using TradeSimulator.Backend.Repositories;
using TradeSimulator.Shared.Models;

namespace TradeSimulator.Backend.Hubs
{
    public class TradeHub : Hub
    {
        private readonly TickerRepository _tickerRepository;
        private readonly BrokerRepository _brokerRepository;
        private readonly OrderBookRepository _orderBookRepository;



        /* ---------------------------------------------------------- */

        public TradeHub(TickerRepository tickerRepository, BrokerRepository brokerRepository, OrderBookRepository orderBookRepository)
        {
            _tickerRepository = tickerRepository;
            _brokerRepository = brokerRepository;
            _orderBookRepository = orderBookRepository;
        }



        /* ---------------------------------------------------------- */

        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("OnConnected", UserName, "Connected");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.SendAsync("OnDisconnected", UserName, "Disconnected");
        }



        /* ---------------------------------------------------------- */

        public IEnumerable<Ticker> GetAllTickers()
        {
            return _tickerRepository.GetAll();
        }

        public Ticker GetTickerById(string id)
        {
            return _tickerRepository.GetById(id);
        }



        /* ---------------------------------------------------------- */

        public Broker GetOrCreateBroker(string id)
        {
            var broker = _brokerRepository.GetById(id);

            if (broker == null)
                broker = _brokerRepository.Create(new Broker() { Id = id });

            return broker;
        }

        public List<OrderBook> GetOrderBooks(string brokerId = null)
        {
            var orderbooks = _orderBookRepository.GetAll(brokerId);

            return orderbooks;
        }



        /* ---------------------------------------------------------- */

        public async Task<OrderBook> CreateOrderBook(string brokerId, string tickerId)
        {
            var Broker = _brokerRepository.GetById(brokerId);

            if (Broker == null)
                throw new HubException("Broker not found.");

            var Ticker = _tickerRepository.GetById(tickerId);

            if (Ticker == null)
                throw new HubException("Ticker not found.");

            var orderbook = _orderBookRepository.Create(new OrderBook()
            {
                BrokerId = brokerId,
                TickerId = tickerId
            });

            await Clients.All.SendAsync("OnCreateOrderBook", orderbook);

            return orderbook;
        }

        public async Task DeleteOrderBook(string orderBookId)
        {
            var orderBook = _orderBookRepository.GetById(orderBookId);

            if (orderBook == null)
                throw new HubException("OrderBook not found.");

            _orderBookRepository.Delete(orderBook.Id);

            await Clients.All.SendAsync("OnDeleteOrderBook", orderBook);
        }



        /* ---------------------------------------------------------- */

        private string UserName => Context.User?.Identity?.Name ?? Context.ConnectionId;
    }
}
