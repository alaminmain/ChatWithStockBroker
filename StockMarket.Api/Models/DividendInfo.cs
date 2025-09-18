using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockMarket.Api.Models
{
    [Table("DIVIDEND_INFO")]
    public class DividendInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("COMP_CD")]
        public int? CompCd { get; set; }

        [Column("AGM_DT")]
        public DateTime? AgmDt { get; set; }

        [Column("FYEAR")]
        public string? Fyear { get; set; }

        [Column("CFYEAR")]
        public string? Cfyear { get; set; }

        [Column("DIV_TYPE")]
        public string? DivType { get; set; }

        [Column("RATE")]
        public decimal? Rate { get; set; }

        [Column("RATIO1")]
        public decimal? Ratio1 { get; set; }

        [Column("RATIO2")]
        public decimal? Ratio2 { get; set; }

        [Column("PREMIUM")]
        public decimal? Premium { get; set; }

        [Column("PAYMENT_DT")]
        public DateTime? PaymentDt { get; set; }

        [Column("BOK_CL_FDT")]
        public DateTime? BokClFdt { get; set; }

        [Column("BOK_CL_TDT")]
        public DateTime? BokClTdt { get; set; }

        [Column("OP_NAME")]
        public string? OpName { get; set; }

        [Column("DISCOUNT")]
        public decimal? Discount { get; set; }

        [Column("REMARKS")]
        public string? Remarks { get; set; }

        [Column("BS_COMP_CD")]
        public int? BsCompCd { get; set; }

        [ForeignKey("CompCd")] // Add this attribute
        public Comp? Comp { get; set; }
    }
}
