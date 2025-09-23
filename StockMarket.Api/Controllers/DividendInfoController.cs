using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockMarket.Api.Data;
using StockMarket.Api.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DividendInfoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DividendInfoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DividendInfo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetDividendInfo(
            [FromQuery] int? compCd, 
            [FromQuery] string? divType, 
            [FromQuery] DateTime? bokClFdt)
        {
            var query = _context.DividendInfos.AsQueryable();

            if (compCd.HasValue)
            {
                query = query.Where(d => d.CompCd == compCd.Value);
            }

            if (!string.IsNullOrEmpty(divType))
            {
                query = query.Where(d => d.DivType == divType);
            }

            if (bokClFdt.HasValue)
            {
                query = query.Where(d => d.BokClFdt == bokClFdt.Value);
            }

            // Join with Comp table to get company name
            var result = await query
                .GroupJoin(_context.Comps,
                    dividend => dividend.CompCd,
                    comp => comp.CompCd,
                    (dividend, compGroup) => new { dividend, compGroup })
                .SelectMany(
                    x => x.compGroup.DefaultIfEmpty(),
                    (x, comp) => new
                    {
                        x.dividend.Id,
                        x.dividend.CompCd,
                        CompNm = comp != null ? comp.CompNm : null, // Include company name
                        x.dividend.AgmDt,
                        x.dividend.Fyear,
                        x.dividend.Cfyear,
                        x.dividend.DivType,
                        x.dividend.Rate,
                        x.dividend.Ratio1,
                        x.dividend.Ratio2,
                        x.dividend.Premium,
                        x.dividend.PaymentDt,
                        x.dividend.BokClFdt,
                        x.dividend.BokClTdt,
                        x.dividend.OpName,
                        x.dividend.Discount,
                        x.dividend.Remarks,
                        x.dividend.BsCompCd
                    })
                .ToListAsync();

            return result;
        }

        // GET: api/DividendInfo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DividendInfo>> GetDividendInfo(int id)
        {
            var dividendInfo = await _context.DividendInfos.FindAsync(id);

            if (dividendInfo == null)
            {
                return NotFound();
            }

            return dividendInfo;
        }

        // POST: api/DividendInfo
        [HttpPost]
        public async Task<ActionResult<DividendInfo>> PostDividendInfo(DividendInfo dividendInfo)
        {
            _context.DividendInfos.Add(dividendInfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDividendInfo", new { id = dividendInfo.Id }, dividendInfo);
        }

        // PUT: api/DividendInfo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDividendInfo(int id, DividendInfo dividendInfo)
        {
            if (id != dividendInfo.Id)
            {
                return BadRequest();
            }

            _context.Entry(dividendInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DividendInfoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/DividendInfo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDividendInfo(int id)
        {
            var dividendInfo = await _context.DividendInfos.FindAsync(id);
            if (dividendInfo == null)
            {
                return NotFound();
            }

            _context.DividendInfos.Remove(dividendInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("company/{compCd}")]
        public async Task<ActionResult<IEnumerable<DividendInfo>>> GetDividendInfoByCompCd(int compCd)
        {
            var dividendInfo = await _context.DividendInfos.Where(d => d.CompCd == compCd).ToListAsync();

            if (dividendInfo == null)
            {
                return NotFound();
            }

            return dividendInfo;
        }

        private bool DividendInfoExists(int id)
        {
            return _context.DividendInfos.Any(e => e.Id == id);
        }
    }
}
