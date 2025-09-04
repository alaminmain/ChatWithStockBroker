using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockMarket.Api.Models
{
    [Table("SECT_MIN")]
    public class SectMin
    {
        [Key]
        [StringLength(2)]
        [Column("SECT_MIN_CD")]
        public string SectMinCd { get; set; }

        [StringLength(2)]
        [Column("SECT_MAJ_CD")]
        public string? SectMajCd { get; set; }

        [Required]
        [StringLength(40)]
        [Column("SECT_MIN_NM")]
        public string SectMinNm { get; set; }
    }
}