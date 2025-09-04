
using CsvHelper.Configuration;
using StockMarket.Api.Models;
using CsvHelper.TypeConversion;
using CsvHelper;

public class SectMajMap : ClassMap<SectMaj>
{
    public SectMajMap()
    {
        AutoMap(System.Globalization.CultureInfo.InvariantCulture);
        Map(m => m.SectMajCd).Name("SECT_MAJ_CD").TypeConverter<SectMajCdConverter>();
        Map(m => m.SectMajNm).Name("SECT_MAJ_NM");
    }
}

public class SectMajCdConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null; // Return null if the string is empty or whitespace
        }
        return text.Trim(); // Trim whitespace from the string
    }
}
