
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace StockMarket.Api.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendPrivateMessage(string sender, string receiver, string message)
        {
            var user = Context.User?.Identity?.Name;
            if (user == null)
            {
                return;
            }
            await Clients.User(receiver).SendAsync("ReceivePrivateMessage", user, message);
        }
    }
}
