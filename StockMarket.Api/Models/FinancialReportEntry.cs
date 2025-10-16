using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockMarket.Api.Models
{
    public class FinancialReportEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int FinancialReportId { get; set; }

        [Required]
        public string StandardAccountName { get; set; } = string.Empty;

        public string OriginalAccountName { get; set; } = string.Empty;

        public decimal Value { get; set; }

        [ForeignKey("FinancialReportId")]
        public virtual FinancialReport? FinancialReport { get; set; }
    }
}
