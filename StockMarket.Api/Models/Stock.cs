namespace StockMarket.Api.Models
{
    public class Stock
    {
        public string? Symbol { get; set; }
        public decimal Price { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
