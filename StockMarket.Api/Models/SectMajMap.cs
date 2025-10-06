
using CsvHelper.Configuration;
using StockMarket.Api.Models;
using CsvHelper.TypeConversion;
using CsvHelper;

public class SectMajMap : ClassMap<SectMaj>
{
    public SectMajMap()
    {
        AutoMap(System.Globalization.CultureInfo.InvariantCulture);
        Map(m => m.SectMajCd).Name("SECT_MAJ_CD");
        Map(m => m.SectMajNm).Name("SECT_MAJ_NM");
    }
}
