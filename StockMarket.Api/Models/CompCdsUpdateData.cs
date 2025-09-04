namespace StockMarket.Api.Models
{
    public class CompCdsUpdateData
    {
        public int CompCd { get; set; }
        public string IsinCd { get; set; }
        public DateTime? StartDt { get; set; }
        public int? Ldrn { get; set; }
    }
}