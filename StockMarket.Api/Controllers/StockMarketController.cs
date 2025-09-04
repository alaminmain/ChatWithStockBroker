using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockMarket.Api.Data;
using StockMarket.Api.Models;
using System.Globalization;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using StockMarket.Api.Models.Maps;
using Microsoft.Data.SqlClient;
using System.Data;
using System.ComponentModel;

namespace StockMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockMarketController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StockMarketController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("process-file")]
        public async Task<IActionResult> ProcessFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var filePath = Path.GetTempFileName();

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileName = file.FileName.ToLower();

            if (fileName.Contains("marprice"))
            {
                BulkInsertMarPrice(filePath);
                return Ok("Successfully processed marprice file.");
            }
            else if (fileName.Contains("sect_maj"))
            {
                BulkInsertSectMajFromCsv(filePath);
                return Ok("Successfully processed sect_maj file.");
            }
            else if (fileName.Contains("comp") && fileName.EndsWith(".xlsx"))
            {
                BulkInsertCompFromExcel(filePath);
                return Ok("Successfully processed comp Excel file.");
            }
            else if (fileName.Contains("comp"))
            {
                BulkInsertCompFromCsv(filePath);
                return Ok("Successfully processed comp CSV file.");
            }
            else
            {
                return BadRequest("Unknown file type.");
            }
        }

        private void BulkInsertMarPrice(string filePath)
        {
            var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                PrepareHeaderForMatch = args => args.Header.ToLower()
            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<MarPriceMap>();
                var marPriceRecords = csv.GetRecords<MarPrice>().ToList();

                // --- Start of new idempotency logic (Revised) ---
                var incomingKeys = marPriceRecords
                    .Select(r => new { r.TransDt, r.InstCd, r.CompCd })
                    .ToHashSet();

                // Fetch existing records that match any of the incoming TransDt values
                // This might still fetch a lot of data if TransDt is not very selective
                var uniqueTransDts = incomingKeys.Select(k => k.TransDt).ToHashSet();
                var existingMarPrices = _context.MarPrices
                    .Where(mp => uniqueTransDts.Contains(mp.TransDt))
                    .ToList(); // Execute query and bring to memory

                // Create a HashSet of existing composite keys for efficient lookup
                var existingCompositeKeys = existingMarPrices
                    .Select(mp => new { mp.TransDt, mp.InstCd, mp.CompCd })
                    .ToHashSet();

                // Filter out records that already exist
                var newRecordsToInsert = marPriceRecords
                    .Where(r => !existingCompositeKeys.Contains(new { r.TransDt, r.InstCd, r.CompCd }))
                    .ToList();
                // --- End of new idempotency logic (Revised) ---

                if (!newRecordsToInsert.Any())
                {
                    // All records already exist or no new records to insert
                    return;
                }

                DataTable marPriceDataTable = ConvertToDataTable(newRecordsToInsert);

                var connectionString = _context.Database.GetConnectionString();

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.BulkCopyTimeout = 300; // 5 minutes
                        bulkCopy.DestinationTableName = "MAR_PRICE";

                        // Column Mappings
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

                        bulkCopy.WriteToServer(marPriceDataTable);
                    }
                }
            }
        }

        private void BulkInsertSectMajFromCsv(string filePath)
        {
            var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                PrepareHeaderForMatch = args => args.Header.ToLower()
            };
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<SectMajMap>();
                var records = csv.GetRecords<SectMaj>().ToList();
                _context.SectMajs.AddRange(records);
                _context.SaveChanges();
            }
        }

        private void BulkInsertCompFromCsv(string filePath)
        {
            var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                PrepareHeaderForMatch = args => args.Header.ToLower()
            };
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<CompMap>();
                var records = new List<Comp>();
                int batchSize = 5000;
                int count = 0;

                foreach (var record in csv.GetRecords<Comp>())
                {
                    record.SectMaj = null; // Explicitly set navigation property to null
                    records.Add(record);
                    count++;

                    if (count % batchSize == 0)
                    {
                        _context.Comps.AddRange(records);
                        _context.SaveChanges();
                        records.Clear();
                    }
                }

                if (records.Any())
                {
                    _context.Comps.AddRange(records);
                    _context.SaveChanges();
                }
            }
        }

        private void BulkInsertCompFromExcel(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fileStream);
                ISheet sheet = workbook.GetSheetAt(0);

                var records = new List<Comp>();
                int batchSize = 5000;

                // Assuming the first row is the header
                for (int row = 1; row <= sheet.LastRowNum; row++)
                {
                    IRow excelRow = sheet.GetRow(row);
                    if (excelRow == null) continue;

                    var comp = new Comp
                    {
                        CompCd = ParseInt(excelRow.GetCell(0)),
                        CompNm = excelRow.GetCell(1)?.ToString(),
                        SectMajCd = excelRow.GetCell(2)?.ToString(),
                        SectMinCd = excelRow.GetCell(3)?.ToString(),
                        InstrCd = excelRow.GetCell(4)?.ToString(),
                        CatTp = excelRow.GetCell(5)?.ToString(),
                        Add1 = excelRow.GetCell(6)?.ToString(),
                        Add2 = excelRow.GetCell(7)?.ToString(),
                        RegOff = excelRow.GetCell(8)?.ToString(),
                        PrnSth = excelRow.GetCell(9)?.ToString(),
                        OpnDt = ParseDateTime(excelRow.GetCell(10)),
                        TaxHday = excelRow.GetCell(11)?.ToString(),
                        Tel = excelRow.GetCell(12)?.ToString(),
                        Tlx = excelRow.GetCell(13)?.ToString(),
                        EMail = excelRow.GetCell(14)?.ToString(),
                        Prod = excelRow.GetCell(15)?.ToString(),
                        ProVol = excelRow.GetCell(16)?.ToString(),
                        Spnr = excelRow.GetCell(17)?.ToString(),
                        AthoCap = ParseDecimal(excelRow.GetCell(18)),
                        PaidCap = ParseDecimal(excelRow.GetCell(19)) ?? 0,
                        NoShrs = ParseDecimal(excelRow.GetCell(20)) ?? 0,
                        FcVal = ParseDecimal(excelRow.GetCell(21)) ?? 0,
                        Mlot = int.Parse(excelRow.GetCell(22)?.ToString() ?? "0"),
                        SbaseRt = ParseDecimal(excelRow.GetCell(23)) ?? 0,
                        FlotDtFm = ParseDateTime(excelRow.GetCell(24)),
                        FlotDtTo = ParseDateTime(excelRow.GetCell(25)),
                        BokClFdt = ParseDateTime(excelRow.GetCell(26)),
                        BokClTdt = ParseDateTime(excelRow.GetCell(27)),
                        Margin = ParseInt(excelRow.GetCell(28)),
                        AvgRt = ParseDecimal(excelRow.GetCell(29)),
                        RtUpdDt = ParseDateTime(excelRow.GetCell(30)),
                        Flag = excelRow.GetCell(31)?.ToString(),
                        Auditor = excelRow.GetCell(32)?.ToString(),
                        NsIcb = ParseDecimal(excelRow.GetCell(33)),
                        NsUnit = ParseDecimal(excelRow.GetCell(34)),
                        NsMutual = ParseDecimal(excelRow.GetCell(35)),
                        Pmargin = ParseInt(excelRow.GetCell(36)),
                        RissuDtFm = ParseDateTime(excelRow.GetCell(37)),
                        RissuDtTo = ParseDateTime(excelRow.GetCell(38)),
                        Premium = ParseDecimal(excelRow.GetCell(39)),
                        Cflag = excelRow.GetCell(40)?.ToString(),
                        MarFloat = ParseDecimal(excelRow.GetCell(41)),
                        MonTo = excelRow.GetCell(42)?.ToString(),
                        TradeMeth = excelRow.GetCell(43)?.ToString(),
                        CseInstrCd = excelRow.GetCell(44)?.ToString(),
                        IndxLst = ParseDecimal(excelRow.GetCell(45)),
                        BaseUpdDt = ParseDateTime(excelRow.GetCell(46)),
                        Cds = excelRow.GetCell(47)?.ToString(),
                        CtlRt = ParseDecimal(excelRow.GetCell(48)),
                        Net = excelRow.GetCell(49)?.ToString(),
                        Grp = excelRow.GetCell(50)?.ToString(),
                        MerchanBankId = excelRow.GetCell(51)?.ToString(),
                        Otc = excelRow.GetCell(52)?.ToString(),
                        IpoCutoffDt = ParseDateTime(excelRow.GetCell(53)),
                        TradePlatform = excelRow.GetCell(54)?.ToString(),
                        PeRatio = ParseDecimal(excelRow.GetCell(55)),
                        IsinCd = excelRow.GetCell(56)?.ToString(),
                        StartDt = ParseDateTime(excelRow.GetCell(57)),
                        Ldrn = ParseInt(excelRow.GetCell(58))
                    };

                    comp.SectMaj = null; // Explicitly set navigation property to null
                    records.Add(comp);

                    if (records.Count % batchSize == 0)
                    {
                        _context.Comps.AddRange(records);
                        _context.SaveChanges();
                        records.Clear();
                    }
                }

                if (records.Any())
                {
                    _context.Comps.AddRange(records);
                    _context.SaveChanges();
                }
            }
        }

        private DateTime? ParseDateTime(ICell cell)
        {
            if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) return null;
            if (DateTime.TryParse(cell.ToString(), out DateTime date)) return date;
            return null;
        }

        private decimal? ParseDecimal(ICell cell)
        {
            if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) return null;
            if (decimal.TryParse(cell.ToString(), out decimal dec)) return dec;
            return null;
        }

        private int? ParseInt(ICell cell)
        {
            if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) return null;
            if (int.TryParse(cell.ToString(), out int i)) return i;
            return null;
        }

        private string GetCellValueAsString(ICell cell)
        {
            if (cell == null)
            {
                return null;
            }

            switch (cell.CellType)
            {
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        return cell.DateCellValue.ToString();
                    }
                    else
                    {
                        return cell.NumericCellValue.ToString();
                    }
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Formula:
                    // Evaluate the formula to get the cached value
                    // This might require a FormulaEvaluator, but for simplicity, we'll try to get the cached value
                    return cell.ToString(); // Fallback for formula cells
                case CellType.Blank:
                    return null;
                default:
                    return cell.ToString();
            }
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

        [HttpGet("companies")]
        public async Task<IActionResult> GetCompanies([FromQuery] string? search = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = null, [FromQuery] string? sortDirection = null)
        {
            var query = _context.Comps.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c =>
                    (c.CompNm != null && c.CompNm.Contains(search)) ||
                    (c.IsinCd != null && c.IsinCd.Contains(search)) ||
                    (c.RegOff != null && c.RegOff.Contains(search)) ||
                    (c.InstrCd != null && c.InstrCd.Contains(search)) ||
                    (c.CatTp != null && c.CatTp.Contains(search)) ||
                    (c.Add1 != null && c.Add1.Contains(search)) ||
                    (c.Add2 != null && c.Add2.Contains(search)) ||
                    (c.Tel != null && c.Tel.Contains(search)) ||
                    (c.EMail != null && c.EMail.Contains(search)) ||
                    (c.Prod != null && c.Prod.Contains(search)) ||
                    (c.Spnr != null && c.Spnr.Contains(search)) ||
                    (c.Auditor != null && c.Auditor.Contains(search)) ||
                    (c.CseInstrCd != null && c.CseInstrCd.Contains(search)) ||
                    (c.MerchanBankId != null && c.MerchanBankId.Contains(search)) ||
                    (c.TradePlatform != null && c.TradePlatform.Contains(search))
                );
            }

            var totalCount = await query.CountAsync();
            if (!string.IsNullOrEmpty(sortBy))
            {
                // Apply sorting dynamically
                switch (sortBy.ToLower())
                {
                    case "compcd":
                        query = sortDirection?.ToLower() == "desc" ? query.OrderByDescending(c => c.CompCd) : query.OrderBy(c => c.CompCd);
                        break;
                    case "compnm":
                        query = sortDirection?.ToLower() == "desc" ? query.OrderByDescending(c => c.CompNm) : query.OrderBy(c => c.CompNm);
                        break;
                    case "athocap":
                        query = sortDirection?.ToLower() == "desc" ? query.OrderByDescending(c => c.AthoCap) : query.OrderBy(c => c.AthoCap);
                        break;
                    case "paidcap":
                        query = sortDirection?.ToLower() == "desc" ? query.OrderByDescending(c => c.PaidCap) : query.OrderBy(c => c.PaidCap);
                        break;
                    case "regoff":
                        query = sortDirection?.ToLower() == "desc" ? query.OrderByDescending(c => c.RegOff) : query.OrderBy(c => c.RegOff);
                        break;
                    case "noshrs":
                        query = sortDirection?.ToLower() == "desc" ? query.OrderByDescending(c => c.NoShrs) : query.OrderBy(c => c.NoShrs);
                        break;
                    case "instrcd":
                        query = sortDirection?.ToLower() == "desc" ? query.OrderByDescending(c => c.InstrCd) : query.OrderBy(c => c.InstrCd);
                        break;
                    case "startdt":
                        query = sortDirection?.ToLower() == "desc" ? query.OrderByDescending(c => c.StartDt) : query.OrderBy(c => c.StartDt);
                        break;
                    case "fcval":
                        query = sortDirection?.ToLower() == "desc" ? query.OrderByDescending(c => c.FcVal) : query.OrderBy(c => c.FcVal);
                        break;
                    default:
                        // Default sort if sortBy is not recognized
                        query = query.OrderBy(c => c.Id); // Or any default column
                        break;
                }
            }
            else
            {
                // Always apply a default order for consistent pagination
                query = query.OrderBy(c => c.Id);
            }

            var companies = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { Companies = companies, TotalCount = totalCount });
        }

        [HttpGet("companies/{compCd}")]
        public async Task<IActionResult> GetCompanyDetails(int compCd)
        {
            var company = await _context.Comps.FirstOrDefaultAsync(c => c.CompCd == compCd);
            if (company == null)
            {
                return NotFound();
            }
            return Ok(company);
        }

        [HttpGet("marprice/{compCd}")]
        public async Task<IActionResult> GetMarPriceData(int compCd)
        {
            // Define the cutoff date for filtering
            var cutoffDate = new DateTime(2016, 1, 1);

            var marPriceData = await _context.MarPrices
                .Where(mp => mp.CompCd == compCd && mp.TransDt >= cutoffDate) // Add the date filter
                .OrderBy(mp => mp.TransDt) // Order by transaction date for chart
                .Select(mp => new
                {
                    mp.TransDt,
                    mp.Open,
                    mp.High,
                    mp.Low,
                    mp.Close
                })
                .ToListAsync();

            if (!marPriceData.Any())
            {
                return NotFound($"No market price data found for Company Code: {compCd}");
            }

            return Ok(marPriceData);
        }

        [HttpPost("update-comp-cds")]
        public async Task<IActionResult> UpdateCompCds([FromBody] List<CompCdsUpdateData> updateDataList)
        {
            foreach (var data in updateDataList)
            {
                var comp = await _context.Comps.FirstOrDefaultAsync(c => c.CompCd == data.CompCd);
                if (comp != null)
                {
                    comp.IsinCd = data.IsinCd;
                    comp.StartDt = data.StartDt;
                    comp.Ldrn = data.Ldrn;
                    _context.Comps.Update(comp);
                }
            }
            await _context.SaveChangesAsync();
            return Ok("Comp table updated successfully.");
        }
    }
}