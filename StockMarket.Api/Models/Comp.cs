using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockMarket.Api.Models
{
    [Table("COMP")]
    public class Comp
    {
        [Key]
        public int Id { get; set; }

        [Column("COMP_CD")]
        public int? CompCd { get; set; }

        [StringLength(80)]
        [Column("COMP_NM")]
        public string? CompNm { get; set; }

        [StringLength(2)]
        [Column("SECT_MAJ_CD")]
        public string? SectMajCd { get; set; }

        [StringLength(2)]
        [Column("SECT_MIN_CD")]
        public string? SectMinCd { get; set; }

        [Required]
        [StringLength(20)]
        [Column("INSTR_CD")]
        public string InstrCd { get; set; }

        [Required]
        [StringLength(2)]
        [Column("CAT_TP")]
        public string CatTp { get; set; }

        [StringLength(50)]
        [Column("ADD1")]
        public string? Add1 { get; set; }

        [StringLength(50)]
        [Column("ADD2")]
        public string? Add2 { get; set; }

                [StringLength(500)]
        public string? RegOff { get; set; }

        [StringLength(100)]
        [Column("PRN_STH")]
        public string? PrnSth { get; set; }

        [Column("OPN_DT")]
        public DateTime? OpnDt { get; set; }

        [StringLength(1)]
        [Column("TAX_HDAY")]
        public string? TaxHday { get; set; }

        [StringLength(50)]
        [Column("TEL")]
        public string? Tel { get; set; }

        [StringLength(30)]
        [Column("TLX")]
        public string? Tlx { get; set; }

        [StringLength(50)]
        [Column("E_MAIL")]
        public string? EMail { get; set; }

        [StringLength(50)]
        [Column("PROD")]
        public string? Prod { get; set; }

        [StringLength(30)]
        [Column("PRO_VOL")]
        public string? ProVol { get; set; }

        [StringLength(50)]
        [Column("SPNR")]
        public string? Spnr { get; set; }

        [Column("ATHO_CAP", TypeName = "decimal(17, 2)")]
        public decimal? AthoCap { get; set; }

        [Column("PAID_CAP", TypeName = "decimal(17, 2)")]
        public decimal PaidCap { get; set; }

        [Column("NO_SHRS", TypeName = "decimal(17, 2)")]
        public decimal NoShrs { get; set; }

        [Column("FC_VAL", TypeName = "decimal(12, 2)")]
        public decimal FcVal { get; set; }

        [Column("MLOT")]
        public int Mlot { get; set; }

        [Column("SBASE_RT", TypeName = "decimal(10, 4)")]
        public decimal SbaseRt { get; set; }

        [Column("FLOT_DT_FM")]
        public DateTime? FlotDtFm { get; set; }

        [Column("FLOT_DT_TO")]
        public DateTime? FlotDtTo { get; set; }

        [Column("BOK_CL_FDT")]
        public DateTime? BokClFdt { get; set; }

        [Column("BOK_CL_TDT")]
        public DateTime? BokClTdt { get; set; }

        [Column("MARGIN")]
        public int? Margin { get; set; }

        [Column("AVG_RT", TypeName = "decimal(12, 4)")]
        public decimal? AvgRt { get; set; }

        [Column("RT_UPD_DT")]
        public DateTime? RtUpdDt { get; set; }

        [StringLength(1)]
        [Column("FLAG")]
        public string? Flag { get; set; }

        [StringLength(30)]
        [Column("AUDITOR")]
        public string? Auditor { get; set; }

        [Column("NS_ICB", TypeName = "decimal(17, 2)")]
        public decimal? NsIcb { get; set; }

        [Column("NS_UNIT", TypeName = "decimal(17, 2)")]
        public decimal? NsUnit { get; set; }

        [Column("NS_MUTUAL", TypeName = "decimal(17, 2)")]
        public decimal? NsMutual { get; set; }

        [Column("PMARGIN")]
        public int? Pmargin { get; set; }

        [Column("RISSU_DT_FM")]
        public DateTime? RissuDtFm { get; set; }

        [Column("RISSU_DT_TO")]
        public DateTime? RissuDtTo { get; set; }

        [Column("PREMIUM", TypeName = "decimal(6, 2)")]
        public decimal? Premium { get; set; }

        [StringLength(1)]
        [Column("CFLAG")]
        public string? Cflag { get; set; }

        [Column("MAR_FLOAT")]
        public decimal? MarFloat { get; set; }

        [StringLength(1)]
        [Column("MON_TO")]
        public string? MonTo { get; set; }

        [StringLength(1)]
        [Column("TRADE_METH")]
        public string? TradeMeth { get; set; }

        [StringLength(20)]
        [Column("CSEINSTR_CD")]
        public string? CseInstrCd { get; set; }

        [Column("INDX_LST", TypeName = "decimal(13, 4)")]
        public decimal? IndxLst { get; set; }

        [Column("BASE_UPD_DT")]
        public DateTime? BaseUpdDt { get; set; }

        [StringLength(1)]
        [Column("CDS")]
        public string? Cds { get; set; }

        [Column("CTL_RT", TypeName = "decimal(10, 2)")]
        public decimal? CtlRt { get; set; }

        [StringLength(1)]
        [Column("NET")]
        public string? Net { get; set; }

        [StringLength(1)]
        [Column("GRP")]
        public string? Grp { get; set; }

        [StringLength(6)]
        [Column("MERCHAN_BANK_ID")]
        public string? MerchanBankId { get; set; }

        [StringLength(1)]
        [Column("OTC")]
        public string? Otc { get; set; }

        [Column("IPO_CUTOFF_DT")]
        public DateTime? IpoCutoffDt { get; set; }

        [StringLength(2)]
        [Column("TRADE_PLATFORM")]
        public string? TradePlatform { get; set; }

        [Column("PE_RATIO")]
        public decimal? PeRatio { get; set; }

        [StringLength(15)]
        [Column("ISIN_CD")]
        public string? IsinCd { get; set; }

        [Column("START_DT")]
        public DateTime? StartDt { get; set; }

        [Column("LDRN")]
        public int? Ldrn { get; set; }

        [ForeignKey("SectMajCd")]
        public virtual SectMaj SectMaj { get; set; }
    }
}