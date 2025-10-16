
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockMarket.Api.Data;
using StockMarket.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace StockMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialReportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FinancialReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/FinancialReport/STANDARINS/2024
        [HttpGet("{instrCd}/{year}")]
        public async Task<ActionResult<FinancialReport>> GetFinancialReport(string instrCd, int year)
        {
            var comp = await _context.Comps.FirstOrDefaultAsync(c => c.InstrCd == instrCd);
            if (comp == null)
            {
                return NotFound("Company not found.");
            }

            var financialReport = await _context.FinancialReports
                .Include(fr => fr.Entries)
                .FirstOrDefaultAsync(fr => fr.CompId == comp.Id && fr.Year == year);

            if (financialReport == null)
            {
                return NotFound("Financial report not found.");
            }

            return financialReport;
        }

        // POST: api/FinancialReport/import
        [HttpPost("import")]
        public async Task<IActionResult> ImportFinancialData([FromBody] JsonElement data)
        {
            var companies = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<JsonElement>>>>(data.GetRawText());

            if (companies == null)
            {
                return BadRequest("Invalid JSON format.");
            }

            foreach (var company in companies)
            {
                var comp = await _context.Comps.FirstOrDefaultAsync(c => c.InstrCd == company.Key);
                if (comp == null)
                {
                    // Optionally create the company if it doesn't exist
                    // comp = new Comp { InstrCd = company.Key, CompNm = company.Key };
                    // _context.Comps.Add(comp);
                    // await _context.SaveChangesAsync();
                    continue; // or handle as an error
                }

                foreach (var metric in company.Value)
                {
                    foreach (var entry in metric.Value)
                    {
                        var companyData = JsonSerializer.Deserialize<CompanyData>(entry.GetRawText());
                        if (companyData == null) continue;

                        var year = companyData.MetaDate.Year;

                        var financialReport = await _context.FinancialReports
                            .FirstOrDefaultAsync(fr => fr.CompId == comp.Id && fr.Year == year);

                        if (financialReport == null)
                        {
                            financialReport = new FinancialReport
                            {
                                CompId = comp.Id,
                                Year = year,
                                StatementType = "Key Metrics"
                            };
                            _context.FinancialReports.Add(financialReport);
                            await _context.SaveChangesAsync();
                        }

                        var financialReportEntry = new FinancialReportEntry
                        {
                            FinancialReportId = financialReport.Id,
                            StandardAccountName = companyData.MetaKey,
                            OriginalAccountName = companyData.MetaKey,
                            Value = decimal.TryParse(companyData.MetaValue, out var val) ? val : 0
                        };
                        _context.FinancialReportEntries.Add(financialReportEntry);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return Ok("Data imported successfully.");
        }

        private class CompanyData
        {
            public required string Code { get; set; }
            public required string MetaKey { get; set; }
            public required string MetaValue { get; set; }
            public DateTime MetaDate { get; set; }
        }
    }
}
