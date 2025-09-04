using CsvHelper.Configuration;
using StockMarket.Api.Models;
using System.Globalization;

namespace StockMarket.Api.Models.Maps
{
    public sealed class MarPriceMap : ClassMap<MarPrice>
    {
        public MarPriceMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.TransDt).Name("trans_dt").TypeConverterOption.Format("M/d/yyyy");
            Map(m => m.InstCd).Name("inst_cd");
            Map(m => m.CompCd).Name("comp_cd");
            Map(m => m.Open).Name("open");
            Map(m => m.High).Name("high");
            Map(m => m.Low).Name("low");
            Map(m => m.Close).Name("close");
            Map(m => m.Chg).Name("chg");
            Map(m => m.Vol).Name("vol");
            Map(m => m.Val).Name("val");
            Map(m => m.Grp).Name("grp");
            Map(m => m.MarkTp).Name("mark_tp");
            Map(m => m.AvrgRt).Name("avrg_rt");
            Map(m => m.GenIndx).Name("gen_indx");
            Map(m => m.IndxChg).Name("indx_chg");
            Map(m => m.MarkCap).Name("mark_cap");
            Map(m => m.TVal).Name("t_val");
            Map(m => m.IsinCd).Name("isin_cd");
            Map(m => m.DsexIndx).Name("dsex_indx");
        }
    }
}