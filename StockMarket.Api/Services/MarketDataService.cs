using Microsoft.EntityFrameworkCore;
using StockMarket.Api.Data;
using StockMarket.Api.Models;
using HtmlAgilityPack;
using System.Globalization;
using System.Data;
using System.ComponentModel;
using Microsoft.Data.SqlClient;

namespace StockMarket.Api.Services
{
    public class MarketDataService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MarketDataService> _logger;

        public MarketDataService(ApplicationDbContext context, ILogger<MarketDataService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task UpdateMarPriceFromAmarstockAsync()
        {
            _logger.LogInformation("Starting market price update from Amarstock.");
            var url = "https://www.amarstock.com/latest-share-price";
            var httpClient = new HttpClient();
            string html;

            try
            {
                html = await httpClient.GetStringAsync(url);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching HTML from Amarstock: {Message}", ex.Message);
                return;
            }

            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(html);

            var table = htmlDocument.DocumentNode.SelectSingleNode("//table[@id='g_content_ctl00_gvData']");

            if (table == null)
            {
                _logger.LogError("Could not find the market data table on Amarstock. XPath might be incorrect.");
                return;
            }

            var marPriceRecords = new List<MarPrice>();
            var rows = table.SelectNodes(".//tr");

            if (rows == null || !rows.Any())
            {
                _logger.LogWarning("No rows found in the market data table on Amarstock.");
                return;
            }

            // Skip header row
            foreach (var row in rows.Skip(1))
            {
                var cells = row.SelectNodes(".//td");
                if (cells == null || cells.Count < 10) // Ensure enough cells
                {
                    _logger.LogWarning("Skipping row due to insufficient cells: {RowHtml}", row.InnerHtml);
                    continue;
                }

                var instCd = cells[1].InnerText.Trim(); // Instrument Code
                var compNm = cells[0].InnerText.Trim(); // Company Name

                var comp = await _context.Comps.FirstOrDefaultAsync(c => c.InstrCd == instCd);
                if (comp == null)
                {
                    comp = await _context.Comps.FirstOrDefaultAsync(c => c.CompNm == compNm);
                }

                if (comp == null)
                {
                    _logger.LogWarning("Company not found for InstrCd: {InstrCd} or CompNm: {CompNm}. Skipping record.", instCd, compNm);
                    continue;
                }

                var marPrice = new MarPrice
                {
                    TransDt = DateTime.Today,
                    InstCd = instCd,
                    CompCd = comp.CompCd,
                    Open = ParseDecimalFromString(cells[2].InnerText),
                    High = ParseDecimalFromString(cells[3].InnerText),
                    Low = ParseDecimalFromString(cells[4].InnerText),
                    Close = ParseDecimalFromString(cells[5].InnerText),
                    Chg = ParseDecimalFromString(cells[6].InnerText),
                    Vol = ParseDecimalFromString(cells[7].InnerText),
                    Val = ParseDecimalFromString(cells[8].InnerText),
                    // Add other fields as per your MarPrice model and Amarstock table structure
                };
                marPriceRecords.Add(marPrice);
            }

            if (marPriceRecords.Any())
            {
                await BulkInsertMarPriceDirect(marPriceRecords);
                _logger.LogInformation("{Count} market price records updated successfully from Amarstock.", marPriceRecords.Count);
            }
            else
            {
                _logger.LogInformation("No new market price records to update from Amarstock.");
            }
        }

        private async Task BulkInsertMarPriceDirect(List<MarPrice> marPriceRecords)
        {
            var incomingKeys = marPriceRecords
                .Select(r => new { r.TransDt, r.InstCd, r.CompCd })
                .ToHashSet();

            var uniqueTransDts = incomingKeys.Select(k => k.TransDt).ToHashSet();
            var existingMarPrices = await _context.MarPrices
                .Where(mp => uniqueTransDts.Contains(mp.TransDt))
                .ToListAsync();

            var existingCompositeKeys = existingMarPrices
                .Select(mp => new { mp.TransDt, mp.InstCd, mp.CompCd })
                .ToHashSet();

            var newRecordsToInsert = marPriceRecords
                .Where(r => !existingCompositeKeys.Contains(new { r.TransDt, r.InstCd, r.CompCd }))
                .ToList();

            if (!newRecordsToInsert.Any())
            {
                _logger.LogInformation("No new records to insert into MarPrice table.");
                return;
            }

            DataTable marPriceDataTable = ConvertToDataTable(newRecordsToInsert);

            var connectionString = _context.Database.GetConnectionString();

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.BulkCopyTimeout = 300;
                    bulkCopy.DestinationTableName = "MAR_PRICE";

                    bulkCopy.ColumnMappings.Add("TransDt", "TRANS_DT");
                    bulkCopy.ColumnMappings.Add("InstCd", "INST_CD");
                    bulkCopy.ColumnMappings.Add("CompCd", "COMP_CD");
                    bulkCopy.ColumnMappings.Add("Open", "OPEN");
                    bulkCopy.ColumnMappings.Add("High", "HIGH");
                    bulkCopy.ColumnMappings.Add("Low", "LOW");
                    bulkCopy.ColumnMappings.Add("Close", "CLOSE");
                    bulkCopy.ColumnMappings.Add("Chg", "CHG");
                    bulkCopy.ColumnMappings.Add("Vol", "VOL");
                    bulkCopy.ColumnMappings.Add("Val", "VAL");
                    bulkCopy.ColumnMappings.Add("Grp", "GRP");
                    bulkCopy.ColumnMappings.Add("MarkTp", "MARK_TP");
                    bulkCopy.ColumnMappings.Add("AvrgRt", "AVRG_RT");
                    bulkCopy.ColumnMappings.Add("GenIndx", "GEN_INDX");
                    bulkCopy.ColumnMappings.Add("IndxChg", "INDX_CHG");
                    bulkCopy.ColumnMappings.Add("MarkCap", "MARK_CAP");
                    bulkCopy.ColumnMappings.Add("TVal", "T_VAL");
                    bulkCopy.ColumnMappings.Add("IsinCd", "ISIN_CD");
                    bulkCopy.ColumnMappings.Add("DsexIndx", "DSEX_INDX");

                    await bulkCopy.WriteToServerAsync(marPriceDataTable);
                }
            }
            _logger.LogInformation("Bulk insert to MarPrice table completed. {Count} new records inserted.", newRecordsToInsert.Count);
        }

        private decimal? ParseDecimalFromString(string value)
        {
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }
            return null;
        }

        private int? ParseIntFromString(string value)
        {
            if (int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }
            return null;
        }

        private DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
