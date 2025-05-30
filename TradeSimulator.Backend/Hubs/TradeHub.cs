using Microsoft.AspNetCore.SignalR;

using TradeSimulator.Backend.Repositories;
using TradeSimulator.Shared.Models;

namespace TradeSimulator.Backend.Hubs
{
    public class TradeHub : Hub<ITradeHubClient>, ITradeHub
    {
        private readonly TickerRepository _tickerRepository;
        private readonly BrokerRepository _brokerRepository;
        private readonly OrderBookRepository _orderBookRepository;
        private readonly TransactionRepository _transactionRepository;



        /* ---------------------------------------------------------- */

        public TradeHub(TickerRepository tickerRepository, BrokerRepository brokerRepository, OrderBookRepository orderBookRepository, TransactionRepository transactionRepository)
        {
            _tickerRepository = tickerRepository;
            _brokerRepository = brokerRepository;
            _orderBookRepository = orderBookRepository;
            _transactionRepository = transactionRepository;
        }



        /* ---------------------------------------------------------- */

        public override async Task OnConnectedAsync()
        {
            await Clients.Others.Connected(UserName);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.Others.Disconnected(UserName);
        }



        /* ---------------------------------------------------------- */

        public IEnumerable<Ticker> GetTickers()
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
            var broker = GetBrokerById(id);

            if (broker == null)
                broker = CreateBroker(id);

            return broker;
        }

        public Broker GetBrokerById(string id)
        {
            var broker = _brokerRepository.GetById(id);

            return broker;
        }

        public Broker CreateBroker(string id)
        {
            var broker = _brokerRepository.Create(new Broker() { Id = id });

            if (broker != null)
            {
                CreateRandomTransactionsForBroker(broker.Id, tickerCount: 2, transactionPerTicker: 3);
            }

            return broker;
        }



        /* ---------------------------------------------------------- */

        public IEnumerable<OrderBook> GetOrderBooks(string brokerId = null)
        {
            var orderbooks = _orderBookRepository.GetAll(brokerId);

            return orderbooks;
        }

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

            await Clients.All.CreatedOrderBook(UserName, orderbook);

            return orderbook;
        }

        public async Task DeleteOrderBook(string orderBookId)
        {
            var orderBook = _orderBookRepository.GetById(orderBookId);

            if (orderBook == null)
                throw new HubException("OrderBook not found.");

            _orderBookRepository.Delete(orderBook.Id);

            await Clients.All.DeletedOrderBook(UserName, orderBook);
        }



        /* ---------------------------------------------------------- */

        public IEnumerable<Transaction> GetTransactions(string brokerId = null)
        {
            return _transactionRepository.GetAll(brokerId);
        }

        private List<Transaction> CreateRandomTransactionsForBroker(string brokerId, int tickerCount = 2, int transactionPerTicker = 3)
        {
            var Broker = _brokerRepository.GetById(brokerId);

            if (Broker == null)
                throw new HubException("Broker not found.");

            var transactions = _transactionRepository.CreateRandomTransactions(brokerId, tickerCount, transactionPerTicker);

            transactions.ForEach(transaction => Console.WriteLine(transaction.TickerDisplayName + ": " + transaction.Price));

            return transactions;
        }


        /* ---------------------------------------------------------- */

        private string UserName => Context.User?.Identity?.Name ?? Context.ConnectionId;
    }
}
