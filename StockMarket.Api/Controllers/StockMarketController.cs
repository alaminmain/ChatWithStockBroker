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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StockMarket.Api.Controllers
{
    public class FundamentalDataItem
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("meta_key")]
        public string MetaKey { get; set; }

        [JsonPropertyName("meta_value")]
        public string MetaValue { get; set; }

        [JsonPropertyName("meta_date")]
        public string MetaDate { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class StockMarketController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StockMarketController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("import-fundamental-data")]
        public async Task<IActionResult> ImportFundamentalData()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "FundementalData.json");
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("FundementalData.json not found.");
            }

            var json = await System.IO.File.ReadAllTextAsync(filePath);
            var fundamentalData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<FundamentalDataItem>>>>(json);

            if (fundamentalData == null)
            {
                return BadRequest("Could not deserialize fundamental data.");
            }

            foreach (var companyData in fundamentalData)
            {
                var instrCd = companyData.Key;
                var comp = await _context.Comps.FirstOrDefaultAsync(c => c.InstrCd == instrCd);

                if (comp != null)
                {
                    UpdateCompanyFromFundamentalData(comp, companyData.Value);
                }
            }

            await _context.SaveChangesAsync();

            return Ok("Fundamental data imported successfully.");
        }

        private void UpdateCompanyFromFundamentalData(Comp comp, Dictionary<string, List<FundamentalDataItem>> data)
        {
            comp.AthoCap = GetDecimalValue(data, "authorized_capital");
            comp.PaidCap = GetDecimalValue(data, "paid_up_capital") ?? comp.PaidCap;
            comp.NoShrs = GetDecimalValue(data, "total_no_securities") ?? comp.NoShrs;
            comp.EMail = GetStringValue(data, "email");
            comp.Tel = GetStringValue(data, "phone_number");
            comp.Website = GetStringValue(data, "website");
            comp.ListingYear = GetIntValue(data, "listing_year");
            comp.LastAgmHeld = GetDateTimeValue(data, "last_agm_held");
            comp.EarningPerShare = GetDecimalValue(data, "earning_per_share");
            comp.NetAssetValPerShare = GetDecimalValue(data, "net_asset_val_per_share");
            comp.NocfPerShare = GetDecimalValue(data, "nocf_per_share");
            comp.SharePercentageDirector = GetDecimalValue(data, "share_percentage_director");
            comp.SharePercentageForeign = GetDecimalValue(data, "share_percentage_foreign");
            comp.SharePercentageGovt = GetDecimalValue(data, "share_percentage_govt");
            comp.SharePercentageInstitute = GetDecimalValue(data, "share_percentage_institute");
            comp.SharePercentagePublic = GetDecimalValue(data, "share_percentage_public");
            comp.YearEnd = GetDateTimeValue(data, "year_end");
            comp.OperationalStatus = GetStringValue(data, "operational_status");
            comp.Fax = GetStringValue(data, "fax_number");
        }

        private string GetStringValue(Dictionary<string, List<FundamentalDataItem>> data, string key)
        {
            if (data.TryGetValue(key, out var items) && items.Any())
            {
                return items.OrderByDescending(i => i.MetaDate).First().MetaValue;
            }
            return null;
        }

        private decimal? GetDecimalValue(Dictionary<string, List<FundamentalDataItem>> data, string key)
        {
            var stringValue = GetStringValue(data, key);
            if (decimal.TryParse(stringValue, out var result))
            {
                return result;
            }
            return null;
        }

        private int? GetIntValue(Dictionary<string, List<FundamentalDataItem>> data, string key)
        {
            var stringValue = GetStringValue(data, key);
            if (int.TryParse(stringValue, out var result))
            {
                return result;
            }
            return null;
        }

        private DateTime? GetDateTimeValue(Dictionary<string, List<FundamentalDataItem>> data, string key)
        {
            var stringValue = GetStringValue(data, key);
            if (DateTime.TryParse(stringValue, out var result))
            {
                return result;
            }
            return null;
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
            else if (fileName.Contains("dividendinfo"))
            {
                BulkInsertDividendInfo(filePath);
                return Ok("Successfully processed dividend info file.");
            }
            else
            {
                return BadRequest("Unknown file type.");
            }
        }

        private void BulkInsertDividendInfo(string filePath)
        {
            var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                PrepareHeaderForMatch = args => args.Header.ToLower(),
                BadDataFound = null
            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<DividendInfoMap>();
                var dividendRecords = csv.GetRecords<DividendInfo>().ToList();

                // Get all existing CompCds from the Comp table
                var existingCompCds = _context.Comps.Select(c => c.CompCd).ToHashSet();

                // Filter dividend records to only include those with a matching CompCd
                var filteredDividendRecords = dividendRecords
                    .Where(dr => dr.CompCd.HasValue && existingCompCds.Contains(dr.CompCd.Value))
                    .ToList();

                DataTable dividendInfoDataTable = ConvertToDataTable(filteredDividendRecords);

                var connectionString = _context.Database.GetConnectionString();

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.BulkCopyTimeout = 300; // 5 minutes
                        bulkCopy.DestinationTableName = "DIVIDEND_INFO";

                        // Column Mappings
                        bulkCopy.ColumnMappings.Add("CompCd", "COMP_CD");
                        bulkCopy.ColumnMappings.Add("AgmDt", "AGM_DT");
                        bulkCopy.ColumnMappings.Add("Fyear", "FYEAR");
                        bulkCopy.ColumnMappings.Add("Cfyear", "CFYEAR");
                        bulkCopy.ColumnMappings.Add("DivType", "DIV_TYPE");
                        bulkCopy.ColumnMappings.Add("Rate", "RATE");
                        bulkCopy.ColumnMappings.Add("Ratio1", "RATIO1");
                        bulkCopy.ColumnMappings.Add("Ratio2", "RATIO2");
                        bulkCopy.ColumnMappings.Add("Premium", "PREMIUM");
                        bulkCopy.ColumnMappings.Add("PaymentDt", "PAYMENT_DT");
                        bulkCopy.ColumnMappings.Add("BokClFdt", "BOK_CL_FDT");
                        bulkCopy.ColumnMappings.Add("BokClTdt", "BOK_CL_TDT");
                        bulkCopy.ColumnMappings.Add("OpName", "OP_NAME");
                        bulkCopy.ColumnMappings.Add("Discount", "DISCOUNT");
                        bulkCopy.ColumnMappings.Add("Remarks", "REMARKS");
                        bulkCopy.ColumnMappings.Add("BsCompCd", "BS_COMP_CD");

                        bulkCopy.WriteToServer(dividendInfoDataTable);
                    }
                }
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
                        CompCd = ParseInt(excelRow.GetCell(0)) ?? 0,
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
                    (c.TradeMeth != null && c.TradeMeth.Contains(search)) || // Changed from CatTp
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

            // We need to handle sorting on the joined Sector Name as well
            var projectedQuery = query
                .GroupJoin(_context.SectMajs,
                    comp => comp.SectMajCd,
                    sect => sect.SectMajCd,
                    (comp, sectGroup) => new { comp, sectGroup })
                .SelectMany(
                    x => x.sectGroup.DefaultIfEmpty(),
                    (x, sect) => new {
                        Company = x.comp,
                        SectorName = sect != null ? sect.SectMajNm : null
                    });

            if (!string.IsNullOrEmpty(sortBy))
            {
                // Apply sorting dynamically
                switch (sortBy.ToLower())
                {
                    case "compcd":
                        projectedQuery = sortDirection?.ToLower() == "desc" ? projectedQuery.OrderByDescending(c => c.Company.CompCd) : projectedQuery.OrderBy(c => c.Company.CompCd);
                        break;
                    case "compnm":
                        projectedQuery = sortDirection?.ToLower() == "desc" ? projectedQuery.OrderByDescending(c => c.Company.CompNm) : projectedQuery.OrderBy(c => c.Company.CompNm);
                        break;
                    case "sectorname": // New sort option
                        projectedQuery = sortDirection?.ToLower() == "desc" ? projectedQuery.OrderByDescending(c => c.SectorName) : projectedQuery.OrderBy(c => c.SectorName);
                        break;
                    case "category": // New sort option
                        projectedQuery = sortDirection?.ToLower() == "desc" ? projectedQuery.OrderByDescending(c => c.Company.TradeMeth) : projectedQuery.OrderBy(c => c.Company.TradeMeth);
                        break;
                    case "instrcd":
                        projectedQuery = sortDirection?.ToLower() == "desc" ? projectedQuery.OrderByDescending(c => c.Company.InstrCd) : projectedQuery.OrderBy(c => c.Company.InstrCd);
                        break;
                    default:
                        projectedQuery = projectedQuery.OrderBy(c => c.Company.Id);
                        break;
                }
            }
            else
            {
                projectedQuery = projectedQuery.OrderBy(c => c.Company.Id);
            }

            var companiesWithSectors = await projectedQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Final projection to a simpler DTO
            var result = companiesWithSectors.Select(cs => new {
                cs.Company.Id,
                cs.Company.CompCd,
                cs.Company.CompNm,
                cs.Company.InstrCd,
                Category = cs.Company.TradeMeth,
                cs.SectorName
            }).ToList();

            return Ok(new { Companies = result, TotalCount = totalCount });
        }

        [HttpGet("companies/{compCd}")]
        public async Task<IActionResult> GetCompanyDetails(int compCd)
        {
            var company = await _context.Comps.FirstOrDefaultAsync(c => c.CompCd == compCd);
            if (company == null)
            {
                return NotFound();
            }

            var sector = await _context.SectMajs.FirstOrDefaultAsync(s => s.SectMajCd == company.SectMajCd);

            // Create a dynamic object or a DTO to return combined information
            var companyDetails = new {
                // Copy properties from company
                company.Id,
                company.CompCd,
                company.CompNm,
                company.SectMajCd,
                company.SectMinCd,
                company.InstrCd,
                company.Add1,
                company.Add2,
                company.RegOff,
                company.PrnSth,
                company.OpnDt,
                company.TaxHday,
                company.Tel,
                company.Tlx,
                company.EMail,
                company.Prod,
                company.ProVol,
                company.Spnr,
                company.AthoCap,
                company.PaidCap,
                company.NoShrs,
                company.FcVal,
                company.Mlot,
                company.SbaseRt,
                company.FlotDtFm,
                company.FlotDtTo,
                company.BokClFdt,
                company.BokClTdt,
                company.Margin,
                company.AvgRt,
                company.RtUpdDt,
                company.Flag,
                company.Auditor,
                company.NsIcb,
                company.NsUnit,
                company.NsMutual,
                company.Pmargin,
                company.RissuDtFm,
                company.RissuDtTo,
                company.Premium,
                company.Cflag,
                company.MarFloat,
                company.MonTo,
                company.CseInstrCd,
                company.IndxLst,
                company.BaseUpdDt,
                company.Cds,
                company.CtlRt,
                company.Net,
                company.Grp,
                company.MerchanBankId,
                company.Otc,
                company.IpoCutoffDt,
                company.TradePlatform,
                company.PeRatio,
                company.IsinCd,
                company.StartDt,
                company.Ldrn,
                company.ListingYear,
                company.LastAgmHeld,
                company.EarningPerShare,
                company.NetAssetValPerShare,
                company.NocfPerShare,
                company.SharePercentageDirector,
                company.SharePercentageForeign,
                company.SharePercentageGovt,
                company.SharePercentageInstitute,
                company.SharePercentagePublic,
                company.YearEnd,
                company.OperationalStatus,
                company.Fax,
                company.Website,

                // Use TradeMeth for Category
                Category = company.TradeMeth,
                // Add SectorName
                SectorName = sector?.SectMajNm
            };

            return Ok(companyDetails);
        }

        [HttpGet("heatmap-data")]
        public async Task<IActionResult> GetHeatmapData()
        {
            var latestDate = await _context.MarPrices
                .OrderByDescending(mp => mp.TransDt)
                .Select(mp => mp.TransDt)
                .FirstOrDefaultAsync();

            if (latestDate == default) return NotFound();

            var latestPrices = await _context.MarPrices
                .Where(mp => mp.TransDt == latestDate)
                .ToListAsync();

            var instrCds = latestPrices.Select(lp => lp.InstCd).ToList();

            var compsData = await _context.Comps
                .Where(c => instrCds.Contains(c.InstrCd))
                .GroupJoin(_context.SectMajs,
                    comp => comp.SectMajCd,
                    sect => sect.SectMajCd,
                    (comp, sectGroup) => new { comp, sectGroup })
                .SelectMany(
                    x => x.sectGroup.DefaultIfEmpty(),
                    (x, sect) => new {
                        x.comp.InstrCd,
                        SectorName = sect != null ? sect.SectMajNm : "Unclassified"
                    })
                .ToDictionaryAsync(data => data.InstrCd);

            var heatmapData = latestPrices
                .Where(p => p.Vol.HasValue && p.Vol > 0) // Only include stocks that traded
                .Select(p => {
                    decimal? yesterdayClose = p.Close - p.Chg;
                    decimal? changePercent = (yesterdayClose.HasValue && yesterdayClose != 0 && p.Chg.HasValue)
                        ? (p.Chg.Value / yesterdayClose.Value) * 100
                        : 0;

                    compsData.TryGetValue(p.InstCd, out var compInfo);

                    return new
                    {
                        Symbol = p.InstCd,
                        Sector = compInfo?.SectorName ?? "Unclassified",
                        Volume = p.Vol.Value,
                        ChangePercent = changePercent ?? 0
                    };
                })
                .GroupBy(p => p.Sector)
                .Select(g => new
                {
                    Sector = g.Key,
                    Stocks = g.ToList()
                })
                .ToList();

            return Ok(heatmapData);
        }

        [HttpGet("marprice/{compCd}")]
        public async Task<IActionResult> GetMarPriceData(int? compCd, [FromQuery] string period = "1y")
        {
            if (!compCd.HasValue)
            {
                return BadRequest("Company code cannot be null.");
            }

            var query = _context.MarPrices.Where(mp => mp.CompCd == compCd.Value);

            DateTime? cutoffDate = null;
            switch (period.ToLower())
            {
                case "2y":
                    cutoffDate = DateTime.Now.AddYears(-2);
                    break;
                case "5y":
                    cutoffDate = DateTime.Now.AddYears(-5);
                    break;
                case "all":
                    break;
                case "1y":
                default:
                    cutoffDate = DateTime.Now.AddYears(-1);
                    break;
            }

            if(cutoffDate.HasValue)
            {
                query = query.Where(mp => mp.TransDt >= cutoffDate.Value);
            }

            var marPriceData = await query
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

        [HttpGet("stocks/latest")]
        public async Task<IActionResult> GetLatestStockPrices()
        {
            var latestDate = await _context.MarPrices
                .OrderByDescending(mp => mp.TransDt)
                .Select(mp => mp.TransDt)
                .FirstOrDefaultAsync();

            if (latestDate == default)
            {
                return Ok(new List<object>());
            }

            var latestPrices = await _context.MarPrices
                .Where(mp => mp.TransDt == latestDate)
                .ToListAsync();

            var instrCds = latestPrices.Select(lp => lp.InstCd).ToList();

            var compsData = await _context.Comps
                .Where(c => instrCds.Contains(c.InstrCd))
                .GroupJoin(_context.SectMajs,
                    comp => comp.SectMajCd,
                    sect => sect.SectMajCd,
                    (comp, sectGroup) => new { comp, sectGroup })
                .SelectMany(
                    x => x.sectGroup.DefaultIfEmpty(),
                    (x, sect) => new {
                        x.comp.InstrCd,
                        x.comp.CompCd,
                        Category = x.comp.TradeMeth,
                        SectorName = sect != null ? sect.SectMajNm : null
                    })
                .ToDictionaryAsync(data => data.InstrCd);

            var result = latestPrices.Select(p => {
                decimal? yesterdayClose = p.Close - p.Chg;
                decimal? changePercent = (yesterdayClose.HasValue && yesterdayClose != 0 && p.Chg.HasValue)
                    ? Math.Round((p.Chg.Value / yesterdayClose.Value) * 100, 2)
                    : 0;
                
                compsData.TryGetValue(p.InstCd, out var compInfo);

                return new
                {
                    CompCd = compInfo?.CompCd,
                    InstrCd = p.InstCd,
                    Ltp = p.Close,
                    Open = p.Open,
                    High = p.High,
                    Low = p.Low,
                    Close = p.Close,
                    Chg = p.Chg,
                    Trade = (decimal?)null, // No data for Trade
                    Value = p.Val,
                    Volume = p.Vol,
                    ChangePercent = changePercent,
                    Category = compInfo?.Category,
                    SectorName = compInfo?.SectorName
                };
            }).ToList();

            return Ok(result);
        }

        [HttpGet("market-leaders")]
        public async Task<IActionResult> GetMarketLeaders()
        {
            var latestDate = await _context.MarPrices
                .OrderByDescending(mp => mp.TransDt)
                .Select(mp => mp.TransDt)
                .FirstOrDefaultAsync();

            if (latestDate == default)
            {
                return NotFound("No market data available.");
            }

            var latestPrices = await _context.MarPrices
                .Where(mp => mp.TransDt == latestDate)
                .ToListAsync();

            var leaders = latestPrices.Select(p => {
                decimal? yesterdayClose = p.Close - p.Chg;
                decimal? changePercent = (yesterdayClose.HasValue && yesterdayClose != 0 && p.Chg.HasValue)
                    ? Math.Round((p.Chg.Value / yesterdayClose.Value) * 100, 2)
                    : 0;
                return new
                {
                    p.InstCd,
                    p.Chg,
                    p.Close,
                    p.Vol,
                    p.Val,
                    ChangePercent = changePercent
                };
            }).ToList();


            var topGainers = leaders.Where(p => p.Chg > 0).OrderByDescending(p => p.ChangePercent).Take(10).ToList();
            var topLosers = leaders.Where(p => p.Chg < 0).OrderBy(p => p.ChangePercent).Take(10).ToList();
            var topVolume = leaders.OrderByDescending(p => p.Vol).Take(10).ToList();
            var topValue = leaders.OrderByDescending(p => p.Val).Take(10).ToList();

            return Ok(new
            {
                topGainers,
                topLosers,
                topVolume,
                topValue
            });
        }

        [HttpPost("update-marprice-from-dse")]
        public async Task<IActionResult> UpdateMarPriceFromDse()
        {
            var url = "https://www.dsebd.org/latest_share_price_scroll_by_ltp.php";
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36");
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(html);

            var table = htmlDocument.DocumentNode.SelectSingleNode("//table[contains(@class, 'shares-table')]");

            if (table == null)
            {
                return StatusCode(500, "Could not find the market data table on the website.");
            }

            var marPriceRecords = new List<MarPrice>();
            var rows = table.SelectNodes(".//tr");

            if (rows == null)
            {
                return StatusCode(500, "Could not find any rows in the market data table.");
            }

            // Skip header row
            foreach (var row in rows.Skip(1))
            {
                var cells = row.SelectNodes(".//td");
                if (cells == null || cells.Count < 11) continue; // Ensure enough cells

                var instCd = cells[1].InnerText.Trim();

                var comp = await _context.Comps.FirstOrDefaultAsync(c => c.InstrCd == instCd);

                if (comp == null)
                {
                    Console.WriteLine($"Company not found for InstrCd: {instCd}");
                    continue;
                }

                var valStr = cells[9].InnerText.Trim();
                var valDecimal = ParseDecimalFromString(valStr);
                var finalVal = valDecimal.HasValue ? valDecimal.Value * 1000000m : (decimal?)null;

                var marPrice = new MarPrice
                {
                    TransDt = DateTime.Today,
                    InstCd = instCd,
                    CompCd = comp.CompCd,
                    Open = ParseDecimalFromString(cells[2].InnerText.Trim()), // LTP as Open
                    High = ParseDecimalFromString(cells[3].InnerText.Trim()),
                    Low = ParseDecimalFromString(cells[4].InnerText.Trim()),
                    Close = ParseDecimalFromString(cells[2].InnerText.Trim()), // LTP as Close
                    Chg = ParseDecimalFromString(cells[7].InnerText.Trim()),
                    Vol = ParseDecimalFromString(cells[10].InnerText.Trim()),
                    Val = finalVal,
                };
                marPriceRecords.Add(marPrice);
            }

            if (marPriceRecords.Any())
            {
                await BulkInsertMarPriceDirect(marPriceRecords);
            }

            return Ok($"{marPriceRecords.Count} market price records updated successfully from DSE.");
        }

        private async Task BulkInsertMarPriceDirect(List<MarPrice> marPriceRecords)
        {
            // --- Start of new idempotency logic (Revised) ---
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
            // --- End of new idempotency logic (Revised) ---

            if (!newRecordsToInsert.Any())
            {
                return; // All records already exist or no new records to insert
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

                    await bulkCopy.WriteToServerAsync(marPriceDataTable);
                }
            }
        }

        [HttpGet("market-summary")]
        public async Task<IActionResult> GetMarketSummary()
        {
            // Find the most recent date in MarPrices
            var latestDate = await _context.MarPrices
                .OrderByDescending(mp => mp.TransDt)
                .Select(mp => mp.TransDt)
                .FirstOrDefaultAsync();

            if (latestDate == default)
            {
                return NotFound("No market price data found.");
            }

            // Get all records for the latest date
            var latestPrices = await _context.MarPrices
                .Where(mp => mp.TransDt == latestDate)
                .ToListAsync();

            if (!latestPrices.Any())
            {
                return NotFound("No prices found for the latest date.");
            }

            var totalInstruments = latestPrices.Count;
            var gainers = latestPrices.Count(mp => mp.Chg.HasValue && mp.Chg > 0);
            var losers = latestPrices.Count(mp => mp.Chg.HasValue && mp.Chg < 0);
            var unchanged = latestPrices.Count(mp => !mp.Chg.HasValue || mp.Chg == 0);

            if (totalInstruments == 0)
            {
                return Ok(new
                {
                    Gainers = new { Count = 0, Percentage = 0 },
                    Losers = new { Count = 0, Percentage = 0 },
                    Unchanged = new { Count = 0, Percentage = 0 }
                });
            }

            var summary = new
            {
                Gainers = new { Count = gainers, Percentage = Math.Round((double)gainers / totalInstruments * 100, 2) },
                Losers = new { Count = losers, Percentage = Math.Round((double)losers / totalInstruments * 100, 2) },
                Unchanged = new { Count = unchanged, Percentage = Math.Round((double)unchanged / totalInstruments * 100, 2) }
            };

            return Ok(summary);
        }

        [HttpPost("import-dividend-info-from-path")]
        public IActionResult ImportDividendInfoFromPath()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "DividendInfo.csv");
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("DividendInfo.csv not found.");
            }

            BulkInsertDividendInfo(filePath);
            return Ok("Successfully processed dividend info file.");
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

    }
}