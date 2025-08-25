
using Microsoft.AspNetCore.SignalR;
using StockMarket.Api.Hubs;
using StockMarket.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StockMarket.Api.Services
{
    public class StockService : IHostedService
    {
        private readonly IHubContext<StockHub> _hubContext;
        private Timer? _timer;
        private readonly List<Stock> _stocks = new List<Stock>
        {
            new Stock { Symbol = "MSFT", Price = 300.00m },
            new Stock { Symbol = "AAPL", Price = 150.00m },
            new Stock { Symbol = "GOOGL", Price = 2800.00m },
            new Stock { Symbol = "AMZN", Price = 3400.00m },
            new Stock { Symbol = "TSLA", Price = 700.00m }
        };

        public StockService(IHubContext<StockHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(UpdateStockPrices, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void UpdateStockPrices(object? state)
        {
            var random = new Random();
            foreach (var stock in _stocks)
            {
                var percentageChange = (decimal)(random.NextDouble() * 0.02 - 0.01);
                stock.Price *= (1 + percentageChange);
                stock.LastUpdate = DateTime.UtcNow;
            }

            _hubContext.Clients.All.SendAsync("ReceiveStockUpdate", _stocks);
        }

        public List<Stock> GetAllStocks()
        {
            return _stocks;
        }
    }
}
