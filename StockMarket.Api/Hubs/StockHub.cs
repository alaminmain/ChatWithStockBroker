using Microsoft.AspNetCore.SignalR;
using StockMarket.Api.Services;
using System.Threading.Tasks;

namespace StockMarket.Api.Hubs
{
    public class StockHub : Hub
    {
        private readonly StockService _stockService;

        public StockHub(StockService stockService)
        {
            _stockService = stockService;
        }

        public async Task GetInitialStocks()
        {
            await Clients.Caller.SendAsync("ReceiveStockUpdate", _stockService.GetAllStocks());
        }
    }
}