using CsvHelper.Configuration;
using StockMarket.Api.Models;

namespace StockMarket.Api.Models.Maps
{
    public class DividendInfoMap : ClassMap<DividendInfo>
    {
        public DividendInfoMap()
        {
            Map(m => m.CompCd).Name("comp_cd");
            Map(m => m.AgmDt).Name("agm_dt").TypeConverterOption.Format("M/d/yyyy");
            Map(m => m.Fyear).Name("fyear");
            Map(m => m.Cfyear).Name("cfyear");
            Map(m => m.DivType).Name("div_type");
            Map(m => m.Rate).Name("rate");
            Map(m => m.Ratio1).Name("ratio1");
            Map(m => m.Ratio2).Name("ratio2");
            Map(m => m.Premium).Name("premium");
            Map(m => m.PaymentDt).Name("payment_dt").TypeConverterOption.Format("M/d/yyyy");
            Map(m => m.BokClFdt).Name("bok_cl_fdt").TypeConverterOption.Format("M/d/yyyy");
            Map(m => m.BokClTdt).Name("bok_cl_tdt").TypeConverterOption.Format("M/d/yyyy");
            Map(m => m.OpName).Name("op_name");
            Map(m => m.Discount).Name("discount");
            Map(m => m.Remarks).Name("remarks");
            Map(m => m.BsCompCd).Name("bs_comp_cd");
        }
    }
}
