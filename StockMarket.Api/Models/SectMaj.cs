using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockMarket.Api.Models
{
    [Table("SECT_MAJ")]
    public class SectMaj
    {
        [Key]
        [StringLength(2)]
        [Column("SECT_MAJ_CD")]
        public string SectMajCd { get; set; } = string.Empty;

        [Required]
        [StringLength(40)]
        [Column("SECT_MAJ_NM")]
        public string SectMajNm { get; set; } = string.Empty;

        public virtual ICollection<Comp> Comps { get; set; } = new List<Comp>();
    }
}