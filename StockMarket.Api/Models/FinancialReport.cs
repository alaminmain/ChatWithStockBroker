using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockMarket.Api.Models
{
    public class FinancialReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CompId { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public string StatementType { get; set; } = string.Empty; // e.g., "Income Statement", "Balance Sheet"

        [ForeignKey("CompId")]
        public virtual Comp? Comp { get; set; }

        public virtual ICollection<FinancialReportEntry> Entries { get; set; } = new List<FinancialReportEntry>();
    }
}
