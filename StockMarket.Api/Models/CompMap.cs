
using CsvHelper.Configuration;
using StockMarket.Api.Models;
using CsvHelper.TypeConversion;

public class CompMap : ClassMap<Comp>
{
    public CompMap()
    {
        AutoMap(System.Globalization.CultureInfo.InvariantCulture);
        Map(m => m.SectMaj).Ignore();
        Map(m => m.SectMajCd).TypeConverter<SectMajCdConverter>();
    }
}
