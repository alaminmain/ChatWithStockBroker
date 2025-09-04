using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockMarket.Api.Models
{
    [Table("MAR_PRICE")]
    public class MarPrice
    {
        [Key]
        public int Id { get; set; }

        [Column("TRANS_DT")]
        public DateTime? TransDt { get; set; }

        [StringLength(20)]
        [Column("INST_CD")]
        public string? InstCd { get; set; }

        [Column("COMP_CD")]
        public int? CompCd { get; set; }

        [Column("OPEN")]
        public decimal? Open { get; set; }

        [Column("HIGH")]
        public decimal? High { get; set; }

        [Column("LOW")]
        public decimal? Low { get; set; }

        [Column("CLOSE")]
        public decimal? Close { get; set; }

        [Column("CHG")]
        public decimal? Chg { get; set; }

        [Column("VOL")]
        public decimal? Vol { get; set; }

        [Column("VAL")]
        public decimal? Val { get; set; }

        [StringLength(1)]
        [Column("GRP")]
        public string? Grp { get; set; }

        [StringLength(15)]
        [Column("MARK_TP")]
        public string? MarkTp { get; set; }

        [Column("AVRG_RT", TypeName = "decimal(12, 4)")]
        public decimal? AvrgRt { get; set; }

        [Column("GEN_INDX", TypeName = "decimal(10, 2)")]
        public decimal? GenIndx { get; set; }

        [Column("INDX_CHG")]
        public decimal? IndxChg { get; set; }

        [Column("MARK_CAP")]
        public decimal? MarkCap { get; set; }

        [Column("T_VAL", TypeName = "decimal(20, 2)")]
        public decimal? TVal { get; set; }

        [StringLength(15)]
        [Column("ISIN_CD")]
        public string? IsinCd { get; set; }

        [Column("DSEX_INDX", TypeName = "decimal(10, 2)")]
        public decimal? DsexIndx { get; set; }
    }
}
