using Microsoft.AspNetCore.Mvc;
using StockMarket.Api.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UglyToad.PdfPig;
using System.Text;
using StockMarket.Api.Data;
using StockMarket.Api.Models;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace StockMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountingDataController : ControllerBase
    {
        private readonly GeminiService _geminiService;
        private readonly ApplicationDbContext _context;

        public AccountingDataController(GeminiService geminiService, ApplicationDbContext context)
        {
            _geminiService = geminiService;
            _context = context;
        }

        // Helper class for deserialization
        private class AnalysisResponse
        {
            public string statementType { get; set; }
            public List<FinancialReportEntry> entries { get; set; }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] int compCd, [FromForm] int fiscalYear)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (compCd == 0 || fiscalYear == 0)
            {
                return BadRequest("Company Code and Fiscal Year are required.");
            }

            var company = await _context.Comps.FirstOrDefaultAsync(c => c.CompCd == compCd);
            if (company == null)
            {
                return BadRequest("Invalid Company Code.");
            }

            var text = new StringBuilder();
            using (var pdf = PdfDocument.Open(file.OpenReadStream()))
            {
                foreach (var page in pdf.GetPages())
                {
                    text.AppendLine(page.Text);
                }
            }

            var jsonResult = await _geminiService.AnalyzeFinancialData(text.ToString());

            try
            {
                var firstBrace = jsonResult.IndexOf('{');
                var lastBrace = jsonResult.LastIndexOf('}');
                if (firstBrace == -1 || lastBrace == -1)
                {
                    return BadRequest("Invalid JSON format from analysis service.");
                }
                var cleanedJson = jsonResult.Substring(firstBrace, lastBrace - firstBrace + 1);

                var analysis = JsonSerializer.Deserialize<AnalysisResponse>(cleanedJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (analysis == null)
                {
                    return BadRequest("Failed to deserialize the analysis result.");
                }

                var report = new FinancialReport
                {
                    CompId = company.Id, // Use the actual primary key
                    Year = fiscalYear,
                    StatementType = analysis.statementType,
                    Entries = analysis.entries
                };

                _context.FinancialReports.Add(report);
                await _context.SaveChangesAsync();

                return Ok(report);
            }
            catch (JsonException ex)
            {
                // Log the exception and the problematic JSON
                // For now, returning a BadRequest with details
                return BadRequest($"Failed to parse analysis result as JSON. Error: {ex.Message}. JSON: {jsonResult}");
            }
        }

        [HttpGet("{compCd}")]
        public async Task<IActionResult> GetFinancialReports(int compCd)
        {
            var company = await _context.Comps.FirstOrDefaultAsync(c => c.CompCd == compCd);
            if (company == null)
            {
                return NotFound("Company not found.");
            }

            var reports = await _context.FinancialReports
                .Include(r => r.Entries)
                .Where(r => r.CompId == company.Id)
                .OrderByDescending(r => r.Year)
                .ToListAsync();

            if (reports == null || !reports.Any())
            {
                return NotFound("No financial reports found for this company.");
            }

            return Ok(reports);
        }
    }
}
