using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using issue_indexer_server.Models;
using issue_indexer_server.Data;

namespace issue_indexer_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketHistoriesController : ControllerBase
    {
        private readonly IssueIndexerContext _context;

        public TicketHistoriesController(IssueIndexerContext context)
        {
            _context = context;
        }

        // GET: api/TicketHistories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketHistory>>> GetTicketHistory(uint? ticketId)
        {
            List<TicketHistory> ticketHistory = null;
            if (ticketId.HasValue)
            {
                bool ticketExists = await _context.Tickets.AnyAsync(t => t.Id == ticketId);
                if (!ticketExists) return NotFound();
                ticketHistory = await (from h in _context.TicketHistory
                                  where h.TicketId == ticketId
                                  select h).ToListAsync();
            }
            if (ticketHistory != null) return ticketHistory;
            //else return NotFound();
            return await _context.TicketHistory.ToListAsync();
        }

        // GET: api/TicketHistories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TicketHistory>> GetTicketHistory(uint id)
        {
            var ticketHistory = await _context.TicketHistory.FindAsync(id);

            if (ticketHistory == null)
            {
                return NotFound();
            }

            return ticketHistory;
        }

        // PUT: api/TicketHistories/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTicketHistory(uint id, TicketHistory ticketHistory)
        {
            if (id != ticketHistory.Id)
            {
                return BadRequest();
            }

            _context.Entry(ticketHistory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketHistoryExists(id))
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

        // POST: api/TicketHistories
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<TicketHistory>> PostTicketHistory(TicketHistory ticketHistory)
        {

            _context.TicketHistory.Add(ticketHistory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTicketHistory", new { id = ticketHistory.Id }, ticketHistory);
        }

        // DELETE: api/TicketHistories/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TicketHistory>> DeleteTicketHistory(uint id)
        {
            var ticketHistory = await _context.TicketHistory.FindAsync(id);
            if (ticketHistory == null)
            {
                return NotFound();
            }

            _context.TicketHistory.Remove(ticketHistory);
            await _context.SaveChangesAsync();

            return ticketHistory;
        }

        private bool TicketHistoryExists(uint id)
        {
            return _context.TicketHistory.Any(e => e.Id == id);
        }
    }
}
