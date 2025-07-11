﻿using Microsoft.AspNetCore.SignalR;

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
            IEnumerable<Ticker> tickers = _tickerRepository.GetAll();

            return tickers;
        }

        public Ticker GetTickerById(string id)
        {
            Ticker ticker = _tickerRepository.GetById(id);

            if (ticker is null)
                throw new HubException("Ticker not found.");

            return ticker;
        }



        /* ---------------------------------------------------------- */

        public Broker GetOrCreateBroker(string id)
        {
            var broker = GetBrokerById(id);

            if (broker == null)
                broker = CreateBroker(id);

            return broker;
        }

        private Broker GetBrokerById(string id)
        {
            var broker = _brokerRepository.GetById(id);

            return broker;
        }

        private Broker CreateBroker(string id)
        {
            var broker = _brokerRepository.Create(new Broker() { Id = id });

            if (broker != null)
            {
                CreateRandomTransactionsForBroker(broker.Id, tickerCount: 2, transactionPerTicker: 3);
            }

            return broker;
        }



        /* ---------------------------------------------------------- */

        public OrderBook GetOrderBook(string orderBookId)
        {
            var orderBook = _orderBookRepository.GetById(orderBookId);

            if (orderBook == null)
                throw new HubException("OrderBook not found.");

            return orderBook;
        }

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
                TickerId = tickerId,

                TickerDisplayName = Ticker.DisplayName,

                IsOpen = false
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

        public async Task OpenOrderBook(string orderBookId)
        {
            var orderBook = _orderBookRepository.GetById(orderBookId);

            if (orderBook == null)
                throw new HubException("OrderBook not found.");

            if (orderBook.IsOpen)
                return;

            orderBook.IsOpen = true;

            await Clients.All.OpenedOrderBook(UserName, orderBook);
        }

        public async Task CloseOrderBook(string orderBookId)
        {
            var orderBook = _orderBookRepository.GetById(orderBookId);

            if (orderBook == null)
                throw new HubException("OrderBook not found.");

            if (!orderBook.IsOpen)
                return;

            orderBook.IsOpen = false;

            await Clients.All.ClosedOrderBook(UserName, orderBook);
        }



        /* ---------------------------------------------------------- */

        public IEnumerable<Transaction> GetTransactions(string brokerId = null)
        {
            IEnumerable<Transaction> transactions = _transactionRepository.GetAll(brokerId);

            return transactions;
        }

        public async Task<Transaction> CreateTransaction(string brokerId, string tickerDisplayName, decimal price, int quantity, TransactionType transactionType)
        {
            var Broker = _brokerRepository.GetById(brokerId);

            if (Broker == null)
                throw new HubException("Broker not found.");

            var transaction = _transactionRepository.Create(new Transaction()
            {
                BrokerId = brokerId,
                CreationDate = DateTime.Now,

                TickerDisplayName = tickerDisplayName,
                Price = price,
                Quantity = quantity,
                TransactionType = transactionType
            });

            await Clients.All.CreatedTransaction(UserName, transaction);

            return transaction;
        }

        private List<Transaction> CreateRandomTransactionsForBroker(string brokerId, int tickerCount = 2, int transactionPerTicker = 3)
        {
            var Broker = _brokerRepository.GetById(brokerId);

            if (Broker == null)
                throw new HubException("Broker not found.");

            var transactions = _transactionRepository.CreateRandomTransactions(brokerId, tickerCount, transactionPerTicker);

            return transactions;
        }

        public async Task OpenTransactionHistory()
        {
            await Clients.All.OpenedTransactionHistory(UserName);
        }

        public async Task CloseTransactionHistory()
        {
            await Clients.All.ClosedTransactionHistory(UserName);
        }


        /* ---------------------------------------------------------- */

        private string UserName => Context.User?.Identity?.Name ?? Context.ConnectionId;
    }
}
