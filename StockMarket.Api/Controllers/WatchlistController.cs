
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockMarket.Api.Data;
using StockMarket.Api.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StockMarket.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WatchlistController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public WatchlistController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Watchlist
        [HttpGet]
        public async Task<IActionResult> GetWatchlist([FromQuery] DateTime? date)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var query = _context.WatchLists.Where(w => w.UserId == userId);

            if (date.HasValue)
            {
                var watchlist = await query
                    .Join(_context.Comps, w => w.CompId, c => c.Id, (w, c) => new { Watchlist = w, Comp = c })
                    .GroupJoin(_context.MarPrices.Where(mp => mp.TransDt <= date.Value), wc => wc.Comp.CompCd, mp => mp.CompCd, (wc, mp) => new { wc.Comp, LatestPrice = mp.OrderByDescending(m => m.TransDt).FirstOrDefault() })
                    .Select(result => new
                    {
                        result.Comp.Id,
                        result.Comp.CompCd,
                        result.Comp.CompNm,
                        result.LatestPrice
                    })
                    .ToListAsync();
                return Ok(watchlist);
            }
            else
            {
                var watchlist = await query
                    .Join(_context.Comps, w => w.CompId, c => c.Id, (w, c) => new { Watchlist = w, Comp = c })
                    .GroupJoin(_context.MarPrices, wc => wc.Comp.CompCd, mp => mp.CompCd, (wc, mp) => new { wc.Comp, LatestPrice = mp.OrderByDescending(m => m.TransDt).FirstOrDefault() })
                    .Select(result => new
                    {
                        result.Comp.Id,
                        result.Comp.CompCd,
                        result.Comp.CompNm,
                        result.LatestPrice
                    })
                    .ToListAsync();
                return Ok(watchlist);
            }
        }

        // POST: api/Watchlist/5
        [HttpPost("{compId}")]
        public async Task<IActionResult> AddToWatchlist(int compId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var watchlistExists = await _context.WatchLists.AnyAsync(w => w.UserId == userId && w.CompId == compId);

            if (watchlistExists)
            {
                return BadRequest("Company already in watchlist.");
            }

            var watchlist = new WatchList
            {
                UserId = userId,
                CompId = compId
            };

            _context.WatchLists.Add(watchlist);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Watchlist/5
        [HttpDelete("{compId}")]
        public async Task<IActionResult> RemoveFromWatchlist(int compId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var watchlistItem = await _context.WatchLists.FirstOrDefaultAsync(w => w.UserId == userId && w.CompId == compId);

            if (watchlistItem == null)
            {
                return NotFound();
            }

            _context.WatchLists.Remove(watchlistItem);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
