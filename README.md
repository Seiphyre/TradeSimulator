# Trade Simulator

### Goal

This project fullfil the requirements of an assignment to evaluate my programming skills about C#, SignalR and WPF.

### Description

The solution is composed of the following projects:
- **TradeSimulator.Backend**: A backend application made Blazor
- **TradeSimulator.Frontend.WPF**: A frontend application made with WPF
- **TradeSimulator.Frontend.Blazor**: An alternative frontend application made with Blazor
- **TradeSimulator.Shared**: A class library to share common classes between projects

In the server app, you can see the log of the user's actions.

In the client app, you can connect to the server, manage order books, trade with different tickers, and see the history of your trades.

### Usage

**Running the build**

- Run the server application. It will open a terminal where you can see the url of the server. Don't close the terminal.
- (optional) Open the web browser of your choice, and open the server url. You will be able too see the server logs.
- Run the client application.
- In the client application, write the name of your choice and the url of the server. Then click connect to start the trading simulation.

**Running the project in Visual Studio**

- In visual studio, make sure the startup project is "Server & WPF", then click "Start".

### Features

- Basic authentication (no password, the broker name represent the user identity).
- Each user has its own order books and transaction history.
- Order books and Transactions lists are updated in real time.
- Order books and Transactions can be detached as sub-windows.
- All data come from server.
- All tables are sortable when clicking on a column header.
- The user can trade in the order book window.
- The app try to automatically try to reconnect to the server when it looses the connection. 